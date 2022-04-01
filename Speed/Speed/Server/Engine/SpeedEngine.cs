using Speed.Shared.Models;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;
using System.Collections.Concurrent;

namespace Speed.Server.Engine {
    public class SpeedEngine {

        //private static readonly ConcurrentBag<GameEngine> games = new ConcurrentBag<GameEngine>();
        private static readonly SpeedGame game = new();

        public async Task DealDeck(Hub hub) {

            await hub.Clients.All.SendAsync("RecieveDeck","test2", game.Deck.cards);

        }


    }
}
