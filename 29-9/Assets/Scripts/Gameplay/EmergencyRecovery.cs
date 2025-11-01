using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script de recuperación de emergencia
/// Recrea objetos esenciales que pueden haber sido eliminados accidentalmente
/// </summary>
public class EmergencyRecovery : MonoBehaviour
{
    [Header("Recovery Options")]
    public bool showDebugLogs = true;
    
    /// <summary>
    /// Recupera objetos esenciales del gameplay
    /// </summary>
    [ContextMenu("Recover Essential Objects")]
    public void RecoverEssentialObjects()
    {
        Debug.Log("🚨 RECUPERACIÓN DE EMERGENCIA - Recreando objetos esenciales");
        Debug.Log("═══════════════════════════════════════════════════════════");
        
        RecoverMainCamera();
        RecoverEventSystem();
        RecoverCanvas();
        RecoverAudioSource();
        RecoverGameplayManager();
        
        Debug.Log("✅ Recuperación completada");
    }
    
    /// <summary>
    /// Recupera la Main Camera si no existe
    /// </summary>
    [ContextMenu("Recover Main Camera")]
    public void RecoverMainCamera()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            GameObject cameraObj = new GameObject("Main Camera");
            Camera camera = cameraObj.AddComponent<Camera>();
            camera.tag = "MainCamera";
            cameraObj.AddComponent<AudioListener>();
            
            // Posición típica para gameplay
            cameraObj.transform.position = new Vector3(0, 1, -10);
            
            Debug.Log("📷 Main Camera recreada");
        }
        else
        {
            Debug.Log("📷 Main Camera ya existe");
        }
    }
    
    /// <summary>
    /// Recupera el EventSystem si no existe
    /// </summary>
    [ContextMenu("Recover EventSystem")]
    public void RecoverEventSystem()
    {
        UnityEngine.EventSystems.EventSystem eventSystem = FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>();
        if (eventSystem == null)
        {
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            
            Debug.Log("🎮 EventSystem recreado");
        }
        else
        {
            Debug.Log("🎮 EventSystem ya existe");
        }
    }
    
    /// <summary>
    /// Recupera el Canvas principal si no existe
    /// </summary>
    [ContextMenu("Recover Canvas")]
    public void RecoverCanvas()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            Canvas canvasComp = canvasObj.AddComponent<Canvas>();
            canvasComp.renderMode = RenderMode.ScreenSpaceOverlay;
            
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            canvasObj.AddComponent<GraphicRaycaster>();
            
            Debug.Log("🖼️ Canvas recreado");
        }
        else
        {
            Debug.Log("🖼️ Canvas ya existe");
        }
    }
    
    /// <summary>
    /// Recupera AudioSource si no existe
    /// </summary>
    [ContextMenu("Recover AudioSource")]
    public void RecoverAudioSource()
    {
        AudioSource audioSource = FindFirstObjectByType<AudioSource>();
        if (audioSource == null)
        {
            GameObject audioObj = new GameObject("Audio Source");
            AudioSource audio = audioObj.AddComponent<AudioSource>();
            audio.playOnAwake = false;
            audio.loop = false;
            
            Debug.Log("🔊 AudioSource recreado");
        }
        else
        {
            Debug.Log("🔊 AudioSource ya existe");
        }
    }
    
    /// <summary>
    /// Recupera GameplayManager básico si no existe
    /// </summary>
    [ContextMenu("Recover GameplayManager")]
    public void RecoverGameplayManager()
    {
        GameplayManager gameplayManager = FindFirstObjectByType<GameplayManager>();
        if (gameplayManager == null)
        {
            GameObject gameplayObj = new GameObject("GameplayManager");
            
            // Intentar agregar GameplayManager si existe la clase
            try
            {
                gameplayObj.AddComponent<GameplayManager>();
                Debug.Log("🎮 GameplayManager recreado");
            }
            catch
            {
                Debug.LogWarning("⚠️ No se pudo recrear GameplayManager (clase no encontrada)");
                Debug.Log("💡 Agrega manualmente el script GameplayManager al objeto creado");
            }
        }
        else
        {
            Debug.Log("🎮 GameplayManager ya existe");
        }
    }
    
    /// <summary>
    /// Recupera TrackBase básico
    /// </summary>
    [ContextMenu("Recover TrackBase")]
    public void RecoverTrackBase()
    {
        GameObject trackBase = GameObject.Find("TrackBase");
        if (trackBase == null)
        {
            trackBase = new GameObject("TrackBase");
            trackBase.transform.position = Vector3.zero;
            
            // Agregar Canvas para UI
            Canvas canvas = trackBase.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            
            trackBase.AddComponent<GraphicRaycaster>();
            
            Debug.Log("🛣️ TrackBase recreado");
        }
        else
        {
            Debug.Log("🛣️ TrackBase ya existe");
        }
    }
    
    /// <summary>
    /// Recupera Highway básico
    /// </summary>
    [ContextMenu("Recover Highway")]
    public void RecoverHighway()
    {
        GameObject highway = GameObject.Find("Highway");
        if (highway == null)
        {
            highway = new GameObject("Highway");
            
            // Crear highway visual básico
            GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            quad.transform.SetParent(highway.transform);
            quad.transform.localPosition = Vector3.zero;
            quad.transform.localRotation = Quaternion.Euler(90, 0, 0);
            quad.transform.localScale = new Vector3(10, 20, 1);
            quad.name = "HighwayVisual";
            
            // Material básico
            Renderer renderer = quad.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.gray;
            }
            
            Debug.Log("🛣️ Highway recreado");
        }
        else
        {
            Debug.Log("🛣️ Highway ya existe");
        }
    }
    
    /// <summary>
    /// Diagnóstico de objetos faltantes
    /// </summary>
    [ContextMenu("Diagnose Missing Objects")]
    public void DiagnoseMissingObjects()
    {
        Debug.Log("🔍 DIAGNÓSTICO DE OBJETOS FALTANTES:");
        Debug.Log("═══════════════════════════════════");
        
        // Verificar objetos esenciales
        Camera mainCamera = Camera.main;
        Debug.Log($"📷 Main Camera: {(mainCamera != null ? "✅ EXISTE" : "❌ FALTANTE")}");
        
        UnityEngine.EventSystems.EventSystem eventSystem = FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>();
        Debug.Log($"🎮 EventSystem: {(eventSystem != null ? "✅ EXISTE" : "❌ FALTANTE")}");
        
        Canvas canvas = FindFirstObjectByType<Canvas>();
        Debug.Log($"🖼️ Canvas: {(canvas != null ? "✅ EXISTE" : "❌ FALTANTE")}");
        
        AudioSource audioSource = FindFirstObjectByType<AudioSource>();
        Debug.Log($"🔊 AudioSource: {(audioSource != null ? "✅ EXISTE" : "❌ FALTANTE")}");
        
        GameplayManager gameplayManager = FindFirstObjectByType<GameplayManager>();
        Debug.Log($"🎮 GameplayManager: {(gameplayManager != null ? "✅ EXISTE" : "❌ FALTANTE")}");
        
        GameObject trackBase = GameObject.Find("TrackBase");
        Debug.Log($"🛣️ TrackBase: {(trackBase != null ? "✅ EXISTE" : "❌ FALTANTE")}");
        
        GameObject highway = GameObject.Find("Highway");
        Debug.Log($"🛣️ Highway: {(highway != null ? "✅ EXISTE" : "❌ FALTANTE")}");
        
        // Contar objetos totales
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        Debug.Log($"\n📊 Total GameObjects en escena: {allObjects.Length}");
        
        if (allObjects.Length < 5)
        {
            Debug.LogWarning("⚠️ Muy pocos objetos en la escena - posible eliminación accidental");
            Debug.Log("💡 Usa 'Recover Essential Objects' para recuperar objetos básicos");
        }
    }
    
    /// <summary>
    /// Recuperación completa de emergencia
    /// </summary>
    [ContextMenu("EMERGENCY - Recover All")]
    public void EmergencyRecoverAll()
    {
        Debug.Log("🚨 RECUPERACIÓN COMPLETA DE EMERGENCIA");
        Debug.Log("═══════════════════════════════════════");
        
        RecoverEssentialObjects();
        RecoverTrackBase();
        RecoverHighway();
        
        Debug.Log("🎉 Recuperación de emergencia completada");
        Debug.Log("💡 Revisa la escena y agrega componentes específicos si es necesario");
    }
    
    /// <summary>
    /// Mostrar información de la escena actual
    /// </summary>
    [ContextMenu("Show Scene Info")]
    public void ShowSceneInfo()
    {
        Debug.Log("📊 INFORMACIÓN DE LA ESCENA ACTUAL:");
        Debug.Log("═══════════════════════════════════");
        
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        Debug.Log($"Total GameObjects: {allObjects.Length}");
        
        Debug.Log("\n📋 Lista de objetos:");
        foreach (GameObject obj in allObjects)
        {
            if (obj != null)
            {
                Component[] components = obj.GetComponents<Component>();
                Debug.Log($"  - {obj.name} ({components.Length} componentes)");
            }
        }
    }
}
