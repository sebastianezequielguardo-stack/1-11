using UnityEngine;

/// <summary>
/// Script para arreglar la visualización de los HitZones
/// - Oculta elementos UI no deseados en pantalla
/// - Mejora el estiramiento de los HitZones inferiores
/// - Asegura que solo se vean los indicadores necesarios
/// </summary>
public class HitZoneVisualFixer : MonoBehaviour
{
    [Header("UI Cleanup")]
    public bool hideDebugInterfaces = true;
    public bool hideTextLabels = true;
    
    [Header("HitZone Enhancement")]
    public bool enhanceHitZoneVisuals = true;
    public float hitZoneStretchFactor = 1.2f;
    public bool makeHitZonesMoreVisible = true;
    
    [Header("Colors")]
    public Color[] enhancedLaneColors = {
        new Color(0.2f, 0.8f, 0.2f, 0.8f), // Verde más brillante
        new Color(0.8f, 0.2f, 0.2f, 0.8f), // Rojo más brillante
        new Color(0.8f, 0.8f, 0.2f, 0.8f), // Amarillo más brillante
        new Color(0.2f, 0.4f, 0.8f, 0.8f), // Azul más brillante
        new Color(1f, 0.6f, 0.2f, 0.8f)    // Naranja más brillante
    };
    
    private HitZone hitZone;
    private HighwaySetup highwaySetup;
    
    void Start()
    {
        // Buscar componentes
        hitZone = FindFirstObjectByType<HitZone>();
        highwaySetup = FindFirstObjectByType<HighwaySetup>();
        
        // Aplicar correcciones
        if (hideDebugInterfaces)
        {
            DisableDebugInterfaces();
        }
        
        if (enhanceHitZoneVisuals)
        {
            EnhanceHitZoneVisuals();
        }
        
        if (hideTextLabels)
        {
            HideTextLabels();
        }
        
        Debug.Log("🎯 HitZoneVisualFixer: Correcciones visuales aplicadas");
    }
    
    void DisableDebugInterfaces()
    {
        // Desactivar QuickTestMode GUI
        QuickTestMode quickTest = FindFirstObjectByType<QuickTestMode>();
        if (quickTest != null)
        {
            quickTest.showDebugGUI = false;
            Debug.Log("🔇 QuickTestMode debug GUI desactivado");
        }
        
        // Desactivar HitDetectionDebugger GUI
        HitDetectionDebugger debugger = FindFirstObjectByType<HitDetectionDebugger>();
        if (debugger != null)
        {
            debugger.enableDebugMode = false;
            Debug.Log("🔇 HitDetectionDebugger GUI desactivado");
        }
        
        // Desactivar HitZoneIndicators que crean los KeyLabels
        DisableHitZoneIndicators();
        
        // Buscar y ocultar cualquier texto flotante con letras de teclas
        HideFloatingKeyLabels();
    }
    
    void DisableHitZoneIndicators()
    {
        // Buscar y desactivar HitZoneIndicators que crean KeyLabels duplicados
        HitZoneIndicators hitZoneIndicators = FindFirstObjectByType<HitZoneIndicators>();
        if (hitZoneIndicators != null)
        {
            hitZoneIndicators.showKeyLabels = false;
            Debug.Log("🔇 HitZoneIndicators KeyLabels desactivados");
            
            // Si ya se crearon, desactivar el componente completo para evitar duplicados
            hitZoneIndicators.enabled = false;
            Debug.Log("🔇 HitZoneIndicators componente desactivado para evitar duplicados");
        }
    }
    
    void HideFloatingKeyLabels()
    {
        // Buscar específicamente los KeyLabels creados por HitZoneIndicators
        GameObject[] keyLabels = GameObject.FindGameObjectsWithTag("Untagged");
        foreach (GameObject obj in keyLabels)
        {
            if (obj.name.StartsWith("KeyLabel_"))
            {
                obj.SetActive(false);
                Debug.Log($"🔇 KeyLabel destruido: {obj.name}");
            }
        }
        
        // Buscar GameObjects que puedan contener las letras D, F, J, K, L
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        
        foreach (GameObject obj in allObjects)
        {
            // Buscar TextMesh (usado por HitZoneIndicators)
            TextMesh textMesh = obj.GetComponent<TextMesh>();
            if (textMesh != null)
            {
                string text = textMesh.text.ToUpper();
                if ((text == "D" || text == "F" || text == "J" || text == "K" || text == "L") && 
                    obj.name.Contains("KeyLabel"))
                {
                    obj.SetActive(false);
                    Debug.Log($"🔇 TextMesh KeyLabel destruido: {obj.name} - {textMesh.text}");
                }
            }
            
            // Buscar componentes de texto que contengan las letras de las teclas
            UnityEngine.UI.Text uiText = obj.GetComponent<UnityEngine.UI.Text>();
            if (uiText != null)
            {
                string text = uiText.text.ToUpper();
                if (text.Contains("D") || text.Contains("F") || text.Contains("J") || 
                    text.Contains("K") || text.Contains("L"))
                {
                    if (text.Length <= 3) // Solo letras individuales o muy cortas
                    {
                        uiText.enabled = false;
                        Debug.Log($"🔇 Texto de tecla oculto: {uiText.text}");
                    }
                }
            }
            
            // También buscar TextMeshPro
            TMPro.TextMeshProUGUI tmpText = obj.GetComponent<TMPro.TextMeshProUGUI>();
            if (tmpText != null)
            {
                string text = tmpText.text.ToUpper();
                if (text.Contains("D") || text.Contains("F") || text.Contains("J") || 
                    text.Contains("K") || text.Contains("L"))
                {
                    if (text.Length <= 3) // Solo letras individuales o muy cortas
                    {
                        tmpText.enabled = false;
                        Debug.Log($"🔇 TextMeshPro de tecla oculto: {tmpText.text}");
                    }
                }
            }
        }
    }
    
    void HideTextLabels()
    {
        // Buscar y ocultar cualquier etiqueta de texto que no sea necesaria
        UnityEngine.UI.Text[] allTexts = FindObjectsByType<UnityEngine.UI.Text>(FindObjectsSortMode.None);
        foreach (UnityEngine.UI.Text text in allTexts)
        {
            if (text.name.ToLower().Contains("key") || 
                text.name.ToLower().Contains("button") ||
                text.name.ToLower().Contains("lane"))
            {
                // Solo ocultar si es texto muy corto (probablemente etiquetas de teclas)
                if (text.text.Length <= 3)
                {
                    text.enabled = false;
                    Debug.Log($"🔇 Etiqueta de texto oculta: {text.name} - {text.text}");
                }
            }
        }
        
        // Lo mismo para TextMeshPro
        TMPro.TextMeshProUGUI[] allTMPTexts = FindObjectsByType<TMPro.TextMeshProUGUI>(FindObjectsSortMode.None);
        foreach (TMPro.TextMeshProUGUI text in allTMPTexts)
        {
            if (text.name.ToLower().Contains("key") || 
                text.name.ToLower().Contains("button") ||
                text.name.ToLower().Contains("lane"))
            {
                if (text.text.Length <= 3)
                {
                    text.enabled = false;
                    Debug.Log($"🔇 Etiqueta TextMeshPro oculta: {text.name} - {text.text}");
                }
            }
        }
    }
    
    void EnhanceHitZoneVisuals()
    {
        if (hitZone == null || hitZone.laneIndicators == null) return;
        
        for (int i = 0; i < hitZone.laneIndicators.Length; i++)
        {
            if (hitZone.laneIndicators[i] != null)
            {
                GameObject indicator = hitZone.laneIndicators[i];
                
                // Estirar el HitZone
                Vector3 currentScale = indicator.transform.localScale;
                indicator.transform.localScale = new Vector3(
                    currentScale.x * hitZoneStretchFactor,
                    currentScale.y * hitZoneStretchFactor,
                    currentScale.z
                );
                
                // Mejorar colores si está habilitado
                if (makeHitZonesMoreVisible && i < enhancedLaneColors.Length)
                {
                    Renderer renderer = indicator.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        Material mat = renderer.material;
                        mat.color = enhancedLaneColors[i];
                        
                        // Agregar emisión para mejor visibilidad
                        if (mat.HasProperty("_EmissionColor"))
                        {
                            mat.EnableKeyword("_EMISSION");
                            mat.SetColor("_EmissionColor", enhancedLaneColors[i] * 0.4f);
                        }
                    }
                    
                    // También actualizar SpriteRenderer si existe
                    SpriteRenderer spriteRenderer = indicator.GetComponent<SpriteRenderer>();
                    if (spriteRenderer != null)
                    {
                        spriteRenderer.color = enhancedLaneColors[i];
                    }
                }
                
                Debug.Log($"🎯 HitZone Lane {i} mejorado - Escala: {indicator.transform.localScale}");
            }
        }
    }
    
    [ContextMenu("Apply Visual Fixes")]
    public void ApplyVisualFixes()
    {
        DisableDebugInterfaces();
        EnhanceHitZoneVisuals();
        HideTextLabels();
        Debug.Log("🎯 Correcciones visuales aplicadas manualmente");
    }
    
    [ContextMenu("Show Debug Interfaces")]
    public void ShowDebugInterfaces()
    {
        QuickTestMode quickTest = FindFirstObjectByType<QuickTestMode>();
        if (quickTest != null)
        {
            quickTest.showDebugGUI = true;
        }
        
        HitDetectionDebugger debugger = FindFirstObjectByType<HitDetectionDebugger>();
        if (debugger != null)
        {
            debugger.enableDebugMode = true;
        }
        
        Debug.Log("🔊 Interfaces de debug reactivadas");
    }
    
    [ContextMenu("Reset HitZone Scales")]
    public void ResetHitZoneScales()
    {
        if (hitZone == null || hitZone.laneIndicators == null) return;
        
        for (int i = 0; i < hitZone.laneIndicators.Length; i++)
        {
            if (hitZone.laneIndicators[i] != null)
            {
                hitZone.laneIndicators[i].transform.localScale = Vector3.one;
            }
        }
        
        Debug.Log("🔄 Escalas de HitZone reiniciadas");
    }
    
    void Update()
    {
        // Monitorear y ocultar cualquier nueva interfaz de debug que aparezca
        if (hideDebugInterfaces && Time.frameCount % 60 == 0) // Cada segundo aproximadamente
        {
            HideFloatingKeyLabels();
        }
    }
}
