using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Speed.Shared.Models;

namespace Speed.Server.Engine
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
            if (middleDeck.Count != 0)
            {
                //Player 1
                if (selectedCard1 != null)
                {
                    if ((int)selectedCard1.Value + 1 == (int)middleDeck.Last().Value || (int)selectedCard1.Value - 1 == (int)middleDeck.Last().Value
                       || ((int)selectedCard1.Value == 13 && (int)middleDeck.Last().Value == 1) || ((int)selectedCard1.Value == 1 && (int)middleDeck.Last().Value == 13))
                    {
                        middleDeck.Add(selectedCard1);
                        SpeedEngine.game.P1Hand.Remove(selectedCard1);
                        //Player won
                        if (SpeedEngine.game.P1Hand.Count == 0)
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
                if (selectedCard2 != null)
                {
                    if ((int)selectedCard2.Value + 1 == (int)middleDeck.Last().Value || (int)selectedCard2.Value - 1 == (int)middleDeck.Last().Value
                       || ((int)selectedCard2.Value == 13 && (int)middleDeck.Last().Value == 1) || ((int)selectedCard2.Value == 1 && (int)middleDeck.Last().Value == 13))
                    {
                        middleDeck.Add(selectedCard2);
                        SpeedEngine.game.P2Hand.Remove(selectedCard2);
                        //Player won
                        if (SpeedEngine.game.P2Hand.Count == 0)
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
            if (SpeedEngine.game.P1Hand.Count < 5 && SpeedEngine.game.P1Draw.Count != 0)
            {
                SpeedEngine.game.P1Hand.Add(SpeedEngine.game.P1Draw.First());
                SpeedEngine.game.P1Draw.RemoveAt(0);
            }
        }
        public void addCards2()
        {
            if (SpeedEngine.game.P2Hand.Count < 5 && SpeedEngine.game.P2Draw.Count != 0)
            {
                SpeedEngine.game.P2Hand.Add(SpeedEngine.game.P2Draw.First());
                SpeedEngine.game.P2Draw.RemoveAt(0);
            }
        }

        public void moveMiddle(String player)
        {
            //Maybe check if there are valid moves
            if (SpeedEngine.game.Mid1Draw.Count != 0)
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
                    SpeedEngine.game.Mid1Discard.Add(SpeedEngine.game.Mid1Draw.First());
                    SpeedEngine.game.Mid2Discard.Add(SpeedEngine.game.Mid2Draw.First());
                    SpeedEngine.game.Mid1Draw.RemoveAt(0);
                    SpeedEngine.game.Mid2Draw.RemoveAt(0);
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
                    SpeedEngine.game.Shuffle(SpeedEngine.game.Mid1Discard);
                    SpeedEngine.game.Shuffle(SpeedEngine.game.Mid2Discard);
                    foreach (Card card in SpeedEngine.game.Mid1Discard)
                    {
                        SpeedEngine.game.Mid1Draw.Add(card);
                    }
                    foreach (Card card in SpeedEngine.game.Mid2Discard)
                    {
                        SpeedEngine.game.Mid2Draw.Add(card);
                    }
                    p1Ready = "none";
                    p2Ready = "none";
                }
            }
        }
        #endregion
    }
}
