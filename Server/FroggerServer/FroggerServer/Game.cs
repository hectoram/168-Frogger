using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FroggerServer
{
    class Game
    {
        Player p1;
        Player p2;
        Player p3;
        Player p4;
        GameLogic gameHandler;

        Game(Player playerOne, Player playerTwo, Player playerThree, Player playerFour)
        {
            p1 = playerOne;
            p2 = playerTwo;
            p3 = playerThree;
            p4 = playerFour;
            gameHandler = new GameLogic();
        }

        Game(Player playerOne, Player playerTwo)
        {
            p1 = playerOne;
            p2 = playerTwo;
            gameHandler = new GameLogic();
        }

        public void update() 
        {
            gameHandler.update();
        }
    }
}
