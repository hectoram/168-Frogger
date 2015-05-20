using UnityEngine;
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
	public Canvas loginNewUserFailedMenu; // Ceci
    public Canvas loggedInMenu;
    
    public Text currentUsername;
    public static string myUsername;

	public InputField username;
	public InputField password;

	public string port = "11000";
	public string ipAddress = "127.0.0.1";

	public bool connectionStarted;
	ClientScript clientManager;
    GameObject networking;

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
        loginNewUserFailedMenu = loginNewUserFailedMenu.GetComponent<Canvas>(); //Ceci
        loggedInMenu = loggedInMenu.GetComponent<Canvas>();

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
        }
        else
        {
            loginMenu.enabled = false;
            loginFailedMenu.enabled = false;
            loginSuccessMenu.enabled = false;
            loginNewUserMenu.enabled = false;
            loginNewUserFailedMenu.enabled = false;
            loggedInMenu.enabled = true;
            currentUsername.text = clientManager.getUsername();
        }
		
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
        myUsername = username.text;
	}

    public void CreateUser()
    {
        if (!connectionStarted)
        {
            clientManager.StartClient("userCreate", username.text, password.text);
            connectionStarted = !connectionStarted;
        }
        else
        {
            clientManager.resetData();
            clientManager.Send("userCreate," + username.text + "," + password.text + "<EOF>");
        }
        //clientManager.StartClient("userCreate", username.text, password.text);
        myUsername = username.text;
    }

	public void DisplayLoginMenu()
	{
        Debug.Log("Displaying Login Menu...");
		loginMenu.enabled = true;
		loginFailedMenu.enabled = false;
		loginSuccessMenu.enabled = false;
		loginNewUserMenu.enabled = false;
		loginNewUserFailedMenu.enabled = false;
        loggedInMenu.enabled = false;
		clientManager.resetData();
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
        Debug.Log("Displaying Main Menu...");
		loginMenu.enabled = false;
		loginFailedMenu.enabled = false;
		loginSuccessMenu.enabled = false;
		loginNewUserMenu.enabled = false;
		loginNewUserFailedMenu.enabled = false;
        loggedInMenu.enabled = true;
        currentUsername.text = username.text;
		clientManager.resetData();
	}

    public void LogoutPressed()
    {
        Debug.Log("Logging out...");
        clientManager.Send("userLogout," + currentUsername.text + "<EOF>");
        DisplayLoginMenu();
    }
}
