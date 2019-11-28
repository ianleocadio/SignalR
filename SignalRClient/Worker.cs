using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SignalRClient.Connections;
using SignalRClient.Connections.Template;
using SignalRClient.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SignalRClient
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger = LoggerProvider.GetLogger<Worker>();

        private readonly HubConnection _connection;

        public static readonly string Unidade = "Filial 1";

        public Worker()
        {
            _connection = ConnectionProvider.GetInstance(true)?.Connection;

            AbstractTemplateSetupConnection templateSetupConnection = 
                new UnidadeColetaTemplateSetupConnection(_connection);

            templateSetupConnection.SetupConnection();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation("[{time}] Realizando conexão...", DateTimeOffset.Now);
                _connection.StartAsync().Wait();
                _logger.LogInformation("[{time}] Conexão realizada", DateTimeOffset.Now);
            }
            catch (Exception ex)
            {
                _logger.LogError("[{time}] Erro ao realiza conexão", DateTimeOffset.Now);
                _logger.LogError("{ex}", ex);
            }
            finally
            {

                Task tryHandShakeTask = null;
                do
                {
                    try
                    {
                        tryHandShakeTask = _connection.InvokeAsync("HandShake", new { Unidade = Worker.Unidade });
                        tryHandShakeTask.Wait();
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


                _logger.LogInformation("[{time}] Serviço na unidade {unidade} está executando", DateTimeOffset.Now, Unidade);
                while (!stoppingToken.IsCancellationRequested)
                {
                }
                _logger.LogInformation("[{time}] Serviço na unidade {unidade} finalizado", DateTimeOffset.Now, Unidade);
            }
            
        }
    }
}
