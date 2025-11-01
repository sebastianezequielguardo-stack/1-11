using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the FACIL and DIFICIL difficulty buttons in the main menu
/// Ensures only one can be selected at a time and they start unselected
/// </summary>
public class DifficultyButtonManager : MonoBehaviour
{
    [Header("Difficulty Buttons")]
    public Button facilButton;
    public Button dificilButton;
    
    [Header("Visual Settings")]
    [SerializeField] private Color selectedColor = new Color(0f, 0.8f, 0f, 1f); // Verde brillante
    [SerializeField] private Color unselectedColor = new Color(0.9f, 0.9f, 0.9f, 1f); // Blanco grisáceo
    [SerializeField] private Color hoverColor = new Color(0.7f, 0.7f, 0.7f, 1f); // Gris claro
    [SerializeField] private Color playButtonEnabledColor = new Color(0f, 0.8f, 0f, 1f); // Verde para JUGAR
    [SerializeField] private Color playButtonDisabledColor = new Color(0.5f, 0.5f, 0.5f, 1f); // Gris para JUGAR
    
    [Header("Play Button")]
    public Button playButton;
    
    private string selectedDifficulty = "";
    private bool isDifficultySelected = false;
    
    void Start()
    {
        InitializeButtons();
        SetupButtonListeners();
        
        // Ensure play button starts in correct state (gray, since no difficulty selected initially)
        UpdatePlayButtonState();
        
        Debug.Log("🎯 DifficultyButtonManager initialized - Play button should be gray until both song and difficulty are selected");
    }
    
    void LateUpdate()
    {
        // Continuously enforce correct play button state to override any other scripts
        if (playButton != null)
        {
            bool songSelected = GameManager.Instance != null && GameManager.Instance.songSelected;
            bool difficultySelected = isDifficultySelected;
            bool shouldBeEnabled = songSelected && difficultySelected;
            
            // If the button state doesn't match what it should be, fix it
            if (playButton.interactable != shouldBeEnabled)
            {
                UpdatePlayButtonState();
                Debug.Log($"🔧 Play button state corrected in LateUpdate - Should be enabled: {shouldBeEnabled}");
            }
        }
    }
    
    void InitializeButtons()
    {
        // Set both buttons to unselected state initially
        SetButtonColor(facilButton, unselectedColor);
        SetButtonColor(dificilButton, unselectedColor);
        
        Debug.Log("🎯 Difficulty buttons initialized - both unselected");
    }
    
    void SetupButtonListeners()
    {
        if (facilButton != null)
        {
            facilButton.onClick.AddListener(() => SelectDifficulty("Facil", facilButton));
        }
        
        if (dificilButton != null)
        {
            dificilButton.onClick.AddListener(() => SelectDifficulty("Dificil", dificilButton));
        }
    }
    
    void SelectDifficulty(string difficulty, Button selectedButton)
    {
        // Update internal state
        selectedDifficulty = difficulty;
        isDifficultySelected = true;
        
        // Update visual state - set selected button to green, other to white
        if (difficulty == "Facil")
        {
            SetButtonColor(facilButton, selectedColor);
            SetButtonColor(dificilButton, unselectedColor);
        }
        else if (difficulty == "Dificil")
        {
            SetButtonColor(dificilButton, selectedColor);
            SetButtonColor(facilButton, unselectedColor);
        }
        
        // Update GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SelectDifficulty(difficulty);
        }
        
        // Update play button state
        UpdatePlayButtonState();
        
        Debug.Log($"🎯 Difficulty selected: {difficulty}");
    }
    
    void SetButtonColor(Button button, Color color)
    {
        if (button == null) return;
        
        ColorBlock colors = button.colors;
        colors.normalColor = color;
        colors.highlightedColor = Color.Lerp(color, hoverColor, 0.3f);
        colors.pressedColor = Color.Lerp(color, Color.black, 0.2f);
        colors.selectedColor = color;
        button.colors = colors;
    }
    
    void UpdatePlayButtonState()
    {
        if (playButton == null) return;
        
        // CRITICAL: Both conditions must be true - song selected AND difficulty selected
        bool songSelected = GameManager.Instance != null && GameManager.Instance.songSelected;
        bool difficultySelected = isDifficultySelected;
        bool canPlay = songSelected && difficultySelected;
        
        playButton.interactable = canPlay;
        
        // Visual feedback for play button
        ColorBlock playColors = playButton.colors;
        if (canPlay)
        {
            // Only green when BOTH song and difficulty are selected
            playColors.normalColor = playButtonEnabledColor;
            playColors.highlightedColor = Color.Lerp(playButtonEnabledColor, Color.white, 0.3f);
            playColors.pressedColor = Color.Lerp(playButtonEnabledColor, Color.black, 0.2f);
            playColors.selectedColor = playButtonEnabledColor;
        }
        else
        {
            // Gray when either song OR difficulty is missing
            playColors.normalColor = playButtonDisabledColor;
            playColors.highlightedColor = playButtonDisabledColor;
            playColors.pressedColor = playButtonDisabledColor;
            playColors.selectedColor = playButtonDisabledColor;
        }
        playButton.colors = playColors;
        
        // Debug information
        Debug.Log($"🎮 Play Button State - Song: {songSelected}, Difficulty: {difficultySelected}, Can Play: {canPlay}");
    }
    
    // Public methods for external access
    public string GetSelectedDifficulty()
    {
        return selectedDifficulty;
    }
    
    public bool IsDifficultySelected()
    {
        return isDifficultySelected;
    }
    
    public void ResetSelection()
    {
        selectedDifficulty = "";
        isDifficultySelected = false;
        
        SetButtonColor(facilButton, unselectedColor);
        SetButtonColor(dificilButton, unselectedColor);
        
        // Force update play button to gray since no difficulty is selected
        UpdatePlayButtonState();
        
        Debug.Log("🎯 Difficulty selection reset - Play button should be gray");
    }
    
    /// <summary>
    /// Force reset all button colors to correct values - call this if colors look wrong
    /// </summary>
    [ContextMenu("Fix Button Colors")]
    public void FixButtonColors()
    {
        // Reset difficulty buttons to unselected
        SetButtonColor(facilButton, unselectedColor);
        SetButtonColor(dificilButton, unselectedColor);
        
        // Reset selection state
        selectedDifficulty = "";
        isDifficultySelected = false;
        
        // Force correct play button state
        UpdatePlayButtonState();
        
        Debug.Log("🎨 All button colors have been reset to correct values");
    }
    
    // Subscribe to GameManager events
    void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnSongSelected += OnSongSelected;
            GameManager.Instance.OnPlayButtonStateChanged += OnPlayButtonStateChanged;
        }
    }
    
    void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnSongSelected -= OnSongSelected;
            GameManager.Instance.OnPlayButtonStateChanged -= OnPlayButtonStateChanged;
        }
    }
    
    void OnSongSelected(string songPath)
    {
        // Force update with correct logic when song is selected
        UpdatePlayButtonState();
    }
    
    void OnPlayButtonStateChanged()
    {
        // Override any other play button state changes with our correct logic
        UpdatePlayButtonState();
        Debug.Log("🎮 Play button state overridden by DifficultyButtonManager");
    }
}
