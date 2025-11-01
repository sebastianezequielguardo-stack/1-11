using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

/// <summary>
/// Manages the post-gameplay screen with results and record entry
/// </summary>
public class PostGameplayManager : MonoBehaviour
{
    [Header("UI Components")]
    public GameObject mainPanel;
    public GameObject initialsInputPanel;
    public GameObject duplicateInitialsPanel;
    
    [Header("Song Information")]
    public TextMeshProUGUI songNameText;
    public TextMeshProUGUI artistText;
    public TextMeshProUGUI difficultyText;
    
    [Header("Performance Results")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI accuracyText;
    public TextMeshProUGUI completionText;
    public TextMeshProUGUI perfectHitsText;
    public TextMeshProUGUI goodHitsText;
    public TextMeshProUGUI okHitsText;
    public TextMeshProUGUI missedHitsText;
    
    [Header("Record Status")]
    public GameObject newRecordIndicator;
    public TextMeshProUGUI recordPositionText;
    
    [Header("Initials Input")]
    public TMP_InputField initialsInputField;
    public Button confirmInitialsButton;
    public Button cancelInitialsButton;
    
    [Header("Duplicate Initials")]
    public TextMeshProUGUI duplicateMessageText;
    public Button updateRecordButton;
    public Button enterNewInitialsButton;
    
    [Header("Navigation")]
    public Button returnToMenuButton;
    
    [Header("Visual Effects")]
    public ParticleSystem newRecordEffect;
    public AudioSource celebrationSound;
    
    // Game results data
    private GameResultsData gameResults;
    private RecordManager recordManager;
    private bool isNewRecord = false;
    
    void Start()
    {
        InitializePostGameplay();
        LoadGameResults();
    }
    
    void InitializePostGameplay()
    {
        // Find RecordManager
        recordManager = FindFirstObjectByType<RecordManager>();
        
        // Setup UI listeners
        if (confirmInitialsButton != null)
            confirmInitialsButton.onClick.AddListener(OnConfirmInitials);
            
        if (cancelInitialsButton != null)
            cancelInitialsButton.onClick.AddListener(OnCancelInitials);
            
        if (updateRecordButton != null)
            updateRecordButton.onClick.AddListener(OnUpdateRecord);
            
        if (enterNewInitialsButton != null)
            enterNewInitialsButton.onClick.AddListener(OnEnterNewInitials);
            
        if (returnToMenuButton != null)
            returnToMenuButton.onClick.AddListener(ReturnToMenu);
        
        // Setup input field
        if (initialsInputField != null)
        {
            initialsInputField.characterLimit = 3;
            initialsInputField.text = "";
            initialsInputField.onValueChanged.AddListener(OnInitialsChanged);
        }
        
        // Hide panels initially
        ShowPanel(mainPanel);
    }
    
    void LoadGameResults()
    {
        // Get results from GameResultsData (passed from gameplay scene)
        gameResults = GameResultsData.Instance;
        
        if (gameResults == null)
        {
            Debug.LogError("PostGameplayManager: No game results found!");
            CreateDummyResults(); // For testing
        }
        
        DisplayResults();
        CheckForNewRecord();
    }
    
    void CreateDummyResults()
    {
        // Create dummy results for testing
        gameResults = new GameResultsData();
        gameResults.songName = "Test Song";
        gameResults.artist = "Test Artist";
        gameResults.difficulty = "Medium";
        gameResults.score = 95000;
        gameResults.accuracy = 87.5f;
        gameResults.completionPercentage = 92.3f;
        gameResults.perfectHits = 150;
        gameResults.goodHits = 45;
        gameResults.okHits = 20;
        gameResults.missedHits = 35;
        gameResults.totalNotes = 250;
    }
    
    void DisplayResults()
    {
        // Display song information
        if (songNameText != null)
            songNameText.text = gameResults.songName;
            
        if (artistText != null)
            artistText.text = $"by {gameResults.artist}";
            
        if (difficultyText != null)
            difficultyText.text = $"Difficulty: {gameResults.difficulty}";
        
        // Display performance results
        if (scoreText != null)
            scoreText.text = $"Score: {gameResults.score:N0}";
            
        if (accuracyText != null)
            accuracyText.text = $"Accuracy: {gameResults.accuracy:F1}%";
            
        if (completionText != null)
            completionText.text = $"Completion: {gameResults.completionPercentage:F1}%";
            
        if (perfectHitsText != null)
            perfectHitsText.text = $"Perfect: {gameResults.perfectHits}";
            
        if (goodHitsText != null)
            goodHitsText.text = $"Good: {gameResults.goodHits}";
            
        if (okHitsText != null)
            okHitsText.text = $"OK: {gameResults.okHits}";
            
        if (missedHitsText != null)
            missedHitsText.text = $"Missed: {gameResults.missedHits}";
    }
    
    void CheckForNewRecord()
    {
        if (recordManager == null)
        {
            Debug.LogWarning("PostGameplayManager: RecordManager not found");
            return;
        }
        
        // Check if this is a new record
        var currentRecords = recordManager.GetCurrentSongRecords();
        
        if (currentRecords.Count == 0)
        {
            // First record for this song
            isNewRecord = true;
            ShowNewRecordIndicator(1);
        }
        else
        {
            // Check position in leaderboard
            int position = GetRecordPosition(currentRecords);
            
            if (position <= 5) // Top 5 records
            {
                isNewRecord = true;
                ShowNewRecordIndicator(position);
            }
        }
        
        if (isNewRecord)
        {
            // Show initials input after a delay
            StartCoroutine(ShowInitialsInputDelayed(2f));
        }
    }
    
    int GetRecordPosition(System.Collections.Generic.List<RecordData> records)
    {
        var tempRecord = new RecordData("TMP", gameResults.score, gameResults.accuracy, gameResults.completionPercentage);
        
        int position = 1;
        foreach (var record in records)
        {
            if (tempRecord.IsBetterThan(record))
                break;
            position++;
        }
        
        return position;
    }
    
    void ShowNewRecordIndicator(int position)
    {
        if (newRecordIndicator != null)
            newRecordIndicator.SetActive(true);
            
        if (recordPositionText != null)
            recordPositionText.text = $"New Record! Position #{position}";
        
        // Play celebration effects
        if (newRecordEffect != null)
            newRecordEffect.Play();
            
        if (celebrationSound != null)
            celebrationSound.Play();
    }
    
    IEnumerator ShowInitialsInputDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowInitialsInput();
    }
    
    void ShowInitialsInput()
    {
        ShowPanel(initialsInputPanel);
        
        if (initialsInputField != null)
        {
            initialsInputField.text = "";
            initialsInputField.Select();
            initialsInputField.ActivateInputField();
        }
    }
    
    void OnInitialsChanged(string value)
    {
        // Convert to uppercase and limit to 3 characters
        if (initialsInputField != null)
        {
            string upperValue = value.ToUpper();
            if (upperValue != value)
            {
                initialsInputField.text = upperValue;
            }
        }
        
        // Enable/disable confirm button
        if (confirmInitialsButton != null)
        {
            confirmInitialsButton.interactable = !string.IsNullOrEmpty(value) && value.Length >= 1;
        }
    }
    
    void OnConfirmInitials()
    {
        string initials = initialsInputField.text.Trim().ToUpper();
        
        if (string.IsNullOrEmpty(initials))
        {
            Debug.LogWarning("PostGameplayManager: Empty initials entered");
            return;
        }
        
        // Pad initials to 3 characters
        initials = initials.PadRight(3, 'A');
        
        // Check for duplicate initials
        if (recordManager != null)
        {
            if (recordManager.recordDisplay.CheckForDuplicateInitials(initials, out RecordData existingRecord))
            {
                ShowDuplicateInitialsDialog(initials, existingRecord);
                return;
            }
        }
        
        // No duplicates - save record
        SaveRecord(initials);
    }
    
    void ShowDuplicateInitialsDialog(string initials, RecordData existingRecord)
    {
        ShowPanel(duplicateInitialsPanel);
        
        if (duplicateMessageText != null)
        {
            duplicateMessageText.text = $"The initials '{initials}' already exist with a record of {existingRecord.GetFormattedRecord()}.\n\nAre you the same person?";
        }
    }
    
    void OnUpdateRecord()
    {
        string initials = initialsInputField.text.Trim().ToUpper().PadRight(3, 'A');
        SaveRecord(initials);
    }
    
    void OnEnterNewInitials()
    {
        ShowInitialsInput();
    }
    
    void OnCancelInitials()
    {
        ShowPanel(mainPanel);
    }
    
    void SaveRecord(string initials)
    {
        if (recordManager == null)
        {
            Debug.LogError("PostGameplayManager: RecordManager not found");
            return;
        }
        
        // Create new record
        RecordData newRecord = new RecordData(
            initials,
            gameResults.score,
            gameResults.accuracy,
            gameResults.completionPercentage,
            gameResults.perfectHits,
            gameResults.goodHits,
            gameResults.okHits,
            gameResults.missedHits,
            gameResults.totalNotes
        );
        
        newRecord.difficulty = gameResults.difficulty;
        
        // Save record
        recordManager.SaveNewRecord(newRecord);
        
        Debug.Log($"PostGameplayManager: Saved record - {newRecord.GetFormattedRecord()}");
        
        // Return to main panel
        ShowPanel(mainPanel);
        
        // Update button text to show record was saved
        if (returnToMenuButton != null)
        {
            var buttonText = returnToMenuButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
                buttonText.text = "Record Saved! Return to Menu";
        }
    }
    
    void ShowPanel(GameObject panelToShow)
    {
        // Hide all panels
        if (mainPanel != null) mainPanel.SetActive(false);
        if (initialsInputPanel != null) initialsInputPanel.SetActive(false);
        if (duplicateInitialsPanel != null) duplicateInitialsPanel.SetActive(false);
        
        // Show requested panel
        if (panelToShow != null)
            panelToShow.SetActive(true);
    }
    
    void ReturnToMenu()
    {
        // Save pending ranking info for MainMenu to consume (Option A)
        if (gameResults != null)
        {
            // Player name from PlayerPrefs (fallback to "Player")
            string playerName = PlayerPrefs.GetString("PlayerName", "Player");
            PlayerPrefs.SetInt("PendingRankingScore", gameResults.score);
            PlayerPrefs.SetString("PendingRankingName", playerName);
            PlayerPrefs.SetString("PendingSongName", gameResults.songName ?? string.Empty);
            PlayerPrefs.SetString("PendingArtist", gameResults.artist ?? string.Empty);
            PlayerPrefs.SetInt("PendingRankingExists", 1);
            PlayerPrefs.Save();
        }

        // Clean up game results
        if (GameResultsData.Instance != null)
        {
            Destroy(GameResultsData.Instance.gameObject);
        }
        
        // Load main menu scene
        SceneManager.LoadScene("MainMenu");
    }
    
    void Update()
    {
        // Handle escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (initialsInputPanel != null && initialsInputPanel.activeInHierarchy)
                OnCancelInitials();
            else if (duplicateInitialsPanel != null && duplicateInitialsPanel.activeInHierarchy)
                OnEnterNewInitials();
        }
        
        // Handle enter key in initials input
        if (initialsInputPanel != null && initialsInputPanel.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                OnConfirmInitials();
            }
        }
    }
}

/// <summary>
/// Data container for game results passed between scenes
/// </summary>
public class GameResultsData : MonoBehaviour
{
    public static GameResultsData Instance { get; private set; }
    
    [Header("Song Information")]
    public string songName;
    public string artist;
    public string difficulty;
    public string songPath;
    
    [Header("Performance Results")]
    public int score;
    public float accuracy;
    public float completionPercentage;
    
    [Header("Hit Statistics")]
    public int perfectHits;
    public int goodHits;
    public int okHits;
    public int missedHits;
    public int totalNotes;
    
    [Header("Metadata")]
    public float songDuration;
    public System.DateTime completionTime;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            completionTime = System.DateTime.Now;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public static void CreateResults(string songName, string artist, string difficulty, 
                                   int score, float accuracy, float completion,
                                   int perfect, int good, int ok, int missed, int total)
    {
        GameObject resultsObj = new GameObject("GameResults");
        GameResultsData results = resultsObj.AddComponent<GameResultsData>();
        
        results.songName = songName;
        results.artist = artist;
        results.difficulty = difficulty;
        results.score = score;
        results.accuracy = accuracy;
        results.completionPercentage = completion;
        results.perfectHits = perfect;
        results.goodHits = good;
        results.okHits = ok;
        results.missedHits = missed;
        results.totalNotes = total;
    }
}
