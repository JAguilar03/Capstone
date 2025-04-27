using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

// This script is responsible for loading player data from a JSON file and applying it to the player character and associated components.
// It loads player stats, health, score, and weapon information, then initializes the player prefab accordingly.
// It also handles level transitions based on the player's saved level data.


public class LoadPlayer : MonoBehaviour
{
    //SO for saving/loading player data
    [SerializeField] PlayerFileSO playerData;
    
    //SO for weapon dictionary
    [SerializeField] WeaponDict weaponDict;

    //Player prefab to load data into
    [SerializeField] GameObject playerPrefab;

    //Filepath arguments
    string filePath; //Used to store destination: root folder + relative path
    [SerializeField] string relativePath = "TestSave.json";

    // Start is called before the first frame update
    void Start()
    {
        filePath = Application.persistentDataPath + "/" + relativePath;
        loadFromJSON();
        applyToPlayer();
    }

    // Load SO config from JSON file
    public void loadFromJSON()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            JsonUtility.FromJsonOverwrite(json, playerData);
            Debug.Log("Game loaded: " + json);
        }
        else
        {
            Debug.LogWarning("Save file not found!");
        }
    }

    // Apply file data to player prefab
    public void applyToPlayer()
    {
        PlayerMovement playerScript = playerPrefab.GetComponent<PlayerMovement>();
        if (playerScript == null)
        {
            Debug.LogError("PlayerMovement script not found on playerPrefab!");
            return;
        }

        WeaponController weaponController = playerPrefab.GetComponentInChildren<WeaponController>();
        if (playerScript == null)
        {
            Debug.LogError("WeaponController script not found among the children of playerPrefab!");
            return;
        }
        
        //Set player values
        playerScript.health = playerData.currentHp;
        HUDManager.hudInstance.SetHealthDisplay(playerScript.health);
        // MaxHP for player is hard-coded, no script for that yet
        // Current stage isn't actually stored by the player right now

        //Set weapon values
        weaponController.initializeFromLoad(playerData);
        
        //Set score
        ScoreManager scoreManager = playerPrefab.GetComponentInChildren<ScoreManager>();
        scoreManager.loadScore(playerData.score);


        // TEMP: Hardcoded logic for what level should be loaded in, based on level listed in file
        // return;
        
        switch (playerData.level % 3)
        {
            case 1:
                SceneManager.LoadScene("Level1");
                break;
            case 2:
                SceneManager.LoadScene("Level2");
                break;
            case 0:
                SceneManager.LoadScene("Level3");
                break;
        }
    }

    // Create a weapon from a WeaponData object
    Weapon fetchWeaponPrefab(WeaponData weaponData)
    {
        //Get the weapon prefab
        return weaponDict.getWeapon(weaponData.WeaponKey);
        
    }

}
