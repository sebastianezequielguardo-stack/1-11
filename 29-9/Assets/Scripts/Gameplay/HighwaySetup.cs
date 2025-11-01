using UnityEngine;

/// <summary>
/// Sets up the 3D highway with proper lane spacing and visual elements
/// </summary>
public class HighwaySetup : MonoBehaviour
{
    [Header("Highway Configuration")]
    public Transform[] laneTransforms = new Transform[5]; // Lane positions
    public float laneSpacing = 0.8f; // Distance between lanes (much tighter spacing like Fortnite)
    public float highwayLength = 50f; // Length of the highway
    public float highwayWidth = 4f; // Total width of the highway (much narrower)
    
    [Header("Visual Elements")]
    public GameObject laneDividerPrefab; // Prefab for lane dividers
    public GameObject hitZoneIndicatorPrefab; // Prefab for hit zone indicators
    public Material highwayMaterial; // Material for the highway surface
    
    [Header("Hit Zone")]
    public float hitZoneZ = -8f; // Z position of the hit zone (moved further back to match detection)
    public float hitZoneWidth = 0.6f; // Width of each hit zone indicator (smaller to match notes)
    
    [Header("Colors")]
    public Color[] laneColors = {
        Color.green,                    // Lane 0 - Green
        Color.red,                      // Lane 1 - Red  
        Color.yellow,                   // Lane 2 - Yellow
        Color.blue,                     // Lane 3 - Blue
        new Color(1f, 0.5f, 0f, 1f)   // Lane 4 - Orange
    };
    
    private GameObject[] hitZoneIndicators = new GameObject[5];
    private GameObject[] laneDividers = new GameObject[6]; // 6 dividers for 5 lanes
    
    void Start()
    {
        SetupHighway();
        CreateLanePositions();
        CreateHitZoneIndicators();
        // CreateLaneDividers(); // Desactivado - no queremos lane dividers
    }
    
    void SetupHighway()
    {
        // Don't create automatic highway surface - let user configure manually
        // This prevents unwanted gray elements
        Debug.Log("🛣️ HighwaySetup: Configure highway surface manually in Unity if needed");
    }
    
    void CreateLanePositions()
    {
        // Calculate lane positions centered around the highway
        float startX = -(laneSpacing * 2f); // Start position for 5 lanes
        
        for (int i = 0; i < 5; i++)
        {
            if (laneTransforms[i] == null)
            {
                // Create lane transform if it doesn't exist
                GameObject laneObj = new GameObject($"Lane_{i}");
                laneObj.transform.parent = transform;
                laneTransforms[i] = laneObj.transform;
            }
            
            // Position the lane
            float laneX = startX + (i * laneSpacing);
            laneTransforms[i].position = new Vector3(laneX, 0f, 0f);
            
            Debug.Log($"🛣️ Lane {i} positioned at X: {laneX}");
        }
    }
    
    void CreateHitZoneIndicators()
    {
        for (int i = 0; i < 5; i++)
        {
            if (laneTransforms[i] != null)
            {
                Vector3 hitZonePos = new Vector3(
                    laneTransforms[i].position.x, 
                    0.05f, 
                    hitZoneZ
                );
                
                GameObject indicator;
                if (hitZoneIndicatorPrefab != null)
                {
                    indicator = Instantiate(hitZoneIndicatorPrefab, hitZonePos, Quaternion.identity);
                }
                else
                {
                    indicator = CreateDefaultHitZoneIndicator(hitZonePos, i);
                }
                
                indicator.name = $"HitZone_Lane_{i}";
                indicator.transform.parent = transform;
                hitZoneIndicators[i] = indicator;
                
                // Set color
                Renderer renderer = indicator.GetComponent<Renderer>();
                if (renderer != null && i < laneColors.Length)
                {
                    Material mat = renderer.material;
                    mat.color = laneColors[i];
                    
                    // Add emission for visibility
                    mat.EnableKeyword("_EMISSION");
                    mat.SetColor("_EmissionColor", laneColors[i] * 0.3f);
                }
            }
        }
    }
    
    GameObject CreateDefaultHitZoneIndicator(Vector3 position, int laneIndex)
    {
        GameObject indicator = GameObject.CreatePrimitive(PrimitiveType.Cube);
        indicator.transform.position = position;
        indicator.transform.localScale = new Vector3(0.6f, 0.1f, 0.4f); // Much smaller to match note size exactly
        
        // Remove collider
        Collider collider = indicator.GetComponent<Collider>();
        if (collider != null)
        {
            DestroyImmediate(collider);
        }
        
        // Set up material with bright colors and emission
        Renderer renderer = indicator.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material mat = new Material(Shader.Find("Standard"));
            
            // Bright, solid colors
            Color color = laneColors[laneIndex];
            mat.color = color;
            
            // Add emission for glow effect
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", color * 0.5f);
            
            // Make it metallic for better visibility
            mat.SetFloat("_Metallic", 0.3f);
            mat.SetFloat("_Smoothness", 0.8f);
            
            renderer.material = mat;
        }
        
        // No text labels - just the colored button indicators
        
        return indicator;
    }
    
    
    void CreateLaneDividers()
    {
        // Create only inner dividers between lanes (not on the sides)
        float startX = -(laneSpacing * 2f);
        
        for (int i = 1; i < 5; i++) // Only 4 inner dividers between 5 lanes
        {
            float dividerX = startX + (i * laneSpacing) - (laneSpacing / 2f);
            Vector3 dividerPos = new Vector3(dividerX, 0.01f, highwayLength / 2f);
            
            GameObject divider;
            if (laneDividerPrefab != null)
            {
                divider = Instantiate(laneDividerPrefab, dividerPos, Quaternion.identity);
            }
            else
            {
                divider = CreateDefaultLaneDivider(dividerPos);
            }
            
            divider.name = $"LaneDivider_{i}";
            divider.transform.parent = transform;
            laneDividers[i-1] = divider; // Adjust index
        }
    }
    
    GameObject CreateDefaultLaneDivider(Vector3 position)
    {
        GameObject divider = GameObject.CreatePrimitive(PrimitiveType.Cube);
        divider.transform.position = position;
        divider.transform.localScale = new Vector3(0.02f, 0.005f, highwayLength); // Thinner dividers
        
        // Remove collider
        Collider collider = divider.GetComponent<Collider>();
        if (collider != null)
        {
            DestroyImmediate(collider);
        }
        
        // Set subtle white color for dividers
        Renderer renderer = divider.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = new Color(0.8f, 0.8f, 0.8f, 0.6f); // Subtle gray-white
            mat.SetFloat("_Metallic", 0f);
            mat.SetFloat("_Smoothness", 0.2f);
            renderer.material = mat;
        }
        
        return divider;
    }
    
    /// <summary>
    /// Get the transform for a specific lane
    /// </summary>
    public Transform GetLaneTransform(int laneIndex)
    {
        if (laneIndex >= 0 && laneIndex < laneTransforms.Length)
        {
            return laneTransforms[laneIndex];
        }
        return null;
    }
    
    /// <summary>
    /// Get all lane transforms
    /// </summary>
    public Transform[] GetAllLaneTransforms()
    {
        return laneTransforms;
    }
    
    /// <summary>
    /// Trigger hit effect for a specific lane
    /// </summary>
    public void TriggerLaneHitEffect(int laneIndex)
    {
        if (laneIndex >= 0 && laneIndex < hitZoneIndicators.Length && hitZoneIndicators[laneIndex] != null)
        {
            StartCoroutine(HitEffectCoroutine(laneIndex));
        }
    }
    
    System.Collections.IEnumerator HitEffectCoroutine(int laneIndex)
    {
        GameObject indicator = hitZoneIndicators[laneIndex];
        if (indicator == null) yield break;
        
        Renderer renderer = indicator.GetComponent<Renderer>();
        if (renderer == null) yield break;
        
        Material mat = renderer.material;
        Color originalColor = mat.color;
        Vector3 originalScale = indicator.transform.localScale;
        
        // Flash and scale effect
        Color flashColor = Color.white;
        flashColor.a = 0.9f;
        mat.color = flashColor;
        indicator.transform.localScale = originalScale * 1.3f;
        
        yield return new WaitForSeconds(0.1f);
        
        // Return to normal
        mat.color = originalColor;
        indicator.transform.localScale = originalScale;
    }
    
    void OnDrawGizmosSelected()
    {
        // Draw lane positions in editor
        Gizmos.color = Color.yellow;
        float startX = -(laneSpacing * 2f);
        
        for (int i = 0; i < 5; i++)
        {
            float laneX = startX + (i * laneSpacing);
            Vector3 lanePos = new Vector3(laneX, 0f, 0f);
            
            // Draw lane line
            Gizmos.DrawLine(lanePos + Vector3.forward * 25f, lanePos - Vector3.forward * 25f);
            
            // Draw hit zone
            Gizmos.color = laneColors[i];
            Vector3 hitZonePos = new Vector3(laneX, 0f, hitZoneZ);
            Gizmos.DrawCube(hitZonePos, new Vector3(hitZoneWidth, 0.1f, 0.5f));
            Gizmos.color = Color.yellow;
        }
        
        // Draw highway bounds
        Gizmos.color = Color.white;
        Vector3 center = new Vector3(0f, 0f, highwayLength / 2f);
        Gizmos.DrawWireCube(center, new Vector3(highwayWidth, 0.1f, highwayLength));
    }
}
