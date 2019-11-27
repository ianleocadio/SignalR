using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using SignalRServer.Caller.Models;
using Microsoft.AspNetCore.SignalR;

namespace SignalRServer.SignalR.Hubs
{

    [Authorize]
    public class MainHub : Hub
    {

        public class HandShakeRequestParameters
        {
            public string Unidade { get; set; }
        }

        public async Task HandShake(HandShakeRequestParameters req)
        {
            if (req?.Unidade == null)
            {
                await Clients.Caller.SendAsync("Recused HandShake");
                return;
            }

            Console.WriteLine(Program.GetTime() + "[MainHub.HandShake] " + req.Unidade + " Informou que está aberta a solicitações");

            // Adiciona Caller
            Program.TesteCallerController.AddCaller(new TesteCaller(req.Unidade, Context.User.Identity.Name, Clients.Caller, true));

            Console.WriteLine(Program.GetTime() + "[MainHub.HandShake] Caller: " + req.Unidade + " adicionado");

            await Clients.Caller.SendAsync("HandShaked");

            Program.TesteCallerController.RunInstance();

        }

        public override async Task OnConnectedAsync()
        {
            Console.WriteLine("Connected: " + Context.User.Identity.Name);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            Console.WriteLine("Disconnected: " + Context.User.Identity.Name);

            Program.TesteCallerController.DisableCaller(Context.User.Identity.Name);

            await base.OnDisconnectedAsync(ex);
        }

    }
}