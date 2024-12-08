// Music by Nicholas Panek from Pixabay

using UnityEngine;

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
