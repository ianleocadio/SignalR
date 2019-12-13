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

        private const string DispatchMethodName = "ExecuteAsync";
        private readonly HubConnection _hubConnection;
        private readonly IEnumerable<IPlugin> _commands;

        public CommandDispatcher(ILogger<CommandDispatcher>  logger, ConnectionProvider connectionProvider, 
            PluginProvider pluginProvider)
        {
            _logger = logger;
            _hubConnection = connectionProvider?.Connection;
            _commands = pluginProvider?.Plugins;
        }

        private static Action<object[]> CreateAction(object instance, MethodInfo methodInfo)
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
            Expression<Action<object[]>> lambda = Expression.Lambda<Action<object[]>>(call, objectArrayParameter);

            Action<object[]> compiled = lambda.Compile();
            return compiled;
        }

        private static IEnumerable<string> GetEndPointNames(MemberInfo memberInfo)
        {
            List<EndPointAttribute> endpointAttributes = memberInfo.GetCustomAttributes<EndPointAttribute>().ToList();
            return endpointAttributes.Select(x => x.Name);
        }

        private static bool HasEndPointNamesDefined(Type type)
        {
            // First search on class level
            List<EndPointAttribute> endpointAttributes = type.GetCustomAttributes<EndPointAttribute>().ToList();

            // Second on all methods
            MethodInfo[] methodInfos = type.GetMethods();
            foreach (MethodInfo methodInfo in methodInfos)
            {
                List<EndPointAttribute> methodEndPointAttributes = methodInfo.GetCustomAttributes<EndPointAttribute>().ToList();
                endpointAttributes.AddRange(methodEndPointAttributes);
            }

            return endpointAttributes.Count > 0 && endpointAttributes.Any(x => !string.IsNullOrEmpty(x.Name));
        }

        public void BindCommands()
        {
            IEnumerable<IPlugin> commands = _commands.Where(x => HasEndPointNamesDefined(x.GetType()));

            foreach (IPlugin command in commands)
            {
                var commandRegistered = false;

                // First search for default 'ExecuteAsync' method, for class level binding
                MethodInfo defaultMethodInfo = command.GetType().GetMethod(DispatchMethodName, DispatchBindingFlags);
                if (defaultMethodInfo != null)
                {
                    ParameterInfo[] arguments = defaultMethodInfo.GetParameters();
                    Type[] types = arguments.Select(x => x.ParameterType).ToArray();

                    foreach (string endPointName in GetEndPointNames(command.GetType()))
                    {
                        Action<object[]> action = CreateAction(command, defaultMethodInfo);

                        MethodInfo onMethod = GetType().GetMethod(nameof(On), DispatchBindingFlags);
                        if (onMethod != null)
                        {
                            onMethod.Invoke(this, new object[] { endPointName, types, action });
                            commandRegistered = true;
                        }
                    }
                }

                // Second search on all methods
                MethodInfo[] methodInfos = command.GetType().GetMethods();
                foreach (MethodInfo methodInfo in methodInfos)
                {
                    List<EndPointAttribute> methodEndPointAttributes = methodInfo.GetCustomAttributes<EndPointAttribute>().ToList();
                    if (methodEndPointAttributes.Count <= 0) continue;

                    foreach (string endPointName in GetEndPointNames(methodInfo))
                    {
                        ParameterInfo[] arguments = methodInfo.GetParameters();
                        Type[] types = arguments.Select(x => x.ParameterType).ToArray();

                        Action<object[]> action = CreateAction(command, methodInfo);

                        MethodInfo onMethod = GetType().GetMethod(nameof(On), DispatchBindingFlags);
                        if (onMethod != null)
                        {
                            onMethod.Invoke(this, new object[] { endPointName, types, action });
                            commandRegistered = true;
                        }
                    }
                }
            }
        }

        private IDisposable On(string methodName, Type[] parameterTypes, Action<object[]> handler)
        {
            return _hubConnection.On(methodName,
                                    parameterTypes,
                                    (parameters, state) =>
                                    {
                                        var currentHandler = (Action<object[]>)state;
                                        currentHandler(parameters);
                                        return Task.CompletedTask;
                                    },
                                    handler);
        }
    }
}
