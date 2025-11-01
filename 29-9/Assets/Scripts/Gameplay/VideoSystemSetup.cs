using UnityEngine;

/// <summary>
/// Script de configuración automática para el sistema de videos de fondo
/// Ejecutar una vez para configurar todo automáticamente
/// </summary>
public class VideoSystemSetup : MonoBehaviour
{
    [Header("Auto Setup")]
    [SerializeField] private bool setupOnStart = true;
    
    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = false;
    
    void Start()
    {
        if (setupOnStart)
        {
            SetupVideoSystem();
        }
    }
    
    /// <summary>
    /// Configura automáticamente el sistema de videos de fondo
    /// </summary>
    [ContextMenu("Setup Video System")]
    public void SetupVideoSystem()
    {
        Debug.Log("🎬 Iniciando configuración automática del sistema de videos...");
        
        // Buscar GameplayManager en la escena
        GameplayManager gameplayManager = FindFirstObjectByType<GameplayManager>();
        if (gameplayManager == null)
        {
            Debug.LogError("❌ No se encontró GameplayManager en la escena");
            return;
        }
        
        // Verificar si ya existe un BackgroundVideoSystem
        BackgroundVideoSystem existingVideoSystem = FindFirstObjectByType<BackgroundVideoSystem>();
        if (existingVideoSystem != null)
        {
            Debug.Log("✅ BackgroundVideoSystem ya existe, conectando con GameplayManager...");
            gameplayManager.backgroundVideoSystem = existingVideoSystem;
        }
        else
        {
            // Crear nuevo BackgroundVideoSystem
            GameObject videoSystemObj = new GameObject("BackgroundVideoSystem");
            BackgroundVideoSystem videoSystem = videoSystemObj.AddComponent<BackgroundVideoSystem>();
            
            // Configurar con opacidad 100% y rotación por defecto
            videoSystem.enableBackgroundVideo = true;
            videoSystem.showDebugInfo = showDebugInfo;
            videoSystem.videoRotation = Vector3.zero; // Sin rotación por defecto
            
            // Conectar con GameplayManager
            gameplayManager.backgroundVideoSystem = videoSystem;
            
            Debug.Log("✅ BackgroundVideoSystem creado y configurado");
        }
        
        // Verificar estructura de carpetas
        CheckSongFolderStructure();
        
        Debug.Log("🎉 ¡Sistema de videos configurado exitosamente!");
        Debug.Log("📁 Coloca tus videos en: StreamingAssets/Songs/[NombreCancion]/video.mp4");
        Debug.Log("🎬 Opacidad configurada al 100% (completamente opaco)");
        
        // Auto-destruir este script después de la configuración
        if (Application.isPlaying)
        {
            Destroy(this);
        }
    }
    
    /// <summary>
    /// Verifica la estructura de carpetas de canciones
    /// </summary>
    void CheckSongFolderStructure()
    {
        string songsPath = System.IO.Path.Combine(Application.streamingAssetsPath, "Songs");
        
        if (!System.IO.Directory.Exists(songsPath))
        {
            Debug.LogWarning("⚠️ Carpeta Songs no encontrada en StreamingAssets");
            return;
        }
        
        string[] songFolders = System.IO.Directory.GetDirectories(songsPath);
        int videosFound = 0;
        
        foreach (string folder in songFolders)
        {
            string folderName = System.IO.Path.GetFileName(folder);
            
            // Buscar videos en la carpeta
            string[] videoExtensions = { ".mp4", ".webm", ".mov", ".avi" };
            bool hasVideo = false;
            
            foreach (string ext in videoExtensions)
            {
                string[] videoFiles = System.IO.Directory.GetFiles(folder, "*" + ext);
                if (videoFiles.Length > 0)
                {
                    hasVideo = true;
                    videosFound++;
                    Debug.Log($"🎬 Video encontrado en: {folderName} - {System.IO.Path.GetFileName(videoFiles[0])}");
                    break;
                }
            }
            
            if (!hasVideo)
            {
                Debug.Log($"📁 Sin video: {folderName} (puedes agregar video.mp4)");
            }
        }
        
        Debug.Log($"📊 Resumen: {videosFound} videos encontrados en {songFolders.Length} canciones");
    }
    
    /// <summary>
    /// Método para probar el sistema con la canción actual
    /// </summary>
    [ContextMenu("Test Video System")]
    public void TestVideoSystem()
    {
        BackgroundVideoSystem videoSystem = FindFirstObjectByType<BackgroundVideoSystem>();
        if (videoSystem == null)
        {
            Debug.LogError("❌ BackgroundVideoSystem no encontrado. Ejecuta 'Setup Video System' primero.");
            return;
        }
        
        if (GameManager.Instance?.selectedSongPath != null)
        {
            string songFolder = System.IO.Path.Combine(Application.streamingAssetsPath, "Songs", GameManager.Instance.selectedSongPath);
            videoSystem.LoadSongVideo(songFolder);
            Debug.Log($"🧪 Probando carga de video para: {GameManager.Instance.selectedSongPath}");
        }
        else
        {
            Debug.LogWarning("⚠️ No hay canción seleccionada para probar");
        }
    }
    
    /// <summary>
    /// Información del sistema en el inspector
    /// </summary>
    void OnGUI()
    {
        if (!showDebugInfo) return;
        
        GUILayout.BeginArea(new Rect(Screen.width - 320, 10, 300, 200));
        GUILayout.Box("🎬 VIDEO SYSTEM SETUP");
        
        if (GUILayout.Button("Setup Video System"))
        {
            SetupVideoSystem();
        }
        
        if (GUILayout.Button("Test Video System"))
        {
            TestVideoSystem();
        }
        
        if (GUILayout.Button("Check Song Folders"))
        {
            CheckSongFolderStructure();
        }
        
        BackgroundVideoSystem videoSystem = FindFirstObjectByType<BackgroundVideoSystem>();
        if (videoSystem != null)
        {
            GUILayout.Label("✅ Sistema configurado");
            GUILayout.Label($"Videos habilitados: {videoSystem.enableBackgroundVideo}");
            GUILayout.Label($"Video cargado: {videoSystem.IsVideoLoaded()}");
            GUILayout.Label($"Video reproduciendo: {videoSystem.IsVideoPlaying()}");
        }
        else
        {
            GUILayout.Label("❌ Sistema no configurado");
        }
        
        GUILayout.EndArea();
    }
    
    void Update()
    {
        // Teclas de acceso rápido
        if (Input.GetKeyDown(KeyCode.F10))
        {
            SetupVideoSystem();
        }
        
        if (Input.GetKeyDown(KeyCode.F11))
        {
            TestVideoSystem();
        }
    }
}
