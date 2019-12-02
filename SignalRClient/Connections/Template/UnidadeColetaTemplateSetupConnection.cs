using System;
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
            ConnectionProvider connectionProvider, HandShake handShake) 
            : base(logger, configuration, connectionProvider)
        {
            _handShake = handShake;
        }

        public override void SetupConnectionEvents(HubConnection connection)
        {
            connection.On<string>("Imprime", (string etiqueta) =>
            {
                _logger.LogInformation("Etiqueta a ser impressa: {etiqueta}", etiqueta);

                Thread.Sleep(3000);

                int binary = new Random().Next(0, 2);
                if (binary == 0)
                {
                    _logger.LogError("Falha ao imprimir etiqueta: {etiqueta}", etiqueta);
                    connection.InvokeAsync("FalhaImpressao", new { Unidade = _configuration["Geral:Unidade"], Etiqueta = etiqueta });
                    return;
                }

                _logger.LogInformation("Etiqueta impressa: {etiqueta}", etiqueta);
            });
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
