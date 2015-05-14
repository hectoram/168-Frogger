using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ClientScript : MonoBehaviour
{
    // The port number for the remote device.
    private const int port = 11000;
    static string[] stringSeparators = new string[] { "<EOF>" };

    IPHostEntry ipHostInfo;
    IPAddress ipAddress;
    IPEndPoint remoteEP;
    //IPAddress ipAddress = IPAddress.Parse("98.164.225.129");
    //IPAddress ipAddress = IPAddress.Parse("72.211.207.66");
    //IPAddress ipAddress = IPAddress.Parse("192.168.1.24");
    //IPAddress ipAddress = IPAddress.Parse("127.0.0.1");

    Socket client;

    // Create the state object for sending.
    StateObject send_so;

    // Create the state object for receiving.
    StateObject recv_so;

    LoginScript loginInfo;
    MultiplayerLobbyScript lobbyInfo;

    GameObject loginMenu;
    GameObject gameMenu;

    bool isGameInProgress;
    int myPlayerNumber = 0; // Starts at zero, if it's zero, that means that the player has not been assigned a number yet

    static string data = "";
    static string[] playerQueue = { "null", "null", "null", "null" };
    static string[] playerReady = { "null", "null", "null", "null" };

    static bool startGame = false;

    public static MenuScript myMenu;
    public GameObject menu;

    static AsyncOperation async;

    public void setPlayerNumber(int number)
    {
        myPlayerNumber = number;
    }

    public void resetData(){
		data = "";
	}

    static void setStartGame(bool value)
    {
        startGame = value;
    }

	public static void setData(string newData)
    {
        data = newData;
    }

    public static void setQueue(string p1, string p2, string p3, string p4)
    {
        playerQueue[0] = p1;
        playerQueue[1] = p2;
        playerQueue[2] = p3;
        playerQueue[3] = p4;
    }

    public static void setReady(string p1, string p2, string p3, string p4)
    {
        playerReady[0] = p1;
        playerReady[1] = p2;
        playerReady[2] = p3;
        playerReady[3] = p4;
    }

    public void Start()
    {
        DontDestroyOnLoad(this);

        isGameInProgress = false;

        Debug.Log("In Start()");

        // Establish the remote endpoint for the socket.
        // The name of the 
        // remote device is "host.contoso.com".
        ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
        ipAddress = ipHostInfo.AddressList[0];
        remoteEP = new IPEndPoint(ipAddress, port);

        Debug.Log("Creating TCP/IP socket...");
        // Create a TCP/IP socket.
        client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        Debug.Log("Connecting to the remote endpoint...");
        send_so = new StateObject();
        send_so.workSocket = client;

        recv_so = new StateObject();
        recv_so.workSocket = client;

        loginMenu = GameObject.FindGameObjectWithTag("Login Menu");
        loginInfo = loginMenu.GetComponent<LoginScript>();

        gameMenu = GameObject.FindGameObjectWithTag("Menus");
        lobbyInfo = gameMenu.GetComponent<MultiplayerLobbyScript>();

        myMenu = menu.GetComponent<MenuScript>();

        async = Application.LoadLevelAsync("Multiplayer Scene");
        // Set this false to wait changing the scene
        async.allowSceneActivation = false;

        //StartClient();
    }

    public void Update()
    {
        if (isGameInProgress)
        {
            //Send("position," + )
        }
        else if (lobbyInfo.lobbyMenu.enabled)
        {
            lobbyInfo.queuedPlayers[0] = playerQueue[0];
            lobbyInfo.queuedPlayers[1] = playerQueue[1];
            lobbyInfo.queuedPlayers[2] = playerQueue[2];
            lobbyInfo.queuedPlayers[3] = playerQueue[3];

            lobbyInfo.readyPlayers[0] = playerReady[0];
            lobbyInfo.readyPlayers[1] = playerReady[1];
            lobbyInfo.readyPlayers[2] = playerReady[2];
            lobbyInfo.readyPlayers[3] = playerReady[3];
        }

        if (startGame)
        {
            //StartCoroutine(LoadMultiplayerLevel());
            //StartMultiplayerGame();
            // Set this false to wait changing the scene
            async.allowSceneActivation = true;
            startGame = false;
        }

        if (data == "true")
        {
            loginInfo.DisplayLoginSuccessMenu();
        }
        else if (data == "false")
        {
            loginInfo.DisplayLoginFailedMenu();
        }
        else if (data == "new")
        {
            loginInfo.DisplayLoginNewUserMenu();
        }
		else if (data == "newfailed")
		{
			loginInfo.DisplayLoginNewUserFailedMenu();
		}
    }

    public void SendMSG(string data, int time)
    {
        Debug.Log("Sending message...");
        // Send test data to the remote device.
        //Send("This is a test message.<EOF>");
        Send(data);
        send_so.sendDone.WaitOne(time);
    }

    public void ReceiveMSG(int time)
    {
        Debug.Log("Waiting for response...");
        // Receive the response from the remote device.
        Receive(recv_so);
        recv_so.receiveDone.WaitOne(time);
    }

	//public void StartClient (string username, string password)
	public void StartClient(string message, string username, string password) //Ceci: added the first param
    {
        Debug.Log("Starting client...");
        // Connect to a remote device.
        try
        {
            // Connect to the remote endpoint.
            client.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), send_so);

            Debug.Log("Waiting for connection...");
            // Waits for 5 seconds for connection to be done
            send_so.connectDone.WaitOne(5000);

            Debug.Log("Sending test data...");
            // Send test data to the remote device.
            //Send("This is a test message.<EOF>");
            Send(message + "," + username + "," + password + "<EOF>");

            send_so.sendDone.WaitOne(5000);

            Debug.Log("Waiting for response...");
            // Receive the response from the remote device.
            Receive(recv_so);
            recv_so.receiveDone.WaitOne(5000);

            Debug.Log("Response received: " + recv_so.response);
            // Write the response to the console.
            Console.WriteLine("Response received : {0}", recv_so.response);

        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    public static void ConnectCallback(IAsyncResult ar)
    {
        try
        {
            // Create the state object.
            StateObject state = (StateObject)ar.AsyncState;
            // Retrieve the socket from the state object.
            Socket client = state.workSocket;

            // Complete the connection.
            client.EndConnect(ar);

            Debug.Log("Socket connected to " + client.RemoteEndPoint.ToString());
            Console.WriteLine("Socket connected to {0}",
                client.RemoteEndPoint.ToString());

            // Signal that the connection has been made.
            state.connectDone.Set();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    public static void Receive(StateObject state)
    {
        try
        {
            Socket client = state.workSocket;

            // Begin receiving the data from the remote device.
            client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }
    
    void StartMultiplayerGame()
    {
        startGame = false;
        Application.LoadLevel("Multiplayer Scene");
    }

    IEnumerator LoadMultiplayerLevel()
    {
        Application.LoadLevel("Multiplayer Scene");
        return null;
    }

    public static void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            // Retrieve the state object and the client socket 
            // from the asynchronous state object.
            StateObject state =  (StateObject)ar.AsyncState;
            Socket client = state.workSocket;

            // Read data from the remote device.
            int bytesRead = client.EndReceive(ar);

            if (bytesRead > 0)
            {
                // Found a 
                state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
                string content = state.sb.ToString();

				char[] delimiterChars = { ' ', ',','<','>'};

				string[] messageToCheck = content.Split(delimiterChars);

                //Debug.Log("I sent you this back: " + messageToCheck[0] + " " + messageToCheck[1]);
				Debug.Log("I recived this : " + messageToCheck[1]);

                if (messageToCheck[0] == "login")
                {
                    if (messageToCheck[1] == "true")
                    {
                        Debug.Log("Login successful!");
                        setData(messageToCheck[1]);
                    }
                    else if (messageToCheck[1] == "false")
                    {
                        Debug.Log("Login failed!");
                        setData(messageToCheck[1]);
                    }
                    else if (messageToCheck[1] == "new")
                    {
                        Debug.Log("A new user has been created!");
                        setData(messageToCheck[1]);
                    }
					else if (messageToCheck[1] == "newfailed")
					{
						Debug.Log("Failed to create new user!");
						setData(messageToCheck[1]);
					}
                }
                else if (messageToCheck[0] == "queue")
                {
                    setQueue(messageToCheck[2], messageToCheck[4], messageToCheck[6], messageToCheck[8]);
                }
                else if (messageToCheck[0] == "ready")
                {
                    setReady(messageToCheck[2], messageToCheck[4], messageToCheck[6], messageToCheck[8]);
                }
                else if (messageToCheck[0] == "start-game")
                {
                    //async.allowSceneActivation = true;
                    //myMenu.StartMultiplayerGame();
                    setStartGame(true);  // Created this function to be able start the Multiplayer Scene from inside ReceiveCallback
                    //Application.LoadLevel("Multiplayer Scene");  // Was getting an error trying to call this from here
                }
                
                if (messageToCheck.Length == 2)
                {
                    state.receiveDone.Set();
                    state.response = messageToCheck[0];

                    state.workSocket.Shutdown(SocketShutdown.Both);
                    state.workSocket.Close();

                }
                else
                {
					StateObject newstate = new StateObject();
					newstate.workSocket = client;
                    // Get the rest of the data.
                    client.BeginReceive(newstate.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), newstate);
                }
            }
            else
            {
                Debug.Log("Connection close has been requested");
                Console.WriteLine("Connection close has been requested.");
                // Signal that all bytes have been received.

            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    public void Send(String data)
    {
        // Convert the string data to byte data using ASCII encoding.
        byte[] byteData = Encoding.ASCII.GetBytes(data);

        // Begin sending the data to the remote device.
        client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), send_so);
    }

    public static void SendCallback(IAsyncResult ar)
    {
        try
        {
            // Retrieve the socket from the state object.
            StateObject so = (StateObject)ar.AsyncState;
            Socket client = so.workSocket;

            // Complete sending the data to the remote device.
            int bytesSent = client.EndSend(ar);
            Debug.Log("Sent " + bytesSent + " to the server.");
            Console.WriteLine("Sent {0} bytes to server.", bytesSent);

            // Signal that all bytes have been sent.
            so.sendDone.Set();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    // State object for receiving data from remote device.
    public class StateObject
    {
        // Client socket.
        public Socket workSocket = null;
        // Size of receive buffer.
        public const int BufferSize = 256;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder sb = new StringBuilder();
        // ManualResetEvent instances signal completion.
		public AutoResetEvent connectDone =
			new AutoResetEvent(false);
		
		public AutoResetEvent receiveDone =
			new AutoResetEvent(false);
		
		public AutoResetEvent sendDone =
			new AutoResetEvent(false);

        // The response from the remote device.
        public String response = String.Empty;
    }
}