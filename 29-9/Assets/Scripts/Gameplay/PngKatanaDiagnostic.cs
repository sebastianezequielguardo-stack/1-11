using UnityEngine;
using System.IO;

/// <summary>
/// Diagnóstico específico para la imagen pngkatana
/// Verifica ubicación, configuración y carga
/// </summary>
public class PngKatanaDiagnostic : MonoBehaviour
{
    [Header("Diagnostic Settings")]
    public bool runOnStart = true;
    
    void Start()
    {
        if (runOnStart)
        {
            RunCompleteDiagnostic();
        }
    }
    
    [ContextMenu("Run Complete Diagnostic")]
    public void RunCompleteDiagnostic()
    {
        Debug.Log("🔍 DIAGNÓSTICO COMPLETO - PNGKATANA");
        Debug.Log("═══════════════════════════════════");
        
        CheckResourcesFolder();
        CheckPngKatanaFile();
        CheckImportSettings();
        TestResourcesLoad();
        ProvideSolution();
    }
    
    void CheckResourcesFolder()
    {
        Debug.Log("\n📁 VERIFICANDO CARPETA RESOURCES:");
        
        string resourcesPath = Path.Combine(Application.dataPath, "Resources");
        bool resourcesExists = Directory.Exists(resourcesPath);
        
        Debug.Log($"Carpeta Resources existe: {(resourcesExists ? "✅ SÍ" : "❌ NO")}");
        Debug.Log($"Ruta completa: {resourcesPath}");
        
        if (resourcesExists)
        {
            // Buscar archivos con "katana" en el nombre
            string[] katanaFiles = Directory.GetFiles(resourcesPath, "*katana*", SearchOption.AllDirectories);
            Debug.Log($"Archivos con 'katana' encontrados: {katanaFiles.Length}");
            
            foreach (string file in katanaFiles)
            {
                string relativePath = file.Replace(Application.dataPath, "Assets");
                Debug.Log($"  📄 {relativePath}");
            }
            
            // Buscar archivos con "png" en el nombre
            string[] pngFiles = Directory.GetFiles(resourcesPath, "*png*", SearchOption.AllDirectories);
            Debug.Log($"Archivos con 'png' encontrados: {pngFiles.Length}");
            
            foreach (string file in pngFiles)
            {
                string relativePath = file.Replace(Application.dataPath, "Assets");
                Debug.Log($"  🖼️ {relativePath}");
            }
        }
    }
    
    void CheckPngKatanaFile()
    {
        Debug.Log("\n🖼️ VERIFICANDO ARCHIVO PNGKATANA:");
        
        string[] possiblePaths = {
            // Archivos en la carpeta pngkatana
            Path.Combine(Application.dataPath, "Resources", "pngkatana", "katana.png"),
            Path.Combine(Application.dataPath, "Resources", "pngkatana", "highway.png"),
            Path.Combine(Application.dataPath, "Resources", "pngkatana", "chatgpt.png"),
            Path.Combine(Application.dataPath, "Resources", "pngkatana", "rectangle.png"),
            Path.Combine(Application.dataPath, "Resources", "pngkatana", "highway katana.png"),
            Path.Combine(Application.dataPath, "Resources", "pngkatana", "Highway Katana.png"),
            Path.Combine(Application.dataPath, "Resources", "pngkatana", "pngkatana.png"),
            // Archivos JPG en la carpeta pngkatana
            Path.Combine(Application.dataPath, "Resources", "pngkatana", "katana.jpg"),
            Path.Combine(Application.dataPath, "Resources", "pngkatana", "highway.jpg"),
            Path.Combine(Application.dataPath, "Resources", "pngkatana", "chatgpt.jpg"),
            // Archivos en la raíz de Resources (por si acaso)
            Path.Combine(Application.dataPath, "Resources", "katana.png"),
            Path.Combine(Application.dataPath, "Resources", "highway.png"),
        };
        
        bool foundAny = false;
        
        foreach (string path in possiblePaths)
        {
            bool exists = File.Exists(path);
            string fileName = Path.GetFileName(path);
            Debug.Log($"{fileName}: {(exists ? "✅ EXISTE" : "❌ NO EXISTE")}");
            
            if (exists)
            {
                foundAny = true;
                FileInfo fileInfo = new FileInfo(path);
                Debug.Log($"  📊 Tamaño: {fileInfo.Length} bytes ({fileInfo.Length / 1024f:F1} KB)");
                Debug.Log($"  📅 Fecha: {fileInfo.LastWriteTime}");
                Debug.Log($"  📂 Ruta completa: {path}");
            }
        }
        
        if (!foundAny)
        {
            Debug.LogError("❌ NO SE ENCONTRÓ NINGÚN ARCHIVO PNGKATANA");
            Debug.LogError("💡 Verifica que el archivo esté en Assets/Resources/ con uno de estos nombres:");
            Debug.LogError("   - pngkatana.png (recomendado)");
            Debug.LogError("   - png_katana.png");
            Debug.LogError("   - PngKatana.png");
        }
    }
    
    void CheckImportSettings()
    {
        Debug.Log("\n⚙️ VERIFICANDO CONFIGURACIÓN DE IMPORTACIÓN:");
        
        string[] spriteNames = {
            "pngkatana/katana",
            "pngkatana/highway",
            "pngkatana/chatgpt",
            "pngkatana/rectangle",
            "pngkatana/highway katana",
            "pngkatana/Highway Katana",
            "pngkatana/pngkatana",
            "katana",
            "highway",
            "chatgpt"
        };
        
        bool foundSprite = false;
        
        foreach (string spriteName in spriteNames)
        {
            // Intentar cargar como Sprite
            Sprite sprite = Resources.Load<Sprite>(spriteName);
            Debug.Log($"Sprite '{spriteName}': {(sprite != null ? "✅ CARGA OK" : "❌ NO CARGA")}");
            
            if (sprite != null)
            {
                foundSprite = true;
                Debug.Log($"  📐 Tamaño bounds: {sprite.bounds.size}");
                Debug.Log($"  🖼️ Textura: {sprite.texture.name}");
                Debug.Log($"  📊 Dimensiones: {sprite.texture.width}x{sprite.texture.height}");
                Debug.Log($"  🎨 Formato: {sprite.texture.format}");
                break;
            }
            
            // Intentar cargar como Texture2D
            Texture2D texture = Resources.Load<Texture2D>(spriteName);
            if (texture != null)
            {
                Debug.LogWarning($"⚠️ '{spriteName}' se carga como Texture2D pero NO como Sprite");
                Debug.LogWarning("💡 SOLUCIÓN: Cambiar Texture Type a 'Sprite (2D and UI)' en Import Settings");
                Debug.Log($"  📊 Dimensiones: {texture.width}x{texture.height}");
                Debug.Log($"  🎨 Formato: {texture.format}");
            }
        }
        
        if (!foundSprite)
        {
            Debug.LogError("❌ NINGÚN ARCHIVO SE CARGA COMO SPRITE");
            Debug.LogError("🔧 PASOS PARA SOLUCIONAR:");
            Debug.LogError("1. Seleccionar el archivo en Unity Project window");
            Debug.LogError("2. En Inspector: Texture Type = 'Sprite (2D and UI)'");
            Debug.LogError("3. Sprite Mode = 'Single'");
            Debug.LogError("4. Click 'Apply'");
            Debug.LogError("5. Assets → Refresh (Ctrl+R)");
        }
    }
    
    void TestResourcesLoad()
    {
        Debug.Log("\n🧪 PROBANDO CARGA DESDE RESOURCES:");
        
        // Cargar todos los recursos para ver qué hay disponible
        Object[] allResources = Resources.LoadAll("");
        Debug.Log($"Total recursos en Resources: {allResources.Length}");
        
        int spriteCount = 0;
        int textureCount = 0;
        
        foreach (Object resource in allResources)
        {
            string resourceName = resource.name.ToLower();
            
            if (resource is Sprite && (resourceName.Contains("katana") || resourceName.Contains("png")))
            {
                spriteCount++;
                Debug.Log($"  🎨 Sprite encontrado: {resource.name}");
            }
            else if (resource is Texture2D && (resourceName.Contains("katana") || resourceName.Contains("png")))
            {
                textureCount++;
                Debug.Log($"  🖼️ Texture2D encontrada: {resource.name}");
            }
        }
        
        Debug.Log($"Sprites relacionados: {spriteCount}");
        Debug.Log($"Texturas relacionadas: {textureCount}");
    }
    
    void ProvideSolution()
    {
        Debug.Log("\n💡 SOLUCIÓN PASO A PASO:");
        Debug.Log("═══════════════════════════");
        
        Debug.Log("1. 📁 VERIFICAR UBICACIÓN:");
        Debug.Log("   ✅ El archivo debe estar en: Assets/Resources/pngkatana/[nombre].png");
        Debug.Log("   ✅ Nombres posibles: katana.png, highway.png, chatgpt.png, rectangle.png");
        
        Debug.Log("\n2. ⚙️ CONFIGURAR IMPORT SETTINGS:");
        Debug.Log("   ✅ Seleccionar archivo en Unity Project window");
        Debug.Log("   ✅ Inspector → Texture Type = 'Sprite (2D and UI)'");
        Debug.Log("   ✅ Sprite Mode = 'Single'");
        Debug.Log("   ✅ Click 'Apply'");
        
        Debug.Log("\n3. 🔄 REFRESCAR UNITY:");
        Debug.Log("   ✅ Assets → Refresh (Ctrl+R)");
        Debug.Log("   ✅ O cerrar y abrir Unity completamente");
        
        Debug.Log("\n4. 🧪 PROBAR CARGA:");
        Debug.Log("   ✅ Ejecutar 'Test Manual Load' en este script");
        Debug.Log("   ✅ Debe mostrar '✅ CARGA OK'");
        
        Debug.Log("\n5. 🎮 CONFIGURAR HIGHWAY:");
        Debug.Log("   ✅ Ejecutar AutoChatGPTHighway → 'Auto Setup ChatGPT Highway'");
        Debug.Log("   ✅ Debe crear highway con la imagen");
    }
    
    [ContextMenu("Test Manual Load")]
    public void TestManualLoad()
    {
        Debug.Log("🧪 PRUEBA DE CARGA MANUAL:");
        
        string[] variations = {
            "pngkatana/katana",
            "pngkatana/highway",
            "pngkatana/chatgpt",
            "pngkatana/rectangle",
            "pngkatana/highway katana",
            "pngkatana/Highway Katana",
            "pngkatana/pngkatana",
            "katana",
            "highway",
            "chatgpt"
        };
        
        foreach (string variation in variations)
        {
            Sprite sprite = Resources.Load<Sprite>(variation);
            if (sprite != null)
            {
                Debug.Log($"✅ ÉXITO: '{variation}' cargado como Sprite");
                Debug.Log($"   Tamaño: {sprite.bounds.size}");
                Debug.Log($"   Textura: {sprite.texture.name}");
                
                // Intentar aplicar al highway
                ApplyToHighway(sprite, variation);
                return;
            }
            else
            {
                Debug.Log($"❌ FALLO: '{variation}' no se cargó como Sprite");
            }
        }
        
        Debug.LogError("❌ Ninguna variación funcionó. Revisar ubicación y configuración del archivo.");
    }
    
    void ApplyToHighway(Sprite sprite, string spriteName)
    {
        Debug.Log($"🎨 Aplicando sprite '{spriteName}' al highway...");
        
        // Buscar o crear highway
        GameObject highway = GameObject.Find("ChatGPT_Highway");
        
        if (highway == null)
        {
            highway = new GameObject("ChatGPT_Highway");
            highway.transform.position = new Vector3(0f, -0.1f, 0f);
            highway.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            highway.transform.localScale = new Vector3(10f, 50f, 1f);
            
            SpriteRenderer sr = highway.AddComponent<SpriteRenderer>();
            sr.sortingOrder = -10;
            
            Debug.Log("🔨 Highway creado para prueba");
        }
        
        SpriteRenderer renderer = highway.GetComponent<SpriteRenderer>();
        if (renderer == null)
        {
            renderer = highway.AddComponent<SpriteRenderer>();
            renderer.sortingOrder = -10;
        }
        
        renderer.sprite = sprite;
        renderer.color = Color.white;
        
        Debug.Log("✅ Sprite aplicado exitosamente al highway!");
        Debug.Log($"   Highway: {highway.name}");
        Debug.Log($"   Sprite: {sprite.name}");
        Debug.Log($"   Posición: {highway.transform.position}");
    }
    
    [ContextMenu("Force Create Highway")]
    public void ForceCreateHighway()
    {
        Debug.Log("🔨 Creando highway de prueba (sin sprite)...");
        
        GameObject highway = new GameObject("Test_Highway");
        highway.transform.position = new Vector3(0f, -0.1f, 0f);
        highway.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        highway.transform.localScale = new Vector3(10f, 50f, 1f);
        
        SpriteRenderer sr = highway.AddComponent<SpriteRenderer>();
        sr.sortingOrder = -10;
        sr.color = Color.red; // Color rojo para visibilidad
        
        Debug.Log("✅ Highway de prueba creado (color rojo)");
        Debug.Log("   Si ves una superficie roja, el sistema de highway funciona");
        Debug.Log("   El problema es solo la carga del sprite");
    }
}
