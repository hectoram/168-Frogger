﻿using System;
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
            bool toReturn = false;
            mutexLock.WaitOne();
            try
            {
                if (gameSessions.ContainsKey(SessionToJoin))
                {
                    if (gameSessions[SessionToJoin].addPlayerToGame(joiningPlayer))
                    {
                        playerSessionNames.Add(joiningPlayer.IP, SessionToJoin);
                        toReturn = true;
                    }
                    else
                    {
                        toReturn = false;
                    }
                }
                else
                { 
                    creatNewSession(SessionToJoin);
                    gameSessions[SessionToJoin].addPlayerToGame(joiningPlayer);
                    playerSessionNames.Add(joiningPlayer.IP, SessionToJoin);
                    toReturn = true;
                }

            }
            finally
            {
                mutexLock.ReleaseMutex();
            }
            

            return toReturn;
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
            gameSessions[session].setScore(IP, score);
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
