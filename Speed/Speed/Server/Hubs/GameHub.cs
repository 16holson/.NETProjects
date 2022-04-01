
using System;
using System.Threading;
using Microsoft.AspNetCore.SignalR;
using Speed.Server.Engine;
using Speed.Shared.Models;



namespace Speed.Server.Hubs {
    public class GameHub : Hub {

        private readonly SpeedEngine engine = new();
        //public GameHub(SpeedEngine engine) {
        //    this.engine = engine;
        //}

        public override async Task OnConnectedAsync() {
            await SendMessage("", "User Connected!");
            await base.OnConnectedAsync();


        }


        public async Task SendMessage(string user, string message) {

            Console.WriteLine(message);

            await Clients.All.SendAsync("ReceiveMessage", user, message);
            

        }

        //public async Task SendDeck(string user, List<Card> cards) {

        //    Console.WriteLine("Deck has arrived!");

        //    //await Clients.All.SendAsync("ReceiveDeck", user, cards);

        //    await engine.DealDeck(this);


        //}

        public async Task SendDeck() {

            Console.WriteLine("Deck has arrived!");

            //await Clients.All.SendAsync("ReceiveDeck", user, cards);

            await engine.DealDeck(this);


        }

        /*public async Task SendDeck(string user, string deckString) {

            Console.WriteLine("Deck has arrived");

            await Clients.All.SendAsync("ReceiveDeck", user, deckString);

        }*/





    }
}
