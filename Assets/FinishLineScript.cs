using UnityEngine;
using System.Collections;

public class FinishLineScript : MonoBehaviour {

	GameObject menuObject;
	GameOverScript menu;
	
	void Start()
	{
		menuObject = GameObject.FindGameObjectWithTag ("Menus");
		menu = menuObject.GetComponent<GameOverScript> ();
	}

	void OnTriggerEnter2D(Collider2D coll)
	{
		Destroy (coll.gameObject);
		menu.ShowWinMenu ();
	}
}
