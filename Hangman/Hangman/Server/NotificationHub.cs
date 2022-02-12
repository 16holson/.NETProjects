using Microsoft.AspNetCore.SignalR;

namespace Hangman.Server
{
    public class NotificationHub : Hub
    {
        public async Task SendMessage(string user, string hashedSaltedPass, string salt)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, hashedSaltedPass, salt);
        }
    }
}
