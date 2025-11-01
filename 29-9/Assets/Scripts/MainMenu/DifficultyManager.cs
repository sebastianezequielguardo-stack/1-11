using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class DifficultyManager : MonoBehaviour
{
    [Header("Difficulty Buttons")]
    public Button easyButton;
    public Button mediumButton;
    public Button hardButton;
    public Button expertButton;
    
    [Header("Visual Feedback")]
    public Color selectedColor = Color.green;
    public Color unselectedColor = Color.white;
    public Color disabledColor = Color.gray;
    
    [Header("Difficulty Info")]
    public TextMeshProUGUI difficultyInfoText;
    public GameObject difficultyInfoPanel;
    
    private Dictionary<string, Button> difficultyButtons = new Dictionary<string, Button>();
    private Dictionary<string, DifficultyInfo> difficultyInfos = new Dictionary<string, DifficultyInfo>();
    private string currentSelectedDifficulty = "Medium";
    
    void Start()
    {
        InitializeDifficultyButtons();
        InitializeDifficultyInfos();
        SetupButtonListeners();
    }
    
    void InitializeDifficultyButtons()
    {
        // Map buttons to difficulty names
        if (easyButton != null) difficultyButtons["Easy"] = easyButton;
        if (mediumButton != null) difficultyButtons["Medium"] = mediumButton;
        if (hardButton != null) difficultyButtons["Hard"] = hardButton;
        if (expertButton != null) difficultyButtons["Expert"] = expertButton;
    }
    
    void InitializeDifficultyInfos()
    {
        difficultyInfos["Easy"] = new DifficultyInfo
        {
            name = "Easy",
            description = "Perfect for beginners. Slower note speed and simpler patterns.",
            noteSpeed = 3f,
            chartTags = new[] { "[EasySingle]", "[EasyGuitar]", "[Single]" }
        };
        
        difficultyInfos["Medium"] = new DifficultyInfo
        {
            name = "Medium", 
            description = "Moderate challenge with standard note speed.",
            noteSpeed = 5f,
            chartTags = new[] { "[MediumSingle]", "[MediumGuitar]", "[Single]" }
        };
        
        difficultyInfos["Hard"] = new DifficultyInfo
        {
            name = "Hard",
            description = "Fast-paced gameplay for experienced players.",
            noteSpeed = 7f,
            chartTags = new[] { "[HardSingle]", "[HardGuitar]", "[ExpertSingle]" }
        };
        
        difficultyInfos["Expert"] = new DifficultyInfo
        {
            name = "Expert",
            description = "Maximum challenge! Lightning-fast notes and complex patterns.",
            noteSpeed = 9f,
            chartTags = new[] { "[ExpertSingle]", "[ExpertGuitar]", "[HardSingle]" }
        };
    }
    
    void SetupButtonListeners()
    {
        foreach (var kvp in difficultyButtons)
        {
            string difficulty = kvp.Key;
            Button button = kvp.Value;
            
            if (button != null)
            {
                button.onClick.AddListener(() => SetDifficulty(difficulty));
                
                // Set initial color
                ColorBlock colors = button.colors;
                colors.normalColor = unselectedColor;
                colors.highlightedColor = Color.Lerp(unselectedColor, Color.white, 0.2f);
                button.colors = colors;
            }
        }
        
        // Set default difficulty
        if (difficultyButtons.ContainsKey("Medium"))
        {
            SetDifficulty("Medium");
        }
        else if (difficultyButtons.Count > 0)
        {
            var enumerator = difficultyButtons.Keys.GetEnumerator();
            if (enumerator.MoveNext())
            {
                SetDifficulty(enumerator.Current);
            }
        }
    }

    void SetDifficulty(string difficulty)
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("❌ [DifficultyManager] GameManager.Instance es null.");
            return;
        }
        
        // Update visual feedback
        UpdateButtonVisuals(difficulty);
        
        // Update game manager
        currentSelectedDifficulty = difficulty;
        GameManager.Instance.selectedDifficulty = GetLegacyDifficultyName(difficulty);
        
        // Try to update play button state if method exists
        var gameManagerType = GameManager.Instance.GetType();
        var updateMethod = gameManagerType.GetMethod("UpdatePlayButtonState");
        if (updateMethod != null)
        {
            updateMethod.Invoke(GameManager.Instance, null);
        }
        
        // Update info display
        UpdateDifficultyInfo();
        
        Debug.Log($"🎯 Dificultad seleccionada: {difficulty}");
    }
    
    void UpdateButtonVisuals(string selectedDifficulty)
    {
        foreach (var kvp in difficultyButtons)
        {
            string difficulty = kvp.Key;
            Button button = kvp.Value;
            
            if (button == null) continue;
            
            ColorBlock colors = button.colors;
            
            if (difficulty == selectedDifficulty)
            {
                colors.normalColor = selectedColor;
                colors.highlightedColor = Color.Lerp(selectedColor, Color.white, 0.3f);
            }
            else
            {
                colors.normalColor = unselectedColor;
                colors.highlightedColor = Color.Lerp(unselectedColor, Color.white, 0.2f);
            }
            
            button.colors = colors;
        }
    }
    
    void UpdateDifficultyInfo()
    {
        if (difficultyInfoText == null || string.IsNullOrEmpty(currentSelectedDifficulty)) return;
        
        if (difficultyInfos.ContainsKey(currentSelectedDifficulty))
        {
            DifficultyInfo info = difficultyInfos[currentSelectedDifficulty];
            difficultyInfoText.text = $"<b>{info.name}</b>\n{info.description}\nNote Speed: {info.noteSpeed}";
        }
    }
    
    // Convert new difficulty names to legacy format for compatibility
    string GetLegacyDifficultyName(string newDifficulty)
    {
        switch (newDifficulty)
        {
            case "Easy": return "Facil";
            case "Medium": return "Medium";
            case "Hard": return "Dificil";
            case "Expert": return "Expert";
            default: return "Medium";
        }
    }
    
    // Public method to get current difficulty info
    public DifficultyInfo GetCurrentDifficultyInfo()
    {
        if (difficultyInfos.ContainsKey(currentSelectedDifficulty))
        {
            return difficultyInfos[currentSelectedDifficulty];
        }
        return difficultyInfos["Medium"]; // Default fallback
    }
    
    // Public method to check if a difficulty is available
    public bool IsDifficultyAvailable(string difficulty)
    {
        return difficultyButtons.ContainsKey(difficulty) && difficultyButtons[difficulty] != null;
    }
    
    // Public method to enable/disable specific difficulties
    public void SetDifficultyEnabled(string difficulty, bool enabled)
    {
        if (difficultyButtons.ContainsKey(difficulty) && difficultyButtons[difficulty] != null)
        {
            Button button = difficultyButtons[difficulty];
            button.interactable = enabled;
            
            ColorBlock colors = button.colors;
            colors.normalColor = enabled ? unselectedColor : disabledColor;
            button.colors = colors;
        }
    }
    
    // Public method to show/hide difficulty info panel
    public void ShowDifficultyInfo(bool show)
    {
        if (difficultyInfoPanel != null)
        {
            difficultyInfoPanel.SetActive(show);
        }
    }
    
    // Public method to get current selected difficulty
    public string GetCurrentDifficulty()
    {
        return currentSelectedDifficulty;
    }
    
    // Public method to get legacy difficulty name
    public string GetCurrentLegacyDifficulty()
    {
        return GetLegacyDifficultyName(currentSelectedDifficulty);
    }
    
    // Public method to set difficulty programmatically
    public void SetDifficultyProgrammatically(string difficulty)
    {
        if (difficultyButtons.ContainsKey(difficulty))
        {
            SetDifficulty(difficulty);
        }
        else
        {
            Debug.LogWarning($"DifficultyManager: Difficulty '{difficulty}' not found");
        }
    }
}

[System.Serializable]
public class DifficultyInfo
{
    public string name;
    public string description;
    public float noteSpeed;
    public string[] chartTags;
    
    public DifficultyInfo()
    {
        chartTags = new string[0];
    }
}
