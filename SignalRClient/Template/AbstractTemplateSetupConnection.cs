using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace SignalRClient.Connections.Template
{
    public abstract class AbstractTemplateSetupConnection
    {
        protected ILogger _logger { get; set; }
        protected readonly IConfiguration _configuration;
        private readonly HubConnection _connection;
        protected readonly CommandDispatcher _commandDispatcher;

        protected AbstractTemplateSetupConnection(ILogger logger, IConfiguration configuration, ConnectionProvider connectionProvider
            , CommandDispatcher commandDispatcher)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _connection = connectionProvider?.Connection ?? throw new ArgumentNullException(nameof(connectionProvider.Connection));
            _commandDispatcher = commandDispatcher ?? throw new ArgumentNullException(nameof(commandDispatcher));
        }

        public virtual void SetupOnCloseConnection(HubConnection connection) { }
        public virtual void SetupOnReconnectConnection(HubConnection connection) { }
        public virtual void SetupOnReconnectingConnection(HubConnection connection) { }
        public virtual void SetupConnectionEvents(HubConnection connection) { }

        public void SetupConnection()
        {
            SetupOnCloseConnection(_connection);
            SetupOnReconnectConnection(_connection);
            SetupOnReconnectingConnection(_connection);
            SetupConnectionEvents(_connection);
        }
    }
}
