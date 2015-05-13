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

        public SortedDictionary<string, GameLogic> gameSessions = new SortedDictionary<string, GameLogic>();

        private string genericSession = "default";

        private static GameHandler instance = null;
        private static readonly object padlock = new object();

       

        GameHandler()
        {
            gameSessions.Add(genericSession, new GameLogic());
        }

        public void checkForMatches()
        { 
            if (waitingPlayers.Count >= 2)
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

        public void creatNewSession(string sessionName)
        {
            gameSessions.Add(sessionName, new GameLogic());
        }

        public void joinSession(string SessionToJoin, Player joiningPlayer)
        {
            gameSessions[SessionToJoin].addPlayerToGame(joiningPlayer);
        }

        public Player getPlayer(string session, int number) 
        {
            return gameSessions[session].getPlayer(number);
        }

        public void setPlayerPosition(string session, string IP, int x, int y)
        {
            gameSessions[session].setPlayerPosition(IP, x, y);
        }

        public void setReady(string session, string IP)
        {
            gameSessions[session].setReady(IP);
        }

        public void setScore(string session, string IP, string score)
        {
            gameSessions[session].setScore(IP, score);
        }

        public int getPlayerNumber(string session, string IP)
        {
            return gameSessions[session].getPlayerNumber(IP);
        }

        public void update() 
        {

            gameSessions[genericSession].update();
            //Change this to cycle the dictionary instead. 
           // foreach (var games in activeGames)
            //{
              // games.update();
           // }
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
