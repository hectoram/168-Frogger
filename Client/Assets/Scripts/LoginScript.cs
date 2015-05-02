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

	public InputField username;
	public InputField password;

	public string port = "11000";
	public string ipAddress = "127.0.0.1";

	ClientScript clientManager;
    GameObject networking;

	// Use this for initialization
	public void Start ()
    {
		loginMenu = loginMenu.GetComponent<Canvas> ();
		loginFailedMenu = loginFailedMenu.GetComponent<Canvas> ();
		loginSuccessMenu = loginSuccessMenu.GetComponent<Canvas> ();
		loginNewUserMenu = loginNewUserMenu.GetComponent<Canvas> ();

		username = username.GetComponent<InputField> ();
		password = password.GetComponent<InputField> ();

        networking = GameObject.FindGameObjectWithTag("Networking");
        clientManager = networking.GetComponent<ClientScript>();

		loginMenu.enabled = true;
		loginFailedMenu.enabled = false;
		loginSuccessMenu.enabled = false;
		loginNewUserMenu.enabled = false;
	}

	public void LogIn()
	{
		//clientManager.Send("userLogin," + username.text + "," + password.text + "<EOF>");

        clientManager.StartClient(username.text, password.text);

		// Testing ability to connect to the server
		//Network.Connect (ipAddress, port);
	}

	public void DisplayLoginMenu()
	{
		loginMenu.enabled = true;
		loginFailedMenu.enabled = false;
		loginSuccessMenu.enabled = false;
		loginNewUserMenu.enabled = false;
        Debug.Log("Displaying Login Menu...");
		clientManager.resetData();
	}
	
	public void DisplayLoginFailedMenu()
	{
		loginMenu.enabled = false;
		loginFailedMenu.enabled = true;
		loginSuccessMenu.enabled = false;
		loginNewUserMenu.enabled = false;
		clientManager.resetData();
	}
	
	public void DisplayLoginSuccessMenu()
	{
		loginMenu.enabled = false;
		loginFailedMenu.enabled = false;
		loginSuccessMenu.enabled = true;
		loginNewUserMenu.enabled = false;
		clientManager.resetData();
	}
	
	public void DisplayLoginNewUserMenu()
	{
		loginMenu.enabled = false;
		loginFailedMenu.enabled = false;
		loginSuccessMenu.enabled = false;
		loginNewUserMenu.enabled = true;
		clientManager.resetData();
	}
	
	public void DisplayMainMenu()
	{
		loginMenu.enabled = false;
		loginFailedMenu.enabled = false;
		loginSuccessMenu.enabled = false;
		loginNewUserMenu.enabled = false;
        Debug.Log("Displaying Main Menu...");
		clientManager.resetData();
	}
}
