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

        public ImprimeCaller(string unidade, HubCallerContext context, IClientProxy client, bool alive)
            : base(context, client, alive)
        {
            Unidade = unidade;
        }

        public override async Task Execute(params object[] args)
        {
            await Client.SendAsync(Event, args[0]?.ToString());
        }
    }
}
