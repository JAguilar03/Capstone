using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerMovement;

// This script defines the behavior of a Pistol weapon in the game. It inherits from the Weapon class and
// overrides necessary properties such as weapon animation set, trigger type (semi-automatic), and weapon ID 
// (set to "pistol"). The fire() method handles the weapon's firing logic, including checking for cooldowns,
// decrementing ammo, instantiating a bullet, calculating its direction based on the player's rotation, and 
// applying velocity to the bullet. It also plays the gun's firing sound and manages the cooldown period 
// before the weapon can be fired again.

public class Pistol : Weapon
{
    public override AnimSet weaponAnimSet { get { return AnimSet.Pistol; } }
    public override TriggerType triggerType { get { return TriggerType.SemiAuto; } }
    public override string weaponID { get { return "pistol"; } }

    public override void fire()
    {
        if (onCooldown) return; //Don't fire if weapon is on cooldown

        //Decrease ammo; do not fire if no ammo left
        int ammoLost = decrementAmmo(1);
        if (ammoLost == 0) return;

        // Debug.Log("Fired Pistol!");

        //Create bullet
        Bullet bullet = Instantiate(bulletPrefab);
        bullet.transform.position = bulletSpawnPoint.position;
        bullet.damage = power; //Set bullet damage
        Rigidbody2D bulletRB = bullet.GetComponent<Rigidbody2D>();

        // Get the transform's global angle
        //  This should rotate with the player, as the WeaponController is a child object of the player
        Vector3 eulerRot = transform.rotation.eulerAngles;
        float angle = eulerRot.z;

        // Calculate bullet direction using the player's angle
        Vector2 bulletDirection = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        bulletRB.velocity = bulletDirection * bulletSpeed;

        // pew 
        gunAudio.PlayOneShot(shootSound);
        // gunAudio.Play();

        onCooldown = true;
        Invoke("readyToFire", fireDelay);
    }
}
