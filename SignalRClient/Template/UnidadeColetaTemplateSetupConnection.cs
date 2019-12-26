using System;
using System.Collections.Generic;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SignalRClient.Execute;

namespace SignalRClient.Connections.Template
{
    public class UnidadeColetaTemplateSetupConnection : AbstractTemplateSetupConnection
    {
        private readonly HandShake _handShake;

        public UnidadeColetaTemplateSetupConnection(ILogger<UnidadeColetaTemplateSetupConnection> logger, IConfiguration configuration, 
            ConnectionProvider connectionProvider, HandShake handShake, CommandDispatcher commandDispatcher) 
            : base(logger, configuration, connectionProvider, commandDispatcher)
        {
            _handShake = handShake;
        }

        public override void SetupConnectionEvents()
        {
            _commandDispatcher.BindCommands();
        }

        public override void SetupOnCloseConnection()
        {
            _connectionProvider.Connection.Closed += async (error) =>
            {
                await _connectionProvider.StartConnection();

                await _handShake.StartHandShake();
            };
        }

        /// <summary>
        /// Não implementado ainda
        /// </summary>
        /// <param name="connection"></param>
        public override void SetupOnReconnectConnection()
        {
            base.SetupOnReconnectConnection();
        }

        /// <summary>
        /// Não implementado ainda
        /// </summary>
        /// <param name="connection"></param>
        public override void SetupOnReconnectingConnection()
        {
            base.SetupOnReconnectingConnection();
        }
    }
}
