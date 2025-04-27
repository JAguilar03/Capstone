using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script manages the singleton instance of the player character (PC) in the game. It ensures that only 
// one instance of the player exists throughout the game, maintaining consistent references to the player 
// for other game objects like the camera and researchers. If there are multiple instances of the PlayerManager, 
// only one is retained (the first one created), and any duplicates are destroyed. The script also ensures that 
// the player instance persists across scene changes with the DontDestroyOnLoad method, preventing it from being 
// destroyed when transitioning between scenes.

public class PlayerManager : MonoBehaviour
{
    //Singleton for player character (PC). Ensures that only one PC exists, and that
    //Camera, Researchers, etc. only ever reference the existing PC.
    //(Should be automatically set to null if stored singleton is destroyed)
    public static PlayerManager playerInstance;

    // Start is called before the first frame update
    void Start()
    {
        if (playerInstance == null)
        {
            playerInstance = this;
            DontDestroyOnLoad(this);
        }
        else if (playerInstance != this)
        {
            //Move existing PC to this position, then delete this duplicate
            playerInstance.transform.position = this.transform.position;
            Destroy(this.gameObject);
        }
    }

    // OnDestroy, clear the singleton
    void OnDestroy()
    {
        if (playerInstance == this)
        {
            playerInstance = null;        
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
