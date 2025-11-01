using UnityEngine;

/// <summary>
/// Sistema completo para configurar el highway usando la imagen "chatgpt" rectangular
/// Busca automáticamente la imagen y la aplica como highway
/// </summary>
public class ChatGPTHighwaySetup : MonoBehaviour
{
    [Header("Highway Configuration")]
    public string chatgptSpriteName = "pngkatana";
    public bool setupOnStart = true;
    public bool replaceExistingHighway = true;
    
    [Header("Highway Settings")]
    public Vector3 highwayPosition = new Vector3(0f, -0.1f, 0f);
    public Vector3 highwayRotation = new Vector3(90f, 0f, 0f);
    public Vector3 highwayScale = new Vector3(10f, 50f, 1f);
    
    [Header("Visual Settings")]
    public bool maintainAspectRatio = true;
    public Color tintColor = Color.white;
    [Range(-100, 100)]
    public int sortingOrder = -10;
    
    private GameObject chatgptHighway;
    private Sprite chatgptSprite;
    
    void Start()
    {
        if (setupOnStart)
        {
            SetupChatGPTHighway();
        }
    }
    
    /// <summary>
    /// Configura el highway usando la imagen chatgpt
    /// </summary>
    [ContextMenu("Setup ChatGPT Highway")]
    public void SetupChatGPTHighway()
    {
        Debug.Log("🤖 Configurando highway con imagen ChatGPT rectangular...");
        
        // Limpiar highways existentes si se solicita
        if (replaceExistingHighway)
        {
            CleanExistingHighways();
        }
        
        // Cargar el sprite ChatGPT
        if (!LoadChatGPTSprite())
        {
            Debug.LogError("❌ No se pudo cargar la imagen ChatGPT");
            return;
        }
        
        // Crear el highway
        CreateChatGPTHighway();
        
        Debug.Log("✅ Highway ChatGPT configurado exitosamente!");
        Debug.Log("🎮 El highway ahora usa la imagen rectangular 'chatgpt'");
    }
    
    /// <summary>
    /// Carga el sprite ChatGPT desde Resources con múltiples intentos
    /// </summary>
    bool LoadChatGPTSprite()
    {
        Debug.Log("🔍 Buscando imagen ChatGPT...");
        
        // Intentar múltiples variaciones del nombre y rutas
        string[] spriteVariations = {
            "pngkatana/katana",         // Carpeta pngkatana, archivo katana
            "pngkatana/highway",        // Carpeta pngkatana, archivo highway
            "pngkatana/pngkatana",      // Carpeta pngkatana, archivo pngkatana
            "pngkatana/chatgpt",        // Carpeta pngkatana, archivo chatgpt
            "pngkatana/rectangle",      // Carpeta pngkatana, archivo rectangle
            "pngkatana/highway katana", // Carpeta pngkatana, archivo highway katana
            "pngkatana/Highway Katana", // Con mayúsculas
            chatgptSpriteName,          // Valor configurado
            "katana",                   // Solo katana
            "highway",                  // Solo highway
            "chatgpt"                   // Solo chatgpt
        };
        
        foreach (string spriteName in spriteVariations)
        {
            chatgptSprite = Resources.Load<Sprite>(spriteName);
            if (chatgptSprite != null)
            {
                Debug.Log($"✅ Sprite ChatGPT cargado exitosamente: '{spriteName}'");
                return true;
            }
        }
        
        // Si no se carga como Sprite, intentar como Texture2D y convertir
        foreach (string spriteName in spriteVariations)
        {
            Texture2D texture = Resources.Load<Texture2D>(spriteName);
            if (texture != null)
            {
                Debug.LogWarning($"⚠️ Encontrado como Texture2D: '{spriteName}'");
                Debug.LogWarning("💡 SOLUCIÓN: En Unity, selecciona el archivo y cambia Texture Type a 'Sprite (2D and UI)'");
                
                // Crear sprite desde texture como solución temporal
                chatgptSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                if (chatgptSprite != null)
                {
                    Debug.Log("✅ Sprite ChatGPT creado desde Texture2D");
                    return true;
                }
            }
        }
        
        Debug.LogError("❌ No se pudo cargar la imagen ChatGPT con ninguna variación");
        Debug.LogError("🔍 Verifica que el archivo esté en Assets/Resources/pngkatana/ con alguno de estos nombres:");
        Debug.LogError("   - katana.png, highway.png, chatgpt.png, rectangle.png");
        Debug.LogError("⚙️ Y que esté configurado como 'Sprite (2D and UI)' en Import Settings");
        
        return false;
    }
    
    /// <summary>
    /// Limpia highways existentes
    /// </summary>
    void CleanExistingHighways()
    {
        Debug.Log("🧹 Limpiando highways existentes...");
        
        string[] highwayNames = {
            "Highway", "highway", "Highway_Katana", "Highway Surface",
            "Highway_Rectangular_Katana", "Highway_Simple_Rectangular",
            "Rectangular_Katana_Highway", "ChatGPT_Highway",
            "Ground", "Plane"
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
    }
    
    /// <summary>
    /// Crea el highway usando el sprite ChatGPT
    /// </summary>
    void CreateChatGPTHighway()
    {
        Debug.Log("🛣️ Creando highway ChatGPT...");
        
        // Crear GameObject para el highway
        chatgptHighway = new GameObject("ChatGPT_Highway");
        chatgptHighway.transform.SetParent(transform);
        
        // Agregar SpriteRenderer
        SpriteRenderer spriteRenderer = chatgptHighway.AddComponent<SpriteRenderer>();
        
        // Configurar sprite
        spriteRenderer.sprite = chatgptSprite;
        spriteRenderer.color = tintColor;
        spriteRenderer.sortingOrder = sortingOrder;
        
        // Configurar transform
        chatgptHighway.transform.position = highwayPosition;
        chatgptHighway.transform.rotation = Quaternion.Euler(highwayRotation);
        
        // Configurar escala
        if (maintainAspectRatio)
        {
            SetScaleWithAspectRatio();
        }
        else
        {
            chatgptHighway.transform.localScale = highwayScale;
        }
        
        // Configurar propiedades adicionales del renderer
        spriteRenderer.flipX = false;
        spriteRenderer.flipY = false;
        spriteRenderer.drawMode = SpriteDrawMode.Simple;
        
        Debug.Log($"✅ Highway ChatGPT creado: {chatgptHighway.name}");
        Debug.Log($"   Posición: {chatgptHighway.transform.position}");
        Debug.Log($"   Rotación: {chatgptHighway.transform.rotation.eulerAngles}");
        Debug.Log($"   Escala: {chatgptHighway.transform.localScale}");
    }
    
    /// <summary>
    /// Configura la escala manteniendo la proporción de la imagen
    /// </summary>
    void SetScaleWithAspectRatio()
    {
        if (chatgptSprite == null) return;
        
        // Obtener dimensiones del sprite
        float spriteWidth = chatgptSprite.bounds.size.x;
        float spriteHeight = chatgptSprite.bounds.size.y;
        
        if (spriteWidth > 0 && spriteHeight > 0)
        {
            // Calcular escala manteniendo proporción
            float aspectRatio = spriteWidth / spriteHeight;
            
            Vector3 newScale = highwayScale;
            
            // Ajustar escala basada en la proporción
            if (aspectRatio > 1f) // Imagen más ancha que alta
            {
                newScale.z = newScale.x / aspectRatio; // Ajustar altura
            }
            else // Imagen más alta que ancha
            {
                newScale.x = newScale.z * aspectRatio; // Ajustar ancho
            }
            
            chatgptHighway.transform.localScale = newScale;
            
            Debug.Log($"📐 Escala ajustada con proporción: {newScale} (Aspect Ratio: {aspectRatio:F2})");
        }
        else
        {
            chatgptHighway.transform.localScale = highwayScale;
        }
    }
    
    /// <summary>
    /// Actualiza la configuración visual del highway
    /// </summary>
    [ContextMenu("Update Highway Visuals")]
    public void UpdateHighwayVisuals()
    {
        if (chatgptHighway == null)
        {
            Debug.LogWarning("⚠️ No hay highway ChatGPT para actualizar");
            return;
        }
        
        SpriteRenderer sr = chatgptHighway.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = tintColor;
            sr.sortingOrder = sortingOrder;
        }
        
        chatgptHighway.transform.position = highwayPosition;
        chatgptHighway.transform.rotation = Quaternion.Euler(highwayRotation);
        
        if (maintainAspectRatio)
        {
            SetScaleWithAspectRatio();
        }
        else
        {
            chatgptHighway.transform.localScale = highwayScale;
        }
        
        Debug.Log("🎨 Visuales del highway ChatGPT actualizados");
    }
    
    /// <summary>
    /// Remueve el highway ChatGPT
    /// </summary>
    [ContextMenu("Remove ChatGPT Highway")]
    public void RemoveChatGPTHighway()
    {
        if (chatgptHighway != null)
        {
            DestroyImmediate(chatgptHighway);
            chatgptHighway = null;
            Debug.Log("🗑️ Highway ChatGPT removido");
        }
        else
        {
            Debug.LogWarning("⚠️ No hay highway ChatGPT para remover");
        }
    }
    
    /// <summary>
    /// Diagnóstico completo del sistema
    /// </summary>
    [ContextMenu("Run ChatGPT Highway Diagnostic")]
    public void RunDiagnostic()
    {
        Debug.Log("🔍 DIAGNÓSTICO DEL HIGHWAY CHATGPT:");
        Debug.Log("═══════════════════════════════════");
        
        // Verificar sprite en Resources
        Sprite testSprite = Resources.Load<Sprite>(chatgptSpriteName);
        Debug.Log($"Sprite '{chatgptSpriteName}': {(testSprite != null ? "✅ ENCONTRADO" : "❌ NO ENCONTRADO")}");
        
        if (testSprite != null)
        {
            Debug.Log($"   Tamaño: {testSprite.bounds.size}");
            Debug.Log($"   Textura: {testSprite.texture.name}");
            Debug.Log($"   Dimensiones: {testSprite.texture.width}x{testSprite.texture.height}");
        }
        
        // Verificar Texture2D alternativo
        Texture2D testTexture = Resources.Load<Texture2D>(chatgptSpriteName);
        Debug.Log($"Texture2D '{chatgptSpriteName}': {(testTexture != null ? "✅ ENCONTRADO" : "❌ NO ENCONTRADO")}");
        
        // Verificar highway actual
        Debug.Log($"Highway ChatGPT actual: {(chatgptHighway != null ? "✅ ACTIVO" : "❌ NO CREADO")}");
        
        if (chatgptHighway != null)
        {
            SpriteRenderer sr = chatgptHighway.GetComponent<SpriteRenderer>();
            Debug.Log($"   Sprite asignado: {(sr.sprite != null ? sr.sprite.name : "Ninguno")}");
            Debug.Log($"   Posición: {chatgptHighway.transform.position}");
            Debug.Log($"   Escala: {chatgptHighway.transform.localScale}");
        }
        
        Debug.Log("\n💡 INSTRUCCIONES:");
        Debug.Log("1. Asegúrate de que 'chatgpt.png' esté en Assets/Resources/");
        Debug.Log("2. Configúralo como 'Sprite (2D and UI)' en Import Settings");
        Debug.Log("3. Ejecuta 'Setup ChatGPT Highway' para crear el highway");
    }
    
    /// <summary>
    /// Información del highway en el inspector
    /// </summary>
    void OnGUI()
    {
        if (!Application.isPlaying) return;
        
        GUILayout.BeginArea(new Rect(10, 10, 300, 150));
        GUILayout.Box("🤖 CHATGPT HIGHWAY SETUP");
        
        if (GUILayout.Button("Setup ChatGPT Highway"))
        {
            SetupChatGPTHighway();
        }
        
        if (GUILayout.Button("Update Visuals"))
        {
            UpdateHighwayVisuals();
        }
        
        if (GUILayout.Button("Run Diagnostic"))
        {
            RunDiagnostic();
        }
        
        if (chatgptHighway != null)
        {
            GUILayout.Label("✅ Highway ChatGPT activo");
        }
        else
        {
            GUILayout.Label("❌ Highway no creado");
        }
        
        GUILayout.EndArea();
    }
    
    void Update()
    {
        // Hotkey para setup rápido
        if (Input.GetKeyDown(KeyCode.G) && Input.GetKey(KeyCode.LeftControl))
        {
            SetupChatGPTHighway();
        }
    }
}
