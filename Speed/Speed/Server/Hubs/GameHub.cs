
using System;
using System.Threading;
using Microsoft.AspNetCore.SignalR;
using Speed.Server.Engine;
using Speed.Shared.Models;



namespace Speed.Server.Hubs
{

    public static class Connections
    {
        public static List<string> connections = new();
    }

    public class GameHub : Hub
    {

        private readonly SpeedEngine engine;
        public GameHub(SpeedEngine engine)
        {
            this.engine = engine;
        }

        public override async Task OnConnectedAsync()
        {

            Connections.connections.Add(Context.ConnectionId);

            string playername;

            if (Connections.connections.Count > 1)
            {
                playername = "Player 2";
            }
            else
            {
                playername = "Player 1";
            }



            await SendMessage("", $"{ playername } Connected!");
            await setPlayer(playername);

            await RequestGame();
            await base.OnConnectedAsync();


        }

        public async Task setPlayer(string playername)
        {
            await Clients.Caller.SendAsync("SetPlayer", playername);
        }


        public async Task SendMessage(string user, string message)
        {

            Console.WriteLine(message);

            await Clients.All.SendAsync("ReceiveMessage", user, message);


        }


        public async Task GetDeck()
        {

            await engine.DealDeck(this);


        }

        public async Task RequestDeck()
        {

            await engine.RequestDeck(this);


        }


        public async Task RequestHand(bool playerOne)
        {

            await engine.RequestHand(this, playerOne);
        }
        public async Task RequestGame()
        {
            await engine.RequestGame(this);
        }

        public async Task SetSelectedCard(Card selectedCard, bool playerOne)
        {
            await engine.SetSelectedCard(this, selectedCard, playerOne);
        }

        public async Task OnMiddleClick(List<Card> middleDeck, string midNum)
        {
            await engine.OnMiddleClick(this, middleDeck, midNum);
        }

        public async Task OnMoreCards(bool playerOne)
        {
            await engine.OnMoreCards(this, playerOne);
        }
        public async Task OnOuterMiddle(bool playerOne)
        {
            await engine.OnOuterMiddle(this, playerOne);
        }

    }
}
