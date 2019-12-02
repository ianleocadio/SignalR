using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SignalRClient.Connections;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SignalRClient.Execute
{
    public class HandShake
    {

        private readonly ILogger<HandShake> _logger;
        private readonly HubConnection _connection;
        private readonly IConfiguration _configuration;

        public HandShake(ILogger<HandShake> logger, ConnectionProvider connectionProvider, IConfiguration configuration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _connection = connectionProvider?.Connection;
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task ExecuteAsync()
        {
            Task tryHandShakeTask = null;
            do
            {
                try
                {
                    _logger.LogInformation("Realizando HandShake...");
                    tryHandShakeTask = _connection.InvokeAsync("HandShake", new { Unidade = _configuration["Geral:Unidade"] });
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
                    _logger.LogError("{ex}", ex.Message);
                }
                finally
                {
                    await Task.Delay(15000);
                }

            } while (!tryHandShakeTask.IsCompletedSuccessfully);

        }

    }
}
