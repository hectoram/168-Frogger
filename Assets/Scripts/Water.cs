using UnityEngine;
using System.Collections;

public class Water : MonoBehaviour {

	GameObject menuObject;
	GameOverScript menu;
	
	void Start()
	{
		menuObject = GameObject.FindGameObjectWithTag ("Menus");
		menu = menuObject.GetComponent<GameOverScript> ();
	}

	void OnTriggerStay2D(Collider2D coll)
	{
		// Check to see if the object is a frog
		if (coll.name == "Frog")
			// Check to see if the frog isn't jumping
		if (!coll.GetComponent<Frog> ().isJumping ())
				// Check to see if the frog isn't on a platform
		if (coll.transform.parent == null) {
			// GAME OVER
			Destroy (coll.gameObject);
				menu.ShowLoseMenu();
		}
	}
}
