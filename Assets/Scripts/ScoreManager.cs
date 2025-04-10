using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI gameOverScoreText;

    private int currentScore = 0;
    private int highScore = 0;
    private const string HIGH_SCORE_KEY = "HighScore";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        LoadHighScore();
    }

    private void Start()
    {
        if (scoreText == null || highScoreText == null)
        {
            Debug.LogError("Score text references not set in ScoreManager!");
            enabled = false;
            return;
        }

        UpdateScoreDisplay();
    }

    public void AddScore()
    {
        if (GameManager.Instance.IsGameOver) return;

        currentScore++;
        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt(HIGH_SCORE_KEY, highScore);
            PlayerPrefs.Save();
        }
        UpdateScoreDisplay();
    }

    public void ResetScore()
    {
        currentScore = 0;
        UpdateScoreDisplay();
    }

    private void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
    }

    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = currentScore.ToString();
        }
        
        if (highScoreText != null)
        {
            highScoreText.text = "High Score: " + highScore.ToString();
        }

        if (gameOverScoreText != null && GameManager.Instance.IsGameOver)
        {
            gameOverScoreText.text = "Score: " + currentScore.ToString() + "\nBest: " + highScore.ToString();
        }
    }

    public int GetCurrentScore()
    {
        return currentScore;
    }

    public int GetHighScore()
    {
        return highScore;
    }
}
