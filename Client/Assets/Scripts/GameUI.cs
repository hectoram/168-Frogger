﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameUI : MonoBehaviour {

    public Text scoreText;
    public Text timeText;

    GameObject menuObject;
    GameOverScript menu;

    int score;
    float time;

    bool isGameOver;
    bool startTimer;

    public void restartGame()
    {
        score = 0;
        time = 30; // 5 minutes = 300 or 1 minute = 60

        isGameOver = false;
        startTimer = true;
    }

	// Use this for initialization
	void Start () {

        startTimer = false;

        menuObject = GameObject.FindGameObjectWithTag("Menus");
        menu = menuObject.GetComponent<GameOverScript>();
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
                scoreText.text = score.ToString();
                menu.finalScoreText.text = "SCORE: " + score.ToString();
                menu.ShowScoreMenu();
            }
        }

        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time - minutes * 60);

        string niceTime = string.Format("{0:0}:{1:00}", minutes, seconds);

        timeText.text = niceTime;
        scoreText.text = score.ToString();
	}

    public void resetGame()
    {
        score = 0;
        time = 300;
        isGameOver = false;
    }

    public void addFinishLineScore()
    {
        score += 100;
    }

    public void addDeathScore()
    {
        if (score > 25)
            score -= 25;
        else
            score = 0;
    }
}
