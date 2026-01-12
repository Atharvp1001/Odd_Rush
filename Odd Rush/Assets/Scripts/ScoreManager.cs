using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("Score Settings")]
    public int wrongClickPenalty = 20;

    [Header("Lives")]
    public int maxHearts = 3;

    [Header("UI - Gameplay")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI heartText;

    [Header("UI - Game Over")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverScoreText;
    public TextMeshProUGUI gameOverHighScoreText;

    [Header("Score Popups")]
    public GameObject plus100Text;
    public GameObject plus80Text;
    public GameObject plus60Text;
    public GameObject plus40Text;
    public GameObject plus20Text;
    public GameObject plus0Text;
    public GameObject minus20Text;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip correctClip;
    public AudioClip wrongClip;

    int totalScore;
    int highScore;
    int currentHearts;

    public bool isGameOver { get; private set; }

    const string HIGH_SCORE_KEY = "HIGH_SCORE";

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        highScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
        ResetScore();
    }

    // 🔄 RESET FOR RETRY
    public void ResetScore()
    {
        totalScore = 0;
        currentHearts = maxHearts;
        isGameOver = false;

        gameOverPanel.SetActive(false);
        DisableAllPopups();
        UpdateGameplayUI();
    }

    // ✅ CORRECT CLICK
    public void AddScore(int points)
    {
        totalScore += points;

        if (correctClip != null)
            audioSource.PlayOneShot(correctClip);

        ShowScorePopup(points);
        UpdateGameplayUI();
    }

    // ❌ WRONG CLICK
    public void ApplyWrongClickPenalty()
    {
        totalScore -= wrongClickPenalty;
        if (totalScore < 0) totalScore = 0;

        currentHearts--;

        if (wrongClip != null)
            audioSource.PlayOneShot(wrongClip);

        ShowScorePopup(-wrongClickPenalty);
        UpdateGameplayUI();

        if (currentHearts <= 0)
        {
            EndGame();
        }
    }

    // 🛑 GAME OVER
    void EndGame()
    {
        isGameOver = true;

        if (totalScore > highScore)
        {
            highScore = totalScore;
            PlayerPrefs.SetInt(HIGH_SCORE_KEY, highScore);
            PlayerPrefs.Save();
        }

        gameOverScoreText.text = totalScore.ToString("00");
        gameOverHighScoreText.text = highScore.ToString("00");

        gameOverPanel.SetActive(true);
    }

    // 🔢 UPDATE HUD
    void UpdateGameplayUI()
    {
        scoreText.text = "Score: " + totalScore;
        heartText.text = "Heart x " + currentHearts;
    }

    // 🎯 SCORE POPUP SYSTEM
    void ShowScorePopup(int value)
    {
        DisableAllPopups();

        switch (value)
        {
            case 100:
                plus100Text.SetActive(true);
                break;
            case 80:
                plus80Text.SetActive(true);
                break;
            case 60:
                plus60Text.SetActive(true);
                break;
            case 40:
                plus40Text.SetActive(true);
                break;
            case 20:
                plus20Text.SetActive(true);
                break;
            case 0:
                plus0Text.SetActive(true);
                break;
            case -20:
                minus20Text.SetActive(true);
                break;
        }
    }

    // 🚫 ENSURE ONLY ONE POPUP ACTIVE
    void DisableAllPopups()
    {
        plus100Text.SetActive(false);
        plus80Text.SetActive(false);
        plus60Text.SetActive(false);
        plus40Text.SetActive(false);
        plus20Text.SetActive(false);
        plus0Text.SetActive(false);
        minus20Text.SetActive(false);
    }
}
