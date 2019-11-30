using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using SignalRClient.Connections;
using SignalRClient.Connections.Template;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace SignalRClient
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private readonly HubConnection _connection;

        public Worker(ILogger<Worker> logger, IConfiguration configuration, ConnectionProvider connectionProvider, AbstractTemplateSetupConnection templateSetupConnection)
        {
            _logger = logger;
            _configuration = configuration;
            _connection = connectionProvider?.Connection;

            templateSetupConnection.SetupConnection();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation("Realizando conexão...");
                _connection.StartAsync().Wait();
                _logger.LogInformation("Conexão realizada");
            }
            catch (Exception ex)
            {
                _logger.LogError("Erro ao realiza conexão");
                _logger.LogError("{ex}", ex.Message);
            }
            finally
            {

                Task tryHandShakeTask = null;
                do
                {
                    try
                    {
                        tryHandShakeTask = _connection.InvokeAsync("HandShake", new { Unidade = _configuration["Geral:Unidade"] });
                        tryHandShakeTask.Wait();
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
                        _logger.LogError("{ex}", ex.Message);
                        await Task.Delay(5000);
                    }

                } while (!tryHandShakeTask.IsCompletedSuccessfully);


                _logger.LogInformation("Serviço na unidade {unidade} está executando", _configuration["Geral:Unidade"]);
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(3000);
                }
                _logger.LogInformation("Serviço na unidade {unidade} finalizado", _configuration["Geral:Unidade"]);
            }
            
        }
    }
}
