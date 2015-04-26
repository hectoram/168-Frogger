using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoginScript : MonoBehaviour {
	
	public Canvas loginMenu;
	public Canvas loginFailedMenu;
	public Canvas loginSuccessMenu;
	public Canvas loginNewUserMenu;

	//public Button playButton;
	//public Button creditsButton;
	//public Button quitButton;
	
	public AudioClip buttonClickSFX;
	public AudioClip buttonHoverSFX;
	
	// Use this for initialization
	void Start () {
		
		loginMenu = loginMenu.GetComponent<Canvas> ();
		loginFailedMenu = loginFailedMenu.GetComponent<Canvas> ();
		loginSuccessMenu = loginSuccessMenu.GetComponent<Canvas> ();
		loginNewUserMenu = loginNewUserMenu.GetComponent<Canvas> ();

		//playButton = playButton.GetComponent<Button> ();
		//creditsButton = creditsButton.GetComponent<Button> ();
		//quitButton = quitButton.GetComponent<Button> ();

		loginMenu.enabled = true;
		loginFailedMenu.enabled = false;
		loginSuccessMenu.enabled = false;
		loginNewUserMenu.enabled = false;
	}
	
	public void DisplayLoginMenu()
	{
		loginMenu.enabled = true;
		loginFailedMenu.enabled = false;
		loginSuccessMenu.enabled = false;
		loginNewUserMenu.enabled = false;
		
		GetComponent<AudioSource>().PlayOneShot(buttonClickSFX);
	}
	
	public void DisplayLoginFailedMenu()
	{
		loginMenu.enabled = false;
		loginFailedMenu.enabled = true;
		loginSuccessMenu.enabled = false;
		loginNewUserMenu.enabled = false;
		
		GetComponent<AudioSource>().PlayOneShot(buttonClickSFX);
	}
	
	public void DisplayLoginSuccessMenu()
	{
		loginMenu.enabled = false;
		loginFailedMenu.enabled = false;
		loginSuccessMenu.enabled = true;
		loginNewUserMenu.enabled = false;
		
		GetComponent<AudioSource>().PlayOneShot(buttonClickSFX);
	}
	
	public void DisplayLoginNewUserMenu()
	{
		loginMenu.enabled = false;
		loginFailedMenu.enabled = false;
		loginSuccessMenu.enabled = false;
		loginNewUserMenu.enabled = true;
		
		GetComponent<AudioSource>().PlayOneShot(buttonClickSFX);
	}
	
	public void DisplayMainMenu()
	{
		loginMenu.enabled = false;
		loginFailedMenu.enabled = false;
		loginSuccessMenu.enabled = false;
		loginNewUserMenu.enabled = false;
		
		GetComponent<AudioSource>().PlayOneShot(buttonClickSFX);
	}
	
	public void PlayButtonHover()
	{
		GetComponent<AudioSource>().PlayOneShot(buttonHoverSFX);
	}
}
