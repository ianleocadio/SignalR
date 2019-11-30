using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SignalRServer.Caller.Models
{
    public abstract class ACaller
    {
        public abstract string Event { get; }
        public string UserAuthentication { get; set; }
        public IClientProxy Caller { get; set; }
        public bool Alive { get; set; }

        protected ACaller(string userAuthentication, IClientProxy caller, bool alive)
        {
            UserAuthentication = userAuthentication ?? throw new ArgumentNullException(nameof(userAuthentication));
            Caller = caller ?? throw new ArgumentNullException(nameof(caller));
            Alive = alive;
        }

        public virtual async Task Execute(params object[] args)
        {
            await Caller.SendAsync(Event, args);
        }

    }
}
