using UnityEngine;

/// <summary>
/// Integra todos los sistemas de gameplay
/// Coordina loading, video, detección de fin y datos
/// </summary>
public class GameplaySystemIntegrator : MonoBehaviour
{
    [Header("System References")]
    public GameplayLoadingSystem loadingSystem;
    public BackgroundVideoManager videoManager;
    public SongEndDetector songEndDetector;
    
    [Header("Auto Setup")]
    public bool autoSetupSystems = true;
    public bool showDebugLogs = true;
    
    void Awake()
    {
        if (autoSetupSystems)
        {
            SetupAllSystems();
        }
    }
    
    /// <summary>
    /// Configura todos los sistemas automáticamente
    /// </summary>
    [ContextMenu("Setup All Systems")]
    public void SetupAllSystems()
    {
        if (showDebugLogs)
        {
            Debug.Log("🔧 Configurando sistemas de gameplay...");
        }
        
        // Crear o encontrar GameplayLoadingSystem
        SetupLoadingSystem();
        
        // Crear o encontrar BackgroundVideoManager
        SetupVideoSystem();
        
        // Crear o encontrar SongEndDetector
        SetupSongEndDetector();
        
        if (showDebugLogs)
        {
            Debug.Log("✅ Todos los sistemas configurados correctamente");
        }
    }
    
    /// <summary>
    /// Configura el sistema de carga
    /// </summary>
    void SetupLoadingSystem()
    {
        if (loadingSystem == null)
        {
            loadingSystem = FindFirstObjectByType<GameplayLoadingSystem>();
        }
        
        if (loadingSystem == null)
        {
            GameObject loadingObj = new GameObject("GameplayLoadingSystem");
            loadingObj.transform.SetParent(transform);
            loadingSystem = loadingObj.AddComponent<GameplayLoadingSystem>();
            
            if (showDebugLogs)
            {
                Debug.Log("🎬 Sistema de carga creado");
            }
        }
    }
    
    /// <summary>
    /// Configura el sistema de video
    /// </summary>
    void SetupVideoSystem()
    {
        if (videoManager == null)
        {
            videoManager = FindFirstObjectByType<BackgroundVideoManager>();
        }
        
        if (videoManager == null)
        {
            GameObject videoObj = new GameObject("BackgroundVideoManager");
            videoObj.transform.SetParent(transform);
            videoManager = videoObj.AddComponent<BackgroundVideoManager>();
            
            // Configurar para sin opacidad
            videoManager.videoOpacity = 1.0f;
            
            if (showDebugLogs)
            {
                Debug.Log("🎥 Sistema de video creado");
            }
        }
        else
        {
            // Asegurar que esté sin opacidad
            videoManager.videoOpacity = 1.0f;
        }
    }
    
    /// <summary>
    /// Configura el detector de fin de canción
    /// </summary>
    void SetupSongEndDetector()
    {
        if (songEndDetector == null)
        {
            songEndDetector = FindFirstObjectByType<SongEndDetector>();
        }
        
        if (songEndDetector == null)
        {
            GameObject detectorObj = new GameObject("SongEndDetector");
            detectorObj.transform.SetParent(transform);
            songEndDetector = detectorObj.AddComponent<SongEndDetector>();
            
            if (showDebugLogs)
            {
                Debug.Log("🎵 Detector de fin de canción creado");
            }
        }
    }
}
