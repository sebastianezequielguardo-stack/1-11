using UnityEngine;

/// <summary>
/// Crea un highway metálico rectangular basado en la imagen proporcionada
/// No depende de archivos externos - genera la apariencia programáticamente
/// </summary>
public class MetallicHighwayCreator : MonoBehaviour
{
    [Header("Auto Setup")]
    public bool createOnStart = true;
    public bool replaceExistingHighway = true;
    
    [Header("Highway Settings")]
    public Vector3 highwayPosition = new Vector3(0f, -0.1f, 0f);
    public Vector3 highwayRotation = new Vector3(90f, 0f, 0f);
    public Vector3 highwayScale = new Vector3(10f, 50f, 1f);
    
    [Header("Metallic Appearance")]
    public Color mainMetalColor = new Color(0.7f, 0.7f, 0.75f, 1f); // Gris metálico
    public Color edgeColor = new Color(0.4f, 0.4f, 0.45f, 1f); // Gris más oscuro para bordes
    public Color centerLineColor = new Color(0.3f, 0.3f, 0.35f, 1f); // Línea central oscura
    [Range(0f, 1f)]
    public float metallic = 0.8f;
    [Range(0f, 1f)]
    public float smoothness = 0.6f;
    
    [Header("Details")]
    public bool addCenterLine = true;
    public bool addEdgeLines = true;
    public float centerLineWidth = 0.15f;
    public float edgeLineWidth = 0.08f;
    
    private GameObject metallicHighway;
    private GameObject centerLine;
    private GameObject[] edgeLines;
    
    void Start()
    {
        if (createOnStart)
        {
            CreateMetallicHighway();
        }
    }
    
    /// <summary>
    /// Crea el highway metálico rectangular
    /// </summary>
    [ContextMenu("Create Metallic Highway")]
    public void CreateMetallicHighway()
    {
        Debug.Log("🛣️ Creando highway metálico rectangular...");
        
        // Limpiar highways existentes
        if (replaceExistingHighway)
        {
            CleanExistingHighways();
        }
        
        // Crear highway principal
        CreateMainHighwaySurface();
        
        // Agregar detalles metálicos
        if (addCenterLine)
        {
            CreateCenterLine();
        }
        
        if (addEdgeLines)
        {
            CreateEdgeLines();
        }
        
        Debug.Log("✅ Highway metálico creado exitosamente!");
        Debug.Log("🎮 Highway rectangular con apariencia metálica aplicado");
    }
    
    /// <summary>
    /// Limpia highways existentes
    /// </summary>
    void CleanExistingHighways()
    {
        string[] highwayNames = {
            "Highway", "highway", "Highway_Katana", "Highway Surface",
            "ChatGPT_Highway", "Metallic_Highway", "Test_Highway",
            "Rectangular_Katana_Highway", "Ground", "Plane"
        };
        
        foreach (string name in highwayNames)
        {
            GameObject existing = GameObject.Find(name);
            if (existing != null)
            {
                DestroyImmediate(existing);
                Debug.Log($"🗑️ Highway existente removido: {name}");
            }
        }
    }
    
    /// <summary>
    /// Crea la superficie principal del highway
    /// </summary>
    void CreateMainHighwaySurface()
    {
        metallicHighway = GameObject.CreatePrimitive(PrimitiveType.Quad);
        metallicHighway.name = "Metallic_Highway";
        
        // Remover collider
        Collider collider = metallicHighway.GetComponent<Collider>();
        if (collider != null)
        {
            DestroyImmediate(collider);
        }
        
        // Configurar transform
        metallicHighway.transform.position = highwayPosition;
        metallicHighway.transform.rotation = Quaternion.Euler(highwayRotation);
        metallicHighway.transform.localScale = highwayScale;
        metallicHighway.transform.SetParent(transform);
        
        // Crear material metálico
        Material metallicMaterial = CreateMetallicMaterial();
        
        // Configurar renderer
        Renderer renderer = metallicHighway.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = metallicMaterial;
            renderer.sortingOrder = -10; // Detrás de las notas
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = true; // Para mejor apariencia metálica
        }
        
        Debug.Log("🛣️ Superficie principal del highway creada");
    }
    
    /// <summary>
    /// Crea material metálico que simula la imagen
    /// </summary>
    Material CreateMetallicMaterial()
    {
        Material material = new Material(Shader.Find("Standard"));
        
        // Configurar propiedades metálicas
        material.color = mainMetalColor;
        material.SetFloat("_Metallic", metallic);
        material.SetFloat("_Smoothness", smoothness);
        
        // Crear textura procedural para simular la superficie metálica
        Texture2D metallicTexture = CreateMetallicTexture();
        material.mainTexture = metallicTexture;
        
        // Configurar para renderizado óptimo
        material.SetFloat("_Mode", 0); // Opaque
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        material.SetInt("_ZWrite", 1);
        material.DisableKeyword("_ALPHATEST_ON");
        material.DisableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = -1;
        
        return material;
    }
    
    /// <summary>
    /// Crea textura procedural que simula la superficie metálica
    /// </summary>
    Texture2D CreateMetallicTexture()
    {
        int width = 512;
        int height = 512;
        Texture2D texture = new Texture2D(width, height);
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Crear gradiente vertical sutil para simular la superficie metálica
                float gradient = Mathf.Lerp(0.9f, 1.1f, (float)y / height);
                
                // Agregar ruido sutil para textura metálica
                float noise = Mathf.PerlinNoise(x * 0.01f, y * 0.01f) * 0.1f;
                
                // Color base metálico con variación
                float grayValue = Mathf.Clamp01(0.7f * gradient + noise);
                Color pixelColor = new Color(grayValue, grayValue, grayValue + 0.05f, 1f);
                
                texture.SetPixel(x, y, pixelColor);
            }
        }
        
        texture.Apply();
        return texture;
    }
    
    /// <summary>
    /// Crea la línea central oscura
    /// </summary>
    void CreateCenterLine()
    {
        centerLine = GameObject.CreatePrimitive(PrimitiveType.Cube);
        centerLine.name = "Highway_Center_Line";
        
        // Remover collider
        Collider collider = centerLine.GetComponent<Collider>();
        if (collider != null)
        {
            DestroyImmediate(collider);
        }
        
        // Configurar posición y escala
        centerLine.transform.position = highwayPosition + new Vector3(0f, 0.01f, 0f);
        centerLine.transform.rotation = Quaternion.Euler(highwayRotation);
        centerLine.transform.localScale = new Vector3(centerLineWidth, highwayScale.y, 0.02f);
        centerLine.transform.SetParent(transform);
        
        // Material para la línea central
        Material centerMaterial = new Material(Shader.Find("Standard"));
        centerMaterial.color = centerLineColor;
        centerMaterial.SetFloat("_Metallic", 0.9f);
        centerMaterial.SetFloat("_Smoothness", 0.7f);
        
        Renderer renderer = centerLine.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = centerMaterial;
            renderer.sortingOrder = -9; // Encima del highway principal
        }
        
        Debug.Log("📏 Línea central del highway creada");
    }
    
    /// <summary>
    /// Crea líneas de los bordes
    /// </summary>
    void CreateEdgeLines()
    {
        edgeLines = new GameObject[2];
        
        for (int i = 0; i < 2; i++)
        {
            edgeLines[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
            edgeLines[i].name = $"Highway_Edge_Line_{i}";
            
            // Remover collider
            Collider collider = edgeLines[i].GetComponent<Collider>();
            if (collider != null)
            {
                DestroyImmediate(collider);
            }
            
            // Configurar posición (izquierda y derecha)
            float xOffset = (i == 0) ? -highwayScale.x * 0.45f : highwayScale.x * 0.45f;
            edgeLines[i].transform.position = highwayPosition + new Vector3(xOffset, 0.01f, 0f);
            edgeLines[i].transform.rotation = Quaternion.Euler(highwayRotation);
            edgeLines[i].transform.localScale = new Vector3(edgeLineWidth, highwayScale.y, 0.02f);
            edgeLines[i].transform.SetParent(transform);
            
            // Material para los bordes
            Material edgeMaterial = new Material(Shader.Find("Standard"));
            edgeMaterial.color = edgeColor;
            edgeMaterial.SetFloat("_Metallic", 1f);
            edgeMaterial.SetFloat("_Smoothness", 0.8f);
            
            Renderer renderer = edgeLines[i].GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = edgeMaterial;
                renderer.sortingOrder = -9;
            }
        }
        
        Debug.Log("⚔️ Líneas de borde del highway creadas");
    }
    
    /// <summary>
    /// Actualiza los colores y materiales
    /// </summary>
    [ContextMenu("Update Highway Colors")]
    public void UpdateHighwayColors()
    {
        if (metallicHighway != null)
        {
            Renderer renderer = metallicHighway.GetComponent<Renderer>();
            if (renderer != null && renderer.material != null)
            {
                renderer.material.color = mainMetalColor;
                renderer.material.SetFloat("_Metallic", metallic);
                renderer.material.SetFloat("_Smoothness", smoothness);
            }
        }
        
        if (centerLine != null)
        {
            Renderer renderer = centerLine.GetComponent<Renderer>();
            if (renderer != null && renderer.material != null)
            {
                renderer.material.color = centerLineColor;
            }
        }
        
        if (edgeLines != null)
        {
            foreach (var edge in edgeLines)
            {
                if (edge != null)
                {
                    Renderer renderer = edge.GetComponent<Renderer>();
                    if (renderer != null && renderer.material != null)
                    {
                        renderer.material.color = edgeColor;
                    }
                }
            }
        }
        
        Debug.Log("🎨 Colores del highway metálico actualizados");
    }
    
    /// <summary>
    /// Remueve el highway metálico
    /// </summary>
    [ContextMenu("Remove Metallic Highway")]
    public void RemoveMetallicHighway()
    {
        if (metallicHighway != null)
        {
            DestroyImmediate(metallicHighway);
            Debug.Log("🗑️ Highway metálico removido");
        }
        
        if (centerLine != null)
        {
            DestroyImmediate(centerLine);
        }
        
        if (edgeLines != null)
        {
            foreach (var edge in edgeLines)
            {
                if (edge != null)
                {
                    DestroyImmediate(edge);
                }
            }
        }
        
        Debug.Log("🧹 Todos los elementos del highway metálico removidos");
    }
    
    /// <summary>
    /// Información del highway
    /// </summary>
    [ContextMenu("Show Highway Info")]
    public void ShowHighwayInfo()
    {
        Debug.Log("📊 INFORMACIÓN DEL HIGHWAY METÁLICO:");
        Debug.Log("═══════════════════════════════════");
        
        Debug.Log($"Highway principal: {(metallicHighway != null ? "✅ ACTIVO" : "❌ NO CREADO")}");
        Debug.Log($"Línea central: {(centerLine != null ? "✅ ACTIVA" : "❌ NO CREADA")}");
        Debug.Log($"Líneas de borde: {(edgeLines != null && edgeLines[0] != null ? "✅ ACTIVAS" : "❌ NO CREADAS")}");
        
        if (metallicHighway != null)
        {
            Debug.Log($"   Posición: {metallicHighway.transform.position}");
            Debug.Log($"   Rotación: {metallicHighway.transform.rotation.eulerAngles}");
            Debug.Log($"   Escala: {metallicHighway.transform.localScale}");
            Debug.Log($"   Color principal: {mainMetalColor}");
            Debug.Log($"   Metálico: {metallic}");
            Debug.Log($"   Suavidad: {smoothness}");
        }
        
        Debug.Log("\n💡 CONTROLES DISPONIBLES:");
        Debug.Log("- 'Create Metallic Highway' - Crear highway metálico");
        Debug.Log("- 'Update Highway Colors' - Actualizar colores");
        Debug.Log("- 'Remove Metallic Highway' - Remover highway");
    }
}
