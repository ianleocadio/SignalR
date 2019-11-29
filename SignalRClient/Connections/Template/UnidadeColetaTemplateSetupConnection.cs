using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using SignalRClient.Configurations;

namespace SignalRClient.Connections.Template
{
    public class UnidadeColetaTemplateSetupConnection : AbstractTemplateSetupConnection
    {

        protected override ILogger _logger { get; set; }

        public UnidadeColetaTemplateSetupConnection(ILogger<UnidadeColetaTemplateSetupConnection> logger, CustomConfiguration configuration, ConnectionProvider connectionProvider) 
            : base(logger, configuration, connectionProvider)
        {
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
                    connection.InvokeAsync("FalhaImpressao", new { Unidade = _configuration.Data.geral.Unidade, Etiqueta = etiqueta });
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
                        _logger.LogInformation("Tentando reconectar...");
                        await connection.StartAsync();
                        _logger.LogInformation("Conexão realizada");

                        Task tryHandShakeTask = null;
                        do
                        {
                            try
                            {
                                tryHandShakeTask = connection.InvokeAsync("HandShake", new { Unidade = _configuration.Data.geral.Unidade });
                                await tryHandShakeTask;
                                if (tryHandShakeTask.IsCompletedSuccessfully)
                                {
                                    _logger.LogInformation("HandShake realizado com sucesso");
                                    break;
                                }
                                else if (tryHandShakeTask.IsCanceled)
                                {
                                    _logger.LogWarning("HandShake cancelado");
                                }
                                else if (tryHandShakeTask.IsFaulted)
                                {
                                    _logger.LogError("Falha ao realizar o HandShake");
                                }
                                else
                                {
                                    _logger.LogError("Ocorreu um erro não tratado durante o handshake");
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError("{ex}", ex);
                            }

                        } while (!tryHandShakeTask.IsCompletedSuccessfully);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("Erro ao conectar");
                        _logger.LogDebug("{ex}", ex);
                        _logger.LogError("{ex}", ex.Message);
                    }

                    await Task.Delay(10000);
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
