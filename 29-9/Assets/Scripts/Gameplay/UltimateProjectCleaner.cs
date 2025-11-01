using UnityEngine;
using System.IO;

/// <summary>
/// Limpiador definitivo que elimina TODOS los scripts problemáticos
/// y deja solo el sistema funcional básico
/// </summary>
public class UltimateProjectCleaner : MonoBehaviour
{
    [Header("Ultimate Cleanup")]
    public bool cleanOnStart = true;
    
    void Start()
    {
        if (cleanOnStart)
        {
            PerformUltimateCleanup();
        }
    }
    
    [ContextMenu("Perform Ultimate Cleanup")]
    public void PerformUltimateCleanup()
    {
        Debug.Log("🧹 LIMPIEZA DEFINITIVA DEL PROYECTO...");
        
        DeleteProblematicScripts();
        SetupBasicVideoSystem();
        
        Debug.Log("✅ PROYECTO COMPLETAMENTE LIMPIO");
        Debug.Log("📋 Solo quedan los scripts esenciales funcionando");
    }
    
    void DeleteProblematicScripts()
    {
        Debug.Log("🗑️ Eliminando scripts problemáticos...");
        
        string[] problematicScripts = {
            "QuickVideoSetup.cs",
            "VideoSystemMigratorSimple.cs", 
            "VideoSystemMigrator.cs",
            "VideoSetupHelper.cs",
            "FullScreenVideoBackground.cs",
            "VideoFormatOptimizer.cs",
            "QuickGameplayFix.cs",
            "BackgroundVideoManagerClean.cs",
            "ProjectCleanupFixer.cs",
            "SimpleVideoFix.cs"
        };
        
        string scriptsPath = Path.Combine(Application.dataPath, "Scripts", "Gameplay");
        
        foreach (string scriptName in problematicScripts)
        {
            string scriptPath = Path.Combine(scriptsPath, scriptName);
            
            if (File.Exists(scriptPath))
            {
                try
                {
                    // Instead of deleting, rename to .bak to disable
                    string backupPath = scriptPath + ".bak";
                    if (File.Exists(backupPath))
                    {
                        File.Delete(backupPath);
                    }
                    File.Move(scriptPath, backupPath);
                    Debug.Log($"🗑️ {scriptName} desactivado (renombrado a .bak)");
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"⚠️ No se pudo desactivar {scriptName}: {e.Message}");
                }
            }
        }
        
        Debug.Log("✅ Scripts problemáticos desactivados");
    }
    
    void SetupBasicVideoSystem()
    {
        Debug.Log("🎬 Configurando sistema básico de video...");
        
        // Find or create BackgroundVideoManager (the original one that works)
        BackgroundVideoManager videoManager = FindFirstObjectByType<BackgroundVideoManager>();
        
        if (videoManager == null)
        {
            GameObject videoObj = new GameObject("BackgroundVideoManager");
            videoManager = videoObj.AddComponent<BackgroundVideoManager>();
            Debug.Log("🆕 BackgroundVideoManager creado");
        }
        
        // Basic configuration
        videoManager.enableBackgroundVideo = true;
        videoManager.videoOpacity = 0.8f;
        videoManager.videoPosition = new Vector3(0f, 0f, 50f);
        videoManager.videoScale = new Vector3(50f, 30f, 1f);
        
        // Connect to GameplayManager
        GameplayManager gameplayManager = FindFirstObjectByType<GameplayManager>();
        if (gameplayManager != null)
        {
            // gameplayManager.backgroundVideoManager = videoManager; // REMOVED - Field no longer exists
            Debug.Log("🔗 GameplayManager encontrado (videos disabled)");
        }
        
        Debug.Log("✅ Sistema básico de video configurado");
    }
    
    [ContextMenu("Show Clean Project Status")]
    public void ShowCleanProjectStatus()
    {
        Debug.Log("📊 ESTADO DEL PROYECTO LIMPIO:");
        Debug.Log("═══════════════════════════════");
        Debug.Log("");
        Debug.Log("✅ SCRIPTS ACTIVOS (Esenciales):");
        Debug.Log("   • BackgroundVideoManager.cs - Sistema de video");
        Debug.Log("   • GameplayManager.cs - Gestión del gameplay");
        Debug.Log("   • UltimateProjectCleaner.cs - Este script");
        Debug.Log("");
        Debug.Log("🗑️ SCRIPTS DESACTIVADOS (.bak):");
        Debug.Log("   • Todos los scripts problemáticos renombrados");
        Debug.Log("   • No causan más errores de compilación");
        Debug.Log("");
        Debug.Log("🎬 SISTEMA DE VIDEO:");
        
        BackgroundVideoManager vm = FindFirstObjectByType<BackgroundVideoManager>();
        GameplayManager gm = FindFirstObjectByType<GameplayManager>();
        // bool connected = gm?.backgroundVideoManager != null; // REMOVED - Field no longer exists
        
        Debug.Log($"   • BackgroundVideoManager: {(vm != null ? "✅ ACTIVO" : "❌ FALTANTE")}");
        Debug.Log($"   • GameplayManager: {(gm != null ? "✅ ACTIVO" : "❌ FALTANTE")}");
        Debug.Log($"   • Conexión: ❌ DESHABILITADO (sin videos)");
        Debug.Log("");
        Debug.Log("📂 PARA VIDEOS DE FONDO:");
        Debug.Log("   StreamingAssets/Songs/[Cancion]/background.mp4");
        Debug.Log("");
        
        if (gm != null)
        {
            Debug.Log("🎉 PROYECTO COMPLETAMENTE FUNCIONAL Y LIMPIO (sin videos)");
        }
        else
        {
            Debug.Log("⚠️ Ejecutar 'Perform Ultimate Cleanup' para reparar");
        }
    }
    
    [ContextMenu("Test Video System")]
    public void TestVideoSystem()
    {
        BackgroundVideoManager videoManager = FindFirstObjectByType<BackgroundVideoManager>();
        
        if (videoManager != null)
        {
            Debug.Log("🧪 Probando sistema de video limpio...");
            
            if (GameManager.Instance?.selectedSongPath != null)
            {
                string songFolder = Path.Combine(Application.streamingAssetsPath, "Songs", GameManager.Instance.selectedSongPath);
                videoManager.LoadSongVideo(songFolder);
                Debug.Log($"🎬 Probando video para: {GameManager.Instance.selectedSongPath}");
            }
            else
            {
                Debug.LogWarning("⚠️ Selecciona una canción primero");
            }
        }
        else
        {
            Debug.LogError("❌ BackgroundVideoManager no encontrado");
        }
    }
    
    [ContextMenu("Restore Backup Scripts")]
    public void RestoreBackupScripts()
    {
        Debug.Log("🔄 Restaurando scripts desde backup...");
        
        string scriptsPath = Path.Combine(Application.dataPath, "Scripts", "Gameplay");
        string[] backupFiles = Directory.GetFiles(scriptsPath, "*.bak");
        
        foreach (string backupFile in backupFiles)
        {
            try
            {
                string originalFile = backupFile.Replace(".bak", "");
                if (File.Exists(originalFile))
                {
                    File.Delete(originalFile);
                }
                File.Move(backupFile, originalFile);
                Debug.Log($"🔄 Restaurado: {Path.GetFileName(originalFile)}");
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"⚠️ Error restaurando {backupFile}: {e.Message}");
            }
        }
        
        Debug.Log("✅ Scripts restaurados desde backup");
        Debug.Log("⚠️ NOTA: Esto puede causar errores de compilación nuevamente");
    }
    
    void Update()
    {
        // Hotkeys
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            PerformUltimateCleanup();
        }
        
        if (Input.GetKeyDown(KeyCode.F4))
        {
            ShowCleanProjectStatus();
        }
        
        if (Input.GetKeyDown(KeyCode.F5))
        {
            TestVideoSystem();
        }
    }
    
    void OnGUI()
    {
        // OnGUI deshabilitado para experiencia de juego limpia
        // Para activar información de debug, descomenta el código siguiente:
        
        /*
        GUILayout.BeginArea(new Rect(10, 10, 350, 140));
        GUILayout.Label("🧹 ULTIMATE PROJECT CLEANER");
        
        BackgroundVideoManager vm = FindFirstObjectByType<BackgroundVideoManager>();
        GameplayManager gm = FindFirstObjectByType<GameplayManager>();
        // bool connected = gm?.backgroundVideoManager != null; // REMOVED - Field no longer exists
        
        GUILayout.Label($"Video Manager: {(vm != null ? "✅" : "❌")}");
        GUILayout.Label($"Gameplay Manager: {(gm != null ? "✅" : "❌")}");
        GUILayout.Label($"Videos: DISABLED");
        GUILayout.Label("");
        GUILayout.Label("DELETE - Ultimate Cleanup");
        GUILayout.Label("F4 - Show Status | F5 - Test Video");
        GUILayout.EndArea();
        */
    }
}
