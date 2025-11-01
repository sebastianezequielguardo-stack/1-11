using UnityEngine;

/// <summary>
/// Script de configuración automática para aplicar el sprite "highway katana" al highway
/// Ejecutar una vez para configurar el highway con el sprite katana
/// </summary>
public class HighwayKatanaSetup : MonoBehaviour
{
    [Header("Auto Setup")]
    [SerializeField] private bool setupOnStart = true;
    
    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = false;
    
    void Start()
    {
        if (setupOnStart)
        {
            SetupKatanaHighway();
        }
    }
    
    /// <summary>
    /// Configura automáticamente el highway con el sprite katana
    /// </summary>
    [ContextMenu("Setup Katana Highway")]
    public void SetupKatanaHighway()
    {
        Debug.Log("🗾 Configurando highway con sprite katana...");
        
        // Verificar si ya existe un HighwaySpriteChanger
        HighwaySpriteChanger existingSpriteChanger = FindFirstObjectByType<HighwaySpriteChanger>();
        if (existingSpriteChanger != null)
        {
            Debug.Log("✅ HighwaySpriteChanger ya existe, aplicando sprite...");
            existingSpriteChanger.ApplyKatanaHighwaySprite();
        }
        else
        {
            // Crear nuevo HighwaySpriteChanger
            GameObject spriteChangerObj = new GameObject("HighwaySpriteChanger");
            HighwaySpriteChanger spriteChanger = spriteChangerObj.AddComponent<HighwaySpriteChanger>();
            
            // Configurar parámetros
            spriteChanger.katanaSpriteName = "highway katana";
            spriteChanger.applyOnStart = false; // Ya lo aplicaremos manualmente
            spriteChanger.searchInChildren = true;
            spriteChanger.createHighwayIfNotFound = true;
            
            // Configurar posición y escala para el highway
            spriteChanger.highwayPosition = new Vector3(0f, -0.1f, 0f);
            spriteChanger.highwayScale = new Vector3(10f, 1f, 50f);
            spriteChanger.highwayRotation = new Vector3(90f, 0f, 0f);
            
            Debug.Log("✅ HighwaySpriteChanger creado");
            
            // Aplicar el sprite inmediatamente
            spriteChanger.ApplyKatanaHighwaySprite();
        }
        
        Debug.Log("🎉 ¡Highway katana configurado exitosamente!");
        Debug.Log("🎮 El highway ahora debería mostrar el sprite 'highway katana'");
        
        // Auto-destruir este script después de la configuración
        if (Application.isPlaying)
        {
            Destroy(this);
        }
    }
    
    /// <summary>
    /// Verificar el estado del highway katana
    /// </summary>
    [ContextMenu("Check Katana Highway Status")]
    public void CheckKatanaHighwayStatus()
    {
        Debug.Log("📊 ESTADO DEL HIGHWAY KATANA:");
        Debug.Log("═══════════════════════════");
        
        // Verificar HighwaySpriteChanger
        HighwaySpriteChanger spriteChanger = FindFirstObjectByType<HighwaySpriteChanger>();
        Debug.Log($"HighwaySpriteChanger: {(spriteChanger != null ? "✅ ACTIVO" : "❌ FALTANTE")}");
        
        // Verificar sprite en Resources
        Sprite katanaSprite = Resources.Load<Sprite>("highway katana");
        Debug.Log($"Sprite 'highway katana': {(katanaSprite != null ? "✅ ENCONTRADO" : "❌ FALTANTE")}");
        
        if (katanaSprite != null)
        {
            Debug.Log($"   Tamaño del sprite: {katanaSprite.bounds.size}");
            Debug.Log($"   Textura: {katanaSprite.texture.name}");
        }
        
        // Buscar highway en la escena
        GameObject[] possibleHighways = {
            GameObject.Find("Highway"),
            GameObject.Find("highway"),
            GameObject.Find("Highway Surface"),
            GameObject.Find("Highway_Katana"),
            GameObject.Find("Ground"),
            GameObject.Find("Plane")
        };
        
        GameObject foundHighway = null;
        foreach (var highway in possibleHighways)
        {
            if (highway != null)
            {
                foundHighway = highway;
                break;
            }
        }
        
        if (foundHighway != null)
        {
            Debug.Log($"Highway encontrado: ✅ {foundHighway.name}");
            
            SpriteRenderer sr = foundHighway.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Debug.Log($"   Sprite actual: {(sr.sprite != null ? sr.sprite.name : "Ninguno")}");
                Debug.Log($"   ¿Es katana?: {(sr.sprite != null && sr.sprite.name.Contains("katana") ? "✅ SÍ" : "❌ NO")}");
            }
            else
            {
                Debug.Log("   Sin SpriteRenderer");
            }
        }
        else
        {
            Debug.Log("Highway: ❌ NO ENCONTRADO");
        }
        
        Debug.Log("");
        if (spriteChanger != null && katanaSprite != null)
        {
            Debug.Log("🎉 SISTEMA LISTO PARA USAR");
        }
        else
        {
            Debug.Log("⚠️ EJECUTAR 'Setup Katana Highway' PARA CONFIGURAR");
        }
    }
    
    /// <summary>
    /// Probar la aplicación del sprite
    /// </summary>
    [ContextMenu("Test Apply Katana Sprite")]
    public void TestApplyKatanaSprite()
    {
        HighwaySpriteChanger spriteChanger = FindFirstObjectByType<HighwaySpriteChanger>();
        if (spriteChanger != null)
        {
            spriteChanger.ApplyKatanaHighwaySprite();
            Debug.Log("🧪 Sprite katana aplicado para prueba");
        }
        else
        {
            Debug.LogWarning("⚠️ No se encontró HighwaySpriteChanger. Ejecuta 'Setup Katana Highway' primero.");
        }
    }
    
    /// <summary>
    /// Información del sistema en el inspector
    /// </summary>
    void OnGUI()
    {
        if (!showDebugInfo) return;
        
        GUILayout.BeginArea(new Rect(Screen.width - 320, 10, 300, 200));
        GUILayout.Box("🗾 HIGHWAY KATANA SETUP");
        
        if (GUILayout.Button("Setup Katana Highway"))
        {
            SetupKatanaHighway();
        }
        
        if (GUILayout.Button("Check Status"))
        {
            CheckKatanaHighwayStatus();
        }
        
        if (GUILayout.Button("Test Apply Sprite"))
        {
            TestApplyKatanaSprite();
        }
        
        HighwaySpriteChanger spriteChanger = FindFirstObjectByType<HighwaySpriteChanger>();
        Sprite katanaSprite = Resources.Load<Sprite>("highway katana");
        
        if (spriteChanger != null && katanaSprite != null)
        {
            GUILayout.Label("✅ Sistema configurado");
            GUILayout.Label("🗾 Sprite katana disponible");
        }
        else
        {
            GUILayout.Label("❌ Sistema no configurado");
        }
        
        GUILayout.EndArea();
    }
    
    void Update()
    {
        // Teclas de acceso rápido
        if (Input.GetKeyDown(KeyCode.F6))
        {
            SetupKatanaHighway();
        }
        
        if (Input.GetKeyDown(KeyCode.F7))
        {
            CheckKatanaHighwayStatus();
        }
    }
}
