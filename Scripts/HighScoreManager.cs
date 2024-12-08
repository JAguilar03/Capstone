using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

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

    private void SaveHighScores(List<int> highScores)
    {
        string scoresString = string.Join(",", highScores);
        PlayerPrefs.SetString(HIGH_SCORES_KEY, scoresString);
        PlayerPrefs.Save();
    }

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

    public void ClearHighScores()
    {
        PlayerPrefs.DeleteKey(HIGH_SCORES_KEY);
        DisplayHighScores();
    }
}
