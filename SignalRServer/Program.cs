using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using System.Globalization;

namespace SignalRServer
{
    public partial class Program
    {

        private static IWebHost wb = null;

        private static CallerController<TesteCaller> _callerController;
        public static CallerController<TesteCaller> CallerController
        {
            get
            {
                if (_callerController == null)
                    _callerController = new CallerController<TesteCaller>();

                return _callerController;
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
            lstPendencias.Add(new Pendencia("Filial 1", "etq1un1", Pendencia.StatusPendencia.Open));
            lstPendencias.Add(new Pendencia("Filial 2", "etq1un2", Pendencia.StatusPendencia.Open));
            lstPendencias.Add(new Pendencia("Filial 3", "etq1un3", Pendencia.StatusPendencia.Open));

            Thread t1 = new Thread(new ThreadStart(RunWebHost));
            t1.Start();
        }

        public static void RunWebHost()
        {
            wb = CreateWebHostBuilder(new string[] { }).Build();
            wb.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();


        public static string GetTime()
        {
            return string.Format("({0}) ", DateTime.UtcNow.ToString("dd-MM-yyyy HH:mm:ss.ffffff",
                                            CultureInfo.InvariantCulture));
        }


    }
}
