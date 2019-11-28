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

        private readonly string Unidade = "Filial 1";

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
                var task = _connection.InvokeAsync("HandShake", new { Unidade });
                task.Wait();
                if (task.IsCompletedSuccessfully)
                    _logger.LogInformation("[{time}] HandShake realizado com sucesso", DateTimeOffset.Now);
                else if (task.IsCanceled)
                    _logger.LogWarning("[{time}] HandShake cancelado", DateTimeOffset.Now);
                else if (task.IsFaulted)
                    _logger.LogError("[{time}] Falha ao realizar o HandShake", DateTimeOffset.Now);
                else
                    _logger.LogCritical("[{time}] Ocorreu um erro não tratado durante o handshake", DateTimeOffset.Now);


                _logger.LogInformation("[{time}] Serviço na unidade {unidade} está executando", DateTimeOffset.Now, Unidade);
                while (!stoppingToken.IsCancellationRequested)
                {
                }
            }
            
        }
    }
}
