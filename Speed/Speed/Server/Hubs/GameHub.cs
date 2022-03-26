
using System;
using System.Threading;
using Microsoft.AspNetCore.SignalR;


namespace Speed.Server.Hubs {
    public class GameHub : Hub {

        public override async Task OnConnectedAsync() {
            await SendMessage("", "User Connected");
            await base.OnConnectedAsync();
            Console.WriteLine("This is getting here");

        }

        public async Task SendMessage(string user, string message) {

            Console.WriteLine(message);

            await Clients.All.SendAsync("ReceiveMessage", user, message);
            

        }





    }
}
