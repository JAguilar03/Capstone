using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.EventSystems;

// This script handles rebinding keyboard and mouse input actions in Unity's Input System.
// It allows users to reassign controls via an interactive UI, supporting both regular and composite bindings (e.g., WASD).
// The script interacts with the KeybindManager to lock input during rebinding, update displayed keybind labels, and save/load bindings.
// It also ensures that the rebinding interface ignores gamepad inputs to keep control schemes consistent.

public class KeybindButton : ControlBindButton
{
    KeybindManager keybindManager;
    InputActionAsset inputActions;
    public InputActionReference buttonAction;

    //Handle composite input (curse you WASD)
    [SerializeField] bool isComposite = false;
    [SerializeField] string compositeElement;

    // Start is called before the first frame update
    void Start()
    {
        assignEventSystem();
        keybindManager = KeybindManager.instance;
        inputActions = keybindManager.inputActions;

        //Update display to match control config
        // if (isComposite)
        // {
        //     int bindingIndex = buttonAction.action.bindings.IndexOf(x => x.isPartOfComposite && x.name == compositeElement);
        //     this.updateText(buttonAction.action.bindings[bindingIndex].ToDisplayString());
        // }
        // else
        // {
        //     this.updateText(buttonAction.action.GetBindingDisplayString());
        // }
        refreshText();
    }

    // Rebind a key
    public void StartRebinding()
    {
        if (KeybindManager.rebindLock == true)
        {
            Debug.LogWarning($"Rebinding is currently locked.");
            return;
        }

        if (isComposite)
        {
            CompositeRebind();
        }
        else
        {
            Rebind();
        }
    }

    public void Rebind()
    {
        KeybindManager.rebindLock = true; //lock
        eventSystem.enabled = false;
        KeybindManager.instance.playerInputActive(false);
        // Debug.LogWarning("Rebinding is now locked.");

        // InputAction actionToRebind = inputActions.FindAction(actionName);
        InputAction actionToRebind = buttonAction.action;

        actionToRebind.Disable();
        this.updateText("...");

        // Identify the correct binding (Keyboard/Mouse OR Gamepad)
        int controlSchemeIndex = -1;
        for (int i = 0; i < actionToRebind.bindings.Count; i++)
        {
            if (actionToRebind.bindings[i].path.Contains("Keyboard") || actionToRebind.bindings[i].path.Contains("Mouse"))
            {
                controlSchemeIndex = i;
                break;
            }
        }
        if (controlSchemeIndex == -1)
        {
            Debug.LogError("Binding not available for keyboard & mouse.");
            KeybindManager.rebindLock = false; //unlock
            eventSystem.enabled = true;
            KeybindManager.instance.playerInputActive(true);
            return;
        }

        actionToRebind.PerformInteractiveRebinding(controlSchemeIndex)
            .WithControlsExcluding("Gamepad")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation =>
            {
                // this.updateText(actionToRebind.GetBindingDisplayString(controlSchemeIndex));
                // this.updateText(InputControlPath.ToHumanReadableString(actionToRebind.GetBindingDisplayString(controlSchemeIndex), InputControlPath.HumanReadableStringOptions.OmitDevice));
                this.refreshText();
                actionToRebind.Enable();
                // KeybindManager.SaveBindings(); // Save updated bindings
                // Debug.LogWarning("Rebinding is now unlocked.");
                // KeybindManager.LoadBindings();
                KeybindManager.rebindLock = false; //unlock
                eventSystem.enabled = true;
                KeybindManager.instance.playerInputActive(true);
                operation.Dispose();
            })
            .Start();
    }

    public void CompositeRebind()
    {
        KeybindManager.rebindLock = true; //lock
        eventSystem.enabled = false;
        KeybindManager.instance.playerInputActive(false);
        // Debug.LogWarning("Rebinding is now locked.");

        // InputAction actionToRebind = inputActions.FindAction(actionName);
        InputAction actionToRebind = buttonAction.action;
        int bindingIndex = actionToRebind.bindings.IndexOf(x => x.isPartOfComposite && x.name == compositeElement);

        // Identify the correct binding (Keyboard/Mouse OR Gamepad)
        int controlSchemeIndex = -1;
        for (int i = 0; i < actionToRebind.bindings.Count; i++)
        {
            if (actionToRebind.bindings[i].path.Contains("Keyboard") || actionToRebind.bindings[i].path.Contains("Mouse"))
            {
                controlSchemeIndex = i;
                break;
            }
        }
        if (controlSchemeIndex == -1)
        {
            Debug.LogError("Binding not available for keyboard & mouse.");
            KeybindManager.rebindLock = false; //unlock
            eventSystem.enabled = true;
            KeybindManager.instance.playerInputActive(true);
            return;
        }

        if (bindingIndex == -1)
        {
            Debug.LogError($"No binding found for composite part '{compositeElement}'");
            KeybindManager.rebindLock = false; //unlock
            eventSystem.enabled = true;
            KeybindManager.instance.playerInputActive(true);
            return;
        }
        
        actionToRebind.Disable();

        actionToRebind.PerformInteractiveRebinding(bindingIndex)
            .WithControlsExcluding("Gamepad")
            .OnMatchWaitForAnother(0.1f)
            .WithTargetBinding(bindingIndex)
            .OnComplete(operation =>
            {
                // this.updateText(actionToRebind.bindings[bindingIndex].ToDisplayString());;
                // this.updateText(InputControlPath.ToHumanReadableString(actionToRebind.bindings[bindingIndex].ToDisplayString()));
                this.refreshText();
                actionToRebind.Enable();
                Debug.Log($"Rebound '{compositeElement}' to {actionToRebind.bindings[bindingIndex].effectivePath}");
                // KeybindManager.SaveBindings(); // Save updated bindings
                // Debug.LogWarning("Rebinding is now unlocked.");
                // KeybindManager.LoadBindings();
                KeybindManager.rebindLock = false; //unlock
                eventSystem.enabled = true;
                KeybindManager.instance.playerInputActive(true);
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
            if (action.bindings[i].path.Contains("Keyboard") || action.bindings[i].path.Contains("Mouse"))
            {
                controlSchemeIndex = i;
                break;
            }
        }
        // int controlSchemeIndex = action.GetBindingIndexForControlScheme;
        if (controlSchemeIndex == -1)
        {
            Debug.LogError("Binding not available for mouse and keyboard.");
            KeybindManager.rebindLock = false; //unlock
            return;
        }

        Debug.Log("Refreshed text...");
        if (!isComposite)
        {
            // this.updateText(action.GetBindingDisplayString(controlSchemeIndex));
            this.updateText(InputControlPath.ToHumanReadableString(action.GetBindingDisplayString(controlSchemeIndex), InputControlPath.HumanReadableStringOptions.OmitDevice));
        }
        else
        {
            int bindingIndex = action.bindings.IndexOf(x => x.isPartOfComposite && x.name == compositeElement);
            // this.updateText(action.bindings[bindingIndex].ToDisplayString());
            this.updateText(InputControlPath.ToHumanReadableString(action.bindings[bindingIndex].ToDisplayString(), InputControlPath.HumanReadableStringOptions.OmitDevice));
        }
    }
}
