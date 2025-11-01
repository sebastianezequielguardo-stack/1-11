using UnityEngine;
using UnityEngine.Video;
using System.IO;
using System.Collections;

/// <summary>
/// Sistema que busca videos en una carpeta externa junto al ejecutable
/// </summary>
public class ExternalFolderVideoLoader : MonoBehaviour
{
    [Header("Video Settings")]
    public bool enableBackgroundVideo = true;
    public bool autoLoadOnStart = true;
    public bool showDebugLogs = true;
    
    [Header("External Folder Settings")]
    public string externalVideosFolderName = "Videos"; // Carpeta junto al .exe
    public bool fallbackToStreamingAssets = true; // Si no encuentra en carpeta externa
    
    [Header("Video Display")]
    public Vector3 videoPosition = new Vector3(0f, 0f, 50f);
    public Vector3 videoRotation = new Vector3(0f, 0f, 0f);
    public Vector3 videoScale = new Vector3(60f, 40f, 1f);
    public int renderQueue = 1000;
    public int sortingOrder = -100;
    
    private VideoPlayer videoPlayer;
    private GameObject videoQuad;
    private RenderTexture videoRenderTexture;
    private Material videoMaterial;
    private bool isVideoLoaded = false;
    private string externalVideosPath;
    
    void Start()
    {
        if (enableBackgroundVideo)
        {
            SetupExternalPath();
            SetupVideoSystem();
            
            if (autoLoadOnStart)
            {
                StartCoroutine(LoadVideoDelayed());
            }
        }
    }
    
    void SetupExternalPath()
    {
        // Obtener la ruta del ejecutable
        string executablePath = Application.dataPath;
        
        if (Application.isEditor)
        {
            // En Editor, usar la carpeta del proyecto
            externalVideosPath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, externalVideosFolderName);
        }
        else
        {
            // En Build, usar la carpeta junto al ejecutable
            string executableDir = Directory.GetParent(executablePath).FullName;
            externalVideosPath = Path.Combine(executableDir, externalVideosFolderName);
        }
        
        if (showDebugLogs)
        {
            Debug.Log($"🎬 Ruta de videos externa: {externalVideosPath}");
            Debug.Log($"🎬 Carpeta existe: {Directory.Exists(externalVideosPath)}");
        }
        
        // Crear la carpeta si no existe (solo en Editor para testing)
        if (Application.isEditor && !Directory.Exists(externalVideosPath))
        {
            try
            {
                Directory.CreateDirectory(externalVideosPath);
                if (showDebugLogs)
                    Debug.Log($"🎬 Carpeta de videos creada: {externalVideosPath}");
            }
            catch (System.Exception e)
            {
                if (showDebugLogs)
                    Debug.LogError($"🎬 Error creando carpeta: {e.Message}");
            }
        }
    }
    
    void SetupVideoSystem()
    {
        if (showDebugLogs)
            Debug.Log("🎬 ExternalFolderVideoLoader: Configurando...");
        
        // Crear VideoPlayer
        GameObject playerObj = new GameObject("ExternalVideoPlayer");
        playerObj.transform.SetParent(transform);
        videoPlayer = playerObj.AddComponent<VideoPlayer>();
        
        // Configuración
        videoPlayer.source = VideoSource.Url;
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.isLooping = true;
        videoPlayer.playOnAwake = false;
        videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
        videoPlayer.skipOnDrop = false;
        videoPlayer.waitForFirstFrame = true;
        
        // Crear RenderTexture
        videoRenderTexture = new RenderTexture(1920, 1080, 0, RenderTextureFormat.ARGB32);
        videoRenderTexture.Create();
        videoPlayer.targetTexture = videoRenderTexture;
        
        // Crear Quad
        CreateVideoQuad();
        
        // Eventos
        videoPlayer.prepareCompleted += OnVideoPrepared;
        videoPlayer.errorReceived += OnVideoError;
        
        if (showDebugLogs)
            Debug.Log("✅ ExternalFolderVideoLoader: Sistema listo");
    }
    
    void CreateVideoQuad()
    {
        videoQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        videoQuad.name = "ExternalVideoQuad";
        videoQuad.transform.SetParent(transform);
        videoQuad.transform.position = videoPosition;
        videoQuad.transform.rotation = Quaternion.Euler(videoRotation);
        videoQuad.transform.localScale = videoScale;
        
        DestroyImmediate(videoQuad.GetComponent<Collider>());
        
        // Material
        videoMaterial = new Material(Shader.Find("Unlit/Texture"));
        videoMaterial.mainTexture = videoRenderTexture;
        videoMaterial.color = Color.white;
        videoMaterial.renderQueue = renderQueue;
        
        Renderer renderer = videoQuad.GetComponent<Renderer>();
        renderer.material = videoMaterial;
        renderer.sortingOrder = sortingOrder;
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.receiveShadows = false;
        
        videoQuad.SetActive(false);
    }
    
    IEnumerator LoadVideoDelayed()
    {
        yield return new WaitForSeconds(1f);
        
        GameManager gameManager = GameManager.Instance;
        if (gameManager != null && !string.IsNullOrEmpty(gameManager.selectedSongPath))
        {
            yield return StartCoroutine(LoadVideoForSong(gameManager.selectedSongPath));
        }
    }
    
    IEnumerator LoadVideoForSong(string songName)
    {
        if (showDebugLogs)
            Debug.Log($"🎬 Buscando video para: {songName}");
        
        string videoPath = null;
        
        // 1. Buscar en carpeta externa primero
        videoPath = FindVideoInExternalFolder(songName);
        
        // 2. Si no encuentra y está habilitado el fallback, buscar en StreamingAssets
        if (string.IsNullOrEmpty(videoPath) && fallbackToStreamingAssets)
        {
            videoPath = FindVideoInStreamingAssets(songName);
        }
        
        if (string.IsNullOrEmpty(videoPath))
        {
            if (showDebugLogs)
                Debug.LogWarning($"🎬 No se encontró video para: {songName}");
            yield break;
        }
        
        // Construir URL
        string videoUrl = "file://" + videoPath.Replace("\\", "/");
        
        if (showDebugLogs)
            Debug.Log($"🎬 Cargando video: {videoUrl}");
        
        // Cargar video
        videoPlayer.url = videoUrl;
        videoPlayer.Prepare();
        
        // Esperar preparación
        float timeout = 10f;
        float timer = 0f;
        
        while (!videoPlayer.isPrepared && timer < timeout)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        
        if (videoPlayer.isPrepared)
        {
            isVideoLoaded = true;
            if (showDebugLogs)
                Debug.Log("✅ Video cargado desde carpeta externa!");
        }
        else
        {
            if (showDebugLogs)
                Debug.LogError($"❌ Timeout cargando video ({timer}s)");
        }
    }
    
    string FindVideoInExternalFolder(string songName)
    {
        if (!Directory.Exists(externalVideosPath))
        {
            if (showDebugLogs)
                Debug.LogWarning($"🎬 Carpeta externa no existe: {externalVideosPath}");
            return null;
        }
        
        // Buscar video con el nombre de la canción
        string[] videoExtensions = { ".mp4", ".webm", ".mov", ".avi" };
        
        // Buscar archivo directo con nombre de canción
        foreach (string ext in videoExtensions)
        {
            string directPath = Path.Combine(externalVideosPath, songName + ext);
            if (File.Exists(directPath))
            {
                if (showDebugLogs)
                    Debug.Log($"🎬 Video encontrado (directo): {directPath}");
                return directPath;
            }
        }
        
        // Buscar en subcarpeta con nombre de canción
        string songFolder = Path.Combine(externalVideosPath, songName);
        if (Directory.Exists(songFolder))
        {
            foreach (string ext in videoExtensions)
            {
                string[] files = Directory.GetFiles(songFolder, "*" + ext);
                if (files.Length > 0)
                {
                    if (showDebugLogs)
                        Debug.Log($"🎬 Video encontrado (subcarpeta): {files[0]}");
                    return files[0];
                }
            }
        }
        
        // Buscar cualquier video que contenga el nombre de la canción
        foreach (string ext in videoExtensions)
        {
            string[] files = Directory.GetFiles(externalVideosPath, "*" + ext, SearchOption.AllDirectories);
            foreach (string file in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                if (fileName.Contains(songName) || songName.Contains(fileName))
                {
                    if (showDebugLogs)
                        Debug.Log($"🎬 Video encontrado (coincidencia parcial): {file}");
                    return file;
                }
            }
        }
        
        return null;
    }
    
    string FindVideoInStreamingAssets(string songName)
    {
        string songPath = Path.Combine(Application.streamingAssetsPath, "Songs", songName);
        
        if (!Directory.Exists(songPath))
            return null;
        
        string[] videoExtensions = { ".mp4", ".webm", ".mov", ".avi" };
        
        foreach (string ext in videoExtensions)
        {
            string[] files = Directory.GetFiles(songPath, "*" + ext);
            if (files.Length > 0)
            {
                if (showDebugLogs)
                    Debug.Log($"🎬 Video encontrado en StreamingAssets: {files[0]}");
                return files[0];
            }
        }
        
        return null;
    }
    
    void OnVideoPrepared(VideoPlayer player)
    {
        if (showDebugLogs)
            Debug.Log("🎬 Video preparado!");
        
        videoQuad.SetActive(true);
        
        GameplayManager gameplayManager = GameplayManager.Instance;
        if (gameplayManager != null && gameplayManager.isGameActive)
        {
            player.Play();
        }
    }
    
    void OnVideoError(VideoPlayer player, string message)
    {
        if (showDebugLogs)
            Debug.LogError($"🎬 Error: {message}");
        videoQuad.SetActive(false);
    }
    
    void Update()
    {
        if (!isVideoLoaded || videoPlayer == null) return;
        
        GameplayManager gameplayManager = GameplayManager.Instance;
        if (gameplayManager == null) return;
        
        // Sincronizar con gameplay
        if (gameplayManager.isGameActive && !gameplayManager.isPaused)
        {
            if (videoPlayer.isPrepared && !videoPlayer.isPlaying)
            {
                videoPlayer.Play();
                videoQuad.SetActive(true);
            }
        }
        else if (gameplayManager.isPaused)
        {
            if (videoPlayer.isPlaying)
            {
                videoPlayer.Pause();
            }
        }
        
        // Actualizar transform
        UpdateVideoTransform();
    }
    
    void UpdateVideoTransform()
    {
        if (videoQuad != null)
        {
            if (videoQuad.transform.position != videoPosition ||
                videoQuad.transform.rotation != Quaternion.Euler(videoRotation) ||
                videoQuad.transform.localScale != videoScale)
            {
                videoQuad.transform.position = videoPosition;
                videoQuad.transform.rotation = Quaternion.Euler(videoRotation);
                videoQuad.transform.localScale = videoScale;
            }
        }
    }
    
    [ContextMenu("Load Current Song Video")]
    public void LoadCurrentSongVideo()
    {
        GameManager gameManager = GameManager.Instance;
        if (gameManager != null && !string.IsNullOrEmpty(gameManager.selectedSongPath))
        {
            StartCoroutine(LoadVideoForSong(gameManager.selectedSongPath));
        }
    }
    
    [ContextMenu("Show External Folder Info")]
    public void ShowExternalFolderInfo()
    {
        Debug.Log("🎬 INFORMACIÓN DE CARPETA EXTERNA:");
        Debug.Log("═══════════════════════════════");
        Debug.Log($"Ruta externa: {externalVideosPath}");
        Debug.Log($"Carpeta existe: {Directory.Exists(externalVideosPath)}");
        Debug.Log($"Fallback a StreamingAssets: {fallbackToStreamingAssets}");
        
        if (Directory.Exists(externalVideosPath))
        {
            string[] videoFiles = Directory.GetFiles(externalVideosPath, "*.mp4", SearchOption.AllDirectories);
            Debug.Log($"Videos MP4 encontrados: {videoFiles.Length}");
            
            foreach (string file in videoFiles)
            {
                Debug.Log($"  - {file}");
            }
        }
        
        // Info del juego actual
        GameManager gameManager = GameManager.Instance;
        if (gameManager != null && !string.IsNullOrEmpty(gameManager.selectedSongPath))
        {
            Debug.Log($"Canción actual: {gameManager.selectedSongPath}");
            string videoPath = FindVideoInExternalFolder(gameManager.selectedSongPath);
            Debug.Log($"Video para canción actual: {(string.IsNullOrEmpty(videoPath) ? "No encontrado" : videoPath)}");
        }
    }
    
    [ContextMenu("Open External Videos Folder")]
    public void OpenExternalVideosFolder()
    {
        if (Directory.Exists(externalVideosPath))
        {
            System.Diagnostics.Process.Start(externalVideosPath);
            if (showDebugLogs)
                Debug.Log($"🎬 Abriendo carpeta: {externalVideosPath}");
        }
        else
        {
            if (showDebugLogs)
                Debug.LogWarning($"🎬 Carpeta no existe: {externalVideosPath}");
        }
    }
    
    void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.prepareCompleted -= OnVideoPrepared;
            videoPlayer.errorReceived -= OnVideoError;
        }
        
        if (videoRenderTexture != null)
        {
            videoRenderTexture.Release();
        }
    }
}
