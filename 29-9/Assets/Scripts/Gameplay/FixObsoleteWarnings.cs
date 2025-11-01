using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;

/// <summary>
/// Arregla automáticamente todos los warnings de FindObjectsOfType obsoleto
/// Reemplaza FindObjectsOfType con FindObjectsByType en todos los scripts
/// </summary>
public class FixObsoleteWarnings : MonoBehaviour
{
    [Header("Fix Settings")]
    public bool fixOnStart = false; // Cambiar a true para arreglar automáticamente
    
    void Start()
    {
        if (fixOnStart)
        {
            FixAllObsoleteWarnings();
        }
    }
    
    /// <summary>
    /// Arregla todos los warnings obsoletos automáticamente
    /// </summary>
    [ContextMenu("Fix All Obsolete Warnings")]
    public void FixAllObsoleteWarnings()
    {
        Debug.Log("🔧 Arreglando warnings obsoletos...");
        
        string scriptsPath = Path.Combine(Application.dataPath, "Scripts", "Gameplay");
        
        if (!Directory.Exists(scriptsPath))
        {
            Debug.LogError($"❌ No se encontró la carpeta: {scriptsPath}");
            return;
        }
        
        string[] csFiles = Directory.GetFiles(scriptsPath, "*.cs", SearchOption.AllDirectories);
        int filesFixed = 0;
        
        foreach (string filePath in csFiles)
        {
            if (FixFileObsoleteWarnings(filePath))
            {
                filesFixed++;
            }
        }
        
        Debug.Log($"✅ Arreglados {filesFixed} archivos");
        Debug.Log("💡 Refrescar Unity (Ctrl+R) para ver los cambios");
        
        // Refrescar automáticamente
        #if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
        #endif
    }
    
    /// <summary>
    /// Arregla warnings obsoletos en un archivo específico
    /// </summary>
    bool FixFileObsoleteWarnings(string filePath)
    {
        try
        {
            string content = File.ReadAllText(filePath);
            string originalContent = content;
            
            // Patrón para FindObjectsByType<T>(FindObjectsSortMode.None)
            string pattern1 = @"FindObjectsOfType<([^>]+)>\(\)";
            string replacement1 = "FindObjectsByType<$1>(FindObjectsSortMode.None)";
            content = Regex.Replace(content, pattern1, replacement1);
            
            // Patrón para FindFirstObjectByType<T>()
            string pattern2 = @"FindObjectOfType<([^>]+)>\(\)";
            string replacement2 = "FindFirstObjectByType<$1>()";
            content = Regex.Replace(content, pattern2, replacement2);
            
            // Si hubo cambios, guardar el archivo
            if (content != originalContent)
            {
                File.WriteAllText(filePath, content);
                string fileName = Path.GetFileName(filePath);
                Debug.Log($"🔧 Arreglado: {fileName}");
                return true;
            }
            
            return false;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"❌ Error arreglando {Path.GetFileName(filePath)}: {ex.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// Arregla warnings en archivos específicos
    /// </summary>
    [ContextMenu("Fix Specific Files")]
    public void FixSpecificFiles()
    {
        Debug.Log("🔧 Arreglando archivos específicos...");
        
        string[] specificFiles = {
            "HighwayAlignmentFixer.cs",
            "HitZoneCleanup.cs", 
            "HitDetectionDebugger.cs",
            "HitZonePositionSync.cs",
            "HitZoneVisualFixer.cs",
            "QuickUIFix.cs",
            "QuickTestMode.cs"
        };
        
        string scriptsPath = Path.Combine(Application.dataPath, "Scripts", "Gameplay");
        int filesFixed = 0;
        
        foreach (string fileName in specificFiles)
        {
            string filePath = Path.Combine(scriptsPath, fileName);
            
            if (File.Exists(filePath))
            {
                if (FixFileObsoleteWarnings(filePath))
                {
                    filesFixed++;
                }
            }
            else
            {
                Debug.LogWarning($"⚠️ No se encontró: {fileName}");
            }
        }
        
        Debug.Log($"✅ Arreglados {filesFixed} archivos específicos");
        
        #if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
        #endif
    }
    
    /// <summary>
    /// Muestra información sobre los warnings
    /// </summary>
    [ContextMenu("Show Warning Info")]
    public void ShowWarningInfo()
    {
        Debug.Log("📊 INFORMACIÓN SOBRE WARNINGS OBSOLETOS:");
        Debug.Log("═══════════════════════════════════════");
        
        Debug.Log("🔍 WARNINGS ENCONTRADOS:");
        Debug.Log("- FindObjectsByType<T>(FindObjectsSortMode.None) → FindObjectsByType<T>(FindObjectsSortMode.None)");
        Debug.Log("- FindFirstObjectByType<T>() → FindFirstObjectByType<T>()");
        
        Debug.Log("\n📁 ARCHIVOS AFECTADOS:");
        Debug.Log("- HighwayAlignmentFixer.cs");
        Debug.Log("- HitZoneCleanup.cs");
        Debug.Log("- HitDetectionDebugger.cs");
        Debug.Log("- HitZonePositionSync.cs");
        Debug.Log("- HitZoneVisualFixer.cs");
        Debug.Log("- QuickUIFix.cs");
        Debug.Log("- QuickTestMode.cs");
        
        Debug.Log("\n💡 SOLUCIÓN:");
        Debug.Log("1. Ejecutar 'Fix All Obsolete Warnings' o 'Fix Specific Files'");
        Debug.Log("2. Unity refrescará automáticamente");
        Debug.Log("3. Los warnings desaparecerán");
        
        Debug.Log("\n⚡ BENEFICIOS:");
        Debug.Log("- Mejor rendimiento (FindObjectsSortMode.None es más rápido)");
        Debug.Log("- Código actualizado a Unity 2023+");
        Debug.Log("- Sin warnings en la consola");
    }
    
    /// <summary>
    /// Crear backup antes de arreglar
    /// </summary>
    [ContextMenu("Create Backup Before Fix")]
    public void CreateBackupBeforeFix()
    {
        Debug.Log("💾 Creando backup de scripts...");
        
        string scriptsPath = Path.Combine(Application.dataPath, "Scripts", "Gameplay");
        string backupPath = Path.Combine(Application.dataPath, "Scripts", "Gameplay_Backup_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss"));
        
        try
        {
            if (Directory.Exists(scriptsPath))
            {
                CopyDirectory(scriptsPath, backupPath);
                Debug.Log($"✅ Backup creado en: {backupPath}");
                Debug.Log("💡 Ahora puedes ejecutar 'Fix All Obsolete Warnings' con seguridad");
            }
            else
            {
                Debug.LogError($"❌ No se encontró carpeta de scripts: {scriptsPath}");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"❌ Error creando backup: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Copia un directorio completo
    /// </summary>
    void CopyDirectory(string sourceDir, string destDir)
    {
        Directory.CreateDirectory(destDir);
        
        foreach (string file in Directory.GetFiles(sourceDir))
        {
            string destFile = Path.Combine(destDir, Path.GetFileName(file));
            File.Copy(file, destFile, true);
        }
        
        foreach (string subDir in Directory.GetDirectories(sourceDir))
        {
            string destSubDir = Path.Combine(destDir, Path.GetDirectoryName(subDir));
            CopyDirectory(subDir, destSubDir);
        }
    }
}
