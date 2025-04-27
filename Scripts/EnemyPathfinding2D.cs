// Enemy swipe Sound Effect by 666HeroHero from Pixabay

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Scripting.APIUpdating;

// This script controls the enemy's movement, pathfinding, and combat behavior. 
// The enemy chases the player within a certain range, attacks when close, and handles health and item drops upon death.
// It uses Unity's NavMesh for pathfinding and adjusts the enemy's animations during movement and attack.


public class EnemyPathfinding2D : MonoBehaviour
{
    private ScoreManager scoreManager;
    public AudioSource attackSound;

    public Transform player;         // Reference to the player's transform
    public PlayerMovement playerMovement;
    public float moveSpeed = 3f;     // Speed of the enemy
    public float chaseDistance = 15f; // Distance at which the enemy starts chasing the player

    private Rigidbody2D rb;          // Reference to the enemy's Rigidbody2D
    private Vector2 movement;        // Stores the direction of movement

    public Animator animateLegs;
    public Animator animateBody;
    public Animator animateDeath;
    public float animationSpeed = 1f;
    public float health = 10f;
    public float attack = 25f;
    public float attackRange = 1.5f;
    private bool isDead = false;
    private bool isAttacking = false;
    public List<GameObject> itemDrops;

    //NavMesh Pathfinding
    [SerializeField] Transform target;
    NavMeshAgent agent;


    void Start()
    {
        // Get the Rigidbody2D component attached to the enemy
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("player").transform;
        target = player;
        agent = GetComponent<NavMeshAgent>();
        playerMovement = player.gameObject.GetComponent<PlayerMovement>();
        scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();

        // set animation speed
        animateLegs.speed = animationSpeed;
        animateBody.speed = animationSpeed;

        //Set NavMesh pathfinding fields
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        
    }

    void Update()
    {
        // Calculate the distance between the enemy and the player
        float distanceToPlayer = Vector2.Distance(player.position, transform.position);
        // Debug.Log("Distance to Player = " + distanceToPlayer);

        if (distanceToPlayer <= chaseDistance && distanceToPlayer > attackRange && !isDead)
        {
            // Calculate the direction vector from the enemy to the player
            // Vector2 direction = (player.position - transform.position).normalized;
            
            // Store the direction in the movement variable
            // movement = direction;
            agent.SetDestination(target.position);
            

            // rotate to look at player
            // Vector3 diff = player.position - transform.position;
            // diff.Normalize();  
            Vector3 moveDirection = agent.desiredVelocity;
            // float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            float rot_z = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, rot_z);
            animateLegs.Play("Enemy_Walk");
            animateBody.Play("Enemy_Walk");
        }
        else if(distanceToPlayer <= attackRange && !isDead) {
            movement = Vector2.zero;
            // attack player
            Vector3 direction = (player.position - transform.position).normalized;
            float rot_z = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, rot_z);
            animateBody.Play("Enemy_Attack");
            // if (!isAttacking) { 
            //     isAttacking = true;
            //     StartCoroutine(Attack());      
            // }
        }
        else
        {
            if (isDead) {
            // stop enemy movement
            movement = Vector2.zero;
            transform.position += new Vector3(0, 0, 0.0f);
            transform.rotation = Quaternion.identity;
            agent.SetDestination(gameObject.transform.position);
            // toggle active states of collider and GOs
            transform.GetComponent<CircleCollider2D>().enabled = false;
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
            transform.GetChild(2).gameObject.SetActive(true);
            transform.GetChild(3).gameObject.SetActive(false);
            // animate death and start death timer
            animateDeath.Play("Enemy_Death");
            StartCoroutine(DeathTimer(5f));
            } 
            else {
                // If the player is out of range, stop movement
                movement = Vector2.zero;
                animateLegs.Play("Enemy_Idle");
                animateBody.Play("Enemy_Idle");
            }

        }

    }

    void FixedUpdate()
    {
        // Move the enemy using Rigidbody2D
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    //NOTE: Bullet damage logic has been moved to the bullet itself (will likely move melee later). This is so
    //      things are more organized if/when we add other damage effects (e.g. hitscan, explosions)
    void OnTriggerEnter2D(Collider2D other) {
        // if (other.CompareTag("bullet")) {
        //     // Debug.Log("Enemy hit by bullet");
        //     health -= 5;
        //     Destroy(other.gameObject);
        //     checkHP();
        // }
        // else if (other.CompareTag("PlayerMelee")) {
        //     health -= 5;
        //     checkHP();
        // }

        if (other.CompareTag("PlayerMelee")) {
            health -= 5;
            checkHP();
        }
    }

    private void checkHP() {
        if (health <= 0) {
            if (scoreManager != null){
                scoreManager.AddScore(100);
            }
            isDead = true;

            // check if items in list
            if (itemDrops.Count == 0)
            {
                Debug.LogWarning("Item Drops list is empty!");
                return;
            }

            // item drop logic
            int randIndex = Random.Range(0, itemDrops.Count);
            GameObject randItem = itemDrops[randIndex];
            GameObject spawnedItem;

            if (randItem.name == "EmptyPack") {
                // if empty spawn as a child of enemy so it auto destroys
                spawnedItem = Instantiate(randItem, transform);
            } else {
                // spawn item drop normally
                spawnedItem = Instantiate(randItem, transform.position, Quaternion.identity);
                // destroy after 30 sec
                Destroy(spawnedItem, 30f);
            }

        }
    }

    public void TakeDamage(float damage) {
        health -= damage;
        checkHP();
    }

    IEnumerator DeathTimer(float delay) {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    IEnumerator Attack() {
        animateBody.Play("Enemy_Attack");
        attackSound.Play();
        playerMovement.TakeDamage(attack); // Use the TakeDamage method instead of directly modifying health
        yield return new WaitForSeconds(3f);
        isAttacking = false;
    }
}
