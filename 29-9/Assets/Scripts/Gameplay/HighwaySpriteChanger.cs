using UnityEngine;

/// <summary>
/// Script para cambiar el sprite del highway por el "highway katana"
/// Busca automáticamente el highway y aplica el sprite desde Resources
/// </summary>
public class HighwaySpriteChanger : MonoBehaviour
{
    [Header("Highway Sprite Configuration")]
    public string katanaSpriteName = "highway katana";
    public bool applyOnStart = true;
    
    [Header("Highway Search Settings")]
    public string[] highwayObjectNames = { "Highway", "highway", "Highway Surface", "HighwaySurface", "Ground", "Plane" };
    public bool searchInChildren = true;
    public bool createHighwayIfNotFound = true;
    
    [Header("Highway Creation Settings")]
    public Vector3 highwayPosition = new Vector3(0f, -0.1f, 0f);
    public Vector3 highwayScale = new Vector3(10f, 1f, 50f);
    public Vector3 highwayRotation = new Vector3(90f, 0f, 0f); // Para que el sprite se vea correctamente
    
    private Sprite katanaSprite;
    private GameObject highwayObject;
    
    void Start()
    {
        if (applyOnStart)
        {
            ApplyKatanaHighwaySprite();
        }
    }
    
    /// <summary>
    /// Aplica el sprite katana al highway
    /// </summary>
    [ContextMenu("Apply Katana Highway Sprite")]
    public void ApplyKatanaHighwaySprite()
    {
        Debug.Log("🗾 Aplicando sprite 'highway katana' al highway...");
        
        // Cargar el sprite
        if (!LoadKatanaSprite())
        {
            Debug.LogError("❌ No se pudo cargar el sprite 'highway katana'");
            return;
        }
        
        // Encontrar o crear el highway
        if (!FindOrCreateHighway())
        {
            Debug.LogError("❌ No se pudo encontrar o crear el highway");
            return;
        }
        
        // Aplicar el sprite
        ApplySpriteToHighway();
        
        Debug.Log("✅ Sprite 'highway katana' aplicado exitosamente al highway!");
    }
    
    /// <summary>
    /// Carga el sprite katana desde Resources con múltiples intentos
    /// </summary>
    bool LoadKatanaSprite()
    {
        // Intentar múltiples variaciones del nombre
        string[] spriteVariations = {
            katanaSpriteName,           // "highway katana"
            "highway_katana",           // Con guión bajo
            "highwaykatana",           // Sin espacios
            "Highway Katana",          // Con mayúsculas
            "Highway_Katana"           // Con mayúsculas y guión bajo
        };
        
        foreach (string spriteName in spriteVariations)
        {
            katanaSprite = Resources.Load<Sprite>(spriteName);
            if (katanaSprite != null)
            {
                Debug.Log($"✅ Sprite cargado exitosamente: '{spriteName}'");
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
                
                // Intentar crear sprite desde texture
                katanaSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                if (katanaSprite != null)
                {
                    Debug.Log("✅ Sprite creado desde Texture2D como solución temporal");
                    return true;
                }
            }
        }
        
        Debug.LogError($"❌ No se pudo cargar el sprite con ninguna variación del nombre");
        Debug.LogError("🔍 Ejecuta el diagnóstico para más información:");
        Debug.LogError("   1. Agrega HighwaySpriteDiagnostic a la escena");
        Debug.LogError("   2. Ejecuta 'Run Complete Diagnostic'");
        
        return false;
    }
    
    /// <summary>
    /// Encuentra el highway existente o crea uno nuevo
    /// </summary>
    bool FindOrCreateHighway()
    {
        // Buscar highway existente
        highwayObject = FindHighwayObject();
        
        if (highwayObject != null)
        {
            Debug.Log($"✅ Highway encontrado: {highwayObject.name}");
            return true;
        }
        
        if (createHighwayIfNotFound)
        {
            Debug.Log("🔨 Highway no encontrado, creando uno nuevo...");
            return CreateNewHighway();
        }
        
        Debug.LogWarning("⚠️ Highway no encontrado y createHighwayIfNotFound está deshabilitado");
        return false;
    }
    
    /// <summary>
    /// Busca el objeto highway en la escena
    /// </summary>
    GameObject FindHighwayObject()
    {
        // Buscar por nombres comunes
        foreach (string name in highwayObjectNames)
        {
            GameObject found = GameObject.Find(name);
            if (found != null)
            {
                return found;
            }
        }
        
        // Buscar en hijos si está habilitado
        if (searchInChildren)
        {
            foreach (string name in highwayObjectNames)
            {
                Transform found = transform.Find(name);
                if (found != null)
                {
                    return found.gameObject;
                }
            }
            
            // Buscar recursivamente en todos los hijos
            return FindHighwayInChildren(transform);
        }
        
        // Buscar cualquier objeto con SpriteRenderer o MeshRenderer que pueda ser el highway
        SpriteRenderer[] spriteRenderers = FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None);
        foreach (var sr in spriteRenderers)
        {
            if (sr.gameObject.name.ToLower().Contains("highway") || 
                sr.gameObject.name.ToLower().Contains("ground") ||
                sr.gameObject.name.ToLower().Contains("plane"))
            {
                return sr.gameObject;
            }
        }
        
        return null;
    }
    
    /// <summary>
    /// Busca highway recursivamente en los hijos
    /// </summary>
    GameObject FindHighwayInChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            foreach (string name in highwayObjectNames)
            {
                if (child.name.ToLower().Contains(name.ToLower()))
                {
                    return child.gameObject;
                }
            }
            
            // Buscar recursivamente
            GameObject found = FindHighwayInChildren(child);
            if (found != null)
            {
                return found;
            }
        }
        return null;
    }
    
    /// <summary>
    /// Crea un nuevo highway con sprite
    /// </summary>
    bool CreateNewHighway()
    {
        highwayObject = new GameObject("Highway_Katana");
        highwayObject.transform.SetParent(transform);
        
        // Configurar posición, rotación y escala
        highwayObject.transform.position = highwayPosition;
        highwayObject.transform.rotation = Quaternion.Euler(highwayRotation);
        highwayObject.transform.localScale = highwayScale;
        
        // Agregar SpriteRenderer
        SpriteRenderer spriteRenderer = highwayObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = -10; // Renderizar detrás de las notas
        
        Debug.Log("✅ Nuevo highway creado exitosamente");
        return true;
    }
    
    /// <summary>
    /// Aplica el sprite katana al highway
    /// </summary>
    void ApplySpriteToHighway()
    {
        // Buscar SpriteRenderer existente
        SpriteRenderer spriteRenderer = highwayObject.GetComponent<SpriteRenderer>();
        
        if (spriteRenderer == null)
        {
            // Crear SpriteRenderer si no existe
            spriteRenderer = highwayObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sortingOrder = -10;
            Debug.Log("🎨 SpriteRenderer agregado al highway");
        }
        
        // Aplicar el sprite
        spriteRenderer.sprite = katanaSprite;
        
        // Configurar propiedades del sprite
        spriteRenderer.color = Color.white; // Asegurar que no esté tintado
        spriteRenderer.flipX = false;
        spriteRenderer.flipY = false;
        
        // Ajustar el tamaño si es necesario
        AdjustHighwaySize(spriteRenderer);
        
        Debug.Log($"🎨 Sprite '{katanaSpriteName}' aplicado al highway: {highwayObject.name}");
    }
    
    /// <summary>
    /// Ajusta el tamaño del highway basado en el sprite
    /// </summary>
    void AdjustHighwaySize(SpriteRenderer spriteRenderer)
    {
        if (katanaSprite == null) return;
        
        // Calcular el tamaño basado en el sprite
        float spriteWidth = katanaSprite.bounds.size.x;
        float spriteHeight = katanaSprite.bounds.size.y;
        
        // Ajustar la escala para que el highway cubra el área apropiada
        Vector3 newScale = highwayScale;
        
        // Si el sprite es muy pequeño o muy grande, ajustar proporcionalmente
        if (spriteWidth > 0 && spriteHeight > 0)
        {
            float scaleFactorX = highwayScale.x / spriteWidth;
            float scaleFactorZ = highwayScale.z / spriteHeight;
            
            newScale = new Vector3(scaleFactorX, highwayScale.y, scaleFactorZ);
            highwayObject.transform.localScale = newScale;
            
            Debug.Log($"📏 Escala del highway ajustada: {newScale}");
        }
    }
    
    /// <summary>
    /// Remueve el sprite del highway
    /// </summary>
    [ContextMenu("Remove Highway Sprite")]
    public void RemoveHighwaySprite()
    {
        if (highwayObject == null)
        {
            highwayObject = FindHighwayObject();
        }
        
        if (highwayObject != null)
        {
            SpriteRenderer spriteRenderer = highwayObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = null;
                Debug.Log("🗑️ Sprite del highway removido");
            }
        }
    }
    
    /// <summary>
    /// Información del sistema en el inspector
    /// </summary>
    [ContextMenu("Show Highway Info")]
    public void ShowHighwayInfo()
    {
        Debug.Log("📊 INFORMACIÓN DEL HIGHWAY:");
        Debug.Log("═══════════════════════════");
        
        GameObject found = FindHighwayObject();
        if (found != null)
        {
            Debug.Log($"✅ Highway encontrado: {found.name}");
            Debug.Log($"   Posición: {found.transform.position}");
            Debug.Log($"   Rotación: {found.transform.rotation.eulerAngles}");
            Debug.Log($"   Escala: {found.transform.localScale}");
            
            SpriteRenderer sr = found.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Debug.Log($"   Sprite actual: {(sr.sprite != null ? sr.sprite.name : "Ninguno")}");
                Debug.Log($"   Sorting Order: {sr.sortingOrder}");
            }
            else
            {
                Debug.Log("   Sin SpriteRenderer");
            }
        }
        else
        {
            Debug.Log("❌ Highway no encontrado");
        }
        
        Sprite sprite = Resources.Load<Sprite>(katanaSpriteName);
        Debug.Log($"Sprite katana disponible: {(sprite != null ? "✅ Sí" : "❌ No")}");
    }
    
    void Update()
    {
        // Hotkey para aplicar sprite rápidamente
        if (Input.GetKeyDown(KeyCode.H) && Input.GetKey(KeyCode.LeftControl))
        {
            ApplyKatanaHighwaySprite();
        }
    }
}
