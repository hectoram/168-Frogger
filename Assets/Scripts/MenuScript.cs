using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuScript : MonoBehaviour {

	public Canvas quitMenu;
	public Button playButton;
	public Button quitButton;

	// Use this for initialization
	void Start () {
	
		quitMenu = quitMenu.GetComponent<Canvas> ();
		playButton = playButton.GetComponent<Button> ();
		quitButton = quitButton.GetComponent<Button> ();
		quitMenu.enabled = false;
	}
	
	public void QuitPressed()
	{
		quitMenu.enabled = true;
		playButton.enabled = false;
		quitButton.enabled = false;
	}

	public void NoPressed()
	{
		quitMenu.enabled = false;
		playButton.enabled = true;
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
