using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace SignalRClient.Connections.Template
{
    public abstract class AbstractTemplateSetupConnection
    {

        private HubConnection Connection;

        protected AbstractTemplateSetupConnection(HubConnection connection)
        {
            Connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public virtual void SetupOnCloseConnection(HubConnection connection) { }
        public virtual void SetupOnReconnectConnection(HubConnection connection) { }
        public virtual void SetupOnReconnectingConnection(HubConnection connection) { }
        public virtual void SetupConnectionEvents(HubConnection connection) { }

        public void SetupConnection()
        {
            SetupOnCloseConnection(Connection);
            SetupOnReconnectConnection(Connection);
            SetupOnReconnectingConnection(Connection);
            SetupConnectionEvents(Connection);
        }
    }
}
