using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttacks : MonoBehaviour
{
    public GameObject Enemy;
    private GameObject EnemyHitbox;
    public bool isMelee = true;
    public GameObject Fireball;

    void Start() {
        EnemyHitbox = Enemy.transform.GetChild(3).gameObject;
    }

    // Called on the attacking frames of Enemy Attack animation
    public void StartAttack() {
        // move hitbox in positon
        EnemyHitbox.transform.localPosition = new Vector3(1f, 0, 0);

        if (isMelee) {
            // Toggle Enemy Attack Hitbox on 
            EnemyHitbox.SetActive(true);
        } else {
            // use hitbox as spawn point for projectile attack
            Instantiate(Fireball, EnemyHitbox.transform.position, EnemyHitbox.transform.rotation);
        }
        
    }

    // Called on the frame after attacking frames
    public void EndAttack() {
        // move hitbox out of positon
        EnemyHitbox.transform.localPosition = new Vector3(-1f, 0, 0);

        if (isMelee) {
            // Toggle Enemy Attack Hitbox off
        EnemyHitbox.SetActive(false);
        }
    }
}
