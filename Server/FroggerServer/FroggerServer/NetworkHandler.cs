﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//networking
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace FroggerServer
{
    class NetworkHandler
    {

        AsynchronousSocketListener connectionLinker;
        Timer checkDCTimer;
        private static bool startDCTimer = true;
        //Key is IP address of the player. 
        public static SortedDictionary<string, Player> connectedPlayers = new SortedDictionary<string, Player>();
        public static List<String> toRemove = new List<string>();
        public SortedDictionary<string, Queue<string>> messagesRecieved = new SortedDictionary<string, Queue<string>>();

        private static readonly Mutex mutexLock = new Mutex();
        private static bool playersToRemove = false;

        NetworkHandler()
        {
            connectionLinker = new AsynchronousSocketListener();
        }

        public void init()
        {
            connectionLinker.StartListening();
        }


        public void update()
        {
            //Do things here
            GameHandler.Instance.update();
            if (startDCTimer)
            {
                checkDCTimer = new Timer(checkForDissconnect, null, 0, 5000);
                startDCTimer = !startDCTimer;
            }

            if (playersToRemove)
            {
                removeUsers();
                playersToRemove = !playersToRemove;
            }

        }

        private void sendMessageToClients(string senderIP , string message)
        {
            try
            {
                sendMessage(GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].first.IP, message);
            }
            catch (Exception e)
            {
                //Do nothing. Client isn't connected. 
            }
            try
            {
                sendMessage(GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].second.IP, message);
            }
            catch (Exception e)
            {

            }
            try
            {
                sendMessage(GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].third.IP, message);
            }
            catch (Exception e)
            {

            }
            try
            {
                sendMessage(GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].fourth.IP, message);
            }catch(Exception e)
            {
            
            }
        }

        //Queue the player to be DC don't do it in the method. 
        private static void checkForDissconnect(Object o)
        {
            mutexLock.WaitOne();
            try
            {
                foreach (var entry in connectedPlayers)
                {
                    if (!isConnected(entry.Value))
                    {
                        handleDissconnect(entry.Key);
                    }
                }
            }
            finally
            {
                mutexLock.ReleaseMutex();
            }

            //Restart the timer
            startDCTimer = !startDCTimer;
        }

        private static bool isConnected(Player playerToPoll)
        {
            try
            {
                return !(playerToPoll.connection.Poll(1, SelectMode.SelectRead) && playerToPoll.connection.Available == 0);
            }
            catch (SocketException)
            {
                return false;
            }
        }

        private static void removeUsers()
        {
            mutexLock.WaitOne();
            try
            {
                foreach (var value in toRemove)
                {  
                    connectedPlayers.Remove(value);
                    GameHandler.Instance.handleDisconnectedPlayer(value);
                }
                //Clear so other problems don't arise later. 
                toRemove.Clear();
            }
            finally
            {
                mutexLock.ReleaseMutex();
            }
        }


        private static void handleDissconnect(string IP)
        {
            toRemove.Add(IP);
            //set equal to true so that the update loop will call the proper functions
            playersToRemove = !playersToRemove;
        }

        public bool sendMessage(string sendToIP, string message)
        {
            try
            {
                connectionLinker.Send(connectedPlayers[sendToIP].connection, message);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public void addNewPlayer(string IP, Socket mySocket)
        {
            mutexLock.WaitOne();
            try
            {
                if (!connectedPlayers.ContainsKey(IP))
                {
                    connectedPlayers.Add(IP, new Player(mySocket, IP));
                    messagesRecieved.Add(IP, new Queue<string>());
                    Console.WriteLine("I've added my newly connected player to my list!");
                }
            }
            finally
            {
                mutexLock.ReleaseMutex();
            }
        }

        public void parseMessage(string senderIP, string toParse)
        {
            char[] delimiterChars = { ',', '<', '>' };//Ceci: removed space, testing
            string[] message = toParse.Split(delimiterChars);

            // handle Login
            if (message[0] == "userLogin")
            {
                if (DataBase.Instance.login(message[1], message[2]))
                {
                    Console.WriteLine("Login was successful");
                    sendMessage(senderIP, "login,true<EOF>");
                    connectedPlayers[senderIP].setPlayerName(message[1]);
                }
                else
                {
                    Console.WriteLine("Login was not successful");
                    sendMessage(senderIP, "login,false<EOF>");
                }
            }
            else if (message[0] == "userCreate")
            {
                // Handles creating new users
                if (DataBase.Instance.registerUser(message[1], message[2]))
                {
                    connectedPlayers[senderIP].setPlayerName(message[1]);
                    sendMessage(senderIP, "login,new<EOF>");
                }
                else
                {
                    Console.WriteLine("User " + message[1] + " was not created!");
                    sendMessage(senderIP, "login,newfailed<EOF>");
                }
            }
            else if (message[0] == "queue")
            {

                string toSend = "queue,1," + GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].getPlayerName(1) + ",2," +
                GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].getPlayerName(2) + ",3," +
                GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].getPlayerName(3) +
                ",4," + GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].getPlayerName(4) + "<EOF>";

                sendMessage(senderIP, toSend);


                string toSendReady;

                try
                {
                    toSendReady =
                    "ready,1," +
                     GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].playerIsReady(1) + ",2," +
                     GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].playerIsReady(2) + ",3," +
                     GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].playerIsReady(3) + ",4," +
                     GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].playerIsReady(4) + "<EOF>";
                }
                catch (Exception e)
                {
                    toSendReady =
                        "ready,1," + ((GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].p1Ready) ? GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].first.getUserName() : "empty") +
                        ",2," + "empty"
                        + ",3,null,4,null<EOF>";
                }

                sendMessage(senderIP, toSendReady);

                //send back a message of who is in the queue
            }
            else if (message[0] == "gameOver")
            {
                Console.WriteLine("I've got a gameOver");
                GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].registerFinalScore(senderIP, message[1]);   
            }
            else if (message[0] == "ready")
            {
                //"ready,1,username1,2,username2,3,username3,4,username4<EOF>"

                GameHandler.Instance.setReady(GameHandler.Instance.getSessionName(senderIP), senderIP);
                string toSend;

                try
                {
                    toSend =
                    "ready,1," +
                     GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].playerIsReady(1) + ",2," +
                     GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].playerIsReady(2) + ",3," +
                     GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].playerIsReady(3) + ",4," +
                     GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].playerIsReady(4) + "<EOF>";
                }
                catch (Exception e)
                {
                    toSend =
                        "ready,1," + ((GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].p1Ready) ? GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].first.getUserName() : "empty") +
                        ",2," + "empty"
                        + ",3,null,4,null<EOF>";
                }

                sendMessage(senderIP, toSend);


                if (GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].allReady())
                {
                    string startGame = "start-game," + GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].getPlayerCount() + "<EOF>";
                    sendMessageToClients(senderIP, startGame);
                }
            }
            else if (message[0] == "frogPosition")
            {
                GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].setPlayerPosition(senderIP, float.Parse(message[1]), float.Parse(message[2]));
                
                string toSend = "frogPosition," + GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].getPlayerPositions(1) + ",2," + 
                    GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].getPlayerPositions(2) +
                    ",3," + GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].getPlayerPositions(3) + ",4," +
                    GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].getPlayerPositions(4) + "<EOF>";
                
                sendMessage(GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].first.IP, toSend);
                sendMessage(GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].second.IP, toSend);
                //Timer info
                sendMessage(senderIP , "timer," + GameHandler.Instance.getCurrentTime(GameHandler.Instance.getSessionName(senderIP)) + "<EOF>");
            }
            else if (message[0] == "chat-message")
            {
                string messageToSend = "";
                int i = 1;
                while (message[i] != "EOF")
                {
                    messageToSend = messageToSend + message[i];

                    // Makes sure commas don't get parsed out
                    if (message[i + 1] != "EOF")
                    {
                        messageToSend = messageToSend  + ",";
                    }

                    i++;
                    
                }

                Console.WriteLine("I'm going to be sending this out to all the clients: " + messageToSend);
                GameHandler.Instance.chatMessageHandle(GameHandler.Instance.getSessionName(senderIP), messageToSend, senderIP);
            }

            else if (message[0] == "timer")
            {
                sendMessage(senderIP, "timer," + GameHandler.Instance.getCurrentTime(GameHandler.Instance.getSessionName(senderIP)) + "<EOF>");
            }
            else if (message[0] == "player-ready")
            {
                GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].setLoadReady(senderIP);
                Console.WriteLine("Ready message recieved");
                if (GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].allPlayersLoaded())
                {
                    if (!GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].readySent)
                    {
                        sendMessageToClients(senderIP, "start-timer<EOF>");
                        GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].readySent = true;
                    }
                    GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].gameHasStarted = true;
                }
            }
            else if (message[0] == "join-session")
            {
                bool temp = GameHandler.Instance.joinSession(message[1], connectedPlayers[senderIP]);
                string toSend = "";
                if (temp)
                    toSend = "join-session,true<EOF>";
                else
                    toSend = "join-session,false<EOF>";

                sendMessage(senderIP, toSend);
            }else if(message[0] == "score")
            {
                GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].setScore(senderIP, message[1]);
                //score, score1, score2, score3, score4<EOF>
                string toSend = "score," + 
                    GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].getPlayerScore(1) + "," +
                    GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].getPlayerScore(2) + "," +
                    GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].getPlayerScore(3) + "," +
                    GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].getPlayerScore(4) + "<EOF>";

                sendMessage(GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].first.IP, toSend);
                sendMessage(GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].second.IP, toSend);
            }
            else
                NetworkHandler.Instance.messagesRecieved[senderIP].Enqueue(toParse);
        }

        //Handle being a singleton
        private static NetworkHandler instance = null;
        private static readonly object padlock = new object();
        public static NetworkHandler Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new NetworkHandler();
                    }
                    return instance;
                }
            }
        }

    }

    // State object for reading client data asynchronously
    public class StateObject
    {
        // Client socket.
        public Socket workSocket = null;
        // Size of receive buffer.
        public const int BufferSize = 1024;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder sb = new StringBuilder();
    }

    public class AsynchronousSocketListener
    {
        // Thread signal.
        public static AutoResetEvent allDone = new AutoResetEvent(false);
        public static List<Socket> clients = new List<Socket>();


        public AsynchronousSocketListener()
        {
        }
        public void StartListening()
        {
            // Data buffer for incoming data.
            byte[] bytes = new Byte[1024];
            // Create a TCP/IP socket.
            Socket listener = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream, ProtocolType.Tcp);
            //Listen to external IP address
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);
            // Listen to any IP Address
            IPEndPoint any = new IPEndPoint(IPAddress.Any, 11000);
            // Bind the socket to the local endpoint and listen for incoming connections.
            try
            {
                listener.Bind(any);
                listener.Listen(100);
                while (true)
                {
                    // Set the event to nonsignaled state.
                    allDone.Reset();
                    // Start an asynchronous socket to listen for connections.
                    Console.WriteLine("Waiting for a connection..");
                    listener.BeginAccept(
                    new AsyncCallback(AcceptCallback),
                    listener);
                    // Wait until a connection is made before continuing.
                    allDone.WaitOne();
                    Console.WriteLine("Client has connected!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();
        }
        public static void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.
            allDone.Set();
            // Get the socket that handles the client request.
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the state object.
            StateObject state = new StateObject();
            state.workSocket = handler;

            String myIP = String.Empty;
            string messageIP = handler.RemoteEndPoint.ToString();
            myIP = messageIP;

            NetworkHandler.Instance.addNewPlayer(myIP, handler);

            clients.Add(handler);
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
            new AsyncCallback(ReadCallback), state);
        }
        public static void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;
            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;
            // Read data from the client socket.
            int bytesRead = handler.EndReceive(ar);
            if (bytesRead > 0)
            {
                // There might be more data, so store the data received so far.
                state.sb.Append(Encoding.ASCII.GetString(
                state.buffer, 0, bytesRead));
                // Check for end-of-file tag. If it is not there, read
                // more data.
                String myIP = String.Empty;
                string messageIP = handler.RemoteEndPoint.ToString();
                myIP = messageIP;

                content = state.sb.ToString();
                if (content.IndexOf("<EOF>") > -1)
                {
                    //Handle the message. 
                    NetworkHandler.Instance.parseMessage(myIP, content.ToString());

                    //Make a new object so you don't append forever. 
                    StateObject newstate = new StateObject();
                    newstate.workSocket = handler;
                    // Call BeginReceive with a new state object
                    handler.BeginReceive(newstate.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), newstate);
                }
                else
                {
                    // Not all data received. Get more.
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);
                }
            }
        }
        public void Send(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.
            Console.WriteLine("I'm sending this: " + data);
            byte[] byteData = Encoding.ASCII.GetBytes(data);
            // Begin sending the data to the remote device.
            handler.BeginSend(byteData, 0, byteData.Length, 0,
            new AsyncCallback(SendCallback), handler);
        }
        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket handler = (Socket)ar.AsyncState;
                // Complete sending the data to the remote device.
                int bytesSent = handler.EndSend(ar);
                //Console.WriteLine("Sent {0} bytes to client.", bytesSent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}