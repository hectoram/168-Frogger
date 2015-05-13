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

    string[] queuedPlayers = {"null", "null", "null", "null"};

    ClientScript clientManager;
    GameObject networking;

	// Use this for initialization
	void Start () {

        lobbyMenu = lobbyMenu.GetComponent<Canvas>();

        lobbyMenu.enabled = false;

        networking = GameObject.FindGameObjectWithTag("Networking");
        clientManager = networking.GetComponent<ClientScript>();

        username1 = username1.GetComponent<Text>();
        username2 = username2.GetComponent<Text>();
        username3 = username3.GetComponent<Text>();
        username4 = username4.GetComponent<Text>();

        username1.text = "EMPTY SLOT";
        username2.text = "EMPTY SLOT";
        username3.text = "EMPTY SLOT";
        username4.text = "EMPTY SLOT";

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
	}
	
	// Update is called once per frame
	void Update () {

        if (lobbyMenu.enabled)
        {
            InvokeRepeating("UpdateQueue", 3, 3);
        }
	}

    void UpdateQueue()
    {
        clientManager.SendMSG("queue<EOF>", 3000);
        clientManager.ReceiveMSG(3000);

        if (queuedPlayers[0] != "null")
            username1.text = queuedPlayers[0];
        if (queuedPlayers[1] != "null")
            username2.text = queuedPlayers[1];
        if (queuedPlayers[2] != "null")
            username3.text = queuedPlayers[2];
        if (queuedPlayers[3] != "null")
            username4.text = queuedPlayers[3];

        clientManager.SendMSG("ready<EOF>", 3000);
        clientManager.ReceiveMSG(3000);

        if (queuedPlayers[0] != "null")
        {
            ready1.text = "READY";
            ready1.color = Color.green;
        }
        if (queuedPlayers[1] != "null")
        {
            ready2.text = "READY";
            ready2.color = Color.green;
        }
        if (queuedPlayers[2] != "null")
        {
            ready2.text = "READY";
            ready2.color = Color.green;
        }
        if (queuedPlayers[3] != "null")
        {
            ready2.text = "READY";
            ready2.color = Color.green;
        }
    }

    public void DisplayLobbyMenu()
    {
        lobbyMenu.enabled = true;
    }

    public void DismissLobbyMenu()
    {
        lobbyMenu.enabled = false;
    }
}
