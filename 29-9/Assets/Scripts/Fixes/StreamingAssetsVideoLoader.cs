using UnityEngine;
using UnityEngine.Video;
using System.IO;
using System.Collections;

/// <summary>
/// Sistema de carga de videos desde StreamingAssets usando las APIs correctas de Unity
/// </summary>
public class StreamingAssetsVideoLoader : MonoBehaviour
{
    [Header("Video Settings")]
    public bool enableBackgroundVideo = true;
    public bool autoLoadOnStart = true;
    public bool showDebugLogs = true;
    
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
    
    void Start()
    {
        if (enableBackgroundVideo)
        {
            SetupVideoSystem();
            
            if (autoLoadOnStart)
            {
                StartCoroutine(LoadCurrentSongVideoDelayed());
            }
        }
    }
    
    IEnumerator LoadCurrentSongVideoDelayed()
    {
        // Esperar a que GameManager esté listo
        yield return new WaitForSeconds(1f);
        
        GameManager gameManager = GameManager.Instance;
        if (gameManager != null && !string.IsNullOrEmpty(gameManager.selectedSongPath))
        {
            yield return StartCoroutine(LoadVideoFromStreamingAssets(gameManager.selectedSongPath));
        }
        else if (showDebugLogs)
        {
            Debug.LogWarning("🎬 No hay canción seleccionada para cargar video");
        }
    }
    
    void SetupVideoSystem()
    {
        if (showDebugLogs)
            Debug.Log("🎬 StreamingAssetsVideoLoader: Configurando sistema...");
        
        // Crear VideoPlayer
        GameObject playerObj = new GameObject("StreamingVideoPlayer");
        playerObj.transform.SetParent(transform);
        videoPlayer = playerObj.AddComponent<VideoPlayer>();
        
        // Configurar VideoPlayer para StreamingAssets
        videoPlayer.source = VideoSource.Url; // Importante: usar URL source
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
        
        // Crear Quad para mostrar el video
        CreateVideoQuad();
        
        // Eventos
        videoPlayer.prepareCompleted += OnVideoPrepared;
        videoPlayer.errorReceived += OnVideoError;
        videoPlayer.started += OnVideoStarted;
        
        if (showDebugLogs)
            Debug.Log("✅ StreamingAssetsVideoLoader: Sistema configurado");
    }
    
    void CreateVideoQuad()
    {
        videoQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        videoQuad.name = "StreamingVideoQuad";
        videoQuad.transform.SetParent(transform);
        videoQuad.transform.position = videoPosition;
        videoQuad.transform.rotation = Quaternion.Euler(videoRotation);
        videoQuad.transform.localScale = videoScale;
        
        // Remover collider
        DestroyImmediate(videoQuad.GetComponent<Collider>());
        
        // Crear material
        videoMaterial = new Material(Shader.Find("Unlit/Texture"));
        videoMaterial.mainTexture = videoRenderTexture;
        videoMaterial.color = Color.white;
        videoMaterial.renderQueue = renderQueue;
        
        // Aplicar material
        Renderer renderer = videoQuad.GetComponent<Renderer>();
        renderer.material = videoMaterial;
        renderer.sortingOrder = sortingOrder;
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.receiveShadows = false;
        
        // Inicialmente oculto
        videoQuad.SetActive(false);
    }
    
    /// <summary>
    /// Carga video desde StreamingAssets usando la ruta correcta según la plataforma
    /// </summary>
    public IEnumerator LoadVideoFromStreamingAssets(string songName)
    {
        if (videoPlayer == null)
        {
            if (showDebugLogs)
                Debug.LogError("🎬 VideoPlayer no configurado!");
            yield break;
        }
        
        if (showDebugLogs)
            Debug.Log($"🎬 Buscando video para: {songName}");
        
        // Buscar archivo de video
        string videoPath = FindVideoInStreamingAssets(songName);
        if (string.IsNullOrEmpty(videoPath))
        {
            if (showDebugLogs)
                Debug.LogWarning($"🎬 No se encontró video para: {songName}");
            yield break;
        }
        
        // Preparar URL según la plataforma
        string videoUrl = GetStreamingAssetsURL(videoPath);
        
        if (showDebugLogs)
            Debug.Log($"🎬 Cargando video: {videoUrl}");
        
        // Configurar y preparar video
        videoPlayer.url = videoUrl;
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
            isVideoLoaded = true;
            if (showDebugLogs)
                Debug.Log("✅ Video cargado exitosamente desde StreamingAssets!");
        }
        else
        {
            if (showDebugLogs)
                Debug.LogError($"❌ Timeout cargando video ({timer}s)");
        }
    }
    
    /// <summary>
    /// Busca archivo de video en StreamingAssets
    /// </summary>
    string FindVideoInStreamingAssets(string songName)
    {
        string songsPath = Path.Combine(Application.streamingAssetsPath, "Songs", songName);
        
        if (!Directory.Exists(songsPath))
        {
            if (showDebugLogs)
                Debug.LogWarning($"🎬 Carpeta no encontrada: {songsPath}");
            return null;
        }
        
        // Buscar archivos de video en orden de preferencia
        string[] videoFormats = { "*.mp4", "*.webm", "*.mov", "*.avi" };
        
        foreach (string format in videoFormats)
        {
            string[] files = Directory.GetFiles(songsPath, format);
            if (files.Length > 0)
            {
                // Retornar ruta relativa desde StreamingAssets
                string relativePath = Path.Combine("Songs", songName, Path.GetFileName(files[0]));
                return relativePath.Replace("\\", "/"); // Normalizar separadores
            }
        }
        
        return null;
    }
    
    /// <summary>
    /// Obtiene la URL correcta para StreamingAssets según la plataforma
    /// </summary>
    string GetStreamingAssetsURL(string relativePath)
    {
        string streamingAssetsPath = Application.streamingAssetsPath;
        
        // En diferentes plataformas, StreamingAssets se maneja diferente
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.OSXEditor:
            case RuntimePlatform.LinuxEditor:
                // En Editor, usar file:// directamente
                string fullPath = Path.Combine(streamingAssetsPath, relativePath);
                return "file://" + fullPath.Replace("\\", "/");
                
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.OSXPlayer:
            case RuntimePlatform.LinuxPlayer:
                // En builds de escritorio, usar file:// con StreamingAssets
                string desktopPath = Path.Combine(streamingAssetsPath, relativePath);
                return "file://" + desktopPath.Replace("\\", "/");
                
            case RuntimePlatform.Android:
                // En Android, StreamingAssets está dentro del APK
                return Path.Combine(streamingAssetsPath, relativePath).Replace("\\", "/");
                
            case RuntimePlatform.WebGLPlayer:
                // En WebGL, usar ruta relativa
                return streamingAssetsPath + "/" + relativePath;
                
            default:
                // Fallback genérico
                string fallbackPath = Path.Combine(streamingAssetsPath, relativePath);
                return "file://" + fallbackPath.Replace("\\", "/");
        }
    }
    
    void OnVideoPrepared(VideoPlayer player)
    {
        if (showDebugLogs)
            Debug.Log("🎬 Video preparado!");
        
        videoQuad.SetActive(true);
        
        // Auto-play si el juego está activo
        GameplayManager gameplayManager = GameplayManager.Instance;
        if (gameplayManager != null && gameplayManager.isGameActive)
        {
            player.Play();
        }
    }
    
    void OnVideoStarted(VideoPlayer player)
    {
        if (showDebugLogs)
            Debug.Log("▶️ Video iniciado!");
    }
    
    void OnVideoError(VideoPlayer player, string message)
    {
        if (showDebugLogs)
            Debug.LogError($"🎬 Error de video: {message}");
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
        
        // Actualizar transformación si se cambian valores en el inspector durante runtime
        UpdateVideoTransform();
    }
    
    /// <summary>
    /// Actualiza la transformación del video (posición, rotación, escala)
    /// </summary>
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
    
    [ContextMenu("Update Video Transform")]
    public void ForceUpdateVideoTransform()
    {
        if (videoQuad != null)
        {
            videoQuad.transform.position = videoPosition;
            videoQuad.transform.rotation = Quaternion.Euler(videoRotation);
            videoQuad.transform.localScale = videoScale;
            
            if (showDebugLogs)
                Debug.Log($"🎬 Transform actualizado - Pos: {videoPosition}, Rot: {videoRotation}, Scale: {videoScale}");
        }
    }
    
    [ContextMenu("Load Current Song Video")]
    public void LoadCurrentSongVideo()
    {
        GameManager gameManager = GameManager.Instance;
        if (gameManager != null && !string.IsNullOrEmpty(gameManager.selectedSongPath))
        {
            StartCoroutine(LoadVideoFromStreamingAssets(gameManager.selectedSongPath));
        }
    }
    
    [ContextMenu("Show Video Info")]
    public void ShowVideoInfo()
    {
        Debug.Log("🎬 INFORMACIÓN DEL VIDEO:");
        Debug.Log("═══════════════════════════");
        Debug.Log($"Video cargado: {isVideoLoaded}");
        Debug.Log($"VideoPlayer preparado: {(videoPlayer != null ? videoPlayer.isPrepared.ToString() : "null")}");
        Debug.Log($"VideoPlayer reproduciendo: {(videoPlayer != null ? videoPlayer.isPlaying.ToString() : "null")}");
        Debug.Log($"URL actual: {(videoPlayer != null ? videoPlayer.url : "null")}");
        Debug.Log($"StreamingAssets path: {Application.streamingAssetsPath}");
        Debug.Log($"Plataforma: {Application.platform}");
    }
    
    void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.prepareCompleted -= OnVideoPrepared;
            videoPlayer.errorReceived -= OnVideoError;
            videoPlayer.started -= OnVideoStarted;
        }
        
        if (videoRenderTexture != null)
        {
            videoRenderTexture.Release();
        }
    }
}
