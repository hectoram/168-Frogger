using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MultiplayerLobbyScript : MonoBehaviour {

    public Canvas lobbyMenu;

    public Text username1;
    public Text username2;
    public Text username3;
    public Text username4;

    public Text ready1;
    public Text ready2;
    public Text ready3;
    public Text ready4;

    public string[] queuedPlayers = {"null", "null", "null", "null"};
    public string[] readyPlayers = { "null", "null", "null", "null" };

    // Ceci
    public Canvas chatbox;
    public InputField chatmessage;
    public Text chatlog;

    ClientScript clientManager;
    GameObject networking;

    LoginScript loginInfo;
    GameObject loginMenu;

    GameObject menuGUI;
    MenuScript menu;

    bool isReady;
    int playerNumber = 0;

    float updateTime = 10;

    bool isInLobby = false;

    public int getPlayerNumber()
    {
        return playerNumber;
    }

    public bool getIsInLobby()
    {
        return isInLobby;
    }

    public void setIsInLobby(bool value)
    {
        isInLobby = value;
    }

	// Use this for initialization
	void Start () {

        //DontDestroyOnLoad(this);
        //DontDestroyOnLoad(lobbyMenu);

        lobbyMenu = lobbyMenu.GetComponent<Canvas>();

        lobbyMenu.enabled = false;

        networking = GameObject.FindGameObjectWithTag("Networking");
        clientManager = networking.GetComponent<ClientScript>();

        loginMenu = GameObject.FindGameObjectWithTag("Login Menu");
        loginInfo = loginMenu.GetComponent<LoginScript>();

        menuGUI = GameObject.FindGameObjectWithTag("Menus");
        menu = menuGUI.GetComponent<MenuScript>();

        username1 = username1.GetComponent<Text>();
        username2 = username2.GetComponent<Text>();
        username3 = username3.GetComponent<Text>();
        username4 = username4.GetComponent<Text>();

        username1.text = "EMPTY";
        username2.text = "EMPTY";
        username3.text = "EMPTY";
        username4.text = "EMPTY";

        ready1 = ready1.GetComponent<Text>();
        ready2 = ready2.GetComponent<Text>();
        ready3 = ready3.GetComponent<Text>();
        ready4 = ready4.GetComponent<Text>();

        ready1.text = "NOT READY";
        ready2.text = "NOT READY";
        ready3.text = "NOT READY";
        ready4.text = "NOT READY";

        //ready1.color = Color.red;
        //ready2.color = Color.red;
        //ready3.color = Color.red;
        //ready4.color = Color.red;

        isReady = false;

        // For the chatbox
        chatlog = chatlog.GetComponent<Text>();
        chatlog.text = "";

        chatbox = chatbox.GetComponent<Canvas>();
        chatmessage.GetComponent<InputField>();
	}
	
	// Update is called once per frame
	void Update () {
        if (lobbyMenu.enabled)
            isInLobby = true;
        else
            isInLobby = false;

        if(!clientManager.getIsGameInProgress())
        {
            if (lobbyMenu.enabled)
            {
                if (updateTime > 0)
                {
                    updateTime -= Time.deltaTime;
                }
                else
                {
                    UpdateQueue();
                    updateTime = 2;
                }
                //InvokeRepeating("UpdateQueue", 5, 5);
                loginInfo.loggedInMenu.enabled = false;
                loginInfo.loginSuccessMenu.enabled = false;
                loginInfo.loginNewUserMenu.enabled = false;
            }
        } 
	}

    void UpdateQueue()
    {
        clientManager.Send("queue<EOF>");

        if (isReady)
        {
            clientManager.Send("ready<EOF>");
        }

        if (clientManager.HasQueueMessage())
        {
            ClientScript.QueueMessage message = clientManager.ConsumeQueueMessage();
            Debug.Log("QUEUE CHECK:" + "Player 1: " + message.playerOneName + "  Player 2: " + message.playerTwoName);
            queuedPlayers[0] = message.playerOneName;
            queuedPlayers[1] = message.playerTwoName;
            queuedPlayers[2] = message.playerThreeName;
            queuedPlayers[3] = message.playerFourName;
            if (queuedPlayers[0] != "null" && queuedPlayers[0] != "empty")
                username1.text = queuedPlayers[0];
            if (queuedPlayers[1] != "null" && queuedPlayers[1] != "empty")
                username2.text = queuedPlayers[1];
            if (queuedPlayers[2] != "null" && queuedPlayers[2] != "empty")
                username3.text = queuedPlayers[2];
            if (queuedPlayers[3] != "null" && queuedPlayers[3] != "empty")
                username4.text = queuedPlayers[3];

            if (username1.text == loginInfo.username.text)
                playerNumber = 1;
            else if (username1.text == loginInfo.username.text)
                playerNumber = 2;
            else if (username1.text == loginInfo.username.text)
                playerNumber = 3;
            else if (username1.text == loginInfo.username.text)
                playerNumber = 4;
        }

        if (clientManager.HasReadyMessage())
        {
            ClientScript.ReadyMessage message = clientManager.ConsumeReadyMessage();
            Debug.Log("READY CHECK: Player 1: " + message.playerOneReady + "  Player 2: " + message.playerTwoReady + "  Player 3: " + message.playerThreeReady + "  Player 4: " + message.playerFourReady);
            readyPlayers[0] = message.playerOneReady;
            readyPlayers[1] = message.playerTwoReady;
            readyPlayers[2] = message.playerThreeReady;
            readyPlayers[3] = message.playerFourReady;

            Debug.Log("QUEUE CHECK: Player 1: " + readyPlayers[0] + " Player 2: " + readyPlayers[1] + " Player 3: " + readyPlayers[2] + " Player 4: " + readyPlayers[3]);

            if (readyPlayers[0] != "null" && readyPlayers[0] != "empty")
            {
                ready1.text = "READY";
                ready1.color = Color.green;
            }
            if (readyPlayers[1] != "null" && readyPlayers[1] != "empty")
            {
                ready2.text = "READY";
                ready2.color = Color.green;
            }
            if (readyPlayers[2] != "null" && readyPlayers[2] != "empty")
            {
                ready3.text = "READY";
                ready3.color = Color.green;
            }
            if (readyPlayers[3] != "null" && readyPlayers[3] != "empty")
            {
                ready4.text = "READY";
                ready4.color = Color.green;
            }
        }
    }

    public void PlayerReady()
    {
        Debug.Log("You have clicked on ready!");
        isReady = true;
        UpdateQueue();
    }

    public void DisplayLobby()
    {
        GetComponent<AudioSource>().PlayOneShot(menu.buttonClickSFX);

        Debug.Log("You have entered the lobby.");
        lobbyMenu.enabled = true;
        clientManager.setIsPlayerInLobby(true);
        loginInfo.lobbyMenu.enabled = false;
        menu.startMenu.enabled = false;
        UpdateQueue();
    }

    public void DismissLobby()
    {
        GetComponent<AudioSource>().PlayOneShot(menu.buttonClickSFX);

        Debug.Log("You have left the lobby.");
        lobbyMenu.enabled = false;
        clientManager.setIsPlayerInLobby(false);
        menu.startMenu.enabled = true;
        isReady = false;
    }

    // When you hit "SEND" in the chatbox
    public void SendChatMessage()
    {
        GetComponent<AudioSource>().PlayOneShot(menu.buttonClickSFX);

        chatmessage = chatmessage.GetComponent<InputField>();

        if (chatmessage.text == "")
        {
            chatmessage.Select();
            return;
        }

        clientManager.resetData();
        clientManager.Send("chat-message," + chatmessage.text + "<EOF>");
        //Debug.Log("chat-message," + chatmessage.text + "<EOF>");

        UpdateChatlog(clientManager.getUsername(), chatmessage.text);

        chatmessage.text = "";
        chatmessage.Select();
    }

    // Updates the log of chat messages
    public void UpdateChatlog(string user, string message)
    {
        chatlog.text = chatlog.text + user + ": " + message + "\n";
    }

    // Sends the chat message when you hit Return/Enter
    void OnGUI()
    {
        if (Event.current.Equals(Event.KeyboardEvent("Return")))
        {
            GetComponent<AudioSource>().PlayOneShot(menu.buttonClickSFX);
            SendChatMessage();
        }
    }

}
