using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private const string HighScoreKey = "HighScore";

    [Header("Config")]
    [SerializeField] private GameConfig config;

    [Header("HUD UI")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI coinText;

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI gameOverHighScoreText;

    public float ScrollSpeed { get; private set; }
    public float Distance { get; private set; }
    public bool IsGameOver { get; private set; }
    public int Coins { get; private set; }

    private int _highScore;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        IsGameOver = false;
        Time.timeScale = 1f;

        _highScore = PlayerPrefs.GetInt(HighScoreKey, 0);

        if (config != null)
        {
            ScrollSpeed = config.startSpeed;
        }
        else
        {
            Debug.LogError("GameManager: GameConfig is missing.");
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        UpdateScoreText();
        UpdateHighScoreText();
        UpdateCoinText();
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            RestartGame();
        }

        if (IsGameOver) return;
        if (config == null) return;

        ScrollSpeed = Mathf.Min(
            ScrollSpeed + config.speedIncreaseRate * Time.deltaTime,
            config.maxSpeed
        );

        Distance += ScrollSpeed * Time.deltaTime;

        UpdateScoreText();
    }

    public void AddCoin()
    {
        if (IsGameOver) return;

        Coins++;
        UpdateCoinText();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + Mathf.FloorToInt(Distance);
        }
    }

    private void UpdateHighScoreText()
    {
        if (highScoreText != null)
        {
            highScoreText.text = "Best: " + _highScore;
        }

        if (gameOverHighScoreText != null)
        {
            gameOverHighScoreText.text = "Best: " + _highScore;
        }
    }

    private void UpdateCoinText()
    {
        if (coinText != null)
        {
            coinText.text = "Coins: " + Coins;
        }
    }

    public void GameOver()
    {
        if (IsGameOver) return;

        IsGameOver = true;
        ScrollSpeed = 0f;

        int finalScore = Mathf.FloorToInt(Distance);

        if (finalScore > _highScore)
        {
            _highScore = finalScore;
            PlayerPrefs.SetInt(HighScoreKey, _highScore);
            PlayerPrefs.Save();
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (finalScoreText != null)
        {
            finalScoreText.text = "Final Score: " + finalScore;
        }

        UpdateHighScoreText();
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;

        int previousSceneIndex = SceneManager.GetActiveScene().buildIndex - 1;

        if (previousSceneIndex >= 0)
        {
            SceneManager.LoadScene(previousSceneIndex);
        }
        else
        {
            Debug.LogError("GameManager: No previous scene exists in Build Settings.");
        }
    }

    public void ResetHighScore()
    {
        _highScore = 0;
        PlayerPrefs.SetInt(HighScoreKey, _highScore);
        PlayerPrefs.Save();

        UpdateHighScoreText();
    }
}