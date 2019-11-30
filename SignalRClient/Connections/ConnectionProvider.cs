using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace SignalRClient.Connections
{
    public class ConnectionProvider
    {
        public HubConnection Connection;

        public ConnectionProvider(ILogger<ConnectionProvider> logger, IConfiguration configuration)
        {
            try
            {
                Connection = new HubConnectionBuilder()
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
                logger.LogInformation("Erro ao construir a conexão com o servidor");
                throw ex;
            }
        }
    }
}
