using Speed.Shared.Models;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;
using System.Collections.Concurrent;

namespace Speed.Server.Engine
{
    public class SpeedEngine
    {

        //private static readonly ConcurrentBag<GameEngine> games = new ConcurrentBag<GameEngine>();
        public static readonly GameEngine game = new();

        public SpeedEngine()
        {
            var cardList = game.Deck.getCards();
            Shuffle(cardList);
            game.Deck.setCards(cardList);
        }

        public async Task DealDeck(Hub hub)
        {

            if (game is null)
            {
                Console.WriteLine("The game is null");
                return;
            }

            var cardList = game.Deck.getCards();
            await hub.Clients.All.SendAsync("ReceiveDeck", "test2", cardList);

        }

        public async Task RequestDeck(Hub hub)
        {


            if (game is null)
            {
                Console.WriteLine("The game is null");
                return;
            }

            var cardList = game.Deck.getCards();
            await hub.Clients.All.SendAsync("ReceiveDeck", game.Deck.GetHashCode().ToString(), cardList);

        }

        public async Task RequestHand(Hub hub, bool playerOne)
        {

            Console.WriteLine("We are at the speedengine");
            if (playerOne)
            {
                await hub.Clients.Caller.SendAsync("ReceiveHand", game.P1Hand);
            }
            else
            {
                await hub.Clients.Caller.SendAsync("ReceiveHand", game.P2Hand);
            }
        }

        public async Task RequestGame(Hub hub)
        {
            await hub.Clients.All.SendAsync("ReceiveGame", game.P1Draw, game.P2Draw, game.Mid1Draw, game.Mid2Draw, game.Mid1Discard, game.Mid2Discard, game.P1Hand, game.P2Hand);
        }

        //Players click on a card in there hand
        public async Task SetSelectedCard(Hub hub, Card selectedCard, bool playerOne)
        {
            if (playerOne)
            {
                game.GameService.onHandClick1(selectedCard);
            }
            else
            {
                game.GameService.onHandClick2(selectedCard);
            }
            await hub.Clients.All.SendAsync("ReceiveSelectedCards", game.P1Hand, game.P2Hand);
        }

        //Players click on middle discard with a selected card(update middle and players hand
        public async Task OnMiddleClick(Hub hub, List<Card> middleDeck, string midNum)
        {

            game.GameService.onPlay(middleDeck, midNum);
            if (midNum == "one")
            {
                await hub.Clients.All.SendAsync("ReceiveMiddleDeck", game.Mid1Discard, game.P1Hand, game.P2Hand, "one");
            }
            else
            {
                await hub.Clients.All.SendAsync("ReceiveMiddleDeck", game.Mid2Discard, game.P1Hand, game.P2Hand, "two");
            }
        }

        //Players click to add card to hand
        public async Task OnMoreCards(Hub hub, bool playerOne)
        {
            if (playerOne)
            {
                game.GameService.addCards1();
                await hub.Clients.Caller.SendAsync("ReceiveDeckandHand", game.P1Hand, game.P1Draw, game.P2Hand);
            }
            else
            {
                game.GameService.addCards2();
                await hub.Clients.Caller.SendAsync("ReceiveDeckandHand", game.P2Hand, game.P2Draw, game.P1Hand);
            }
        }

        //Move outer middle to inner middle deck
        public async Task OnOuterMiddle(Hub hub, bool playerOne)
        {
            if (playerOne)
            {
                game.GameService.moveMiddle("one");
            }
            else
            {
                game.GameService.moveMiddle("two");
            }
            await hub.Clients.All.SendAsync("ReceiveMiddleDecks", game.Mid1Discard, game.Mid2Discard, game.Mid1Draw, game.Mid2Draw);
        }


        public void Shuffle(List<Card> cardPile)
        {
            Random rng = new Random();
            int size = cardPile.Count();
            while (size > 1)
            {
                size--;
                int swapSpot = rng.Next(size + 1);
                Card a = cardPile[swapSpot];
                cardPile[swapSpot] = cardPile[size];
                cardPile[size] = a;
            }
        }


    }
}

