using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


public class GameManager : MonoBehaviour
{
    [Header("UI")]
    //score
    public TextMeshProUGUI scoreText;
    int score = 0;

    //time
    public TextMeshProUGUI timerText;
    float time = 0;


    //deathscreen
    public TextMeshProUGUI finalScoreText;

    //winscreen
    public GameObject winScreen;
    public TextMeshProUGUI finalTimeText;
    public TextMeshProUGUI winScoreText;

    public GameObject hud;

    void Update()
    {
        DisplayTime();
        DisplayScore();
    }

    #region UI
    void DisplayTime()
    {
        //updates timer with the elapsed time of the current attempt
        time += Time.deltaTime;
        timerText.text = "Time: " + time.ToString("0");
    }
    public void GainScore(int scoreToAdd)
    {
        //increases score with a value passed in by another script
        score += scoreToAdd;
    }

    void DisplayScore()
    {
        //displays the current score
        scoreText.text = "Score: " + score;

        
    }

    #endregion

    #region deathscreen

    public void DisplayDeathScreen()
    {
        hud.SetActive(false);


        //if you die, your score is displayed
        if (finalScoreText != null)
        {
            finalScoreText.text = "Your final score was " + score;
        }

    }  

    #endregion

    #region winscreen

    public void DisplayWinScreen()
    {
        //turns winscreen on and turn the text at the top off
        winScreen.SetActive(true);
        finalTimeText.text = "Time: " + time.ToString("0");
        winScoreText.text = "Score: " + score;

        hud.SetActive(false);
    }

    #endregion

}
