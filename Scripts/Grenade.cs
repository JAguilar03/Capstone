using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    enum GrenadeState {inFlight, landed};

    [Header("Inscribed")]
    [SerializeField] float fuseLength = 2.0f; //How long the grenade will exist before exploding
    [SerializeField] float explosionRadius = 1.0f;
    [SerializeField] GameObject explosionParticle;
    [SerializeField] private LayerMask HitLayer;    //Layers explosion can hit
    [SerializeField] private LayerMask CoverLayer;  //Layers that provide cover from explosion
    [SerializeField] private int MaxHits = 25;

    [Header("Dynamic")]
    [SerializeField] GrenadeState state = GrenadeState.inFlight;
    [SerializeField] float explosionTime; //Timestamp at which grenade will explode

    private Collider2D[] Hits;

    // Start is called before the first frame update
    private void Awake() {
        Hits = new Collider2D[MaxHits];
    }

    // Update is called once per frame
    void Update()
    {
        if (state == GrenadeState.inFlight)
        {
            state = GrenadeState.landed;
            Invoke("explode", fuseLength);
        }
    }

    void explode()
    {
        Vector3 origin = transform.position;

        //Get all entities in range
        int numHits = Physics2D.OverlapCircleNonAlloc(origin, explosionRadius, Hits, HitLayer);

        //Check each entity in range to determine exposure to explosion; deal damage
        for (int i = 0; i < numHits; i++)
        {
            GameObject hitGO = Hits[i].gameObject;
            Debug.Log(hitGO);
            if (Hits[i].TryGetComponent<PlayerMovement>(out PlayerMovement player))
            {
                float distance = Vector3.Distance(origin, Hits[i].transform.position);

                if (!Physics2D.Raycast(origin, (Hits[i].transform.position - origin).normalized, explosionRadius, CoverLayer.value))
                {
                    player.TakeDamage(80f);
                }
            }
            else if (Hits[i].TryGetComponent<EnemyPathfinding2D>(out EnemyPathfinding2D enemy))
            {
                float distance = Vector3.Distance(origin, Hits[i].transform.position);

                if (!Physics2D.Raycast(origin, (Hits[i].transform.position - origin).normalized, explosionRadius, CoverLayer.value))
                {
                    enemy.TakeDamage(80f);
                }
            }
        }

        //Spawn Explosion Particle
        Instantiate<GameObject>(explosionParticle, transform.position, transform.rotation); //No need to store gameobject in variable, it's just a particle

        //Grenade has exploded -- destroy it
        Destroy(this.gameObject);
    }
}
