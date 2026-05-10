using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private const string HighScoreKey = "HighScore";

    [Header("Config")]
    [SerializeField] private GameConfig config;

    [Header("HUD UI")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI gameOverHighScoreText;

    [Header("Scenes")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    public float ScrollSpeed { get; private set; }
    public float Distance { get; private set; }
    public bool IsGameOver { get; private set; }

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
    }

    void Update()
    {
        if (IsGameOver) return;
        if (config == null) return;

        ScrollSpeed = Mathf.Min(
            ScrollSpeed + config.speedIncreaseRate * Time.deltaTime,
            config.maxSpeed
        );

        Distance += ScrollSpeed * Time.deltaTime;

        UpdateScoreText();
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
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void ResetHighScore()
    {
        _highScore = 0;
        PlayerPrefs.SetInt(HighScoreKey, _highScore);
        PlayerPrefs.Save();

        UpdateHighScoreText();
    }
}