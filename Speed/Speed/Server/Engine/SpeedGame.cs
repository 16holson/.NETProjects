using Speed.Shared.Models;

namespace Speed.Server.Engine {
    public class SpeedGame {


        public Deck Deck { get; set; }

        public SpeedGame() {
            Deck = new Deck();
            Deck.BuildDeck(); 

        }

    }
}
