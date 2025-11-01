using UnityEngine;
using UnityEngine.Video;
using System.IO;
using System.Collections;

/// <summary>
/// Fix para que los videos funcionen tanto en Editor como en Build
/// Maneja rutas de StreamingAssets y Resources automáticamente
/// </summary>
public class VideoPlayerBuildFix : MonoBehaviour
{
    [Header("Video Configuration")]
    public VideoPlayer videoPlayer;
    public bool enableDebugLogs = true;
    
    [Header("Fallback Settings")]
    public bool useStreamingAssets = true;
    public bool useResources = false;
    public string streamingAssetsVideoFolder = "Songs";
    public string resourcesVideoFolder = "Videos";
    
    private string[] supportedFormats = { ".mp4", ".webm", ".mov", ".avi" };
    
    void Start()
    {
        // Limpiar URLs externas automáticamente
        if (videoPlayer != null && !string.IsNullOrEmpty(videoPlayer.url) && videoPlayer.url.StartsWith("file://"))
        {
            if (enableDebugLogs)
                Debug.LogWarning($"🧹 VideoPlayerBuildFix: Limpiando URL externa: {videoPlayer.url}");
            
            videoPlayer.url = "";
            videoPlayer.Stop();
        }
    }
    
    /// <summary>
    /// Carga un video con compatibilidad para Editor y Build
    /// </summary>
    public void LoadVideo(string songFolderPath, System.Action<bool> onComplete = null)
    {
        StartCoroutine(LoadVideoCoroutine(songFolderPath, onComplete));
    }
    
    IEnumerator LoadVideoCoroutine(string songFolderPath, System.Action<bool> onComplete)
    {
        string videoPath = null;
        
        // Estrategia 1: Buscar en la carpeta original (funciona en Editor)
        if (Application.isEditor)
        {
            videoPath = FindVideoInFolder(songFolderPath);
            if (!string.IsNullOrEmpty(videoPath))
            {
                if (enableDebugLogs)
                    Debug.Log($"🎬 [Editor] Video encontrado: {videoPath}");
                
                yield return StartCoroutine(LoadVideoFromPath(videoPath, onComplete));
                yield break;
            }
        }
        
        // Estrategia 2: Buscar en StreamingAssets (funciona en Build)
        if (useStreamingAssets)
        {
            string songName = Path.GetFileName(songFolderPath);
            videoPath = FindVideoInStreamingAssets(songName);
            if (!string.IsNullOrEmpty(videoPath))
            {
                if (enableDebugLogs)
                    Debug.Log($"🎬 [StreamingAssets] Video encontrado: {videoPath}");
                
                yield return StartCoroutine(LoadVideoFromPath(videoPath, onComplete));
                yield break;
            }
        }
        
        // Estrategia 3: Buscar en Resources (funciona en Build)
        if (useResources)
        {
            string songName = Path.GetFileName(songFolderPath);
            videoPath = FindVideoInResources(songName);
            if (!string.IsNullOrEmpty(videoPath))
            {
                if (enableDebugLogs)
                    Debug.Log($"🎬 [Resources] Video encontrado: {videoPath}");
                
                yield return StartCoroutine(LoadVideoFromPath(videoPath, onComplete));
                yield break;
            }
        }
        
        // No se encontró video
        if (enableDebugLogs)
            Debug.LogWarning($"🎬 No se encontró video para: {songFolderPath}");
        
        onComplete?.Invoke(false);
    }
    
    /// <summary>
    /// Busca video en carpeta original (Editor)
    /// </summary>
    string FindVideoInFolder(string folderPath)
    {
        if (!Directory.Exists(folderPath))
            return null;
            
        foreach (string format in supportedFormats)
        {
            string[] files = Directory.GetFiles(folderPath, "*" + format);
            if (files.Length > 0)
                return files[0];
        }
        
        return null;
    }
    
    /// <summary>
    /// Busca video en StreamingAssets (Build)
    /// </summary>
    string FindVideoInStreamingAssets(string songName)
    {
        string streamingPath = Path.Combine(Application.streamingAssetsPath, streamingAssetsVideoFolder);
        
        if (!Directory.Exists(streamingPath))
        {
            if (enableDebugLogs)
                Debug.LogWarning($"🎬 StreamingAssets folder not found: {streamingPath}");
            return null;
        }
        
        // Buscar en la carpeta de la canción específica
        string songFolderPath = Path.Combine(streamingPath, songName);
        if (Directory.Exists(songFolderPath))
        {
            // Buscar nombres genéricos de video en la carpeta de la canción
            string[] genericNames = { "video", "background", "bg", "movie" };
            foreach (string name in genericNames)
            {
                foreach (string format in supportedFormats)
                {
                    string videoPath = Path.Combine(songFolderPath, name + format);
                    if (File.Exists(videoPath))
                    {
                        if (enableDebugLogs)
                            Debug.Log($"🎬 Video encontrado en StreamingAssets: {videoPath}");
                        return videoPath;
                    }
                }
            }
            
            // Buscar cualquier archivo de video en la carpeta
            foreach (string format in supportedFormats)
            {
                string[] videoFiles = Directory.GetFiles(songFolderPath, "*" + format);
                if (videoFiles.Length > 0)
                {
                    if (enableDebugLogs)
                        Debug.Log($"🎬 Video encontrado en StreamingAssets: {videoFiles[0]}");
                    return videoFiles[0];
                }
            }
        }
        
        // Buscar directamente por nombre de canción (fallback)
        foreach (string format in supportedFormats)
        {
            string videoPath = Path.Combine(streamingPath, songName + format);
            if (File.Exists(videoPath))
            {
                if (enableDebugLogs)
                    Debug.Log($"🎬 Video encontrado en StreamingAssets (directo): {videoPath}");
                return videoPath;
            }
        }
        
        if (enableDebugLogs)
            Debug.LogWarning($"🎬 No se encontró video para '{songName}' en StreamingAssets");
        
        return null;
    }
    
    /// <summary>
    /// Busca video en Resources (Build)
    /// </summary>
    string FindVideoInResources(string songName)
    {
        // Resources no soporta videos directamente, pero podemos intentar
        // Esta función está aquí para completitud, pero StreamingAssets es mejor
        return null;
    }
    
    /// <summary>
    /// Carga el video desde la ruta especificada
    /// </summary>
    IEnumerator LoadVideoFromPath(string videoPath, System.Action<bool> onComplete)
    {
        if (videoPlayer == null)
        {
            if (enableDebugLogs)
                Debug.LogError("🎬 VideoPlayer no asignado!");
            onComplete?.Invoke(false);
            yield break;
        }
        
        
        try
        {
            // Preparar URL según el contexto
            string url = PrepareVideoURL(videoPath);
            
            if (enableDebugLogs)
                Debug.Log($"🎬 Cargando video: {url}");
            
            videoPlayer.url = url;
            videoPlayer.Prepare();
        }
        catch (System.Exception e)
        {
            if (enableDebugLogs)
                Debug.LogError($"🎬 Excepción cargando video: {e.Message}");
            onComplete?.Invoke(false);
            yield break;
        }
        
        // Esperar a que se prepare (fuera del try-catch para poder usar yield)
        bool prepared = false;
        bool error = false;
        
        VideoPlayer.EventHandler onPrepared = (vp) => prepared = true;
        VideoPlayer.ErrorEventHandler onError = (vp, msg) => {
            error = true;
            if (enableDebugLogs)
                Debug.LogError($"🎬 Error cargando video: {msg}");
        };
        
        videoPlayer.prepareCompleted += onPrepared;
        videoPlayer.errorReceived += onError;
        
        // Timeout de 10 segundos
        float timeout = 10f;
        float timer = 0f;
        
        while (!prepared && !error && timer < timeout)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        
        // Limpiar eventos
        videoPlayer.prepareCompleted -= onPrepared;
        videoPlayer.errorReceived -= onError;
        
        if (prepared && !error)
        {
            if (enableDebugLogs)
                Debug.Log("🎬 Video cargado exitosamente!");
            onComplete?.Invoke(true);
        }
        else
        {
            if (enableDebugLogs)
                Debug.LogError($"🎬 Error o timeout cargando video (timer: {timer}s)");
            onComplete?.Invoke(false);
        }
    }
    
    /// <summary>
    /// Prepara la URL del video usando StreamingAssets correctamente
    /// </summary>
    string PrepareVideoURL(string videoPath)
    {
        // Si la ruta ya es absoluta y contiene StreamingAssets, convertir a relativa
        string relativePath = videoPath;
        if (videoPath.Contains(Application.streamingAssetsPath))
        {
            relativePath = videoPath.Replace(Application.streamingAssetsPath, "").TrimStart('/', '\\');
        }
        
        // Usar la ruta correcta según la plataforma
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.OSXEditor:
            case RuntimePlatform.LinuxEditor:
                // En Editor, usar StreamingAssets directamente
                string editorPath = Path.Combine(Application.streamingAssetsPath, relativePath);
                return "file://" + editorPath.Replace("\\", "/");
                
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.OSXPlayer:
            case RuntimePlatform.LinuxPlayer:
                // En builds de escritorio
                string desktopPath = Path.Combine(Application.streamingAssetsPath, relativePath);
                return "file://" + desktopPath.Replace("\\", "/");
                
            case RuntimePlatform.Android:
                // En Android, StreamingAssets está dentro del APK
                return Path.Combine(Application.streamingAssetsPath, relativePath).Replace("\\", "/");
                
            case RuntimePlatform.WebGLPlayer:
                // En WebGL, usar ruta relativa
                return Application.streamingAssetsPath + "/" + relativePath.Replace("\\", "/");
                
            default:
                // Fallback
                string fallbackPath = Path.Combine(Application.streamingAssetsPath, relativePath);
                return "file://" + fallbackPath.Replace("\\", "/");
        }
    }
    
    /// <summary>
    /// Copia videos a StreamingAssets para el build
    /// </summary>
    [ContextMenu("Copy Videos to StreamingAssets")]
    public void CopyVideosToStreamingAssets()
    {
        #if UNITY_EDITOR
        string streamingAssetsPath = Path.Combine(Application.dataPath, "StreamingAssets", streamingAssetsVideoFolder);
        
        if (!Directory.Exists(streamingAssetsPath))
        {
            Directory.CreateDirectory(streamingAssetsPath);
            Debug.Log($"🎬 Created StreamingAssets folder: {streamingAssetsPath}");
        }
        
        // Buscar carpetas de canciones
        string songsPath = Path.Combine(Application.dataPath, "..", "Songs"); // Ajustar según tu estructura
        
        if (Directory.Exists(songsPath))
        {
            string[] songFolders = Directory.GetDirectories(songsPath);
            int copiedCount = 0;
            
            foreach (string songFolder in songFolders)
            {
                string songName = Path.GetFileName(songFolder);
                string videoFile = FindVideoInFolder(songFolder);
                
                if (!string.IsNullOrEmpty(videoFile))
                {
                    string fileName = Path.GetFileName(videoFile);
                    string destPath = Path.Combine(streamingAssetsPath, songName + Path.GetExtension(videoFile));
                    
                    try
                    {
                        File.Copy(videoFile, destPath, true);
                        copiedCount++;
                        Debug.Log($"🎬 Copied: {fileName} -> StreamingAssets");
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"🎬 Error copying {fileName}: {e.Message}");
                    }
                }
            }
            
            Debug.Log($"🎬 Copied {copiedCount} videos to StreamingAssets");
            UnityEditor.AssetDatabase.Refresh();
        }
        else
        {
            Debug.LogWarning($"🎬 Songs folder not found: {songsPath}");
        }
        #endif
    }
    
    /// <summary>
    /// Información de debug
    /// </summary>
    [ContextMenu("Show Video Paths Info")]
    public void ShowVideoPathsInfo()
    {
        Debug.Log("🎬 VIDEO PATHS INFO:");
        Debug.Log($"Application.dataPath: {Application.dataPath}");
        Debug.Log($"Application.streamingAssetsPath: {Application.streamingAssetsPath}");
        Debug.Log($"Application.persistentDataPath: {Application.persistentDataPath}");
        Debug.Log($"Application.platform: {Application.platform}");
        Debug.Log($"Application.isEditor: {Application.isEditor}");
        
        string streamingPath = Path.Combine(Application.streamingAssetsPath, streamingAssetsVideoFolder);
        Debug.Log($"StreamingAssets video path: {streamingPath}");
        Debug.Log($"StreamingAssets exists: {Directory.Exists(streamingPath)}");
        
        if (Directory.Exists(streamingPath))
        {
            string[] files = Directory.GetFiles(streamingPath);
            Debug.Log($"Files in StreamingAssets: {files.Length}");
            foreach (string file in files)
            {
                Debug.Log($"  - {Path.GetFileName(file)}");
            }
        }
    }
}
