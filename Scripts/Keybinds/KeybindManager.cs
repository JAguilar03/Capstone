using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// This script manages custom keybindings for the player's controls using Unity's Input System.
// It acts as a singleton to ensure consistent access across the project and supports saving, loading,
// and resetting bindings for keyboard, mouse, and gamepad devices.
// The manager handles rebinding locks, control scheme detection, and communication with player input,
// ensuring only one rebinding operation happens at a time and that changes persist across sessions.

public class KeybindManager : MonoBehaviour
{
    //Singleton for player character (PC). Anything involving custom keybinds 
    // should reference this object. (Gamepad bindings, too)
    //(Should be automatically set to null if stored singleton is destroyed)
    //(...that shouldn't ever happen, though.)
    public static KeybindManager instance;

    public InputActionAsset inputActions;
    
    public PlayerMovement player;
    public WeaponController weaponController;

    //Used to prevent multiple rebind operations from occurring at the same time
    [SerializeField] bool _rebindLock = false;
    public static bool rebindLock {
        set {
                if (instance != null)
                    instance._rebindLock = value;
                else
                    Debug.LogError("No KeybindManager instance set.");
            }
        get {
                if (instance != null)
                    return instance._rebindLock;
                else
                {
                    Debug.LogError("No KeybindManager instance set.");
                    return false;
                }
            }
    }
    private bool isGamepad = false; //Is the current control scheme gamepad?
    // private float controllerDeadzone = 0.1f;

    // Use this to reset bindings in-editor
    // public bool resetBindings = false;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            Debug.Log("KeybindManager initialized!");
            instance = this;
            loadBindings();
            DontDestroyOnLoad(this);
            // Make sure PlayerInput is attached
            if (GetComponent<PlayerInput>() == null)
            {
                Debug.LogError("PlayerInput component is missing from GameObject!");
            }
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
    }

    void Start()
    {
        updateRefs();
    }

    // OnDestroy, clear the singleton
    void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;        
        }
    }

    // Enable/Disable
    // Need to disable KeybindManager on rebind screen, it's the simplest way to avoid problems
    public static void Enable()
    {
        if (instance == null)
            return;
        instance.gameObject.SetActive(true);
    }

    public static void Disable()
    {
        if (instance == null)
            return;
        instance.gameObject.SetActive(false);
    }

    //Hides transform, forces defaults
    void OnValidate()
    {
        transform.position = new Vector3(0f, 0f, 0f);
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        transform.localScale = new Vector3(1f, 1f, 1f);
        
        transform.hideFlags = HideFlags.NotEditable | HideFlags.HideInInspector;
        // transform.hideFlags = 0;
    }

    public void updateRefs()
    {
        //Get player instance
        if (PlayerManager.playerInstance != null)
        {
            player = PlayerManager.playerInstance.GetComponent<PlayerMovement>();
            weaponController = player.GetComponentInChildren<WeaponController>();
        }
    }

    //Rebind a key
    public void rebind(string actionName, string keyName)
    {
        InputAction actionToRebind = inputActions.FindAction(actionName);

        if (actionToRebind == null)
        {
            Debug.LogError($"Action {actionName} not found!");
            return;
        }
        else
        {
            Debug.Log($"Action {actionName} found. Rebind would have been successful.");
        }

        // actionToRebind.Disable();
        // rebindText.text = "Press a key...";

        // actionToRebind.PerformInteractiveRebinding()
        //     .OnComplete(operation =>
        //     {
        //         rebindText.text = actionToRebind.GetBindingDisplayString();
        //         actionToRebind.Enable();
        //         SaveBindings(); // Save updated bindings
        //     })
        //     .Start();
    }

    public void OnDeviceChange()
    {
        Debug.Log("OnDeviceChange called");  // Debug log to check if this method is being triggered

        PlayerInput pin = GetComponent<PlayerInput>();
        if (string.IsNullOrEmpty(pin.currentControlScheme))
        {
            Debug.LogWarning("No control scheme detected at start. Forcing Keyboard&Mouse.");
            pin.SwitchCurrentControlScheme("Keyboard", Keyboard.current, Mouse.current);
        }
        isGamepad = pin.currentControlScheme.Equals("Gamepad") ? true : false;

        if (player != null)
        {
            player.isGamepad = isGamepad;
        }

        if (pin == null)
        {
            Debug.LogError("PlayerInput component not found!");
            return;
        }
    }

    public void prevWeapon(InputAction.CallbackContext context)
    {
        if (PlayerManager.playerInstance == null)
            return;
        weaponController.prevWeapon(context);
    }

    public void nextWeapon(InputAction.CallbackContext context)
    {
        if (PlayerManager.playerInstance == null)
            return;
        weaponController.nextWeapon(context);
    }

    public void shoot(InputAction.CallbackContext context)
    {
        if (PlayerManager.playerInstance == null)
            return;
        weaponController.fireWeapon(context);
    }

    public void grenade(InputAction.CallbackContext context)
    {
        if (PlayerManager.playerInstance == null)
            return;
        player.ThrowGrenade(context);
    }

    public void melee(InputAction.CallbackContext context)
    {
        if (PlayerManager.playerInstance == null)
            return;
        // Debug.Log(context.ReadValue<Vector2>());
        player.punch(context);
    }

    public void debugBind(InputAction.CallbackContext context)
    {
        Debug.Log(context);
    }

    public void movement(InputAction.CallbackContext context)
    {
        if (PlayerManager.playerInstance == null)
            return;
        // Debug.Log(context.ReadValue<Vector2>());
        player.setMovement(context);
    }

    public void aim(InputAction.CallbackContext context)
    {
        if (PlayerManager.playerInstance == null)
            return;
        // Debug.Log(context.ReadValue<Vector2>());
        player.setAim(context);
    }

    public void pause(InputAction.CallbackContext context)
    {
        if (PauseManager.instance == null)
            return;
        PauseManager.instance.TogglePause();
    }

    //Save/Load Keybinds
    public void loadBindings()
    {
        var rebinds = PlayerPrefs.GetString("rebinds");
        if (!string.IsNullOrEmpty(rebinds))
            inputActions.LoadBindingOverridesFromJson(rebinds);
    }

    public void saveBindings()
    {
        var rebinds = inputActions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("rebinds", rebinds);
    }

    // static public void SaveBindings()
    // {
    //     string bindings = instance.inputActions.SaveBindingOverridesAsJson();
    //     // Debug.Log(bindings);
    //     PlayerPrefs.SetString("KeyBindings", bindings);
    //     PlayerPrefs.Save();
    // }

    // static public void LoadBindings()
    // {
    //     if (PlayerPrefs.HasKey("KeyBindings"))
    //     {
    //         string bindings = PlayerPrefs.GetString("KeyBindings");
    //         instance.inputActions.LoadBindingOverridesFromJson(bindings);
    //     }
    // }

    // static public void ResetBindings()
    // {
    //     if (instance != null)
    //         instance.inputActions.RemoveAllBindingOverrides();
    //     PlayerPrefs.DeleteKey("KeyBindings");
    //     PlayerPrefs.Save();
    // }

    // static public void ResetKeyboardBindings()
    // {
    //     foreach (var action in instance.inputActions)
    //     {
    //         for (int i = 0; i < action.bindings.Count; i++)
    //         {
    //             // Check if the binding is from a Keyboard or Mouse
    //             if (action.bindings[i].path.Contains("Keyboard") || action.bindings[i].path.Contains("Mouse"))
    //             {
    //                 action.RemoveBindingOverride(i);
    //             }
    //         }
    //     }
    //     SaveBindings();
    //     LoadBindings();
    // }

    // static public void ResetGamepadBindings()
    // {
    //     foreach (var action in instance.inputActions)
    //     {
    //         for (int i = 0; i < action.bindings.Count; i++)
    //         {
    //             // Check if the binding is from a Keyboard or Mouse
    //             if (action.bindings[i].path.Contains("Gamepad"))
    //             {
    //                 action.RemoveBindingOverride(i);
    //             }
    //         }
    //     }
    //     SaveBindings();
    //     LoadBindings();
    // }

    public void playerInputActive(bool isActive)
    {
        GetComponent<PlayerInput>().enabled = isActive;
    }

    public void lockControlScheme(string schemeName)
    {
        PlayerInput playerInput = GetComponent<PlayerInput>();
        playerInput.SwitchCurrentControlScheme(schemeName);
        playerInput.neverAutoSwitchControlSchemes = true;
    }

    public void unlockControlScheme()
    {
        PlayerInput playerInput = GetComponent<PlayerInput>();
        playerInput.neverAutoSwitchControlSchemes = false;
    }
}
