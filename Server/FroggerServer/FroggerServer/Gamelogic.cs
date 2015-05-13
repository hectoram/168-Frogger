using System;
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

        private int playerCount = 0;

        public int playerOneScore;
        public int playerTwoScore;

        private bool winnerSet = false;
        public int winner = 0;

        bool firstScore = false;
        bool secondScore = false;
        public bool bothScoresSet;
        

        public GameLogic() 
        {
            gameHasStarted = false;
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
            if (first.IP.Equals(IP))
                return true;
            else if (second.IP.Equals(IP))
                return true;
            else if (third.IP.Equals(IP))
                return true;
            else if (fourth.IP.Equals(IP))
                return true;

            return false;
        }

        public void setPlayerPosition(string IP, int x, int y)
        {
            int position = getPlayerNumber(IP);

            if(position == 1)
            {
                positions[0] = x;
                positions[1] = y;
            }
            else if (position == 2)
            {
                positions[2] = x;
                positions[3] = y;
            }
            else if (position == 3)
            {
                positions[4] = x;
                positions[5] = y;
            }
            else if (position == 4)
            {
                positions[6] = x;
                positions[7] = y;
            }

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

        public Player getPlayer(int playerNumber)
        {
            if (playerNumber == 1)
                return first;
            else if (playerNumber == 2)
                return second;

            return null;
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

        public void addMessage(string username)
        { 
            
        }
    }
}
