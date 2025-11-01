using UnityEngine;
using System.IO;

/// <summary>
/// Script de diagnóstico para identificar problemas con la carga del sprite highway katana
/// </summary>
public class HighwaySpriteDiagnostic : MonoBehaviour
{
    [Header("Diagnostic Settings")]
    public bool runDiagnosticOnStart = true;
    
    void Start()
    {
        if (runDiagnosticOnStart)
        {
            RunCompleteDiagnostic();
        }
    }
    
    [ContextMenu("Run Complete Diagnostic")]
    public void RunCompleteDiagnostic()
    {
        Debug.Log("🔍 DIAGNÓSTICO COMPLETO DEL HIGHWAY SPRITE");
        Debug.Log("═══════════════════════════════════════════");
        
        CheckResourcesFolder();
        CheckSpriteFile();
        CheckSpriteImportSettings();
        CheckResourcesLoad();
        CheckAlternativeLocations();
        ProvideSolutions();
    }
    
    void CheckResourcesFolder()
    {
        Debug.Log("\n📁 VERIFICANDO CARPETA RESOURCES:");
        
        string resourcesPath = Path.Combine(Application.dataPath, "Resources");
        bool resourcesExists = Directory.Exists(resourcesPath);
        
        Debug.Log($"Carpeta Resources existe: {(resourcesExists ? "✅ SÍ" : "❌ NO")}");
        Debug.Log($"Ruta: {resourcesPath}");
        
        if (resourcesExists)
        {
            string[] files = Directory.GetFiles(resourcesPath, "*katana*", SearchOption.AllDirectories);
            Debug.Log($"Archivos con 'katana' encontrados: {files.Length}");
            
            foreach (string file in files)
            {
                string relativePath = file.Replace(Application.dataPath, "Assets");
                Debug.Log($"  - {relativePath}");
            }
        }
    }
    
    void CheckSpriteFile()
    {
        Debug.Log("\n🖼️ VERIFICANDO ARCHIVO DE SPRITE:");
        
        string[] possiblePaths = {
            Path.Combine(Application.dataPath, "Resources", "highway katana.png"),
            Path.Combine(Application.dataPath, "Resources", "highway_katana.png"),
            Path.Combine(Application.dataPath, "Resources", "highwaykatana.png"),
            Path.Combine(Application.dataPath, "Resources", "Highway Katana.png"),
            Path.Combine(Application.dataPath, "Resources", "highway katana.jpg"),
        };
        
        foreach (string path in possiblePaths)
        {
            bool exists = File.Exists(path);
            string fileName = Path.GetFileName(path);
            Debug.Log($"{fileName}: {(exists ? "✅ EXISTE" : "❌ NO EXISTE")}");
            
            if (exists)
            {
                FileInfo fileInfo = new FileInfo(path);
                Debug.Log($"  Tamaño: {fileInfo.Length} bytes");
                Debug.Log($"  Fecha: {fileInfo.LastWriteTime}");
            }
        }
    }
    
    void CheckSpriteImportSettings()
    {
        Debug.Log("\n⚙️ VERIFICANDO CONFIGURACIÓN DE IMPORTACIÓN:");
        
        // Intentar cargar el sprite con diferentes nombres
        string[] spriteNames = {
            "highway katana",
            "highway_katana", 
            "highwaykatana",
            "Highway Katana"
        };
        
        foreach (string spriteName in spriteNames)
        {
            Sprite sprite = Resources.Load<Sprite>(spriteName);
            Debug.Log($"'{spriteName}': {(sprite != null ? "✅ CARGA OK" : "❌ NO CARGA")}");
            
            if (sprite != null)
            {
                Debug.Log($"  Tamaño: {sprite.bounds.size}");
                Debug.Log($"  Textura: {sprite.texture.name}");
                Debug.Log($"  Formato: {sprite.texture.format}");
                return; // Encontramos uno que funciona
            }
        }
        
        // Intentar cargar como Texture2D
        Texture2D texture = Resources.Load<Texture2D>("highway katana");
        Debug.Log($"Como Texture2D: {(texture != null ? "✅ CARGA OK" : "❌ NO CARGA")}");
        
        if (texture != null)
        {
            Debug.Log($"  Tamaño: {texture.width}x{texture.height}");
            Debug.Log($"  Formato: {texture.format}");
            Debug.Log("  ⚠️ PROBLEMA: Se carga como Texture2D pero no como Sprite");
            Debug.Log("  💡 SOLUCIÓN: Cambiar Texture Type a 'Sprite (2D and UI)' en Import Settings");
        }
    }
    
    void CheckResourcesLoad()
    {
        Debug.Log("\n🔄 PROBANDO CARGA DESDE RESOURCES:");
        
        // Cargar todos los recursos para ver qué hay disponible
        Object[] allResources = Resources.LoadAll("");
        Debug.Log($"Total de recursos en Resources: {allResources.Length}");
        
        int spriteCount = 0;
        int textureCount = 0;
        
        foreach (Object resource in allResources)
        {
            if (resource is Sprite)
            {
                spriteCount++;
                if (resource.name.ToLower().Contains("katana") || resource.name.ToLower().Contains("highway"))
                {
                    Debug.Log($"  Sprite encontrado: {resource.name}");
                }
            }
            else if (resource is Texture2D)
            {
                textureCount++;
                if (resource.name.ToLower().Contains("katana") || resource.name.ToLower().Contains("highway"))
                {
                    Debug.Log($"  Texture2D encontrada: {resource.name}");
                }
            }
        }
        
        Debug.Log($"Sprites totales: {spriteCount}");
        Debug.Log($"Texturas totales: {textureCount}");
    }
    
    void CheckAlternativeLocations()
    {
        Debug.Log("\n🔍 BUSCANDO EN UBICACIONES ALTERNATIVAS:");
        
        // Buscar en toda la carpeta Assets
        string[] allFiles = Directory.GetFiles(Application.dataPath, "*katana*", SearchOption.AllDirectories);
        
        Debug.Log($"Archivos con 'katana' en Assets: {allFiles.Length}");
        
        foreach (string file in allFiles)
        {
            string relativePath = file.Replace(Application.dataPath, "Assets");
            string extension = Path.GetExtension(file).ToLower();
            
            if (extension == ".png" || extension == ".jpg" || extension == ".jpeg")
            {
                Debug.Log($"  Imagen encontrada: {relativePath}");
                
                // Si está fuera de Resources, sugerir moverla
                if (!relativePath.Contains("Resources"))
                {
                    Debug.Log($"    💡 SUGERENCIA: Mover a Assets/Resources/");
                }
            }
        }
    }
    
    void ProvideSolutions()
    {
        Debug.Log("\n💡 SOLUCIONES RECOMENDADAS:");
        Debug.Log("═══════════════════════════");
        
        Debug.Log("1. 📁 VERIFICAR UBICACIÓN:");
        Debug.Log("   - El archivo debe estar en: Assets/Resources/highway katana.png");
        Debug.Log("   - Nombre exacto: 'highway katana.png' (con espacio)");
        
        Debug.Log("\n2. ⚙️ CONFIGURAR IMPORT SETTINGS:");
        Debug.Log("   - Seleccionar el archivo en Unity");
        Debug.Log("   - En Inspector: Texture Type = 'Sprite (2D and UI)'");
        Debug.Log("   - Sprite Mode = 'Single'");
        Debug.Log("   - Click 'Apply'");
        
        Debug.Log("\n3. 🔄 REFRESCAR UNITY:");
        Debug.Log("   - Assets → Refresh (Ctrl+R)");
        Debug.Log("   - O cerrar y abrir Unity");
        
        Debug.Log("\n4. 🧪 PROBAR CARGA MANUAL:");
        Debug.Log("   - Usar 'Test Manual Load' en este script");
        Debug.Log("   - Verificar que aparezca en Project window");
        
        Debug.Log("\n5. 📋 ALTERNATIVA:");
        Debug.Log("   - Renombrar archivo a 'highway_katana.png' (con guión bajo)");
        Debug.Log("   - Actualizar el script para usar el nuevo nombre");
    }
    
    [ContextMenu("Test Manual Load")]
    public void TestManualLoad()
    {
        Debug.Log("🧪 PRUEBA DE CARGA MANUAL:");
        
        // Intentar diferentes variaciones del nombre
        string[] variations = {
            "highway katana",
            "highway_katana",
            "highwaykatana", 
            "Highway Katana",
            "Highway_Katana"
        };
        
        foreach (string variation in variations)
        {
            Sprite sprite = Resources.Load<Sprite>(variation);
            if (sprite != null)
            {
                Debug.Log($"✅ ÉXITO: '{variation}' se cargó correctamente");
                Debug.Log($"   Tamaño: {sprite.bounds.size}");
                
                // Intentar aplicar al highway
                ApplyTestSprite(sprite);
                return;
            }
            else
            {
                Debug.Log($"❌ FALLO: '{variation}' no se pudo cargar");
            }
        }
        
        Debug.Log("❌ Ninguna variación funcionó. Revisar las soluciones recomendadas.");
    }
    
    void ApplyTestSprite(Sprite sprite)
    {
        Debug.Log("🎨 Intentando aplicar sprite de prueba al highway...");
        
        // Buscar o crear highway para prueba
        GameObject highway = GameObject.Find("Highway") ?? GameObject.Find("highway");
        
        if (highway == null)
        {
            highway = new GameObject("Highway_Test");
            highway.transform.position = new Vector3(0f, -0.1f, 0f);
            highway.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            highway.transform.localScale = new Vector3(10f, 1f, 50f);
            Debug.Log("🔨 Highway de prueba creado");
        }
        
        SpriteRenderer sr = highway.GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            sr = highway.AddComponent<SpriteRenderer>();
            sr.sortingOrder = -10;
            Debug.Log("🎨 SpriteRenderer agregado");
        }
        
        sr.sprite = sprite;
        sr.color = Color.white;
        
        Debug.Log("✅ Sprite aplicado al highway de prueba!");
        Debug.Log($"   Highway: {highway.name}");
        Debug.Log($"   Sprite: {sprite.name}");
    }
    
    [ContextMenu("Create Test Highway")]
    public void CreateTestHighway()
    {
        GameObject highway = new GameObject("Highway_Diagnostic_Test");
        highway.transform.position = new Vector3(0f, -0.1f, 0f);
        highway.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        highway.transform.localScale = new Vector3(10f, 1f, 50f);
        
        SpriteRenderer sr = highway.AddComponent<SpriteRenderer>();
        sr.sortingOrder = -10;
        sr.color = Color.red; // Color rojo para que sea visible
        
        Debug.Log("🔨 Highway de prueba creado (color rojo para visibilidad)");
        Debug.Log("   Si ves una superficie roja, el sistema funciona");
        Debug.Log("   El problema es solo la carga del sprite");
    }
}
