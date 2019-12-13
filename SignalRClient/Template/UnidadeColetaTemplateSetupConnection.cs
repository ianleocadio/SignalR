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

        public override void SetupConnectionEvents(HubConnection connection)
        {
            _commandDispatcher.BindCommands();
        }

        public override void SetupOnCloseConnection(HubConnection connection)
        {
            connection.Closed += async (error) =>
            {
                while (connection.State == HubConnectionState.Disconnected)
                {
                    try
                    {
                        _logger.LogInformation("Tentando realizar conexão...");

                        var taskConnection = connection.StartAsync();
                        await taskConnection;
                        if (!taskConnection.IsCompletedSuccessfully)
                            continue;

                        _logger.LogInformation("Conexão realizada");

                        var taskHandShake = _handShake.ExecuteAsync();
                        await taskHandShake;
                        while (!taskHandShake.IsCompletedSuccessfully)
                        {
                            taskHandShake = _handShake.ExecuteAsync();
                            await taskHandShake;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("Erro ao conectar");
                        _logger.LogError("{ex}", ex.Message);
                    }
                }
            };
        }

        /// <summary>
        /// Não implementado ainda
        /// </summary>
        /// <param name="connection"></param>
        public override void SetupOnReconnectConnection(HubConnection connection)
        {
            base.SetupOnReconnectConnection(connection);
        }

        /// <summary>
        /// Não implementado ainda
        /// </summary>
        /// <param name="connection"></param>
        public override void SetupOnReconnectingConnection(HubConnection connection)
        {
            base.SetupOnReconnectingConnection(connection);
        }
    }
}
