using UnityEngine;
using System.Collections;

public class Water : MonoBehaviour {

	GameObject menuObject;
	GameOverScript menu;
    GameUI gameUI;
	
	void Start()
	{
		menuObject = GameObject.FindGameObjectWithTag ("Menus");
		menu = menuObject.GetComponent<GameOverScript> ();
        gameUI = menuObject.GetComponent<GameUI>();
	}

    void OnTriggerStay2D(Collider2D coll)
	//void OnTriggerEnter2D(Collider2D coll)
	{
		// Check to see if the object is a frog
        if (coll.tag == "Player")
			// Check to see if the frog isn't jumping
		    if (!coll.GetComponent<Frog> ().isJumping ())
				// Check to see if the frog isn't on a platform
		        if (coll.transform.parent == null && coll.transform.position.y > -1 && coll.transform.position.y < 5) {
			        // GAME OVER
			        //Destroy (coll.gameObject);
			        //menu.ShowLoseMenu();
                    Debug.Log("WATER DEATHHH!");
                    gameUI.addDeathScore();
                    coll.gameObject.transform.position = new Vector3(0, -8, 0);
		}
	}
}
