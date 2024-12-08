using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fireball : MonoBehaviour
{
    public float speed = 5f; // Speed of the projectile
    public float lifeTime = 30f; // Time before the projectile is destroyed

    void Start()
    {
        // Destroy the projectile after a certain amount of time
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // Move the projectile forward based on its local rotation
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }
}
