using UnityEngine;
using TMPro;
using System.Collections.Generic;


public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;
    private int score = 0;
    private int highScore = 0;

    private HighScoreManager highScoreManager;


    void Start()
    {
       
        highScoreManager = FindObjectOfType<HighScoreManager>();
        
        // Load the saved high score from HighScoreManager
        List<int> scores = highScoreManager.GetHighScores();
        highScore = scores.Count > 0 ? scores[0] : 0;

        highScoreText.text = "High Score: " + highScore.ToString();
        scoreText.text = "Score: " + score.ToString();
    }

    public void AddScore(int points)
    {
        score += points;
        scoreText.text = "Score: " + score.ToString();

        // Check and update high score
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
            highScoreText.text = "High Score: " + highScore.ToString();
        }
    }

    public int GetCurrentScore()
    {
        return score;
    }

}
