using UnityEngine;
using System.Collections;

public class Spawn : MonoBehaviour {

	public GameObject prefab;
	public float interval = 1;
	public Vector2 velocity = Vector2.right;

    bool runOnce = false;

	// Use this for initialization
	void Start () 
	{
		InvokeRepeating ("SpawnNext", 0, interval);
	}

    void Update()
    {
        /*if (ClientScript.spawnObstacles)
        {
            runOnce = true;

            if (runOnce)
            {
                InvokeRepeating("SpawnNext", 0, interval);
                runOnce = false;
                ClientScript.spawnObstacles = false;
            }
        }*/    
    }

	void SpawnNext()
	{
		// Instantiate
		GameObject g = (GameObject)Instantiate (prefab, transform.position, Quaternion.identity);

		// Set the velocity
		g.GetComponent<Rigidbody2D> ().velocity = velocity;
	}
}
