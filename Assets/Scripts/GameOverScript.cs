using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameOverScript : MonoBehaviour {
	
	public Canvas winMenu;
	public Canvas loseMenu;
	public Button yesWinButton;
	public Button noWinButton;
	public Button yesLoseButton;
	public Button noLoseButton;
	
	// Use this for initialization
	void Start () {
		
		winMenu = winMenu.GetComponent<Canvas> ();
		loseMenu = loseMenu.GetComponent<Canvas> ();
		yesWinButton = yesWinButton.GetComponent<Button> ();
		noWinButton = noWinButton.GetComponent<Button> ();
		yesLoseButton = yesLoseButton.GetComponent<Button> ();
		noLoseButton = noLoseButton.GetComponent<Button> ();
		winMenu.enabled = false;
		loseMenu.enabled = false;
	}
	
	public void YesPressed()
	{
		Application.LoadLevel ("Main Scene");
	}
	
	public void NoPressed()
	{
		Application.LoadLevel ("Menu Scene");
	}

	public void ShowWinMenu()
	{
		winMenu.enabled = true;
	}

	public void ShowLoseMenu()
	{
		loseMenu.enabled = true;
	}
}
