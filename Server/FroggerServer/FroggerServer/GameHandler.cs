using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace FroggerServer
{
    class GameHandler
    { 
        public SortedDictionary<string, GameLogic> gameSessions = new SortedDictionary<string, GameLogic>();

        //Maps IP to the game session name
        public SortedDictionary<string, string> playerSessionNames = new SortedDictionary<string, string>();

        private List<string> toRemove = new List<string>();
        private bool itemsToRemove = false;
        private string genericSession = "default"; 

        private static GameHandler instance = null;
        private static readonly object padlock = new object();
        private static readonly Mutex mutexLock = new Mutex();

       

        GameHandler()
        {
            gameSessions.Add(genericSession, new GameLogic());
        }

        public void addNewGame(Player one, Player two, Player three, Player four)
        { 
            
        }

        public void handleDisconnectedPlayer(string IP)
        {
            foreach (var entry in gameSessions)
            {
                if (entry.Value.playerIsInGame(IP))
                {
                    entry.Value.handleDissconnectedPlayer(IP);
                }

            }
        }

        public bool playerInSession(string IP)
        {
            return playerSessionNames.ContainsKey(IP);
        }

        public void creatNewSession(string sessionName)
        {
            gameSessions.Add(sessionName, new GameLogic());
        }

        public bool joinSession(string SessionToJoin, Player joiningPlayer)
        {
            
            mutexLock.WaitOne();
            try
            {   
                if(SessionToJoin == "")
                {
                    if (gameSessions.ContainsKey(genericSession))
                    {
                        if (gameSessions[genericSession].addPlayerToGame(joiningPlayer))
                        {
                            playerSessionNames.Add(joiningPlayer.IP, SessionToJoin);
                            return true;
                        }
                        else
                            return false;
                    }
                    else 
                    {
                        creatNewSession(genericSession);
                        gameSessions[genericSession].setSessionName(genericSession);
                        gameSessions[genericSession].addPlayerToGame(joiningPlayer);
                        playerSessionNames.Add(joiningPlayer.IP, genericSession);
                        return true;
                    }
                }
                else if (gameSessions.ContainsKey(SessionToJoin))
                {
                    if (gameSessions[SessionToJoin].addPlayerToGame(joiningPlayer))
                    {
                        playerSessionNames.Add(joiningPlayer.IP, SessionToJoin);
                        return true;
                    }
                    else
                        return false;
                }
                else
                { 
                    creatNewSession(SessionToJoin);
                    gameSessions[SessionToJoin].setSessionName(SessionToJoin);
                    gameSessions[SessionToJoin].addPlayerToGame(joiningPlayer);
                    playerSessionNames.Add(joiningPlayer.IP, SessionToJoin);
                    return true;
                }
            

            }
            finally
            {
                mutexLock.ReleaseMutex();
            }
        }

        public string getSessionName(string IP)
        {
            //returns the value that IP maps to.
            return playerSessionNames[IP];
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
            try
            {
                gameSessions[session].setScore(IP, score);
            }
            catch (Exception e)
            { 
                //Game is over. No point in trying to set old scores. 
            }
        }

        public void chatMessageHandle(string session,string message ,string IP)
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

        public void endGame(string sessionName)
        {
            toRemove.Add(sessionName);
            itemsToRemove = true;
        }

        private void removeGames()
        {
            mutexLock.WaitOne();
            try
            {
                foreach (var value in toRemove)
                {
                    gameSessions.Remove(value);
                }
                //Clear so other problems don't arise later.
                itemsToRemove = false;
                toRemove.Clear();
            }
            finally
            {
                mutexLock.ReleaseMutex();
            }
        }

        public int getCurrentTime(string session)
        {
            return gameSessions[session].getCurrentTime();
        }

        public void update() 
        {
            mutexLock.WaitOne();
            try
            {
                foreach (var games in gameSessions)
                {
                    games.Value.update();
                }
            }
            finally
            {
                mutexLock.ReleaseMutex();
            }

            //This might cause a lock up. 
            //if (itemsToRemove)
                //removeGames();

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
