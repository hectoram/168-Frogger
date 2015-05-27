using System;
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

            if(playersToRemove)
            {
                removeUsers();
                playersToRemove = !playersToRemove;
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
                    Console.WriteLine("I've removed: " + connectedPlayers[value].getUserName());
                    connectedPlayers.Remove(value);
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
                Console.WriteLine(e.ToString());
                return false;
            }         
        }

        public void addNewPlayer( string IP, Socket mySocket) 
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
            char[] delimiterChars = { ' ', ',', '<', '>' };
            string[] message = toParse.Split(delimiterChars);

            // handle Login
            if (message[0] == "userLogin") 
            {
                if (DataBase.Instance.login(message[1], message[2]))
                {
                    Console.WriteLine("Login was successful");
                    connectionLinker.Send(connectedPlayers[senderIP].connection, "login,true<EOF>");
                    connectedPlayers[senderIP].setPlayerName(message[1]);
                }
                else
                {
                    Console.WriteLine("Login was not successful");
                    connectionLinker.Send(connectedPlayers[senderIP].connection, "login,false<EOF>");
                }
            }
            else if (message[0] == "userCreate")
            { 
                // Handles creating new users
                if (DataBase.Instance.registerUser(message[1], message[2]))
                    connectionLinker.Send(connectedPlayers[senderIP].connection, "login,new<EOF>");
                else
                {
                    Console.WriteLine("User " + message[1] + " was not created!");
                    connectionLinker.Send(connectedPlayers[senderIP].connection, "login,newfailed<EOF>");
                }
            }
            else if (message[0] == "queue")
            { 

                    string toSend = "queue,1," + GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].getPlayerName(1) + ",2," +
                    GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].getPlayerName(2) + ",3," +
                    GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].getPlayerName(3) +
                    ",4," + GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].getPlayerName(4) + "<EOF>";

                    connectionLinker.Send(connectedPlayers[senderIP].connection, toSend);
               

                string toSendReady;

                try
                {
                    toSendReady =
                    "ready,1," + ((GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].p1Ready) ? GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].first.getUserName() : "empty") +
                    ",2," + ((GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].p2Ready) ? GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].second.getUserName() : "empty")
                    + ",3,null,4,null<EOF>";
                }
                catch (Exception e)
                {
                    toSendReady =
                        "ready,1," + ((GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].p1Ready) ? GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].first.getUserName() : "empty") +
                        ",2," + "empty"
                        + ",3,null,4,null<EOF>";
                }

                connectionLinker.Send(connectedPlayers[senderIP].connection, toSendReady);
                //send back a message of who is in the queue
            }
            else if (message[0] == "gameOver")
            {
                GameHandler.Instance.setScore(GameHandler.Instance.getSessionName(senderIP),senderIP, message[1]);
                //If I've recieved both players scores
                if(GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].bothScoresSet)
                {
                    string toSendP1;

                    if (GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].winner == 1)
                    {
                        toSendP1 = "gameOver," + "1," + "won," + GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].playerOneScore + ",2," + "lost," + GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].playerTwoScore + ",3," + "null,4,null<EOF>";
                    }
                    else if (GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].winner == 2)
                    {
                        toSendP1 = "gameOver," + "1," + "lost," + GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].playerOneScore + ",2," + "won," + GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].playerTwoScore + ",3," + "null,4,null<EOF>";
                    }
                    else
                    {
                        toSendP1 = "gameOver," + "1," + "tie," + GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].playerOneScore + ",2," + "tie," + GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].playerTwoScore + ",3," + "null,4,null<EOF>";
                    }
                        
                    connectionLinker.Send(GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].first.connection, toSendP1);
                    connectionLinker.Send(GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].second.connection, toSendP1);

                }
            }
            else if (message[0] == "ready")
            {
                //"ready,1,username1,2,username2,3,username3,4,username4<EOF>"

                GameHandler.Instance.setReady(GameHandler.Instance.getSessionName(senderIP),senderIP);
                string toSend;

                try
                {
                        toSend =
                        "ready,1," + ((GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].p1Ready) ? GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].first.getUserName() : "empty") +
                        ",2," + ((GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].p2Ready) ? GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].second.getUserName() : "empty")
                        + ",3,null,4,null<EOF>";
                }
                catch (Exception e)
                {
                    toSend =
                        "ready,1," + ((GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].p1Ready) ? GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].first.getUserName() : "empty") +
                        ",2," +  "empty"
                        + ",3,null,4,null<EOF>";
                }

                connectionLinker.Send(connectedPlayers[senderIP].connection, toSend);


                if (GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].p1Ready && GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].p2Ready)
                {
                    string startGame = "start-game," + GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].getPlayerCount() + "<EOF>";
                    connectionLinker.Send(connectedPlayers[senderIP].connection, startGame);
                }
            }
            else if (message[0] == "frogPosition")
            {
                GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].setPlayerPosition(senderIP, float.Parse(message[1]), float.Parse(message[2]));
                string toSend = "frogPosition," + GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].getPlayerPositions(1) + ",2," + GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].getPlayerPositions(2) + ",3,null,null,4,null,null<EOF>";
                connectionLinker.Send(connectedPlayers[senderIP].connection, toSend);
                //Timer info
                connectionLinker.Send(connectedPlayers[senderIP].connection, "timer," + GameHandler.Instance.getCurrentTime(GameHandler.Instance.getSessionName(senderIP)) + "<EOF>");
            }else if(message[0] == "chat-message")
            {
                GameHandler.Instance.chatMessageHandle(GameHandler.Instance.getSessionName(senderIP),message [1] ,senderIP);
            }else if(message[0] == "timer")
            {
                connectionLinker.Send(connectedPlayers[senderIP].connection, "timer," + GameHandler.Instance.getCurrentTime(GameHandler.Instance.getSessionName(senderIP)) + "<EOF>");
            }else if(message[0] == "player-ready")
            {
                GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].setLoadReady(senderIP);
                if (GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].allPlayersLoaded())
                {
                    connectionLinker.Send(GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].first.connection, "start-timer<EOF>");
                    connectionLinker.Send(GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].second.connection, "start-timer<EOF>");
                    GameHandler.Instance.gameSessions[GameHandler.Instance.getSessionName(senderIP)].setTimer();
                }
            }
            else if (message[0] == "join-session")
            {
                bool temp = GameHandler.Instance.joinSession(message[1], connectedPlayers[senderIP]);
                string toSend = "";
                if(temp)
                    toSend = "join-session,true<EOF>";
                else
                    toSend = "join-session,false<EOF>";

                connectionLinker.Send(connectedPlayers[senderIP].connection,toSend);
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

                Console.WriteLine("The following IP is sending me a message: " + myIP);

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