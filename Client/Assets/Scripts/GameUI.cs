using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameUI : MonoBehaviour {

    public Text scoreText1;
    public Text scoreText2;
    public Text scoreText3;
    public Text scoreText4;

    public GameObject score1Panel;
    public GameObject score2Panel;
    public GameObject score3Panel;
    public GameObject score4Panel;

    Vector3 score1Pos;
    Vector3 score2Pos;
    Vector3 score3Pos;
    Vector3 score4Pos;

    public Text timeText;

    public static string score1 = "000";
    public static string score2 = "000";
    public static string score3 = "000";
    public static string score4 = "000";

    GameObject menuObject;
    GameOverScript menu;

    GameObject networking;
    ClientScript clientManager;

    static int score;
    static float time;

    static bool isGameOver;
    static bool startTimer;

    public bool getIsGameOver()
    {
        return isGameOver;
    }

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

        score1Panel = GameObject.Find("Score 1 Panel");
        score2Panel = GameObject.Find("Score 2 Panel");
        score3Panel = GameObject.Find("Score 3 Panel");
        score4Panel = GameObject.Find("Score 4 Panel");

        if (clientManager.getNumberOfPlayers() == "2")
        {
            score1Panel.SetActive(true);
            score2Panel.SetActive(true);

            Debug.Log("Printing the location of the SCORE 1 PANEL: X = " + score1Panel.transform.position.x + " Y = " + score1Panel.transform.position.y);
            Debug.Log("Printing the location of the SCORE 2 PANEL: X = " + score2Panel.transform.position.x + " Y = " + score2Panel.transform.position.y);

            score1Pos.Set(-3, 7, 0);
            score2Pos.Set(3, 7, 0);

            score1Panel.transform.position = score1Pos;
            score2Panel.transform.position = score2Pos;
        }
        else if (clientManager.getNumberOfPlayers() == "3")
        {
            score1Panel.SetActive(true);
            score2Panel.SetActive(true);
            score3Panel.SetActive(true);

            Debug.Log("Printing the location of the SCORE 1 PANEL: X = " + score1Panel.transform.position.x + " Y = " + score1Panel.transform.position.y);
            Debug.Log("Printing the location of the SCORE 2 PANEL: X = " + score2Panel.transform.position.x + " Y = " + score2Panel.transform.position.y);
            Debug.Log("Printing the location of the SCORE 3 PANEL: X = " + score3Panel.transform.position.x + " Y = " + score3Panel.transform.position.y);

            score1Pos.Set(-5, 7, 0);
            score2Pos.Set(0, 7, 0);
            score3Pos.Set(5, 7, 0);

            score1Panel.transform.position = score1Pos;
            score2Panel.transform.position = score2Pos;
            score3Panel.transform.position = score3Pos;
        }
        else if (clientManager.getNumberOfPlayers() == "4")
        {
            score1Panel.SetActive(true);
            score2Panel.SetActive(true);
            score3Panel.SetActive(true);
            score4Panel.SetActive(true);

            Debug.Log("Printing the location of the SCORE 1 PANEL: X = " + score1Panel.transform.position.x + " Y = " + score1Panel.transform.position.y);
            Debug.Log("Printing the location of the SCORE 2 PANEL: X = " + score2Panel.transform.position.x + " Y = " + score2Panel.transform.position.y);
            Debug.Log("Printing the location of the SCORE 3 PANEL: X = " + score3Panel.transform.position.x + " Y = " + score3Panel.transform.position.y);
            Debug.Log("Printing the location of the SCORE 4 PANEL: X = " + score4Panel.transform.position.x + " Y = " + score4Panel.transform.position.y);

            score1Pos.Set(-7, 7, 0);
            score2Pos.Set(-2, 7, 0);
            score3Pos.Set(2, 7, 0);
            score4Pos.Set(7, 7, 0);

            score1Panel.transform.position = score1Pos;
            score2Panel.transform.position = score2Pos;
            score3Panel.transform.position = score3Pos;
            score4Panel.transform.position = score4Pos;
        }
	}
	
	// Update is called once per frame
	void Update () {

        if (startTimer)
        {
            if (time > 0)
            {
                time -= Time.deltaTime;

                if (score1 == "DICONNECTED" || score1 == "-1")
                {
                    //scoreText1.fontSize = 12;
                    scoreText1.text = "DISCONNECTED";
                }
                else if (score2 == "DICONNECTED" || score2 == "-1")
                {
                    //scoreText2.fontSize = 12;
                    scoreText2.text = "DISCONNECTED";
                }
                else if (score3 == "DICONNECTED" || score3 == "-1")
                {
                    //scoreText3.fontSize = 12;
                    scoreText3.text = "DISCONNECTED";
                }
                else if (score4 == "DICONNECTED" || score4 == "-1")
                {
                    //scoreText4.fontSize = 12;
                    scoreText4.text = "DISCONNECTED";
                }
                else
                {
                    scoreText1.text = score1.ToString();
                    scoreText2.text = score2.ToString();
                    scoreText3.text = score3.ToString();
                    scoreText4.text = score4.ToString();
                }
            }
            else
            {
                isGameOver = true;
                startTimer = false;
                PlayerSpawner.updatePositions = false;

                menu.finalScoreText.text = "SCORE: " + score.ToString();

                clientManager.Send("gameOver," + score.ToString() + "<EOF>");
            }
        }

        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time - minutes * 60);

        string niceTime = string.Format("{0:0}:{1:00}", minutes, seconds);

        timeText.text = niceTime;
	}

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
