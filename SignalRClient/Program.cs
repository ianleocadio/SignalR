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
            var credential = Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes("Filial 3" + ":" + "Filial3Password"));

            Connection = new HubConnectionBuilder()
                   .WithUrl("http://localhost:5000/ImpressaoPostoColeta", options =>
                   {
                       options.Headers.Add("Authorization", $"Basic {credential}");
                   })
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
                var task = Connection.InvokeAsync("HandShake", new { Unidade = "Filial 3" });
                //throw new Exception("HAHAHHAHAHA");
                task.Wait();
                while (!task.IsCompleted)
                {
                    Console.WriteLine(GetTime() + " Trying to handshake with the server...");
                    Thread.Sleep(2000);
                }

                if (task.IsCompletedSuccessfully)
                    Console.WriteLine("Task [" + task.Id + "] completed");
                else if (task.IsCanceled)
                    Console.WriteLine("Task [" + task.Id + "] Canceled");
                else if (task.IsFaulted)
                    Console.WriteLine("Task [" + task.Id + "] Faulted " + task?.Exception.Message);
                else
                    Console.WriteLine("Task [" + task.Id + "] unhandled error");


                var console = Console.ReadKey();
                while (console.Key != ConsoleKey.Escape)
                {
                    console = Console.ReadKey();
                }

            }

        }


        public static string GetTime()
        {
            return string.Format("{0} -", DateTime.UtcNow.ToString("dd-MM-yyyy HH:mm:ss.ffffff",
                                            CultureInfo.InvariantCulture));
        }
    }
}
