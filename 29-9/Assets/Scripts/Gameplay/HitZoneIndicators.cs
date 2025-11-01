using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Creates visual indicators for the hit zone buttons/keys
/// </summary>
public class HitZoneIndicators : MonoBehaviour
{
    [Header("Hit Zone Settings")]
    public Transform[] lanePositions; // Positions for each lane indicator
    public GameObject buttonIndicatorPrefab; // Prefab for button indicators
    
    [Header("Visual Settings")]
    public float indicatorSize = 0.6f; // Much smaller to match notes exactly
    public float glowIntensity = 0.5f;
    public Color[] laneColors = {
        Color.green,                    // Lane 0 - Green
        Color.red,                      // Lane 1 - Red  
        Color.yellow,                   // Lane 2 - Yellow
        Color.blue,                     // Lane 3 - Blue
        new Color(1f, 0.5f, 0f, 1f)   // Lane 4 - Orange
    };
    
    [Header("Key Bindings Display")]
    public bool showKeyLabels = false; // Desactivado para evitar letras en pantalla
    public string[] keyLabels = { "D", "F", "J", "K", "L" }; // Match InputManager key bindings
    
    private GameObject[] indicators = new GameObject[5];
    private Renderer[] indicatorRenderers = new Renderer[5];
    private TextMesh[] keyTexts = new TextMesh[5];
    
    void Start()
    {
        CreateHitZoneIndicators();
        SetupInputListeners();
    }
    
    void CreateHitZoneIndicators()
    {
        for (int i = 0; i < 5; i++)
        {
            if (i < lanePositions.Length && lanePositions[i] != null)
            {
                // Create indicator at lane position
                Vector3 indicatorPos = lanePositions[i].position;
                
                GameObject indicator;
                if (buttonIndicatorPrefab != null)
                {
                    indicator = Instantiate(buttonIndicatorPrefab, indicatorPos, Quaternion.identity);
                }
                else
                {
                    // Create default indicator
                    indicator = CreateDefaultIndicator(indicatorPos, i);
                }
                
                indicator.name = $"HitZone_Lane_{i}";
                indicator.transform.parent = transform;
                
                indicators[i] = indicator;
                indicatorRenderers[i] = indicator.GetComponent<Renderer>();
                
                // Set up color
                if (indicatorRenderers[i] != null && i < laneColors.Length)
                {
                    Material mat = indicatorRenderers[i].material;
                    mat.color = laneColors[i];
                    
                    // Make it slightly transparent
                    Color color = mat.color;
                    color.a = 0.8f;
                    mat.color = color;
                }
                
                // Add key label only if enabled
                if (showKeyLabels)
                {
                    CreateKeyLabel(indicator, i);
                }
            }
        }
    }
    
    GameObject CreateDefaultIndicator(Vector3 position, int laneIndex)
    {
        // Create a cylinder as default indicator
        GameObject indicator = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        indicator.transform.position = position;
        indicator.transform.localScale = new Vector3(indicatorSize, 0.1f, indicatorSize);
        
        // Remove collider
        Collider collider = indicator.GetComponent<Collider>();
        if (collider != null)
        {
            DestroyImmediate(collider);
        }
        
        // Set up material for glow effect
        Renderer renderer = indicator.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material mat = renderer.material;
            mat.shader = Shader.Find("Standard");
            mat.SetFloat("_Mode", 3); // Transparent mode
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = 3000;
            
            // Add emission for glow
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", laneColors[laneIndex] * glowIntensity);
        }
        
        return indicator;
    }
    
    void CreateKeyLabel(GameObject indicator, int laneIndex)
    {
        if (laneIndex >= keyLabels.Length) return;
        
        // Create text object for key label
        GameObject textObj = new GameObject($"KeyLabel_{laneIndex}");
        textObj.transform.parent = indicator.transform;
        textObj.transform.localPosition = new Vector3(0, 0.2f, 0);
        textObj.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        
        TextMesh textMesh = textObj.AddComponent<TextMesh>();
        textMesh.text = keyLabels[laneIndex];
        textMesh.fontSize = 20;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.color = Color.white;
        textMesh.fontStyle = FontStyle.Bold;
        
        // Set sorting order to appear on top
        MeshRenderer textRenderer = textObj.GetComponent<MeshRenderer>();
        if (textRenderer != null)
        {
            textRenderer.sortingOrder = 10;
        }
        
        keyTexts[laneIndex] = textMesh;
    }
    
    void SetupInputListeners()
    {
        // Subscribe to input events if InputManager exists
        InputManager inputManager = FindFirstObjectByType<InputManager>();
        if (inputManager != null)
        {
            // This would need to be implemented in InputManager
            // inputManager.OnKeyPressed += OnKeyPressed;
            // inputManager.OnKeyReleased += OnKeyReleased;
        }
    }
    
    void Update()
    {
        // Handle input feedback manually for now
        HandleInputFeedback();
    }
    
    void HandleInputFeedback()
    {
        // Check for key presses and provide visual feedback (match InputManager)
        KeyCode[] keys = { KeyCode.D, KeyCode.F, KeyCode.J, KeyCode.K, KeyCode.L };
        
        for (int i = 0; i < keys.Length && i < indicators.Length; i++)
        {
            if (indicators[i] != null && indicatorRenderers[i] != null)
            {
                bool keyPressed = Input.GetKey(keys[i]);
                
                // Scale effect when key is pressed (smaller scale change)
                float targetScale = keyPressed ? indicatorSize * 1.1f : indicatorSize;
                Vector3 currentScale = indicators[i].transform.localScale;
                currentScale.x = Mathf.Lerp(currentScale.x, targetScale, Time.deltaTime * 10f);
                currentScale.z = Mathf.Lerp(currentScale.z, targetScale, Time.deltaTime * 10f);
                indicators[i].transform.localScale = currentScale;
                
                // Brightness effect when key is pressed
                Material mat = indicatorRenderers[i].material;
                Color baseColor = laneColors[i];
                Color targetColor = keyPressed ? baseColor * 1.5f : baseColor;
                targetColor.a = 0.8f;
                mat.color = Color.Lerp(mat.color, targetColor, Time.deltaTime * 15f);
                
                // Emission effect
                Color emissionColor = keyPressed ? baseColor * glowIntensity * 2f : baseColor * glowIntensity;
                mat.SetColor("_EmissionColor", emissionColor);
            }
        }
    }
    
    /// <summary>
    /// Trigger hit effect for a specific lane
    /// </summary>
    public void TriggerHitEffect(int laneIndex)
    {
        if (laneIndex >= 0 && laneIndex < indicators.Length && indicators[laneIndex] != null)
        {
            StartCoroutine(HitEffectCoroutine(laneIndex));
        }
    }
    
    System.Collections.IEnumerator HitEffectCoroutine(int laneIndex)
    {
        if (indicatorRenderers[laneIndex] == null) yield break;
        
        Material mat = indicatorRenderers[laneIndex].material;
        Color originalColor = mat.color;
        Color originalEmission = mat.GetColor("_EmissionColor");
        
        // Flash effect
        Color flashColor = Color.white;
        flashColor.a = 0.9f;
        mat.color = flashColor;
        mat.SetColor("_EmissionColor", laneColors[laneIndex] * glowIntensity * 3f);
        
        yield return new WaitForSeconds(0.1f);
        
        // Return to normal
        mat.color = originalColor;
        mat.SetColor("_EmissionColor", originalEmission);
    }
    
    void OnDestroy()
    {
        // Clean up any event subscriptions
    }
}
