using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using SignalRServer.Caller.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SignalRClient.Logging;
using SignalRServer.Caller.Controllers;

namespace SignalRServer.SignalR.Hubs
{

    [Authorize]
    public class MainHub : Hub
    {
        private readonly ILogger<MainHub> _logger;
        private readonly ImprimeCallerController _imprimeCallerController;

        public MainHub(ILogger<MainHub> logger, ImprimeCallerController imprimeCallerController) : base()
        {
            _logger = logger;
            _imprimeCallerController = imprimeCallerController;
        }

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

            _logger.LogInformation("{Unidade} Informou que está aberta a solicitações", req.Unidade);
            // Adiciona Caller
            _imprimeCallerController.AddCaller(new ImprimeCaller(req.Unidade, Context, Clients.Caller, true));

            _logger.LogInformation("Caller: {Unidade} adicionado", req.Unidade);

            _imprimeCallerController.RunInstance();

        }

        public void FalhaImpressao(FalhaImpressaoRequestParameters req)
        {
            if (req?.Unidade == null || req?.Etiqueta == null)
                return;

            _logger.LogWarning("{Unidade} Informou que houve erro ao imprimir a etiqueta: {etiqueta}", req.Unidade, req.Etiqueta);

            // Tratamentos...
            // -> Reenviar pendência ?
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("Connected: {Client}", Context.UserIdentifier);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            _logger.LogWarning("Disconnected: {Client}", Context.UserIdentifier);

            _imprimeCallerController.RemoveCaller(Context.UserIdentifier);

            await base.OnDisconnectedAsync(ex);
        }

    }
}