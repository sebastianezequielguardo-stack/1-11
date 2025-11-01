using UnityEngine;
using UnityEngine.Video;
using System.IO;

/// <summary>
/// Fuerza el uso exclusivo de StreamingAssets para videos, limpiando URLs externas
/// </summary>
public class ForceStreamingAssetsOnly : MonoBehaviour
{
    [Header("Cleanup Settings")]
    public bool cleanupOnStart = true;
    public bool disableExternalVideoPlayers = true;
    public bool showDebugLogs = true;
    
    void Start()
    {
        if (cleanupOnStart)
        {
            CleanupExternalVideoPlayers();
        }
    }
    
    [ContextMenu("Cleanup External Video Players")]
    public void CleanupExternalVideoPlayers()
    {
        if (showDebugLogs)
            Debug.Log("🧹 Limpiando VideoPlayers con URLs externas...");
        
        // Encontrar todos los VideoPlayers
        VideoPlayer[] allVideoPlayers = FindObjectsByType<VideoPlayer>(FindObjectsSortMode.None);
        
        int cleanedCount = 0;
        
        foreach (VideoPlayer vp in allVideoPlayers)
        {
            if (vp == null) continue;
            
            // Verificar si tiene URL externa (file://)
            if (!string.IsNullOrEmpty(vp.url) && vp.url.StartsWith("file://"))
            {
                if (showDebugLogs)
                    Debug.Log($"🧹 Limpiando VideoPlayer: {vp.name} (URL: {vp.url})");
                
                // Limpiar URL externa
                vp.url = "";
                vp.Stop();
                
                // Si está configurado, desactivar el VideoPlayer
                if (disableExternalVideoPlayers)
                {
                    vp.enabled = false;
                    if (showDebugLogs)
                        Debug.Log($"❌ VideoPlayer {vp.name} desactivado");
                }
                
                cleanedCount++;
            }
        }
        
        if (showDebugLogs)
            Debug.Log($"✅ Limpiados {cleanedCount} VideoPlayers con URLs externas");
        
        // Verificar que StreamingAssetsVideoLoader esté activo
        EnsureStreamingAssetsVideoLoader();
    }
    
    void EnsureStreamingAssetsVideoLoader()
    {
        StreamingAssetsVideoLoader[] streamingLoaders = FindObjectsByType<StreamingAssetsVideoLoader>(FindObjectsSortMode.None);
        
        if (streamingLoaders.Length == 0)
        {
            if (showDebugLogs)
                Debug.LogWarning("⚠️ No se encontró StreamingAssetsVideoLoader activo");
            
            // Crear uno automáticamente
            CreateStreamingAssetsVideoLoader();
        }
        else
        {
            // Asegurar que esté habilitado
            foreach (var loader in streamingLoaders)
            {
                if (!loader.enabled)
                {
                    loader.enabled = true;
                    if (showDebugLogs)
                        Debug.Log($"✅ StreamingAssetsVideoLoader {loader.name} activado");
                }
            }
        }
    }
    
    void CreateStreamingAssetsVideoLoader()
    {
        GameObject loaderObj = new GameObject("StreamingAssetsVideoLoader");
        StreamingAssetsVideoLoader loader = loaderObj.AddComponent<StreamingAssetsVideoLoader>();
        
        // Configuración por defecto
        loader.enableBackgroundVideo = true;
        loader.autoLoadOnStart = true;
        loader.showDebugLogs = true;
        
        if (showDebugLogs)
            Debug.Log("✅ StreamingAssetsVideoLoader creado automáticamente");
    }
    
    [ContextMenu("Force StreamingAssets Video Load")]
    public void ForceStreamingAssetsVideoLoad()
    {
        // Limpiar primero
        CleanupExternalVideoPlayers();
        
        // Buscar StreamingAssetsVideoLoader
        StreamingAssetsVideoLoader loader = FindFirstObjectByType<StreamingAssetsVideoLoader>();
        
        if (loader != null)
        {
            // Forzar carga del video actual
            loader.LoadCurrentSongVideo();
            if (showDebugLogs)
                Debug.Log("🎬 Forzando carga de video desde StreamingAssets");
        }
        else
        {
            if (showDebugLogs)
                Debug.LogError("❌ No se encontró StreamingAssetsVideoLoader");
        }
    }
    
    [ContextMenu("Show Video System Status")]
    public void ShowVideoSystemStatus()
    {
        Debug.Log("📊 ESTADO DEL SISTEMA DE VIDEOS:");
        Debug.Log("═══════════════════════════════");
        
        // VideoPlayers
        VideoPlayer[] allVideoPlayers = FindObjectsByType<VideoPlayer>(FindObjectsSortMode.None);
        Debug.Log($"VideoPlayers encontrados: {allVideoPlayers.Length}");
        
        foreach (VideoPlayer vp in allVideoPlayers)
        {
            string status = vp.enabled ? "Activo" : "Inactivo";
            string url = string.IsNullOrEmpty(vp.url) ? "Sin URL" : vp.url;
            Debug.Log($"  - {vp.name}: {status} | URL: {url}");
        }
        
        // StreamingAssetsVideoLoader
        StreamingAssetsVideoLoader[] loaders = FindObjectsByType<StreamingAssetsVideoLoader>(FindObjectsSortMode.None);
        Debug.Log($"StreamingAssetsVideoLoader encontrados: {loaders.Length}");
        
        foreach (var loader in loaders)
        {
            string status = loader.enabled ? "Activo" : "Inactivo";
            Debug.Log($"  - {loader.name}: {status}");
        }
        
        // BackgroundVideoSystem
        BackgroundVideoSystem[] bgSystems = FindObjectsByType<BackgroundVideoSystem>(FindObjectsSortMode.None);
        Debug.Log($"BackgroundVideoSystem encontrados: {bgSystems.Length}");
        
        foreach (var bg in bgSystems)
        {
            string status = bg.enabled ? "Activo" : "Inactivo";
            Debug.Log($"  - {bg.name}: {status}");
        }
        
        // StreamingAssets info
        Debug.Log($"StreamingAssets path: {Application.streamingAssetsPath}");
        string songsPath = Path.Combine(Application.streamingAssetsPath, "Songs");
        Debug.Log($"Songs folder exists: {Directory.Exists(songsPath)}");
        
        if (Directory.Exists(songsPath))
        {
            string[] songFolders = Directory.GetDirectories(songsPath);
            Debug.Log($"Song folders: {songFolders.Length}");
        }
    }
    
    [ContextMenu("Reset All Video Systems")]
    public void ResetAllVideoSystems()
    {
        if (showDebugLogs)
            Debug.Log("🔄 Reiniciando todos los sistemas de video...");
        
        // Desactivar todos los VideoPlayers externos
        VideoPlayer[] allVideoPlayers = FindObjectsByType<VideoPlayer>(FindObjectsSortMode.None);
        foreach (VideoPlayer vp in allVideoPlayers)
        {
            if (vp.name.Contains("StreamingVideoPlayer")) continue; // Mantener el de StreamingAssets
            
            vp.Stop();
            vp.url = "";
            vp.enabled = false;
        }
        
        // Desactivar BackgroundVideoSystem
        BackgroundVideoSystem[] bgSystems = FindObjectsByType<BackgroundVideoSystem>(FindObjectsSortMode.None);
        foreach (var bg in bgSystems)
        {
            bg.enabled = false;
        }
        
        // Asegurar StreamingAssetsVideoLoader
        EnsureStreamingAssetsVideoLoader();
        
        if (showDebugLogs)
            Debug.Log("✅ Sistemas de video reiniciados - Solo StreamingAssets activo");
    }
}
