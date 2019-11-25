using Microsoft.AspNetCore.SignalR.Client;
using System;

namespace SignalRClient
{
    class Program
    {

        // "Método REST/Business"
        static void Main(string[] args)
        {
            // Servidor WEBSERVICE tenta abrir conexão com o servidor de impressão
            var connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/ImpressaoPostoColeta")
                .Build();

            try
            {
                // Abre conexão
                connection.StartAsync().Wait();

                // Invoca evento de impressão passando parametros necessários
                connection.InvokeCoreAsync("Imprime", args: new[] { "Report teste", "DataSet teste", "Endereco teste" });
                

                // Pode escutar 
                connection.On("SucessoImpressao",() => Console.WriteLine("Imprimiu com sucesso!"));
                connection.On("ErroImpressao", (Exception ex) => Console.WriteLine(ex));
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
