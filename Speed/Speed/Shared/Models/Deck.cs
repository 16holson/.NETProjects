using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Speed.Shared.Models.Enums;

namespace Speed.Shared.Models {
    public class Deck : IEnumerator, IEnumerable {

        /// <summary>
        /// List that holds are the cards in the deck
        /// </summary>
        public List<Card> cards;

        /// <summary>
        ///Position is used for iEnummerable and Ienumerator interfaces
        /// </summary>
        private int position = -1;

        /// <summary>
        /// helps keep track of which deck this is
        /// -1 for main deck
        /// </summary>
        private int deckId = -1;

        /// <summary>
        /// rng for shuffling
        /// </summary>
        private static Random rng = new Random();

        /// <summary>
        /// Constructor for the deck; 
        /// </summary>
        public Deck() {
            cards = new List<Card>();
        }

        /// <summary>
        /// Builds a new deck of 52 cards from the ground up.
        /// </summary>
        public void BuildDeck() {

            //cards = new List<Card>();

            foreach (CardSuit suit in (CardSuit[])Enum.GetValues(typeof(CardSuit))) {

                foreach (CardValue value in (CardValue[])Enum.GetValues(typeof(CardValue))) {

                    //For each suit and value, create and insert a new Card object.
                    Card newCard = new Card() {
                        Suit = suit,
                        Value = value,
                    };

                    cards.Add(newCard);
                }

            }

        }

        /// <summary>
        /// Returns the amount of cards currently in the deck
        /// </summary>
        /// <returns></returns>
        public int Count() {
            if (cards is not null) {
                return cards.Count();
            }

            return 0;
        }

        /// <summary>
        /// "Draws" the card at the "Top" of the deck and returns it. Top of the deck is referred to as position 0 in the array
        /// </summary>
        /// <returns></returns>
        public Card DrawTopCard() {

            if (cards.Count() > 0) {
                var card = cards[0];
                cards.RemoveAt(0);
                return card;
            }

            return null;

        }

        public IEnumerator GetEnumerator() {
            return (IEnumerator)this;
        }

        public bool MoveNext() {
            position++;
            return (position < cards.Count);
        }

        public void Reset() {
            position = -1;
        }

        public object Current {
            get { return cards[position]; }
        }

        public void addCard(Card card)
        {
            cards.Add(card);
        }

        /// <summary>
        /// Function to shuffle the deck of cards
        /// </summary>
        public void Shuffle()
        {
            int size = cards.Count();
            while (size > 1) {
                size--;
                int swapSpot = rng.Next(size + 1);
                Card a = cards[swapSpot];
                cards[swapSpot] = cards[size];
                cards[size] = a;
            }
        }
    }
}
