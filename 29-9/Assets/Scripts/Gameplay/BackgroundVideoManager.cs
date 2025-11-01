using UnityEngine;
using UnityEngine.Video;
using System.IO;
using System.Collections;

/// <summary>
/// Manages background videos for songs during gameplay
/// </summary>
public class BackgroundVideoManager : MonoBehaviour
{
    [Header("Video Settings")]
    public VideoPlayer videoPlayer;
    public RenderTexture videoRenderTexture;
    public GameObject videoQuad; // Quad to display the video
    
    [Header("Video Configuration")]
    public bool enableBackgroundVideo = true;
    public float videoOpacity = 1.0f; // Sin opacidad - video completamente visible
    public Vector3 videoPosition = new Vector3(0f, 0f, 50f); // Far behind everything
    public Vector3 videoScale = new Vector3(50f, 30f, 1f); // Cover entire screen
    
    [Header("Supported Formats")]
    public string[] supportedExtensions = { ".mp4", ".mov", ".avi", ".webm" };
    
    [Header("Loading Settings")]
    public float videoLoadTimeout = 10f; // Timeout for video loading
    public bool skipVideoOnTimeout = true; // Skip video if it takes too long
    
    private Material videoMaterial;
    private string currentSongPath;
    private bool videoLoaded = false;
    
    void Start()
    {
        InitializeVideoSystem();
    }
    
    void InitializeVideoSystem()
    {
        // Create video player if not assigned
        if (videoPlayer == null)
        {
            GameObject videoPlayerObj = new GameObject("VideoPlayer");
            videoPlayerObj.transform.parent = transform;
            videoPlayer = videoPlayerObj.AddComponent<VideoPlayer>();
        }
        
        // Create render texture for video
        if (videoRenderTexture == null)
        {
            videoRenderTexture = new RenderTexture(1920, 1080, 0);
            videoRenderTexture.Create();
        }
        
        // Create video display quad if not assigned
        if (videoQuad == null)
        {
            CreateVideoQuad();
        }
        
        // Configure video player
        SetupVideoPlayer();
        
        Debug.Log("🎬 Background Video Manager initialized");
    }
    
    void CreateVideoQuad()
    {
        videoQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        videoQuad.name = "BackgroundVideoQuad";
        videoQuad.transform.parent = transform;
        
        // Position behind the highway
        videoQuad.transform.position = videoPosition;
        videoQuad.transform.localScale = videoScale;
        videoQuad.transform.rotation = Quaternion.identity;
        
        // Remove collider
        Collider collider = videoQuad.GetComponent<Collider>();
        if (collider != null)
        {
            DestroyImmediate(collider);
        }
        
        // Create material for video
        CreateVideoMaterial();
    }
    
    void CreateVideoMaterial()
    {
        // Use Unlit/Texture shader for full screen background video
        videoMaterial = new Material(Shader.Find("Unlit/Texture"));
        videoMaterial.mainTexture = videoRenderTexture;
        
        // Set as background - render first (lowest queue)
        videoMaterial.renderQueue = 1000; // Render before everything else
        
        // Set opacity (can be adjusted for dimming effect)
        Color color = videoMaterial.color;
        color.a = videoOpacity;
        videoMaterial.color = color;
        
        // Apply material to quad
        Renderer renderer = videoQuad.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = videoMaterial;
            // Ensure it renders behind everything
            renderer.sortingOrder = -100;
        }
    }
    
    void SetupVideoPlayer()
    {
        if (videoPlayer == null) return;
        
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.targetTexture = videoRenderTexture;
        videoPlayer.isLooping = true;
        videoPlayer.playOnAwake = false;
        videoPlayer.audioOutputMode = VideoAudioOutputMode.None; // No audio from video
        
        // Subscribe to events
        videoPlayer.prepareCompleted += OnVideoPrepared;
        videoPlayer.errorReceived += OnVideoError;
    }
    
    /// <summary>
    /// Load and play background video for a song (async, non-blocking)
    /// </summary>
    public void LoadSongVideo(string songFolderPath)
    {
        if (!enableBackgroundVideo)
        {
            HideVideo();
            return;
        }
        
        // Start async loading
        StartCoroutine(LoadSongVideoCoroutine(songFolderPath));
    }
    
    IEnumerator LoadSongVideoCoroutine(string songFolderPath)
    {
        currentSongPath = songFolderPath;
        
        // Quick check for video file
        string videoPath = FindVideoInSongFolder(songFolderPath);
        
        if (string.IsNullOrEmpty(videoPath))
        {
            Debug.Log($"🎬 No background video found for song in: {songFolderPath}");
            HideVideo();
            yield break;
        }
        
        Debug.Log($"🎬 Found video: {videoPath} - Loading asynchronously...");
        
        // Start loading with timeout
        StartCoroutine(LoadVideoWithTimeout(videoPath));
        yield break;
    }
    
    string FindVideoInSongFolder(string songFolderPath)
    {
        if (!Directory.Exists(songFolderPath))
        {
            Debug.LogWarning($"🎬 Song folder not found: {songFolderPath}");
            return null;
        }
        
        // Try to use format optimizer for fastest loading
        // VideoFormatOptimizer optimizer = GetComponent<VideoFormatOptimizer>(); // REMOVED - Script deleted
        // if (optimizer != null)
        // {
        //     string optimizedVideo = optimizer.FindFastestVideoFormat(songFolderPath);
        //     if (!string.IsNullOrEmpty(optimizedVideo))
        //     {
        //         return optimizedVideo;
        //     }
        // }
        
        // Fallback: Look for video files in order of speed (MP4 first)
        string[] fastFormats = { ".mp4", ".webm", ".mov", ".avi" };
        foreach (string extension in fastFormats)
        {
            string[] videoFiles = Directory.GetFiles(songFolderPath, "*" + extension);
            if (videoFiles.Length > 0)
            {
                Debug.Log($"🎬 Found background video: {videoFiles[0]} (Format: {extension})");
                return videoFiles[0];
            }
        }
        
        // Look for common video names
        string[] commonNames = { "background", "video", "bg", "movie" };
        foreach (string name in commonNames)
        {
            foreach (string extension in supportedExtensions)
            {
                string videoPath = Path.Combine(songFolderPath, name + extension);
                if (File.Exists(videoPath))
                {
                    Debug.Log($"🎬 Found background video: {videoPath}");
                    return videoPath;
                }
            }
        }
        
        return null;
    }
    
    IEnumerator LoadVideoWithTimeout(string videoPath)
    {
        bool loadingComplete = false;
        bool loadingFailed = false;
        
        // Start loading
        try
        {
            videoPlayer.url = "file://" + videoPath.Replace("\\", "/");
            videoPlayer.Prepare();
            
            Debug.Log($"🎬 Starting video preparation: {videoPath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"🎬 Error starting video load: {e.Message}");
            HideVideo();
            yield break;
        }
        
        // Wait for loading with timeout
        float timeoutTimer = 0f;
        
        while (!loadingComplete && !loadingFailed && timeoutTimer < videoLoadTimeout)
        {
            if (videoLoaded)
            {
                loadingComplete = true;
                break;
            }
            
            timeoutTimer += Time.deltaTime;
            yield return null;
        }
        
        // Handle timeout
        if (timeoutTimer >= videoLoadTimeout && !loadingComplete)
        {
            Debug.LogWarning($"🎬 Video loading timeout ({videoLoadTimeout}s) - Skipping video");
            if (skipVideoOnTimeout)
            {
                HideVideo();
            }
        }
    }
    
    void LoadVideo(string videoPath)
    {
        if (videoPlayer == null) return;
        
        try
        {
            videoPlayer.url = "file://" + videoPath.Replace("\\", "/");
            videoPlayer.Prepare();
            
            Debug.Log($"🎬 Loading background video: {videoPath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"🎬 Error loading video: {e.Message}");
            HideVideo();
        }
    }
    
    void OnVideoPrepared(VideoPlayer player)
    {
        videoLoaded = true;
        ShowVideo();
        
        // Start video when gameplay starts
        if (GameplayManager.Instance != null && GameplayManager.Instance.isGameActive)
        {
            PlayVideo();
        }
        
        Debug.Log("🎬 Background video prepared and ready");
    }
    
    void OnVideoError(VideoPlayer player, string message)
    {
        Debug.LogError($"🎬 Video error: {message}");
        HideVideo();
    }
    
    /// <summary>
    /// Start playing the background video
    /// </summary>
    public void PlayVideo()
    {
        if (videoPlayer != null && videoLoaded && enableBackgroundVideo)
        {
            videoPlayer.Play();
            ShowVideo();
            Debug.Log("🎬 Background video started");
        }
    }
    
    /// <summary>
    /// Pause the background video
    /// </summary>
    public void PauseVideo()
    {
        if (videoPlayer != null && videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
            Debug.Log("🎬 Background video paused");
        }
    }
    
    /// <summary>
    /// Stop the background video
    /// </summary>
    public void StopVideo()
    {
        if (videoPlayer != null)
        {
            videoPlayer.Stop();
            HideVideo();
            Debug.Log("🎬 Background video stopped");
        }
    }
    
    /// <summary>
    /// Show the video quad
    /// </summary>
    public void ShowVideo()
    {
        if (videoQuad != null)
        {
            videoQuad.SetActive(true);
        }
    }
    
    /// <summary>
    /// Hide the video quad
    /// </summary>
    public void HideVideo()
    {
        if (videoQuad != null)
        {
            videoQuad.SetActive(false);
        }
    }
    
    /// <summary>
    /// Set video opacity
    /// </summary>
    public void SetVideoOpacity(float opacity)
    {
        videoOpacity = Mathf.Clamp01(opacity);
        
        if (videoMaterial != null)
        {
            Color color = videoMaterial.color;
            color.a = videoOpacity;
            videoMaterial.color = color;
        }
    }
    
    /// <summary>
    /// Toggle background video on/off
    /// </summary>
    public void ToggleBackgroundVideo(bool enabled)
    {
        enableBackgroundVideo = enabled;
        
        if (enabled && videoLoaded)
        {
            ShowVideo();
            if (GameplayManager.Instance != null && GameplayManager.Instance.isGameActive)
            {
                PlayVideo();
            }
        }
        else
        {
            HideVideo();
            PauseVideo();
        }
    }
    
    void Update()
    {
        // Sync video with gameplay state
        if (GameplayManager.Instance != null)
        {
            if (GameplayManager.Instance.isGameActive && !GameplayManager.Instance.isPaused)
            {
                if (videoLoaded && !videoPlayer.isPlaying && enableBackgroundVideo)
                {
                    PlayVideo();
                }
            }
            else
            {
                if (videoPlayer.isPlaying)
                {
                    PauseVideo();
                }
            }
        }
    }
    
    void OnDestroy()
    {
        // Clean up
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
