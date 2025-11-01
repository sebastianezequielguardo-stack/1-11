using UnityEngine;

/// <summary>
/// Integra el menú de pausa con el sistema de gameplay existente
/// </summary>
public class GameplayPauseIntegration : MonoBehaviour
{
    [Header("References")]
    public PauseMenu pauseMenu;

    [Header("Auto Detection")]
    public bool autoDetectComponents = false; // Disabled to prevent conflicts

    // Component references
    private GameplayManager gameplayManager;
    private AudioManager audioManager;
    private NoteSpawner noteSpawner;
    private NoteSpawner2D noteSpawner2D;
    private AudioSource[] allAudioSources;

    // Pause state tracking
    private bool[] audioSourceWasPlaying;
    private float[] originalAudioVolumes;

    void Start()
    {
        if (autoDetectComponents)
        {
            DetectGameplayComponents();
        }

        SetupPauseMenuIntegration();
    }

    void DetectGameplayComponents()
    {
        // Find gameplay components
        gameplayManager = FindFirstObjectByType<GameplayManager>();
        audioManager = FindFirstObjectByType<AudioManager>();
        noteSpawner = FindFirstObjectByType<NoteSpawner>();
        noteSpawner2D = FindFirstObjectByType<NoteSpawner2D>();

        // Find all audio sources
        allAudioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        audioSourceWasPlaying = new bool[allAudioSources.Length];
        originalAudioVolumes = new float[allAudioSources.Length];

        // Store original volumes
        for (int i = 0; i < allAudioSources.Length; i++)
        {
            originalAudioVolumes[i] = allAudioSources[i].volume;
        }

        Debug.Log($"Detected {allAudioSources.Length} audio sources for pause integration");
    }

    void SetupPauseMenuIntegration()
    {
        if (pauseMenu == null)
        {
            pauseMenu = FindFirstObjectByType<PauseMenu>();
        }

        if (pauseMenu == null)
        {
            Debug.LogWarning("PauseMenu not found! Trying SimplePauseSetup instead...");

            // Try to find SimplePauseSetup as alternative
            SimplePauseSetup simplePause = FindFirstObjectByType<SimplePauseSetup>();
            if (simplePause == null)
            {
                Debug.Log("Creating SimplePauseSetup as fallback...");
                GameObject pauseObj = new GameObject("SimplePauseSetup");
                pauseObj.transform.SetParent(transform);
                simplePause = pauseObj.AddComponent<SimplePauseSetup>();
            }

            Debug.Log("✅ Using SimplePauseSetup for pause functionality");
        }

        // Set up song info if available
        UpdateSongInfo();
    }

    void CreatePauseMenuAutomatically()
    {
        GameObject pauseMenuObj = new GameObject("PauseMenuSystem");
        pauseMenuObj.transform.SetParent(transform);

        // Add PauseMenu component first
        pauseMenu = pauseMenuObj.AddComponent<PauseMenu>();

        // Add PauseMenuUI to create the UI automatically
        PauseMenuUI pauseMenuUI = pauseMenuObj.AddComponent<PauseMenuUI>();
        pauseMenuUI.createFromScratch = true;
        pauseMenuUI.setupOnStart = true;

        Debug.Log("✅ PauseMenu created automatically!");
    }

    void UpdateSongInfo()
    {
        if (pauseMenu == null) return;

        string songTitle = "Unknown Song";
        string artist = "Unknown Artist";

        // Try to get song info from various sources
        if (gameplayManager != null)
        {
            // Try to get from GameplayManager
            songTitle = GetSongTitleFromGameplayManager();
            artist = GetArtistFromGameplayManager();
        }

        // Try to get from AudioManager
        if (audioManager != null && (songTitle == "Unknown Song" || artist == "Unknown Artist"))
        {
            // Add logic to get song info from AudioManager if available
        }

        // Update pause menu with song info
        pauseMenu.SetSongInfo(songTitle, artist);

        Debug.Log($"Song info updated: {songTitle} by {artist}");
    }

    string GetSongTitleFromGameplayManager()
    {
        // Implement based on your GameplayManager structure
        // This is a placeholder - adjust based on your actual implementation
        return "Current Song";
    }

    string GetArtistFromGameplayManager()
    {
        // Implement based on your GameplayManager structure
        // This is a placeholder - adjust based on your actual implementation
        return "Current Artist";
    }

    void Update()
    {
        // Monitor pause state and handle additional pause logic
        if (pauseMenu != null && pauseMenu.IsPaused)
        {
            HandlePausedState();
        }
    }

    void HandlePausedState()
    {
        // Additional logic for when game is paused
        // Stop note spawning
        if (noteSpawner != null)
        {
            // Add pause logic for original note spawner if needed
        }

        if (noteSpawner2D != null)
        {
            // Pause 2D note spawner
            noteSpawner2D.enabled = false;
        }
    }

    void OnEnable()
    {
        // Subscribe to pause events if needed
        if (pauseMenu != null)
        {
            // Add event subscriptions here
        }
    }

    void OnDisable()
    {
        // Unsubscribe from pause events
        if (pauseMenu != null)
        {
            // Remove event subscriptions here
        }
    }

    // Public methods for external control
    public void PauseAllAudio()
    {
        for (int i = 0; i < allAudioSources.Length; i++)
        {
            if (allAudioSources[i] != null)
            {
                audioSourceWasPlaying[i] = allAudioSources[i].isPlaying;
                if (audioSourceWasPlaying[i])
                {
                    allAudioSources[i].Pause();
                }
            }
        }
    }

    public void ResumeAllAudio()
    {
        for (int i = 0; i < allAudioSources.Length; i++)
        {
            if (allAudioSources[i] != null && audioSourceWasPlaying[i])
            {
                allAudioSources[i].UnPause();
            }
        }
    }

    public void StopAllNoteSpawning()
    {
        if (noteSpawner2D != null)
        {
            noteSpawner2D.CancelInvoke();
        }
    }

    public void ResumeAllNoteSpawning()
    {
        if (noteSpawner2D != null && noteSpawner2D.autoSpawn)
        {
            // Restart auto spawning
            noteSpawner2D.enabled = true;
        }
    }

    // Method to update song section from external sources
    public void UpdateCurrentSection(string section)
    {
        if (pauseMenu != null)
        {
            pauseMenu.SetCurrentSection(section);
        }
    }

    // Method to be called when song changes
    public void OnSongChanged(string newTitle, string newArtist)
    {
        if (pauseMenu != null)
        {
            pauseMenu.SetSongInfo(newTitle, newArtist);
        }
    }
}