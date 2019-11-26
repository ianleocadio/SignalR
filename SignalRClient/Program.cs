using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace SignalRClient
{
    class Program
    {

        static HubConnection Connection;

        static void Main(string[] args)
        {

            Connection = new HubConnectionBuilder()
                   .WithUrl("http://localhost:5000/ImpressaoPostoColeta")
                   .Build();

            //Connection.Closed += async (error) =>
            //{
            //    await Task.Delay(new Random().Next(0, 5) * 1000);
            //    await Connection.StartAsync();
            //};

            Connection.On<string>("Imprime", (string etiqueta) =>
            {
                Console.WriteLine(GetTime() + " Etiqueta impressa: " + etiqueta);
            });

            try
            {
                Connection.StartAsync().Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine(GetTime() + " Erro: " + ex.Message);
            }
            finally 
            {
                var task = Connection.InvokeAsync("HandShake", new[] { "Filial 1" });
                while (!task.IsCompleted)
                {
                    Thread.Sleep(10000);
                    Console.WriteLine(GetTime() + " Trying to handshake with the server...");
                }


                var console = Console.ReadKey();
                while (console.Key != ConsoleKey.Escape)
                {
                    console = Console.ReadKey();
                }
                
            }

        }

        //public static async RunTask(string unidade)
        //{ 
            
        //}

        public static string GetTime()
        {
            return string.Format("{0} -", DateTime.UtcNow.ToString("dd-MM-yyyy HH:mm:ss.ffffff",
                                            CultureInfo.InvariantCulture));
        }
    }
}
