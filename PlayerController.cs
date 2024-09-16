public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public GameObject bulletPrefab;
    
    void Update()
    {
        MovePlayer();
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    void MovePlayer()
    {
        // Placeholder for player movement using WASD or arrow keys
    }

    void Shoot()
    {
        // Placeholder for shooting mechanic
    }

    void TakeDamage(int damage)
    {
        // Placeholder for player taking damage
    }
}
