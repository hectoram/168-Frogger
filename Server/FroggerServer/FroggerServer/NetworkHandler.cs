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

        AsynchronousSocketListener listener;
        //Add some sort of list of messages with a pair type architecutre
        //So <Username,message, timestamp>
        //In the update send it off to the game class to process.
        //After that it's up to the gamelogic to send them or have another queue that can be emptied
        List<Player> connectedPlayers = new List<Player>();

        NetworkHandler()
        {
            listener = new AsynchronousSocketListener();
        }

        public bool init()
        {
            listener.StartListening();
            return true;
        }


        public void update() 
        { 
            //Do things here
        }

        public void addNewPlayer(string ID, Socket mySocket) 
        {
            connectedPlayers.Add(new Player(ID,mySocket));
            Console.WriteLine("I've added my newly connected player to my list!");
        }

        private void parseMessage(string toParse)
        {

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
        public static ManualResetEvent allDone = new ManualResetEvent(false);
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
            // Games have bidirectional communication (as opposed to request/response)
            // So I need to store all clients sockets so I can send them messages later
            // TODO: store in meaningful way,such as Dictionary<string,Socket>
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
                content = state.sb.ToString();
                if (content.IndexOf("<EOF>") > -1)
                {
                    // All the data has been read from the
                    // client. Display it on the console.
                    char[] delimiterChars = { ' ', ',', '<', '>' };
                    string[] message = content.Split(delimiterChars);


                    //userLogin, username, password<EOF>

                    if (message[0] == "userLogin")
                    {
                        if (DataBase.Instance.login(message[1], message[2]))
                        {
                            Console.WriteLine("Login was successful");
                            NetworkHandler.Instance.addNewPlayer(message[1],handler);
                            Send(handler, "login,true<EOF>");
                        }
                        else
                        {
                            Console.WriteLine("Login was not successful");
                            Console.WriteLine("Attemping to create new user......");

                            if (DataBase.Instance.registerUser(message[1], message[2]))
                            {
                                Console.WriteLine("User " + message[1] + " was created!");
                                Send(handler, "login,new<EOF>");
                            }
                            else
                            {
                                Console.WriteLine("User " + message[1] + " was not created!");
                                Send(handler, "login,false<EOF>");
                            }
                        }


                    }

                    // Echo the data back to the client.
                    //Send(handler, content);
                    // Setup a new state object
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
        private static void Send(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.
            //Console.WriteLine("I'm sending this: " + data);
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