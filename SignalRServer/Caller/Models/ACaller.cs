using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SignalRServer.Caller.Models
{
    public abstract class ACaller
    {

        protected abstract string Event { get; }

        public string UserAuthentication;
        public IClientProxy Caller;
        public bool Alive;

        protected ACaller(string userAuthentication, IClientProxy caller, bool alive)
        {
            UserAuthentication = userAuthentication ?? throw new ArgumentNullException(nameof(userAuthentication));
            Caller = caller ?? throw new ArgumentNullException(nameof(caller));
            Alive = alive;
        }

        public virtual async Task Execute(params object[] args)
        {
            Console.WriteLine("Calling " + Event + " " + args);
            await Caller.SendAsync(Event, args);
        }

    }
}
