using UnityEngine;
using UnityEngine.SceneManagement;

// This script manages the goal object in the game, including its availability and color. The goal is initially 
// unavailable and marked red. Once all enemies are defeated, the goal becomes available and turns green. When the 
// player enters the goal area and a researcher is nearby, the player earns score points. The script also handles 
// scene transitions based on the current level, loading the next level or restarting the cycle.

public class GoalController : MonoBehaviour
{
    private bool isAvailable = false;
    private SpriteRenderer spriteRenderer;
    private ScoreManager scoreManager;
    private SavePlayer saveHandler;
    public float researcherCheckRadius = 3f;


    // This sets up the goal object, sets its initial color to red, and initializes the score manager.
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        scoreManager = FindObjectOfType<ScoreManager>();
        saveHandler = GetComponent<SavePlayer>();
        // Start with red color
        spriteRenderer.color = Color.red;
        // MakeGoalAvailable();
    }

    void Update()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        
        if (enemies.Length == 0)
        {
            MakeGoalAvailable();
        } 
        else 
        {
            isAvailable = false;
            spriteRenderer.color = Color.red;
        }
    }

    // This method makes the goal available by changing its color to green.
    void MakeGoalAvailable()
    {
        isAvailable = true;
        // Change to green color
        spriteRenderer.color = Color.green;
    }

    // This method handles the player entering the goal area. If a researcher is nearby, the player earns score points.
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isAvailable)
        {
            Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(transform.position, researcherCheckRadius);
            foreach (Collider2D collider in nearbyColliders)
            {
                if (collider.GetComponent<Researcher>() != null)
                {
                    scoreManager.AddScore(1000);
                    break;
                }
            }

            string currentScene = SceneManager.GetActiveScene().name;
            switch (currentScene)
            {
                case "Level1":
                    SceneManager.LoadScene("Level2");
                    break;
                case "Level2":
                    SceneManager.LoadScene("Level3");
                    break;
                case "Level3":
                    SceneManager.LoadScene("BSP level");
                    break;
                case "BSP level":
                    SceneManager.LoadScene("BSP level");
                    break;

            }
        }
    }
}
