using UnityEngine;

/// <summary>
/// Script de configuración rápida para solucionar el problema del highway katana trapezoidal
/// Ofrece múltiples opciones para crear un highway rectangular
/// </summary>
public class KatanaHighwayFixer : MonoBehaviour
{
    [Header("Quick Fix Options")]
    public bool autoFixOnStart = true;
    
    [Header("Fix Method")]
    public FixMethod preferredMethod = FixMethod.RectangularMaterial;
    
    public enum FixMethod
    {
        RectangularMaterial,    // Crear highway rectangular con material metálico
        PerspectiveCorrection,  // Corregir perspectiva del sprite existente
        SimpleScaling          // Escalado simple del sprite existente
    }
    
    void Start()
    {
        if (autoFixOnStart)
        {
            FixKatanaHighway();
        }
    }
    
    /// <summary>
    /// Aplica la corrección del highway katana
    /// </summary>
    [ContextMenu("Fix Katana Highway")]
    public void FixKatanaHighway()
    {
        Debug.Log("🔧 Solucionando problema del highway katana trapezoidal...");
        
        switch (preferredMethod)
        {
            case FixMethod.RectangularMaterial:
                CreateRectangularMaterialHighway();
                break;
                
            case FixMethod.PerspectiveCorrection:
                ApplyPerspectiveCorrection();
                break;
                
            case FixMethod.SimpleScaling:
                ApplySimpleScaling();
                break;
        }
    }
    
    /// <summary>
    /// Método 1: Crear highway rectangular con material metálico (Recomendado)
    /// </summary>
    void CreateRectangularMaterialHighway()
    {
        Debug.Log("🗾 Método 1: Creando highway rectangular con material metálico...");
        
        // Buscar o crear RectangularKatanaHighway
        RectangularKatanaHighway rectangularHighway = FindFirstObjectByType<RectangularKatanaHighway>();
        
        if (rectangularHighway == null)
        {
            GameObject highwayObj = new GameObject("RectangularKatanaHighway");
            rectangularHighway = highwayObj.AddComponent<RectangularKatanaHighway>();
        }
        
        // Configurar y crear
        rectangularHighway.createOnStart = false;
        rectangularHighway.replaceExistingHighway = true;
        rectangularHighway.CreateRectangularKatanaHighway();
        
        Debug.Log("✅ Highway rectangular con material metálico creado");
        Debug.Log("💡 Este método crea un highway completamente rectangular sin usar el sprite trapezoidal");
    }
    
    /// <summary>
    /// Método 2: Corregir perspectiva del sprite existente
    /// </summary>
    void ApplyPerspectiveCorrection()
    {
        Debug.Log("🔧 Método 2: Aplicando corrección de perspectiva...");
        
        // Buscar o crear HighwayPerspectiveCorrector
        HighwayPerspectiveCorrector corrector = FindFirstObjectByType<HighwayPerspectiveCorrector>();
        
        if (corrector == null)
        {
            GameObject correctorObj = new GameObject("HighwayPerspectiveCorrector");
            corrector = correctorObj.AddComponent<HighwayPerspectiveCorrector>();
        }
        
        // Configurar y aplicar corrección
        corrector.applyCorrection = false;
        corrector.createRectangularVersion = true;
        corrector.useSimpleScaling = false;
        corrector.ApplyPerspectiveCorrection();
        
        Debug.Log("✅ Corrección de perspectiva aplicada");
        Debug.Log("💡 Este método usa el sprite katana pero lo transforma para que se vea rectangular");
    }
    
    /// <summary>
    /// Método 3: Escalado simple del sprite
    /// </summary>
    void ApplySimpleScaling()
    {
        Debug.Log("📐 Método 3: Aplicando escalado simple...");
        
        // Buscar highway con sprite katana
        GameObject highway = FindHighwayWithKatanaSprite();
        
        if (highway == null)
        {
            Debug.LogWarning("⚠️ No se encontró highway con sprite katana. Aplicando sprite primero...");
            ApplyKatanaSpriteFirst();
            highway = FindHighwayWithKatanaSprite();
        }
        
        if (highway != null)
        {
            // Aplicar escalado correctivo
            Vector3 correctionScale = new Vector3(1f, 1.3f, 1f); // Estirar verticalmente
            highway.transform.localScale = Vector3.Scale(highway.transform.localScale, correctionScale);
            
            Debug.Log($"✅ Escalado correctivo aplicado: {correctionScale}");
            Debug.Log("💡 Este método estira el sprite existente para compensar la forma trapezoidal");
        }
        else
        {
            Debug.LogError("❌ No se pudo encontrar highway con sprite katana");
        }
    }
    
    /// <summary>
    /// Busca highway que tenga el sprite katana
    /// </summary>
    GameObject FindHighwayWithKatanaSprite()
    {
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
        
        return null;
    }
    
    /// <summary>
    /// Aplica el sprite katana si no existe
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
            GameObject changerObj = new GameObject("HighwaySpriteChanger");
            HighwaySpriteChanger changer = changerObj.AddComponent<HighwaySpriteChanger>();
            changer.ApplyKatanaHighwaySprite();
        }
    }
    
    /// <summary>
    /// Probar todos los métodos disponibles
    /// </summary>
    [ContextMenu("Test All Methods")]
    public void TestAllMethods()
    {
        Debug.Log("🧪 PROBANDO TODOS LOS MÉTODOS DE CORRECCIÓN:");
        Debug.Log("═══════════════════════════════════════════");
        
        Debug.Log("\n1️⃣ MÉTODO RECTANGULAR MATERIAL (Recomendado):");
        CreateRectangularMaterialHighway();
        
        Debug.Log("\n2️⃣ MÉTODO CORRECCIÓN DE PERSPECTIVA:");
        ApplyPerspectiveCorrection();
        
        Debug.Log("\n3️⃣ MÉTODO ESCALADO SIMPLE:");
        ApplySimpleScaling();
        
        Debug.Log("\n✅ Todos los métodos probados. Revisa la escena para ver los resultados.");
        Debug.Log("💡 Usa 'Clean All Highways' para limpiar y probar individualmente.");
    }
    
    /// <summary>
    /// Limpiar todos los highways para empezar de nuevo
    /// </summary>
    [ContextMenu("Clean All Highways")]
    public void CleanAllHighways()
    {
        Debug.Log("🧹 Limpiando todos los highways...");
        
        string[] highwayNames = { 
            "Highway", "highway", "Highway_Katana", "Highway Surface",
            "Highway_Rectangular_Katana", "Highway_Simple_Rectangular",
            "Rectangular_Katana_Highway", "RectangularKatanaHighway",
            "HighwayPerspectiveCorrector", "HighwaySpriteChanger",
            "Ground", "Plane"
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
        
        Debug.Log("✅ Limpieza completa. Ahora puedes probar un método específico.");
    }
    
    /// <summary>
    /// Mostrar información de todos los métodos
    /// </summary>
    [ContextMenu("Show Method Comparison")]
    public void ShowMethodComparison()
    {
        Debug.Log("📊 COMPARACIÓN DE MÉTODOS DE CORRECCIÓN:");
        Debug.Log("═══════════════════════════════════════");
        
        Debug.Log("\n🗾 MÉTODO 1: RECTANGULAR MATERIAL (⭐ RECOMENDADO)");
        Debug.Log("✅ Pros:");
        Debug.Log("   - Completamente rectangular, sin distorsión");
        Debug.Log("   - Apariencia metálica realista de katana");
        Debug.Log("   - Líneas centrales y bordes detallados");
        Debug.Log("   - No depende del sprite original");
        Debug.Log("❌ Contras:");
        Debug.Log("   - No usa la imagen original exacta");
        
        Debug.Log("\n🔧 MÉTODO 2: CORRECCIÓN DE PERSPECTIVA");
        Debug.Log("✅ Pros:");
        Debug.Log("   - Usa la imagen katana original");
        Debug.Log("   - Corrige la perspectiva automáticamente");
        Debug.Log("❌ Contras:");
        Debug.Log("   - Puede tener distorsión residual");
        Debug.Log("   - Depende de que el sprite cargue correctamente");
        
        Debug.Log("\n📐 MÉTODO 3: ESCALADO SIMPLE");
        Debug.Log("✅ Pros:");
        Debug.Log("   - Rápido y simple");
        Debug.Log("   - Usa la imagen original");
        Debug.Log("❌ Contras:");
        Debug.Log("   - Puede verse estirado");
        Debug.Log("   - Corrección limitada");
        
        Debug.Log("\n💡 RECOMENDACIÓN:");
        Debug.Log("Usar Método 1 (Rectangular Material) para mejor resultado visual");
    }
}
