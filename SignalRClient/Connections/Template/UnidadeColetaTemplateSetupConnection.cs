using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using SignalRClient.Logging;

namespace SignalRClient.Connections.Template
{
    public class UnidadeColetaTemplateSetupConnection : AbstractTemplateSetupConnection
    {

        private static readonly ILogger<UnidadeColetaTemplateSetupConnection> _logger = LoggerProvider.GetLogger<UnidadeColetaTemplateSetupConnection>();

        public UnidadeColetaTemplateSetupConnection(HubConnection connection) : base(connection)
        {

        }

        public override void SetupConnectionEvents(HubConnection connection)
        {
            connection.On<string>("Imprime", (string etiqueta) =>
            {
                _logger.LogInformation("[{time}] Etiqueta a ser impressa: {etiqueta}", DateTimeOffset.Now, etiqueta);
                
                Thread.Sleep(3000);
                
                _logger.LogInformation("[{time}] Etiqueta impressa: {etiqueta}", DateTimeOffset.Now, etiqueta);

                connection.InvokeAsync("ImpressaoSucesso", new { Unidade = "Filial 3", Etiqueta = etiqueta });
            });
        }

        public override void SetupOnCloseConnection(HubConnection connection)
        {
            connection.Closed += async (error) =>
            {
                
                while (connection.State == HubConnectionState.Disconnected || connection.State == HubConnectionState.Reconnecting)
                {
                    try
                    {
                        _logger.LogInformation("[{time}] Tentando reconectar...", DateTimeOffset.Now);
                        await connection.StartAsync();
                        _logger.LogInformation("[{time}] Conexão realizada", DateTimeOffset.Now);

                        Task tryHandShakeTask = null;
                        do
                        {
                            try
                            {
                                tryHandShakeTask = connection.InvokeAsync("HandShake", new { Unidade = Worker.Unidade });
                                await tryHandShakeTask;
                                if (tryHandShakeTask.IsCompletedSuccessfully)
                                {
                                    _logger.LogInformation("[{time}] HandShake realizado com sucesso", DateTimeOffset.Now);
                                    break;
                                }
                                else if (tryHandShakeTask.IsCanceled)
                                {
                                    _logger.LogWarning("[{time}] HandShake cancelado", DateTimeOffset.Now);
                                }
                                else if (tryHandShakeTask.IsFaulted)
                                {
                                    _logger.LogError("[{time}] Falha ao realizar o HandShake", DateTimeOffset.Now);
                                }
                                else
                                {
                                    _logger.LogCritical("[{time}] Ocorreu um erro não tratado durante o handshake", DateTimeOffset.Now);
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogCritical("[{time}] {ex}", ex);
                            }

                        } while (!tryHandShakeTask.IsCompletedSuccessfully);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("[{time}] Erro ao conectar", DateTimeOffset.Now);
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
            connection.Reconnected += async (string s) =>
            {
                Task tryHandShakeTask = null;
                do
                {
                    try
                    {
                        tryHandShakeTask = connection.InvokeAsync("HandShake", new { Unidade = Worker.Unidade });
                        await tryHandShakeTask;
                        if (tryHandShakeTask.IsCompletedSuccessfully)
                        {
                            _logger.LogInformation("[{time}] HandShake realizado com sucesso", DateTimeOffset.Now);
                            break;
                        }
                        else if (tryHandShakeTask.IsCanceled)
                        {
                            _logger.LogWarning("[{time}] HandShake cancelado", DateTimeOffset.Now);
                        }
                        else if (tryHandShakeTask.IsFaulted)
                        {
                            _logger.LogError("[{time}] Falha ao realizar o HandShake", DateTimeOffset.Now);
                        }
                        else
                        {
                            _logger.LogCritical("[{time}] Ocorreu um erro não tratado durante o handshake", DateTimeOffset.Now);
                        }
                    }
                    catch (Exception ex) 
                    {
                        _logger.LogCritical("[{time}] {ex}", ex);
                    }

                } while (!tryHandShakeTask.IsCompletedSuccessfully);

            };
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
