using System;
using System.Configuration;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;
using SignalRClient.Configurations;

namespace SignalRClient
{
    class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) => 
                    services.AddHostedService<Worker>()
                            .Configure<EventLogSettings>(config =>
                            {
                                config.LogName = "Filial 1 Service";
                                config.SourceName = "Filial 1 Service Source";
                            }))
                .UseWindowsService();
    }
}
