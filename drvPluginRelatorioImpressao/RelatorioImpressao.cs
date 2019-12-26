using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using SignalRLibrary;
using System;
using System.Composition;
using System.Threading.Tasks;

namespace drvPluginRelatorioImpressao
{

    [Export(typeof(Plugin))]
    public class RelatorioImpressao : Plugin
    {
        public class Parameters
        {
            public string Unidade { get; set; }
            public string Codigo { get; set; }
            public string Descricao { get; set; }
        }


        #region Evento ImprimeRelatorio
        [Event("ImprimeRelatorio", ResponseHandlerMethodName = "ImprimeRelatorioResponse")]
        public Task ImprimeRelatorio(Parameters p)
        {
            Task.Delay(3000).Wait();

            Logger.LogInformation("Impressão relatório: {Codigo}", p.Codigo);

            Random r = new Random();
            var n = r.Next(0, 2);
            if (n == 1)
            {
                return Task.CompletedTask;
            }
            else
            {
                return Task.FromException(new Exception("Erro"));
            }

        }

        public Task ImprimeRelatorioResponse(HubConnection conn, Exception ex, Parameters p)
        {
            if (ex == null)
                Logger.LogInformation("Sucesso impressão Relatorio: {Codigo}", p.Codigo);
            else
                Logger.LogError("Falha impressão Relatorio: {Codigo}", p.Codigo);

            conn.InvokeAsync("DefaultResponse", p.Unidade, p.Codigo, ex?.Message);

            return Task.CompletedTask;
        }
        #endregion
    }
}
