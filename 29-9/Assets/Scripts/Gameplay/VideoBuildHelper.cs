using UnityEngine;
using UnityEngine.Video;
using System.IO;
using System.Linq;

/// <summary>
/// Helper para configurar videos automáticamente en builds
/// </summary>
public class VideoBuildHelper : MonoBehaviour
{
    [Header("Auto Setup")]
    public bool autoSetupOnStart = true;
    public bool enableDebugLogs = true;
    
    void Start()
    {
        if (autoSetupOnStart)
        {
            SetupVideoSystemsForBuild();
        }
    }
    
    /// <summary>
    /// Configura automáticamente todos los sistemas de video para funcionar en builds
    /// </summary>
    [ContextMenu("Setup Video Systems for Build")]
    public void SetupVideoSystemsForBuild()
    {
        if (enableDebugLogs)
            Debug.Log("🎬 Configurando sistemas de video para builds...");
        
        // Encontrar todos los VideoPlayer en la escena
        VideoPlayer[] videoPlayers = FindObjectsByType<VideoPlayer>(FindObjectsSortMode.None);
        
        foreach (VideoPlayer vp in videoPlayers)
        {
            SetupVideoPlayerForBuild(vp);
        }
        
        // Encontrar BackgroundVideoSystem
        BackgroundVideoSystem[] videoSystems = FindObjectsByType<BackgroundVideoSystem>(FindObjectsSortMode.None);
        
        foreach (BackgroundVideoSystem vs in videoSystems)
        {
            SetupBackgroundVideoSystemForBuild(vs);
        }
        
        // Encontrar BackgroundVideoManager
        BackgroundVideoManager[] videoManagers = FindObjectsByType<BackgroundVideoManager>(FindObjectsSortMode.None);
        
        foreach (BackgroundVideoManager vm in videoManagers)
        {
            SetupBackgroundVideoManagerForBuild(vm);
        }
        
        if (enableDebugLogs)
            Debug.Log($"✅ Configurados {videoPlayers.Length} VideoPlayers, {videoSystems.Length} VideoSystems, {videoManagers.Length} VideoManagers");
    }
    
    /// <summary>
    /// Configura un VideoPlayer individual para builds
    /// </summary>
    void SetupVideoPlayerForBuild(VideoPlayer videoPlayer)
    {
        if (videoPlayer == null) return;
        
        // Configuración optimizada para builds
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
        videoPlayer.isLooping = true;
        videoPlayer.playOnAwake = false;
        videoPlayer.skipOnDrop = false;
        videoPlayer.waitForFirstFrame = true;
        
        // Agregar BuildFix si no existe
        VideoPlayerBuildFix buildFix = videoPlayer.GetComponent<VideoPlayerBuildFix>();
        if (buildFix == null)
        {
            buildFix = videoPlayer.gameObject.AddComponent<VideoPlayerBuildFix>();
            buildFix.videoPlayer = videoPlayer;
            buildFix.enableDebugLogs = enableDebugLogs;
            
            if (enableDebugLogs)
                Debug.Log($"🎬 Agregado VideoPlayerBuildFix a {videoPlayer.name}");
        }
    }
    
    /// <summary>
    /// Configura BackgroundVideoSystem para builds
    /// </summary>
    void SetupBackgroundVideoSystemForBuild(BackgroundVideoSystem videoSystem)
    {
        if (videoSystem == null) return;
        
        // Habilitar debug si está configurado
        if (enableDebugLogs)
        {
            // Usar reflection para acceder al campo privado showDebugInfo
            var field = typeof(BackgroundVideoSystem).GetField("showDebugInfo", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(videoSystem, true);
            }
        }
        
        if (enableDebugLogs)
            Debug.Log($"🎬 Configurado BackgroundVideoSystem: {videoSystem.name}");
    }
    
    /// <summary>
    /// Configura BackgroundVideoManager para builds
    /// </summary>
    void SetupBackgroundVideoManagerForBuild(BackgroundVideoManager videoManager)
    {
        if (videoManager == null) return;
        
        // Asegurar que está habilitado
        videoManager.enableBackgroundVideo = true;
        
        // Agregar BuildFix si no existe
        VideoPlayerBuildFix buildFix = videoManager.GetComponent<VideoPlayerBuildFix>();
        if (buildFix == null && videoManager.videoPlayer != null)
        {
            buildFix = videoManager.gameObject.AddComponent<VideoPlayerBuildFix>();
            buildFix.videoPlayer = videoManager.videoPlayer;
            buildFix.enableDebugLogs = enableDebugLogs;
            
            if (enableDebugLogs)
                Debug.Log($"🎬 Agregado VideoPlayerBuildFix a {videoManager.name}");
        }
    }
    
    /// <summary>
    /// Verifica la configuración de StreamingAssets
    /// </summary>
    [ContextMenu("Verify StreamingAssets Setup")]
    public void VerifyStreamingAssetsSetup()
    {
        string streamingAssetsPath = Path.Combine(Application.dataPath, "StreamingAssets");
        string songsPath = Path.Combine(streamingAssetsPath, "Songs");
        
        Debug.Log("🎬 VERIFICACIÓN DE STREAMINGASSETS:");
        Debug.Log($"StreamingAssets path: {streamingAssetsPath}");
        Debug.Log($"StreamingAssets exists: {Directory.Exists(streamingAssetsPath)}");
        Debug.Log($"Songs path: {songsPath}");
        Debug.Log($"Songs exists: {Directory.Exists(songsPath)}");
        
        if (Directory.Exists(songsPath))
        {
            string[] songFolders = Directory.GetDirectories(songsPath);
            Debug.Log($"Song folders found: {songFolders.Length}");
            
            int videosFound = 0;
            foreach (string songFolder in songFolders)
            {
                string songName = Path.GetFileName(songFolder);
                
                // Buscar archivos de video de todos los formatos
                var videoFilesList = new System.Collections.Generic.List<string>();
                videoFilesList.AddRange(Directory.GetFiles(songFolder, "*.mp4"));
                videoFilesList.AddRange(Directory.GetFiles(songFolder, "*.webm"));
                videoFilesList.AddRange(Directory.GetFiles(songFolder, "*.mov"));
                videoFilesList.AddRange(Directory.GetFiles(songFolder, "*.avi"));
                string[] videoFiles = videoFilesList.ToArray();
                
                if (videoFiles.Length > 0)
                {
                    videosFound++;
                    Debug.Log($"  ✅ {songName}: {videoFiles.Length} video(s)");
                }
                else
                {
                    Debug.Log($"  ❌ {songName}: No videos");
                }
            }
            
            Debug.Log($"Total videos found: {videosFound}/{songFolders.Length}");
        }
    }
    
    /// <summary>
    /// Test de carga de video
    /// </summary>
    [ContextMenu("Test Video Loading")]
    public void TestVideoLoading()
    {
        string streamingAssetsPath = Application.streamingAssetsPath;
        string songsPath = Path.Combine(streamingAssetsPath, "Songs");
        
        if (Directory.Exists(songsPath))
        {
            string[] songFolders = Directory.GetDirectories(songsPath);
            if (songFolders.Length > 0)
            {
                string testSongFolder = songFolders[0];
                string songName = Path.GetFileName(testSongFolder);
                
                Debug.Log($"🎬 Probando carga de video para: {songName}");
                
                VideoPlayerBuildFix buildFix = GetComponent<VideoPlayerBuildFix>();
                if (buildFix == null)
                {
                    buildFix = gameObject.AddComponent<VideoPlayerBuildFix>();
                    
                    // Crear VideoPlayer temporal para la prueba
                    GameObject tempPlayer = new GameObject("TempVideoPlayer");
                    VideoPlayer vp = tempPlayer.AddComponent<VideoPlayer>();
                    buildFix.videoPlayer = vp;
                }
                
                buildFix.LoadVideo(testSongFolder, (success) => {
                    if (success)
                    {
                        Debug.Log("✅ Test de carga de video exitoso!");
                    }
                    else
                    {
                        Debug.LogError("❌ Test de carga de video falló!");
                    }
                });
            }
        }
    }
}
