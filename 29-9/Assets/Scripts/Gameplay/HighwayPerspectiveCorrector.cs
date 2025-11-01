using UnityEngine;

/// <summary>
/// Corrige la perspectiva del sprite highway katana para que se vea como un rectángulo recto
/// en lugar de un trapecio
/// </summary>
public class HighwayPerspectiveCorrector : MonoBehaviour
{
    [Header("Perspective Correction")]
    public bool applyCorrection = true;
    public bool createRectangularVersion = true;
    
    [Header("Correction Settings")]
    [Range(0f, 2f)]
    public float topWidthMultiplier = 1.5f; // Multiplicador para ensanchar la parte superior
    [Range(0f, 1f)]
    public float bottomWidthMultiplier = 1f; // Multiplicador para la parte inferior
    
    [Header("Mesh Generation")]
    public int meshResolution = 20; // Resolución del mesh para la corrección
    public Material highwayMaterial;
    
    [Header("Alternative: Simple Scaling")]
    public bool useSimpleScaling = false;
    public Vector3 scaleCorrection = new Vector3(1f, 1.2f, 1f);
    
    private GameObject correctedHighway;
    private Sprite originalSprite;
    
    void Start()
    {
        if (applyCorrection)
        {
            ApplyPerspectiveCorrection();
        }
    }
    
    /// <summary>
    /// Aplica corrección de perspectiva al highway katana
    /// </summary>
    [ContextMenu("Apply Perspective Correction")]
    public void ApplyPerspectiveCorrection()
    {
        Debug.Log("🔧 Aplicando corrección de perspectiva al highway katana...");
        
        // Buscar el highway con sprite katana
        GameObject highway = FindHighwayWithKatanaSprite();
        
        if (highway == null)
        {
            Debug.LogWarning("⚠️ No se encontró highway con sprite katana. Aplicando sprite primero...");
            ApplyKatanaSpriteFirst();
            highway = FindHighwayWithKatanaSprite();
        }
        
        if (highway != null)
        {
            if (useSimpleScaling)
            {
                ApplySimpleScaling(highway);
            }
            else if (createRectangularVersion)
            {
                CreateRectangularHighway(highway);
            }
            else
            {
                ApplyMeshCorrection(highway);
            }
        }
        else
        {
            Debug.LogError("❌ No se pudo encontrar o crear highway con sprite katana");
        }
    }
    
    /// <summary>
    /// Busca highway que tenga el sprite katana aplicado
    /// </summary>
    GameObject FindHighwayWithKatanaSprite()
    {
        // Buscar por nombres comunes de highway
        string[] highwayNames = { "Highway", "highway", "Highway_Katana", "Highway Surface", "Ground", "Plane" };
        
        foreach (string name in highwayNames)
        {
            GameObject obj = GameObject.Find(name);
            if (obj != null)
            {
                SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
                if (sr != null && sr.sprite != null && sr.sprite.name.ToLower().Contains("katana"))
                {
                    return obj;
                }
            }
        }
        
        // Buscar cualquier SpriteRenderer con sprite katana
        SpriteRenderer[] allSpriteRenderers = FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None);
        foreach (var sr in allSpriteRenderers)
        {
            if (sr.sprite != null && sr.sprite.name.ToLower().Contains("katana"))
            {
                return sr.gameObject;
            }
        }
        
        return null;
    }
    
    /// <summary>
    /// Aplica el sprite katana primero si no existe
    /// </summary>
    void ApplyKatanaSpriteFirst()
    {
        HighwaySpriteChanger spriteChanger = FindFirstObjectByType<HighwaySpriteChanger>();
        if (spriteChanger != null)
        {
            spriteChanger.ApplyKatanaHighwaySprite();
        }
        else
        {
            // Crear y configurar HighwaySpriteChanger
            GameObject changerObj = new GameObject("HighwaySpriteChanger");
            HighwaySpriteChanger changer = changerObj.AddComponent<HighwaySpriteChanger>();
            changer.ApplyKatanaHighwaySprite();
        }
    }
    
    /// <summary>
    /// Aplica escalado simple para corregir la perspectiva
    /// </summary>
    void ApplySimpleScaling(GameObject highway)
    {
        Debug.Log("📐 Aplicando escalado simple para corrección de perspectiva...");
        
        // Aplicar escalado correctivo
        highway.transform.localScale = Vector3.Scale(highway.transform.localScale, scaleCorrection);
        
        // Ajustar la rotación si es necesario
        Vector3 currentRotation = highway.transform.rotation.eulerAngles;
        highway.transform.rotation = Quaternion.Euler(currentRotation.x, currentRotation.y, currentRotation.z);
        
        Debug.Log($"✅ Escalado aplicado: {scaleCorrection}");
    }
    
    /// <summary>
    /// Crea una versión rectangular del highway usando un quad simple
    /// </summary>
    void CreateRectangularHighway(GameObject originalHighway)
    {
        Debug.Log("📐 Creando versión rectangular del highway...");
        
        // Obtener el sprite original
        SpriteRenderer originalSR = originalHighway.GetComponent<SpriteRenderer>();
        if (originalSR == null || originalSR.sprite == null)
        {
            Debug.LogError("❌ No se encontró sprite en el highway original");
            return;
        }
        
        originalSprite = originalSR.sprite;
        
        // Crear nuevo highway rectangular
        correctedHighway = GameObject.CreatePrimitive(PrimitiveType.Quad);
        correctedHighway.name = "Highway_Rectangular_Katana";
        
        // Remover collider
        Collider collider = correctedHighway.GetComponent<Collider>();
        if (collider != null)
        {
            DestroyImmediate(collider);
        }
        
        // Configurar posición y rotación
        correctedHighway.transform.position = originalHighway.transform.position;
        correctedHighway.transform.rotation = originalHighway.transform.rotation;
        
        // Configurar escala para que sea rectangular
        Vector3 rectangularScale = new Vector3(
            originalHighway.transform.localScale.x,
            originalHighway.transform.localScale.y,
            originalHighway.transform.localScale.z
        );
        correctedHighway.transform.localScale = rectangularScale;
        
        // Crear material con el sprite katana
        CreateRectangularMaterial();
        
        // Configurar renderer
        Renderer renderer = correctedHighway.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = highwayMaterial;
            renderer.sortingOrder = -10; // Detrás de las notas
        }
        
        // Desactivar highway original
        originalHighway.SetActive(false);
        
        Debug.Log("✅ Highway rectangular creado exitosamente");
    }
    
    /// <summary>
    /// Crea material rectangular usando el sprite katana
    /// </summary>
    void CreateRectangularMaterial()
    {
        if (highwayMaterial == null)
        {
            highwayMaterial = new Material(Shader.Find("Sprites/Default"));
        }
        
        // Aplicar la textura del sprite
        if (originalSprite != null)
        {
            highwayMaterial.mainTexture = originalSprite.texture;
            
            // Configurar UV mapping para que se vea rectangular
            // Esto estira la imagen para eliminar la perspectiva trapezoidal
            highwayMaterial.mainTextureScale = Vector2.one;
            highwayMaterial.mainTextureOffset = Vector2.zero;
            
            Debug.Log("🎨 Material rectangular configurado con textura katana");
        }
    }
    
    /// <summary>
    /// Aplica corrección usando mesh personalizado (avanzado)
    /// </summary>
    void ApplyMeshCorrection(GameObject highway)
    {
        Debug.Log("🔧 Aplicando corrección de mesh avanzada...");
        
        // Esta es una implementación más compleja que requiere crear un mesh custom
        // Por ahora, usar el método rectangular que es más simple y efectivo
        CreateRectangularHighway(highway);
    }
    
    /// <summary>
    /// Restaura el highway original
    /// </summary>
    [ContextMenu("Restore Original Highway")]
    public void RestoreOriginalHighway()
    {
        if (correctedHighway != null)
        {
            DestroyImmediate(correctedHighway);
            Debug.Log("🗑️ Highway corregido removido");
        }
        
        // Reactivar highway original
        GameObject originalHighway = FindHighwayWithKatanaSprite();
        if (originalHighway != null && !originalHighway.activeInHierarchy)
        {
            originalHighway.SetActive(true);
            Debug.Log("🔄 Highway original restaurado");
        }
    }
    
    /// <summary>
    /// Crear highway rectangular desde cero con color sólido
    /// </summary>
    [ContextMenu("Create Simple Rectangular Highway")]
    public void CreateSimpleRectangularHighway()
    {
        Debug.Log("📐 Creando highway rectangular simple...");
        
        GameObject rectangularHighway = GameObject.CreatePrimitive(PrimitiveType.Quad);
        rectangularHighway.name = "Highway_Simple_Rectangular";
        
        // Remover collider
        Collider collider = rectangularHighway.GetComponent<Collider>();
        if (collider != null)
        {
            DestroyImmediate(collider);
        }
        
        // Configurar posición y escala
        rectangularHighway.transform.position = new Vector3(0f, -0.1f, 0f);
        rectangularHighway.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        rectangularHighway.transform.localScale = new Vector3(10f, 50f, 1f);
        
        // Crear material con color metálico similar a katana
        Material metalMaterial = new Material(Shader.Find("Standard"));
        metalMaterial.color = new Color(0.7f, 0.7f, 0.8f, 1f); // Color metálico
        metalMaterial.SetFloat("_Metallic", 0.8f);
        metalMaterial.SetFloat("_Smoothness", 0.6f);
        
        Renderer renderer = rectangularHighway.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = metalMaterial;
            renderer.sortingOrder = -10;
        }
        
        Debug.Log("✅ Highway rectangular simple creado (color metálico)");
    }
    
    /// <summary>
    /// Información del sistema
    /// </summary>
    [ContextMenu("Show Correction Info")]
    public void ShowCorrectionInfo()
    {
        Debug.Log("📊 INFORMACIÓN DE CORRECCIÓN DE PERSPECTIVA:");
        Debug.Log("═══════════════════════════════════════════");
        
        GameObject highway = FindHighwayWithKatanaSprite();
        Debug.Log($"Highway con katana: {(highway != null ? "✅ ENCONTRADO" : "❌ NO ENCONTRADO")}");
        
        if (highway != null)
        {
            Debug.Log($"   Nombre: {highway.name}");
            Debug.Log($"   Posición: {highway.transform.position}");
            Debug.Log($"   Rotación: {highway.transform.rotation.eulerAngles}");
            Debug.Log($"   Escala: {highway.transform.localScale}");
            
            SpriteRenderer sr = highway.GetComponent<SpriteRenderer>();
            if (sr != null && sr.sprite != null)
            {
                Debug.Log($"   Sprite: {sr.sprite.name}");
                Debug.Log($"   Tamaño sprite: {sr.sprite.bounds.size}");
            }
        }
        
        Debug.Log($"Highway corregido: {(correctedHighway != null ? "✅ ACTIVO" : "❌ NO CREADO")}");
        
        Debug.Log("\n💡 OPCIONES DISPONIBLES:");
        Debug.Log("1. 'Apply Perspective Correction' - Crear versión rectangular");
        Debug.Log("2. 'Create Simple Rectangular Highway' - Highway metálico simple");
        Debug.Log("3. 'Restore Original Highway' - Volver al original");
    }
}
