using UnityEngine;

/// <summary>
/// Integrates the textured hit zone buttons with the existing hit zone system
/// </summary>
public class HitZoneButtonIntegrator : MonoBehaviour
{
    [Header("Components")]
    public TexturedHitZoneIndicators texturedIndicators;
    public HitZone hitZone;
    public InputManager inputManager;
    
    void Start()
    {
        // Find components if not assigned
        if (texturedIndicators == null)
            texturedIndicators = FindFirstObjectByType<TexturedHitZoneIndicators>();
            
        if (hitZone == null)
            hitZone = FindFirstObjectByType<HitZone>();
            
        if (inputManager == null)
            inputManager = FindFirstObjectByType<InputManager>();
            
        // Subscribe to input events if available
        SetupInputIntegration();
    }
    
    void SetupInputIntegration()
    {
        // This method can be expanded when InputManager has events
        // For now, the TexturedHitZoneIndicators handles input directly
    }
    
    /// <summary>
    /// Call this method when a note is hit to trigger the button effect
    /// </summary>
    public void OnNoteHit(int laneIndex)
    {
        if (texturedIndicators != null)
        {
            texturedIndicators.TriggerHitEffect(laneIndex);
        }
    }
    
    /// <summary>
    /// Call this method when a key is pressed
    /// </summary>
    public void OnKeyPressed(int laneIndex)
    {
        // This can be used for additional effects when keys are pressed
        // The visual feedback is already handled in TexturedHitZoneIndicators
    }
    
    /// <summary>
    /// Call this method when a key is released
    /// </summary>
    public void OnKeyReleased(int laneIndex)
    {
        // This can be used for additional effects when keys are released
        // The visual feedback is already handled in TexturedHitZoneIndicators
    }
}
