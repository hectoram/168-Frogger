using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuScript : MonoBehaviour {

	public Canvas quitMenu;
	public Canvas creditsMenu;
	public Button playButton;
	public Button creditsButton;
	public Button quitButton;

	// Use this for initialization
	void Start () {
	
		quitMenu = quitMenu.GetComponent<Canvas> ();
		playButton = playButton.GetComponent<Button> ();
		creditsButton = creditsButton.GetComponent<Button> ();
		quitButton = quitButton.GetComponent<Button> ();
		quitMenu.enabled = false;
		creditsMenu.enabled = false;
	}
	
	public void QuitPressed()
	{
		quitMenu.enabled = true;
		playButton.enabled = false;
		creditsButton.enabled = false;
		quitButton.enabled = false;
	}

	public void CreditsPressed()
	{
		creditsMenu.enabled = true;
		playButton.enabled = false;
		creditsButton.enabled = false;
		quitButton.enabled = false;
	}

	public void NoPressed()
	{
		quitMenu.enabled = false;
		playButton.enabled = true;
		creditsButton.enabled = true;
		quitButton.enabled = true;
	}

	public void ContinuePressed()
	{
		creditsMenu.enabled = false;
		playButton.enabled = true;
		creditsButton.enabled = true;
		quitButton.enabled = true;
	}

	public void StartGame()
	{
		Application.LoadLevel ("Main Scene");
	}

	public void QuitGame()
	{
		Application.Quit ();
	}
}
