using UnityEngine;
using System.Collections;

public class Border : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D coll)
	{
		Destroy (coll.gameObject);
	}
}
