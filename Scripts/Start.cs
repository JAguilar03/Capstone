using UnityEngine;
using UnityEngine.SceneManagement;

// This script handles scene navigation from the main menu, including starting the game,
// opening settings, viewing instructions, and viewing credits.
// Each public method is linked to a UI button in the menu.

public class Start : MonoBehaviour
{
    public void StartGame()
    {
        // Load the game
        SceneManager.LoadScene("Level1");
    }

    public void RestartGame()
    {
        // Reload the game
        SceneManager.LoadScene("StartGame");
    }

    public void Settings() {
        // Open settings
        SceneManager.LoadScene("Settings");
    }

    public void MainMenu()
    {
        // Load the main menu
        KeybindManager.Enable();
        SceneManager.LoadScene("StartGame");
    }

    public void HowToPlay()
    {
        // Load the how to play screen
        SceneManager.LoadScene("HowToPlay");
    }

    public void HowToPlay2()
    {
        // Load xbox controller layout
        SceneManager.LoadScene("HowToPlay2");
    }

    public void Credits()
    {
        // Load the credits screen
        SceneManager.LoadScene("Credits");
    }
}
