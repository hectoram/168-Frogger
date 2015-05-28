using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameUI : MonoBehaviour {

    public Text scoreText1;
    public Text scoreText2;
    public Text timeText;

    public static string score1 = "000";
    public static string score2 = "000";

    GameObject menuObject;
    GameOverScript menu;

    GameObject networking;
    ClientScript clientManager;

    static int score;
    static float time;

    static bool isGameOver;
    static bool startTimer;

    public static void restartGame()
    {
        score = 0;
        time = 60; // 5 minutes = 300 or 1 minute = 60

        isGameOver = false;
        startTimer = true;
        PlayerSpawner.updatePositions = true;
    }

    public static void setTimer(float newTime)
    {
        time = newTime;
    }

	// Use this for initialization
	void Start () {

        startTimer = false;

        menuObject = GameObject.FindGameObjectWithTag("Menus");
        menu = menuObject.GetComponent<GameOverScript>();

        networking = GameObject.FindGameObjectWithTag("Networking");
        clientManager = networking.GetComponent<ClientScript>();
	}
	
	// Update is called once per frame
	void Update () {

        if (startTimer)
        {
            if (time > 0)
            {
                time -= Time.deltaTime;
            }
            else
            {
                isGameOver = true;
                startTimer = false;
                PlayerSpawner.updatePositions = false;
                scoreText1.text = score1.ToString();
                scoreText2.text = score2.ToString();
                menu.finalScoreText.text = "SCORE: " + score.ToString();
                menu.ShowScoreMenu();

                clientManager.Send("gameOver," + score.ToString() + "<EOF>");
            }
        }

        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time - minutes * 60);

        string niceTime = string.Format("{0:0}:{1:00}", minutes, seconds);

        timeText.text = niceTime;

        /*if (ClientScript.myPlayerNumber == "1")
        {
            scoreText1.text = score.ToString();
        }*/

        scoreText1.text = score1.ToString();
        scoreText2.text = score2.ToString();
	}

    /*public void resetGame()
    {
        score = 0;
        time = 300;
        isGameOver = false;
    }*/

    public void addFinishLineScore()
    {
        Debug.Log("Adding FINISH LINE score!");
        score += 100;
        clientManager.Send("score," + score + "<EOF>");
    }

    public void addDeathScore()
    {
        Debug.Log("Adding DEATH score!");
        if (score > 25)
            score -= 25;
        else
            score = 0;

        clientManager.Send("score," + score + "<EOF>");
    }
}
