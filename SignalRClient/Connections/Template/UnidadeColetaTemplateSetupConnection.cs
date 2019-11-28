using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using SignalRClient.Logging;

namespace SignalRClient.Connections.Template
{
    public class UnidadeColetaTemplateSetupConnection : AbstractTemplateSetupConnection
    {

        private static readonly ILogger<UnidadeColetaTemplateSetupConnection> _logger = LoggerProvider.GetLogger<UnidadeColetaTemplateSetupConnection>();

        public UnidadeColetaTemplateSetupConnection(HubConnection connection) : base(connection)
        {

        }

        public override void SetupConnectionEvents(HubConnection connection)
        {
            connection.On<string>("Imprime", (string etiqueta) =>
            {
                _logger.Log(LogLevel.Information, "teste {param}", "teste2");

                _logger.LogInformation("[{time}] Etiqueta a ser impressa: {etiqueta}", DateTimeOffset.Now, etiqueta);
                
                Thread.Sleep(3000);
                
                _logger.LogInformation("[{time}] Etiqueta impressa: {etiqueta}", DateTimeOffset.Now, etiqueta);

                connection.InvokeAsync("ImpressaoSucesso", new { Unidade = "Filial 3", Etiqueta = etiqueta });
            });
        }

        public override void SetupOnCloseConnection(HubConnection connection)
        {
            connection.Closed += async (error) =>
            {
                await Task.Delay(10000);
                await connection.StartAsync();
            };
        }

        public override void SetupOnReconnectConnection(HubConnection connection)
        {
            base.SetupOnReconnectConnection(connection);
        }

        public override void SetupOnReconnectingConnection(HubConnection connection)
        {
            base.SetupOnReconnectingConnection(connection);
        }
    }
}
