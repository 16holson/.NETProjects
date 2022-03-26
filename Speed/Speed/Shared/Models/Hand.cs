using Speed.Shared.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speed.Shared.Models
{
    /// <summary>
    /// class to handle the collection of cards for each user
    /// </summary>
    public class Hand
    {
        private int playerID;
        private List<Card> playerHand;
        private static int maxHandSize = 5;

        public Hand(int iD)
        {
            playerID = iD;
            playerHand = new List<Card>();
        }

        /// <summary>
        /// adds a card to a player's hand if it wouldn't make it overfull
        /// </summary>
        /// <param name="card"> the card to add </param>
        /// <returns>
        /// 1 if hand is full
        /// </returns>
        public int addCard(Card card)
        {
            if (playerHand.Count < maxHandSize)
            {
                playerHand.Add(card);
                return 0;
            }

            return 1;   // hand is full
        }

        /// <summary>
        /// removes a card from player's hand if it is there
        /// </summary>
        /// <param name="card"> the card to pull </param>
        /// <returns>
        /// 1 if doesn't exist in hand
        /// </returns>
        public int removeCard(Card card)
        {
            foreach (Card c in playerHand)
            {
                if (c.Equals(card))
                {
                    playerHand.Remove(c);
                    return 0;
                }
            }

            return 1; // if it reaches here, card was not in hand
        }
    }
}
