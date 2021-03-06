﻿using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using SignalRLibrary;
using System;
using System.Composition;
using System.Threading.Tasks;

namespace SignalRPrototype.Drivers.drvPluginEtiquetadoraImpressao
{
    [Export(typeof(Plugin))]
    public class EtiquetadoraImpressao : Plugin
    {

        #region Parameters
        public class Parameters
        {
            public string Unidade { get; set; }
            public string Codigo { get; set; }
            public string Descricao { get; set; }
        }
        #endregion

        #region Evento Imprime
        [Event("ImprimeEtiqueta", ResponseHandlerMethodName = "ImprimeEtiquetaResponse")]
        public Task ImprimeEtiqueta(Parameters p)
        {

            Task.Delay(3000).Wait();

            Logger.LogInformation("Impressão etiqueta: {Codigo}", p.Codigo);

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

        public Task ImprimeEtiquetaResponse(HubConnection conn, Exception ex, Parameters p)
        {
            if (ex == null)
                Logger.LogInformation("Sucesso impressão etiqueta: {Codigo}", p.Codigo);
            else
                Logger.LogError("Falha impressão etiqueta: {Codigo}", p.Codigo);

            conn.InvokeAsync("DefaultResponse", p.Unidade, p.Codigo, ex?.Message);

            return Task.CompletedTask;
        }
        #endregion

    }
}
