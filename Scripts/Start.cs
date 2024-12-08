using UnityEngine;
using UnityEngine.SceneManagement;

public class Start : MonoBehaviour
{
    public void StartGame()
    {
        // Load the game
        SceneManager.LoadScene("Level1");
    }

    public void Settings() {
        // Open settings
        SceneManager.LoadScene("Settings");
    }
}
