using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using SignalRClient.Configurations;
using SignalRClient.Connections;
using SignalRClient.Connections.Template;
using SignalRClient.Logging;

namespace SignalRClient
{
    class Program
    {

        public static void Main(string[] args)
        {
            CustomConfiguration config = new CustomConfiguration(IsDevelopmentEnviroment: true);
            LoggerProvider loggerProvider = new LoggerProvider(config);
            Log.Logger = loggerProvider.GetLogger();

            try
            {
                Log.Information("Iniciando o serviço");
                CreateHostBuilder(config, args).Build().Run();
                return;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Houve um problema ao inciar o serviço");
                return;
            }
            finally
            {
                Log.CloseAndFlush();
            }

            
        }

        public static IHostBuilder CreateHostBuilder(CustomConfiguration configuration, string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();

                    #region Singletons
                    services.AddSingleton<CustomConfiguration>(configuration)
                            .AddSingleton<ConnectionProvider>((provider) =>
                            {
                                ILogger<ConnectionProvider> logger = provider.GetService<ILogger<ConnectionProvider>>();
                                CustomConfiguration config = provider.GetService<CustomConfiguration>();

                                return new ConnectionProvider(logger, config);
                            });
                    #endregion

                    #region Scoped's
                    services.AddScoped<AbstractTemplateSetupConnection, UnidadeColetaTemplateSetupConnection>((provider) =>
                            {
                                ILogger<UnidadeColetaTemplateSetupConnection> logger = provider.GetService<ILogger<UnidadeColetaTemplateSetupConnection>>();
                                CustomConfiguration config = provider.GetService<CustomConfiguration>();
                                ConnectionProvider connectionProvider = provider.GetService<ConnectionProvider>();

                                return new UnidadeColetaTemplateSetupConnection(logger, configuration, connectionProvider);
                                
                            });
                    #endregion
                })
                .UseSerilog();
    }
}
