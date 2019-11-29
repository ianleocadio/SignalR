using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using SignalRServer.Caller.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SignalRClient.Logging;

namespace SignalRServer.SignalR.Hubs
{

    [Authorize]
    public class MainHub : Hub
    {

        private readonly ILogger<MainHub> _logger = LoggerProvider.GetLogger<MainHub>();

        #region DTO's para mapear paramêtros da requisição
        public class HandShakeRequestParameters
        {
            public string Unidade { get; set; }
        }

        public class FalhaImpressaoRequestParameters
        {
            public string Unidade { get; set; }
            public string Etiqueta { get; set; }
        }
        #endregion

        public void HandShake(HandShakeRequestParameters req)
        {
            if (req?.Unidade == null)
                return;

            _logger.LogInformation("[{time}] {Unidade} Informou que está aberta a solicitações", DateTimeOffset.Now, req.Unidade);

            // Adiciona Caller
            Program.ImprimeCallerController.AddCaller(new ImprimeCaller(req.Unidade, Context.User.Identity.Name, Clients.Caller, true));

            _logger.LogInformation("[{time}] Caller: {Unidade} adicionado", DateTimeOffset.Now, req.Unidade);

            Program.ImprimeCallerController.RunInstance();

        }

        public void FalhaImpressao(FalhaImpressaoRequestParameters req)
        {
            if (req?.Unidade == null || req?.Etiqueta == null)
                return;

            _logger.LogWarning("[{time}] {Unidade} Informou que houve erro ao imprimir a etiqueta: {etiqueta}", DateTimeOffset.Now, req.Unidade, req.Etiqueta);

            // Tratamentos...
            // -> Reenviar pendência ?
        }

        public override async Task OnConnectedAsync()
        {

            _logger.LogWarning("[{time}] Connected: {Client}", DateTimeOffset.Now, Context.User.Identity.Name);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            _logger.LogWarning("[{time}] Disconnected: {Client}", DateTimeOffset.Now, Context.User.Identity.Name);

            Program.ImprimeCallerController.DisableCaller(Context.User.Identity.Name);

            await base.OnDisconnectedAsync(ex);
        }

    }
}