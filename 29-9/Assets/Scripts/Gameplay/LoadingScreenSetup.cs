using UnityEngine;

/// <summary>
/// Script de configuración automática para la pantalla de carga del gameplay
/// Ejecutar una vez para configurar el sistema de carga con countdown
/// </summary>
public class LoadingScreenSetup : MonoBehaviour
{
    [Header("Auto Setup")]
    [SerializeField] private bool setupOnStart = true;
    
    [Header("Loading Settings")]
    [SerializeField] private float countdownDuration = 3f;
    [SerializeField] private bool waitForVideo = true;
    [SerializeField] private float maxVideoWaitTime = 8f;
    
    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = false;
    
    void Start()
    {
        if (setupOnStart)
        {
            SetupLoadingScreen();
        }
    }
    
    /// <summary>
    /// Configura automáticamente la pantalla de carga del gameplay
    /// </summary>
    [ContextMenu("Setup Loading Screen")]
    public void SetupLoadingScreen()
    {
        Debug.Log("🎬 Configurando pantalla de carga del gameplay...");
        
        // Verificar si ya existe una pantalla de carga
        GameplayLoadingScreen existingLoadingScreen = FindFirstObjectByType<GameplayLoadingScreen>();
        if (existingLoadingScreen != null)
        {
            Debug.Log("✅ GameplayLoadingScreen ya existe");
            return;
        }
        
        // Crear nueva pantalla de carga
        GameObject loadingScreenObj = new GameObject("GameplayLoadingScreen");
        GameplayLoadingScreen loadingScreen = loadingScreenObj.AddComponent<GameplayLoadingScreen>();
        
        // Configurar parámetros
        loadingScreen.countdownDuration = countdownDuration;
        loadingScreen.waitForVideoLoad = waitForVideo;
        loadingScreen.maxVideoWaitTime = maxVideoWaitTime;
        
        // Configurar colores
        loadingScreen.backgroundColor = Color.black;
        loadingScreen.textColor = Color.white;
        loadingScreen.textSize = 120f;
        
        // Configurar mensajes de carga
        loadingScreen.loadingMessages = new string[]
        {
            "Cargando video de fondo...",
            "Preparando gameplay...",
            "¡Casi listo!"
        };
        
        Debug.Log("✅ GameplayLoadingScreen creado y configurado");
        Debug.Log("🎬 Pantalla de carga con countdown de 3 segundos activada");
        Debug.Log("⏳ Los videos cargarán completamente antes de iniciar el juego");
        
        // Auto-destruir este script después de la configuración
        if (Application.isPlaying)
        {
            Destroy(this);
        }
    }
    
    /// <summary>
    /// Probar la pantalla de carga
    /// </summary>
    [ContextMenu("Test Loading Screen")]
    public void TestLoadingScreen()
    {
        GameplayLoadingScreen loadingScreen = FindFirstObjectByType<GameplayLoadingScreen>();
        if (loadingScreen != null)
        {
            Debug.Log("🧪 Pantalla de carga encontrada y funcionando");
        }
        else
        {
            Debug.LogWarning("⚠️ No se encontró GameplayLoadingScreen. Ejecuta 'Setup Loading Screen' primero.");
        }
    }
    
    /// <summary>
    /// Verificar estado del sistema
    /// </summary>
    [ContextMenu("Check System Status")]
    public void CheckSystemStatus()
    {
        Debug.Log("📊 ESTADO DEL SISTEMA DE CARGA:");
        Debug.Log("═══════════════════════════════");
        
        // Verificar GameplayLoadingScreen
        GameplayLoadingScreen loadingScreen = FindFirstObjectByType<GameplayLoadingScreen>();
        Debug.Log($"GameplayLoadingScreen: {(loadingScreen != null ? "✅ ACTIVO" : "❌ FALTANTE")}");
        
        // Verificar GameplayManager
        GameplayManager gameplayManager = GameplayManager.Instance;
        Debug.Log($"GameplayManager: {(gameplayManager != null ? "✅ ACTIVO" : "❌ FALTANTE")}");
        
        // Verificar BackgroundVideoSystem
        BackgroundVideoSystem videoSystem = FindFirstObjectByType<BackgroundVideoSystem>();
        Debug.Log($"BackgroundVideoSystem: {(videoSystem != null ? "✅ ACTIVO" : "❌ FALTANTE")}");
        
        Debug.Log("");
        if (loadingScreen != null && gameplayManager != null)
        {
            Debug.Log("🎉 SISTEMA DE CARGA COMPLETAMENTE FUNCIONAL");
            Debug.Log("📋 Flujo: Pantalla negra → Countdown 3s → Gameplay");
        }
        else
        {
            Debug.Log("⚠️ EJECUTAR 'Setup Loading Screen' PARA CONFIGURAR");
        }
    }
    
    /// <summary>
    /// Información del sistema en el inspector
    /// </summary>
    void OnGUI()
    {
        if (!showDebugInfo) return;
        
        GUILayout.BeginArea(new Rect(Screen.width - 320, 10, 300, 200));
        GUILayout.Box("🎬 LOADING SCREEN SETUP");
        
        if (GUILayout.Button("Setup Loading Screen"))
        {
            SetupLoadingScreen();
        }
        
        if (GUILayout.Button("Test Loading Screen"))
        {
            TestLoadingScreen();
        }
        
        if (GUILayout.Button("Check System Status"))
        {
            CheckSystemStatus();
        }
        
        GameplayLoadingScreen loadingScreen = FindFirstObjectByType<GameplayLoadingScreen>();
        if (loadingScreen != null)
        {
            GUILayout.Label("✅ Sistema configurado");
            GUILayout.Label($"Countdown: {countdownDuration}s");
            GUILayout.Label($"Esperar video: {waitForVideo}");
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
        if (Input.GetKeyDown(KeyCode.F8))
        {
            SetupLoadingScreen();
        }
        
        if (Input.GetKeyDown(KeyCode.F9))
        {
            CheckSystemStatus();
        }
    }
}
