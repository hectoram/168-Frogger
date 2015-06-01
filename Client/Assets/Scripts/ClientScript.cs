using System;
using System.Collections.Generic;
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
    //IPAddress ipAddress = IPAddress.Parse("192.168.1.20");

    Socket client;

    // THREAD STUFF FOR RECEIVING
    public struct QueueMessage {
      public string title;
      public string playerOneName;
      public string playerTwoName;
      public string playerThreeName;
      public string playerFourName;
    }

    public struct ReadyMessage {
      public string playerOneReady;
      public string playerTwoReady;
      public string playerThreeReady;
      public string playerFourReady;
    }

    public struct LoginMessage {
      public string title;
      public string result;
    }

    Thread t;
    bool threadStarted = false;
    bool stopThread = false;
    static Queue<QueueMessage> messageQueue = new Queue<QueueMessage>();
    static Queue<ReadyMessage> readyQueue = new Queue<ReadyMessage>();
    static Queue<LoginMessage> loginQueue = new Queue<LoginMessage>();
    //


  // Create the state object for sending.
  StateObject send_so;

    // Create the state object for receiving.
    StateObject recv_so;

    LoginScript loginInfo;
    MultiplayerLobbyScript lobbyInfo;

    GameObject gameUI;
    GameUI gameplay;

    GameObject loginMenu;
    GameObject gameMenu;

    bool isGameInProgress;
    public static string myPlayerNumber = "0"; // Starts at zero, if it's zero, that means that the player has not been assigned a number yet
    public static string numberOfPlayers = "0";
    public static string myUsername = "not set";

    static string data = "";
    static string[] playerQueue = { "null", "null", "null", "null" };
    static string[] playerReady = { "null", "null", "null", "null" };

    static bool startGame = false;

    bool isPlayerInLobby = false;

    static AsyncOperation async;

    public static bool spawnObstacles = false;

    bool loggedIn = false;

    public static string currentScene = "Menu Scene";

    public static string gameOverResult = "";

    public static string chatUser = "";
    public static string chatMessage = "";

    public string getGameOverResult()
    {
        return gameOverResult;
    }

    public void setLoggedIn(bool value)
    {
        loggedIn = value;
    }

    public bool getLoggedIn()
    {
        return loggedIn;
    }

    public void closeLobbyMenu()
    {
        lobbyInfo.enabled = false;
    }

    public bool getIsGameInProgress()
    {
        return isGameInProgress;
    }

    public string getPlayerNumber()
    {
        return myPlayerNumber;
    }

    public string getNumberOfPlayers()
    {
        return numberOfPlayers;
    }

    public string getUsername()
    {
        return myUsername;
    }

    public void setPlayerNumber(string number)
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
        QueueMessage message = new QueueMessage();
        message.playerOneName = p1;
        message.playerTwoName = p2;
        message.playerThreeName = p3;
        message.playerFourName = p4;

        // Set info to display usernames in MultiplayerLobbyScript
        playerQueue[0] = p1;
        playerQueue[1] = p2;
        playerQueue[2] = p3;
        playerQueue[3] = p4;

        Debug.Log("Does Player 1 == my username? " + p1 + " == " + myUsername + "?");
        Debug.Log("Does Player 1 == my username? " + p2 + " == " + myUsername + "?");
        if (p1 == myUsername)
            myPlayerNumber = "1";
        else if (p2 == myUsername)
            myPlayerNumber = "2";
        else if (p3 == myUsername)
            myPlayerNumber = "3";
        else if (p4 == myUsername)
            myPlayerNumber = "4";

        messageQueue.Enqueue(message);
    }

    public static void setReady(string p1, string p2, string p3, string p4)
    {
        ReadyMessage message = new ReadyMessage();
        message.playerOneReady = p1;
        message.playerTwoReady = p2;
        message.playerThreeReady = p3;
        message.playerFourReady = p4;

        // Set info the display "READY" messages in MultiplayerLobbyScript
        playerReady[0] = p1;
        playerReady[1] = p2;
        playerReady[2] = p3;
        playerReady[3] = p4;

        readyQueue.Enqueue(message);
    }

    public bool getIsPlayerInLobby()
    {
        return isPlayerInLobby;
    }

    public void setIsPlayerInLobby(bool value)
    {
        isPlayerInLobby = value;
    }

    public void Start()
    {
        DontDestroyOnLoad(this);

        isGameInProgress = false;
        isPlayerInLobby = false;

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

        async = Application.LoadLevelAsync("Multiplayer Scene");
        // Set this false to wait changing the scene
        async.allowSceneActivation = false;

        gameUI = GameObject.FindGameObjectWithTag("Menus");
        gameplay = gameUI.GetComponent<GameUI>();
    }

    public void Update()
    {
        if (isPlayerInLobby)
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
            async.allowSceneActivation = true;
            startGame = false;
            isGameInProgress = true;
            currentScene = "Multiplayer Scene";
        }

        if (currentScene != "Multiplayer Scene")
        {
            if (data == "true")
            {
                loginInfo.DisplayLoginSuccessMenu();
                loggedIn = true;
            }
            else if (data == "false")
            {
                loginInfo.DisplayLoginFailedMenu();
                loggedIn = false;
            }
            else if (data == "new")
            {
                loginInfo.DisplayLoginNewUserMenu();
                loggedIn = true;
            }
            else if (data == "newfailed")
            {
                loginInfo.DisplayLoginNewUserFailedMenu();
                loggedIn = false;
            }
            else if (data == "sessionFailed")
            {
                loginInfo.DisplaySessionFailedMenu();
                resetData();
            }
            else if (data == "showLobby")
            {
                lobbyInfo.DisplayLobby();
                resetData();
            }
            else if (data == "chat-message")
            {
                lobbyInfo.UpdateChatlog(chatUser, chatMessage);
                resetData();
            }
        }
    }

    public void OnDestroy() {
      Debug.Log("ending receive thread");
      StopReceivingMessage();
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
        //Debug.Log("Waiting for response...");
        // Receive the response from the remote device.
        Receive(recv_so);
        recv_so.receiveDone.WaitOne(time);
    }

	//public void StartClient (string username, string password)
	  public void StartClient(string message, string username, string password)
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
            myUsername = username;

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
    
    public void StartMultiplayerGame()
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
                state.sb.Length = 0;
                state.sb.Capacity = 0;
                state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
                string content = state.sb.ToString();
                Debug.Log("Content: " + content);

                char[] delimiterChars = { ',','<','>'};

                string[] messageToCheck = content.Split(delimiterChars);

                //Debug.Log("split: " + messageToCheck[1] + " " + messageToCheck[2] + " " + messageToCheck[3] + " " + messageToCheck[4]);

                if (messageToCheck[0] == "login")
                {
                    if (messageToCheck[1] == "true")
                    {
                        Debug.Log("Login successful!");
                        setData(messageToCheck[1]);
                        myUsername = LoginScript.getUsername();
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
                        myUsername = LoginScript.getUsername();
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
                    //Set the number of players here
                    Debug.Log("Getting \"start-game\" message...  " + messageToCheck[0] + "   " + messageToCheck[1]);

                    numberOfPlayers = messageToCheck[1];

                    Debug.Log("Sending the number of players: " + numberOfPlayers);

                    //async.allowSceneActivation = true;
                    //myMenu.StartMultiplayerGame();
                    setStartGame(true);  // Created this function to be able start the Multiplayer Scene from inside ReceiveCallback
                    //Application.LoadLevel("Multiplayer Scene");  // Was getting an error trying to call this from here
                }
                else if (messageToCheck[0] == "start-timer")
                {
                    Debug.Log("Getting \"start-timer\" message...");

                    // Spawn obstacles
                    spawnObstacles = true;

                    // Start the timer
                    GameUI.restartGame();

                    // Unrestrict player movement
                    Frog.restrictMovement = false;
                }
                else if (messageToCheck[0] == "timer")
                {
                    Debug.Log("Current Timer: " + messageToCheck[1]);
                    GameUI.setTimer(float.Parse(messageToCheck[1]));
                }
                // "frogPosition, 1, x1, y1, 2, x2, y2, 3, x3, y3, 4, x4, y4<EOF>"
                else if (messageToCheck[0] == "frogPosition")
                {
                    Vector2 newPosition1;
                    Vector2 newPosition2;

                    // GOAL 4 PLAYERS
                    Vector2 newPosition3;
                    Vector2 newPosition4;

                    Debug.Log("Frog positions: " + messageToCheck[1] + " " + messageToCheck[2] + " " + messageToCheck[3] + " " + messageToCheck[4] + " " + messageToCheck[5] + " " + messageToCheck[6] + " " + messageToCheck[7] + " " + messageToCheck[8] + " " + messageToCheck[9] + " " + messageToCheck[10] + messageToCheck[11]);
                    if (numberOfPlayers == "2")
                    {
                        newPosition1.x = float.Parse(messageToCheck[1]);
                        newPosition1.y = float.Parse(messageToCheck[2]);

                        newPosition2.x = float.Parse(messageToCheck[4]);
                        newPosition2.y = float.Parse(messageToCheck[5]);

                        newPosition3.x = 0;
                        newPosition3.y = 0;

                        newPosition4.x = 0;
                        newPosition4.y = 0;

                        Debug.Log("Sending positions: " + newPosition1 + " " + newPosition2 + " " + newPosition3 + " " + newPosition4);
                        PlayerSpawner.setPlayerPositions(newPosition1, newPosition2, newPosition3, newPosition4);
                    }
                    else if (numberOfPlayers == "3")
                    {
                        newPosition1.x = float.Parse(messageToCheck[1]);
                        newPosition1.y = float.Parse(messageToCheck[2]);

                        newPosition2.x = float.Parse(messageToCheck[4]);
                        newPosition2.y = float.Parse(messageToCheck[5]);

                        newPosition3.x = float.Parse(messageToCheck[7]);
                        newPosition3.y = float.Parse(messageToCheck[8]);

                        newPosition4.x = 0;
                        newPosition4.y = 0;

                        Debug.Log("Sending positions: " + newPosition1 + " " + newPosition2 + " " + newPosition3 + " " + newPosition4);
                        PlayerSpawner.setPlayerPositions(newPosition1, newPosition2, newPosition3, newPosition4);
                    }
                    else if (numberOfPlayers == "4")
                    {
                        newPosition1.x = float.Parse(messageToCheck[1]);
                        newPosition1.y = float.Parse(messageToCheck[2]);

                        newPosition2.x = float.Parse(messageToCheck[4]);
                        newPosition2.y = float.Parse(messageToCheck[5]);

                        newPosition3.x = float.Parse(messageToCheck[7]);
                        newPosition3.y = float.Parse(messageToCheck[8]);

                        newPosition4.x = float.Parse(messageToCheck[10]);
                        newPosition4.y = float.Parse(messageToCheck[11]);

                        Debug.Log("Sending positions: " + newPosition1 + " " + newPosition2 + " " + newPosition3 + " " + newPosition4);
                        PlayerSpawner.setPlayerPositions(newPosition1, newPosition2, newPosition3, newPosition4);
                    }
                }
                else if (messageToCheck[0] == "join-session")
                {
                    if (messageToCheck[1] == "true")
                        setData("showLobby");
                    else
                        setData("sessionFailed");
                }
                else if (messageToCheck[0] == "disconnected")
                {
                    // GOAL 4 PLAYERS
                    Debug.Log("Receiving player disconnect message from the server!");
                    Holder.DCPlayer(messageToCheck[1]);
                }
                else if (messageToCheck[0] == "score")
                {
                    GameUI.score1 = messageToCheck[1];
                    GameUI.score2 = messageToCheck[2];
                    GameUI.score3 = messageToCheck[3];
                    GameUI.score4 = messageToCheck[4];
                }
                // "gameOver, result, score1, score2, score3, score4<EOF>"
                else if (messageToCheck[0] == "gameOver")
                {
                    Debug.Log("My player number is: " + myPlayerNumber);
                    Debug.Log("My result is: " + messageToCheck[1]);

                    //GameOverScript.result = messageToCheck[1];
                    //GameOverScript.setGameOverResult(messageToCheck[1]);
                    //gameOverResult = messageToCheck[1];
                    Holder.finalGameOverResult = messageToCheck[1];

                    setData("gameOver");
                    GameOverScript.gameOverReceived = true;

                    Debug.Log("Sending this RESULT: " + Holder.finalGameOverResult);

                    GameUI.score1 = messageToCheck[2];
                    GameUI.score2 = messageToCheck[3];
                    GameUI.score3 = messageToCheck[3];
                    GameUI.score4 = messageToCheck[4];
                }
                else if (messageToCheck[0] == "chat-message")
                {
                    chatUser = messageToCheck[1];

                    string wholeMessage = "";
                    int i = 2;
                    while (messageToCheck[i] != "EOF")
                    {
						wholeMessage = wholeMessage + messageToCheck[i];

                        // Makes sure commas don't get parsed out
						if (messageToCheck[i+1] != "EOF"){
							wholeMessage = wholeMessage + ",";
						}
						i++;
                    }

                    chatMessage = wholeMessage;
                    setData("chat-message");
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

    public void ThreadReceive() {
        while (true) {
          //Debug.Log("In ThreadReceive");
          ReceiveMSG(0);
          Thread.Sleep(500);
          if (stopThread) {
            break;
          }
        }
    }

    public void StartReceivingMessages() {
      if (!threadStarted) {
        t = new Thread(new ThreadStart(ThreadReceive));
        t.Start();
      }

      threadStarted = true;
    }

    public void StopReceivingMessage() {
      stopThread = true;
    }

    public bool HasQueueMessage() {
      return !(messageQueue.Count == 0);
    }

    public QueueMessage ConsumeQueueMessage() {
      return messageQueue.Dequeue();
    }

    public bool HasReadyMessage() {
      return !(readyQueue.Count == 0);
    }

    public ReadyMessage ConsumeReadyMessage() {
      return readyQueue.Dequeue();
    }

    public static class Holder
    {
        public static string finalGameOverResult = "";

        public static void DCPlayer(string thisPlayerNumber)
        {
            if (thisPlayerNumber == "1")
            {
                GameUI.score1 = "DISCONNECTED";
            }
            else if (thisPlayerNumber == "2")
            {
                GameUI.score2 = "DISCONNECTED";
            }
            else if (thisPlayerNumber == "3")
            {
                GameUI.score3 = "DISCONNECTED";
            }
            else if (thisPlayerNumber == "4")
            {
                GameUI.score4 = "DISCONNECTED";
            }

            PlayerSpawner.setPlayerDC(thisPlayerNumber, true);
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