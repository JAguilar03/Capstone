using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script defines the Shotgun weapon behavior, inheriting from the base Weapon class.
// It fires multiple pellet projectiles with randomized spread to simulate a shotgun blast.
// Uses semi-auto trigger and includes cooldown, ammo check, and firing audio feedback.

public class Shotgun : Weapon
{
    [SerializeField] private int pelletCount = 8; // Number of pellets per shot
    [SerializeField] private float spreadAngle = 30f; // Total spread angle in degrees
    public override string weaponID { get { return "shotgun"; } }

    public override AnimSet weaponAnimSet { get { return AnimSet.Rifle; } }
    public override TriggerType triggerType { get { return TriggerType.SemiAuto; } }

    public override void fire()
    {
        if (onCooldown) return;

        //Decrease ammo; do not fire if no ammo left
        int ammoLost = decrementAmmo(1);
        if (ammoLost == 0) return;

        for (int i = 0; i < pelletCount; i++)
        {
            Bullet bullet = Instantiate(bulletPrefab);
            bullet.transform.position = bulletSpawnPoint.position;
            bullet.damage = power; //Set bullet damage
            Rigidbody2D bulletRB = bullet.GetComponent<Rigidbody2D>();

            // Get base angle from transform
            float baseAngle = transform.rotation.eulerAngles.z;
            
            // Add random spread within spreadAngle
            float randomSpread = Random.Range(-spreadAngle/2, spreadAngle/2);
            float finalAngle = baseAngle + randomSpread;

            // Calculate bullet direction with spread
            Vector2 bulletDirection = new Vector2(
                Mathf.Cos(finalAngle * Mathf.Deg2Rad), 
                Mathf.Sin(finalAngle * Mathf.Deg2Rad)
            );
            
            bulletRB.velocity = bulletDirection * bulletSpeed;
        }

        gunAudio.PlayOneShot(shootSound);
        onCooldown = true;
        Invoke("readyToFire", fireDelay);
    }
}
