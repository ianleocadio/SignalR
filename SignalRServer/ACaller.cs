using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SignalRServer
{
    public abstract class ACaller
    {

        protected abstract string Event { get; }

        public IClientProxy Caller;
        public bool Alive;

        public ACaller(IClientProxy caller, bool alive)
        {
            Caller = caller;
            Alive = alive;
        }


        public virtual Task Execute(params object[] args)
        {
            Console.WriteLine("Calling " + Event + " " + args);
            return Caller.SendAsync(Event, args);
        }
    }
}
