using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class PlayButton : MonoBehaviour
{
    [Header("UI Components")]
    public Button playButton;
    public TextMeshProUGUI buttonText;
    public GameObject loadingPanel;
    public Slider loadingSlider;
    public TextMeshProUGUI loadingText;
    
    [Header("Button States")]
    public Color enabledColor = Color.green;
    public Color disabledColor = Color.gray;
    public string enabledText = "PLAY";
    public string disabledText = "Select Song & Difficulty";
    public string loadingButtonText = "Loading...";
    
    private bool isLoading = false;

    void Start()
    {
        InitializeButton();
        SetupEventListeners();
    }
    
    void InitializeButton()
    {
        if (playButton == null)
        {
            Debug.LogError("❌ [PlayButton] El botón 'playButton' no está asignado en el Inspector.");
            return;
        }

        playButton.onClick.AddListener(PlayGame);
        
        // Hide loading panel initially
        if (loadingPanel != null)
            loadingPanel.SetActive(false);
    }
    
    void SetupEventListeners()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPlayButtonStateChanged += UpdateState;
            UpdateState();
        }
        else
        {
            Debug.LogError("❌ [PlayButton] GameManager.Instance es null.");
        }
    }

    void UpdateState()
    {
        if (isLoading) return; // Don't update state while loading
        
        if (GameManager.Instance == null)
        {
            Debug.LogError("❌ [PlayButton] GameManager.Instance is null in UpdateState");
            return;
        }
        
        bool canPlay = GameManager.Instance.songSelected && GameManager.Instance.difficultySelected;
        
        // Update button interactability
        if (playButton != null)
            playButton.interactable = canPlay;
        
        // Update visual appearance
        UpdateButtonVisuals(canPlay);
        
        Debug.Log($"🎮 Play button state updated - Can play: {canPlay}");
    }
    
    void UpdateButtonVisuals(bool canPlay)
    {
        if (playButton == null) return;
        
        // Update button color
        ColorBlock colors = playButton.colors;
        colors.normalColor = canPlay ? enabledColor : disabledColor;
        colors.highlightedColor = canPlay ? Color.Lerp(enabledColor, Color.white, 0.2f) : disabledColor;
        playButton.colors = colors;
        
        // Update button text
        if (buttonText != null)
        {
            buttonText.text = canPlay ? enabledText : disabledText;
            buttonText.color = canPlay ? Color.white : Color.gray;
        }
    }

    void PlayGame()
    {
        if (isLoading) return;
        
        if (GameManager.Instance == null)
        {
            Debug.LogError("❌ [PlayButton] GameManager.Instance is null");
            ShowErrorMessage("Game Manager not found!");
            return;
        }
        
        if (!GameManager.Instance.songSelected || !GameManager.Instance.difficultySelected)
        {
            Debug.LogWarning("⚠️ [PlayButton] Falta seleccionar canción o dificultad.");
            ShowErrorMessage("Please select a song and difficulty first!");
            return;
        }

        Debug.Log($"🚀 [PlayButton] Iniciando juego - Canción: {GameManager.Instance.selectedSongPath}, Dificultad: {GameManager.Instance.selectedDifficulty}");
        
        // Start loading coroutine
        StartCoroutine(LoadGameplayScene());
    }
    
    IEnumerator LoadGameplayScene()
    {
        isLoading = true;
        UpdateLoadingState(true);
        
        // Show loading panel
        if (loadingPanel != null)
            loadingPanel.SetActive(true);
        
        // Simulate loading progress (you can replace this with actual loading logic)
        float progress = 0f;
        while (progress < 1f)
        {
            progress += Time.deltaTime * 2f; // Adjust speed as needed
            UpdateLoadingProgress(progress, "Loading gameplay scene...");
            yield return null;
        }
        
        // Additional delay for smooth transition
        yield return new WaitForSeconds(0.5f);
        
        // Load the scene
        try
        {
            // Use GameManager's StartGame method if available
            if (GameManager.Instance != null)
            {
                GameManager.Instance.StartGame();
            }
            else
            {
                SceneManager.LoadScene("Gameplay");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Error loading Gameplay scene: {e.Message}");
            ShowErrorMessage("Failed to load gameplay scene!");
            
            // Reset loading state
            UpdateLoadingState(false);
            if (loadingPanel != null)
                loadingPanel.SetActive(false);
            isLoading = false;
        }
    }
    
    void UpdateLoadingState(bool loading)
    {
        if (buttonText != null)
        {
            buttonText.text = loading ? loadingButtonText : enabledText;
        }
        
        if (playButton != null)
            playButton.interactable = !loading;
    }
    
    void UpdateLoadingProgress(float progress, string message)
    {
        if (loadingSlider != null)
            loadingSlider.value = progress;
            
        if (loadingText != null)
            loadingText.text = message;
    }
    
    void ShowErrorMessage(string message)
    {
        Debug.LogWarning($"⚠️ {message}");
        
        // Show temporary error message on button
        if (buttonText != null)
        {
            StartCoroutine(ShowTemporaryMessage(message));
        }
    }
    
    IEnumerator ShowTemporaryMessage(string message)
    {
        if (buttonText != null)
        {
            string originalText = buttonText.text;
            Color originalColor = buttonText.color;
            
            buttonText.text = message;
            buttonText.color = Color.red;
            
            yield return new WaitForSeconds(2f);
            
            buttonText.text = originalText;
            buttonText.color = originalColor;
        }
    }
    
    // Public method to manually trigger state update
    public void ForceUpdateState()
    {
        UpdateState();
    }
    
    // Public method to check if ready to play
    public bool IsReadyToPlay()
    {
        return GameManager.Instance != null && 
               GameManager.Instance.songSelected && 
               GameManager.Instance.difficultySelected && 
               !isLoading;
    }
    
    // Public method to set loading state externally
    public void SetLoadingState(bool loading)
    {
        isLoading = loading;
        UpdateLoadingState(loading);
        
        if (loadingPanel != null)
            loadingPanel.SetActive(loading);
    }
    
    // Public method to show custom error
    public void ShowCustomError(string errorMessage)
    {
        ShowErrorMessage(errorMessage);
    }
    
    void OnEnable()
    {
        // Re-subscribe to events when enabled
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPlayButtonStateChanged += UpdateState;
            UpdateState();
        }
    }
    
    void OnDisable()
    {
        // Unsubscribe from events when disabled
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPlayButtonStateChanged -= UpdateState;
        }
    }
    
    void OnDestroy()
    {
        // Unsubscribe from events
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPlayButtonStateChanged -= UpdateState;
        }
    }
}
