using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class PauseMenu : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pauseMenuPanel;
    public Image backgroundBlur;
    public TextMeshProUGUI songTitleText;
    public TextMeshProUGUI artistText;
    public TextMeshProUGUI currentSectionText;

    [Header("Menu Buttons")]
    public Button resumeButton;
    public Button restartButton;
    public Button mainMenuButton;
    public Button optionsButton;
    public Button songOptionsButton;
    public Button exitButton;

    [Header("Visual Settings")]
    public Color blurColor = new Color(0f, 0f, 0f, 0.7f);
    public float blurTransitionSpeed = 5f;

    [Header("Audio")]
    public AudioSource musicSource;

    // State management
    private bool isPaused = false;
    private float originalTimeScale;
    private float originalMusicVolume;

    // References
    private GameplayManager gameplayManager;
    private AudioManager audioManager;
    private Canvas pauseCanvas;

    // Current song info
    private string currentSongTitle = "Unknown Song";
    private string currentArtist = "Unknown Artist";
    private string currentSection = "INTRO";

    void Start()
    {
        InitializePauseMenu();
        SetupButtonListeners();
        LoadSongInfo();
    }

    void InitializePauseMenu()
    {
        // Get references
        gameplayManager = FindFirstObjectByType<GameplayManager>();
        audioManager = FindFirstObjectByType<AudioManager>();

        // Store original values
        originalTimeScale = Time.timeScale;
        if (musicSource != null)
        {
            originalMusicVolume = musicSource.volume;
        }

        // Setup canvas
        if (pauseMenuPanel != null)
        {
            pauseCanvas = pauseMenuPanel.GetComponentInParent<Canvas>();
            if (pauseCanvas != null)
            {
                pauseCanvas.sortingOrder = 100; // Ensure it's on top
            }
        }

        // Initially hide the pause menu
        SetPauseMenuActive(false);
    }

    void SetupButtonListeners()
    {
        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartSong);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(GoToMainMenu);

        if (optionsButton != null)
            optionsButton.onClick.AddListener(OpenOptions);

        if (songOptionsButton != null)
            songOptionsButton.onClick.AddListener(OpenSongOptions);

        if (exitButton != null)
            exitButton.onClick.AddListener(ExitGame);
    }

    void LoadSongInfo()
    {
        // Try to get song info from GameplayManager
        if (gameplayManager != null)
        {
            // Assuming GameplayManager has song info
            // You might need to adjust these based on your actual GameplayManager structure
            currentSongTitle = GetSongTitle();
            currentArtist = GetArtist();
        }

        UpdateSongInfoDisplay();
    }

    string GetSongTitle()
    {
        // Try to get from GameplayManager or other sources
        if (gameplayManager != null)
        {
            // Add your logic to get song title
            return "Current Song"; // Placeholder
        }
        return "Unknown Song";
    }

    string GetArtist()
    {
        // Try to get from GameplayManager or other sources
        if (gameplayManager != null)
        {
            // Add your logic to get artist
            return "Unknown Artist"; // Placeholder
        }
        return "Unknown Artist";
    }

    void UpdateSongInfoDisplay()
    {
        if (songTitleText != null)
            songTitleText.text = currentSongTitle;

        if (artistText != null)
            artistText.text = currentArtist;

        if (currentSectionText != null)
            currentSectionText.text = $"CURRENT SECTION:\n{currentSection}";
    }

    void Update()
    {
        // Check for pause input
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }

        // Update current section (you can implement section detection logic here)
        UpdateCurrentSection();
    }

    void UpdateCurrentSection()
    {
        // Implement logic to detect current song section
        // This could be based on song time, chart data, etc.
        if (gameplayManager != null)
        {
            float songTime = gameplayManager.GetSongTime();

            // Example section detection (adjust based on your needs)
            if (songTime < 10f)
                currentSection = "INTRO";
            else if (songTime < 30f)
                currentSection = "VERSE 1";
            else if (songTime < 50f)
                currentSection = "CHORUS";
            else if (songTime < 70f)
                currentSection = "VERSE 2";
            else if (songTime < 90f)
                currentSection = "CHORUS";
            else if (songTime < 110f)
                currentSection = "BRIDGE";
            else if (songTime < 130f)
                currentSection = "FINAL CHORUS";
            else
                currentSection = "OUTRO";

            // Update display only if section changed
            if (currentSectionText != null && !currentSectionText.text.Contains(currentSection))
            {
                currentSectionText.text = $"CURRENT SECTION:\n{currentSection}";
            }
        }
    }

    public void PauseGame()
    {
        if (isPaused) return;

        isPaused = true;

        // Pause time
        Time.timeScale = 0f;

        // Pause music
        if (musicSource != null)
        {
            musicSource.Pause();
        }

        // Pause audio manager
        if (audioManager != null)
        {
            // audioManager.PauseMusic(); // Method might not exist, using alternative
            AudioSource[] allSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
            foreach (var source in allSources)
            {
                if (source.isPlaying) source.Pause();
            }
        }

        // Show pause menu
        SetPauseMenuActive(true);

        // Start blur transition
        StartCoroutine(BlurTransition(true));

        Debug.Log("Game Paused");
    }

    public void ResumeGame()
    {
        if (!isPaused) return;

        StartCoroutine(ResumeGameCoroutine());
    }

    IEnumerator ResumeGameCoroutine()
    {
        // Start blur transition out
        StartCoroutine(BlurTransition(false));

        // Wait a bit for visual feedback
        yield return new WaitForSecondsRealtime(0.1f);

        isPaused = false;

        // Resume time
        Time.timeScale = originalTimeScale;

        // Resume music
        if (musicSource != null)
        {
            musicSource.UnPause();
        }

        // Resume audio manager
        if (audioManager != null)
        {
            // audioManager.ResumeMusic(); // Method might not exist, using alternative
            AudioSource[] allSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
            foreach (var source in allSources)
            {
                source.UnPause();
            }
        }

        // Hide pause menu
        SetPauseMenuActive(false);

        Debug.Log("Game Resumed");
    }

    public void RestartSong()
    {
        // Resume time first
        Time.timeScale = originalTimeScale;

        // Reload current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        // Resume time first
        Time.timeScale = originalTimeScale;

        // Load main menu scene
        SceneManager.LoadScene("MainMenu"); // Adjust scene name as needed
    }

    public void OpenOptions()
    {
        // Implement options menu
        Debug.Log("Options menu not implemented yet");
    }

    public void OpenSongOptions()
    {
        // Implement song-specific options
        Debug.Log("Song options not implemented yet");
    }

    public void ExitGame()
    {
        // Resume time first
        Time.timeScale = originalTimeScale;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    void SetPauseMenuActive(bool active)
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(active);
        }
    }

    IEnumerator BlurTransition(bool fadeIn)
    {
        if (backgroundBlur == null) yield break;

        Color startColor = fadeIn ? Color.clear : blurColor;
        Color endColor = fadeIn ? blurColor : Color.clear;

        float elapsed = 0f;
        float duration = 1f / blurTransitionSpeed;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;

            backgroundBlur.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }

        backgroundBlur.color = endColor;
    }

    // Public methods for external control
    public bool IsPaused => isPaused;

    public void SetSongInfo(string title, string artist)
    {
        currentSongTitle = title;
        currentArtist = artist;
        UpdateSongInfoDisplay();
    }

    public void SetCurrentSection(string section)
    {
        currentSection = section;
        if (currentSectionText != null)
        {
            currentSectionText.text = $"CURRENT SECTION:\n{section}";
        }
    }
}
