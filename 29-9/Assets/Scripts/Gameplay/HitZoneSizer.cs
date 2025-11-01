using UnityEngine;

[System.Serializable]
public class HitZonePreset
{
    public string name;
    public float width;
    public float height;
    public float visualScale;
    public string description;
}

public class HitZoneSizer : MonoBehaviour
{
    [Header("Hit Zone Size Presets")]
    public HitZonePreset[] presets = new HitZonePreset[]
    {
        new HitZonePreset 
        { 
            name = "Fortnite Style", 
            width = 0.4f, 
            height = 0.3f, 
            visualScale = 0.8f,
            description = "Compact hit zones similar to Fortnite Festival" 
        },
        new HitZonePreset 
        { 
            name = "Clone Hero", 
            width = 0.6f, 
            height = 0.6f, 
            visualScale = 1.0f,
            description = "Standard Clone Hero hit zone size" 
        },
        new HitZonePreset 
        { 
            name = "Guitar Hero", 
            width = 0.5f, 
            height = 0.4f, 
            visualScale = 0.9f,
            description = "Classic Guitar Hero style" 
        },
        new HitZonePreset 
        { 
            name = "Compact", 
            width = 0.35f, 
            height = 0.25f, 
            visualScale = 0.7f,
            description = "Very small and precise hit zones" 
        },
        new HitZonePreset 
        { 
            name = "Large", 
            width = 0.8f, 
            height = 0.8f, 
            visualScale = 1.2f,
            description = "Large and forgiving hit zones" 
        }
    };
    
    [Header("Current Configuration")]
    [SerializeField] private int selectedPresetIndex = 0;
    [SerializeField] private HitZonePreset customPreset = new HitZonePreset 
    { 
        name = "Custom", 
        width = 0.4f, 
        height = 0.3f, 
        visualScale = 0.8f,
        description = "Custom configuration" 
    };
    
    [Header("Advanced Settings")]
    public bool matchNoteSize = true;
    public float noteSizeMultiplier = 1.0f;
    public bool autoApplyOnStart = true;
    
    [Header("Visual Feedback")]
    public bool showGizmos = true;
    public Color gizmoColor = Color.yellow;
    
    private HitZone hitZone;
    
    void Start()
    {
        hitZone = GetComponent<HitZone>();
        if (hitZone == null)
        {
            hitZone = FindFirstObjectByType<HitZone>();
        }
        
        if (autoApplyOnStart)
        {
            ApplyPreset(selectedPresetIndex);
        }
    }
    
    [ContextMenu("Apply Selected Preset")]
    public void ApplySelectedPreset()
    {
        ApplyPreset(selectedPresetIndex);
    }
    
    public void ApplyPreset(int presetIndex)
    {
        if (presetIndex < 0 || presetIndex >= presets.Length)
        {
            Debug.LogWarning($"Invalid preset index: {presetIndex}");
            return;
        }
        
        HitZonePreset preset = presets[presetIndex];
        ApplyPresetSettings(preset);
        
        Debug.Log($"Applied hit zone preset: {preset.name} - {preset.description}");
    }
    
    public void ApplyCustomPreset()
    {
        ApplyPresetSettings(customPreset);
        Debug.Log($"Applied custom hit zone settings");
    }
    
    private void ApplyPresetSettings(HitZonePreset preset)
    {
        if (hitZone == null)
        {
            Debug.LogError("No HitZone component found!");
            return;
        }
        
        // Apply size settings
        hitZone.hitZoneWidth = preset.width;
        hitZone.hitZoneHeight = preset.height;
        
        // Apply visual scaling to indicators
        ApplyVisualScaling(preset.visualScale);
        
        // If matching note size, adjust based on note size
        if (matchNoteSize)
        {
            AdjustToMatchNoteSize();
        }
        
        // Update hit zone setup
        hitZone.SendMessage("SetupVisualFeedback", SendMessageOptions.DontRequireReceiver);
    }
    
    private void ApplyVisualScaling(float scale)
    {
        if (hitZone.laneIndicators == null) return;
        
        foreach (GameObject indicator in hitZone.laneIndicators)
        {
            if (indicator != null)
            {
                Vector3 newScale = Vector3.one * scale;
                indicator.transform.localScale = newScale;
            }
        }
    }
    
    private void AdjustToMatchNoteSize()
    {
        // Find a note prefab or existing note to match size
        Note sampleNote = FindFirstObjectByType<Note>();
        if (sampleNote != null)
        {
            Vector3 noteScale = sampleNote.transform.localScale;
            float noteSize = Mathf.Max(noteScale.x, noteScale.y);
            
            hitZone.hitZoneWidth = noteSize * noteSizeMultiplier;
            hitZone.hitZoneHeight = noteSize * noteSizeMultiplier;
            
            Debug.Log($"Adjusted hit zone size to match note size: {noteSize}");
        }
    }
    
    [ContextMenu("Apply Fortnite Style")]
    public void ApplyFortniteStyle()
    {
        ApplyPreset(0); // Fortnite Style is index 0
    }
    
    [ContextMenu("Apply Clone Hero Style")]
    public void ApplyCloneHeroStyle()
    {
        ApplyPreset(1); // Clone Hero is index 1
    }
    
    [ContextMenu("Apply Compact Style")]
    public void ApplyCompactStyle()
    {
        ApplyPreset(3); // Compact is index 3
    }
    
    // Method to create a new custom preset
    public void CreateCustomPreset(string name, float width, float height, float visualScale, string description = "")
    {
        customPreset = new HitZonePreset
        {
            name = name,
            width = width,
            height = height,
            visualScale = visualScale,
            description = description
        };
        
        ApplyCustomPreset();
    }
    
    // Get current preset info
    public HitZonePreset GetCurrentPreset()
    {
        if (selectedPresetIndex >= 0 && selectedPresetIndex < presets.Length)
        {
            return presets[selectedPresetIndex];
        }
        return customPreset;
    }
    
    // Validation method
    void OnValidate()
    {
        // Clamp preset index
        selectedPresetIndex = Mathf.Clamp(selectedPresetIndex, 0, presets.Length - 1);
        
        // Validate custom preset values
        customPreset.width = Mathf.Clamp(customPreset.width, 0.1f, 2.0f);
        customPreset.height = Mathf.Clamp(customPreset.height, 0.1f, 2.0f);
        customPreset.visualScale = Mathf.Clamp(customPreset.visualScale, 0.1f, 3.0f);
        
        noteSizeMultiplier = Mathf.Clamp(noteSizeMultiplier, 0.1f, 3.0f);
    }
    
    void OnDrawGizmos()
    {
        if (!showGizmos || hitZone == null || hitZone.lanePositions == null) return;
        
        Gizmos.color = gizmoColor;
        
        HitZonePreset currentPreset = GetCurrentPreset();
        
        for (int i = 0; i < hitZone.lanePositions.Length; i++)
        {
            if (hitZone.lanePositions[i] != null)
            {
                Vector3 pos = hitZone.lanePositions[i].position;
                Vector3 size = new Vector3(currentPreset.width, currentPreset.height, 0.1f);
                
                // Draw wire cube for hit zone
                Gizmos.DrawWireCube(pos, size);
                
                // Draw filled cube with transparency
                Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 0.2f);
                Gizmos.DrawCube(pos, size);
                Gizmos.color = gizmoColor;
            }
        }
    }
}
