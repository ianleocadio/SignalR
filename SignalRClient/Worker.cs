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
        private readonly ConnectionProvider _connectionProvider;
        private readonly HandShake _handShake;

        public Worker(ILogger<Worker> logger, IConfiguration configuration,
            ConnectionProvider connectionProvider, AbstractTemplateSetupConnection templateSetupConnection,
            HandShake handShake)
        {
            _logger = logger;
            _configuration = configuration;
            _connectionProvider = connectionProvider;
            _handShake = handShake;

            templateSetupConnection.SetupConnection();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _connectionProvider.StartConnection(_logger);

            await _handShake.StartHandShake(_logger);
            
            _logger.LogInformation("Serviço na unidade {unidade} está executando", _configuration["Geral:Unidade"]);
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(10000);
            }
            _logger.LogInformation("Serviço na unidade {unidade} finalizado", _configuration["Geral:Unidade"]);

        }

    }

}
