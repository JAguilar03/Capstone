using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

// This script controls the game’s audio settings through an exposed AudioMixer parameter.
// It provides a method to adjust the master volume, typically hooked up to a UI slider in the settings menu.

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
        Debug.Log(volume);
    }

    void Start()
    {
        KeybindManager.Disable(); //Disable keybind manager so that rebinding works smoothly
    }
}
