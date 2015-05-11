using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class GameOverScript : MonoBehaviour {
	
	public Canvas winMenu;
	public Canvas loseMenu;
    public Canvas scoreMenu;
    public Text finalScoreText;

	public Button yesWinButton;
	public Button noWinButton;
	public Button yesLoseButton;
	public Button noLoseButton;

	public AudioClip loserSFX;
	public AudioClip winnerSFX;
	public AudioClip buttonClickSFX;
	public AudioClip buttonHoverSFX;
	
	// Use this for initialization
	void Start () {
		
		winMenu = winMenu.GetComponent<Canvas> ();
		loseMenu = loseMenu.GetComponent<Canvas> ();
        scoreMenu = scoreMenu.GetComponent<Canvas>();
        finalScoreText = finalScoreText.GetComponent<Text>();

		yesWinButton = yesWinButton.GetComponent<Button> ();
		noWinButton = noWinButton.GetComponent<Button> ();
		yesLoseButton = yesLoseButton.GetComponent<Button> ();
		noLoseButton = noLoseButton.GetComponent<Button> ();

		winMenu.enabled = false;
		loseMenu.enabled = false;
        scoreMenu.enabled = false;
	}
	
	public void YesPressed()
	{
		GetComponent<AudioSource>().PlayOneShot(buttonClickSFX);

		Application.LoadLevel ("Main Scene");
	}
	
	public void NoPressed()
	{
		GetComponent<AudioSource>().PlayOneShot(buttonClickSFX);

		Application.LoadLevel ("Menu Scene");
	}

	public void ShowWinMenu()
	{
		winMenu.enabled = true;
		GetComponent<AudioSource>().PlayOneShot(winnerSFX);
	}

	public void ShowLoseMenu()
	{
		loseMenu.enabled = true;
		GetComponent<AudioSource>().PlayOneShot(loserSFX);
	}

    public void ShowScoreMenu()
    {
        if (!scoreMenu.enabled)
        {
            scoreMenu.enabled = true;
            GetComponent<AudioSource>().PlayOneShot(winnerSFX);
        }
    }

	public void PlayButtonHover()
	{
		GetComponent<AudioSource>().PlayOneShot(buttonHoverSFX);
	}
}
