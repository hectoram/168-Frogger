using UnityEngine;
using System.Collections;

public class Platform : MonoBehaviour {

	void OnTriggerStay2D(Collider2D coll)
	{
		// If the frog is on the platform, make it a child of the platform
		if (coll.name == "Frog" || coll.tag == "Player")
			coll.transform.parent = transform;
	}

	void OnTriggerExit2D(Collider2D coll)
	{
		coll.transform.parent = null;
	}
}
