using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

/// <summary>
/// Manages global game state and scene transitions
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Game State")]
    public string selectedSongPath = "";
    public string selectedDifficulty = "Medium";
    public string selectedChartSection = "Full";
    
    [Header("Game Settings")]
    public float globalNoteSpeed = 5f;
    public float audioOffset = 0f;
    public bool showDebugInfo = false;
    
    [Header("Scene Management")]
    public string gameplaySceneName = "Gameplay";
    public string mainMenuSceneName = "MainMenu";
    
    // Properties for easy access
    public bool songSelected => !string.IsNullOrEmpty(selectedSongPath);
    public bool difficultySelected => !string.IsNullOrEmpty(selectedDifficulty);
    public bool canStartGame => songSelected && difficultySelected;
    
    // Events
    public System.Action OnPlayButtonStateChanged;
    public System.Action<string> OnSongSelected;
    public System.Action<string> OnDifficultySelected;
    
    // Song data cache
    private SongData currentSongData;
    
    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            // Ensure this GameObject is at root level for DontDestroyOnLoad
            if (transform.parent != null)
            {
                transform.SetParent(null);
            }
            DontDestroyOnLoad(gameObject);
            InitializeGameManager();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void InitializeGameManager()
    {
        // Load saved settings
        LoadGameSettings();
        
        // Initialize default values
        if (string.IsNullOrEmpty(selectedDifficulty))
            selectedDifficulty = "Medium";
            
        Debug.Log("🎮 GameManager initialized");
    }
    
    void LoadGameSettings()
    {
        globalNoteSpeed = PlayerPrefs.GetFloat("GlobalNoteSpeed", 5f);
        audioOffset = PlayerPrefs.GetFloat("AudioOffset", 0f);
        showDebugInfo = PlayerPrefs.GetInt("ShowDebugInfo", 0) == 1;
        selectedDifficulty = PlayerPrefs.GetString("LastSelectedDifficulty", "Medium");
    }
    
    public void SaveGameSettings()
    {
        PlayerPrefs.SetFloat("GlobalNoteSpeed", globalNoteSpeed);
        PlayerPrefs.SetFloat("AudioOffset", audioOffset);
        PlayerPrefs.SetInt("ShowDebugInfo", showDebugInfo ? 1 : 0);
        PlayerPrefs.SetString("LastSelectedDifficulty", selectedDifficulty);
        PlayerPrefs.Save();
    }
    
    public void SelectSong(string songPath, SongData songData = null)
    {
        selectedSongPath = songPath;
        currentSongData = songData;
        
        // Inform RankingManager of the current song to ensure per-song rankings
        if (songData != null)
        {
            var rankingMgr = FindFirstObjectByType<RankingManager>();
            if (rankingMgr != null)
            {
                rankingMgr.SetCurrentSong(songData);
            }
        }

        OnSongSelected?.Invoke(songPath);
        UpdatePlayButtonState();
        
        Debug.Log($"🎵 Song selected: {songPath}");
    }
    
    public void SelectDifficulty(string difficulty)
    {
        selectedDifficulty = difficulty;
        
        OnDifficultySelected?.Invoke(difficulty);
        UpdatePlayButtonState();
        
        Debug.Log($"🎯 Difficulty selected: {difficulty}");
    }
    
    public void UpdatePlayButtonState()
    {
        OnPlayButtonStateChanged?.Invoke();
    }
    
    public void StartGame()
    {
        if (!canStartGame)
        {
            Debug.LogWarning("⚠️ Cannot start game - missing song or difficulty selection");
            return;
        }
        
        // Validate song path exists
        string fullSongPath = GetFullSongPath(selectedSongPath);
        if (!Directory.Exists(fullSongPath))
        {
            Debug.LogError($"❌ Song path does not exist: {fullSongPath}");
            return;
        }
        
        // Check for required files
        if (!ValidateSongFiles(fullSongPath))
        {
            Debug.LogError("❌ Song is missing required files");
            return;
        }
        
        Debug.Log($"🚀 Starting game - Song: {selectedSongPath}, Difficulty: {selectedDifficulty}");
        
        // Save current settings
        SaveGameSettings();
        
        // Load gameplay scene
        SceneManager.LoadScene(gameplaySceneName);
    }
    
    public void ReturnToMainMenu()
    {
        Debug.Log("🏠 Returning to main menu");
        SceneManager.LoadScene(mainMenuSceneName);
    }
    
    public void QuitGame()
    {
        Debug.Log("👋 Quitting game");
        SaveGameSettings();
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    string GetFullSongPath(string songPath)
    {
        if (Path.IsPathRooted(songPath))
            return songPath;
            
        return Path.Combine(Application.streamingAssetsPath, "Songs", songPath);
    }
    
    bool ValidateSongFiles(string songPath)
    {
        string oggPath = Path.Combine(songPath, "song.ogg");
        string chartPath = Path.Combine(songPath, "notes.chart");
        
        bool hasAudio = File.Exists(oggPath);
        bool hasChart = File.Exists(chartPath);
        
        if (!hasAudio)
            Debug.LogWarning($"⚠️ Missing audio file: {oggPath}");
            
        if (!hasChart)
            Debug.LogWarning($"⚠️ Missing chart file: {chartPath}");
            
        return hasAudio && hasChart;
    }
    
    // Public getters for game state
    public string GetSelectedSongPath()
    {
        return selectedSongPath;
    }
    
    public string GetSelectedDifficulty()
    {
        return selectedDifficulty;
    }
    
    public SongData GetCurrentSongData()
    {
        return currentSongData;
    }
    
    public string GetFullSelectedSongPath()
    {
        return GetFullSongPath(selectedSongPath);
    }
    
    // Settings management
    public void SetNoteSpeed(float speed)
    {
        globalNoteSpeed = Mathf.Clamp(speed, 1f, 15f);
        SaveGameSettings();
    }
    
    public void SetAudioOffset(float offset)
    {
        audioOffset = Mathf.Clamp(offset, -1f, 1f);
        SaveGameSettings();
    }
    
    public void SetDebugInfo(bool enabled)
    {
        showDebugInfo = enabled;
        SaveGameSettings();
    }
    
    // Legacy compatibility methods
    public void Jugar()
    {
        StartGame();
    }
    
    public void SetFacil()
    {
        SelectDifficulty("Facil");
    }
    
    public void SetDificil()
    {
        SelectDifficulty("Dificil");
    }
    
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
            SaveGameSettings();
    }
    
    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
            SaveGameSettings();
    }
    
    void OnDestroy()
    {
        SaveGameSettings();
    }
}
