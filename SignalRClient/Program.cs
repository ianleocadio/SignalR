using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Microsoft.Extensions.Configuration;
using SignalRClient.Connections;
using SignalRClient.Connections.Template;
using SignalRClient.Logging;
using System.Diagnostics;

namespace SignalRClient
{
    class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {

            #region Contruíndo Configuração
            var configFilePath = "appsettings.json";

            if (Debugger.IsAttached)
                configFilePath = "appsettings.Development.json";

            var builtConfig = new ConfigurationBuilder()
                                .AddJsonFile(configFilePath, optional: false, reloadOnChange: false)
                                .AddCommandLine(args)
                                .Build();
            #endregion

            #region Criando Logger
            Log.Logger = new LoggerProvider(builtConfig).GetLogger();
            #endregion

            IHostBuilder hostBuilder = null;
            try
            {
                Log.Information("Iniciando serviço");

                hostBuilder = Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureAppConfiguration((hostContext, config) => config.AddConfiguration(builtConfig))
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();

                    #region Singletons
                    services.AddSingleton<ConnectionProvider>((provider) =>
                    {
                        ILogger<ConnectionProvider> logger = provider.GetRequiredService<ILogger<ConnectionProvider>>();
                        IConfiguration config = provider.GetRequiredService<IConfiguration>();

                        return new ConnectionProvider(logger, config);
                    });
                    #endregion

                    #region Scoped's
                    services.AddScoped<AbstractTemplateSetupConnection, UnidadeColetaTemplateSetupConnection>((provider) =>
                    {
                        ILogger<UnidadeColetaTemplateSetupConnection> logger = provider.GetRequiredService<ILogger<UnidadeColetaTemplateSetupConnection>>();
                        IConfiguration config = provider.GetRequiredService<IConfiguration>();
                        ConnectionProvider connectionProvider = provider.GetRequiredService<ConnectionProvider>();

                        return new UnidadeColetaTemplateSetupConnection(logger, config, connectionProvider);
                    });
                    #endregion
                })
                .UseSerilog(logger: Log.Logger);

            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Erro ao contruir serviço");
            }
            finally
            {
                Log.CloseAndFlush();
            }

            return hostBuilder;

        }
    }
}
