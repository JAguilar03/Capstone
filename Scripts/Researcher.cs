using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Researcher : MonoBehaviour
{
    public Transform player;          // Reference to the player's transform

    public static GameObject[] researcherSpawnPoints; // Spawn points for researcher
    public GameObject goal;           // Reference to the goal object (assigned in the inspector)
    public float moveSpeed = 3f;      // Speed of the NPC
    public float followDistance = 5f; // Maximum distance to start following the player
    public float stopDistance = 2f;   // Minimum distance to stop near the player
    public float goalSwitchDistance = 3f; // Distance from the goal to switch to it

    private Rigidbody2D rb;           // Reference to the NPC's Rigidbody2D
    private Vector2 movement;         // Stores the direction of movement
    private bool isFollowing = false; // Flag to check if the researcher has started following the player
    private bool isDead = false;
    // Animation
    public Animator animateLegs;
    public Animator animateBody;
    public Animator animateDeath;
    public float animationSpeed = 1f;

    // NavMesh Pathfinding
    private NavMeshAgent agent;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("player").transform;
        agent = GetComponent<NavMeshAgent>();

         // Get all spawn points
        researcherSpawnPoints = GameObject.FindGameObjectsWithTag("ResearcherSpawn");
    
        // Pick random spawn point and set position
        if (researcherSpawnPoints.Length > 0)
        {
            int randomIndex = Random.Range(0, researcherSpawnPoints.Length);
            print("Selected spawn point: " + randomIndex);
            transform.position = researcherSpawnPoints[randomIndex].transform.position;
            agent.Warp(transform.position); // This ensures NavMeshAgent aligns exactly with the spawn position
        }

        // Set animation speed
        animateLegs.speed = animationSpeed;
        animateBody.speed = animationSpeed;

        // Configure NavMesh Agent for 2D
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Update()
    {
        if (isDead) {
            // stop movement
            movement = Vector2.zero;
            transform.position += new Vector3(0, 0, 0.2f);
            transform.rotation = Quaternion.identity;
            // toggle GO's
            transform.GetComponent<CircleCollider2D>().enabled = false;
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
            transform.GetChild(2).gameObject.SetActive(true);
            // play animation
            animateDeath.Play("NPC_Scientist_Death");
            Destroy(gameObject, 5f);

            return;
        }


        // Calculate distance to the player
        float distanceToPlayer = Vector2.Distance(player.position, transform.position);

        // Calculate distance to the goal
        float distanceToGoal = Vector2.Distance(goal.transform.position, transform.position);

        // Check if the researcher should start or continue following the player
        if (!isFollowing && distanceToPlayer <= followDistance)
        {
            isFollowing = true; // Start following the player
        }

        if (isFollowing)
        {
            if (distanceToPlayer > stopDistance)
            {
                // Follow the player
                agent.SetDestination(player.position);

                // Rotate to face the player
                Vector3 moveDirection = agent.desiredVelocity;
                float rot_z = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0f, 0f, rot_z);

                // Play walking animation
                animateLegs.Play("NPC_Scientist_Walk");
                animateBody.Play("NPC_Scientist_Walk");
            }
            else
            {
                // Stop moving if too close to the player
                movement = Vector2.zero;
                agent.ResetPath();

                // Play idle animation
                animateLegs.Play("NPC_Scientist_Idle");
                animateBody.Play("NPC_Scientist_Idle");
            }

            // Check if the researcher is within the range to switch to the goal
            if (distanceToGoal <= goalSwitchDistance && goal != null)
            {
                // Stop following the player and switch to the goal
                isFollowing = false;
                agent.SetDestination(goal.transform.position);

                // Play walking animation while moving to the goal
                animateLegs.Play("NPC_Scientist_Walk");
                animateBody.Play("NPC_Scientist_Walk");
            }
        }
        else
        {
            movement = Vector2.zero;
            agent.ResetPath();

            // Play idle animation
            animateLegs.Play("NPC_Scientist_Idle");
            animateBody.Play("NPC_Scientist_Idle");

            // // If not following the player, continue moving towards the goal
            // if (distanceToGoal > stopDistance)
            // {
            //     // Move to the goal
            //     agent.SetDestination(goal.transform.position);

            //     // Play walking animation
            //     animateLegs.Play("NPC_Scientist_Walk");
            //     animateBody.Play("NPC_Scientist_Walk");
            // }
            // else
            // {
            //     // Stop moving if too close to the goal
            //     movement = Vector2.zero;
            //     agent.ResetPath();

            //     // Play idle animation
            //     animateLegs.Play("NPC_Scientist_Idle");
            //     animateBody.Play("NPC_Scientist_Idle");
            // }
        }
    }

    void FixedUpdate()
    {
        // Move the NPC using Rigidbody2D (optional, depending on your movement system)
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("EnemyAttack")) {
            // Debug.Log("Player Hit by Enemy");
            isDead = true;
        }
        if (other.CompareTag("Fireball")) {
            // Debug.Log("Player Hit by Fireball");
            isDead = true;
            Destroy(other.gameObject);
        }
    }
}
