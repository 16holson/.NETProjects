using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speed.Shared.Models
{
    public class GameService
    {
        #region Properties
        public String p1Ready { get; set; }
        public String p2Ready { get; set; }
        public String p1Won { get; set; }
        public String p2Won { get; set; }
        public String currentPlayer { get; set; }
        #endregion

        #region Local
        private Card selectedCard;
        private Card previousCard;
        private List<Card> selectedHand;
        #endregion

        #region Constructor
        public GameService()
        {
            p1Ready = "none";
            p2Ready = "none";
            p1Won = "none";
            p2Won = "none";
            currentPlayer = "";
        }
        #endregion

        #region Methods
        //instead of passing the List<Card> make the Lists in GameEngine static and only pass the player
        public void onHandClick(List<Card> selectedHand, Card selectedCard, String player)
        {
            this.selectedCard = selectedCard;
            this.selectedHand = selectedHand;
            this.selectedCard.highlight = "0px 12px 22px 1px #00FF00;";
            currentPlayer = player;
            if(previousCard != null && previousCard != selectedCard)
            {
                previousCard.highlight = "";
            }
            previousCard = selectedCard;
        }

        public void onPlay(List<Card> middleDeck)
        {
            //If the play is valid
            if(selectedCard == null || middleDeck.Count == 0)
            {
                //Not valid move
            }
            else if((int)selectedCard.Value + 1 == (int)middleDeck.Last().Value || (int)selectedCard.Value - 1 == (int)middleDeck.Last().Value || ((int)selectedCard.Value == 13 && (int)middleDeck.Last().Value == 1) || ((int)selectedCard.Value == 1 && (int)middleDeck.Last().Value == 13))
            {
                middleDeck.Add(selectedCard);
                selectedHand.Remove(selectedCard);     
                //Player won
                if (selectedHand.Count == 0)
                {
                    if(currentPlayer == "one")
                    {
                        p1Won = "inline";
                    }
                    else if(currentPlayer == "two")
                    {
                        p2Won = "inline";
                    }  
                }
                selectedCard.highlight = "";
                selectedCard = null;
                selectedHand = null;
                currentPlayer = "";
                
            }
            else
            {
                //Highlight card with red border 
                //Figure out how to change highlight for 1 sec the change it back
                selectedCard.highlight = "0px 12px 22px 1px #FF0000;";
                selectedCard.highlight = "";
                
            }
        }

        public void addCards(List<Card> playersDeck, List<Card> playersHand)
        {
            if (playersHand.Count < 5 && playersDeck.Count != 0)
            {
                playersHand.Add(playersDeck.First());
                playersDeck.RemoveAt(0);
            }
        }

        public void moveMiddle(List<Card> outer1Mid, List<Card> outer2Mid, List<Card> inner1Mid, List<Card> inner2Mid, String player)
        {
            //Maybe check if there are valid moves
            if(outer1Mid.Count != 0)
            {
                if(player == "one")
                {
                    p1Ready = "inline";
                }
                else if(player == "two")
                {
                    p2Ready = "inline";
                }
                if(p1Ready == "inline" && p2Ready == "inline")
                {
                    inner1Mid.Add(outer1Mid.First());
                    inner2Mid.Add(outer2Mid.First());
                    outer1Mid.RemoveAt(0);
                    outer2Mid.RemoveAt(0);
                    p1Ready = "none";
                    p2Ready = "none";
                }    
            }
        }
        #endregion
    }
}
