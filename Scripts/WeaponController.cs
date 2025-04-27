using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using TMPro;

// Handles player weapon management, including initialization, firing, switching, and ammo tracking.
// Supports both automatic and semi-automatic trigger types, and allows dynamic loading of weapons
// from save files using a weapon dictionary. Works in tandem with HUD and PlayerMovement to manage
// in-game visuals and interactions. Also handles weapon instantiation and cycling through inventory.

public class WeaponController : MonoBehaviour
{
    [SerializeField] bool ready = true; //Set to false for scenarios where the player is being loaded from a file

    [SerializeField] private PlayerMovement player;
    [SerializeField] private List<Weapon> weapons;

    [SerializeField] private int currentWeaponIndex = 0;

    [SerializeField] bool firing; //True if player is firing a weapon

    // Weapon Dictionary - used for restoring save files
    [SerializeField] private WeaponDict weaponDict;

    private Weapon currentWeapon {
        get { return transform.GetChild(currentWeaponIndex).GetComponent<Weapon>(); }
    }

    public void Start()
    {
        if (ready) initializeNew();
    }
    
    public void Update() {
        //Lazy display solution for now, ammo display should only be updated when ammo might change
        // UpdateAmmoDisplay();
        
        HUDManager.hudInstance.SetAmmoDisplay(currentWeapon.currentAmmo, currentWeapon.maxAmmo);
    }

    //======GETTERS AND SETTERS======
    public List<Weapon> getWeaponList()
    {
        return weapons;
    }

    //Set up weapon controller
    //Runs automatically by default, or can be run manually if loading from file
    public void initializeNew()
    {
        if (weapons.Count == 0) //Failsafe: If player has no weapons, abort function
        {
            Debug.LogError("WeaponController contains no weapons!");
            return;
        }

        //Get current player
        player = GetComponentInParent<PlayerMovement>();

        //Instantiate all weapons in the player's inventory, and disable all but one
        foreach(Weapon w in weapons)
        {
            instantiateWeapon(w);
        }
        currentWeapon.gameObject.SetActive(true); //Enable first weapon in list
    }

    //Set up a single weapon
    //Called when first initializing the WeaponController, as well as whenever a weapon is added to the inventory
    Weapon instantiateWeapon(Weapon newWeapon)
    {
        Weapon weaponObject = Instantiate<Weapon>(newWeapon);
        weaponObject.gameObject.SetActive(false);
        weaponObject.transform.SetParent(this.transform);
        weaponObject.transform.localPosition = new Vector3(0f, 0f, 0f);
        weaponObject.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
        // weaponObject.setBulletSpawn(transform);
        return weaponObject;
    }

    //Add weapon to list
    //Used for loading file as well as pickups
    //(For pickups, use instantiate = true)
    public void addWeapon(Weapon newWeapon, bool instantiate = false)
    {
        if(!hasWeapon(newWeapon.weaponID))
        {
            Debug.Log("Picked up weapon of type " + newWeapon.weaponID);
            weapons.Add(newWeapon);

            if (instantiate)
                instantiateWeapon(newWeapon);
        }
        else
        {
            Debug.Log("Picked up duplicate weapon of type " + newWeapon.weaponID + ". Restoring ammo.");
            pickupAmmo();
        }
    }

    //Switch to another weapon
    public void selectWeapon(int index)
    {
        GameObject weaponObject = transform.GetChild(index).gameObject;
        weaponObject.SetActive(true); //Enable first weapon in list
        player.animSet = weaponObject.GetComponent<Weapon>().weaponAnimSet;

        HUDManager.hudInstance.SetAmmoIcon(weaponObject.GetComponent<Weapon>().GetType().Name);
    }

    //Cleanup function for switching away from a weapon
    public void deselectWeapon(int index)
    {
        GameObject weaponObject = transform.GetChild(index).gameObject;
        weaponObject.SetActive(false); //Enable first weapon in list
    }

    //Cycle weapon to the previous weapon in array
    public void prevWeapon(InputAction.CallbackContext context)
    {
        if (PauseManager.IsGamePaused) return;
        if (context.phase != InputActionPhase.Started) return;
        if (weapons.Count == 0) return;

        deselectWeapon(currentWeaponIndex);
        currentWeaponIndex = (currentWeaponIndex - 1 + weapons.Count) % weapons.Count; //I'm going to throttle whoever made `%` a remainder operator rather than modulo
        selectWeapon(currentWeaponIndex);
    }

    //Cycle weapon to the next weapon in array
    public void nextWeapon(InputAction.CallbackContext context)
    {
        if (PauseManager.IsGamePaused) return;
        if (context.phase != InputActionPhase.Started) return;
        if (weapons.Count == 0) return;

        deselectWeapon(currentWeaponIndex);
        currentWeaponIndex = (currentWeaponIndex + 1) % weapons.Count;
        selectWeapon(currentWeaponIndex);
    }

    //======WEAPON FIRING FUNCTIONS======

    //Fire current weapon
    public void fireWeapon(InputAction.CallbackContext context)
    {
        if (PauseManager.IsGamePaused) return;

        if (currentWeaponIndex >= 0 && currentWeaponIndex < transform.childCount)
        {
            Transform child = transform.GetChild(currentWeaponIndex);
        }
        else
        {
            Debug.LogError($"Invalid index {currentWeaponIndex}. Child count: {transform.childCount}. Weapons Array Length: {weapons.Count}");
        }

        switch (currentWeapon.triggerType)
        {
            case (TriggerType.SemiAuto):
                handleSemiAuto(context);
                break;
            case (TriggerType.Auto):
                handleAuto(context);
                break;
        }
    }

    //Semi-Automatic
    private void handleSemiAuto(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Started)
        {
            currentWeapon.fire();
        }
    }

    //Semi-Automatic
    private void handleAuto(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Started)
        {
            currentWeapon.holdFire = true;
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            currentWeapon.holdFire = false;
        }
    }

    //======WEAPON AMMO FUNCTIONS======
    //Restore ammo to all weapons via pickup
    public void pickupAmmo() {
        for (int i = 0; i < weapons.Count; i++) {
            transform.GetChild(i).gameObject.GetComponent<Weapon>().pickupAmmo();
        }
    }

    // private void UpdateAmmoDisplay()
    // {
    //     ammoText.text = "Ammo: " + currentWeapon.currentAmmo.ToString() + " / " + currentWeapon.maxAmmo.ToString();
    // }

    //======LOAD FILE OPERATIONS======
    //Load a set of weapons from a PlayerFileSO
    public void initializeFromLoad(PlayerFileSO playerData)
    {
        //Get current player
        player = GetComponentInParent<PlayerMovement>();

        foreach(WeaponData wD in playerData.weapons.ToList())
        {
            //Get the prefab associated with a weapon key, and instantiate it
            Debug.Log("Loading weapon from key \"" + wD.WeaponKey + "\"...");
            Weapon weaponPrefab = weaponDict.getWeapon(wD.WeaponKey);
            if (weaponPrefab == null)
            {
                Debug.LogError("Weapon key \"" + wD.WeaponKey + "\" is invalid!");
                continue;
            }
            weapons.Add(weaponPrefab); //Add weapon to inventory list just in case
            Weapon weaponInstance = instantiateWeapon(weaponPrefab);
            weaponInstance.currentAmmo = wD.currAmmo;
            weaponInstance.maxAmmo = wD.maxAmmo;
        }
        currentWeapon.gameObject.SetActive(true); //Enable first weapon in list
        HUDManager.hudInstance.SetAmmoIcon(currentWeapon.GetType().Name);

        ready = true;
    }

    //=====MISC UTILITY FUNCTIONS=====
    bool hasWeapon(string weaponKey)
    {
        foreach(Weapon w in weapons)
        {
            if (weaponKey == w.weaponID)
            {
                return true;
            }
        }
        return false;
    }
}
