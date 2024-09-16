public class GameManager : MonoBehaviour
{
    public int currentRound;
    public int playerScore;
    
    void Start()
    {
        StartGame();
    }

    void StartGame()
    {
        currentRound = 1;
        playerScore = 0;
        SpawnEnemies();
    }

    void SpawnEnemies()
    {
        // Placeholder for spawning enemies for the current round
    }

    void Update()
    {
        // Placeholder for managing game state, including transitions between rounds
    }

    void EndRound()
    {
        currentRound++;
        SpawnEnemies();
    }

    void GameOver()
    {
        // Placeholder for ending the game
    }
}
