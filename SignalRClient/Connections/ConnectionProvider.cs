using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using SignalRClient.Configurations;
using System;

namespace SignalRClient.Connections
{
    public class ConnectionProvider
    {
        private readonly ILogger _logger;
        public HubConnection Connection;

        public ConnectionProvider(ILogger<ConnectionProvider> logger, CustomConfiguration configuration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            try
            {
                Connection = new HubConnectionBuilder()
                       .WithUrl(configuration.Data.serverConnection.URI, options =>
                       {
                           options.Headers.Add("Authorization", $"Basic {configuration.Data.authentication.GetCredentials()}");
                       })
                       .Build();
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Erro ao construir a conexão com o servidor");
                throw ex;
            }
        }
    }
}
