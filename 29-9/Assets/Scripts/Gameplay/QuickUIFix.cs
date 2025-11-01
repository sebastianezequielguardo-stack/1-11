using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Quick fix to move UI elements to the right side
/// Add this to any GameObject and press Space in play mode to fix UI
/// </summary>
public class QuickUIFix : MonoBehaviour
{
    void Update()
    {
        // Press Space to fix UI layout
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FixUILayout();
        }
    }
    
    void FixUILayout()
    {
        Debug.Log("🔧 Fixing UI layout...");
        
        // Find and fix Score Text
        GameObject scoreObj = GameObject.Find("ScoreText");
        if (scoreObj == null) scoreObj = FindObjectWithText("Score:");
        if (scoreObj != null)
        {
            MoveToTopRight(scoreObj.GetComponent<RectTransform>(), -20f, -20f);
            Debug.Log("✅ Score text moved to top-right");
        }
        
        // Find and fix Combo Text
        GameObject comboObj = GameObject.Find("ComboText");
        if (comboObj == null) comboObj = FindObjectWithText("Combo:");
        if (comboObj != null)
        {
            MoveToTopRight(comboObj.GetComponent<RectTransform>(), -20f, -60f);
            Debug.Log("✅ Combo text moved below score");
        }
        
        // Find and fix Progress Slider
        Slider[] sliders = FindObjectsByType<Slider>(FindObjectsSortMode.None);
        foreach (Slider slider in sliders)
        {
            if (slider.name.Contains("Progress") || slider.name.Contains("Song"))
            {
                MoveSliderToRight(slider.GetComponent<RectTransform>(), -20f, -100f);
                Debug.Log("✅ Progress slider moved to right side");
                break;
            }
        }
        
        // Find and fix Time Text
        GameObject timeObj = GameObject.Find("TimeText");
        if (timeObj == null) timeObj = FindObjectWithText(":");
        if (timeObj != null)
        {
            MoveToTopRight(timeObj.GetComponent<RectTransform>(), -20f, -140f);
            Debug.Log("✅ Time text moved below slider");
        }
        
        Debug.Log("🎨 UI layout fixed! Elements moved to right side.");
    }
    
    GameObject FindObjectWithText(string searchText)
    {
        TextMeshProUGUI[] texts = FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None);
        foreach (TextMeshProUGUI text in texts)
        {
            if (text.text.Contains(searchText))
            {
                return text.gameObject;
            }
        }
        return null;
    }
    
    void MoveToTopRight(RectTransform rectTransform, float x, float y)
    {
        if (rectTransform == null) return;
        
        rectTransform.anchorMin = new Vector2(1f, 1f);
        rectTransform.anchorMax = new Vector2(1f, 1f);
        rectTransform.pivot = new Vector2(1f, 1f);
        rectTransform.anchoredPosition = new Vector2(x, y);
        
        // Set text alignment if it's a text component
        TextMeshProUGUI textComponent = rectTransform.GetComponent<TextMeshProUGUI>();
        if (textComponent != null)
        {
            textComponent.alignment = TextAlignmentOptions.TopRight;
        }
    }
    
    void MoveSliderToRight(RectTransform rectTransform, float x, float y)
    {
        if (rectTransform == null) return;
        
        rectTransform.anchorMin = new Vector2(1f, 1f);
        rectTransform.anchorMax = new Vector2(1f, 1f);
        rectTransform.pivot = new Vector2(1f, 1f);
        rectTransform.anchoredPosition = new Vector2(x, y);
        rectTransform.sizeDelta = new Vector2(200f, 20f);
    }
    
    void Start()
    {
        Debug.Log("🎮 QuickUIFix ready! Press SPACE in play mode to fix UI layout");
    }
}
