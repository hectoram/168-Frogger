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

        //Key is IP address of the player. 
        public  SortedDictionary<string, Player> connectedPlayers = new SortedDictionary<string, Player>();
        public SortedDictionary<string, Queue<string>> messagesRecieved = new SortedDictionary<string, Queue<string>>();


        NetworkHandler()
        {
            connectionLinker = new AsynchronousSocketListener();
        }

        public bool init()
        {
            connectionLinker.StartListening();
            return true;
        }


        public void update() 
        { 
            //Do things here
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
            if (!connectedPlayers.ContainsKey(IP))
            {
                connectedPlayers.Add(IP, new Player(mySocket, IP));
                messagesRecieved.Add(IP, new Queue<string>());
                Console.WriteLine("I've added my newly connected player to my list!");
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
                GameHandler.Instance.joinSession("default", connectedPlayers[senderIP]);
            }
            else if (message[0] == "gameOver")
            {
                GameHandler.Instance.setScore("default",senderIP, message[1]);
                //If I've recieved both players scores
                if(GameHandler.Instance.gameSessions["default"].bothScoresSet)
                {
                    string toSendP1 = "";
                    string toSendP2 = "";

                    if (GameHandler.Instance.gameSessions["default"].winner == 1)
                    {
                        toSendP1 = "gameOver,won," + GameHandler.Instance.gameSessions["default"].playerTwoScore + ",score<EOF>";
                        toSendP2 = "gameOver,lost," + GameHandler.Instance.gameSessions["default"].playerOneScore + ",score<EOF>";
                    }
                    else if (GameHandler.Instance.gameSessions["default"].winner == 2)
                    {
                        toSendP1 = "gameOver,lost," + GameHandler.Instance.gameSessions["default"].playerTwoScore + ",score<EOF>";
                        toSendP2 = "gameOver,won," + GameHandler.Instance.gameSessions["default"].playerOneScore + ",score<EOF>";
                    }
                    else
                    {
                        toSendP1 = "gameOver,tie," + GameHandler.Instance.gameSessions["default"].playerTwoScore + ",score<EOF>";
                        toSendP2 = "gameOver,tie," + GameHandler.Instance.gameSessions["default"].playerOneScore + ",score<EOF>";
                    }
                        
                    connectionLinker.Send(GameHandler.Instance.gameSessions["default"].first.connection, toSendP1);
                    connectionLinker.Send(GameHandler.Instance.gameSessions["default"].second.connection, toSendP2);

                }
            }
            else if (message[0] == "ready")
            {
                int playerCount = GameHandler.Instance.gameSessions["default"].getPlayerCount() + 1;
                string toSend = "ready," + playerCount + "<EOF>";
                connectionLinker.Send(connectedPlayers[senderIP].connection, toSend);

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
            string[] messageIP = handler.RemoteEndPoint.ToString().Split(':');
            myIP = messageIP[0];

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
                string[] messageIP = handler.RemoteEndPoint.ToString().Split(':');
                myIP = messageIP[0];

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
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}