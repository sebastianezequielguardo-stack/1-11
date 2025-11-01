using UnityEngine;

/// <summary>
/// Ensures perfect alignment between highway lanes, hit zone indicators, and note positions
/// This script synchronizes all highway-related components to match Fortnite-style layout
/// </summary>
public class HighwayAlignment : MonoBehaviour
{
    [Header("Highway Components")]
    public HighwaySetup highwaySetup;
    public HitZoneIndicators hitZoneIndicators;
    public HitZone hitZone;
    
    [Header("Alignment Settings")]
    [Range(0.4f, 1.2f)]
    public float laneSpacing = 0.8f; // Tight spacing like Fortnite
    [Range(0.3f, 1.0f)]
    public float buttonSize = 0.6f; // Small buttons to match notes
    [Range(0.3f, 1.0f)]
    public float noteSize = 0.6f; // Note size reference
    
    [Header("Visual Adjustments")]
    public bool autoAlign = true;
    public bool showDebugInfo = true;
    
    void Start()
    {
        if (autoAlign)
        {
            AlignHighwayComponents();
        }
    }
    
    void Update()
    {
        // Allow real-time adjustments in editor
        if (Application.isEditor && autoAlign)
        {
            AlignHighwayComponents();
        }
    }
    
    [ContextMenu("Align Highway Components")]
    public void AlignHighwayComponents()
    {
        SyncHighwaySetup();
        SyncHitZoneIndicators();
        SyncHitZone();
        
        if (showDebugInfo)
        {
            Debug.Log($"🎸 Highway aligned - Lane Spacing: {laneSpacing}, Button Size: {buttonSize}");
        }
    }
    
    void SyncHighwaySetup()
    {
        if (highwaySetup != null)
        {
            highwaySetup.laneSpacing = laneSpacing;
            highwaySetup.hitZoneWidth = buttonSize;
            highwaySetup.highwayWidth = laneSpacing * 4f + buttonSize; // Calculated width
            
            // Force recreation of lane positions
            if (Application.isPlaying)
            {
                highwaySetup.SendMessage("CreateLanePositions", SendMessageOptions.DontRequireReceiver);
                highwaySetup.SendMessage("CreateHitZoneIndicators", SendMessageOptions.DontRequireReceiver);
            }
        }
    }
    
    void SyncHitZoneIndicators()
    {
        if (hitZoneIndicators != null)
        {
            hitZoneIndicators.indicatorSize = buttonSize;
            
            // Update key labels to match InputManager
            string[] correctKeys = { "D", "F", "J", "K", "L" };
            hitZoneIndicators.keyLabels = correctKeys;
        }
    }
    
    void SyncHitZone()
    {
        if (hitZone != null)
        {
            hitZone.hitZoneHeight = buttonSize;
            hitZone.hitZoneWidth = buttonSize;
        }
    }
    
    /// <summary>
    /// Get the calculated lane positions based on current spacing
    /// </summary>
    public Vector3[] GetCalculatedLanePositions()
    {
        Vector3[] positions = new Vector3[5];
        float startX = -(laneSpacing * 2f); // Center the 5 lanes
        
        for (int i = 0; i < 5; i++)
        {
            float laneX = startX + (i * laneSpacing);
            positions[i] = new Vector3(laneX, 0f, 0f);
        }
        
        return positions;
    }
    
    /// <summary>
    /// Check if all components are properly aligned
    /// </summary>
    public bool ValidateAlignment()
    {
        bool isValid = true;
        
        if (highwaySetup != null)
        {
            if (Mathf.Abs(highwaySetup.laneSpacing - laneSpacing) > 0.01f)
            {
                Debug.LogWarning("⚠️ HighwaySetup lane spacing mismatch");
                isValid = false;
            }
        }
        
        if (hitZoneIndicators != null)
        {
            if (Mathf.Abs(hitZoneIndicators.indicatorSize - buttonSize) > 0.01f)
            {
                Debug.LogWarning("⚠️ HitZoneIndicators size mismatch");
                isValid = false;
            }
        }
        
        if (hitZone != null)
        {
            if (Mathf.Abs(hitZone.hitZoneWidth - buttonSize) > 0.01f)
            {
                Debug.LogWarning("⚠️ HitZone size mismatch");
                isValid = false;
            }
        }
        
        if (isValid && showDebugInfo)
        {
            Debug.Log("✅ All highway components are properly aligned");
        }
        
        return isValid;
    }
    
    void OnDrawGizmosSelected()
    {
        // Draw lane positions and hit zones for visualization
        Vector3[] lanePositions = GetCalculatedLanePositions();
        
        for (int i = 0; i < lanePositions.Length; i++)
        {
            // Draw lane line
            Gizmos.color = Color.yellow;
            Vector3 lanePos = lanePositions[i];
            Gizmos.DrawLine(lanePos + Vector3.forward * 25f, lanePos - Vector3.forward * 25f);
            
            // Draw hit zone area
            Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
            Vector3 hitZonePos = new Vector3(lanePos.x, 0f, -5f);
            Gizmos.DrawCube(hitZonePos, new Vector3(buttonSize, 0.1f, 0.5f));
            
            // Draw button indicator area
            Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
            Gizmos.DrawWireCube(hitZonePos, new Vector3(buttonSize, 0.2f, 0.6f));
        }
        
        // Draw highway bounds
        Gizmos.color = Color.white;
        float calculatedWidth = laneSpacing * 4f + buttonSize;
        Vector3 center = new Vector3(0f, 0f, 25f);
        Gizmos.DrawWireCube(center, new Vector3(calculatedWidth, 0.1f, 50f));
    }
}
