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

        private bool fourPlayer;
        private bool gameHasStarted = false;
        private int playerCount = 0;

        public string playerOneScore;
        public string playerTwoScore;
        

        public GameLogic() 
        { 
            
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
