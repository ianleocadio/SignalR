using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using SignalRClient.Connections;
using SignalRLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace SignalRClient
{
    public class CommandDispatcher
    {

        private readonly ILogger<CommandDispatcher> _logger;

        private const BindingFlags DispatchBindingFlags = BindingFlags.IgnoreCase
                                                        | BindingFlags.IgnoreReturn
                                                        | BindingFlags.Instance
                                                        | BindingFlags.Public
                                                        | BindingFlags.NonPublic
                                                        | BindingFlags.Static;

        private readonly HubConnection _hubConnection;
        private readonly IEnumerable<Plugin> _commands;

        public CommandDispatcher(ILogger<CommandDispatcher>  logger, ConnectionProvider connectionProvider, 
            PluginProvider pluginProvider)
        {
            _logger = logger;
            _hubConnection = connectionProvider?.Connection;
            _commands = pluginProvider?.Plugins;
        }

        //private static Action<object[]> CreateAction(object instance, MethodInfo methodInfo)
        //{
        //    ParameterExpression objectArrayParameter = Expression.Parameter(typeof(object[]), "args");

        //    ParameterInfo[] methodInfoParameters = methodInfo.GetParameters();
        //    var parameters = new Expression[methodInfoParameters.Length];
        //    for (var i = 0; i < methodInfoParameters.Length; ++i)
        //    {
        //        ConstantExpression index = Expression.Constant(i);
        //        Type methodInfoParameterType = methodInfoParameters[i].ParameterType;
        //        BinaryExpression objectArrayParameterAccessor = Expression.ArrayIndex(objectArrayParameter, index);
        //        UnaryExpression objectArrayParameterCast = Expression.Convert(objectArrayParameterAccessor, methodInfoParameterType);
        //        parameters[i] = objectArrayParameterCast;
        //    }

        //    MethodCallExpression call = Expression.Call(Expression.Constant(instance), methodInfo, parameters);
        //    Expression<Action<object[]>> lambda = Expression.Lambda<Action<object[]>>(call, objectArrayParameter);

        //    Action<object[]> compiled = lambda.Compile();
        //    return compiled;
        //}

        private static Func<object[], Task> CreateFunc(object instance, MethodInfo methodInfo)
        {
            ParameterExpression objectArrayParameter = Expression.Parameter(typeof(object[]), "args");

            ParameterInfo[] methodInfoParameters = methodInfo.GetParameters();
            var parameters = new Expression[methodInfoParameters.Length];
            for (var i = 0; i < methodInfoParameters.Length; ++i)
            {
                ConstantExpression index = Expression.Constant(i);
                Type methodInfoParameterType = methodInfoParameters[i].ParameterType;
                BinaryExpression objectArrayParameterAccessor = Expression.ArrayIndex(objectArrayParameter, index);
                UnaryExpression objectArrayParameterCast = Expression.Convert(objectArrayParameterAccessor, methodInfoParameterType);
                parameters[i] = objectArrayParameterCast;
            }

            MethodCallExpression call = Expression.Call(Expression.Constant(instance), methodInfo, parameters);
            Expression<Func<object[], Task>> lambda = Expression.Lambda<Func<object[], Task>>(call, objectArrayParameter);

            Func<object[], Task> compiled = lambda.Compile();
            return compiled;
        }


        private static bool HasEndPointNamesDefined(Type type)
        {
            List<EventAttribute> EventAttributes = new List<EventAttribute>();

            // Second on all methods
            MethodInfo[] methodInfos = type.GetMethods();
            foreach (MethodInfo methodInfo in methodInfos)
            {
                List<EventAttribute> methodEventAttributes = methodInfo.GetCustomAttributes<EventAttribute>().ToList();
                EventAttributes.AddRange(methodEventAttributes);
            }

            return EventAttributes.Count > 0 && EventAttributes.Any(x => !string.IsNullOrEmpty(x.EndPoint));
        }

        public void BindCommands()
        {
            IEnumerable<Plugin> commands = _commands.Where(x => HasEndPointNamesDefined(x.GetType()));

            foreach (Plugin command in commands)
            {

                // Second search on all methods
                MethodInfo[] methodInfos = command.GetType().GetMethods();
                foreach (MethodInfo methodInfo in methodInfos)
                {
                    List<EventAttribute> methodEventAttributes = methodInfo.GetCustomAttributes<EventAttribute>().ToList();
                    if (methodEventAttributes.Count <= 0) continue;

                    foreach (var Event in methodEventAttributes)
                    {
                        ParameterInfo[] arguments = methodInfo.GetParameters();
                        Type[] types = arguments.Select(x => x.ParameterType).ToArray();

                        //Action<object[]> action = CreateAction(command, methodInfo);
                        Func<object[], Task> func = CreateFunc(command, methodInfo);

                        Func<object[], Task> responseHandler = null;
                        MethodInfo responseHandlerMethodInfo = null;
                        if (Event.HasReponseHandler())
                        {
                            responseHandlerMethodInfo = command.GetType().GetMethod(Event.ResponseHandlerMethodName, DispatchBindingFlags);
                            var t = responseHandlerMethodInfo.GetParameters();
                            responseHandler = CreateFunc(command, responseHandlerMethodInfo);
                        }

                        On(Event.EndPoint, types, func, responseHandler, responseHandlerMethodInfo);
                    }
                }
            }
        }

        private IDisposable On(string methodName, Type[] parameterTypes, Func<object[], Task> handler,
            Func<object[], Task> responseHandler = null, MethodInfo responseHandlerMethodInfo = null)
        {
            return _hubConnection.On(methodName,
                                    parameterTypes,
                                    (valueParameters, state) =>
                                    {
                                        var currentHandler = (Func<object[], Task>)state;

                                        currentHandler(valueParameters)
                                        .ContinueWith((task) =>
                                        {
                                            if (responseHandler == null)
                                                return;

                                            if (task.IsCompletedSuccessfully)
                                            {
                                                ExecuteHandlerResponse(responseHandler, responseHandlerMethodInfo, valueParameters);
                                            }
                                            else if (task.IsFaulted)
                                            {
                                                ExecuteHandlerResponse(responseHandler, responseHandlerMethodInfo, valueParameters,
                                                    true, task?.Exception.InnerException);
                                            }

                                        });

                                        return Task.CompletedTask;
                                    },
                                    handler);
        }

        private void ExecuteHandlerResponse(Func<object[], Task> handler, MethodInfo info,
            object[] valueParameters, bool isFailHandler = false, Exception exception = null)
        {
            handler(InjectDependenciesHandler(valueParameters.ToList(), info.GetParameters(), isFailHandler, exception));
        }

        private object[] InjectDependenciesHandler(List<object> valueParameters, ParameterInfo[] methodParameters,
            bool isFailHandler, Exception exception)
        {
            IEnumerable<object> handlerParameters = new object[] { };

            if (methodParameters.Length <= 0)
                return handlerParameters.ToArray();

            if (methodParameters[0].ParameterType != typeof(HubConnection))
            {
                _logger.LogError("HubConnection deve ser o primeiro argumento para qualquer assinatura de método do ReponseHandler do EventAttribute");
                throw new Exception("HubConnection deve ser o primeiro argumento para qualquer assinatura de método do ReponseHandler do EventAttribute");
            }
            else if (methodParameters[1].ParameterType != typeof(Exception))
            {
                _logger.LogError("Exception deve ser o segundo argumento para qualquer assinatura de método do ReponseHandler do EventAttribute");
                throw new Exception("Exception deve ser o segundo argumento para qualquer assinatura de método do ReponseHandler do EventAttribute");
            }

            handlerParameters = handlerParameters.Append(_hubConnection);
            handlerParameters = handlerParameters.Append(exception);
            return handlerParameters.Concat(valueParameters).ToArray();
        }
    }
}
    