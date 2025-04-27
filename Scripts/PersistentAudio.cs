// Music by Nicholas Panek from Pixabay

using UnityEngine;

// This script ensures that the background music persists across scenes without being destroyed when 
// transitioning between them. It uses the singleton pattern to ensure only one instance of the audio 
// manager exists at any time. If a duplicate instance is created, it will be destroyed. The script also 
// prevents the audio manager from being destroyed by marking it with DontDestroyOnLoad, allowing the 
// music to continue playing throughout the game.

public class PersistentAudio : MonoBehaviour
{
    private static PersistentAudio instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // Destroy duplicates
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); // Make persistent across scenes
    }
}
