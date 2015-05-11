using UnityEngine;
using System.Collections;

public class FinishLineScript : MonoBehaviour {

	GameObject menuObject;
	GameOverScript menu;
    GameUI gameUI;
	
	void Start()
	{
		menuObject = GameObject.FindGameObjectWithTag ("Menus");
		menu = menuObject.GetComponent<GameOverScript> ();
        gameUI = menuObject.GetComponent<GameUI>();
	}

	void OnTriggerEnter2D(Collider2D coll)
	{
		//Destroy (coll.gameObject);
		//menu.ShowWinMenu ();

        gameUI.addFinishLineScore();
        coll.gameObject.transform.position = new Vector3(0, -8, 0);
	}
}
