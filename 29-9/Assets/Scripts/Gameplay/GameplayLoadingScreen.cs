using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Pantalla de carga con countdown para el gameplay
/// Permite que los videos carguen completamente antes de iniciar el juego
/// </summary>
public class GameplayLoadingScreen : MonoBehaviour
{
    [Header("Loading Settings")]
    public float countdownDuration = 3f;
    public bool waitForVideoLoad = true;
    public float maxVideoWaitTime = 8f;
    
    [Header("Visual Settings")]
    public Color backgroundColor = Color.black;
    public Color textColor = Color.white;
    public float textSize = 120f;
    
    [Header("Loading Messages")]
    public string[] loadingMessages = {
        "Cargando video...",
        "Preparando gameplay...",
        "Casi listo..."
    };
    
    // UI Components
    private GameObject loadingCanvas;
    private Image backgroundImage;
    private TextMeshProUGUI countdownText;
    
    // System References
    private GameplayManager gameplayManager;
    private BackgroundVideoSystem videoSystem;
    
    // State
    private bool isLoading = true;
    private bool videoLoaded = false;
    
    void Start()
    {
        StartCoroutine(InitializeLoadingScreen());
    }
    
    IEnumerator InitializeLoadingScreen()
    {
        // Find system references
        gameplayManager = GameplayManager.Instance;
        videoSystem = FindFirstObjectByType<BackgroundVideoSystem>();
        
        // Create loading screen
        CreateLoadingScreen();
        
        // Pause everything completely during loading
        PauseEverythingForLoading();
        
        // Start loading process
        yield return StartCoroutine(LoadingProcess());
    }
    
    void CreateLoadingScreen()
    {
        // Create Canvas
        GameObject canvasObj = new GameObject("LoadingCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10000; // Highest priority
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        canvasObj.AddComponent<GraphicRaycaster>();
        loadingCanvas = canvasObj;
        
        // Create Background
        GameObject backgroundObj = new GameObject("Background");
        backgroundObj.transform.SetParent(loadingCanvas.transform, false);
        
        RectTransform bgRect = backgroundObj.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        
        backgroundImage = backgroundObj.AddComponent<Image>();
        backgroundImage.color = backgroundColor;
        
        // Create Countdown Text (solo contador, sin texto adicional)
        GameObject countdownObj = new GameObject("CountdownText");
        countdownObj.transform.SetParent(loadingCanvas.transform, false);
        
        RectTransform countdownRect = countdownObj.AddComponent<RectTransform>();
        countdownRect.anchorMin = new Vector2(0.5f, 0.5f);
        countdownRect.anchorMax = new Vector2(0.5f, 0.5f);
        countdownRect.sizeDelta = new Vector2(400f, 200f);
        countdownRect.anchoredPosition = Vector2.zero;
        
        countdownText = countdownObj.AddComponent<TextMeshProUGUI>();
        countdownText.text = "";
        countdownText.fontSize = textSize;
        countdownText.color = textColor;
        countdownText.alignment = TextAlignmentOptions.Center;
        countdownText.fontStyle = FontStyles.Bold;
        
        // Strong outline for visibility
        countdownText.outlineWidth = 0.3f;
        countdownText.outlineColor = Color.black;
        
        Debug.Log("🎬 Loading screen created");
    }
    
    IEnumerator LoadingProcess()
    {
        Debug.Log("🎬 Starting gameplay loading process...");
        
        // Wait for video to load (silently, without messages)
        if (waitForVideoLoad && videoSystem != null)
        {
            yield return StartCoroutine(WaitForVideoLoad());
        }
        
        // Small pause before countdown
        yield return new WaitForSecondsRealtime(0.5f);
        
        // Start countdown directly
        yield return StartCoroutine(StartCountdown());
        
        // Start gameplay
        StartGameplay();
    }
    
    IEnumerator WaitForVideoLoad()
    {
        float waitTime = 0f;
        
        while (waitTime < maxVideoWaitTime && !videoLoaded)
        {
            // Check if video is loaded
            if (videoSystem != null && videoSystem.IsVideoLoaded())
            {
                videoLoaded = true;
                Debug.Log("🎬 Video loaded successfully");
                break;
            }
            
            waitTime += Time.unscaledDeltaTime;
            yield return null;
        }
        
        if (!videoLoaded)
        {
            Debug.LogWarning("🎬 Video loading timeout - continuing without video");
        }
    }
    
    IEnumerator StartCountdown()
    {
        Debug.Log("🔢 Starting gameplay countdown...");
        
        // Countdown from 3 to 1 (solo números)
        for (int i = 3; i >= 1; i--)
        {
            if (countdownText != null)
            {
                countdownText.text = i.ToString();
                
                // Scale animation
                yield return StartCoroutine(AnimateCountdownNumber());
                
                yield return new WaitForSecondsRealtime(0.8f);
            }
            else
            {
                // Si no hay texto, solo esperar
                yield return new WaitForSecondsRealtime(1f);
            }
        }
        
        // Clear countdown text
        if (countdownText != null)
        {
            countdownText.text = "";
        }
        yield return new WaitForSecondsRealtime(0.2f);
    }
    
    IEnumerator AnimateCountdownNumber()
    {
        // Verificar si el objeto aún existe
        if (countdownText == null || countdownText.gameObject == null)
        {
            yield break;
        }
        
        Vector3 originalScale = countdownText.transform.localScale;
        Vector3 bigScale = originalScale * 1.3f;
        
        float duration = 0.2f;
        float elapsed = 0f;
        
        // Scale up
        while (elapsed < duration && countdownText != null)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            
            if (countdownText != null && countdownText.transform != null)
            {
                countdownText.transform.localScale = Vector3.Lerp(originalScale, bigScale, t);
            }
            yield return null;
        }
        
        elapsed = 0f;
        
        // Scale down
        while (elapsed < duration && countdownText != null)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            
            if (countdownText != null && countdownText.transform != null)
            {
                countdownText.transform.localScale = Vector3.Lerp(bigScale, originalScale, t);
            }
            yield return null;
        }
        
        if (countdownText != null && countdownText.transform != null)
        {
            countdownText.transform.localScale = originalScale;
        }
    }
    
    void StartGameplay()
    {
        Debug.Log("🎮 Starting gameplay now!");
        
        // Resume everything first
        ResumeEverythingAfterLoading();
        
        // Start gameplay
        if (gameplayManager != null)
        {
            gameplayManager.ForceStartGameplay();
        }
        
        // Start video if loaded
        if (videoSystem != null && videoSystem.IsVideoLoaded())
        {
            videoSystem.PlayVideo();
        }
        
        // Fade out loading screen
        StartCoroutine(FadeOutLoadingScreen());
    }
    
    IEnumerator FadeOutLoadingScreen()
    {
        float fadeDuration = 0.5f;
        float elapsed = 0f;
        
        Color originalBgColor = backgroundImage != null ? backgroundImage.color : Color.black;
        Color originalCountdownColor = countdownText != null ? countdownText.color : Color.white;
        
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            
            if (backgroundImage != null)
            {
                backgroundImage.color = new Color(originalBgColor.r, originalBgColor.g, originalBgColor.b, alpha);
            }
            
            if (countdownText != null)
            {
                countdownText.color = new Color(originalCountdownColor.r, originalCountdownColor.g, originalCountdownColor.b, alpha);
            }
            
            yield return null;
        }
        
        // Destroy loading screen
        Destroy(loadingCanvas);
        Destroy(gameObject);
        
        Debug.Log("✅ Loading screen completed and removed");
    }
    
    /// <summary>
    /// Pausa completamente todo durante la carga
    /// </summary>
    void PauseEverythingForLoading()
    {
        Debug.Log("⏸️ Pausing everything for loading screen");
        
        // Prevent gameplay from starting
        if (gameplayManager != null)
        {
            gameplayManager.isGameActive = false;
        }
        
        // DON'T pause Time.timeScale to allow coroutines to work
        // Time.timeScale = 0f; // REMOVED
        
        // Pause all audio
        AudioListener.pause = true;
        AudioSource[] audioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        foreach (var source in audioSources)
        {
            if (source != null && source.isPlaying)
            {
                source.Pause();
            }
        }
        
        // Disable note spawners
        NoteSpawner[] spawners = FindObjectsByType<NoteSpawner>(FindObjectsSortMode.None);
        foreach (var spawner in spawners)
        {
            if (spawner != null)
            {
                spawner.enabled = false;
            }
        }
        
        // Disable note movement
        Note[] notes = FindObjectsByType<Note>(FindObjectsSortMode.None);
        foreach (var note in notes)
        {
            if (note != null)
            {
                note.enabled = false;
            }
        }
    }
    
    /// <summary>
    /// Reanuda todo después de la carga
    /// </summary>
    void ResumeEverythingAfterLoading()
    {
        Debug.Log("▶️ Resuming everything after loading screen");
        
        // Ensure time scale is normal (should already be 1f)
        Time.timeScale = 1f;
        
        // Resume audio
        AudioListener.pause = false;
        AudioSource[] audioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        foreach (var source in audioSources)
        {
            if (source != null)
            {
                source.UnPause();
            }
        }
        
        // Enable note spawners
        NoteSpawner[] spawners = FindObjectsByType<NoteSpawner>(FindObjectsSortMode.None);
        foreach (var spawner in spawners)
        {
            if (spawner != null)
            {
                spawner.enabled = true;
            }
        }
        
        // Enable note movement
        Note[] notes = FindObjectsByType<Note>(FindObjectsSortMode.None);
        foreach (var note in notes)
        {
            if (note != null)
            {
                note.enabled = true;
            }
        }
    }
    
    /// <summary>
    /// Skip loading screen (for testing)
    /// </summary>
    [ContextMenu("Skip Loading")]
    public void SkipLoading()
    {
        StopAllCoroutines();
        StartGameplay();
    }
    
    /// <summary>
    /// Force start gameplay (emergency)
    /// </summary>
    public void ForceStartGameplay()
    {
        Debug.Log("🚨 Force starting gameplay");
        StopAllCoroutines();
        
        // Resume everything first
        ResumeEverythingAfterLoading();
        
        if (gameplayManager != null)
        {
            gameplayManager.ForceStartGameplay();
        }
        
        if (loadingCanvas != null)
        {
            Destroy(loadingCanvas);
        }
        
        Destroy(gameObject);
    }
    
    void Update()
    {
        // Emergency skip with Space key
        if (Input.GetKeyDown(KeyCode.Space) && isLoading)
        {
            Debug.Log("⏭️ Loading skipped by user");
            SkipLoading();
        }
    }
}
