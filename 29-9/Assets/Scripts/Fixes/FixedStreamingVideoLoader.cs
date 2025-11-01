using UnityEngine;
using UnityEngine.Video;
using System.IO;
using System.Collections;

/// <summary>
/// Sistema de video corregido que usa StreamingAssets con URLs relativas correctas
/// </summary>
public class FixedStreamingVideoLoader : MonoBehaviour
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
                StartCoroutine(LoadVideoDelayed());
            }
        }
    }
    
    void SetupVideoSystem()
    {
        if (showDebugLogs)
            Debug.Log("🎬 FixedStreamingVideoLoader: Configurando...");
        
        // Crear VideoPlayer
        GameObject playerObj = new GameObject("FixedVideoPlayer");
        playerObj.transform.SetParent(transform);
        videoPlayer = playerObj.AddComponent<VideoPlayer>();
        
        // Configuración correcta para StreamingAssets
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
            Debug.Log("✅ FixedStreamingVideoLoader: Sistema listo");
    }
    
    void CreateVideoQuad()
    {
        videoQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        videoQuad.name = "FixedVideoQuad";
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
            yield return StartCoroutine(LoadVideoFromStreamingAssets(gameManager.selectedSongPath));
        }
    }
    
    IEnumerator LoadVideoFromStreamingAssets(string songName)
    {
        if (showDebugLogs)
            Debug.Log($"🎬 Buscando video para: {songName}");
        
        // Buscar video en StreamingAssets
        string videoFileName = FindVideoFile(songName);
        if (string.IsNullOrEmpty(videoFileName))
        {
            if (showDebugLogs)
                Debug.LogWarning($"🎬 No se encontró video para: {songName}");
            yield break;
        }
        
        // Construir URL correcta
        string videoUrl = BuildCorrectVideoURL(songName, videoFileName);
        
        if (showDebugLogs)
            Debug.Log($"🎬 URL del video: {videoUrl}");
        
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
                Debug.Log("✅ Video cargado desde StreamingAssets!");
        }
        else
        {
            if (showDebugLogs)
                Debug.LogError($"❌ Timeout cargando video ({timer}s)");
        }
    }
    
    string FindVideoFile(string songName)
    {
        string songPath = Path.Combine(Application.streamingAssetsPath, "Songs", songName);
        
        if (!Directory.Exists(songPath))
            return null;
        
        string[] videoExtensions = { "*.mp4", "*.webm", "*.mov", "*.avi" };
        
        foreach (string extension in videoExtensions)
        {
            string[] files = Directory.GetFiles(songPath, extension);
            if (files.Length > 0)
            {
                return Path.GetFileName(files[0]);
            }
        }
        
        return null;
    }
    
    string BuildCorrectVideoURL(string songName, string videoFileName)
    {
        // Construir la ruta relativa desde StreamingAssets
        string relativePath = $"Songs/{songName}/{videoFileName}";
        
        // URL según la plataforma
        if (Application.platform == RuntimePlatform.Android)
        {
            // Android: usar la ruta de StreamingAssets directamente
            return Path.Combine(Application.streamingAssetsPath, relativePath).Replace("\\", "/");
        }
        else if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            // WebGL: URL relativa
            return $"{Application.streamingAssetsPath}/{relativePath}";
        }
        else
        {
            // Desktop (Windows, Mac, Linux): file:// con StreamingAssets
            string fullPath = Path.Combine(Application.streamingAssetsPath, relativePath);
            return "file://" + fullPath.Replace("\\", "/");
        }
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
        
        // Verificar archivos
        GameManager gameManager = GameManager.Instance;
        if (gameManager != null && !string.IsNullOrEmpty(gameManager.selectedSongPath))
        {
            string songPath = Path.Combine(Application.streamingAssetsPath, "Songs", gameManager.selectedSongPath);
            Debug.Log($"Carpeta de canción: {songPath}");
            Debug.Log($"Carpeta existe: {Directory.Exists(songPath)}");
            
            if (Directory.Exists(songPath))
            {
                string[] files = Directory.GetFiles(songPath, "*.mp4");
                Debug.Log($"Archivos MP4 encontrados: {files.Length}");
                foreach (string file in files)
                {
                    Debug.Log($"  - {Path.GetFileName(file)}");
                }
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
                Debug.Log($"🎬 Transform actualizado");
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
