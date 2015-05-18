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

        public void messageHandle(string session,string message ,string IP)
        {
            int myPosition = gameSessions[session].getPlayerNumber(IP);
            string toSend = "chat-message," + gameSessions[session].getPlayer(myPosition).getUserName() + "," + message + "<EOF>";
            //Try to send the message and if they return null then you do nothing. 
            try
            {
                if (myPosition != 1)
                    NetworkHandler.Instance.sendMessage(gameSessions[session].getPlayer(1).IP, toSend);
            }
            catch (Exception e)
            { 
                
            }
            try
            {
                if (myPosition != 2)
                    NetworkHandler.Instance.sendMessage(gameSessions[session].getPlayer(2).IP, toSend);
            }
            catch (Exception e)
            {

            }
            try
            {
                if (myPosition != 3)
                    NetworkHandler.Instance.sendMessage(gameSessions[session].getPlayer(3).IP, toSend);
            }
            catch (Exception e)
            {

            }
            try
            {
                if (myPosition != 4)
                    NetworkHandler.Instance.sendMessage(gameSessions[session].getPlayer(4).IP, toSend);
            }
            catch (Exception e)
            {

            }
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
