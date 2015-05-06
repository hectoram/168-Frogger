using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FroggerServer
{
    class GameHandler
    {
        List<GameLogic> activeGames = new List<GameLogic>();
        Queue<Player> waitingPlayers = new Queue<Player>();

        private static GameHandler instance = null;
        private static readonly object padlock = new object();

        GameHandler()
        {
          
        }

        public void checkForMatches()
        { 
            if (NetworkHandler.Instance.connectedPlayers.Count() >= 2)
            {
                
            }
        }

        public void addNewGame(Player one, Player two)
        {
            activeGames.Add(new GameLogic(one, two));
        }

        public void addNewGame(Player one, Player two, Player three, Player four)
        { 
            
        }

        public void update() 
        {
            foreach (var games in activeGames)
            {
               games.update();
            }
        }

        public static GameHandler Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new GameHandler();
                    }
                    return instance;
                }
            }
        }
    }
}
