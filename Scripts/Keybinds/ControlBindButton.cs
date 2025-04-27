using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// Manages the functionality of control bind buttons in the settings menu. This script allows players to rebind controls 
// and displays the updated input binding text on the UI. It includes a method to assign the event system and a method 
// to update the displayed text for the button. The refreshText function is intended to be overridden in subclasses 
// to refresh the text according to the specific control bindings.

public class ControlBindButton : MonoBehaviour
{
    [SerializeField] TMP_Text rebindText;
    protected EventSystem eventSystem;
    
    protected void assignEventSystem()
    {
        eventSystem = GameObject.FindObjectOfType<EventSystem>();
    }

    public void updateText(string newText)
    {
        rebindText.text = newText;
    }

    public virtual void refreshText() {
        Debug.LogError("Virtual refreshText function called! Double-check the code!");
    }
}
