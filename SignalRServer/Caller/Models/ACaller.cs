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
        
        protected HubCallerContext Context { get; private set; }
        public IClientProxy Client { get; private set; }

        public string UserAuthentication => Context.UserIdentifier;
        public string ConnectionId => Context.ConnectionId;
        
        public bool Alive { get; set; }

        protected ACaller(HubCallerContext context, IClientProxy client, bool alive)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            Client = client ?? throw new ArgumentNullException(nameof(client));
            Alive = alive;
        }

        public virtual async Task Execute(params object[] args)
        {
            await Client.SendAsync(Event, args);
        }

        public virtual void CloseConnection()
        {
            Context.Abort();
        }
    }
}
