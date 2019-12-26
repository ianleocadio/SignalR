using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace SignalRLibrary
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class EventAttribute : Attribute
    {
        public string EndPoint { get; }
        /// <summary>
        /// Nome do método a ser executa em caso de sucesso
        /// </summary>
        public string ResponseHandlerMethodName { get; set; }

        public EventAttribute(string endPoint)
        {
            EndPoint = endPoint ?? throw new ArgumentNullException(nameof(endPoint));
        }

        public bool HasReponseHandler() 
            =>  (!string.IsNullOrEmpty(this.ResponseHandlerMethodName) 
            && !string.IsNullOrWhiteSpace(this.ResponseHandlerMethodName));

    }
}
