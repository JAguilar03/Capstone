using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles weapon pickup functionality in the game. This script is attached to a pickup object and allows the player
// to collect weapons by interacting with the pickup. The contained weapon is specified by the 'weaponPrefab' variable,
// which can be retrieved using the GetWeapon() method.

public class WeaponPickup : MonoBehaviour
{
    //Weapon contained by the pickup
    [SerializeField] Weapon weaponPrefab;

    public Weapon GetWeapon()
    {
        return weaponPrefab;
    }
}
