using UnityEngine;

/// <summary>
/// Automatically sets up the highway with proper Fortnite-style configuration
/// This script should be added to the main highway GameObject
/// </summary>
public class HighwayAutoSetup : MonoBehaviour
{
    [Header("Auto Setup Configuration")]
    public bool setupOnStart = true;
    public bool forceReconfigure = false;
    
    [Header("Fortnite-Style Settings")]
    [Range(0.4f, 1.0f)]
    public float laneSpacing = 0.8f;
    [Range(0.3f, 0.8f)]
    public float buttonSize = 0.6f;
    [Range(2f, 6f)]
    public float highwayWidth = 4f;
    
    [Header("Component References")]
    public HighwaySetup highwaySetup;
    public HitZoneIndicators hitZoneIndicators;
    public HitZone hitZone;
    public InputManager inputManager;
    
    void Start()
    {
        if (setupOnStart)
        {
            AutoConfigureHighway();
        }
    }
    
    [ContextMenu("Auto Configure Highway")]
    public void AutoConfigureHighway()
    {
        Debug.Log("🎸 Starting Fortnite-style highway auto-configuration...");
        
        FindComponents();
        ConfigureHighwaySetup();
        ConfigureHitZoneIndicators();
        ConfigureHitZone();
        ConfigureInputManager();
        ValidateConfiguration();
        
        Debug.Log("✅ Highway auto-configuration complete!");
    }
    
    void FindComponents()
    {
        // Auto-find components if not assigned
        if (highwaySetup == null)
            highwaySetup = FindFirstObjectByType<HighwaySetup>();
        
        if (hitZoneIndicators == null)
            hitZoneIndicators = FindFirstObjectByType<HitZoneIndicators>();
        
        if (hitZone == null)
            hitZone = FindFirstObjectByType<HitZone>();
        
        if (inputManager == null)
            inputManager = FindFirstObjectByType<InputManager>();
        
        Debug.Log($"🔍 Found components - Highway: {highwaySetup != null}, Indicators: {hitZoneIndicators != null}, HitZone: {hitZone != null}, Input: {inputManager != null}");
    }
    
    void ConfigureHighwaySetup()
    {
        if (highwaySetup == null) return;
        
        // Apply Fortnite-style settings
        highwaySetup.laneSpacing = laneSpacing;
        highwaySetup.highwayWidth = highwayWidth;
        highwaySetup.hitZoneWidth = buttonSize;
        
        // Ensure colors are vibrant like Fortnite
        highwaySetup.laneColors = new Color[]
        {
            new Color(0.2f, 1f, 0.2f, 1f),    // Bright Green
            new Color(1f, 0.2f, 0.2f, 1f),    // Bright Red
            new Color(1f, 1f, 0.2f, 1f),      // Bright Yellow
            new Color(0.2f, 0.5f, 1f, 1f),    // Bright Blue
            new Color(1f, 0.6f, 0.2f, 1f)     // Bright Orange
        };
        
        Debug.Log($"⚙️ Configured HighwaySetup - Spacing: {laneSpacing}, Width: {highwayWidth}");
    }
    
    void ConfigureHitZoneIndicators()
    {
        if (hitZoneIndicators == null) return;
        
        // Set small button size to match notes
        hitZoneIndicators.indicatorSize = buttonSize;
        
        // Ensure key labels match InputManager
        hitZoneIndicators.keyLabels = new string[] { "D", "F", "J", "K", "L" };
        
        // Match colors with highway setup
        hitZoneIndicators.laneColors = new Color[]
        {
            new Color(0.2f, 1f, 0.2f, 1f),    // Bright Green
            new Color(1f, 0.2f, 0.2f, 1f),    // Bright Red
            new Color(1f, 1f, 0.2f, 1f),      // Bright Yellow
            new Color(0.2f, 0.5f, 1f, 1f),    // Bright Blue
            new Color(1f, 0.6f, 0.2f, 1f)     // Bright Orange
        };
        
        Debug.Log($"🎯 Configured HitZoneIndicators - Size: {buttonSize}");
    }
    
    void ConfigureHitZone()
    {
        if (hitZone == null) return;
        
        // Set hit zone dimensions to match button size
        hitZone.hitZoneHeight = buttonSize;
        hitZone.hitZoneWidth = buttonSize;
        
        // Optimize hit detection windows for better gameplay
        hitZone.perfectWindow = 0.04f;  // Tighter perfect window
        hitZone.greatWindow = 0.07f;    // Tighter great window
        hitZone.goodWindow = 0.10f;     // Reasonable good window
        hitZone.okWindow = 0.13f;       // OK window
        hitZone.missWindow = 0.16f;     // Miss window
        
        Debug.Log($"🎯 Configured HitZone - Dimensions: {buttonSize}x{buttonSize}");
    }
    
    void ConfigureInputManager()
    {
        if (inputManager == null) return;
        
        // Ensure correct key bindings (Fortnite style)
        inputManager.laneKeys = new KeyCode[]
        {
            KeyCode.D, KeyCode.F, KeyCode.J, KeyCode.K, KeyCode.L
        };
        
        Debug.Log("⌨️ Configured InputManager - Keys: D, F, J, K, L");
    }
    
    void ValidateConfiguration()
    {
        bool isValid = true;
        
        // Check if all components are properly configured
        if (highwaySetup != null && Mathf.Abs(highwaySetup.laneSpacing - laneSpacing) > 0.01f)
        {
            Debug.LogWarning("⚠️ HighwaySetup lane spacing mismatch");
            isValid = false;
        }
        
        if (hitZoneIndicators != null && Mathf.Abs(hitZoneIndicators.indicatorSize - buttonSize) > 0.01f)
        {
            Debug.LogWarning("⚠️ HitZoneIndicators size mismatch");
            isValid = false;
        }
        
        if (hitZone != null && Mathf.Abs(hitZone.hitZoneWidth - buttonSize) > 0.01f)
        {
            Debug.LogWarning("⚠️ HitZone size mismatch");
            isValid = false;
        }
        
        if (inputManager != null)
        {
            KeyCode[] expectedKeys = { KeyCode.D, KeyCode.F, KeyCode.J, KeyCode.K, KeyCode.L };
            for (int i = 0; i < expectedKeys.Length && i < inputManager.laneKeys.Length; i++)
            {
                if (inputManager.laneKeys[i] != expectedKeys[i])
                {
                    Debug.LogWarning($"⚠️ InputManager key mismatch at index {i}");
                    isValid = false;
                    break;
                }
            }
        }
        
        if (isValid)
        {
            Debug.Log("✅ All components are properly configured for Fortnite-style gameplay!");
        }
        else
        {
            Debug.LogWarning("⚠️ Some components may need manual adjustment");
        }
    }
    
    /// <summary>
    /// Get recommended component assignments for Unity Inspector
    /// </summary>
    [ContextMenu("Show Component Assignment Guide")]
    public void ShowComponentAssignmentGuide()
    {
        Debug.Log("📋 COMPONENT ASSIGNMENT GUIDE:");
        Debug.Log("1. Add HighwayAutoSetup to your main Highway GameObject");
        Debug.Log("2. Assign HighwaySetup component to 'Highway Setup' field");
        Debug.Log("3. Assign HitZoneIndicators component to 'Hit Zone Indicators' field");
        Debug.Log("4. Assign HitZone component to 'Hit Zone' field");
        Debug.Log("5. Assign InputManager component to 'Input Manager' field");
        Debug.Log("6. Click 'Auto Configure Highway' button or enable 'Setup On Start'");
        Debug.Log("7. Test in play mode - buttons should be small and aligned with notes");
    }
}
