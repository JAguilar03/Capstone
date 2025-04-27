using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base class for all weapon types. Handles shared weapon properties such as ammo management,
// fire rate control, bullet spawning, and firing logic. Designed to be inherited by specific
// weapon implementations (e.g., Rifle, Shotgun) which override the fire behavior and other properties.
// Supports both semi-auto and automatic trigger types.

public enum TriggerType {
    SemiAuto, //Weapon only fires on button press
    Auto, //Weapon fires on press/hold
}

public class Weapon : MonoBehaviour
{
    public virtual AnimSet weaponAnimSet { get { return AnimSet.Rifle; } }

    [Header("General Weapon Fields")]
    [Header("Inscribed")]
    [SerializeField] public int maxAmmo = 100;
    [SerializeField] public int currentAmmo = 100;
    
    [SerializeField] protected int ammoPickupAmount = 100; //Ammo regained from ammo pickup
    
    [SerializeField] protected float fireDelay = 1.0f; //Cooldown between shots

    [SerializeField] protected float bulletSpeed = 10.0f;

    [SerializeField] protected float power = 5.0f;

    public virtual TriggerType triggerType { get { return TriggerType.SemiAuto; } }
    public virtual string weaponID { get { return "invalid"; } }

    //Assets
    [SerializeField] protected Bullet bulletPrefab;
    [SerializeField] protected Transform bulletSpawnPoint;
    [SerializeField] protected AudioSource gunAudio;
    [SerializeField] protected AudioClip shootSound;

    [Header("Dynamic")]
    [SerializeField] public bool onCooldown = false;
    [SerializeField] public bool holdFire = false;

    //Main
    public virtual void Update() {
        if (triggerType == TriggerType.Auto)
        {
            if (holdFire && !onCooldown)
            {
                fire();
            }
        }
    }

    //Call to fire gun once
    public virtual void fire() {}
    
    //Enable gun to fire after cooldown is finished
    public virtual void readyToFire()
    {
        onCooldown = false;
    }

    //Initialization functions
    public void setBulletSpawn(Transform t) {
        bulletSpawnPoint = t;
    }

    /// <summary>
    /// Decrease ammo, keeping within bounds
    /// </summary>
    /// <param name="cost">Amount of ammo to decrease currentAmmo by</param>
    /// <returns>Amount of ammo lost. Will be equal to cost, unless cost exceeds currentAmmo.</returns>
    public int decrementAmmo(int cost) {
        int previousAmmo = currentAmmo;
        currentAmmo -= cost;
        if (currentAmmo < 0) currentAmmo = 0; //Clamp
        return previousAmmo - currentAmmo;
    }

    /// <summary>
    /// Increase currentAmmo by ammoPickupAmount, up to maxAmmo
    /// </summary>
    /// <param name="restore">Amount of ammo to restore</param>
    public void pickupAmmo() {
        currentAmmo += ammoPickupAmount;
        if (currentAmmo > maxAmmo) currentAmmo = maxAmmo; //Clamp
    }
}
