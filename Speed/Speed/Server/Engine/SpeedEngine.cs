using Speed.Shared.Models;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;
using System.Collections.Concurrent;

namespace Speed.Server.Engine {
    public class SpeedEngine {

        //private static readonly ConcurrentBag<GameEngine> games = new ConcurrentBag<GameEngine>();
        private static readonly GameEngine game = new();

        public SpeedEngine() {
            var cardList = game.Deck.getCards();
            Shuffle(ref cardList);
            game.Deck.setCards(cardList);
        }

        public async Task DealDeck(Hub hub) {

            if (game is null) {
                Console.WriteLine("The game is null");
                return;
            }

            var cardList = game.Deck.getCards();
            await hub.Clients.All.SendAsync("ReceiveDeck", "test2", cardList);

        }

        public async Task RequestDeck(Hub hub) {


            if (game is null) {
                Console.WriteLine("The game is null");
                return;
            }

            var cardList = game.Deck.getCards();
            await hub.Clients.Caller.SendAsync("ReceiveDeck", game.Deck.GetHashCode().ToString(), cardList);

        }

        public async Task RequestHand(Hub hub, bool playerOne) {

            Console.WriteLine("We are at the speedengine");
            if (playerOne) {
                await hub.Clients.Caller.SendAsync("ReceiveHand", game.P1Hand);
            } else {
                await hub.Clients.Caller.SendAsync("ReceiveHand", game.P2Hand);
            }
        }


        public void Shuffle(ref List<Card> cardPile) {
            Random rng = new Random();
            int size = cardPile.Count();
            while (size > 1) {
                size--;
                int swapSpot = rng.Next(size + 1);
                Card a = cardPile[swapSpot];
                cardPile[swapSpot] = cardPile[size];
                cardPile[size] = a;
            }
        }


    }
}
