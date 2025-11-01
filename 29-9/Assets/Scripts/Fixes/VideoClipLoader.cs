using UnityEngine;
using UnityEngine.Video;
using System.Collections;

/// <summary>
/// Sistema que usa VideoClip assets directamente en lugar de URLs
/// </summary>
public class VideoClipLoader : MonoBehaviour
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
    
    [Header("Video Assets")]
    public VideoClip[] videoClips; // Array de VideoClips asignados desde el inspector
    
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
                StartCoroutine(LoadVideoForCurrentSong());
            }
        }
    }
    
    void SetupVideoSystem()
    {
        if (showDebugLogs)
            Debug.Log("🎬 VideoClipLoader: Configurando sistema...");
        
        // Crear VideoPlayer
        GameObject playerObj = new GameObject("VideoClipPlayer");
        playerObj.transform.SetParent(transform);
        videoPlayer = playerObj.AddComponent<VideoPlayer>();
        
        // Configurar VideoPlayer para usar VideoClip
        videoPlayer.source = VideoSource.VideoClip; // IMPORTANTE: usar VideoClip source
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
            Debug.Log("✅ VideoClipLoader: Sistema configurado");
    }
    
    void CreateVideoQuad()
    {
        videoQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        videoQuad.name = "VideoClipQuad";
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
    
    IEnumerator LoadVideoForCurrentSong()
    {
        yield return new WaitForSeconds(1f); // Esperar a que GameManager esté listo
        
        GameManager gameManager = GameManager.Instance;
        if (gameManager != null && !string.IsNullOrEmpty(gameManager.selectedSongPath))
        {
            LoadVideoClipBySongName(gameManager.selectedSongPath);
        }
        else if (showDebugLogs)
        {
            Debug.LogWarning("🎬 No hay canción seleccionada");
        }
    }
    
    /// <summary>
    /// Carga un VideoClip basado en el nombre de la canción
    /// </summary>
    public void LoadVideoClipBySongName(string songName)
    {
        if (videoPlayer == null)
        {
            if (showDebugLogs)
                Debug.LogError("🎬 VideoPlayer no configurado!");
            return;
        }
        
        if (showDebugLogs)
            Debug.Log($"🎬 Buscando VideoClip para: {songName}");
        
        // Buscar VideoClip que coincida con el nombre de la canción
        VideoClip targetClip = FindVideoClipByName(songName);
        
        if (targetClip != null)
        {
            LoadVideoClip(targetClip);
        }
        else
        {
            if (showDebugLogs)
                Debug.LogWarning($"🎬 No se encontró VideoClip para: {songName}");
        }
    }
    
    /// <summary>
    /// Busca un VideoClip por nombre de canción
    /// </summary>
    VideoClip FindVideoClipByName(string songName)
    {
        if (videoClips == null || videoClips.Length == 0)
        {
            if (showDebugLogs)
                Debug.LogWarning("🎬 No hay VideoClips asignados en el array");
            return null;
        }
        
        // Buscar por nombre exacto
        foreach (VideoClip clip in videoClips)
        {
            if (clip != null && clip.name.Equals(songName, System.StringComparison.OrdinalIgnoreCase))
            {
                return clip;
            }
        }
        
        // Buscar por nombre parcial
        foreach (VideoClip clip in videoClips)
        {
            if (clip != null && (clip.name.Contains(songName) || songName.Contains(clip.name)))
            {
                if (showDebugLogs)
                    Debug.Log($"🎬 Coincidencia parcial encontrada: {clip.name}");
                return clip;
            }
        }
        
        return null;
    }
    
    /// <summary>
    /// Carga un VideoClip específico
    /// </summary>
    public void LoadVideoClip(VideoClip clip)
    {
        if (videoPlayer == null || clip == null)
        {
            if (showDebugLogs)
                Debug.LogError("🎬 VideoPlayer o VideoClip es null!");
            return;
        }
        
        if (showDebugLogs)
            Debug.Log($"🎬 Cargando VideoClip: {clip.name}");
        
        // Asignar VideoClip directamente
        videoPlayer.clip = clip;
        videoPlayer.Prepare();
    }
    
    void OnVideoPrepared(VideoPlayer player)
    {
        if (showDebugLogs)
            Debug.Log("🎬 VideoClip preparado!");
        
        isVideoLoaded = true;
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
            Debug.Log("▶️ VideoClip iniciado!");
    }
    
    void OnVideoError(VideoPlayer player, string message)
    {
        if (showDebugLogs)
            Debug.LogError($"🎬 Error de VideoClip: {message}");
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
        
        // Actualizar transformación
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
            LoadVideoClipBySongName(gameManager.selectedSongPath);
        }
    }
    
    [ContextMenu("Show VideoClips Info")]
    public void ShowVideoClipsInfo()
    {
        Debug.Log("🎬 INFORMACIÓN DE VIDEOCLIPS:");
        Debug.Log("═══════════════════════════");
        Debug.Log($"VideoClips asignados: {(videoClips != null ? videoClips.Length : 0)}");
        
        if (videoClips != null)
        {
            for (int i = 0; i < videoClips.Length; i++)
            {
                if (videoClips[i] != null)
                {
                    Debug.Log($"  [{i}] {videoClips[i].name} ({videoClips[i].length:F1}s)");
                }
                else
                {
                    Debug.Log($"  [{i}] NULL");
                }
            }
        }
        
        Debug.Log($"VideoPlayer configurado: {(videoPlayer != null ? "Sí" : "No")}");
        Debug.Log($"VideoClip actual: {(videoPlayer != null && videoPlayer.clip != null ? videoPlayer.clip.name : "Ninguno")}");
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
