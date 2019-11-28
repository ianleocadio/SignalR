using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using SignalRClient.Configurations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static SignalRClient.Configurations.CustomConfiguration;

namespace SignalRClient.Connections
{
    public class ConnectionProvider
    {

        private static ConnectionProvider _instance;
        
        public HubConnection Connection;

        private ConnectionProvider(HubConnection connection)
        {
            Connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public static ConnectionProvider GetInstance(bool IsDevelopmentEnviroment)
        {
            if (_instance != null)
                return _instance;


            IConfigurationRoot config = GetConfiguration(IsDevelopmentEnviroment);
            Authentication auth = config.GetSection("Authentication").Get<Authentication>();
            ServerConnection serverConn = config.GetSection("ServerConnection").Get<ServerConnection>();

            try
            {
                var connection = new HubConnectionBuilder()
                       .WithUrl(serverConn.URI, options =>
                       {
                           options.Headers.Add("Authorization", $"Basic {auth.GetCredentials()}");
                       })
                       .Build();

                _instance = new ConnectionProvider(connection);

                return _instance;
            } 
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao construir a conexão com o servidor");
                throw ex;
            }
        }


        private static IConfigurationRoot GetConfiguration(bool IsDevelopmentEnviroment = false)
        {

            IConfigurationRoot config = null;

            try
            {
                Console.WriteLine(Directory.GetCurrentDirectory());
                config = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile((IsDevelopmentEnviroment) ? "appsettings.Development.json" : "appsettings.json", false, true)
                        .Build();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao carregar arquivo de configurações appsettigs");
                throw ex;
            }

            return config;
        }
    }
}
