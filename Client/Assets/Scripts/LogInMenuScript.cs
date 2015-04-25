using UnityEngine;
using System.Collections;

public class LogInMenuScript : MonoBehaviour {

	string loginURL = "http://127.0.0.1/login.php";

	string username = "";
	string password = "";
	string label = "";

	void OnGUI(){
		GUI.Window (0, new Rect (Screen.width / 4, Screen.height / 4, Screen.width / 2, Screen.height / 2 - 70), LoginWindow, "Login");
	}


	void LoginWindow (int windowID){
		GUI.Label (new Rect (140,40,130,180), "Username");
		username = GUI.TextField (new Rect(25, 60, 375, 30), username);
		GUI.Label (new Rect (140,92,130,100), "Password");
		password = GUI.PasswordField (new Rect(25, 115, 375, 30), password, '*');

		GUI.Button (new Rect (25, 160, 375, 50), "Login");

		GUI.Label (new Rect(55, 222, 250, 100), label);

	}

}
