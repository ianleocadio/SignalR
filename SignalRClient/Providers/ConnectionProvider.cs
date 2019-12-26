using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace SignalRClient.Connections
{
    public class ConnectionProvider
    {
        public readonly ILogger<ConnectionProvider> _logger;

        private HubConnection _connection;
        public HubConnection Connection { get => _connection; }

        public ConnectionProvider(ILogger<ConnectionProvider> logger, IConfiguration configuration)
        {
            _logger = logger;

            try
            {
                _connection = new HubConnectionBuilder()
                       .WithAutomaticReconnect()
                       .WithUrl(configuration["ServerConnection:URI"], options =>
                       {
                           var username = configuration["Authentication:Username"];
                           var password = configuration["Authentication:Password"];
                           var credetials = Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
                           options.Headers.Add("Authorization", $"Basic {credetials}");
                       })
                       .Build();
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Erro ao construir a conexão com o servidor");
                throw ex;
            }
        }

        public async Task StartConnection(ILogger logger = null)
        {
            if (logger == null)
                logger = _logger;
            try
            {
                Task taskConn = null;
                logger.LogInformation("Realizando conexão...");
                do
                {
                    try
                    {
                        taskConn = _connection.StartAsync();
                        await taskConn;
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, ex.Message);
                        logger.LogInformation("Tentando realizar conexão...");
                        await Task.Delay(7000);
                    }

                } while (_connection.State == HubConnectionState.Disconnected);

                logger.LogInformation("Conexão realizada");

            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }

        }
    }
}
