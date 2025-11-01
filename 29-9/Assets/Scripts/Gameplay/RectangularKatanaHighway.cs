using UnityEngine;

/// <summary>
/// Crea un highway rectangular con apariencia de katana usando materiales y shaders
/// en lugar de sprites trapezoidales
/// </summary>
public class RectangularKatanaHighway : MonoBehaviour
{
    [Header("Highway Configuration")]
    public bool createOnStart = true;
    public bool replaceExistingHighway = true;
    
    [Header("Visual Settings")]
    public Color katanaColor = new Color(0.75f, 0.75f, 0.85f, 1f); // Color metálico azulado
    public Color edgeColor = new Color(0.3f, 0.3f, 0.4f, 1f); // Color del borde
    [Range(0f, 1f)]
    public float metallic = 0.8f;
    [Range(0f, 1f)]
    public float smoothness = 0.7f;
    
    [Header("Dimensions")]
    public Vector3 highwayPosition = new Vector3(0f, -0.1f, 0f);
    public Vector3 highwayRotation = new Vector3(90f, 0f, 0f);
    public Vector3 highwayScale = new Vector3(10f, 50f, 1f);
    
    [Header("Katana Details")]
    public bool addCenterLine = true;
    public bool addEdgeLines = true;
    public float centerLineWidth = 0.1f;
    public float edgeLineWidth = 0.05f;
    
    private GameObject rectangularHighway;
    private GameObject centerLine;
    private GameObject[] edgeLines;
    
    void Start()
    {
        if (createOnStart)
        {
            CreateRectangularKatanaHighway();
        }
    }
    
    /// <summary>
    /// Crea el highway rectangular con apariencia de katana
    /// </summary>
    [ContextMenu("Create Rectangular Katana Highway")]
    public void CreateRectangularKatanaHighway()
    {
        Debug.Log("🗾 Creando highway rectangular con apariencia de katana...");
        
        // Remover highway existente si se solicita
        if (replaceExistingHighway)
        {
            RemoveExistingHighways();
        }
        
        // Crear highway principal
        CreateMainHighway();
        
        // Agregar detalles de katana
        if (addCenterLine)
        {
            CreateCenterLine();
        }
        
        if (addEdgeLines)
        {
            CreateEdgeLines();
        }
        
        Debug.Log("✅ Highway rectangular con apariencia de katana creado exitosamente!");
    }
    
    /// <summary>
    /// Remueve highways existentes
    /// </summary>
    void RemoveExistingHighways()
    {
        string[] highwayNames = { 
            "Highway", "highway", "Highway_Katana", "Highway Surface", 
            "Highway_Rectangular_Katana", "Highway_Simple_Rectangular",
            "Ground", "Plane" 
        };
        
        foreach (string name in highwayNames)
        {
            GameObject existing = GameObject.Find(name);
            if (existing != null)
            {
                existing.SetActive(false);
                Debug.Log($"🗑️ Highway existente desactivado: {name}");
            }
        }
    }
    
    /// <summary>
    /// Crea el highway principal rectangular
    /// </summary>
    void CreateMainHighway()
    {
        rectangularHighway = GameObject.CreatePrimitive(PrimitiveType.Quad);
        rectangularHighway.name = "Rectangular_Katana_Highway";
        
        // Remover collider
        Collider collider = rectangularHighway.GetComponent<Collider>();
        if (collider != null)
        {
            DestroyImmediate(collider);
        }
        
        // Configurar transform
        rectangularHighway.transform.position = highwayPosition;
        rectangularHighway.transform.rotation = Quaternion.Euler(highwayRotation);
        rectangularHighway.transform.localScale = highwayScale;
        rectangularHighway.transform.SetParent(transform);
        
        // Crear y aplicar material katana
        Material katanaMaterial = CreateKatanaMaterial();
        
        Renderer renderer = rectangularHighway.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = katanaMaterial;
            renderer.sortingOrder = -10; // Detrás de las notas
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = false;
        }
        
        Debug.Log("🛣️ Highway principal rectangular creado");
    }
    
    /// <summary>
    /// Crea material con apariencia de katana
    /// </summary>
    Material CreateKatanaMaterial()
    {
        Material material = new Material(Shader.Find("Standard"));
        
        // Configurar propiedades metálicas
        material.color = katanaColor;
        material.SetFloat("_Metallic", metallic);
        material.SetFloat("_Smoothness", smoothness);
        
        // Agregar un poco de emisión para brillo sutil
        material.EnableKeyword("_EMISSION");
        material.SetColor("_EmissionColor", katanaColor * 0.1f);
        
        // Configurar para mejor apariencia
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
    /// Crea línea central de la katana
    /// </summary>
    void CreateCenterLine()
    {
        centerLine = GameObject.CreatePrimitive(PrimitiveType.Cube);
        centerLine.name = "Katana_Center_Line";
        
        // Remover collider
        Collider collider = centerLine.GetComponent<Collider>();
        if (collider != null)
        {
            DestroyImmediate(collider);
        }
        
        // Configurar posición y escala
        centerLine.transform.position = highwayPosition + new Vector3(0f, 0.01f, 0f);
        centerLine.transform.rotation = Quaternion.Euler(highwayRotation);
        centerLine.transform.localScale = new Vector3(centerLineWidth, highwayScale.y, 0.01f);
        centerLine.transform.SetParent(transform);
        
        // Material para la línea central
        Material centerMaterial = new Material(Shader.Find("Standard"));
        centerMaterial.color = edgeColor;
        centerMaterial.SetFloat("_Metallic", 0.9f);
        centerMaterial.SetFloat("_Smoothness", 0.8f);
        
        Renderer renderer = centerLine.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = centerMaterial;
            renderer.sortingOrder = -9; // Encima del highway principal
        }
        
        Debug.Log("📏 Línea central de katana creada");
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
            edgeLines[i].name = $"Katana_Edge_Line_{i}";
            
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
            edgeLines[i].transform.localScale = new Vector3(edgeLineWidth, highwayScale.y, 0.01f);
            edgeLines[i].transform.SetParent(transform);
            
            // Material para los bordes
            Material edgeMaterial = new Material(Shader.Find("Standard"));
            edgeMaterial.color = edgeColor;
            edgeMaterial.SetFloat("_Metallic", 1f);
            edgeMaterial.SetFloat("_Smoothness", 0.9f);
            
            Renderer renderer = edgeLines[i].GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = edgeMaterial;
                renderer.sortingOrder = -9;
            }
        }
        
        Debug.Log("⚔️ Líneas de borde de katana creadas");
    }
    
    /// <summary>
    /// Actualiza los colores del highway
    /// </summary>
    [ContextMenu("Update Katana Colors")]
    public void UpdateKatanaColors()
    {
        if (rectangularHighway != null)
        {
            Renderer renderer = rectangularHighway.GetComponent<Renderer>();
            if (renderer != null && renderer.material != null)
            {
                renderer.material.color = katanaColor;
                renderer.material.SetFloat("_Metallic", metallic);
                renderer.material.SetFloat("_Smoothness", smoothness);
                renderer.material.SetColor("_EmissionColor", katanaColor * 0.1f);
            }
        }
        
        if (centerLine != null)
        {
            Renderer renderer = centerLine.GetComponent<Renderer>();
            if (renderer != null && renderer.material != null)
            {
                renderer.material.color = edgeColor;
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
        
        Debug.Log("🎨 Colores de katana actualizados");
    }
    
    /// <summary>
    /// Remueve el highway rectangular
    /// </summary>
    [ContextMenu("Remove Rectangular Highway")]
    public void RemoveRectangularHighway()
    {
        if (rectangularHighway != null)
        {
            DestroyImmediate(rectangularHighway);
            Debug.Log("🗑️ Highway rectangular removido");
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
        
        Debug.Log("🧹 Todos los elementos de katana removidos");
    }
    
    /// <summary>
    /// Información del highway rectangular
    /// </summary>
    [ContextMenu("Show Highway Info")]
    public void ShowHighwayInfo()
    {
        Debug.Log("📊 INFORMACIÓN DEL HIGHWAY RECTANGULAR KATANA:");
        Debug.Log("═══════════════════════════════════════════");
        
        Debug.Log($"Highway principal: {(rectangularHighway != null ? "✅ ACTIVO" : "❌ NO CREADO")}");
        Debug.Log($"Línea central: {(centerLine != null ? "✅ ACTIVA" : "❌ NO CREADA")}");
        Debug.Log($"Líneas de borde: {(edgeLines != null && edgeLines[0] != null ? "✅ ACTIVAS" : "❌ NO CREADAS")}");
        
        if (rectangularHighway != null)
        {
            Debug.Log($"   Posición: {rectangularHighway.transform.position}");
            Debug.Log($"   Rotación: {rectangularHighway.transform.rotation.eulerAngles}");
            Debug.Log($"   Escala: {rectangularHighway.transform.localScale}");
            Debug.Log($"   Color: {katanaColor}");
            Debug.Log($"   Metálico: {metallic}");
            Debug.Log($"   Suavidad: {smoothness}");
        }
        
        Debug.Log("\n💡 CONTROLES DISPONIBLES:");
        Debug.Log("- 'Create Rectangular Katana Highway' - Crear highway");
        Debug.Log("- 'Update Katana Colors' - Actualizar colores");
        Debug.Log("- 'Remove Rectangular Highway' - Remover highway");
    }
}
