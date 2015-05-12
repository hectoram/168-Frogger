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

	// Use this for initialization
	void Start () {

        lobbyMenu = lobbyMenu.GetComponent<Canvas>();

        lobbyMenu.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
	
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
