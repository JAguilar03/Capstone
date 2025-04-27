using UnityEngine;
using TMPro;
using System.Collections.Generic;

// This script manages the player's score and high score in the game.
// It interfaces with the HighScoreManager to retrieve saved high scores at startup,
// updates the score and high score dynamically, and reflects changes through the HUDManager.
// It also allows for loading a saved score from external sources or files.

public class ScoreManager : MonoBehaviour
{
    // public TextMeshProUGUI scoreText;
    // public TextMeshProUGUI highScoreText;
    private int score = 0;
    private int highScore = 0;

    private HighScoreManager highScoreManager;


    void Start()
    {
       
        highScoreManager = FindObjectOfType<HighScoreManager>();
        
        // Load the saved high score from HighScoreManager
        List<int> scores = highScoreManager.GetHighScores();
        highScore = scores.Count > 0 ? scores[0] : 0;

        // highScoreText.text = "High Score: " + highScore.ToString();
        // scoreText.text = "Score: " + score.ToString();
        HUDManager.hudInstance.SetHighScoreDisplay(highScore);
        HUDManager.hudInstance.SetScoreDisplay(score);
    }

    public void AddScore(int points)
    {
        score += points;
        // scoreText.text = "Score: " + score.ToString();
        HUDManager.hudInstance.SetScoreDisplay(score);

        // Check and update high score
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
            // highScoreText.text = "High Score: " + highScore.ToString();
            HUDManager.hudInstance.SetHighScoreDisplay(highScore);
        }
    }

    public int GetCurrentScore()
    {
        return score;
    }

    //Used when loading a score from a file
    public void loadScore(int loadedScore)
    {
        score = loadedScore;
        HUDManager.hudInstance.SetScoreDisplay(score);
    }

}
