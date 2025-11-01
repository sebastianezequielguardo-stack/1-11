using UnityEngine;
using UnityEngine.Video;
using System.IO;

/// <summary>
/// Helper para diagnosticar y solucionar problemas de videos
/// </summary>
public class VideoDebugHelper : MonoBehaviour
{
    [Header("Debug Settings")]
    public bool enableDebugLogs = true;
    public bool autoTestOnStart = true;
    
    [Header("Manual Testing")]
    public string testSongName = "Los Angeles Azules - Como te voy a olvidar";
    
    void Start()
    {
        if (autoTestOnStart)
        {
            Invoke("RunVideoTest", 2f); // Esperar 2 segundos para que todo se inicialice
        }
    }
    
    [ContextMenu("Test Video System")]
    public void RunVideoTest()
    {
        Debug.Log("🎬 === DIAGNÓSTICO DEL SISTEMA DE VIDEOS ===");
        
        // 1. Verificar componentes de video
        CheckVideoComponents();
        
        // 2. Verificar archivos de video
        CheckVideoFiles();
        
        // 3. Intentar cargar video manualmente
        TryLoadTestVideo();
    }
    
    void CheckVideoComponents()
    {
        Debug.Log("🔍 Verificando componentes de video...");
        
        // Buscar BackgroundVideoSystem
        BackgroundVideoSystem[] videoSystems = FindObjectsByType<BackgroundVideoSystem>(FindObjectsSortMode.None);
        Debug.Log($"BackgroundVideoSystem encontrados: {videoSystems.Length}");
        
        foreach (var system in videoSystems)
        {
            Debug.Log($"  - {system.name}: {(system.enabled ? "Habilitado" : "Deshabilitado")}");
        }
        
        // Buscar BackgroundVideoManager
        BackgroundVideoManager[] videoManagers = FindObjectsByType<BackgroundVideoManager>(FindObjectsSortMode.None);
        Debug.Log($"BackgroundVideoManager encontrados: {videoManagers.Length}");
        
        foreach (var manager in videoManagers)
        {
            Debug.Log($"  - {manager.name}: {(manager.enabled ? "Habilitado" : "Deshabilitado")}");
        }
        
        // Buscar VideoPlayer
        VideoPlayer[] videoPlayers = FindObjectsByType<VideoPlayer>(FindObjectsSortMode.None);
        Debug.Log($"VideoPlayer encontrados: {videoPlayers.Length}");
        
        foreach (var player in videoPlayers)
        {
            Debug.Log($"  - {player.name}: {(player.enabled ? "Habilitado" : "Deshabilitado")}");
            Debug.Log($"    URL: {player.url}");
            Debug.Log($"    Prepared: {player.isPrepared}");
            Debug.Log($"    Playing: {player.isPlaying}");
        }
    }
    
    void CheckVideoFiles()
    {
        Debug.Log("📁 Verificando archivos de video...");
        
        string streamingAssetsPath = Application.streamingAssetsPath;
        string songsPath = Path.Combine(streamingAssetsPath, "Songs");
        
        Debug.Log($"StreamingAssets path: {streamingAssetsPath}");
        Debug.Log($"Songs path: {songsPath}");
        Debug.Log($"Songs folder exists: {Directory.Exists(songsPath)}");
        
        if (Directory.Exists(songsPath))
        {
            string[] songFolders = Directory.GetDirectories(songsPath);
            Debug.Log($"Song folders found: {songFolders.Length}");
            
            foreach (string songFolder in songFolders)
            {
                string songName = Path.GetFileName(songFolder);
                string[] videoFiles = Directory.GetFiles(songFolder, "*.mp4");
                
                if (videoFiles.Length > 0)
                {
                    Debug.Log($"✅ {songName}: {videoFiles[0]}");
                }
                else
                {
                    Debug.Log($"❌ {songName}: No video found");
                }
            }
        }
    }
    
    void TryLoadTestVideo()
    {
        Debug.Log("🎬 Intentando cargar video de prueba...");
        
        string streamingAssetsPath = Application.streamingAssetsPath;
        string testSongPath = Path.Combine(streamingAssetsPath, "Songs", testSongName);
        
        if (Directory.Exists(testSongPath))
        {
            string[] videoFiles = Directory.GetFiles(testSongPath, "*.mp4");
            if (videoFiles.Length > 0)
            {
                string videoPath = videoFiles[0];
                Debug.Log($"Video encontrado: {videoPath}");
                
                // Buscar o crear VideoPlayer
                VideoPlayer videoPlayer = FindFirstObjectByType<VideoPlayer>();
                if (videoPlayer == null)
                {
                    GameObject videoObj = new GameObject("TestVideoPlayer");
                    videoPlayer = videoObj.AddComponent<VideoPlayer>();
                    Debug.Log("VideoPlayer creado para prueba");
                }
                
                // Configurar y cargar video
                StartCoroutine(LoadTestVideo(videoPlayer, videoPath));
            }
            else
            {
                Debug.LogError($"No se encontró video en: {testSongPath}");
            }
        }
        else
        {
            Debug.LogError($"Carpeta de canción no encontrada: {testSongPath}");
        }
    }
    
    System.Collections.IEnumerator LoadTestVideo(VideoPlayer videoPlayer, string videoPath)
    {
        Debug.Log("🎬 Configurando VideoPlayer para prueba...");
        
        // Configurar VideoPlayer
        videoPlayer.renderMode = VideoRenderMode.CameraNearPlane;
        videoPlayer.isLooping = true;
        videoPlayer.playOnAwake = false;
        videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
        
        // Preparar URL
        string url;
        if (Application.isEditor)
        {
            url = "file://" + videoPath.Replace("\\", "/");
        }
        else
        {
            url = "file://" + videoPath.Replace("\\", "/");
        }
        
        Debug.Log($"URL del video: {url}");
        
        videoPlayer.url = url;
        videoPlayer.Prepare();
        
        // Esperar a que se prepare
        float timeout = 10f;
        float timer = 0f;
        
        while (!videoPlayer.isPrepared && timer < timeout)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        
        if (videoPlayer.isPrepared)
        {
            Debug.Log("✅ Video preparado exitosamente!");
            videoPlayer.Play();
            Debug.Log("▶️ Video reproduciéndose");
        }
        else
        {
            Debug.LogError($"❌ Timeout preparando video ({timer}s)");
        }
    }
    
    [ContextMenu("Force Setup Video Systems")]
    public void ForceSetupVideoSystems()
    {
        Debug.Log("🔧 Configurando sistemas de video forzadamente...");
        
        // Buscar GameplayManager
        GameplayManager gameplayManager = FindFirstObjectByType<GameplayManager>();
        if (gameplayManager == null)
        {
            Debug.LogError("GameplayManager no encontrado!");
            return;
        }
        
        // Buscar o crear BackgroundVideoSystem
        BackgroundVideoSystem videoSystem = FindFirstObjectByType<BackgroundVideoSystem>();
        if (videoSystem == null)
        {
            GameObject videoSystemObj = new GameObject("BackgroundVideoSystem");
            videoSystem = videoSystemObj.AddComponent<BackgroundVideoSystem>();
            Debug.Log("BackgroundVideoSystem creado");
        }
        
        // Buscar o crear VideoPlayerBuildFix
        VideoPlayerBuildFix buildFix = videoSystem.GetComponent<VideoPlayerBuildFix>();
        if (buildFix == null)
        {
            buildFix = videoSystem.gameObject.AddComponent<VideoPlayerBuildFix>();
            Debug.Log("VideoPlayerBuildFix agregado");
        }
        
        Debug.Log("✅ Sistemas de video configurados");
    }
    
    [ContextMenu("Test Current Song Video")]
    public void TestCurrentSongVideo()
    {
        GameManager gameManager = GameManager.Instance;
        if (gameManager != null && !string.IsNullOrEmpty(gameManager.selectedSongPath))
        {
            string songName = gameManager.selectedSongPath;
            Debug.Log($"🎬 Probando video para canción actual: {songName}");
            
            // Buscar video para esta canción
            string streamingAssetsPath = Application.streamingAssetsPath;
            string songPath = Path.Combine(streamingAssetsPath, "Songs", songName);
            
            if (Directory.Exists(songPath))
            {
                string[] videoFiles = Directory.GetFiles(songPath, "*.mp4");
                if (videoFiles.Length > 0)
                {
                    Debug.Log($"✅ Video encontrado: {videoFiles[0]}");
                    
                    // Intentar cargar con BackgroundVideoSystem
                    BackgroundVideoSystem videoSystem = FindFirstObjectByType<BackgroundVideoSystem>();
                    if (videoSystem != null)
                    {
                        videoSystem.LoadSongVideo(songPath);
                        Debug.Log("📤 Comando de carga enviado a BackgroundVideoSystem");
                    }
                }
                else
                {
                    Debug.LogWarning($"❌ No se encontró video para: {songName}");
                }
            }
        }
        else
        {
            Debug.LogWarning("No hay canción actual cargada");
        }
    }
}
