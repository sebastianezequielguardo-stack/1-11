using UnityEngine;

/// <summary>
/// Debug helper to verify that all gameplay components are working correctly
/// </summary>
public class GameplayDebugger : MonoBehaviour
{
    [Header("Debug Settings")]
    public bool enableDebugLogs = false;  // Deshabilitado para experiencia limpia
    public bool showButtonStatus = false; // Deshabilitado para experiencia limpia
    
    private TexturedHitZoneIndicators texturedIndicators;
    private HitZone hitZone;
    private InputManager inputManager;
    private GameplayManager gameplayManager;
    
    void Start()
    {
        if (enableDebugLogs)
        {
            VerifyComponents();
        }
    }
    
    void VerifyComponents()
    {
        Debug.Log("=== GAMEPLAY DEBUGGER - VERIFICANDO COMPONENTES ===");
        
        // Check TexturedHitZoneIndicators
        texturedIndicators = FindFirstObjectByType<TexturedHitZoneIndicators>();
        if (texturedIndicators != null)
        {
            Debug.Log("✅ TexturedHitZoneIndicators encontrado");
            VerifyButtonTextures();
        }
        else
        {
            Debug.LogError("❌ TexturedHitZoneIndicators NO encontrado");
        }
        
        // Check HitZone
        hitZone = FindFirstObjectByType<HitZone>();
        if (hitZone != null)
        {
            Debug.Log("✅ HitZone encontrado");
        }
        else
        {
            Debug.LogError("❌ HitZone NO encontrado");
        }
        
        // Check InputManager
        inputManager = FindFirstObjectByType<InputManager>();
        if (inputManager != null)
        {
            Debug.Log("✅ InputManager encontrado");
        }
        else
        {
            Debug.LogError("❌ InputManager NO encontrado");
        }
        
        // Check GameplayManager
        gameplayManager = FindFirstObjectByType<GameplayManager>();
        if (gameplayManager != null)
        {
            Debug.Log("✅ GameplayManager encontrado");
        }
        else
        {
            Debug.LogError("❌ GameplayManager NO encontrado");
        }
        
        Debug.Log("=== FIN VERIFICACIÓN ===");
    }
    
    void VerifyButtonTextures()
    {
        if (texturedIndicators == null) return;
        
        string[] expectedTextures = {
            "boton verde base",
            "boton rojo base",
            "boton amarillo base",
            "boton azul base",
            "boton rosa base"
        };
        
        Debug.Log("--- Verificando texturas de botones ---");
        for (int i = 0; i < expectedTextures.Length; i++)
        {
            Texture2D texture = Resources.Load<Texture2D>(expectedTextures[i]);
            if (texture != null)
            {
                Debug.Log($"✅ Textura encontrada: {expectedTextures[i]}");
            }
            else
            {
                Debug.LogWarning($"⚠️ Textura faltante: {expectedTextures[i]}");
            }
        }
    }
    
    void Update()
    {
        if (showButtonStatus && Input.GetKeyDown(KeyCode.F1))
        {
            ShowButtonStatus();
        }
    }
    
    void ShowButtonStatus()
    {
        Debug.Log("=== ESTADO DE BOTONES (Presiona F1 para actualizar) ===");
        
        KeyCode[] keys = { KeyCode.D, KeyCode.F, KeyCode.J, KeyCode.K, KeyCode.L };
        string[] colors = { "Verde", "Rojo", "Amarillo", "Azul", "Rosa" };
        
        for (int i = 0; i < keys.Length; i++)
        {
            bool isPressed = Input.GetKey(keys[i]);
            string status = isPressed ? "PRESIONADO" : "Normal";
            Debug.Log($"Botón {i} ({colors[i]}) - Tecla {keys[i]}: {status}");
        }
    }
    
    void OnGUI()
    {
        if (!enableDebugLogs) return;
        
        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.Label("GAMEPLAY DEBUGGER", GUI.skin.box);
        
        if (GUILayout.Button("Verificar Componentes"))
        {
            VerifyComponents();
        }
        
        if (GUILayout.Button("Mostrar Estado Botones"))
        {
            ShowButtonStatus();
        }
        
        GUILayout.Label("Presiona F1 para estado de botones");
        
        GUILayout.EndArea();
    }
}
