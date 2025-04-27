using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script defines the behavior of a rifle weapon, inheriting from a base Weapon class.
// It handles automatic firing, cooldown management, ammo reduction, bullet instantiation, and applying velocity based on the player's rotation.
// The rifle uses a specific animation set and plays a gunshot sound when firing.

public class Rifle : Weapon
{
    public override AnimSet weaponAnimSet { get { return AnimSet.Rifle; } }
    public override TriggerType triggerType { get { return TriggerType.Auto; } }
    public override string weaponID { get { return "rifle"; } }

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
