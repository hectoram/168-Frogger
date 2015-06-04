using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class GameOverScript : MonoBehaviour {
	
	public Canvas winMenu;
	public Canvas loseMenu;
    public Canvas scoreMenu;
    public Text finalScoreText;
    public Text resultText;

    public string result = "";

    //public Canvas multiScoreMenu;

	public Button yesWinButton;
	public Button noWinButton;
	public Button yesLoseButton;
	public Button noLoseButton;

	public AudioClip loserSFX;
	public AudioClip winnerSFX;
	public AudioClip buttonClickSFX;
	public AudioClip buttonHoverSFX;

    GameObject networking;
    ClientScript clientManager;

    GameObject gameUI;
    GameUI menu;

    static AsyncOperation async;

    public static bool gameOverReceived = false;

    public void setGameOverResult(string newResult)
    {
        result = newResult;
    }
	
	// Use this for initialization
	void Start () {
		
		winMenu = winMenu.GetComponent<Canvas> ();
		loseMenu = loseMenu.GetComponent<Canvas> ();
        scoreMenu = scoreMenu.GetComponent<Canvas>();
        finalScoreText = finalScoreText.GetComponent<Text>();
        resultText = resultText.GetComponent<Text>();

		yesWinButton = yesWinButton.GetComponent<Button> ();
		noWinButton = noWinButton.GetComponent<Button> ();
		yesLoseButton = yesLoseButton.GetComponent<Button> ();
		noLoseButton = noLoseButton.GetComponent<Button> ();

        networking = GameObject.FindGameObjectWithTag("Networking");
        clientManager = networking.GetComponent<ClientScript>();

        gameUI = GameObject.FindGameObjectWithTag("Menus");
        menu = gameUI.GetComponent<GameUI>();

		winMenu.enabled = false;
		loseMenu.enabled = false;
        scoreMenu.enabled = false;

        async = Application.LoadLevelAsync("Menu Scene");
        // Set this false to wait changing the scene
        async.allowSceneActivation = false;
	}
	
    void Update()
    {
        if (gameOverReceived)
            ShowScoreMenu();
    }

	public void YesPressed()
	{
		GetComponent<AudioSource>().PlayOneShot(buttonClickSFX);

		Application.LoadLevel ("Main Scene");
	}
	
	public void NoPressed()
	{
		GetComponent<AudioSource>().PlayOneShot(buttonClickSFX);

        Debug.Log("Loading Main Menu from in-game...");
		//Application.LoadLevel ("Menu Scene");
        //async.allowSceneActivation = true;

        ClientScript.returnMainMenu = true;
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
            Debug.Log("Received RESULT: " + ClientScript.Holder.finalGameOverResult);
            if (ClientScript.Holder.finalGameOverResult == "won")
            {
                resultText.color = Color.green;
                resultText.text = "YOU WON!";
            }
            else if (ClientScript.Holder.finalGameOverResult == "lost")
            {
                resultText.color = Color.red;
                resultText.text = "YOU LOST!";
            }
            else if (ClientScript.Holder.finalGameOverResult == "tie")
            {
                resultText.color = Color.yellow;
                resultText.text = "IT'S A TIE!";
            }

            Debug.Log("Result text says:  " + resultText.text);

            scoreMenu.enabled = true;
            GetComponent<AudioSource>().PlayOneShot(winnerSFX);
        }
    }

	public void PlayButtonHover()
	{
		GetComponent<AudioSource>().PlayOneShot(buttonHoverSFX);
	}

    public void LoadMultiplayerLevel()
    {
        Application.LoadLevel("Multiplayer Level");
    }
}
