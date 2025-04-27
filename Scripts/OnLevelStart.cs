using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script is responsible for saving the player's data at the start of each level. 
// It references the SavePlayer component and calls its saveFile method to ensure that the player's progress 
// is stored as soon as a new level starts. This helps maintain player progress across levels.

public class OnLevelStart : MonoBehaviour
{
    SavePlayer saveHandler;
    
    // Start is called before the first frame update
    void Start()
    {
        saveHandler = this.GetComponent<SavePlayer>();
        
        //Save player data at the start of every level
        saveHandler.saveFile();    
    }
}
