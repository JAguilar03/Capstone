public class EnemyController : MonoBehaviour
{
    public int health;
    public float moveSpeed;
    
    void Start()
    {
        // Placeholder for enemy initialization
    }

    void Update()
    {
        MoveTowardsPlayer();
    }

    void MoveTowardsPlayer()
    {
        // Placeholder for enemy movement towards player
    }

    void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Placeholder for enemy death
        Destroy(gameObject);
    }
}
