using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Creates textured visual indicators for the hit zone buttons using 2D sprites in 3D space
/// </summary>
public class TexturedHitZoneIndicators : MonoBehaviour
{
    [Header("Hit Zone Settings")]
    public Transform[] lanePositions; // Positions for each lane indicator
    
    [Header("Visual Settings")]
    public float indicatorSize = 1.2f; // Size of the button indicators
    public float buttonDepth = 0.1f; // Thickness of the 3D button
    
    [Header("Button Textures")]
    public Texture2D[] baseButtonTextures = new Texture2D[5]; // Base button textures
    public Texture2D[] pressedButtonTextures = new Texture2D[5]; // Pressed button textures
    
    [Header("Key Bindings Display")]
    public bool showKeyLabels = false;
    public string[] keyLabels = { "D", "F", "J", "K", "L" };
    
    private GameObject[] indicators = new GameObject[5];
    private Renderer[] indicatorRenderers = new Renderer[5];
    private Material[] baseMaterials = new Material[5];
    private Material[] pressedMaterials = new Material[5];
    private TextMesh[] keyTexts = new TextMesh[5];
    
    // Button texture names in Resources folder
    private string[] baseTextureNames = {
        "boton verde base",
        "boton rojo base", 
        "boton amarillo base",
        "boton azul base",
        "boton rosa base"
    };
    
    private string[] pressedTextureNames = {
        "boton verde pulsado",
        "boton rojo pulsado",
        "boton amarillo pulsado", 
        "boton azul pulsado",
        "boton rosa pulsado"
    };
    
    void Start()
    {
        LoadButtonTextures();
        CreateTexturedHitZoneIndicators();
        SetupInputListeners();
    }
    
    void LoadButtonTextures()
    {
        // Initialize arrays if not already done
        if (baseButtonTextures == null || baseButtonTextures.Length != 5)
        {
            baseButtonTextures = new Texture2D[5];
        }
        if (pressedButtonTextures == null || pressedButtonTextures.Length != 5)
        {
            pressedButtonTextures = new Texture2D[5];
        }
        
        // Load textures from Resources folder
        for (int i = 0; i < 5; i++)
        {
            if (i < baseTextureNames.Length)
            {
                baseButtonTextures[i] = Resources.Load<Texture2D>(baseTextureNames[i]);
                if (baseButtonTextures[i] == null)
                {
                    Debug.LogWarning($"Could not load base texture: {baseTextureNames[i]}");
                }
            }
            
            if (i < pressedTextureNames.Length)
            {
                pressedButtonTextures[i] = Resources.Load<Texture2D>(pressedTextureNames[i]);
                if (pressedButtonTextures[i] == null)
                {
                    Debug.LogWarning($"Could not load pressed texture: {pressedTextureNames[i]}");
                }
            }
        }
    }
    
    void CreateTexturedHitZoneIndicators()
    {
        for (int i = 0; i < 5; i++)
        {
            if (i < lanePositions.Length && lanePositions[i] != null)
            {
                // Create textured indicator at lane position
                Vector3 indicatorPos = lanePositions[i].position;
                GameObject indicator = CreateTexturedIndicator(indicatorPos, i);
                
                indicator.name = $"HitZone_Lane_{i}";
                indicator.transform.parent = transform;
                
                indicators[i] = indicator;
                indicatorRenderers[i] = indicator.GetComponent<Renderer>();
                
                // Create materials for base and pressed states
                CreateButtonMaterials(i);
                
                // Set initial material to base state
                if (indicatorRenderers[i] != null && baseMaterials[i] != null)
                {
                    indicatorRenderers[i].material = baseMaterials[i];
                }
                
                // Add key label only if enabled
                if (showKeyLabels)
                {
                    CreateKeyLabel(indicator, i);
                }
            }
        }
    }
    
    GameObject CreateTexturedIndicator(Vector3 position, int laneIndex)
    {
        // Create a cylinder as the base shape for the button
        GameObject indicator = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        indicator.transform.position = position;
        indicator.transform.localScale = new Vector3(indicatorSize, buttonDepth, indicatorSize);
        
        // Remove collider as it's not needed for visual indicators
        Collider collider = indicator.GetComponent<Collider>();
        if (collider != null)
        {
            DestroyImmediate(collider);
        }
        
        return indicator;
    }
    
    void CreateButtonMaterials(int laneIndex)
    {
        // Validate index bounds
        if (laneIndex < 0 || laneIndex >= 5) return;
        
        // Create base material
        if (baseButtonTextures != null && laneIndex < baseButtonTextures.Length && baseButtonTextures[laneIndex] != null)
        {
            baseMaterials[laneIndex] = new Material(Shader.Find("Standard"));
            baseMaterials[laneIndex].mainTexture = baseButtonTextures[laneIndex];
            baseMaterials[laneIndex].SetFloat("_Mode", 0); // Opaque mode
            baseMaterials[laneIndex].EnableKeyword("_EMISSION");
            baseMaterials[laneIndex].SetColor("_EmissionColor", Color.black);
        }
        else
        {
            // Create fallback material if texture is missing
            baseMaterials[laneIndex] = new Material(Shader.Find("Standard"));
            baseMaterials[laneIndex].color = GetFallbackColor(laneIndex);
        }
        
        // Create pressed material
        if (pressedButtonTextures != null && laneIndex < pressedButtonTextures.Length && pressedButtonTextures[laneIndex] != null)
        {
            pressedMaterials[laneIndex] = new Material(Shader.Find("Standard"));
            pressedMaterials[laneIndex].mainTexture = pressedButtonTextures[laneIndex];
            pressedMaterials[laneIndex].SetFloat("_Mode", 0); // Opaque mode
            pressedMaterials[laneIndex].EnableKeyword("_EMISSION");
            pressedMaterials[laneIndex].SetColor("_EmissionColor", Color.white * 0.2f); // Slight glow when pressed
        }
        else
        {
            // Create fallback pressed material
            pressedMaterials[laneIndex] = new Material(Shader.Find("Standard"));
            pressedMaterials[laneIndex].color = GetFallbackColor(laneIndex) * 1.5f;
            pressedMaterials[laneIndex].EnableKeyword("_EMISSION");
            pressedMaterials[laneIndex].SetColor("_EmissionColor", Color.white * 0.2f);
        }
    }
    
    Color GetFallbackColor(int laneIndex)
    {
        // Fallback colors matching the original system
        Color[] fallbackColors = {
            Color.green,                    // Lane 0 - Green
            Color.red,                      // Lane 1 - Red  
            Color.yellow,                   // Lane 2 - Yellow
            Color.blue,                     // Lane 3 - Blue
            new Color(1f, 0.5f, 1f, 1f)   // Lane 4 - Pink
        };
        
        if (laneIndex >= 0 && laneIndex < fallbackColors.Length)
        {
            return fallbackColors[laneIndex];
        }
        return Color.white;
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
                
                // Switch between base and pressed materials
                Material targetMaterial = keyPressed ? pressedMaterials[i] : baseMaterials[i];
                if (targetMaterial != null && indicatorRenderers[i].material != targetMaterial)
                {
                    indicatorRenderers[i].material = targetMaterial;
                }
                
                // Scale effect when key is pressed (subtle effect)
                float targetScale = keyPressed ? indicatorSize * 0.95f : indicatorSize; // Slightly smaller when pressed
                Vector3 currentScale = indicators[i].transform.localScale;
                currentScale.x = Mathf.Lerp(currentScale.x, targetScale, Time.deltaTime * 15f);
                currentScale.z = Mathf.Lerp(currentScale.z, targetScale, Time.deltaTime * 15f);
                indicators[i].transform.localScale = currentScale;
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
        if (indicatorRenderers[laneIndex] == null || pressedMaterials[laneIndex] == null) yield break;
        
        // Store original material
        Material originalMaterial = indicatorRenderers[laneIndex].material;
        
        // Switch to pressed material for hit effect
        indicatorRenderers[laneIndex].material = pressedMaterials[laneIndex];
        
        // Add extra glow for hit effect
        Material hitMaterial = new Material(pressedMaterials[laneIndex]);
        hitMaterial.SetColor("_EmissionColor", Color.white * 0.5f);
        indicatorRenderers[laneIndex].material = hitMaterial;
        
        yield return new WaitForSeconds(0.15f);
        
        // Return to original material
        indicatorRenderers[laneIndex].material = originalMaterial;
        
        // Clean up temporary material
        if (hitMaterial != null)
        {
            DestroyImmediate(hitMaterial);
        }
    }
    
    void OnDestroy()
    {
        // Clean up materials
        for (int i = 0; i < 5; i++)
        {
            if (baseMaterials[i] != null)
            {
                DestroyImmediate(baseMaterials[i]);
            }
            if (pressedMaterials[i] != null)
            {
                DestroyImmediate(pressedMaterials[i]);
            }
        }
    }
}
