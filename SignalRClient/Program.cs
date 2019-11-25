using Microsoft.AspNetCore.SignalR.Client;
using System;

namespace SignalRClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/testeHub")
                .Build();

            try
            {

                connection.StartAsync().Wait();
                connection.InvokeCoreAsync("SendMessage", args: new[] { "Ian", "Hello World!" });

                connection.On("ReceiveMessage", (string username, string message) => Console.WriteLine(username + ": " + message));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Console.ReadKey();

            }
        }
    }
}
