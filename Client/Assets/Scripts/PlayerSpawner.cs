using UnityEngine;
using System.Collections;

public class PlayerSpawner : MonoBehaviour {

    public GameObject player1;
    public GameObject player2;

    public GameObject thisPlayer;

    ClientScript clientManager;
    GameObject networking;

    int numberOfPlayers;
    int playerNumber;
    string username;

    Vector3 onePlayerP1Position;

    Vector3 twoPlayerP1Position;
    Vector3 twoPlayerP2Position;

    public static bool updatePositions = false;

    /*Vector3 threePlayerP1Position;
    Vector3 threePlayerP2Position;
    Vector3 threePlayerP3Position;

    Vector3 fourPlayerP1Position;
    Vector3 fourPlayerP2Position;
    Vector3 fourPlayerP3Position;
    Vector3 fourPlayerP4Position;*/

    public int getPlayerNumber()
    {
        return playerNumber;
    }

    public GameObject getThisPlayer()
    {
        return thisPlayer;
    }

    public void spawnPlayers()
    {
        numberOfPlayers = clientManager.getNumberOfPlayers();  //being set in ClientScript when "start-game" is received
        playerNumber = clientManager.getPlayerNumber();
        username = clientManager.getUsername();

        if (numberOfPlayers == 1)
        {
            Debug.Log("Spawning Player 1");
            GameObject p1 = (GameObject)Instantiate(player1, onePlayerP1Position, Quaternion.identity);
            thisPlayer = p1;
        }
        else if (numberOfPlayers == 2)
        {
            Debug.Log("Spawning Player 1");
            GameObject p1 = (GameObject)Instantiate(player1, twoPlayerP1Position, Quaternion.identity);
            Debug.Log("Spawning Player 2");
            GameObject p2 = (GameObject)Instantiate(player2, twoPlayerP2Position, Quaternion.identity);

            if (playerNumber == 1)
                thisPlayer = p1;
            else if (playerNumber == 2)
                thisPlayer = p2;
        }
        /*else if (numberOfPlayers == 3)
        {
            Debug.Log("Spawning Player 1");
            GameObject p1 = (GameObject)Instantiate(player1, threePlayerP1Position, Quaternion.identity);
            Debug.Log("Spawning Player 2");
            GameObject p2 = (GameObject)Instantiate(player1, threePlayerP2Position, Quaternion.identity);
            Debug.Log("Spawning Player 3");
            GameObject p3 = (GameObject)Instantiate(player1, threePlayerP3Position, Quaternion.identity);

            if (playerNumber == 1)
                thisPlayer = p1;
            else if (playerNumber == 2)
                thisPlayer = p2;
            else if (playerNumber == 3)
                thisPlayer = p3;
        }
        else if (numberOfPlayers == 4)
        {
            Debug.Log("Spawning Player 1");
            GameObject p1 = (GameObject)Instantiate(player1, fourPlayerP1Position, Quaternion.identity);
            Debug.Log("Spawning Player 2");
            GameObject p2 = (GameObject)Instantiate(player1, fourPlayerP2Position, Quaternion.identity);
            Debug.Log("Spawning Player 3");
            GameObject p3 = (GameObject)Instantiate(player1, fourPlayerP3Position, Quaternion.identity);
            Debug.Log("Spawning Player 4");
            GameObject p4 = (GameObject)Instantiate(player1, fourPlayerP4Position, Quaternion.identity);

            if (playerNumber == 1)
                thisPlayer = p1;
            else if (playerNumber == 2)
                thisPlayer = p2;
            else if (playerNumber == 3)
                thisPlayer = p3;
            else if (playerNumber == 4)
                thisPlayer = p4;
        }*/

        Debug.Log("Starting multiplayer game...");
        Debug.Log("Number of players: " + numberOfPlayers);
        Debug.Log("You are player number: " + playerNumber);
        Debug.Log("Your username is: " + username);

        updatePositions = true;
    }

	// Use this for initialization
	void Start () {

        onePlayerP1Position.Set(0, -8, 0);      // One Player - Player 1 Starting Position

        twoPlayerP1Position.Set(-3, -8, 0);      // Two Player - Player 1 Starting Position
        twoPlayerP2Position.Set(3, -8, 0);      // Two Player - Player 2 Starting Position

        /*threePlayerP1Position.Set(-4, -8, 0);      // Two Player - Player 1 Starting Position
        threePlayerP2Position.Set(0, -8, 0);      // Two Player - Player 2 Starting Position
        threePlayerP3Position.Set(4, -8, 0);      // Two Player - Player 2 Starting Position

        fourPlayerP1Position.Set(-6, -8, 0);      // Two Player - Player 1 Starting Position
        fourPlayerP2Position.Set(-2, -8, 0);      // Two Player - Player 2 Starting Position
        fourPlayerP3Position.Set(2, -8, 0);      // Two Player - Player 2 Starting Position
        fourPlayerP4Position.Set(6, -8, 0);*/      // Two Player - Player 2 Starting Position

        networking = GameObject.FindGameObjectWithTag("Networking");
        clientManager = networking.GetComponent<ClientScript>();

        clientManager.setIsPlayerInLobby(false);

        clientManager.SendMSG("player-ready<EOF>", 1000);
        spawnPlayers();
	}
	
	// Update is called once per frame
	void Update () {

        if (updatePositions)
        {
            if (numberOfPlayers == 2)
            {

            }
            /*else if (numberOfPlayers == 3)
            {

            }
            else if (numberOfPlayers == 4)
            {

            }*/
        }
	}
}
