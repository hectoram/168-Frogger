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

	public AudioClip buttonClickSFX;
	public AudioClip buttonHoverSFX;

	// Use this for initialization
	void Start () {
	
		quitMenu = quitMenu.GetComponent<Canvas> ();
		playButton = playButton.GetComponent<Button> ();
		creditsButton = creditsButton.GetComponent<Button> ();
		quitButton = quitButton.GetComponent<Button> ();
        multiplayerButton = multiplayerButton.GetComponent<Button>();

		quitMenu.enabled = false;
		creditsMenu.enabled = false;
	}
	
	public void QuitPressed()
	{
		quitMenu.enabled = true;
		playButton.enabled = false;
		creditsButton.enabled = false;
		quitButton.enabled = false;
        multiplayerButton.enabled = false;

		GetComponent<AudioSource>().PlayOneShot(buttonClickSFX);
	}

	public void CreditsPressed()
	{
		creditsMenu.enabled = true;
		playButton.enabled = false;
		creditsButton.enabled = false;
		quitButton.enabled = false;
        multiplayerButton.enabled = false;

		GetComponent<AudioSource>().PlayOneShot(buttonClickSFX);
	}

	public void NoPressed()
	{
		quitMenu.enabled = false;
		playButton.enabled = true;
		creditsButton.enabled = true;
		quitButton.enabled = true;
        multiplayerButton.enabled = true;

		GetComponent<AudioSource>().PlayOneShot(buttonClickSFX);
	}

	public void ContinuePressed()
	{
		creditsMenu.enabled = false;
		playButton.enabled = true;
		creditsButton.enabled = true;
		quitButton.enabled = true;
        multiplayerButton.enabled = true;

		GetComponent<AudioSource>().PlayOneShot(buttonClickSFX);
	}

	public void StartGame()
	{
		GetComponent<AudioSource>().PlayOneShot(buttonClickSFX);

		Application.LoadLevel ("Main Scene");
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

        Application.LoadLevel("Multiplayer Scene");
    }
}
