using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using System.Globalization;
using SignalRServer.Caller.Controllers;
using SignalRClient.Logging;
using Microsoft.Extensions.Logging;

namespace SignalRServer
{
    public partial class Program
    {

        private static readonly ILogger<Program> _logger = LoggerProvider.GetLogger<Program>();

        private static IWebHost wb = null;

        private static ImprimeCallerController _imprimeCallerController;
        public static ImprimeCallerController ImprimeCallerController
        {
            get
            {
                if (_imprimeCallerController == null)
                    _imprimeCallerController = new ImprimeCallerController();

                return _imprimeCallerController;
            }
        }

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

        public static void Main(string[] args)
        {
            CreateLstPendencia();

            Parallel.Invoke(() =>
            {
                RunApp();
            },
            () =>
            {
                RunWebHost();
            });
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();

        public static void RunWebHost()
        {
            wb = CreateWebHostBuilder(new string[] { }).Build();
            wb.Run();
        }

        public static void RunApp()
        {
            var console = Console.ReadKey();

            while (console.Key != ConsoleKey.Escape) 
            {
                if (console.Key == ConsoleKey.DownArrow)
                {
                    var nFilial = new Random().Next(1, 4);
                    var nEtiqueta = new Random().Next(0, 99);

                    var p = new Pendencia($"Filial {nFilial}", $"etq{nEtiqueta}un{nFilial}", Pendencia.StatusPendencia.Open);
                    _logger.LogInformation("[{time} Criando Pendência: {unidade} -> {etiqueta}]", DateTimeOffset.Now, p.unidade, p.etiqueta);
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
