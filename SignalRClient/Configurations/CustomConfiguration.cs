using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using SignalRClient.Logging;
using System;
using System.IO;
using static SignalRClient.Configurations.CustomConfigurationPOJO;

namespace SignalRClient.Configurations
{
    public class CustomConfiguration
    {
        public CustomConfigurationPOJO Data { get; set; }

        public CustomConfiguration(bool IsDevelopmentEnviroment = false)
        {
            IConfigurationRoot config = GetConfiguration(IsDevelopmentEnviroment);

            Data = config.GetSection("Configuration").Get<CustomConfigurationPOJO>();
        }

        private IConfigurationRoot GetConfiguration(bool IsDevelopmentEnviroment = false)
        {

            IConfigurationRoot config = null;

            try
            {
                config = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile((IsDevelopmentEnviroment) ? "appsettings.Development.json" : "appsettings.json", false, true)
                        .Build();
            }
            catch (Exception ex)
            {
                Log.Error("Erro ao carregar arquivo de configurações appsettigs");
                Log.Error("{ex}", ex);
                throw ex;
            }

            return config;
        }
    }
}
