using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Transición inteligente que NO se activa durante pausas
/// Reemplaza AutoSceneTransition con detección de pausa
/// </summary>
public class PauseAwareTransition : MonoBehaviour
{
    [Header("Smart Transition")]
    public bool enableTransition = true;
    public float delayAfterSongEnd = 3f;
    
    [Header("Pause Detection")]
    public bool detectPauseState = true;
    public KeyCode pauseKey = KeyCode.Escape;
    
    private AudioSource audioSource;
    private bool transitionStarted = false;
    private float songEndTime = 0f;
    private bool gameIsPaused = false;
    private bool songWasPlayingBeforePause = false;
    
    void Start()
    {
        // Buscar AudioSource en la escena
        audioSource = FindFirstObjectByType<AudioSource>();
        
        if (audioSource == null)
        {
            Debug.LogWarning("⚠️ No se encontró AudioSource para detectar fin de canción");
        }
        else
        {
            Debug.Log("✅ PauseAwareTransition inicializado - detectará fin de canción (sin pausas)");
        }
        
        // Desactivar AutoSceneTransition si existe para evitar conflictos
        AutoSceneTransition oldTransition = FindFirstObjectByType<AutoSceneTransition>();
        if (oldTransition != null)
        {
            oldTransition.enableTransition = false;
            Debug.Log("🔧 AutoSceneTransition desactivado para evitar conflictos");
        }
    }
    
    void Update()
    {
        if (!enableTransition || transitionStarted || audioSource == null) return;
        
        // Detectar estado de pausa
        DetectPauseState();
        
        // Solo verificar fin de canción si NO está pausado
        if (!gameIsPaused)
        {
            CheckForSongEnd();
        }
        else
        {
            // Si está pausado, resetear el timer de fin de canción
            if (songEndTime > 0f)
            {
                songEndTime = 0f;
                Debug.Log("⏸️ Pausa detectada - reseteando timer de transición");
            }
        }
    }
    
    /// <summary>
    /// Detecta si el juego está pausado
    /// </summary>
    void DetectPauseState()
    {
        if (!detectPauseState) return;
        
        bool wasPaused = gameIsPaused;
        
        // Método 1: Detectar por tecla de pausa
        if (Input.GetKeyDown(pauseKey))
        {
            gameIsPaused = !gameIsPaused;
        }
        
        // Método 2: Detectar por Time.timeScale
        if (Time.timeScale == 0f || Time.timeScale < 0.1f)
        {
            gameIsPaused = true;
        }
        else if (Time.timeScale >= 0.9f)
        {
            gameIsPaused = false;
        }
        
        // Método 3: Detectar por estado del AudioSource
        if (audioSource != null)
        {
            // Si el audio se pausó pero la canción no terminó
            if (!audioSource.isPlaying && audioSource.time > 0f && audioSource.time < audioSource.clip.length - 1f)
            {
                // Verificar si es pausa o fin natural
                if (audioSource.time < audioSource.clip.length * 0.95f) // No está cerca del final
                {
                    gameIsPaused = true;
                    songWasPlayingBeforePause = true;
                }
            }
            
            // Si el audio se reanudó
            if (audioSource.isPlaying && songWasPlayingBeforePause)
            {
                gameIsPaused = false;
                songWasPlayingBeforePause = false;
            }
        }
        
        // Método 4: Buscar componentes de pausa en la escena
        GameObject pauseMenu = GameObject.Find("PauseMenu");
        if (pauseMenu == null) pauseMenu = GameObject.Find("Pause Menu");
        if (pauseMenu == null) pauseMenu = GameObject.Find("PausePanel");
        if (pauseMenu == null) pauseMenu = GameObject.Find("Pause Panel");
        
        if (pauseMenu != null && pauseMenu.activeInHierarchy)
        {
            gameIsPaused = true;
        }
        
        // Log cambios de estado
        if (wasPaused != gameIsPaused)
        {
            Debug.Log($"{(gameIsPaused ? "⏸️ PAUSA DETECTADA" : "▶️ JUEGO REANUDADO")}");
        }
    }
    
    /// <summary>
    /// Verifica si la canción terminó (solo si no está pausado)
    /// </summary>
    void CheckForSongEnd()
    {
        // Detectar si la canción terminó NATURALMENTE (no por pausa)
        if (!audioSource.isPlaying && audioSource.time > 0)
        {
            // Verificar que esté cerca del final de la canción
            float songProgress = audioSource.time / audioSource.clip.length;
            
            if (songProgress >= 0.95f) // Al menos 95% de la canción completada
            {
                if (songEndTime == 0f)
                {
                    songEndTime = Time.time;
                    Debug.Log("🎵 Canción terminada NATURALMENTE - iniciando countdown para PostGameplay");
                }
                
                // Esperar el delay y cambiar escena
                if (Time.time - songEndTime >= delayAfterSongEnd)
                {
                    StartTransition();
                }
            }
        }
        else if (audioSource.isPlaying)
        {
            // Si la canción se está reproduciendo, resetear timer
            songEndTime = 0f;
        }
    }
    
    /// <summary>
    /// Inicia la transición a PostGameplay
    /// </summary>
    void StartTransition()
    {
        transitionStarted = true;
        Debug.Log("🚀 Cambiando a PostGameplay (canción terminada naturalmente)...");
        
        // Intentar cargar PostGameplay
        try
        {
            SceneManager.LoadScene("PostGameplay");
        }
        catch
        {
            // Si no existe, intentar nombres alternativos
            TryAlternativeScenes();
        }
    }
    
    /// <summary>
    /// Intenta nombres alternativos de escena
    /// </summary>
    void TryAlternativeScenes()
    {
        string[] sceneNames = {
            "Post Gameplay",
            "PostGame", 
            "Post Game",
            "Results",
            "GameResults",
            "Score",
            "EndGame"
        };
        
        foreach (string sceneName in sceneNames)
        {
            try
            {
                SceneManager.LoadScene(sceneName);
                Debug.Log($"✅ Cargando escena: {sceneName}");
                return;
            }
            catch
            {
                continue;
            }
        }
        
        Debug.LogError("❌ No se encontró escena PostGameplay");
    }
    
    /// <summary>
    /// Forzar transición inmediata (solo si no está pausado)
    /// </summary>
    [ContextMenu("Go to PostGameplay Now")]
    public void GoToPostGameplayNow()
    {
        if (gameIsPaused)
        {
            Debug.LogWarning("⚠️ No se puede hacer transición mientras está pausado");
            return;
        }
        
        Debug.Log("🔧 Transición manual a PostGameplay");
        StartTransition();
    }
    
    /// <summary>
    /// Mostrar estado del sistema
    /// </summary>
    [ContextMenu("Show Transition Status")]
    public void ShowTransitionStatus()
    {
        Debug.Log("📊 ESTADO DE TRANSICIÓN INTELIGENTE:");
        Debug.Log("═══════════════════════════════════");
        
        Debug.Log($"Transición habilitada: {(enableTransition ? "✅ SÍ" : "❌ NO")}");
        Debug.Log($"Juego pausado: {(gameIsPaused ? "⏸️ SÍ" : "▶️ NO")}");
        Debug.Log($"Transición iniciada: {(transitionStarted ? "✅ SÍ" : "❌ NO")}");
        
        if (audioSource != null && audioSource.clip != null)
        {
            float progress = audioSource.time / audioSource.clip.length;
            Debug.Log($"AudioSource: ✅ Encontrado");
            Debug.Log($"   Reproduciendo: {(audioSource.isPlaying ? "✅ SÍ" : "❌ NO")}");
            Debug.Log($"   Progreso: {progress:P1} ({audioSource.time:F1}s / {audioSource.clip.length:F1}s)");
            Debug.Log($"   Cerca del final: {(progress >= 0.95f ? "✅ SÍ" : "❌ NO")}");
        }
        
        if (songEndTime > 0f)
        {
            float timeRemaining = delayAfterSongEnd - (Time.time - songEndTime);
            Debug.Log($"Timer de transición: {timeRemaining:F1}s restantes");
        }
        else
        {
            Debug.Log("Timer de transición: ⏸️ Inactivo");
        }
        
        Debug.Log("\n💡 DETECCIÓN DE PAUSA:");
        Debug.Log($"- Time.timeScale: {Time.timeScale}");
        Debug.Log($"- Tecla pausa ({pauseKey}): Monitoreada");
        Debug.Log($"- PauseMenu activo: {(GameObject.Find("PauseMenu")?.activeInHierarchy == true ? "✅ SÍ" : "❌ NO")}");
    }
}
