using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using SignalRClient.Connections;
using SignalRClient.Connections.Template;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using SignalRClient.Execute;

namespace SignalRClient
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private readonly HubConnection _connection;
        private readonly HandShake _handShake;

        public Worker(ILogger<Worker> logger, IConfiguration configuration, 
            ConnectionProvider connectionProvider, AbstractTemplateSetupConnection templateSetupConnection,
            HandShake handShake)
        {
            _logger = logger;
            _configuration = configuration;
            _connection = connectionProvider?.Connection;
            _handShake = handShake;

            templateSetupConnection.SetupConnection();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation("Realizando conexão...");
                var taskConnection = _connection.StartAsync();
                await taskConnection;
                while (taskConnection.IsFaulted || taskConnection.IsCanceled)
                {
                    await Task.Delay(10000);
                    _logger.LogInformation("Tentando realizar conexão...");
                    taskConnection = _connection.StartAsync();
                    await taskConnection;
                }

                _logger.LogInformation("Conexão realizada");

                var taskHandShake = _handShake.ExecuteAsync();
                await taskHandShake;
                while (taskHandShake.IsFaulted || taskHandShake.IsCanceled)
                {
                    await Task.Delay(10000);
                    taskHandShake = _handShake.ExecuteAsync();
                    await taskHandShake;
                }

                _logger.LogInformation("Serviço na unidade {unidade} está executando", _configuration["Geral:Unidade"]);
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(10000);
                }
                _logger.LogInformation("Serviço na unidade {unidade} finalizado", _configuration["Geral:Unidade"]);
            }
            // Arrumar trycatch
            catch (Exception ex)
            {
                _logger.LogError("Erro ao realiza conexão");
                _logger.LogError("{ex}", ex.Message);
            }
            
        }
    }
}
