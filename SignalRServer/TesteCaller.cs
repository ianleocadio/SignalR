using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SignalRServer
{
    public class TesteCaller : ACaller
    {
        protected override string Event { get => "Imprime"; }

        public string Unidade;

        public TesteCaller(string unidade, IClientProxy caller, bool alive) : base(caller, alive)
        {
            Unidade = unidade;
        }

        public override Task Execute(params object[] args)
        {
            Console.WriteLine(Program.GetTime() + "[TesteCaller.Execute] Calling " + Event + " on " + Unidade + " with: " + args[0]?.ToString());
            return Caller.SendAsync(Event, args[0]?.ToString());
        }
    }
}
