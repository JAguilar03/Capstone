using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using TMPro;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private PlayerMovement player;
    [SerializeField] private List<Weapon> weapons;

    [SerializeField] private int currentWeaponIndex = 0;

    [SerializeField] bool firing; //True if player is firing a weapon

    [SerializeField] public TextMeshProUGUI ammoText;

    private Weapon currentWeapon {
        get { return transform.GetChild(currentWeaponIndex).GetComponent<Weapon>(); }
    }

    public void Start()
    {
        if (weapons.Count == 0) return; //Failsafe: If player has no weapons, abort function

        //Get current player
        player = GetComponentInParent<PlayerMovement>();

        //Instantiate all weapons in the player's inventory, and disable all but one
        foreach(Weapon w in weapons)
        {
            Weapon weaponObject = Instantiate<Weapon>(w);
            weaponObject.gameObject.SetActive(false);
            weaponObject.transform.SetParent(this.transform);
            weaponObject.transform.localPosition = new Vector3(0f, 0f, 0f);
            // weaponObject.setBulletSpawn(transform);
        }
        currentWeapon.gameObject.SetActive(true); //Enable first weapon in list
    }

    public void Update() {
        //Lazy display solution for now, ammo display should only be updated when ammo might change
        UpdateAmmoDisplay();
    }

    //Switch to another weapon
    public void selectWeapon(int index)
    {
        GameObject weaponObject = transform.GetChild(index).gameObject;
        weaponObject.SetActive(true); //Enable first weapon in list
        player.animSet = weaponObject.GetComponent<Weapon>().weaponAnimSet;
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

    private void UpdateAmmoDisplay()
    {
        ammoText.text = "Ammo: " + currentWeapon.currentAmmo.ToString() + " / " + currentWeapon.maxAmmo.ToString();
    }
}
