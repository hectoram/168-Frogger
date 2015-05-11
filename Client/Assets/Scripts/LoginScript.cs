using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

public class LoginScript : MonoBehaviour {
	
	public Canvas loginMenu;
	public Canvas loginFailedMenu;
	public Canvas loginSuccessMenu;
	public Canvas loginNewUserMenu;
	public Canvas loginNewUserFailedMenu; // Ceci
    public Canvas loggedInMenu;
    
    public Text currentUsername;

	public InputField username;
	public InputField password;

	public string port = "11000";
	public string ipAddress = "127.0.0.1";

	public bool connectionStarted;
	ClientScript clientManager;
    GameObject networking;

	// Use this for initialization
	public void Start ()
    {
		loginMenu = loginMenu.GetComponent<Canvas> ();
		loginFailedMenu = loginFailedMenu.GetComponent<Canvas> ();
		loginSuccessMenu = loginSuccessMenu.GetComponent<Canvas> ();
		loginNewUserMenu = loginNewUserMenu.GetComponent<Canvas> ();
        loginNewUserFailedMenu = loginNewUserFailedMenu.GetComponent<Canvas>(); //Ceci
        loggedInMenu = loggedInMenu.GetComponent<Canvas>();

        currentUsername = currentUsername.GetComponent<Text> ();
        currentUsername.text = "";

		username = username.GetComponent<InputField> ();
		password = password.GetComponent<InputField> ();

        networking = GameObject.FindGameObjectWithTag("Networking");
        clientManager = networking.GetComponent<ClientScript> ();
		connectionStarted = false;

		loginMenu.enabled = true;
		loginFailedMenu.enabled = false;
		loginSuccessMenu.enabled = false;
		loginNewUserMenu.enabled = false;
		loginNewUserFailedMenu.enabled = false;
        loggedInMenu.enabled = false;
	}

	public void LogIn()
	{
		//clientManager.Send("userLogin," + username.text + "," + password.text + "<EOF>");
		if (!connectionStarted) {
            clientManager.StartClient("userLogin", username.text, password.text);
			connectionStarted = !connectionStarted;
		} else
		{
			clientManager.resetData();
			clientManager.Send("userLogin," + username.text + "," + password.text + "<EOF>");
		}
			
		// Testing ability to connect to the server
		//Network.Connect (ipAddress, port);

        //clientManager.StartClient("userLogin", username.text, password.text);
	}

	public void DisplayLoginMenu()
	{
		loginMenu.enabled = true;
		loginFailedMenu.enabled = false;
		loginSuccessMenu.enabled = false;
		loginNewUserMenu.enabled = false;
		loginNewUserFailedMenu.enabled = false;
        loggedInMenu.enabled = false;
        Debug.Log("Displaying Login Menu...");
		clientManager.resetData();
	}
	
	public void DisplayLoginFailedMenu()
	{
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
		loginMenu.enabled = false;
		loginFailedMenu.enabled = false;
		loginSuccessMenu.enabled = false;
		loginNewUserMenu.enabled = false;
		loginNewUserFailedMenu.enabled = false;
        loggedInMenu.enabled = true;
        currentUsername.text = username.text;
        Debug.Log("Displaying Main Menu...");
		clientManager.resetData();
	}

    public void LogoutPressed()
    {
        clientManager.Send("userLogout," + currentUsername.text + "<EOF>");
        DisplayLoginMenu();
    }
}
