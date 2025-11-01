using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Utility script to fix button colors when they appear wrong
/// Use this as a temporary solution to reset all button colors
/// </summary>
public class ButtonColorFixer : MonoBehaviour
{
    [Header("Buttons to Fix")]
    public Button facilButton;
    public Button dificilButton;
    public Button playButton;
    
    [Header("Correct Colors")]
    public Color difficultyUnselectedColor = new Color(0.9f, 0.9f, 0.9f, 1f); // Blanco grisáceo
    public Color difficultySelectedColor = new Color(0f, 0.8f, 0f, 1f); // Verde brillante
    public Color playButtonDisabledColor = new Color(0.5f, 0.5f, 0.5f, 1f); // Gris
    public Color playButtonEnabledColor = new Color(0f, 0.8f, 0f, 1f); // Verde
    
    [ContextMenu("Fix All Button Colors Now")]
    public void FixAllButtonColors()
    {
        // Fix difficulty buttons to unselected state
        if (facilButton != null)
        {
            SetButtonColor(facilButton, difficultyUnselectedColor);
            Debug.Log("✅ FACIL button color fixed");
        }
        
        if (dificilButton != null)
        {
            SetButtonColor(dificilButton, difficultyUnselectedColor);
            Debug.Log("✅ DIFICIL button color fixed");
        }
        
        // Fix play button to disabled state
        if (playButton != null)
        {
            SetButtonColor(playButton, playButtonDisabledColor);
            playButton.interactable = false;
            Debug.Log("✅ JUGAR button color fixed");
        }
        
        Debug.Log("🎨 All button colors have been corrected!");
    }
    
    void SetButtonColor(Button button, Color color)
    {
        if (button == null) return;
        
        ColorBlock colors = button.colors;
        colors.normalColor = color;
        colors.highlightedColor = Color.Lerp(color, Color.white, 0.2f);
        colors.pressedColor = Color.Lerp(color, Color.black, 0.2f);
        colors.selectedColor = color;
        colors.disabledColor = Color.Lerp(color, Color.gray, 0.5f);
        button.colors = colors;
    }
    
    void Start()
    {
        // Automatically fix colors when the script starts
        FixAllButtonColors();
    }
}
