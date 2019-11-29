using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Microsoft.Extensions.Configuration;
using SignalRClient.Connections;
using SignalRClient.Connections.Template;
using SignalRClient.Logging;

namespace SignalRClient
{
    class Program
    {
        static LoggerProvider loggerProvider;

        public static void Main(string[] args)
        {
            //LoggerProvider loggerProvider = new LoggerProvider();
            Log.Logger = new LoggerConfiguration()
                        .Enrich.FromLogContext()
                        .WriteTo.File(@"c:\temp")
                        .CreateLogger();

            try
            {
                
            }
            catch (Exception ex)
            {

            }
            finally
            {
                Log.CloseAndFlush();
            }
            
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    var configFilePath = "appsettings.json";

                    if (hostContext.HostingEnvironment.IsDevelopment())
                        configFilePath = "appsettings.Development.json";

                    config.AddJsonFile(configFilePath, optional: false, reloadOnChange: false);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();

                    #region Singletons
                    services.AddSingleton<ConnectionProvider>((provider) =>
                            {
                                ILogger<ConnectionProvider> logger = provider.GetService<ILogger<ConnectionProvider>>();
                                IConfiguration config = provider.GetService<IConfiguration>();

                                return new ConnectionProvider(logger, config);
                            });
                    #endregion

                    #region Scoped's
                    services.AddScoped<AbstractTemplateSetupConnection, UnidadeColetaTemplateSetupConnection>((provider) =>
                            {
                                ILogger<UnidadeColetaTemplateSetupConnection> logger = provider.GetService<ILogger<UnidadeColetaTemplateSetupConnection>>();
                                IConfiguration config = provider.GetService<IConfiguration>();
                                ConnectionProvider connectionProvider = provider.GetService<ConnectionProvider>();

                                return new UnidadeColetaTemplateSetupConnection(logger, configuration, connectionProvider);
                                
                            });
                    #endregion
                })
                .UseSerilog();
    }
}
