using System;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SignalRServer
{
    public class TesteHub : Hub
    {
        public async Task Imprime(object Report, object DataSet, string Endereco)
        {
            Console.WriteLine("===================================================");
            Console.WriteLine("Impressão do relatório: " + Report.ToString());
            Console.WriteLine("    DataSet: " + DataSet.ToString());
            Console.WriteLine("    Endereço: " + Endereco);
            Console.WriteLine("===================================================");

            // Realiza impressão
            // Verifica status de impressão
            // Após términar impressão envia resposta
            // Resposta pode ser positiva ou negativa
            await Clients.Caller.SendAsync("SucessoImpressao");
            //await Clients.Caller.SendAsync("ErroImpressao", Exception error);
        }
    }
}