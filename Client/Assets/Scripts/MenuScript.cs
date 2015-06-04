using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuScript : MonoBehaviour {

	public Canvas quitMenu;
	public Canvas creditsMenu;
	public Button playButton;
	public Button creditsButton;
	public Button quitButton;
    public Button multiplayerButton;
    public Canvas startMenu;

	public AudioClip buttonClickSFX;
	public AudioClip buttonHoverSFX;

    LoginScript loginInfo;
    GameObject loginMenu;

    AsyncOperation async;

	// Use this for initialization
	void Start () {
	
		quitMenu = quitMenu.GetComponent<Canvas> ();
		playButton = playButton.GetComponent<Button> ();
		creditsButton = creditsButton.GetComponent<Button> ();
        multiplayerButton = multiplayerButton.GetComponent<Button>();
		quitButton = quitButton.GetComponent<Button> ();
        startMenu = startMenu.GetComponent<Canvas>();

        loginMenu = GameObject.FindGameObjectWithTag("Login Menu");
        loginInfo = loginMenu.GetComponent<LoginScript>();

		quitMenu.enabled = false;
		creditsMenu.enabled = false;
        startMenu.enabled = true;

        async = Application.LoadLevelAsync("Main Scene");
        // Set this false to wait changing the scene
        async.allowSceneActivation = false;
	}
	
	public void QuitPressed()
	{
		quitMenu.enabled = true;
		playButton.enabled = false;
		creditsButton.enabled = false;
		quitButton.enabled = false;
        multiplayerButton.enabled = false;
        loginInfo.loggedInMenu.enabled = false;

		GetComponent<AudioSource>().PlayOneShot(buttonClickSFX);
	}

	public void CreditsPressed()
	{
        Debug.Log("Displaying Credits Menu...");
		creditsMenu.enabled = true;
		playButton.enabled = false;
		creditsButton.enabled = false;
		quitButton.enabled = false;
        multiplayerButton.enabled = false;
        loginInfo.loggedInMenu.enabled = false;

		GetComponent<AudioSource>().PlayOneShot(buttonClickSFX);
	}

	public void NoPressed()
	{
		quitMenu.enabled = false;
		playButton.enabled = true;
		creditsButton.enabled = true;
		quitButton.enabled = true;
        multiplayerButton.enabled = true;
        loginInfo.loggedInMenu.enabled = true;

		GetComponent<AudioSource>().PlayOneShot(buttonClickSFX);
	}

	public void ContinuePressed()
	{
		creditsMenu.enabled = false;
		playButton.enabled = true;
		creditsButton.enabled = true;
		quitButton.enabled = true;
        multiplayerButton.enabled = true;
        loginInfo.loggedInMenu.enabled = true;

		GetComponent<AudioSource>().PlayOneShot(buttonClickSFX);
	}

	public void StartGame()
	{
        Debug.Log("Starting single player game...");
		//GetComponent<AudioSource>().PlayOneShot(buttonClickSFX);

        //ClientScript.currentScene = "Main Scene";
        //Application.LoadLevelAsync("Main Scene");

        //async.allowSceneActivation = true;
        //Application.LoadLevel("Main Scene");

        ClientScript.startSingleGame = true;
	}

	public void QuitGame()
	{
		GetComponent<AudioSource>().PlayOneShot(buttonClickSFX);

		Application.Quit ();
	}

	public void PlayButtonHover()
	{
		GetComponent<AudioSource>().PlayOneShot(buttonHoverSFX);
	}

    public void StartMultiplayerGame()
    {
        GetComponent<AudioSource>().PlayOneShot(buttonClickSFX);

        ClientScript.currentScene = "Multiplayer Scene";
        Application.LoadLevelAsync("Multiplayer Scene");
    }
}
