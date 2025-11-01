using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Main menu controller that handles navigation and UI interactions
/// </summary>
public class MainMenu : MonoBehaviour
{
    [Header("Main Menu Panels")]
    public GameObject mainMenuPanel;
    public GameObject songSelectionPanel;
    public GameObject settingsPanel;
    public GameObject calibrationPanel;
    public GameObject creditsPanel;
    
    [Header("Navigation Buttons")]
    public Button playButton;
    public Button settingsButton;
    public Button calibrationButton;
    public Button creditsButton;
    public Button quitButton;
    
    [Header("Back Buttons")]
    public Button backFromSongSelection;
    public Button backFromSettings;
    public Button backFromCalibration;
    public Button backFromCredits;
    
    [Header("Game Info Display")]
    public TextMeshProUGUI selectedSongText;
    public TextMeshProUGUI selectedDifficultyText;
    public TextMeshProUGUI gameVersionText;
    
    [Header("Audio")]
    public AudioSource menuMusicSource;
    public AudioClip menuMusic;
    
    void Start()
    {
        InitializeMainMenu();
        SetupButtonListeners();
        UpdateGameInfoDisplay();
    }
    
    void InitializeMainMenu()
    {
        // Show main menu panel, hide others
        ShowPanel(mainMenuPanel);
        
        // Set game version if available
        if (gameVersionText != null)
            gameVersionText.text = $"Version {Application.version}";
        
        // Start menu music
        if (menuMusicSource != null && menuMusic != null)
        {
            menuMusicSource.clip = menuMusic;
            menuMusicSource.loop = true;
            menuMusicSource.Play();
        }
        
        Debug.Log("🏠 Main Menu initialized");
    }
    
    void SetupButtonListeners()
    {
        // Main navigation buttons
        if (playButton != null)
            playButton.onClick.AddListener(ShowSongSelection);
            
        if (settingsButton != null)
            settingsButton.onClick.AddListener(ShowSettings);
            
        if (calibrationButton != null)
            calibrationButton.onClick.AddListener(ShowCalibration);
            
        if (creditsButton != null)
            creditsButton.onClick.AddListener(ShowCredits);
            
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
        
        // Back buttons
        if (backFromSongSelection != null)
            backFromSongSelection.onClick.AddListener(ShowMainMenu);
            
        if (backFromSettings != null)
            backFromSettings.onClick.AddListener(ShowMainMenu);
            
        if (backFromCalibration != null)
            backFromCalibration.onClick.AddListener(ShowMainMenu);
            
        if (backFromCredits != null)
            backFromCredits.onClick.AddListener(ShowMainMenu);
    }
    
    void ShowPanel(GameObject panelToShow)
    {
        // Hide all panels
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (songSelectionPanel != null) songSelectionPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (calibrationPanel != null) calibrationPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(false);
        
        // Show the requested panel
        if (panelToShow != null)
            panelToShow.SetActive(true);
    }
    
    // Navigation methods
    public void ShowMainMenu()
    {
        ShowPanel(mainMenuPanel);
        UpdateGameInfoDisplay();

        // Option A: consume pending ranking stored by PostGameplay
        if (PlayerPrefs.GetInt("PendingRankingExists", 0) == 1)
        {
            int score = PlayerPrefs.GetInt("PendingRankingScore", 0);
            string playerName = PlayerPrefs.GetString("PendingRankingName", "Player");
            string pendingSongName = PlayerPrefs.GetString("PendingSongName", string.Empty);
            string pendingArtist = PlayerPrefs.GetString("PendingArtist", string.Empty);

            var gm = GameManager.Instance;
            var songData = gm != null ? gm.GetCurrentSongData() : null;
            var rankingMgr = FindFirstObjectByType<RankingManager>();

            // Fallback: if we lost the SongData reference, try to find it via SongLoader
            if (songData == null)
            {
                var loader = FindFirstObjectByType<SongLoader>();
                if (loader != null)
                {
                    var songs = loader.GetAllSongs();
                    if (songs != null)
                    {
                        songData = songs.Find(s =>
                            s != null &&
                            !string.IsNullOrEmpty(s.songName) &&
                            !string.IsNullOrEmpty(s.artist) &&
                            s.songName.Equals(pendingSongName, System.StringComparison.OrdinalIgnoreCase) &&
                            s.artist.Equals(pendingArtist, System.StringComparison.OrdinalIgnoreCase));
                    }
                }
            }

            if (rankingMgr != null && score > 0)
            {
                if (songData != null)
                {
                    rankingMgr.SetCurrentSong(songData);
                }
                else
                {
                    // As a last resort, generate the same songID pattern used by SongData
                    string generatedId = GenerateSongIDFrom(pendingSongName, pendingArtist);
                    rankingMgr.SetCurrentSong(generatedId, string.IsNullOrEmpty(pendingSongName) ? "Unknown Song" : pendingSongName);
                }
                rankingMgr.AddScore(playerName, score);
                rankingMgr.ShowRanking();
            }

            // Clear pending flags
            PlayerPrefs.DeleteKey("PendingRankingExists");
            PlayerPrefs.DeleteKey("PendingRankingScore");
            PlayerPrefs.DeleteKey("PendingRankingName");
            PlayerPrefs.DeleteKey("PendingSongName");
            PlayerPrefs.DeleteKey("PendingArtist");
            PlayerPrefs.Save();
        }
        Debug.Log("🏠 Showing main menu");
    }

    // Fallback helper: replicate SongData's ID generation (name+artist sanitized)
    string GenerateSongIDFrom(string name, string artist)
    {
        if (name == null) name = string.Empty;
        if (artist == null) artist = string.Empty;
        string cleanName = name.ToLower().Replace(" ", "_").Replace("-", "_");
        string cleanArtist = artist.ToLower().Replace(" ", "_").Replace("-", "_");
        cleanName = System.Text.RegularExpressions.Regex.Replace(cleanName, @"[^a-z0-9_]", "");
        cleanArtist = System.Text.RegularExpressions.Regex.Replace(cleanArtist, @"[^a-z0-9_]", "");
        return $"{cleanArtist}_{cleanName}";
    }
    
    public void ShowSongSelection()
    {
        ShowPanel(songSelectionPanel);
        Debug.Log("🎵 Showing song selection");
    }
    
    public void ShowSettings()
    {
        ShowPanel(settingsPanel);
        
        // Notify SettingsManager to show settings
        SettingsManager settingsManager = FindFirstObjectByType<SettingsManager>();
        if (settingsManager != null)
            settingsManager.ShowSettings();
            
        Debug.Log("⚙️ Showing settings");
    }
    
    public void ShowCalibration()
    {
        ShowPanel(calibrationPanel);
        
        // Notify CalibrationManager to show calibration
        CalibrationManager calibrationManager = FindFirstObjectByType<CalibrationManager>();
        if (calibrationManager != null)
            calibrationManager.ShowCalibrationMenu();
            
        Debug.Log("🎯 Showing calibration");
    }
    
    public void ShowCredits()
    {
        ShowPanel(creditsPanel);
        Debug.Log("👥 Showing credits");
    }
    
    public void QuitGame()
    {
        Debug.Log("👋 Quitting game");
        
        if (GameManager.Instance != null)
            GameManager.Instance.QuitGame();
        else
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }
    
    // Legacy compatibility methods
    public void Jugar()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("❌ GameManager.Instance is null");
            return;
        }
        
        if (string.IsNullOrEmpty(GameManager.Instance.selectedSongPath))
        {
            Debug.LogWarning("⚠️ No se seleccionó ninguna canción.");
            ShowSongSelection(); // Show song selection instead
            return;
        }

        GameManager.Instance.StartGame();
    }

    // NOTE: SetFacil() and SetDificil() methods are now handled by DifficultyButtonManager
    // These methods are kept for backward compatibility but should not be used directly
    public void SetFacil()
    {
        Debug.LogWarning("⚠️ SetFacil() called directly - should use DifficultyButtonManager instead");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SelectDifficulty("Facil");
            UpdateGameInfoDisplay();
            Debug.Log("🎯 Dificultad: Fácil");
        }
    }

    public void SetDificil()
    {
        Debug.LogWarning("⚠️ SetDificil() called directly - should use DifficultyButtonManager instead");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SelectDifficulty("Dificil");
            UpdateGameInfoDisplay();
            Debug.Log("🎯 Dificultad: Difícil");
        }
    }
    
    void UpdateGameInfoDisplay()
    {
        if (GameManager.Instance == null) return;
        
        // Update selected song display
        if (selectedSongText != null)
        {
            if (GameManager.Instance.songSelected)
            {
                SongData currentSong = GameManager.Instance.GetCurrentSongData();
                if (currentSong != null)
                    selectedSongText.text = $"Song: {currentSong.GetDisplayName()}";
                else
                    selectedSongText.text = $"Song: {GameManager.Instance.selectedSongPath}";
            }
            else
            {
                selectedSongText.text = "Song: None Selected";
            }
        }
        
        // Update selected difficulty display
        if (selectedDifficultyText != null)
        {
            if (GameManager.Instance.difficultySelected)
                selectedDifficultyText.text = $"Difficulty: {GameManager.Instance.selectedDifficulty}";
            else
                selectedDifficultyText.text = "Difficulty: None Selected";
        }
    }
    
    void OnEnable()
    {
        // Subscribe to GameManager events
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnSongSelected += OnSongSelected;
            GameManager.Instance.OnDifficultySelected += OnDifficultySelected;
        }
    }
    
    void OnDisable()
    {
        // Unsubscribe from GameManager events
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnSongSelected -= OnSongSelected;
            GameManager.Instance.OnDifficultySelected -= OnDifficultySelected;
        }
    }
    
    void OnSongSelected(string songPath)
    {
        UpdateGameInfoDisplay();
    }
    
    void OnDifficultySelected(string difficulty)
    {
        UpdateGameInfoDisplay();
    }
    
    // Public methods for external access
    public void RefreshGameInfo()
    {
        UpdateGameInfoDisplay();
    }
    
    public bool IsMainMenuActive()
    {
        return mainMenuPanel != null && mainMenuPanel.activeInHierarchy;
    }
    
    public void SetMenuMusicVolume(float volume)
    {
        if (menuMusicSource != null)
            menuMusicSource.volume = Mathf.Clamp01(volume);
    }
    
    public void StopMenuMusic()
    {
        if (menuMusicSource != null && menuMusicSource.isPlaying)
            menuMusicSource.Stop();
    }
    
    public void PlayMenuMusic()
    {
        if (menuMusicSource != null && menuMusic != null && !menuMusicSource.isPlaying)
        {
            menuMusicSource.clip = menuMusic;
            menuMusicSource.Play();
        }
    }
    
    void Update()
    {
        // Handle escape key to go back
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (songSelectionPanel != null && songSelectionPanel.activeInHierarchy)
                ShowMainMenu();
            else if (settingsPanel != null && settingsPanel.activeInHierarchy)
                ShowMainMenu();
            else if (calibrationPanel != null && calibrationPanel.activeInHierarchy)
                ShowMainMenu();
            else if (creditsPanel != null && creditsPanel.activeInHierarchy)
                ShowMainMenu();
        }
    }
}
