using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

// This script is responsible for saving player data to a JSON file.
// It extracts relevant player data (such as health, score, weapons, and level) and serializes it to a file for later use (persistent storage).
// The `saveFile` function is the main method used for saving the data.

public class SavePlayer : MonoBehaviour
{
    //SO for saving/loading player data
    [SerializeField] PlayerFileSO playerData;
    
    //Filepath arguments
    string filePath; //Used to store destination: root folder + relative path
    [SerializeField] string relativePath = "TestSave.json";


    // Start is called before the first frame update
    void Awake()
    {
        filePath = Application.persistentDataPath + "/" + relativePath;
    }

    public void saveFile()
    {
        extractPlayerData();
        saveToJSON();
    }

    // Extract player data from player instance
    void extractPlayerData()
    {
        PlayerManager player = PlayerManager.playerInstance;
        if (player == null)
        {
            Debug.LogError("SavePlayer: No player instance found!");
            return;
        }
        
        PlayerMovement playerScript = player.GetComponent<PlayerMovement>();
        if (playerScript == null)
        {
            Debug.LogError("SavePlayer: No PlayerMovement script found on player instance!");
            return;
        }
        
        WeaponController weaponController = player.GetComponentInChildren<WeaponController>();
        if (weaponController == null)
        {
            Debug.LogError("SavePlayer: No WeaponController object found on player instance!");
            return;
        }

        ScoreManager scoreManager = playerScript.GetComponentInChildren<ScoreManager>();
        if (scoreManager == null)
        {
            Debug.LogError("SavePlayer: No ScoreManager object found on player instance!");
            return;
        }

        playerData.currentHp = playerScript.health;
        
        Weapon[] weaponList = weaponController.GetComponentsInChildren<Weapon>(true);
        playerData.weapons.list = new List<WeaponData>();
        for (int i = 0; i < weaponList.Length; i++)
        {
            WeaponData weaponData = new WeaponData();
            weaponData.WeaponKey = weaponList[i].weaponID;
            weaponData.currAmmo = weaponList[i].currentAmmo;
            weaponData.maxAmmo = weaponList[i].maxAmmo;
            playerData.weapons.list.Add(weaponData);
        }

        playerData.score = scoreManager.GetCurrentScore();

        //TEMPORARY: Use this case-switch until level id system is updated (i.e. until randomized endless levels are working)
        string currentScene = SceneManager.GetActiveScene().name;
        switch (currentScene)
        {
            case "Level1":
                playerData.level = 1;
                break;
            case "Level2":
                playerData.level = 2;
                break;
            case "Level3":
                playerData.level = 3;
                break;
        }
    }

    // Save player data to JSON file
    void saveToJSON()
    {
        string saveJSON = playerData.toJSON();
        File.WriteAllText(filePath, saveJSON);
        Debug.Log("Saved player data to " + filePath);
    }
}
