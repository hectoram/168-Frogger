using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Frog : MonoBehaviour 
{
	public AudioClip jumpSFX;

	GameObject menuObject;
	GameOverScript menu;
    GameUI gameUI;
	public bool enabled = true;

	void Start()
	{
		menuObject = GameObject.FindGameObjectWithTag ("Menus");
		menu = menuObject.GetComponent<GameOverScript> ();
        gameUI = menuObject.GetComponent<GameUI>();
	}

	// Jump Speed - how fast the frog will jump
	public float speed = 0.1f;

	// Current Jump - keeps track of the current jumping progress
	Vector2 jump = Vector2.zero;

	// Check to see if the frog is currently jumping
	public bool isJumping()
	{
		return jump != Vector2.zero;
	}

	// If the frog collides with a vehicle, then it's GAME OVER
	void OnCollisionEnter2D(Collision2D coll)
	{
		//Destroy (gameObject);
		//menu.ShowLoseMenu ();

        gameUI.addDeathScore();
        transform.position = new Vector3(0, -8, 0);
	}

	// FixedUpdate is called in a fixed time interval
	void FixedUpdate () 
	{
		// Is the frog currently jumping?
		if (enabled) {

			if (isJumping ()) {
				if (isJumping ()) {
					// Remember current position
					Vector2 currentPosition = transform.position;

					// Jump a bit futher
					transform.position = Vector2.MoveTowards (currentPosition, currentPosition + jump, speed);

					// Subtract stepsize from jump vector
					jump -= (Vector2)transform.position - currentPosition;
				}
			}
		// Otherwise allow for next jump
		else {
				// Detects arrow key presses
				// UP ARROW OR W KEY
				if (Input.GetKey (KeyCode.UpArrow) || Input.GetKey (KeyCode.W)) {
					jump = Vector2.up;
					GetComponent<AudioSource> ().PlayOneShot (jumpSFX);
				}
			// RIGHT ARROW OR D KEY
			else if (Input.GetKey (KeyCode.RightArrow) || Input.GetKey (KeyCode.D)) {
					jump = Vector2.right;
					GetComponent<AudioSource> ().PlayOneShot (jumpSFX);
				}
			// DOWN ARROW OR S KEY
			else if (Input.GetKey (KeyCode.DownArrow) || Input.GetKey (KeyCode.S)) {
					jump = -Vector2.up; // -up means down
					GetComponent<AudioSource> ().PlayOneShot (jumpSFX);
				}
			// LEFT ARROW OR A KEY
			else if (Input.GetKey (KeyCode.LeftArrow) || Input.GetKey (KeyCode.A)) {
					jump = -Vector2.right; // -right means left
					GetComponent<AudioSource> ().PlayOneShot (jumpSFX);
				}
			}
		} else 
		{
			//Use the network to update me instead. 
		}

		// Setting up animation parameters
		GetComponent<Animator> ().SetFloat ("X", jump.x);
		GetComponent<Animator> ().SetFloat ("Y", jump.y);
		GetComponent<Animator> ().speed = Convert.ToSingle (isJumping ());
	}
}
