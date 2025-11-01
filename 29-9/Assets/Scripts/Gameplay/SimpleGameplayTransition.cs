using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Transición simple y confiable al PostGameplay
/// Versión simplificada que funciona siempre
/// </summary>
public class SimpleGameplayTransition : MonoBehaviour
{
    [Header("Simple Transition")]
    public bool enableTransition = true;
    public float delayAfterSongEnd = 2f;
    public string postGameplayScene = "PostGameplay";
    
    [Header("Detection Method")]
    public bool useAudioSource = true;
    public bool useTimer = true;
    public bool useManualTrigger = false;
    
    [Header("Debug")]
    public bool showDebugLogs = true;
    
    private AudioSource audioSource;
    private bool transitionTriggered = false;
    private float gameStartTime;
    private float songDuration = 0f;
    
    void Start()
    {
        InitializeTransition();
    }
    
    void Update()
    {
        if (!enableTransition || transitionTriggered) return;
        
        CheckForTransition();
    }
    
    /// <summary>
    /// Inicializa el sistema de transición
    /// </summary>
    void InitializeTransition()
    {
        gameStartTime = Time.time;
        
        // Buscar AudioSource
        if (useAudioSource)
        {
            audioSource = FindFirstObjectByType<AudioSource>();
            if (audioSource != null && audioSource.clip != null)
            {
                songDuration = audioSource.clip.length;
                if (showDebugLogs)
                {
                    Debug.Log($"🎵 AudioSource encontrado - Duración: {songDuration:F1}s");
                }
            }
        }
        
        // Desactivar otros sistemas de transición
        DisableOtherTransitionSystems();
        
        if (showDebugLogs)
        {
            Debug.Log("✅ SimpleGameplayTransition inicializado");
        }
    }
    
    /// <summary>
    /// Desactiva otros sistemas de transición para evitar conflictos
    /// </summary>
    void DisableOtherTransitionSystems()
    {
        // Desactivar AutoSceneTransition
        AutoSceneTransition autoTransition = FindFirstObjectByType<AutoSceneTransition>();
        if (autoTransition != null)
        {
            autoTransition.enableTransition = false;
            if (showDebugLogs) Debug.Log("🔧 AutoSceneTransition desactivado");
        }
        
        // Desactivar PauseAwareTransition
        PauseAwareTransition pauseTransition = FindFirstObjectByType<PauseAwareTransition>();
        if (pauseTransition != null)
        {
            pauseTransition.enableTransition = false;
            if (showDebugLogs) Debug.Log("🔧 PauseAwareTransition desactivado");
        }
    }
    
    /// <summary>
    /// Verifica si debe hacer la transición
    /// </summary>
    void CheckForTransition()
    {
        bool shouldTransition = false;
        string reason = "";
        
        // Método 1: Por AudioSource
        if (useAudioSource && audioSource != null)
        {
            // Si el audio no está reproduciéndose Y ha pasado tiempo suficiente
            if (!audioSource.isPlaying && audioSource.time > 1f)
            {
                shouldTransition = true;
                reason = "Audio terminado";
            }
        }
        
        // Método 2: Por timer (backup)
        if (useTimer && songDuration > 0f)
        {
            float elapsedTime = Time.time - gameStartTime;
            if (elapsedTime >= songDuration + 1f) // +1 segundo de margen
            {
                shouldTransition = true;
                reason = "Timer completado";
            }
        }
        
        // Método 3: Trigger manual
        if (useManualTrigger && Input.GetKeyDown(KeyCode.Return))
        {
            shouldTransition = true;
            reason = "Trigger manual (Enter)";
        }
        
        if (shouldTransition)
        {
            TriggerTransition(reason);
        }
    }
    
    /// <summary>
    /// Activa la transición
    /// </summary>
    void TriggerTransition(string reason)
    {
        if (transitionTriggered) return;
        
        transitionTriggered = true;
        
        if (showDebugLogs)
        {
            Debug.Log($"🚀 Transición activada: {reason}");
            Debug.Log($"⏱️ Esperando {delayAfterSongEnd}s antes de cambiar a {postGameplayScene}");
        }
        
        StartCoroutine(TransitionAfterDelay());
    }
    
    /// <summary>
    /// Transición con delay
    /// </summary>
    IEnumerator TransitionAfterDelay()
    {
        yield return new WaitForSeconds(delayAfterSongEnd);
        
        if (showDebugLogs)
        {
            Debug.Log($"🎯 Cambiando a escena: {postGameplayScene}");
        }
        
        try
        {
            SceneManager.LoadScene(postGameplayScene);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"❌ Error cargando escena '{postGameplayScene}': {ex.Message}");
            TryAlternativeScenes();
        }
    }
    
    /// <summary>
    /// Intenta escenas alternativas
    /// </summary>
    void TryAlternativeScenes()
    {
        string[] alternatives = {
            "Post Gameplay",
            "PostGame",
            "Post Game", 
            "Results",
            "GameResults",
            "Score",
            "EndGame"
        };
        
        foreach (string sceneName in alternatives)
        {
            try
            {
                SceneManager.LoadScene(sceneName);
                Debug.Log($"✅ Cargando escena alternativa: {sceneName}");
                return;
            }
            catch
            {
                continue;
            }
        }
        
        Debug.LogError("❌ No se pudo cargar ninguna escena PostGameplay");
        Debug.LogError("💡 Verifica que la escena esté en Build Settings");
    }
    
    /// <summary>
    /// Forzar transición inmediata
    /// </summary>
    [ContextMenu("Force Transition Now")]
    public void ForceTransitionNow()
    {
        Debug.Log("🔧 Forzando transición inmediata...");
        TriggerTransition("Forzado manual");
    }
    
    /// <summary>
    /// Mostrar estado del sistema
    /// </summary>
    [ContextMenu("Show Transition Status")]
    public void ShowTransitionStatus()
    {
        Debug.Log("📊 ESTADO DE TRANSICIÓN SIMPLE:");
        Debug.Log("═══════════════════════════════");
        
        Debug.Log($"Transición habilitada: {(enableTransition ? "✅ SÍ" : "❌ NO")}");
        Debug.Log($"Transición activada: {(transitionTriggered ? "✅ SÍ" : "❌ NO")}");
        Debug.Log($"Escena objetivo: {postGameplayScene}");
        
        float elapsedTime = Time.time - gameStartTime;
        Debug.Log($"Tiempo transcurrido: {elapsedTime:F1}s");
        
        if (audioSource != null)
        {
            Debug.Log($"AudioSource: ✅ Encontrado");
            Debug.Log($"   Reproduciendo: {(audioSource.isPlaying ? "✅ SÍ" : "❌ NO")}");
            Debug.Log($"   Tiempo actual: {audioSource.time:F1}s");
            if (audioSource.clip != null)
            {
                Debug.Log($"   Duración total: {audioSource.clip.length:F1}s");
                float progress = audioSource.time / audioSource.clip.length;
                Debug.Log($"   Progreso: {progress:P1}");
            }
        }
        else
        {
            Debug.Log("AudioSource: ❌ No encontrado");
        }
        
        Debug.Log($"\n⚙️ MÉTODOS ACTIVOS:");
        Debug.Log($"- AudioSource: {(useAudioSource ? "✅" : "❌")}");
        Debug.Log($"- Timer: {(useTimer ? "✅" : "❌")}");
        Debug.Log($"- Manual (Enter): {(useManualTrigger ? "✅" : "❌")}");
        
        if (songDuration > 0f)
        {
            Debug.Log($"\n⏱️ TIMER INFO:");
            Debug.Log($"Duración canción: {songDuration:F1}s");
            Debug.Log($"Tiempo para transición: {songDuration + 1f:F1}s");
            Debug.Log($"Tiempo restante: {Mathf.Max(0f, (songDuration + 1f) - elapsedTime):F1}s");
        }
    }
    
    /// <summary>
    /// Configurar escena de destino
    /// </summary>
    public void SetPostGameplayScene(string sceneName)
    {
        postGameplayScene = sceneName;
        Debug.Log($"🎯 Escena de destino configurada: {sceneName}");
    }
    
    /// <summary>
    /// Test rápido de transición
    /// </summary>
    [ContextMenu("Test Transition (5 seconds)")]
    public void TestTransitionQuick()
    {
        Debug.Log("🧪 Test de transición en 5 segundos...");
        delayAfterSongEnd = 0.1f; // Casi inmediato
        TriggerTransition("Test rápido");
    }
}
