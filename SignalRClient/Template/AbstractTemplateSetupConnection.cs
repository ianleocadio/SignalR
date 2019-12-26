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
        protected readonly ConnectionProvider _connectionProvider;
        protected readonly CommandDispatcher _commandDispatcher;

        protected AbstractTemplateSetupConnection(ILogger logger, IConfiguration configuration, ConnectionProvider connectionProvider
            , CommandDispatcher commandDispatcher)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
            _commandDispatcher = commandDispatcher ?? throw new ArgumentNullException(nameof(commandDispatcher));
        }

        public virtual void SetupOnCloseConnection() { }
        public virtual void SetupOnReconnectConnection() { }
        public virtual void SetupOnReconnectingConnection() { }
        public virtual void SetupConnectionEvents() { }

        public void SetupConnection()
        {
            SetupOnCloseConnection();
            SetupOnReconnectConnection();
            SetupOnReconnectingConnection();
            SetupConnectionEvents();
        }
    }
}
