using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// This script manages the HUD (Heads-Up Display) elements of the game, including health, ammo, score, and high score. 
// It ensures that only one instance of the HUD exists throughout the game using a singleton pattern. 
// The script updates the display of health, ammo count, score, and high score. It also manages different ammo icons 
// based on the current weapon type and adjusts the heart icon according to the player's health. The HUD is persistent 
// across scenes using DontDestroyOnLoad and is cleared when destroyed.

public class HUDManager : MonoBehaviour
{
    //Singleton for HUD. Ensures that only one HUD exists, and that
    //updates to health, ammo, score, etc. are applied to this object
    //(Should be automatically set to null if stored singleton is destroyed)
    public static HUDManager hudInstance;

    //Text Elements
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;

    public Image ammoIcon;

    public Image heartIcon;

    public Image goldTrophy;

    public Image shotgunAmmoIcon;

    // Heart Sprites
    public Sprite fullHeart;
    public Sprite halfHeart;
    public Sprite quarterHeart;
    public Sprite emptyHeart;

    public Image rifleAmmoIcon;

    // Start is called before the first frame update
    void Start()
    {
        if (hudInstance == null)
        {
            hudInstance = this;
            DontDestroyOnLoad(this);
        }
        else if (hudInstance != this)
        {
            Destroy(this.gameObject);
        }
        
        //Should HUD not destroy on load?
        //Hard to say if it'll make things easier, or cause more issues.
        // DontDestroyOnLoad(this);
    }

    // OnDestroy, clear the singleton
    void OnDestroy()
    {
        if (hudInstance == this)
        {
            hudInstance = null;
        }
    }

    public void SetHealthDisplay(float health)
    {
        healthText.text = ": " + health.ToString();

        // Update heart image based on health
        if (health > 50)
        {
            heartIcon.sprite = fullHeart;
        }
        else if (health > 25)
        {
            heartIcon.sprite = halfHeart;
        }
        else if (health > 0)
        {
            heartIcon.sprite = quarterHeart;
        }
        else
        {
            heartIcon.sprite = emptyHeart;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetAmmoDisplay(float currentAmmo, float maxAmmo)
    {
        ammoText.text = ": " + currentAmmo.ToString() + " / " + maxAmmo.ToString();
    }

    public void SetScoreDisplay(float score)
    {
        scoreText.text = "Score: " + score.ToString();
    }

    public void SetHighScoreDisplay(float highScore)
    {
        highScoreText.text = ": " + highScore.ToString();
    }

    public void SetAmmoIcon(string weaponType)
    {
        // Hide all ammo icons first
        ammoIcon.gameObject.SetActive(false);
        rifleAmmoIcon.gameObject.SetActive(false);
        shotgunAmmoIcon.gameObject.SetActive(false);
        
        // Show the appropriate icon based on weapon type
        switch (weaponType.ToLower())
        {
            case "pistol":
                ammoIcon.gameObject.SetActive(true);
                break;
            case "rifle":
                rifleAmmoIcon.gameObject.SetActive(true);
                break;
            case "shotgun":
                shotgunAmmoIcon.gameObject.SetActive(true);
                break;
        }
    }

}
