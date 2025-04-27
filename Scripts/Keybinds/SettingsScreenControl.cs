using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script controls the navigation between different settings screens in the game,
// allowing the player to switch between the main settings menu, keybinding settings, and gamepad settings.
// It manages the active screen, ensuring that only the relevant menu is visible at any given time by enabling and disabling the respective GameObjects.

public class SettingsScreenControl : MonoBehaviour
{
    [SerializeField] GameObject currentScreen;
    [SerializeField] GameObject mainSettings;
    [SerializeField] GameObject keybindSettings;
    [SerializeField] GameObject gamepadSettings;
    
    // Start is called before the first frame update
    void Start()
    {
        currentScreen = mainSettings;
    }

    public void gotoMainSettingsMenu()
    {
        //Save keybinds
        KeybindManager.instance.saveBindings();
        
        //Disable this
        currentScreen.SetActive(false);
        //Enable Main settings
        currentScreen = mainSettings;
        mainSettings.SetActive(true);
    }

    public void gotoKeybindMenu()
    {
        //Disable current screen
        currentScreen.SetActive(false);
        //Enable Keybind settings
        currentScreen = keybindSettings;
        keybindSettings.SetActive(true);
    }

    public void gotoGamepadMenu()
    {
        //Disable current screen
        currentScreen.SetActive(false);
        //Enable Gamepad settings
        currentScreen = gamepadSettings;
        gamepadSettings.SetActive(true);
    }
}
