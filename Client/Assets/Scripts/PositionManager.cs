using UnityEngine;
using System.Collections;

public class PositionManager : MonoBehaviour {

    GameObject spawner;
    PlayerSpawner playerSpawner;

	// Use this for initialization
	void Start () {

        spawner = GameObject.FindGameObjectWithTag("Spawner");
        playerSpawner = spawner.GetComponent<PlayerSpawner>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
