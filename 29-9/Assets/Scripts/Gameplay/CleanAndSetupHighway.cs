using UnityEngine;

/// <summary>
/// Limpia todos los scripts antiguos de highway y configura el nuevo sistema metálico
/// Ejecutar una vez para solucionar todos los problemas
/// </summary>
public class CleanAndSetupHighway : MonoBehaviour
{
    void Start()
    {
        CleanAndSetupNewHighway();
    }
    
    /// <summary>
    /// Limpia todo y configura el nuevo highway metálico
    /// </summary>
    [ContextMenu("Clean And Setup New Highway")]
    public void CleanAndSetupNewHighway()
    {
        Debug.Log("🧹 LIMPIANDO SISTEMAS ANTIGUOS Y CONFIGURANDO NUEVO HIGHWAY...");
        Debug.Log("═══════════════════════════════════════════════════════════");
        
        // Paso 1: Limpiar scripts antiguos
        CleanOldHighwayScripts();
        
        // Paso 2: Limpiar highways existentes
        CleanExistingHighways();
        
        // Paso 3: Configurar nuevo sistema metálico
        SetupNewMetallicHighway();
        
        Debug.Log("✅ LIMPIEZA Y CONFIGURACIÓN COMPLETADA!");
        Debug.Log("🎉 Highway metálico rectangular configurado exitosamente");
        
        // Auto-destruir este script
        Destroy(this);
    }
    
    /// <summary>
    /// Remueve todos los scripts antiguos que causan errores
    /// </summary>
    void CleanOldHighwayScripts()
    {
        Debug.Log("🗑️ Removiendo scripts antiguos...");
        
        // Remover scripts antiguos de highway
        PngKatanaDiagnostic[] diagnostics = FindObjectsByType<PngKatanaDiagnostic>(FindObjectsSortMode.None);
        foreach (var diagnostic in diagnostics)
        {
            if (diagnostic != null)
            {
                DestroyImmediate(diagnostic.gameObject);
                Debug.Log("🗑️ PngKatanaDiagnostic removido");
            }
        }
        
        ChatGPTHighwaySetup[] chatgptSetups = FindObjectsByType<ChatGPTHighwaySetup>(FindObjectsSortMode.None);
        foreach (var setup in chatgptSetups)
        {
            if (setup != null)
            {
                DestroyImmediate(setup.gameObject);
                Debug.Log("🗑️ ChatGPTHighwaySetup removido");
            }
        }
        
        AutoChatGPTHighway[] autoSetups = FindObjectsByType<AutoChatGPTHighway>(FindObjectsSortMode.None);
        foreach (var autoSetup in autoSetups)
        {
            if (autoSetup != null)
            {
                DestroyImmediate(autoSetup.gameObject);
                Debug.Log("🗑️ AutoChatGPTHighway removido");
            }
        }
        
        HighwaySpriteChanger[] spriteChangers = FindObjectsByType<HighwaySpriteChanger>(FindObjectsSortMode.None);
        foreach (var changer in spriteChangers)
        {
            if (changer != null)
            {
                DestroyImmediate(changer.gameObject);
                Debug.Log("🗑️ HighwaySpriteChanger removido");
            }
        }
        
        // Remover otros scripts antiguos de highway
        Component[] allComponents = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
        foreach (var component in allComponents)
        {
            if (component != null && component != this)
            {
                string componentName = component.GetType().Name;
                if (componentName.Contains("Katana") || 
                    componentName.Contains("ChatGPT") || 
                    componentName.Contains("HighwaySprite") ||
                    componentName.Contains("PngKatana"))
                {
                    DestroyImmediate(component);
                    Debug.Log($"🗑️ {componentName} removido");
                }
            }
        }
        
        Debug.Log("✅ Scripts antiguos removidos");
    }
    
    /// <summary>
    /// Limpia highways existentes en la escena
    /// </summary>
    void CleanExistingHighways()
    {
        Debug.Log("🧹 Limpiando highways existentes...");
        
        string[] highwayNames = {
            "Highway", "highway", "Highway_Katana", "Highway Surface",
            "Highway_Rectangular_Katana", "Highway_Simple_Rectangular",
            "Rectangular_Katana_Highway", "ChatGPT_Highway",
            "Test_Highway", "Metallic_Highway", "Ground", "Plane"
        };
        
        foreach (string name in highwayNames)
        {
            GameObject existing = GameObject.Find(name);
            if (existing != null)
            {
                DestroyImmediate(existing);
                Debug.Log($"🗑️ Highway removido: {name}");
            }
        }
        
        Debug.Log("✅ Highways existentes limpiados");
    }
    
    /// <summary>
    /// Configura el nuevo sistema de highway metálico
    /// </summary>
    void SetupNewMetallicHighway()
    {
        Debug.Log("🛣️ Configurando nuevo highway metálico...");
        
        // Crear MetallicHighwayCreator
        GameObject creatorObj = new GameObject("MetallicHighwayCreator");
        MetallicHighwayCreator creator = creatorObj.AddComponent<MetallicHighwayCreator>();
        
        // Configurar parámetros optimizados
        creator.createOnStart = false; // Lo crearemos manualmente
        creator.replaceExistingHighway = true;
        
        // Configurar posición y escala
        creator.highwayPosition = new Vector3(0f, -0.1f, 0f);
        creator.highwayRotation = new Vector3(90f, 0f, 0f);
        creator.highwayScale = new Vector3(10f, 50f, 1f);
        
        // Configurar colores metálicos realistas
        creator.mainMetalColor = new Color(0.7f, 0.7f, 0.75f, 1f); // Gris metálico
        creator.edgeColor = new Color(0.4f, 0.4f, 0.45f, 1f); // Bordes oscuros
        creator.centerLineColor = new Color(0.3f, 0.3f, 0.35f, 1f); // Línea central
        creator.metallic = 0.8f;
        creator.smoothness = 0.6f;
        
        // Configurar detalles
        creator.addCenterLine = true;
        creator.addEdgeLines = true;
        creator.centerLineWidth = 0.15f;
        creator.edgeLineWidth = 0.08f;
        
        Debug.Log("✅ MetallicHighwayCreator configurado");
        
        // Crear highway inmediatamente
        creator.CreateMetallicHighway();
        
        Debug.Log("🎉 Highway metálico creado exitosamente!");
    }
    
    /// <summary>
    /// Verificar el estado final
    /// </summary>
    [ContextMenu("Check Final Status")]
    public void CheckFinalStatus()
    {
        Debug.Log("📊 ESTADO FINAL DEL HIGHWAY:");
        Debug.Log("═══════════════════════════");
        
        // Verificar MetallicHighwayCreator
        MetallicHighwayCreator creator = FindFirstObjectByType<MetallicHighwayCreator>();
        Debug.Log($"MetallicHighwayCreator: {(creator != null ? "✅ ACTIVO" : "❌ FALTANTE")}");
        
        // Verificar highway creado
        GameObject highway = GameObject.Find("Metallic_Highway");
        Debug.Log($"Highway Metálico: {(highway != null ? "✅ CREADO" : "❌ NO CREADO")}");
        
        if (highway != null)
        {
            Debug.Log($"   Posición: {highway.transform.position}");
            Debug.Log($"   Rotación: {highway.transform.rotation.eulerAngles}");
            Debug.Log($"   Escala: {highway.transform.localScale}");
        }
        
        // Verificar línea central
        GameObject centerLine = GameObject.Find("Highway_Center_Line");
        Debug.Log($"Línea Central: {(centerLine != null ? "✅ CREADA" : "❌ NO CREADA")}");
        
        // Verificar scripts antiguos
        PngKatanaDiagnostic diagnostic = FindFirstObjectByType<PngKatanaDiagnostic>();
        ChatGPTHighwaySetup chatgptSetup = FindFirstObjectByType<ChatGPTHighwaySetup>();
        AutoChatGPTHighway autoSetup = FindFirstObjectByType<AutoChatGPTHighway>();
        
        Debug.Log($"Scripts antiguos removidos: {(diagnostic == null && chatgptSetup == null && autoSetup == null ? "✅ SÍ" : "❌ AÚN PRESENTES")}");
        
        Debug.Log("");
        if (creator != null && highway != null)
        {
            Debug.Log("🎉 SISTEMA COMPLETAMENTE FUNCIONAL");
            Debug.Log("🛣️ Highway metálico rectangular listo para usar");
        }
        else
        {
            Debug.Log("⚠️ EJECUTAR 'Clean And Setup New Highway' PARA CONFIGURAR");
        }
    }
    
    /// <summary>
    /// Crear highway de emergencia si algo falla
    /// </summary>
    [ContextMenu("Emergency Highway Creation")]
    public void CreateEmergencyHighway()
    {
        Debug.Log("🚨 CREANDO HIGHWAY DE EMERGENCIA...");
        
        // Crear highway simple con color metálico
        GameObject emergencyHighway = GameObject.CreatePrimitive(PrimitiveType.Quad);
        emergencyHighway.name = "Emergency_Highway";
        
        // Remover collider
        Collider collider = emergencyHighway.GetComponent<Collider>();
        if (collider != null)
        {
            DestroyImmediate(collider);
        }
        
        // Configurar transform
        emergencyHighway.transform.position = new Vector3(0f, -0.1f, 0f);
        emergencyHighway.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        emergencyHighway.transform.localScale = new Vector3(10f, 50f, 1f);
        
        // Crear material metálico simple
        Material emergencyMaterial = new Material(Shader.Find("Standard"));
        emergencyMaterial.color = new Color(0.7f, 0.7f, 0.75f, 1f);
        emergencyMaterial.SetFloat("_Metallic", 0.8f);
        emergencyMaterial.SetFloat("_Smoothness", 0.6f);
        
        // Aplicar material
        Renderer renderer = emergencyHighway.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = emergencyMaterial;
            renderer.sortingOrder = -10;
        }
        
        Debug.Log("✅ Highway de emergencia creado");
        Debug.Log("🛣️ Highway metálico simple funcionando");
    }
}
