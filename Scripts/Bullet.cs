using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(SelfDestruct());
    }

    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.layer == 8) {
            Destroy(gameObject);
        }
    }
}
