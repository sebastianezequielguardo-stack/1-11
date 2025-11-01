using UnityEngine;
using UnityEngine.Video;
using System.IO;

/// <summary>
/// Fuerza la carga de videos de fondo de manera directa
/// </summary>
public class ForceVideoLoader : MonoBehaviour
{
    [Header("Video Settings")]
    public bool enableBackgroundVideo = true;
    public bool autoLoadOnGameStart = true;
    
    [Header("Video Configuration")]
    public Vector3 videoPosition = new Vector3(0f, 0f, 50f);
    public Vector3 videoScale = new Vector3(60f, 40f, 1f);
    
    private VideoPlayer videoPlayer;
    private GameObject videoQuad;
    private RenderTexture videoRenderTexture;
    private Material videoMaterial;
    private GameplayManager gameplayManager;
    
    void Start()
    {
        gameplayManager = GameplayManager.Instance;
        
        if (enableBackgroundVideo)
        {
            SetupVideoSystem();
            
            if (autoLoadOnGameStart)
            {
                Invoke("TryLoadCurrentSongVideo", 1f);
            }
        }
    }
    
    void SetupVideoSystem()
    {
        Debug.Log("🎬 ForceVideoLoader: Configurando sistema de video...");
        
        // Crear VideoPlayer
        GameObject playerObj = new GameObject("ForceVideoPlayer");
        playerObj.transform.SetParent(transform);
        videoPlayer = playerObj.AddComponent<VideoPlayer>();
        
        // Configurar VideoPlayer
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.isLooping = true;
        videoPlayer.playOnAwake = false;
        videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
        videoPlayer.skipOnDrop = false;
        videoPlayer.waitForFirstFrame = true;
        
        // Crear RenderTexture
        videoRenderTexture = new RenderTexture(1920, 1080, 0, RenderTextureFormat.ARGB32);
        videoRenderTexture.antiAliasing = 1;
        videoRenderTexture.filterMode = FilterMode.Bilinear;
        videoRenderTexture.wrapMode = TextureWrapMode.Clamp;
        videoRenderTexture.useMipMap = false;
        videoRenderTexture.Create();
        
        videoPlayer.targetTexture = videoRenderTexture;
        
        // Crear Quad para mostrar el video
        videoQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        videoQuad.name = "ForceVideoQuad";
        videoQuad.transform.SetParent(transform);
        videoQuad.transform.position = videoPosition;
        videoQuad.transform.localScale = videoScale;
        
        // Remover collider
        DestroyImmediate(videoQuad.GetComponent<Collider>());
        
        // Crear material
        videoMaterial = new Material(Shader.Find("Unlit/Texture"));
        videoMaterial.mainTexture = videoRenderTexture;
        videoMaterial.color = Color.white;
        videoMaterial.renderQueue = 1000; // Renderizar detrás
        
        // Aplicar material
        Renderer renderer = videoQuad.GetComponent<Renderer>();
        renderer.material = videoMaterial;
        renderer.sortingOrder = -100;
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.receiveShadows = false;
        
        // Eventos
        videoPlayer.prepareCompleted += OnVideoPrepared;
        videoPlayer.errorReceived += OnVideoError;
        
        // Inicialmente oculto
        videoQuad.SetActive(false);
        
        Debug.Log("✅ ForceVideoLoader: Sistema configurado");
    }
    
    void TryLoadCurrentSongVideo()
    {
        GameManager gameManager = GameManager.Instance;
        if (gameManager == null || string.IsNullOrEmpty(gameManager.selectedSongPath))
        {
            Debug.LogWarning("🎬 No hay canción actual para cargar video");
            return;
        }
        
        string songName = gameManager.selectedSongPath;
        LoadVideoForSong(songName);
    }
    
    [ContextMenu("Load Video for Current Song")]
    public void LoadVideoForSong(string songName)
    {
        Debug.Log($"🎬 ForceVideoLoader: Buscando video para '{songName}'");
        
        string streamingAssetsPath = Application.streamingAssetsPath;
        string songFolderPath = Path.Combine(streamingAssetsPath, "Songs", songName);
        
        if (!Directory.Exists(songFolderPath))
        {
            Debug.LogWarning($"🎬 Carpeta no encontrada: {songFolderPath}");
            return;
        }
        
        // Buscar archivo de video
        string[] videoFormats = { "*.mp4", "*.webm", "*.mov", "*.avi" };
        string videoPath = null;
        
        foreach (string format in videoFormats)
        {
            string[] files = Directory.GetFiles(songFolderPath, format);
            if (files.Length > 0)
            {
                videoPath = files[0];
                break;
            }
        }
        
        if (string.IsNullOrEmpty(videoPath))
        {
            Debug.LogWarning($"🎬 No se encontró video para '{songName}'");
            return;
        }
        
        Debug.Log($"🎬 Video encontrado: {videoPath}");
        LoadVideo(videoPath);
    }
    
    void LoadVideo(string videoPath)
    {
        if (videoPlayer == null)
        {
            Debug.LogError("🎬 VideoPlayer no configurado!");
            return;
        }
        
        // Preparar URL
        string url = "file://" + videoPath.Replace("\\", "/");
        Debug.Log($"🎬 Cargando video: {url}");
        
        videoPlayer.url = url;
        videoPlayer.Prepare();
    }
    
    void OnVideoPrepared(VideoPlayer player)
    {
        Debug.Log("🎬 Video preparado exitosamente!");
        videoQuad.SetActive(true);
        
        if (gameplayManager != null && gameplayManager.isGameActive)
        {
            player.Play();
            Debug.Log("▶️ Video reproduciéndose");
        }
    }
    
    void OnVideoError(VideoPlayer player, string message)
    {
        Debug.LogError($"🎬 Error de video: {message}");
        videoQuad.SetActive(false);
    }
    
    void Update()
    {
        if (videoPlayer == null || gameplayManager == null) return;
        
        // Sincronizar con gameplay
        if (gameplayManager.isGameActive && !gameplayManager.isPaused)
        {
            if (videoPlayer.isPrepared && !videoPlayer.isPlaying)
            {
                videoPlayer.Play();
                videoQuad.SetActive(true);
            }
        }
        else
        {
            if (videoPlayer.isPlaying)
            {
                videoPlayer.Pause();
            }
        }
    }
    
    [ContextMenu("Show Video")]
    public void ShowVideo()
    {
        if (videoQuad != null)
        {
            videoQuad.SetActive(true);
            Debug.Log("🎬 Video mostrado");
        }
    }
    
    [ContextMenu("Hide Video")]
    public void HideVideo()
    {
        if (videoQuad != null)
        {
            videoQuad.SetActive(false);
            Debug.Log("🎬 Video oculto");
        }
    }
    
    [ContextMenu("Play Video")]
    public void PlayVideo()
    {
        if (videoPlayer != null && videoPlayer.isPrepared)
        {
            videoPlayer.Play();
            videoQuad.SetActive(true);
            Debug.Log("▶️ Video reproduciéndose");
        }
    }
    
    [ContextMenu("Stop Video")]
    public void StopVideo()
    {
        if (videoPlayer != null)
        {
            videoPlayer.Stop();
            videoQuad.SetActive(false);
            Debug.Log("⏹️ Video detenido");
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
