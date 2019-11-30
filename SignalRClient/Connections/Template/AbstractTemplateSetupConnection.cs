using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace SignalRClient.Connections.Template
{
    public abstract class AbstractTemplateSetupConnection
    {
        protected abstract ILogger _logger { get; set; }
        protected readonly IConfiguration _configuration;
        private readonly HubConnection Connection;

        protected AbstractTemplateSetupConnection(ILogger logger, IConfiguration configuration, ConnectionProvider connectionProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            Connection = connectionProvider?.Connection ?? throw new ArgumentNullException(nameof(connectionProvider.Connection));
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
