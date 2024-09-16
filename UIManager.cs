public class UIManager : MonoBehaviour
{
    public Text scoreText;
    public Text roundText;

    void UpdateUI()
    {
        scoreText.text = "Score: " + playerScore;
        roundText.text = "Round: " + currentRound;
    }
}
