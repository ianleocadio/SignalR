using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using SignalRClient.Logging;
using Serilog;

namespace SignalRServer
{
    public partial class Program
    {

        #region Pendência
        public class Pendencia
        {
            public enum StatusPendencia
            { 
                Open,
                Processing,
                Closed
            }

            public string unidade;
            public string etiqueta;
            public StatusPendencia status;

            public Pendencia(string unidade, string etiqueta, StatusPendencia status)
            {
                this.unidade = unidade ?? throw new ArgumentNullException(nameof(unidade));
                this.etiqueta = etiqueta ?? throw new ArgumentNullException(nameof(etiqueta));
                this.status = status;
            }
        }

        public static List<Pendencia> lstPendencias = new List<Pendencia>();
        #endregion

        public static void Main(string[] args)
        {
            CreateLstPendencia();

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

            Parallel.Invoke(() =>
            {
                RunApp(Log.Logger);
            },
            () =>
            {
                RunWebHost(Log.Logger, builtConfig);
            });
        }

        public static IWebHostBuilder CreateWebHostBuilder(ILogger logger, IConfiguration configuration, string[] args)
        {

            IWebHostBuilder webHostBuilder = null;
            try
            {
                Log.Information("Iniciando aplicação");
                webHostBuilder = WebHost.CreateDefaultBuilder(args)
                          .ConfigureAppConfiguration((hostContext, config) => config.AddConfiguration(configuration))
                          .UseSerilog(logger: logger)
                          .UseStartup<Startup>();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Erro ao construir aplicação");
                throw ex;
            }
            finally
            {
                Log.CloseAndFlush();
            }

            return webHostBuilder;
        }

        public static void RunWebHost(ILogger logger, IConfiguration configuration)
        {
            CreateWebHostBuilder(logger, configuration, new string[] { }).Build().Run();
        }

        public static void RunApp(ILogger logger)
        {
            var console = Console.ReadKey();

            while (console.Key != ConsoleKey.Escape) 
            {
                if (console.Key == ConsoleKey.DownArrow)
                {
                    var nFilial = new Random().Next(1, 4);
                    var nEtiqueta = new Random().Next(0, 99);

                    var p = new Pendencia($"Filial {nFilial}", $"etq{nEtiqueta}un{nFilial}", Pendencia.StatusPendencia.Open);
                    logger.Information($"Criando Pendência: {p.unidade} -> {p.etiqueta}");
                    lstPendencias.Add(p);
                }

                console = Console.ReadKey();
            }
        }


        private static void CreateLstPendencia()
        {
            lstPendencias.Add(new Pendencia("Filial 1", "etq1un1", Pendencia.StatusPendencia.Open));
            lstPendencias.Add(new Pendencia("Filial 1", "etq2un1", Pendencia.StatusPendencia.Open));
            lstPendencias.Add(new Pendencia("Filial 1", "etq3un1", Pendencia.StatusPendencia.Open));
            lstPendencias.Add(new Pendencia("Filial 1", "etq4un1", Pendencia.StatusPendencia.Open));

            lstPendencias.Add(new Pendencia("Filial 2", "etq1un2", Pendencia.StatusPendencia.Open));
            lstPendencias.Add(new Pendencia("Filial 2", "etq2un2", Pendencia.StatusPendencia.Open));
            lstPendencias.Add(new Pendencia("Filial 2", "etq3un2", Pendencia.StatusPendencia.Open));
            lstPendencias.Add(new Pendencia("Filial 2", "etq4un2", Pendencia.StatusPendencia.Open));
            lstPendencias.Add(new Pendencia("Filial 2", "etq5un2", Pendencia.StatusPendencia.Open));
            lstPendencias.Add(new Pendencia("Filial 2", "etq6un2", Pendencia.StatusPendencia.Open));
            lstPendencias.Add(new Pendencia("Filial 2", "etq7un2", Pendencia.StatusPendencia.Open));
            lstPendencias.Add(new Pendencia("Filial 2", "etq8un2", Pendencia.StatusPendencia.Open));
            lstPendencias.Add(new Pendencia("Filial 2", "etq9un2", Pendencia.StatusPendencia.Open));
            lstPendencias.Add(new Pendencia("Filial 2", "etq10un2", Pendencia.StatusPendencia.Open));

            //lstPendencias.Add(new Pendencia("Filial 3", "etq1un3", Pendencia.StatusPendencia.Open));
            //lstPendencias.Add(new Pendencia("Filial 3", "etq2un3", Pendencia.StatusPendencia.Open));
            //lstPendencias.Add(new Pendencia("Filial 3", "etq3un3", Pendencia.StatusPendencia.Open));
            //lstPendencias.Add(new Pendencia("Filial 3", "etq4un3", Pendencia.StatusPendencia.Open));
            //lstPendencias.Add(new Pendencia("Filial 3", "etq5un3", Pendencia.StatusPendencia.Open));
            //lstPendencias.Add(new Pendencia("Filial 3", "etq6un3", Pendencia.StatusPendencia.Open));
            //lstPendencias.Add(new Pendencia("Filial 3", "etq7un3", Pendencia.StatusPendencia.Open));
        }
    }
}
