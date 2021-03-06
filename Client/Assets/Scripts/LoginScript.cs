﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoginScript : MonoBehaviour {
	
	public Canvas loginMenu;
	public Canvas loginFailedMenu;
	public Canvas loginSuccessMenu;
	public Canvas loginNewUserMenu;
	public Canvas loginNewUserFailedMenu;
    public Canvas loggedInMenu;
    public Canvas lobbyMenu;
    public Canvas sessionFailedMenu;

    public InputField lobbyName;
    
    public Text currentUsername;
    public static string myUsername;

	public InputField username;
	public InputField password;

	public string port = "11000";
	public string ipAddress = "127.0.0.1";

	public bool connectionStarted;
	ClientScript clientManager;
    GameObject networking;

    GameObject menuGUI;
    MenuScript menu;

    public static string getUsername()
    {
        return myUsername;
    }

	// Use this for initialization
	public void Start ()
    {
        DontDestroyOnLoad(this);

		loginMenu = loginMenu.GetComponent<Canvas> ();
		loginFailedMenu = loginFailedMenu.GetComponent<Canvas> ();
		loginSuccessMenu = loginSuccessMenu.GetComponent<Canvas> ();
		loginNewUserMenu = loginNewUserMenu.GetComponent<Canvas> ();
        loginNewUserFailedMenu = loginNewUserFailedMenu.GetComponent<Canvas>();
        loggedInMenu = loggedInMenu.GetComponent<Canvas>();
        lobbyMenu = lobbyMenu.GetComponent<Canvas>();
        sessionFailedMenu = sessionFailedMenu.GetComponent<Canvas>();

        menuGUI = GameObject.FindGameObjectWithTag("Menus");
        menu = menuGUI.GetComponent<MenuScript>();

        lobbyName = lobbyName.GetComponent<InputField>();

        currentUsername = currentUsername.GetComponent<Text> ();
        currentUsername.text = "";

		username = username.GetComponent<InputField> ();
		password = password.GetComponent<InputField> ();

        networking = GameObject.FindGameObjectWithTag("Networking");
        clientManager = networking.GetComponent<ClientScript> ();
		connectionStarted = false;

        if (!clientManager.getLoggedIn())
        {
            loginMenu.enabled = true;
            loginFailedMenu.enabled = false;
            loginSuccessMenu.enabled = false;
            loginNewUserMenu.enabled = false;
            loginNewUserFailedMenu.enabled = false;
            loggedInMenu.enabled = false;
            sessionFailedMenu.enabled = false;
            lobbyMenu.enabled = false;
        }
        else
        {
            loginMenu.enabled = false;
            loginFailedMenu.enabled = false;
            loginSuccessMenu.enabled = false;
            loginNewUserMenu.enabled = false;
            loginNewUserFailedMenu.enabled = false;
            loggedInMenu.enabled = true;
            lobbyMenu.enabled = false;
            sessionFailedMenu.enabled = false;
            currentUsername.text = clientManager.getUsername();
        }
		
	}

	public void LogIn()
	{
        GetComponent<AudioSource>().PlayOneShot(menu.buttonClickSFX);

        Debug.Log("CALLING LOGIN!!!!!!!!!!!!!!!!!!!!!!!");
		    //clientManager.Send("userLogin," + username.text + "," + password.text + "<EOF>");
		    if (!connectionStarted) {
                clientManager.StartClient("userLogin", username.text, password.text);
                ClientScript.myUsername = username.text;
			    connectionStarted = !connectionStarted;
		    } else
		    {
			    clientManager.resetData();
			    clientManager.Send("userLogin," + username.text + "," + password.text + "<EOF>");
                ClientScript.myUsername = username.text;
		    }
			
		    // Testing ability to connect to the server
		    //Network.Connect (ipAddress, port);

            //clientManager.StartClient("userLogin", username.text, password.text);
        myUsername = username.text;
        clientManager.StartReceivingMessages();
	}

    public void CreateUser()
    {
        GetComponent<AudioSource>().PlayOneShot(menu.buttonClickSFX);

        if (!connectionStarted)
        {
            clientManager.StartClient("userCreate", username.text, password.text);
            connectionStarted = !connectionStarted;
            ClientScript.myUsername = username.text;
        }
        else
        {
            clientManager.resetData();
            clientManager.Send("userCreate," + username.text + "," + password.text + "<EOF>");
            ClientScript.myUsername = username.text;
        }
        //clientManager.StartClient("userCreate", username.text, password.text);
        myUsername = username.text;
    }

	public void DisplayLoginMenu()
	{
        GetComponent<AudioSource>().PlayOneShot(menu.buttonClickSFX);

        Debug.Log("Displaying Login Menu...");
		loginMenu.enabled = true;
		loginFailedMenu.enabled = false;
		loginSuccessMenu.enabled = false;
		loginNewUserMenu.enabled = false;
		loginNewUserFailedMenu.enabled = false;
        loggedInMenu.enabled = false;
		clientManager.resetData();
	}

    public void DisplaySessionFailedMenu()
    {
        Debug.Log("Displaying Session Failed Menu...");
        sessionFailedMenu.enabled = true;
    }

    public void DisplayLobbyMenu()
    {
        GetComponent<AudioSource>().PlayOneShot(menu.buttonClickSFX);

        Debug.Log("Displaying session menu...");
        lobbyMenu.enabled = true;
        sessionFailedMenu.enabled = false;
        loggedInMenu.enabled = false;
        loginFailedMenu.enabled = false;
        lobbyName.enabled = true;
    }

    public void DismissLobbyMenu()
    {
        GetComponent<AudioSource>().PlayOneShot(menu.buttonClickSFX);

        Debug.Log("Dismissing session menu...");
        lobbyMenu.enabled = false;
        loggedInMenu.enabled = true;
    }

    public void ConnectLobby()
    {
        GetComponent<AudioSource>().PlayOneShot(menu.buttonClickSFX);

        Debug.Log("Requesting to join session: " + lobbyName.text);
        clientManager.Send("join-session," + lobbyName.text + "<EOF>");
    }
	
	public void DisplayLoginFailedMenu()
	{
        Debug.Log("Displaying Login Failed Menu...");
		loginMenu.enabled = false;
		loginFailedMenu.enabled = true;
		loginSuccessMenu.enabled = false;
		loginNewUserMenu.enabled = false;
		loginNewUserFailedMenu.enabled = false;
        loggedInMenu.enabled = false;
		clientManager.resetData();
	}
	
	public void DisplayLoginSuccessMenu()
	{
        Debug.Log("Displaying Login Success Menu...");
		loginMenu.enabled = false;
		loginFailedMenu.enabled = false;
		loginSuccessMenu.enabled = true;
		loginNewUserMenu.enabled = false;
		loginNewUserFailedMenu.enabled = false;
        loggedInMenu.enabled = false;
		clientManager.resetData();
	}
	
	public void DisplayLoginNewUserMenu()
	{
        Debug.Log("Displaying New User Menu...");
		loginMenu.enabled = false;
		loginFailedMenu.enabled = false;
		loginSuccessMenu.enabled = false;
		loginNewUserMenu.enabled = true;
		loginNewUserFailedMenu.enabled = false;
        loggedInMenu.enabled = false;
		clientManager.resetData();
	}

	public void DisplayLoginNewUserFailedMenu()
	{
        Debug.Log("Displaying New User Failed Menu...");
		loginMenu.enabled = false;
		loginFailedMenu.enabled = false;
		loginSuccessMenu.enabled = false;
		loginNewUserMenu.enabled = false;
		loginNewUserFailedMenu.enabled = true;
        loggedInMenu.enabled = false;
		clientManager.resetData();
	}


	public void DisplayMainMenu()
	{
        GetComponent<AudioSource>().PlayOneShot(menu.buttonClickSFX);

        Debug.Log("Displaying Main Menu...");
		loginMenu.enabled = false;
		loginFailedMenu.enabled = false;
		loginSuccessMenu.enabled = false;
		loginNewUserMenu.enabled = false;
		loginNewUserFailedMenu.enabled = false;
        loggedInMenu.enabled = true;
        lobbyMenu.enabled = false;
        currentUsername.text = username.text;
		clientManager.resetData();
	}

    public void LogoutPressed()
    {
        GetComponent<AudioSource>().PlayOneShot(menu.buttonClickSFX);

        Debug.Log("Logging out...");
        clientManager.Send("userLogout," + currentUsername.text + "<EOF>");
        DisplayLoginMenu();
    }

    // GUI keyboard shortcuts
    void OnGUI()
    {
        if (Event.current.Equals(Event.KeyboardEvent("Return")) && loginMenu.enabled == true) // Enter/Return to login
        {
            LogIn();
        }
        else if (Event.current.Equals(Event.KeyboardEvent("Tab")) && loginMenu.enabled == true) // Tab to password input
        {
            password.Select();
        }
        else if (Event.current.Equals(Event.KeyboardEvent("Return")) && lobbyMenu.enabled == true) // Enter/Return to submit lobby
        {
            ConnectLobby();
        }
    }

}
