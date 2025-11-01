using UnityEngine;
using UnityEngine.Video;
using System.IO;
using System.Collections;

/// <summary>
/// Sistema de videos de fondo para gameplay - Implementación desde cero
/// Opacidad 100%, carga asíncrona, sincronización perfecta con gameplay
/// </summary>
public class BackgroundVideoSystem : MonoBehaviour
{
    [Header("Video Configuration")]
    public bool enableBackgroundVideo = true;
    public float videoLoadTimeout = 8f;

    [Header("Display Settings")]
    public Vector3 videoPosition = new Vector3(0f, 0f, 50f); // Detrás del highway
    public Vector3 videoRotation = new Vector3(0f, 0f, 0f);  // Rotación del video
    public Vector3 videoScale = new Vector3(60f, 40f, 1f);   // Pantalla completa

    [Header("Debug")]
    public bool showDebugInfo = false;

    // Componentes principales
    private VideoPlayer videoPlayer;
    private RenderTexture videoRenderTexture;
    private GameObject videoQuad;
    private Material videoMaterial;
    private VideoPlayerBuildFix buildFix;

    // Estado del sistema
    private bool videoLoaded = false;
    private bool isLoadingVideo = false;
    private string currentVideoPath = "";

    // Formatos soportados (ordenados por velocidad de carga)
    private readonly string[] supportedFormats = { ".mp4", ".webm", ".mov", ".avi" };

    void Start()
    {
        InitializeVideoSystem();
    }

    /// <summary>
    /// Inicializa todo el sistema de video
    /// </summary>
    void InitializeVideoSystem()
    {
        CreateVideoPlayer();
        CreateRenderTexture();
        CreateVideoQuad();
        SetupVideoMaterial();
        SetupBuildFix();

        // Background Video System inicializado
    }

    /// <summary>
    /// Crea el VideoPlayer con configuración optimizada
    /// </summary>
    void CreateVideoPlayer()
    {
        // Crear GameObject para el VideoPlayer
        GameObject playerObj = new GameObject("BackgroundVideoPlayer");
        playerObj.transform.SetParent(transform);
        videoPlayer = playerObj.AddComponent<VideoPlayer>();

        // Configuración optimizada para alta calidad
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.isLooping = true;
        videoPlayer.playOnAwake = false;
        videoPlayer.audioOutputMode = VideoAudioOutputMode.None; // Sin audio del video
        videoPlayer.skipOnDrop = false; // No saltar frames para mejor calidad
        videoPlayer.waitForFirstFrame = true; // Esperar primer frame para mejor sincronización

        // Eventos
        videoPlayer.prepareCompleted += OnVideoPrepared;
        videoPlayer.errorReceived += OnVideoError;
        videoPlayer.started += OnVideoStarted;
    }

    /// <summary>
    /// Crea la RenderTexture para el video con alta calidad
    /// </summary>
    void CreateRenderTexture()
    {
        // Usar resolución nativa de pantalla para máxima calidad
        int width = Mathf.Max(1920, Screen.width);
        int height = Mathf.Max(1080, Screen.height);

        videoRenderTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);

        // Configuración para alta calidad
        videoRenderTexture.antiAliasing = 1; // Sin antialiasing para mejor rendimiento
        videoRenderTexture.filterMode = FilterMode.Bilinear; // Filtrado suave
        videoRenderTexture.wrapMode = TextureWrapMode.Clamp;
        videoRenderTexture.useMipMap = false; // Sin mipmaps para videos

        videoRenderTexture.Create();
        videoPlayer.targetTexture = videoRenderTexture;

        if (showDebugInfo)
        {
            // RenderTexture creada
        }
    }

    /// <summary>
    /// Crea el quad que mostrará el video
    /// </summary>
    void CreateVideoQuad()
    {
        videoQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        videoQuad.name = "BackgroundVideoQuad";
        videoQuad.transform.SetParent(transform);

        // Posicionar y rotar detrás de todo el gameplay
        videoQuad.transform.position = videoPosition;
        videoQuad.transform.rotation = Quaternion.Euler(videoRotation);
        videoQuad.transform.localScale = videoScale;

        // Remover el collider (no necesario)
        DestroyImmediate(videoQuad.GetComponent<Collider>());

        // Inicialmente oculto
        videoQuad.SetActive(false);
    }

    /// <summary>
    /// Configura el material del video con opacidad 100% y alta calidad
    /// </summary>
    void SetupVideoMaterial()
    {
        // Usar shader Unlit/Texture para mejor rendimiento y calidad
        videoMaterial = new Material(Shader.Find("Unlit/Texture"));
        videoMaterial.mainTexture = videoRenderTexture;

        // Opacidad al 100% (completamente opaco)
        Color color = Color.white;
        color.a = 1.0f; // 100% opacidad
        videoMaterial.color = color;

        // Configuración para alta calidad visual
        videoMaterial.mainTextureScale = Vector2.one;
        videoMaterial.mainTextureOffset = Vector2.zero;

        // Configurar para renderizar como fondo
        videoMaterial.renderQueue = 1000; // Renderizar primero

        // Aplicar material al quad
        Renderer renderer = videoQuad.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = videoMaterial;
            renderer.sortingOrder = -100; // Renderizar detrás de todo

            // Configurar para mejor calidad de renderizado
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = false;
            renderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        }

        if (showDebugInfo)
        {
            // Material configurado
        }
    }

    /// <summary>
    /// Configura el sistema de compatibilidad para builds
    /// </summary>
    void SetupBuildFix()
    {
        buildFix = gameObject.GetComponent<VideoPlayerBuildFix>();
        if (buildFix == null)
        {
            buildFix = gameObject.AddComponent<VideoPlayerBuildFix>();
        }
        
        buildFix.videoPlayer = videoPlayer;
        buildFix.enableDebugLogs = showDebugInfo;
    }

    /// <summary>
    /// Carga el video de la canción actual (llamado por GameplayManager)
    /// </summary>
    public void LoadSongVideo(string songFolderPath)
    {
        if (!enableBackgroundVideo || isLoadingVideo)
            return;
            
        // Verificar si el GameObject está activo antes de iniciar corrutina
        if (!gameObject.activeInHierarchy)
        {
            if (showDebugInfo)
            {
                Debug.LogWarning("🎬 BackgroundVideoSystem está inactivo, no se puede cargar video");
            }
            return;
        }

        StartCoroutine(LoadVideoAsync(songFolderPath));
    }

    /// <summary>
    /// Carga el video de forma asíncrona usando BuildFix
    /// </summary>
    IEnumerator LoadVideoAsync(string songFolderPath)
    {
        isLoadingVideo = true;
        videoLoaded = false;

        if (showDebugInfo)
        {
            Debug.Log($"🎬 Cargando video para: {songFolderPath}");
        }

        // Usar BuildFix para cargar el video
        bool loadCompleted = false;
        bool loadSuccess = false;

        buildFix.LoadVideo(songFolderPath, (success) => {
            loadCompleted = true;
            loadSuccess = success;
            videoLoaded = success;
        });

        // Esperar a que termine la carga
        float timeout = videoLoadTimeout;
        float timer = 0f;

        while (!loadCompleted && timer < timeout)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        if (loadCompleted && loadSuccess)
        {
            if (showDebugInfo)
            {
                Debug.Log("🎬 Video cargado exitosamente con BuildFix");
            }
            
            // Auto-iniciar si el gameplay está activo
            if (IsGameplayActive())
            {
                PlayVideo();
            }
        }
        else
        {
            if (showDebugInfo)
            {
                Debug.LogWarning($"🎬 Error o timeout cargando video (timer: {timer}s)");
            }
        }

        isLoadingVideo = false;
    }

    /// <summary>
    /// Busca el archivo de video en la carpeta de la canción
    /// </summary>
    string FindVideoFile(string songFolderPath)
    {
        if (!Directory.Exists(songFolderPath))
            return null;

        // Buscar por formato (MP4 primero por velocidad)
        foreach (string format in supportedFormats)
        {
            string[] files = Directory.GetFiles(songFolderPath, "*" + format);
            if (files.Length > 0)
            {
                return files[0];
            }
        }

        // Buscar nombres comunes
        string[] commonNames = { "video", "background", "bg", "movie" };
        foreach (string name in commonNames)
        {
            foreach (string format in supportedFormats)
            {
                string path = Path.Combine(songFolderPath, name + format);
                if (File.Exists(path))
                {
                    return path;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Carga el video con sistema de timeout
    /// </summary>
    IEnumerator LoadVideoWithTimeout(string videoPath)
    {
        // Iniciar carga del video
        try
        {
            string url = "file://" + videoPath.Replace("\\", "/");
            videoPlayer.url = url;
            videoPlayer.Prepare();

            if (showDebugInfo)
            {
                // Preparando video
            }
        }
        catch (System.Exception)
        {
            // Error cargando video
            yield break;
        }

        // Esperar con timeout
        float timer = 0f;
        while (!videoLoaded && timer < videoLoadTimeout)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // Manejar timeout
        if (!videoLoaded && timer >= videoLoadTimeout)
        {
            // Timeout cargando video
            HideVideo();
        }
    }

    /// <summary>
    /// Eventos del VideoPlayer
    /// </summary>
    void OnVideoPrepared(VideoPlayer player)
    {
        videoLoaded = true;

        if (showDebugInfo)
        {
            // Video preparado
        }

        // Auto-iniciar si el gameplay está activo
        if (IsGameplayActive())
        {
            PlayVideo();
        }
    }

    void OnVideoStarted(VideoPlayer player)
    {
        ShowVideo();

        if (showDebugInfo)
        {
            // Video iniciado
        }
    }

    void OnVideoError(VideoPlayer player, string message)
    {
        // Error de video
        HideVideo();
        videoLoaded = false;
    }

    /// <summary>
    /// Verifica si el gameplay está activo
    /// </summary>
    bool IsGameplayActive()
    {
        GameplayManager gm = GameplayManager.Instance;
        return gm != null && gm.isGameActive && !gm.isPaused;
    }

    /// <summary>
    /// Verifica si el juego está pausado (incluyendo SimplePauseSetup)
    /// </summary>
    bool IsGamePaused()
    {
        // Verificar GameplayManager
        GameplayManager gm = GameplayManager.Instance;
        if (gm != null && gm.isPaused)
            return true;

        // Verificar SimplePauseSetup
        SimplePauseSetup pauseSetup = FindFirstObjectByType<SimplePauseSetup>();
        if (pauseSetup != null && pauseSetup.IsPaused)
            return true;

        // Verificar Time.timeScale
        if (Time.timeScale == 0f)
            return true;

        return false;
    }

    /// <summary>
    /// Controles públicos del video
    /// </summary>
    public void PlayVideo()
    {
        if (videoPlayer != null && videoLoaded && enableBackgroundVideo)
        {
            videoPlayer.Play();

            if (showDebugInfo)
            {
                // Video reproduciéndose
            }
        }
    }

    public void PauseVideo()
    {
        if (videoPlayer != null && videoPlayer.isPlaying)
        {
            videoPlayer.Pause();

            if (showDebugInfo)
            {
                // Video pausado
            }
        }
    }

    public void StopVideo()
    {
        if (videoPlayer != null)
        {
            videoPlayer.Stop();
            HideVideo();

            if (showDebugInfo)
            {
                // Video detenido
            }
        }
    }

    public void ShowVideo()
    {
        if (videoQuad != null && enableBackgroundVideo)
        {
            videoQuad.SetActive(true);
        }
    }

    public void HideVideo()
    {
        if (videoQuad != null)
        {
            videoQuad.SetActive(false);
        }
    }

    /// <summary>
    /// Sincronización automática con el gameplay
    /// </summary>
    void Update()
    {
        if (!enableBackgroundVideo) return;

        GameplayManager gm = GameplayManager.Instance;
        if (gm == null) return;

        // Sincronizar con el estado del gameplay usando detección mejorada de pausa
        if (IsGamePaused())
        {
            // Juego pausado - pausar video
            if (videoPlayer.isPlaying)
            {
                PauseVideo();
            }
        }
        else if (gm.isGameActive)
        {
            // Gameplay activo - reproducir video si está cargado
            if (videoLoaded && !videoPlayer.isPlaying)
            {
                PlayVideo();
            }
        }
        else if (!gm.isGameActive)
        {
            // Gameplay terminado - detener video
            if (videoPlayer.isPlaying)
            {
                StopVideo();
            }
        }

        // Actualizar transform si se cambian valores en el inspector durante runtime
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

    /// <summary>
    /// Métodos de configuración
    /// </summary>
    public void EnableBackgroundVideo(bool enable)
    {
        enableBackgroundVideo = enable;

        if (!enable)
        {
            StopVideo();
        }
        else if (videoLoaded && IsGameplayActive())
        {
            PlayVideo();
        }
    }

    /// <summary>
    /// Configura la rotación del video
    /// </summary>
    public void SetVideoRotation(Vector3 rotation)
    {
        videoRotation = rotation;
        if (videoQuad != null)
        {
            videoQuad.transform.rotation = Quaternion.Euler(videoRotation);
        }

        if (showDebugInfo)
        {
            // Rotación actualizada
        }
    }

    /// <summary>
    /// Configura la rotación del video con valores individuales
    /// </summary>
    public void SetVideoRotation(float x, float y, float z)
    {
        SetVideoRotation(new Vector3(x, y, z));
    }

    /// <summary>
    /// Rota el video en el eje Z (más común para videos)
    /// </summary>
    public void SetVideoRotationZ(float zRotation)
    {
        SetVideoRotation(videoRotation.x, videoRotation.y, zRotation);
    }

    /// <summary>
    /// Actualiza posición, rotación y escala del video
    /// </summary>
    public void UpdateVideoTransform()
    {
        if (videoQuad != null)
        {
            videoQuad.transform.position = videoPosition;
            videoQuad.transform.rotation = Quaternion.Euler(videoRotation);
            videoQuad.transform.localScale = videoScale;

            if (showDebugInfo)
            {
                // Transform actualizado
            }
        }
    }

    public bool IsVideoLoaded()
    {
        return videoLoaded;
    }

    public bool IsVideoPlaying()
    {
        return videoPlayer != null && videoPlayer.isPlaying;
    }

    /// <summary>
    /// Debug GUI - Deshabilitado para gameplay limpio
    /// </summary>
    void OnGUI()
    {
        // Debug GUI deshabilitado para experiencia de juego limpia
        // Para activar debug, cambiar showDebugInfo = true en el inspector
        if (!showDebugInfo) return;

        GUILayout.BeginArea(new Rect(10, 10, 300, 150));
        GUILayout.Box("🎬 BACKGROUND VIDEO SYSTEM");
        GUILayout.Label($"Enabled: {enableBackgroundVideo}");
        GUILayout.Label($"Video Loaded: {videoLoaded}");
        GUILayout.Label($"Video Playing: {IsVideoPlaying()}");
        GUILayout.Label($"Loading: {isLoadingVideo}");
        GUILayout.Label($"Current Video: {Path.GetFileName(currentVideoPath)}");
        GUILayout.Label($"Opacity: 100%");
        GUILayout.EndArea();
    }

    /// <summary>
    /// Limpieza al destruir
    /// </summary>
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
