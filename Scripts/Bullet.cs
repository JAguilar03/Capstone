using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script defines the behavior of a bullet in a 2D environment. 
// It handles bullet destruction after 5 seconds and collision detection with enemies or specific layers. 
// If the bullet hits an enemy, it applies damage and destroys itself.

public class Bullet : MonoBehaviour
{
    [Header("Inscribed")]
    public float damage;

    void Start()
    {
        StartCoroutine(SelfDestruct());
    }

    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }

    //Bullet collision logic
    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.layer == 8) {
            Destroy(gameObject);
        }

        if (other.CompareTag("Enemy")) {
            // EnemyPathfinding2D enemy = other.GetComponent<EnemyPathfinding2D>();
            // Debug.Log("Enemy hit by bullet");
            other.GetComponent<EnemyPathfinding2D>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
