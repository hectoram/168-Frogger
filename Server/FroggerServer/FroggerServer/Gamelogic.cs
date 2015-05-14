﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FroggerServer
{
    class GameLogic
    {
        //Add some type of datastructure to hold messages of the players. Map maybe?
        public Player first;
        public Player second;
        Player third;
        Player fourth;

        int[] positions = new int[8];

        private bool fourPlayer;

        private bool gameHasStarted;
        private bool gameIsOver;

        private int playerCount = 0;

        public int playerOneScore;
        public int playerTwoScore;

        private bool winnerSet = false;
        public int winner = 0;

        bool firstScore = false;
        bool secondScore = false;
        public bool bothScoresSet;

        public bool p1Ready = false;
        public bool p2Ready = false;
        

        public GameLogic() 
        {
            gameHasStarted = false;
            positions[0] = 4-7;
            positions[1] = 13-7;

            positions[2] = 10-7;
            positions[3] = -(13)-5;

            positions[4] = 0-7;
            positions[5] = -(0)-5;

            positions[6] = 0-7;
            positions[7] = -(0)-5;
        }

        public GameLogic(Player one, Player two, Player three, Player four) 
        {
            first = one;
            second = two;
            third = three;
            fourth = four;
            fourPlayer = true;
        }

        public GameLogic(Player one, Player two)
        {
            first = one;
            second = two;
            fourPlayer = false;
        }

        public bool playerIsInGame(string IP) 
        {
            try
            {
                if (first.IP.Equals(IP))
                    return true;
                else if (second.IP.Equals(IP))
                    return true;
                else if (third.IP.Equals(IP))
                    return true;
                else if (fourth.IP.Equals(IP))
                    return true;
            }
            catch (Exception e)
            {
                return false;
            }
            return false;
        }

        public void setPlayerPosition(string IP, int x, int y)
        {
            //Sub -7 from x to get translated x
            //Negate and sub 5 for y
            int position = getPlayerNumber(IP);

            if(position == 1)
            {
                positions[0] = x-7;
                positions[1] = -y-5;
            }
            else if (position == 2)
            {
                positions[2] = x-7;
                positions[3] = -y-5;
            }
            else if (position == 3)
            {
                positions[4] = x-7;
                positions[5] = -y-5;
            }
            else if (position == 4)
            {
                positions[6] = x-7;
                positions[7] = -y-5;
            }

        }

        public string getPlayerPositions(int pos)
        {
            if (pos== 1)
                return positions[0] + "," + positions[1];
            else if (pos == 2)
                return positions[2] + "," + positions[3];
            else if (pos == 3)
                return positions[4] + "," + positions[5];
            else if (pos == 4)
                return positions[6] + "," + positions[7];

            return "";
        }

        public void setScore(string IP, string myScore)
        {
            if (first.IP.Equals(IP))
            {
                playerOneScore = int.Parse(myScore);
                firstScore = true;
            }
            else if (second.IP.Equals(IP))
            {
                playerTwoScore = int.Parse(myScore);
                secondScore = true;
            }

            if (!winnerSet && firstScore && secondScore) 
            {
                winnerSet = !winnerSet;
                bothScoresSet = true;

                if (playerOneScore > playerTwoScore)
                    winner = 1;
                else if (playerTwoScore > playerOneScore)
                    winner = 2;
                else
                    winner = 3;
            }
            //else if (third.IP.Equals(IP)) 
           // else if (fourth.IP.Equals(IP))
                
        }

        public void setReady(string IP) 
        {
            try
            {
                if (first.IP.Equals(IP))
                    p1Ready = true;
                else if (second.IP.Equals(IP))
                    p2Ready = true;
            }
            catch (Exception e)
            { 
                //Do nothing. Second player probably isn't connected. 
            }
        }

        public bool addPlayerToGame(Player toAdd) 
        {
            if (playerCount != 3)
            {
                if (playerCount == 0)
                    first = toAdd;
                else if (playerCount == 1)
                    second = toAdd;
                else if (playerCount == 2)
                    third = toAdd;
                else if (playerCount == 3)
                    fourth = toAdd;

                if (playerCount < 3)
                    playerCount++;
                
                return true;
            }
            else
                return false;

        }

        public int getPlayerNumber(string IP)
        {
            if (first.IP.Equals(IP))
                return 1;
            else if (second.IP.Equals(IP))
                return 2;
            else if (third.IP.Equals(IP))
                return 3;
            else if (fourth.IP.Equals(IP))
                return 4;

            return 0;
        }

        public int getPlayerCount()
        {
            return playerCount;
        }

        public void update() 
        {
            if (playerCount == 2)
            {
                gameHasStarted = true;
                Console.WriteLine("We have two players and the game has started.");
            }
        }
    }
}
