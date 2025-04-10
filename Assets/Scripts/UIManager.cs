using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject gameplayPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject pausePanel;

    [Header("Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button exitButton;

    [Header("Text Elements")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI tapToPlayText;
    [SerializeField] private TextMeshProUGUI gameOverText;

    private bool isPaused = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        SetupButtonListeners();
    }

    private void Start()
    {
        // Initialize UI state
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (gameplayPanel != null) gameplayPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    private void SetupButtonListeners()
    {
        if (startButton != null)
            startButton.onClick.AddListener(() => GameManager.Instance.StartGame());

        if (restartButton != null)
            restartButton.onClick.AddListener(() => GameManager.Instance.RestartGame());

        if (menuButton != null)
            menuButton.onClick.AddListener(() => GameManager.Instance.LoadMainMenu());

        if (resumeButton != null)
            resumeButton.onClick.AddListener(TogglePause);

        if (pauseButton != null)
            pauseButton.onClick.AddListener(TogglePause);

        if (exitButton != null)
            exitButton.onClick.AddListener(() => GameManager.Instance.ExitGame());
    }

    public void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        
        if (gameplayPanel != null)
        {
            gameplayPanel.SetActive(false);
        }

        // Animate game over text
        if (gameOverText != null)
        {
            gameOverText.transform.localScale = Vector3.zero;
            LeanTween.scale(gameOverText.gameObject, Vector3.one, 0.5f)
                .setEaseOutBack();
        }
    }

    public void ShowGameplay()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (gameplayPanel != null) gameplayPanel.SetActive(true);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    public void ShowMainMenu()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (gameplayPanel != null) gameplayPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    private void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;
        
        if (pausePanel != null)
        {
            pausePanel.SetActive(isPaused);
        }
    }

    private void OnDestroy()
    {
        // Ensure time scale is reset when scene changes
        Time.timeScale = 1;
    }

    // Animation helper methods
    public void PulseText(TextMeshProUGUI text)
    {
        if (text != null)
        {
            LeanTween.scale(text.gameObject, Vector3.one * 1.2f, 0.5f)
                .setEasePunch()
                .setLoopPingPong();
        }
    }

    public void FadeIn(CanvasGroup canvasGroup, float duration = 0.5f)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0;
            LeanTween.alphaCanvas(canvasGroup, 1, duration);
        }
    }

    public void FadeOut(CanvasGroup canvasGroup, float duration = 0.5f)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1;
            LeanTween.alphaCanvas(canvasGroup, 0, duration);
        }
    }
}
