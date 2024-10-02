using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


public class GameManager : MonoBehaviour
{
    //score
    public TextMeshProUGUI scoreText;
    int score = 0;

    //time
    public TextMeshProUGUI timerText;
    float time = 0;

    //deathscreen
    public TextMeshProUGUI finalScoreText;
    
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

        //if you die, your score is displayed
        if (finalScoreText != null)
        {
            finalScoreText.text = "Your final score was " + score;
        }
    }

    #endregion

    #region deathscreen

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    #endregion

}
