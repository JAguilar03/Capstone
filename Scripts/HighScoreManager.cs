using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

// This script manages the high score system in the game, allowing scores from previous sessions to be saved, 
// displayed, and updated. It retrieves the last score from PlayerPrefs and adds it to the high score list if applicable. 
// The script maintains the top 5 high scores, displays them using TextMeshPro UI elements, and provides functionality 
// for clearing the high scores. The high scores are stored in PlayerPrefs and are displayed in descending order.

public class HighScoreManager : MonoBehaviour
{
    public TextMeshProUGUI[] highScoreTexts; // Array of 5 TextMeshPro texts for displaying scores
    private const string HIGH_SCORES_KEY = "HighScores";
    private const int MAX_HIGH_SCORES = 5;
    
    private void Start()
    {
        // Get the score from the previous game
        int lastScore = PlayerPrefs.GetInt("LastScore", 0);
        if (lastScore > 0)
        {
            AddHighScore(lastScore);
            // Clear the last score
            PlayerPrefs.DeleteKey("LastScore");
        }
        
        DisplayHighScores();
    }

    // Add high scores
    public void AddHighScore(int newScore)
    {
        // Get existing scores
        List<int> highScores = GetHighScores();
        
        // Add new score
        highScores.Add(newScore);
        
        // Sort in descending order and take top 5
        highScores = highScores.OrderByDescending(x => x).Take(MAX_HIGH_SCORES).ToList();
        
        // Save updated scores
        SaveHighScores(highScores);
        
        // Update display
        DisplayHighScores();
    }

    // Get high scores from PlayerPrefs
    public List<int> GetHighScores()
    {
        string scoresString = PlayerPrefs.GetString(HIGH_SCORES_KEY, "");
        if (string.IsNullOrEmpty(scoresString))
            return new List<int>();
            
        return scoresString.Split(',')
            .Where(x => !string.IsNullOrEmpty(x))
            .Select(int.Parse)
            .ToList();
    }

    // Save high scores to PlayerPrefs
    private void SaveHighScores(List<int> highScores)
    {
        string scoresString = string.Join(",", highScores);
        PlayerPrefs.SetString(HIGH_SCORES_KEY, scoresString);
        PlayerPrefs.Save();
    }

    // Display high scores
    private void DisplayHighScores()
    {
        List<int> highScores = GetHighScores();
        
        for (int i = 0; i < highScoreTexts.Length; i++)
        {
            if (i < highScores.Count)
                highScoreTexts[i].text = $"{i + 1}. {highScores[i]}";
            else
                highScoreTexts[i].text = $"{i + 1}. ---";
        }
    }

    // Clear high scores
    public void ClearHighScores()
    {
        PlayerPrefs.DeleteKey(HIGH_SCORES_KEY);
        DisplayHighScores();
    }
}
