using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.EventSystems;

// A subclass of ControlBindButton designed specifically for gamepad key rebinding functionality. This script allows
// players to rebind gamepad controls during runtime and updates the UI with the new keybinding. It supports both single
// key bindings and composite input bindings (e.g., combining multiple buttons for a single action). The script provides
// functions for starting the rebind process, performing the rebind for both single and composite bindings, and updating 
// the UI with the new control mappings.
public class GamepadRebindButton : ControlBindButton
{
    // public string actionName;
    // public string keybindName;
    KeybindManager keybindManager;
    InputActionAsset inputActions;
    // public InputAction buttonAction;
    public InputActionReference buttonAction;

    //Handle composite input
    [SerializeField] bool isComposite = false;
    [SerializeField] string compositeElement;

    // Start is called before the first frame update
    void Start()
    {
        // Set up event system if needed
        assignEventSystem();
        keybindManager = KeybindManager.instance;
        inputActions = keybindManager.inputActions;

        //Update display to match control config
        this.refreshText();
    }

    // Rebind a key
    public void StartRebinding()
    {
        EventSystem.current.SetSelectedGameObject(null);
        // Prent rebinding if already rebinding
        if (KeybindManager.rebindLock == true)
        {
            Debug.LogWarning($"Rebinding is currently locked.");
            return;
        }
        // Handle composite or single binding
        if (isComposite)
        {
            CompositeRebind();
        }
        else
        {
            Rebind();
        }
    }

    // Rebin for a single (non-composite) gamepad binding
    public void Rebind()
    {
        KeybindManager.rebindLock = true; //lock
        // eventSystem.enabled = false;
        // KeybindManager.instance.playerInputActive(false);
        KeybindManager.instance.lockControlScheme("Gamepad");

        // InputAction actionToRebind = inputActions.FindAction(actionName);
        InputAction actionToRebind = buttonAction.action;

        int controlSchemeIndex = -1;
        for (int i = 0; i < actionToRebind.bindings.Count; i++)
        {
            if (actionToRebind.bindings[i].path.Contains("Gamepad"))
            {
                controlSchemeIndex = i;
                break;
            }
        }
        if (controlSchemeIndex == -1)
        {
            Debug.LogError("Binding not available for gamepad.");
            KeybindManager.rebindLock = false; //unlock
            // eventSystem.enabled = true;
            // KeybindManager.instance.playerInputActive(true);
            KeybindManager.instance.unlockControlScheme();
            return;
        }

        actionToRebind.Disable();
        this.updateText("...");

        // Start the rebind process
        actionToRebind.PerformInteractiveRebinding(controlSchemeIndex)
            .WithControlsExcluding("Mouse") // Exclude kbm for gamepad-only rebinding
            .WithControlsExcluding("Keyboard")
            .OnMatchWaitForAnother(0.2f)
            .OnComplete(operation => 
            {
                // this.updateText(actionToRebind.GetBindingDisplayString(controlSchemeIndex));
                this.updateText(InputControlPath.ToHumanReadableString(actionToRebind.GetBindingDisplayString()));
                actionToRebind.Enable();
                // KeybindManager.SaveBindings(); // Save updated bindings
                // Debug.LogWarning("Rebinding is now unlocked.");
                // KeybindManager.LoadBindings();
                KeybindManager.rebindLock = false; //unlock
                // eventSystem.enabled = true;
                // KeybindManager.instance.playerInputActive(true);
                KeybindManager.instance.unlockControlScheme();
                operation.Dispose();
            })
            .Start();
    }


    // Rebind for a composite gamepad binding
    public void CompositeRebind()
    {
        KeybindManager.rebindLock = true; //lock
        // eventSystem.enabled = false;
        // KeybindManager.instance.playerInputActive(false);
        KeybindManager.instance.lockControlScheme("Gamepad");

        // InputAction actionToRebind = inputActions.FindAction(actionName);
        InputAction actionToRebind = buttonAction.action;
        int bindingIndex = actionToRebind.bindings.IndexOf(x => x.isPartOfComposite && x.name == compositeElement);

        int controlSchemeIndex = -1;
        for (int i = 0; i < actionToRebind.bindings.Count; i++)
        {
            if (actionToRebind.bindings[i].path.Contains("Gamepad"))
            {
                controlSchemeIndex = i;
                break;
            }
        }
        if (controlSchemeIndex == -1)
        {
            Debug.LogError("Binding not available for gamepad.");
            KeybindManager.rebindLock = false; //unlock
            // eventSystem.enabled = true;
            // KeybindManager.instance.playerInputActive(true);
            KeybindManager.instance.unlockControlScheme();
            return;
        }

        if (bindingIndex == -1)
        {
            Debug.LogError($"No binding found for composite part '{compositeElement}'");
            KeybindManager.rebindLock = false; //unlock
            // eventSystem.enabled = true;
            // KeybindManager.instance.playerInputActive(true);
            KeybindManager.instance.unlockControlScheme();
            return;
        }
        
        actionToRebind.Disable();

        // Start the interactve rebinding for the specific composite part
        actionToRebind.PerformInteractiveRebinding(controlSchemeIndex)
            .WithControlsExcluding("Mouse") // Exclude kbm for gamepad-only rebinding
            .WithControlsExcluding("Keyboard")
            .OnMatchWaitForAnother(0.2f)
            .WithTargetBinding(bindingIndex)
            .OnComplete(operation =>
            {
                this.updateText(actionToRebind.bindings[bindingIndex].ToDisplayString());
                actionToRebind.Enable();
                Debug.Log($"Rebound '{compositeElement}' to {actionToRebind.bindings[bindingIndex].effectivePath}");
                // KeybindManager.SaveBindings(); // Save updated bindings
                // Debug.LogWarning("Rebinding is now unlocked.");
                // KeybindManager.LoadBindings();
                KeybindManager.rebindLock = false; //unlock
                // eventSystem.enabled = true;
                KeybindManager.instance.unlockControlScheme();
                operation.Dispose();
            })
            .Start();
    }

    //If an external source changes a keybinding, use this to update the menu text
    public override void refreshText()
    {
        // InputAction action = buttonAction.action;
        InputAction action = buttonAction.action;

        // Identify the correct binding (Keyboard/Mouse OR Gamepad)
        int controlSchemeIndex = -1;
        for (int i = 0; i < action.bindings.Count; i++)
        {
            if (action.bindings[i].path.Contains("Gamepad"))
            {
                controlSchemeIndex = i;
                break;
            }
        }
        // int controlSchemeIndex = action.GetBindingIndexForControlScheme;
        if (controlSchemeIndex == -1)
        {
            Debug.LogError("Binding not available for gamepad.");
            return;
        }

        Debug.Log("Refreshed text...");
        if (!isComposite)
        {
            this.updateText(action.GetBindingDisplayString(controlSchemeIndex));
        }
        else
        {
            int bindingIndex = action.bindings.IndexOf(x => x.isPartOfComposite && x.name == compositeElement);
            this.updateText(action.bindings[bindingIndex].ToDisplayString());
        }
    }
}
