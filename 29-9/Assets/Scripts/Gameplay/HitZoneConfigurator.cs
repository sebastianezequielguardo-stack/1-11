using UnityEngine;

[System.Serializable]
public class HitZoneStyle
{
    [Header("Size Configuration")]
    [Range(0.1f, 1.0f)]
    public float width = 0.4f;
    
    [Range(0.1f, 1.0f)]
    public float height = 0.3f;
    
    [Header("Visual Style")]
    [Range(0.5f, 2.0f)]
    public float indicatorScale = 0.8f;
    
    [Range(0.5f, 3.0f)]
    public float glowIntensity = 1.0f;
    
    [Header("Hit Windows (seconds)")]
    [Range(0.02f, 0.1f)]
    public float perfectWindow = 0.04f;
    
    [Range(0.05f, 0.15f)]
    public float greatWindow = 0.07f;
    
    [Range(0.08f, 0.2f)]
    public float goodWindow = 0.1f;
    
    [Range(0.1f, 0.25f)]
    public float okWindow = 0.13f;
    
    [Range(0.12f, 0.3f)]
    public float missWindow = 0.16f;
}

public class HitZoneConfigurator : MonoBehaviour
{
    [Header("Quick Style Selection")]
    [SerializeField] private StylePreset currentStyle = StylePreset.Fortnite;
    
    [Header("Custom Configuration")]
    public HitZoneStyle customStyle = new HitZoneStyle();
    
    [Header("Auto Configuration")]
    public bool autoConfigureOnStart = true;
    public bool matchNoteSize = true;
    public bool syncWithNoteSpeed = true;
    
    [Header("Real-time Preview")]
    public bool livePreview = true;
    
    private HitZone hitZone;
    private HitZoneStyle lastAppliedStyle;
    
    public enum StylePreset
    {
        Fortnite,
        CloneHero,
        GuitarHero,
        RockBand,
        Compact,
        Large,
        Custom
    }
    
    void Start()
    {
        hitZone = GetComponent<HitZone>();
        if (hitZone == null)
        {
            hitZone = FindFirstObjectByType<HitZone>();
        }
        
        if (autoConfigureOnStart)
        {
            ApplyCurrentStyle();
        }
    }
    
    void Update()
    {
        if (livePreview && HasStyleChanged())
        {
            ApplyCurrentStyle();
        }
    }
    
    private bool HasStyleChanged()
    {
        HitZoneStyle current = GetCurrentStyle();
        if (lastAppliedStyle == null) return true;
        
        return !StylesAreEqual(current, lastAppliedStyle);
    }
    
    private bool StylesAreEqual(HitZoneStyle a, HitZoneStyle b)
    {
        return Mathf.Approximately(a.width, b.width) &&
               Mathf.Approximately(a.height, b.height) &&
               Mathf.Approximately(a.indicatorScale, b.indicatorScale) &&
               Mathf.Approximately(a.perfectWindow, b.perfectWindow) &&
               Mathf.Approximately(a.greatWindow, b.greatWindow) &&
               Mathf.Approximately(a.goodWindow, b.goodWindow) &&
               Mathf.Approximately(a.okWindow, b.okWindow) &&
               Mathf.Approximately(a.missWindow, b.missWindow);
    }
    
    [ContextMenu("Apply Current Style")]
    public void ApplyCurrentStyle()
    {
        if (hitZone == null) return;
        
        HitZoneStyle style = GetCurrentStyle();
        ApplyStyle(style);
        
        lastAppliedStyle = CloneStyle(style);
    }
    
    private HitZoneStyle CloneStyle(HitZoneStyle original)
    {
        return new HitZoneStyle
        {
            width = original.width,
            height = original.height,
            indicatorScale = original.indicatorScale,
            glowIntensity = original.glowIntensity,
            perfectWindow = original.perfectWindow,
            greatWindow = original.greatWindow,
            goodWindow = original.goodWindow,
            okWindow = original.okWindow,
            missWindow = original.missWindow
        };
    }
    
    private void ApplyStyle(HitZoneStyle style)
    {
        // Apply size
        hitZone.hitZoneWidth = style.width;
        hitZone.hitZoneHeight = style.height;
        
        // Apply hit windows
        hitZone.perfectWindow = style.perfectWindow;
        hitZone.greatWindow = style.greatWindow;
        hitZone.goodWindow = style.goodWindow;
        hitZone.okWindow = style.okWindow;
        hitZone.missWindow = style.missWindow;
        
        // Apply visual scaling
        ApplyIndicatorScaling(style.indicatorScale);
        
        // Update visual feedback
        hitZone.UpdateVisualFeedback();
        
        Debug.Log($"Applied {currentStyle} style to hit zones");
    }
    
    private void ApplyIndicatorScaling(float scale)
    {
        if (hitZone.laneIndicators == null) return;
        
        foreach (GameObject indicator in hitZone.laneIndicators)
        {
            if (indicator != null)
            {
                indicator.transform.localScale = Vector3.one * scale;
            }
        }
    }
    
    private HitZoneStyle GetCurrentStyle()
    {
        switch (currentStyle)
        {
            case StylePreset.Fortnite:
                return new HitZoneStyle
                {
                    width = 0.35f,
                    height = 0.25f,
                    indicatorScale = 0.7f,
                    perfectWindow = 0.04f,
                    greatWindow = 0.07f,
                    goodWindow = 0.1f,
                    okWindow = 0.13f,
                    missWindow = 0.16f
                };
                
            case StylePreset.CloneHero:
                return new HitZoneStyle
                {
                    width = 0.6f,
                    height = 0.6f,
                    indicatorScale = 1.0f,
                    perfectWindow = 0.05f,
                    greatWindow = 0.08f,
                    goodWindow = 0.1f,
                    okWindow = 0.12f,
                    missWindow = 0.15f
                };
                
            case StylePreset.GuitarHero:
                return new HitZoneStyle
                {
                    width = 0.5f,
                    height = 0.4f,
                    indicatorScale = 0.9f,
                    perfectWindow = 0.045f,
                    greatWindow = 0.075f,
                    goodWindow = 0.095f,
                    okWindow = 0.115f,
                    missWindow = 0.14f
                };
                
            case StylePreset.RockBand:
                return new HitZoneStyle
                {
                    width = 0.55f,
                    height = 0.45f,
                    indicatorScale = 0.95f,
                    perfectWindow = 0.05f,
                    greatWindow = 0.08f,
                    goodWindow = 0.105f,
                    okWindow = 0.125f,
                    missWindow = 0.15f
                };
                
            case StylePreset.Compact:
                return new HitZoneStyle
                {
                    width = 0.3f,
                    height = 0.2f,
                    indicatorScale = 0.6f,
                    perfectWindow = 0.03f,
                    greatWindow = 0.06f,
                    goodWindow = 0.08f,
                    okWindow = 0.1f,
                    missWindow = 0.12f
                };
                
            case StylePreset.Large:
                return new HitZoneStyle
                {
                    width = 0.8f,
                    height = 0.8f,
                    indicatorScale = 1.3f,
                    perfectWindow = 0.07f,
                    greatWindow = 0.1f,
                    goodWindow = 0.13f,
                    okWindow = 0.16f,
                    missWindow = 0.2f
                };
                
            case StylePreset.Custom:
            default:
                return customStyle;
        }
    }
    
    // Quick preset methods
    [ContextMenu("Apply Fortnite Style")]
    public void SetFortniteStyle()
    {
        currentStyle = StylePreset.Fortnite;
        ApplyCurrentStyle();
    }
    
    [ContextMenu("Apply Clone Hero Style")]
    public void SetCloneHeroStyle()
    {
        currentStyle = StylePreset.CloneHero;
        ApplyCurrentStyle();
    }
    
    [ContextMenu("Apply Compact Style")]
    public void SetCompactStyle()
    {
        currentStyle = StylePreset.Compact;
        ApplyCurrentStyle();
    }
    
    // Utility methods
    public void SetCustomSize(float width, float height)
    {
        customStyle.width = Mathf.Clamp(width, 0.1f, 1.0f);
        customStyle.height = Mathf.Clamp(height, 0.1f, 1.0f);
        
        if (currentStyle == StylePreset.Custom)
        {
            ApplyCurrentStyle();
        }
    }
    
    public void SetCustomHitWindows(float perfect, float great, float good, float ok, float miss)
    {
        customStyle.perfectWindow = perfect;
        customStyle.greatWindow = great;
        customStyle.goodWindow = good;
        customStyle.okWindow = ok;
        customStyle.missWindow = miss;
        
        if (currentStyle == StylePreset.Custom)
        {
            ApplyCurrentStyle();
        }
    }
    
    void OnValidate()
    {
        // Ensure hit windows are in correct order
        customStyle.perfectWindow = Mathf.Clamp(customStyle.perfectWindow, 0.02f, 0.1f);
        customStyle.greatWindow = Mathf.Max(customStyle.greatWindow, customStyle.perfectWindow);
        customStyle.goodWindow = Mathf.Max(customStyle.goodWindow, customStyle.greatWindow);
        customStyle.okWindow = Mathf.Max(customStyle.okWindow, customStyle.goodWindow);
        customStyle.missWindow = Mathf.Max(customStyle.missWindow, customStyle.okWindow);
    }
}
