using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script controls the behavior of a fireball projectile. It sets the initial speed and direction of the fireball,
// using a Rigidbody2D component to apply velocity. The fireball will automatically destroy itself after a specified 
// lifetime. Additionally, the fireball checks for collisions with certain objects (in this case, objects on layer 8) 
// and destroys itself upon collision.

public class Fireball : MonoBehaviour
{
    public float speed = 5f; // Speed of the projectile
    public float lifeTime = 30f; // Time before the projectile is destroyed
    private Rigidbody2D rb; // Reference to the Rigidbody2D component

    void Start()
    {
        // Get the Rigidbody2D component
        rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            // Set the initial velocity of the fireball
            rb.velocity = transform.right * speed;
        }

        // Destroy the projectile after a certain amount of time
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 8)
        {
            Destroy(gameObject);
        }
    }
}
