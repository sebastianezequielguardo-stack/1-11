using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Manages UI elements during gameplay with clean, minimal feedback
/// </summary>
public class UIManagerNew : MonoBehaviour
{
    [Header("Score Display")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;
    
    [Header("Progress Display")]
    public Slider songProgressSlider;
    public TextMeshProUGUI timeRemainingText;
    
    [Header("Lane Effects")]
    public GameObject[] laneEffects; // Visual effects for each lane
    public Color hitEffectColor = Color.white;
    public float effectDuration = 0.2f;
    
    [Header("Pause Menu")]
    public GameObject pauseMenu;
    public Button resumeButton;
    public Button restartButton;
    public Button mainMenuButton;
    
    [Header("End Game UI")]
    public GameObject endGamePanel;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI finalAccuracyText;
    public TextMeshProUGUI finalComboText;
    public Button playAgainButton;
    public Button backToMenuButton;
    
    private GameplayManager gameplayManager;
    private ScoreManager scoreManager;
    private Coroutine[] laneEffectCoroutines = new Coroutine[5];
    
    void Start()
    {
        InitializeUI();
        SetupEventListeners();
    }
    
    void InitializeUI()
    {
        gameplayManager = GameplayManager.Instance;
        scoreManager = FindFirstObjectByType<ScoreManager>();
        
        // Hide menus initially
        if (pauseMenu != null) pauseMenu.SetActive(false);
        if (endGamePanel != null) endGamePanel.SetActive(false);
        
        // Initialize progress slider
        if (songProgressSlider != null)
        {
            songProgressSlider.value = 0f;
            songProgressSlider.minValue = 0f;
            songProgressSlider.maxValue = 1f;
        }
        
        // Initialize lane effect coroutines array
        laneEffectCoroutines = new Coroutine[5];
    }
    
    void SetupEventListeners()
    {
        // Subscribe to gameplay events
        if (gameplayManager != null)
        {
            gameplayManager.OnNoteHit += OnNoteHit;
            gameplayManager.OnNoteMissed += OnNoteMissed;
            gameplayManager.OnSongFinished += OnSongFinished;
        }
        
        // Setup button listeners
        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);
            
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);
            
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(ReturnToMainMenu);
            
        if (playAgainButton != null)
            playAgainButton.onClick.AddListener(RestartGame);
            
        if (backToMenuButton != null)
            backToMenuButton.onClick.AddListener(ReturnToMainMenu);
    }
    
    void Update()
    {
        UpdateScoreDisplay();
        UpdateProgressDisplay();
        HandlePauseInput();
    }
    
    void UpdateScoreDisplay()
    {
        if (scoreManager == null) return;
        
        // Update score
        if (scoreText != null)
        {
            scoreText.text = $"Score: {scoreManager.score:N0}";
        }
        
        // Update combo
        if (comboText != null)
        {
            if (scoreManager.currentCombo > 0)
            {
                comboText.text = $"Combo: {scoreManager.currentCombo}x";
                comboText.gameObject.SetActive(true);
            }
            else
            {
                comboText.gameObject.SetActive(false);
            }
        }
    }
    
    void UpdateProgressDisplay()
    {
        if (gameplayManager == null) return;
        
        float currentTime = gameplayManager.GetSongTime();
        float totalTime = gameplayManager.songLength;
        
        // Update progress slider
        if (songProgressSlider != null && totalTime > 0)
        {
            songProgressSlider.value = currentTime / totalTime;
        }
        
        // Update time remaining
        if (timeRemainingText != null && totalTime > 0)
        {
            float timeRemaining = totalTime - currentTime;
            int minutes = Mathf.FloorToInt(timeRemaining / 60f);
            int seconds = Mathf.FloorToInt(timeRemaining % 60f);
            timeRemainingText.text = $"{minutes:00}:{seconds:00}";
        }
    }
    
    void HandlePauseInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && gameplayManager != null)
        {
            if (gameplayManager.isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
    
    void OnNoteHit(NoteData noteData, HitAccuracy accuracy)
    {
        // Only show lane effect, no text or sound feedback
        TriggerLaneEffect(noteData.laneIndex);
    }
    
    void OnNoteMissed(NoteData noteData)
    {
        // No feedback for misses - clean gameplay
    }
    
    void OnSongFinished()
    {
        ShowEndGamePanel();
    }
    
    void TriggerLaneEffect(int laneIndex)
    {
        if (laneIndex < 0 || laneIndex >= laneEffects.Length) return;
        if (laneEffects[laneIndex] == null) return;
        
        // Stop existing effect for this lane
        if (laneEffectCoroutines[laneIndex] != null)
        {
            StopCoroutine(laneEffectCoroutines[laneIndex]);
        }
        
        // Start new effect
        laneEffectCoroutines[laneIndex] = StartCoroutine(LaneEffectCoroutine(laneIndex));
    }
    
    IEnumerator LaneEffectCoroutine(int laneIndex)
    {
        GameObject effect = laneEffects[laneIndex];
        Image effectImage = effect.GetComponent<Image>();
        
        if (effectImage != null)
        {
            // Flash effect
            effectImage.color = hitEffectColor;
            effect.SetActive(true);
            
            yield return new WaitForSeconds(effectDuration);
            
            effect.SetActive(false);
        }
        
        laneEffectCoroutines[laneIndex] = null;
    }
    
    public void PauseGame()
    {
        if (gameplayManager != null)
        {
            gameplayManager.PauseGame();
        }
        
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(true);
        }
    }
    
    public void ResumeGame()
    {
        if (gameplayManager != null)
        {
            gameplayManager.ResumeGame();
        }
        
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
    }
    
    public void RestartGame()
    {
        // Reload current scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
    
    public void ReturnToMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
    
    void ShowEndGamePanel()
    {
        if (endGamePanel == null || scoreManager == null) return;
        
        endGamePanel.SetActive(true);
        
        // Display final stats
        if (finalScoreText != null)
            finalScoreText.text = $"Final Score: {scoreManager.score:N0}";
            
        if (finalAccuracyText != null)
        {
            float accuracy = scoreManager.totalNotes > 0 ? 
                (float)scoreManager.hitNotes / scoreManager.totalNotes * 100f : 0f;
            finalAccuracyText.text = $"Accuracy: {accuracy:F1}%";
        }
        
        if (finalComboText != null)
            finalComboText.text = $"Max Combo: {scoreManager.maxCombo}";
    }
    
    void OnDestroy()
    {
        // Unsubscribe from events
        if (gameplayManager != null)
        {
            gameplayManager.OnNoteHit -= OnNoteHit;
            gameplayManager.OnNoteMissed -= OnNoteMissed;
            gameplayManager.OnSongFinished -= OnSongFinished;
        }
        
        // Stop all lane effect coroutines
        for (int i = 0; i < laneEffectCoroutines.Length; i++)
        {
            if (laneEffectCoroutines[i] != null)
            {
                StopCoroutine(laneEffectCoroutines[i]);
            }
        }
    }
}
