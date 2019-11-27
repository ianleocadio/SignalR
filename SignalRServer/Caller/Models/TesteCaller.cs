using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace SignalRServer.Caller.Models
{
    public class TesteCaller : ACaller
    {
        protected override string Event { get => "Imprime"; }

        public string Unidade;
        

        public TesteCaller(string unidade, string userAuthentication, IClientProxy caller, bool alive) 
            : base(userAuthentication, caller, alive)
        {
            Unidade = unidade;
        }

        public override async Task Execute(params object[] args)
        {
            Console.WriteLine(Program.GetTime() + "[TesteCaller.Execute] Calling " + Event + " on " + Unidade + " with: " + args[0]?.ToString());
            await Caller.SendAsync(Event, args[0]?.ToString());
        }
    }
}
