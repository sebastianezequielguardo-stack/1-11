using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Configuración simple del menú de pausa - alternativa más directa
/// </summary>
public class SimplePauseSetup : MonoBehaviour
{
    [Header("Pause Settings")]
    public KeyCode pauseKey = KeyCode.Escape;
    public KeyCode altPauseKey = KeyCode.P;

    [Header("UI Settings")]
    public Color backgroundColor = new Color(0f, 0f, 0f, 0.8f);
    public string mainMenuSceneName = "MainMenu";

    [Header("Audio Control")]
    public bool forceStopAudio = true; // Forzar parada de audio si Pause() no funciona

    // UI Elements
    private GameObject pausePanel;
    private bool isPaused = false;

    // Audio tracking
    private AudioSource[] allAudioSources;
    private bool[] wasPlayingBeforePause;
    private float[] originalVolumes;

    void Start()
    {
        CreateSimplePauseMenu();
    }

    void Update()
    {
        // Check for pause input
        if (Input.GetKeyDown(pauseKey) || Input.GetKeyDown(altPauseKey))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    void CreateSimplePauseMenu()
    {
        // Create Canvas
        GameObject canvasObj = new GameObject("PauseCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        canvasObj.AddComponent<GraphicRaycaster>();

        // Create Pause Panel
        pausePanel = new GameObject("PausePanel");
        pausePanel.transform.SetParent(canvasObj.transform, false);

        RectTransform panelRect = pausePanel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        // Background
        Image background = pausePanel.AddComponent<Image>();
        background.color = backgroundColor;

        // Create Menu Container
        GameObject menuContainer = new GameObject("MenuContainer");
        menuContainer.transform.SetParent(pausePanel.transform, false);

        RectTransform menuRect = menuContainer.AddComponent<RectTransform>();
        menuRect.anchorMin = new Vector2(0.5f, 0.5f);
        menuRect.anchorMax = new Vector2(0.5f, 0.5f);
        menuRect.sizeDelta = new Vector2(400f, 500f);
        menuRect.anchoredPosition = Vector2.zero;

        // Create Title
        CreateTitle(menuContainer.transform);

        // Create Buttons
        CreateButton(menuContainer.transform, "CONTINUAR", new Vector2(0f, 100f), ResumeGame);
        CreateButton(menuContainer.transform, "REINICIAR", new Vector2(0f, 50f), RestartGame);
        CreateButton(menuContainer.transform, "MENÚ PRINCIPAL", new Vector2(0f, 0f), GoToMainMenu);
        CreateButton(menuContainer.transform, "SALIR", new Vector2(0f, -50f), ExitGame);

        // Initially hide
        pausePanel.SetActive(false);

        Debug.Log("✅ Simple Pause Menu created successfully!");
    }

    void CreateTitle(Transform parent)
    {
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(parent, false);

        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 1f);
        titleRect.anchorMax = new Vector2(0.5f, 1f);
        titleRect.sizeDelta = new Vector2(300f, 60f);
        titleRect.anchoredPosition = new Vector2(0f, -50f);

        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "PAUSA";
        titleText.fontSize = 36f;
        titleText.color = Color.white;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.fontStyle = FontStyles.Bold;
    }

    void CreateButton(Transform parent, string text, Vector2 position, System.Action onClick)
    {
        GameObject buttonObj = new GameObject($"Button_{text}");
        buttonObj.transform.SetParent(parent, false);

        RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
        buttonRect.sizeDelta = new Vector2(300f, 40f);
        buttonRect.anchoredPosition = position;

        // Button component
        Button button = buttonObj.AddComponent<Button>();
        button.onClick.AddListener(() => onClick?.Invoke());

        // Button background
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

        // Button colors
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        colors.highlightedColor = new Color(0.3f, 0.3f, 0.3f, 1f);
        colors.pressedColor = new Color(0.1f, 0.1f, 0.1f, 1f);
        button.colors = colors;

        // Button text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        TextMeshProUGUI buttonText = textObj.AddComponent<TextMeshProUGUI>();
        buttonText.text = text;
        buttonText.fontSize = 18f;
        buttonText.color = Color.white;
        buttonText.alignment = TextAlignmentOptions.Center;
    }

    public void PauseGame()
    {
        if (isPaused) return;

        isPaused = true;
        Time.timeScale = 0f;

        // Pause all audio sources with enhanced method
        PauseAllAudio();

        // Stop all note spawning
        StopAllNoteSpawning();

        // Disable all note movement
        DisableAllNoteMovement();

        // Disable gameplay scripts
        DisableGameplayScripts();

        pausePanel.SetActive(true);
        Debug.Log("Game Paused - All systems stopped");
    }

    public void ResumeGame()
    {
        if (!isPaused) return;

        // Start countdown before resuming
        StartCoroutine(ResumeCountdown());
    }

    System.Collections.IEnumerator ResumeCountdown()
    {
        // Hide pause menu but keep game paused
        pausePanel.SetActive(false);

        // Create simple countdown UI
        GameObject countdownObj = CreateSimpleCountdownUI();
        TextMeshProUGUI countdownText = countdownObj.GetComponentInChildren<TextMeshProUGUI>();

        Debug.Log("🔢 Starting resume countdown...");

        // Simple countdown from 3 to 1
        for (int i = 3; i >= 1; i--)
        {
            countdownText.text = i.ToString();
            Debug.Log($"Countdown: {i}");

            // Simple scale effect
            countdownText.transform.localScale = Vector3.one * 1.2f;
            yield return new WaitForSecondsRealtime(0.1f);
            countdownText.transform.localScale = Vector3.one;

            yield return new WaitForSecondsRealtime(0.9f);
        }

        // Small pause after 1 before resuming
        yield return new WaitForSecondsRealtime(0.2f);

        // Actually resume the game
        Debug.Log("🎮 Resuming game now...");
        ActuallyResumeGame();

        // Quick fade out
        float fadeTime = 0.3f;
        float elapsed = 0f;
        Color originalColor = countdownText.color;

        while (elapsed < fadeTime)
        {
            elapsed += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeTime);
            countdownText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        // Clean up
        Destroy(countdownObj);
        Debug.Log("✅ Resume countdown completed!");
    }

    void ActuallyResumeGame()
    {
        isPaused = false;

        // Re-enable gameplay scripts
        EnableGameplayScripts();

        // Re-enable note movement
        EnableAllNoteMovement();

        // Resume note spawning
        ResumeAllNoteSpawning();

        Time.timeScale = 1f;

        // Resume all audio sources with enhanced method
        ResumeAllAudio();

        Debug.Log("Game Resumed - All systems restored after countdown");
    }

    public void RestartGame()
    {
        // Restore everything before reloading - NO COUNTDOWN for restart
        Time.timeScale = 1f;
        AudioListener.pause = false;

        // Force resume all audio before scene reload
        AudioSource[] sources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        foreach (var source in sources)
        {
            if (source != null)
            {
                source.volume = 1f; // Reset volume
                source.UnPause();
            }
        }

        Debug.Log("🔄 Restarting game immediately (no countdown)");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void ExitGame()
    {
        Time.timeScale = 1f;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    // Pause system methods
    void StopAllNoteSpawning()
    {
        // Stop NoteSpawner2D
        NoteSpawner2D[] noteSpawners2D = FindObjectsByType<NoteSpawner2D>(FindObjectsSortMode.None);
        foreach (var spawner in noteSpawners2D)
        {
            spawner.CancelInvoke();
            spawner.enabled = false;
        }

        // Stop original NoteSpawner
        NoteSpawner[] noteSpawners = FindObjectsByType<NoteSpawner>(FindObjectsSortMode.None);
        foreach (var spawner in noteSpawners)
        {
            spawner.enabled = false;
        }

        Debug.Log("All note spawning stopped");
    }

    void ResumeAllNoteSpawning()
    {
        // Resume NoteSpawner2D
        NoteSpawner2D[] noteSpawners2D = FindObjectsByType<NoteSpawner2D>(FindObjectsSortMode.None);
        foreach (var spawner in noteSpawners2D)
        {
            spawner.enabled = true;
            if (spawner.autoSpawn)
            {
                float interval = spawner.spawnInterval;
                spawner.InvokeRepeating(nameof(spawner.SpawnRandomNote), 0f, interval);
            }
        }

        // Resume original NoteSpawner
        NoteSpawner[] noteSpawners = FindObjectsByType<NoteSpawner>(FindObjectsSortMode.None);
        foreach (var spawner in noteSpawners)
        {
            spawner.enabled = true;
        }

        Debug.Log("All note spawning resumed");
    }

    void DisableAllNoteMovement()
    {
        // Disable all Note scripts
        Note[] notes = FindObjectsByType<Note>(FindObjectsSortMode.None);
        foreach (var note in notes)
        {
            note.enabled = false;
        }

        // Disable all FallingNote2D scripts
        FallingNote2D[] fallingNotes = FindObjectsByType<FallingNote2D>(FindObjectsSortMode.None);
        foreach (var note in fallingNotes)
        {
            note.enabled = false;
        }

        Debug.Log($"Disabled {notes.Length} 3D notes and {fallingNotes.Length} 2D notes");
    }

    void EnableAllNoteMovement()
    {
        // Enable all Note scripts
        Note[] notes = FindObjectsByType<Note>(FindObjectsSortMode.None);
        foreach (var note in notes)
        {
            note.enabled = true;
        }

        // Enable all FallingNote2D scripts
        FallingNote2D[] fallingNotes = FindObjectsByType<FallingNote2D>(FindObjectsSortMode.None);
        foreach (var note in fallingNotes)
        {
            note.enabled = true;
        }

        Debug.Log($"Enabled {notes.Length} 3D notes and {fallingNotes.Length} 2D notes");
    }

    void DisableGameplayScripts()
    {
        // Disable GameplayManager
        GameplayManager[] gameplayManagers = FindObjectsByType<GameplayManager>(FindObjectsSortMode.None);
        foreach (var manager in gameplayManagers)
        {
            manager.enabled = false;
        }

        // Disable NoteVisualOverlay
        NoteVisualOverlay[] overlays = FindObjectsByType<NoteVisualOverlay>(FindObjectsSortMode.None);
        foreach (var overlay in overlays)
        {
            overlay.enabled = false;
        }

        // Disable other gameplay scripts that might continue running
        MonoBehaviour[] allScripts = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
        foreach (var script in allScripts)
        {
            // Skip this pause script and UI components
            if (script == this || script.GetComponent<Canvas>() != null || script is Image || script is Button || script is TextMeshProUGUI)
                continue;

            // Disable scripts that might affect gameplay
            if (script.GetType().Name.Contains("Note") ||
                script.GetType().Name.Contains("Gameplay") ||
                script.GetType().Name.Contains("Spawner"))
            {
                script.enabled = false;
            }
        }

        Debug.Log("Gameplay scripts disabled");
    }

    void EnableGameplayScripts()
    {
        // Enable GameplayManager
        GameplayManager[] gameplayManagers = FindObjectsByType<GameplayManager>(FindObjectsSortMode.None);
        foreach (var manager in gameplayManagers)
        {
            manager.enabled = true;
        }

        // Enable NoteVisualOverlay
        NoteVisualOverlay[] overlays = FindObjectsByType<NoteVisualOverlay>(FindObjectsSortMode.None);
        foreach (var overlay in overlays)
        {
            overlay.enabled = true;
        }

        // Re-enable other gameplay scripts
        MonoBehaviour[] allScripts = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
        foreach (var script in allScripts)
        {
            // Skip this pause script and UI components
            if (script == this || script.GetComponent<Canvas>() != null || script is Image || script is Button || script is TextMeshProUGUI)
                continue;

            // Re-enable scripts that might affect gameplay
            if (script.GetType().Name.Contains("Note") ||
                script.GetType().Name.Contains("Gameplay") ||
                script.GetType().Name.Contains("Spawner"))
            {
                script.enabled = true;
            }
        }

        Debug.Log("Gameplay scripts enabled");
    }

    // Simple countdown UI method
    GameObject CreateSimpleCountdownUI()
    {
        // Create a simple canvas just for countdown
        GameObject canvasObj = new GameObject("SimpleCountdownCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999; // Highest priority

        // Create countdown text directly
        GameObject countdownObj = new GameObject("CountdownText");
        countdownObj.transform.SetParent(canvasObj.transform, false);

        RectTransform rect = countdownObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(200f, 200f);
        rect.anchoredPosition = Vector2.zero;

        TextMeshProUGUI text = countdownObj.AddComponent<TextMeshProUGUI>();
        text.text = "3";
        text.fontSize = 150f;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.Center;
        text.fontStyle = FontStyles.Bold;

        // Strong outline for visibility
        text.outlineWidth = 0.5f;
        text.outlineColor = Color.black;

        Debug.Log("✅ Simple countdown UI created");
        return canvasObj; // Return the canvas, not just the text
    }

    System.Collections.IEnumerator AnimateCountdownNumber(TextMeshProUGUI text)
    {
        // Scale animation
        Vector3 originalScale = text.transform.localScale;
        Vector3 bigScale = originalScale * 1.5f;

        float duration = 0.3f;
        float elapsed = 0f;

        // Scale up
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            text.transform.localScale = Vector3.Lerp(originalScale, bigScale, t);
            yield return null;
        }

        elapsed = 0f;

        // Scale down
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            text.transform.localScale = Vector3.Lerp(bigScale, originalScale, t);
            yield return null;
        }

        text.transform.localScale = originalScale;
    }

    System.Collections.IEnumerator FadeOutCountdown(GameObject countdownObj)
    {
        TextMeshProUGUI text = countdownObj.GetComponent<TextMeshProUGUI>();
        Color originalColor = text.color;

        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;

            Color newColor = originalColor;
            newColor.a = Mathf.Lerp(1f, 0f, t);
            text.color = newColor;

            yield return null;
        }
    }

    // Enhanced audio control methods
    void PauseAllAudio()
    {
        allAudioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        wasPlayingBeforePause = new bool[allAudioSources.Length];
        originalVolumes = new float[allAudioSources.Length];

        Debug.Log($"🔊 Found {allAudioSources.Length} audio sources to pause");

        for (int i = 0; i < allAudioSources.Length; i++)
        {
            if (allAudioSources[i] != null)
            {
                wasPlayingBeforePause[i] = allAudioSources[i].isPlaying;
                originalVolumes[i] = allAudioSources[i].volume;

                if (wasPlayingBeforePause[i])
                {
                    Debug.Log($"🔇 Pausing audio: {allAudioSources[i].name} (was playing: {allAudioSources[i].isPlaying})");

                    // Try multiple methods to stop the audio
                    allAudioSources[i].Pause();

                    // If force stop is enabled and pause didn't work
                    if (forceStopAudio && allAudioSources[i].isPlaying)
                    {
                        Debug.Log($"⚠️ Pause() failed for {allAudioSources[i].name}, using Stop()");
                        allAudioSources[i].Stop();
                    }

                    // As last resort, mute the audio
                    if (allAudioSources[i].isPlaying)
                    {
                        Debug.Log($"⚠️ Stop() failed for {allAudioSources[i].name}, muting volume");
                        allAudioSources[i].volume = 0f;
                    }
                }
            }
        }

        // Also try to pause AudioListener
        AudioListener.pause = true;

        Debug.Log("🔇 All audio paused/stopped");
    }

    void ResumeAllAudio()
    {
        // Resume AudioListener first
        AudioListener.pause = false;

        if (allAudioSources == null || wasPlayingBeforePause == null || originalVolumes == null)
        {
            Debug.LogWarning("⚠️ Audio tracking arrays are null, using fallback resume method");
            ResumeAllAudioFallback();
            return;
        }

        Debug.Log($"🔊 Resuming {allAudioSources.Length} audio sources");

        for (int i = 0; i < allAudioSources.Length; i++)
        {
            if (allAudioSources[i] != null && i < wasPlayingBeforePause.Length && i < originalVolumes.Length)
            {
                // Restore original volume first
                allAudioSources[i].volume = originalVolumes[i];

                if (wasPlayingBeforePause[i])
                {
                    Debug.Log($"🔊 Resuming audio: {allAudioSources[i].name}");

                    // Try UnPause first
                    allAudioSources[i].UnPause();

                    // If still not playing, try Play
                    if (!allAudioSources[i].isPlaying)
                    {
                        Debug.Log($"⚠️ UnPause() failed for {allAudioSources[i].name}, using Play()");
                        allAudioSources[i].Play();
                    }
                }
            }
        }

        Debug.Log("🔊 All audio resumed");
    }

    void ResumeAllAudioFallback()
    {
        AudioSource[] currentSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        Debug.Log($"🔊 Fallback: Found {currentSources.Length} audio sources");

        foreach (var source in currentSources)
        {
            if (source != null)
            {
                source.UnPause();
                Debug.Log($"🔊 Fallback resumed: {source.name}");
            }
        }
    }

    // Public properties
    public bool IsPaused => isPaused;

    // Emergency method to force resume if stuck
    [ContextMenu("Force Resume Game")]
    public void ForceResumeGame()
    {
        Debug.Log("🚨 FORCE RESUMING GAME");

        // Stop all coroutines
        StopAllCoroutines();

        // Clean up any countdown UI
        GameObject[] countdownObjects = GameObject.FindGameObjectsWithTag("Untagged");
        foreach (var obj in countdownObjects)
        {
            if (obj.name.Contains("Countdown"))
            {
                Destroy(obj);
            }
        }

        // Force resume everything
        isPaused = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;

        // Enable all scripts
        EnableGameplayScripts();
        EnableAllNoteMovement();
        ResumeAllNoteSpawning();
        ResumeAllAudio();

        // Hide pause panel
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        Debug.Log("✅ Game force resumed!");
    }

    // Public method to check audio status
    [ContextMenu("Check Audio Status")]
    public void CheckAudioStatus()
    {
        AudioSource[] sources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        Debug.Log($"🔊 AUDIO STATUS CHECK - Found {sources.Length} audio sources:");

        for (int i = 0; i < sources.Length; i++)
        {
            if (sources[i] != null)
            {
                Debug.Log($"  {i}: {sources[i].name} - Playing: {sources[i].isPlaying}, Volume: {sources[i].volume:F2}, Clip: {(sources[i].clip ? sources[i].clip.name : "None")}");
            }
        }

        Debug.Log($"🔊 AudioListener.pause: {AudioListener.pause}");
        Debug.Log($"🔊 Time.timeScale: {Time.timeScale}");
    }
}
