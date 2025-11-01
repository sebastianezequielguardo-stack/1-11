using UnityEngine;

/// <summary>
/// Configuración automática ultra-simple para el highway ChatGPT
/// Solo agregar a la escena y funciona automáticamente
/// </summary>
public class AutoChatGPTHighway : MonoBehaviour
{
    [Header("Auto Setup")]
    [SerializeField] private bool setupOnStart = true;
    
    void Start()
    {
        if (setupOnStart)
        {
            SetupChatGPTHighwayAuto();
        }
    }
    
    /// <summary>
    /// Configuración automática del highway ChatGPT
    /// </summary>
    [ContextMenu("Auto Setup ChatGPT Highway")]
    public void SetupChatGPTHighwayAuto()
    {
        Debug.Log("🤖 Configuración automática del highway ChatGPT...");
        
        // Verificar si ya existe ChatGPTHighwaySetup
        ChatGPTHighwaySetup existingSetup = FindFirstObjectByType<ChatGPTHighwaySetup>();
        if (existingSetup != null)
        {
            Debug.Log("✅ ChatGPTHighwaySetup ya existe, configurando...");
            existingSetup.SetupChatGPTHighway();
        }
        else
        {
            // Crear nuevo ChatGPTHighwaySetup
            GameObject setupObj = new GameObject("ChatGPTHighwaySetup");
            ChatGPTHighwaySetup setup = setupObj.AddComponent<ChatGPTHighwaySetup>();
            
            // Configurar parámetros optimizados
            setup.chatgptSpriteName = "pngkatana/katana"; // Carpeta/archivo
            setup.setupOnStart = false; // Ya lo configuraremos manualmente
            setup.replaceExistingHighway = true;
            
            // Configurar posición y escala para highway rectangular
            setup.highwayPosition = new Vector3(0f, -0.1f, 0f);
            setup.highwayRotation = new Vector3(90f, 0f, 0f);
            setup.highwayScale = new Vector3(10f, 50f, 1f);
            setup.maintainAspectRatio = true;
            setup.tintColor = Color.white;
            setup.sortingOrder = -10;
            
            Debug.Log("✅ ChatGPTHighwaySetup creado");
            
            // Configurar inmediatamente
            setup.SetupChatGPTHighway();
        }
        
        Debug.Log("🎉 ¡Highway ChatGPT configurado automáticamente!");
        Debug.Log("🎮 El highway ahora usa tu imagen rectangular 'chatgpt'");
        
        // Auto-destruir este script después de la configuración
        if (Application.isPlaying)
        {
            Destroy(this);
        }
    }
    
    /// <summary>
    /// Verificar estado del highway ChatGPT
    /// </summary>
    [ContextMenu("Check ChatGPT Highway Status")]
    public void CheckChatGPTHighwayStatus()
    {
        Debug.Log("📊 ESTADO DEL HIGHWAY CHATGPT:");
        Debug.Log("═══════════════════════════════");
        
        // Verificar ChatGPTHighwaySetup
        ChatGPTHighwaySetup setup = FindFirstObjectByType<ChatGPTHighwaySetup>();
        Debug.Log($"ChatGPTHighwaySetup: {(setup != null ? "✅ ACTIVO" : "❌ FALTANTE")}");
        
        // Verificar sprite en Resources (probar múltiples rutas)
        string[] testPaths = { "pngkatana/katana", "pngkatana/highway", "pngkatana/chatgpt" };
        Sprite chatgptSprite = null;
        string foundPath = "";
        
        foreach (string path in testPaths)
        {
            chatgptSprite = Resources.Load<Sprite>(path);
            if (chatgptSprite != null)
            {
                foundPath = path;
                break;
            }
        }
        
        Debug.Log($"Sprite en carpeta pngkatana: {(chatgptSprite != null ? $"✅ ENCONTRADO ({foundPath})" : "❌ FALTANTE")}");
        
        if (chatgptSprite != null)
        {
            Debug.Log($"   Tamaño: {chatgptSprite.bounds.size}");
            Debug.Log($"   Textura: {chatgptSprite.texture.name}");
        }
        
        // Buscar highway en la escena
        GameObject chatgptHighway = GameObject.Find("ChatGPT_Highway");
        Debug.Log($"Highway ChatGPT: {(chatgptHighway != null ? "✅ CREADO" : "❌ NO CREADO")}");
        
        if (chatgptHighway != null)
        {
            SpriteRenderer sr = chatgptHighway.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Debug.Log($"   Sprite actual: {(sr.sprite != null ? sr.sprite.name : "Ninguno")}");
                Debug.Log($"   ¿Es ChatGPT?: {(sr.sprite != null && sr.sprite.name.ToLower().Contains("chatgpt") ? "✅ SÍ" : "❌ NO")}");
            }
        }
        
        Debug.Log("");
        if (setup != null && chatgptSprite != null)
        {
            Debug.Log("🎉 SISTEMA LISTO PARA USAR");
        }
        else
        {
            Debug.Log("⚠️ EJECUTAR 'Auto Setup ChatGPT Highway' PARA CONFIGURAR");
        }
    }
    
    /// <summary>
    /// Limpiar todo y empezar de nuevo
    /// </summary>
    [ContextMenu("Clean and Restart")]
    public void CleanAndRestart()
    {
        Debug.Log("🧹 Limpiando sistema ChatGPT Highway...");
        
        // Remover highways existentes
        string[] highwayNames = {
            "ChatGPT_Highway", "ChatGPTHighwaySetup", "Highway", "highway"
        };
        
        foreach (string name in highwayNames)
        {
            GameObject obj = GameObject.Find(name);
            if (obj != null)
            {
                DestroyImmediate(obj);
                Debug.Log($"🗑️ Removido: {name}");
            }
        }
        
        Debug.Log("✅ Limpieza completa");
        Debug.Log("💡 Ahora ejecuta 'Auto Setup ChatGPT Highway' para configurar desde cero");
    }
}
