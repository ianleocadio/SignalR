using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SignalRClient.Logging;
using System;
using System.Threading.Tasks;

namespace SignalRServer.Caller.Models
{
    public class ImprimeCaller : ACaller
    {
        public override string Event { get => "Imprime"; }
        public string Unidade;

        public ImprimeCaller(string unidade, string userAuthentication, IClientProxy caller, bool alive)
            : base(userAuthentication, caller, alive)
        {
            Unidade = unidade;
        }

        public override async Task Execute(params object[] args)
        {
            
            await Caller.SendAsync(Event, args[0]?.ToString());
        }
    }
}
