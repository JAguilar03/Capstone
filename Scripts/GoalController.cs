using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalController : MonoBehaviour
{
    private bool isAvailable = false;
    private SpriteRenderer spriteRenderer;
    private ScoreManager scoreManager;
    public float researcherCheckRadius = 3f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        scoreManager = FindObjectOfType<ScoreManager>();
        // Start with red color
        spriteRenderer.color = Color.red;
    }

    void Update()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        
        if (enemies.Length == 0 && !isAvailable)
        {
            MakeGoalAvailable();
        }
    }

    void MakeGoalAvailable()
    {
        isAvailable = true;
        // Change to green color
        spriteRenderer.color = Color.green;
    }

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
                    SceneManager.LoadScene("Level1");
                    break;
            }
        }
    }
}
