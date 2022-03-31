using Speed.Shared.Models;
using System;
using System.Threading;
using Microsoft.AspNetCore.SignalR;


namespace Speed.Server.Hubs {
    public class GameHub : Hub {

        public GameEngine gameEngine;
        private string player1;
        private string player2;
        private int numPlayers;

        public GameHub()
        {
            gameEngine = new GameEngine();
        }

        public override async Task OnConnectedAsync() {
            
            await Groups.AddToGroupAsync(Context.ConnectionId, "Players");
            Console.WriteLine(Context.ConnectionId);
            Console.WriteLine(Groups.ToString());
            await SendMessage("", "User Connected");
            await base.OnConnectedAsync();
            Console.WriteLine("This is getting here");
            

        }

        public async Task SendMessage(string user, string message) {

            Console.WriteLine(message);

            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        // public async Task LogIn() to set the player1 or player2 as the connectionId and the num players
        public async Task Login()
        {
            // increment number of users
            // set the player 
            numPlayers++;
            if (numPlayers == 1) 
            {
                player1 = Context.ConnectionId;
                await Clients.All.SendAsync("SetClientInfo", player1, numPlayers);
                // await Clients.Client(Context.ConnectionId).SendAsync("SetUserInfo", player1, numPlayers);
                // await Clients.Client(Context.ConnectionId).SendAsync("SetClientInfo", player1, "Welcome new player " + player1);
            }
            else if (numPlayers == 2)
            {
                player2 = Context.ConnectionId;
                await Clients.Client(Context.ConnectionId).SendAsync("SetClientInfo", player2, numPlayers);
            }
            else
            {
                return;
            }
            
        }



    }
}
