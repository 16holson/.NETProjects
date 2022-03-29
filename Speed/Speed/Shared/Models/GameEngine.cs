using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speed.Shared.Models
{
    internal class GameEngine
    {

        public void deal()
        {
            // add 5 cards to each player's hand
            for (var i = 0; i < 5; i++)
            {
                p1Hand.addCard(deck.DrawTopCard());
                p2Hand.addCard(deck.DrawTopCard());
            }

            // add 15 cards to each player's draw pile
            for (var i = 0; i < 15; i++)
            {
                p1Draw.addCard(deck.DrawTopCard());
                p2Draw.addCard(deck.DrawTopCard());
            }

            // add 6 cards to each middle draw pile
            for (var i = 0; i < 6; i++)
            {
                mid1Draw.addCard(deck.DrawTopCard());
                mid2Draw.addCard(deck.DrawTopCard());
            }


            // Test all Decks and Hands

            //// Player 1 Hand
            //Console.WriteLine("Player 1's Hands contains: ");
            //foreach (Card card in p1Hand.playerHand)
            //{
            //    Console.WriteLine(card.ToString());
            //}
            //// Player 2 Hand
            //Console.WriteLine("Player 2's Hands contains: ");
            //foreach (Card card in p2Hand.playerHand)
            //{
            //    Console.WriteLine(card.ToString());
            //}
            //// Player 1 Draw pile
            //Console.WriteLine("Player 1's Draw pile contains: ");
            //foreach (Card card in p1Draw.cards)
            //{
            //    Console.WriteLine(card.ToString());
            //}
            //// Player 2 Draw pile
            //Console.WriteLine("Player 2's Draw pile contains: ");
            //foreach (Card card in p2Draw.cards)
            //{
            //    Console.WriteLine(card.ToString());
            //}
            //// Middle Draw pile 1
            //Console.WriteLine("Middle Draw pile 1 contains: ");
            //foreach (Card card in mid1Draw.cards)
            //{
            //    Console.WriteLine(card.ToString());
            //}
            //// Middle Draw pile 2
            //Console.WriteLine("Middle Draw pile 2 contains: ");
            //foreach (Card card in mid2Draw.cards)
            //{
            //    Console.WriteLine(card.ToString());
            //}
            //// Middle Discard pile 1
            //Console.WriteLine("Middle Discard pile 1 contains: ");
            //foreach (Card card in mid1Discard.cards)
            //{
            //    Console.WriteLine(card.ToString());
            //}
            //// Middle Discard pile 2
            //Console.WriteLine("Middle Discard pile 2 contains: ");
            //foreach (Card card in mid2Discard.cards)
            //{
            //    Console.WriteLine(card.ToString());
            //}
        }
    }
}
