using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Forces UI elements to be positioned correctly on the right side
/// </summary>
public class UILayoutManager : MonoBehaviour
{
    [Header("UI Elements to Reposition")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;
    public Slider songProgressSlider;
    public TextMeshProUGUI timeRemainingText;
    
    [Header("Layout Settings")]
    public float rightMargin = 20f;
    public float topMargin = 20f;
    public float elementSpacing = 40f;
    
    void Start()
    {
        // Wait a frame then force layout
        Invoke("ForceUILayout", 0.1f);
    }
    
    void ForceUILayout()
    {
        RepositionUIElements();
        Debug.Log("🎨 UI Layout forced to right side");
    }
    
    void RepositionUIElements()
    {
        float currentY = -topMargin;
        
        // Position Score Text
        if (scoreText != null)
        {
            PositionElement(scoreText.rectTransform, currentY);
            currentY -= elementSpacing;
            Debug.Log("📊 Score text repositioned");
        }
        
        // Position Combo Text
        if (comboText != null)
        {
            PositionElement(comboText.rectTransform, currentY);
            currentY -= elementSpacing;
            Debug.Log("🔥 Combo text repositioned");
        }
        
        // Position Progress Slider
        if (songProgressSlider != null)
        {
            PositionSlider(songProgressSlider.GetComponent<RectTransform>(), currentY);
            currentY -= elementSpacing;
            Debug.Log("⏳ Progress slider repositioned");
        }
        
        // Position Time Text
        if (timeRemainingText != null)
        {
            PositionElement(timeRemainingText.rectTransform, currentY);
            Debug.Log("⏰ Time text repositioned");
        }
    }
    
    void PositionElement(RectTransform rectTransform, float yPosition)
    {
        if (rectTransform == null) return;
        
        // Set anchors to top-right
        rectTransform.anchorMin = new Vector2(1f, 1f);
        rectTransform.anchorMax = new Vector2(1f, 1f);
        rectTransform.pivot = new Vector2(1f, 1f);
        
        // Set position
        rectTransform.anchoredPosition = new Vector2(-rightMargin, yPosition);
        
        // Set text alignment if it's a text component
        TextMeshProUGUI textComponent = rectTransform.GetComponent<TextMeshProUGUI>();
        if (textComponent != null)
        {
            textComponent.alignment = TextAlignmentOptions.TopRight;
        }
    }
    
    void PositionSlider(RectTransform rectTransform, float yPosition)
    {
        if (rectTransform == null) return;
        
        // Set anchors to top-right
        rectTransform.anchorMin = new Vector2(1f, 1f);
        rectTransform.anchorMax = new Vector2(1f, 1f);
        rectTransform.pivot = new Vector2(1f, 1f);
        
        // Set position and size
        rectTransform.anchoredPosition = new Vector2(-rightMargin, yPosition);
        rectTransform.sizeDelta = new Vector2(200f, 20f);
    }
    
    /// <summary>
    /// Call this to manually refresh the layout
    /// </summary>
    [ContextMenu("Refresh UI Layout")]
    public void RefreshLayout()
    {
        RepositionUIElements();
    }
    
    /// <summary>
    /// Auto-find UI elements in the scene
    /// </summary>
    [ContextMenu("Auto Find UI Elements")]
    public void AutoFindUIElements()
    {
        // Find score text
        if (scoreText == null)
        {
            GameObject scoreObj = GameObject.Find("ScoreText");
            if (scoreObj != null)
                scoreText = scoreObj.GetComponent<TextMeshProUGUI>();
        }
        
        // Find combo text
        if (comboText == null)
        {
            GameObject comboObj = GameObject.Find("ComboText");
            if (comboObj != null)
                comboText = comboObj.GetComponent<TextMeshProUGUI>();
        }
        
        // Find progress slider
        if (songProgressSlider == null)
        {
            GameObject sliderObj = GameObject.Find("ProgressSlider");
            if (sliderObj != null)
                songProgressSlider = sliderObj.GetComponent<Slider>();
        }
        
        // Find time text
        if (timeRemainingText == null)
        {
            GameObject timeObj = GameObject.Find("TimeText");
            if (timeObj != null)
                timeRemainingText = timeObj.GetComponent<TextMeshProUGUI>();
        }
        
        Debug.Log("🔍 Auto-found UI elements");
        RefreshLayout();
    }
}
