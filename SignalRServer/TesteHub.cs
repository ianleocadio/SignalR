using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SignalRServer
{
    public class TesteHub : Hub
    {
        public async Task SendMessage(string username, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", username, message);
        }
    }
}