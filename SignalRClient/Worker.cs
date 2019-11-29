using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using SignalRClient.Connections;
using SignalRClient.Connections.Template;
using System;
using System.Threading;
using System.Threading.Tasks;
using SignalRClient.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace SignalRClient
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly CustomConfiguration _configuration;
        private readonly HubConnection _connection;

        public Worker(ILogger<Worker> logger, CustomConfiguration configuration, ConnectionProvider connectionProvider, AbstractTemplateSetupConnection templateSetupConnection)
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
                        tryHandShakeTask = _connection.InvokeAsync("HandShake", new { _configuration.Data.geral.Unidade });
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
                    }

                } while (!tryHandShakeTask.IsCompletedSuccessfully);


                _logger.LogInformation("Serviço na unidade {unidade} está executando", _configuration.Data.geral.Unidade);
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(3000);
                }
                _logger.LogInformation("Serviço na unidade {unidade} finalizado", _configuration.Data.geral.Unidade);
            }
            
        }
    }
}
