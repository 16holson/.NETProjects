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
        private Card selectedCard1;
        private Card selectedCard2;
        private Card previousCard1;
        private Card previousCard2;
        #endregion

        #region Constructor
        public GameService()
        {
            p1Ready = "none";
            p2Ready = "none";
            p1Won = "none";
            p2Won = "none";
        }
        #endregion

        #region Methods
        public void onHandClick1(Card selectedCard)
        {
            selectedCard1 = selectedCard;
            selectedCard1.highlight = "0px 12px 22px 1px #00FF00;";
            if (previousCard1 != null && previousCard1 != selectedCard)
            {
                previousCard1.highlight = "";
            }
            previousCard1 = selectedCard;
        }
        public void onHandClick2(Card selectedCard)
        {
            selectedCard2 = selectedCard;
            selectedCard2.highlight = "0px 12px 22px 1px #00FF00;";
            if (previousCard2 != null && previousCard2 != selectedCard)
            {
                previousCard2.highlight = "";
            }
            previousCard2 = selectedCard;
        }

        public void onPlay(List<Card> middleDeck)
        {
            if(middleDeck.Count != 0)
            {
                //Player 1
                if(selectedCard1 != null)
                {
                    if((int)selectedCard1.Value + 1 == (int)middleDeck.Last().Value || (int)selectedCard1.Value - 1 == (int)middleDeck.Last().Value
                       || ((int)selectedCard1.Value == 13 && (int)middleDeck.Last().Value == 1) || ((int)selectedCard1.Value == 1 && (int)middleDeck.Last().Value == 13))
                    {
                        middleDeck.Add(selectedCard1);
                        GameEngine.P1Hand.Remove(selectedCard1);
                        //Player won
                        if (GameEngine.P1Hand.Count == 0)
                        {
                            p1Won = "inline";
                        }
                        selectedCard1.highlight = "";
                        selectedCard1 = null;
                    }
                    else
                    {
                        //Highlight card with red border 
                        //Figure out how to change highlight for 1 sec the change it back
                        selectedCard1.highlight = "";
                        selectedCard1 = null;
                    }
                }
                else
                {
                    //Highlight card with red border 
                    //Figure out how to change highlight for 1 sec the change it back
                }

                //Player 2
                if(selectedCard2 != null)
                {
                    if((int)selectedCard2.Value + 1 == (int)middleDeck.Last().Value || (int)selectedCard2.Value - 1 == (int)middleDeck.Last().Value
                       || ((int)selectedCard2.Value == 13 && (int)middleDeck.Last().Value == 1) || ((int)selectedCard2.Value == 1 && (int)middleDeck.Last().Value == 13))
                    {
                        middleDeck.Add(selectedCard2);
                        GameEngine.P2Hand.Remove(selectedCard2);
                        //Player won
                        if (GameEngine.P2Hand.Count == 0)
                        {
                            p1Won = "inline";
                        }
                        selectedCard2.highlight = "";
                        selectedCard2 = null;
                    }
                    else
                    {
                        //Highlight card with red border 
                        //Figure out how to change highlight for 1 sec the change it back
                        selectedCard2.highlight = "";
                        selectedCard2 = null;
                    }
                }
                else
                {
                    //Highlight card with red border 
                    //Figure out how to change highlight for 1 sec the change it back
                }
            }
        }

        public void addCards1()
        {
            if (GameEngine.P1Hand.Count < 5 && GameEngine.P1Draw.Count != 0)
            {
                GameEngine.P1Hand.Add(GameEngine.P1Draw.First());
                GameEngine.P1Draw.RemoveAt(0);
            }
        }
        public void addCards2()
        {
            if (GameEngine.P2Hand.Count < 5 && GameEngine.P2Draw.Count != 0)
            {
                GameEngine.P2Hand.Add(GameEngine.P2Draw.First());
                GameEngine.P2Draw.RemoveAt(0);
            }
        }

        public void moveMiddle(String player)
        {
            //Maybe check if there are valid moves
            if (GameEngine.Mid1Draw.Count != 0)
            {
                if (player == "one")
                {
                    p1Ready = "inline";
                }
                else if (player == "two")
                {
                    p2Ready = "inline";
                }
                if (p1Ready == "inline" && p2Ready == "inline")
                {
                    GameEngine.Mid1Discard.Add(GameEngine.Mid1Draw.First());
                    GameEngine.Mid2Discard.Add(GameEngine.Mid2Draw.First());
                    GameEngine.Mid1Draw.RemoveAt(0);
                    GameEngine.Mid2Draw.RemoveAt(0);
                    p1Ready = "none";
                    p2Ready = "none";
                }
            }
            else
            {
                //Shuffle the 2 middle discard decks and move to 2 middle draw decks
                if (player == "one")
                {
                    p1Ready = "inline";
                }
                else if (player == "two")
                {
                    p2Ready = "inline";
                }
                if (p1Ready == "inline" && p2Ready == "inline")
                {
                    GameEngine.Shuffle(GameEngine.Mid1Discard);
                    GameEngine.Shuffle(GameEngine.Mid2Discard);
                    foreach (Card card in GameEngine.Mid1Discard)
                    {
                        GameEngine.Mid1Draw.Add(card);
                    }
                    foreach (Card card in GameEngine.Mid2Discard)
                    {
                        GameEngine.Mid2Draw.Add(card);
                    }
                    p1Ready = "none";
                    p2Ready = "none";
                }
            }
        }
        #endregion
    }
}
