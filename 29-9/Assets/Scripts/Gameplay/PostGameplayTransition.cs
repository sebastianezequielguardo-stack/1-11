using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Maneja la transición automática a PostGameplay cuando termina la canción
/// Detecta el final del gameplay y cambia de escena automáticamente
/// </summary>
public class PostGameplayTransition : MonoBehaviour
{
    [Header("Scene Transition")]
    public string postGameplaySceneName = "PostGameplay";
    public bool enableAutoTransition = true;
    public float transitionDelay = 2f; // Delay antes de cambiar escena
    
    [Header("Detection Settings")]
    public bool detectBySongEnd = true;
    public bool detectByAllNotesHit = true;
    public bool detectByTimeElapsed = true;
    
    [Header("Debug")]
    public bool showDebugInfo = true;
    
    // Referencias a componentes del juego
    private AudioSource audioSource;
    private GameplayManager gameplayManager;
    private bool transitionTriggered = false;
    private float gameStartTime;
    
    void Start()
    {
        InitializeTransitionSystem();
    }
    
    void Update()
    {
        if (enableAutoTransition && !transitionTriggered)
        {
            CheckForGameplayEnd();
        }
    }
    
    /// <summary>
    /// Inicializa el sistema de transición
    /// </summary>
    void InitializeTransitionSystem()
    {
        Debug.Log("🎮 Inicializando sistema de transición a PostGameplay...");
        
        // Buscar AudioSource
        audioSource = FindFirstObjectByType<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogWarning("⚠️ No se encontró AudioSource");
        }
        
        // Buscar GameplayManager
        gameplayManager = FindFirstObjectByType<GameplayManager>();
        if (gameplayManager == null)
        {
            Debug.LogWarning("⚠️ No se encontró GameplayManager");
        }
        
        gameStartTime = Time.time;
        
        if (showDebugInfo)
        {
            Debug.Log($"✅ Sistema de transición inicializado");
            Debug.Log($"   Escena objetivo: {postGameplaySceneName}");
            Debug.Log($"   Delay de transición: {transitionDelay}s");
        }
    }
    
    /// <summary>
    /// Verifica si el gameplay ha terminado
    /// </summary>
    void CheckForGameplayEnd()
    {
        bool gameEnded = false;
        string endReason = "";
        
        // Método 1: Verificar si la canción terminó
        if (detectBySongEnd && audioSource != null)
        {
            if (!audioSource.isPlaying && audioSource.time >= audioSource.clip.length - 0.1f)
            {
                gameEnded = true;
                endReason = "Canción terminada";
            }
        }
        
        // Método 2: Verificar si todas las notas fueron procesadas
        if (detectByAllNotesHit && gameplayManager != null)
        {
            if (AreAllNotesProcessed())
            {
                gameEnded = true;
                endReason = "Todas las notas procesadas";
            }
        }
        
        // Método 3: Verificar por tiempo transcurrido (backup)
        if (detectByTimeElapsed && audioSource != null && audioSource.clip != null)
        {
            float songDuration = audioSource.clip.length;
            float elapsedTime = Time.time - gameStartTime;
            
            if (elapsedTime >= songDuration + 1f) // +1 segundo de margen
            {
                gameEnded = true;
                endReason = "Tiempo de canción excedido";
            }
        }
        
        if (gameEnded)
        {
            TriggerPostGameplayTransition(endReason);
        }
    }
    
    /// <summary>
    /// Verifica si todas las notas han sido procesadas
    /// </summary>
    bool AreAllNotesProcessed()
    {
        if (gameplayManager == null) return false;
        
        try
        {
            // Verificar si hay notas restantes en la escena
            Note[] remainingNotes = FindObjectsByType<Note>(FindObjectsSortMode.None);
            
            if (remainingNotes.Length == 0)
            {
                return true; // No hay notas restantes
            }
            
            // Verificar si todas las notas están muy lejos (ya pasaron)
            bool allNotesPassed = true;
            foreach (Note note in remainingNotes)
            {
                if (note.transform.position.z > -50f) // Si alguna nota no ha pasado completamente
                {
                    allNotesPassed = false;
                    break;
                }
            }
            
            return allNotesPassed;
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"⚠️ Error verificando notas: {ex.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// Activa la transición a PostGameplay
    /// </summary>
    void TriggerPostGameplayTransition(string reason)
    {
        if (transitionTriggered) return;
        
        transitionTriggered = true;
        
        if (showDebugInfo)
        {
            Debug.Log($"🎯 Gameplay terminado: {reason}");
            Debug.Log($"🔄 Iniciando transición a {postGameplaySceneName} en {transitionDelay}s...");
        }
        
        StartCoroutine(TransitionToPostGameplay());
    }
    
    /// <summary>
    /// Corrutina para la transición con delay
    /// </summary>
    IEnumerator TransitionToPostGameplay()
    {
        // Esperar el delay configurado
        yield return new WaitForSeconds(transitionDelay);
        
        // Verificar si la escena existe
        if (DoesSceneExist(postGameplaySceneName))
        {
            Debug.Log($"🚀 Cambiando a escena: {postGameplaySceneName}");
            SceneManager.LoadScene(postGameplaySceneName);
        }
        else
        {
            Debug.LogError($"❌ Escena '{postGameplaySceneName}' no encontrada");
            Debug.LogError("💡 Verifica que la escena esté agregada en Build Settings");
            
            // Intentar nombres alternativos
            TryAlternativeSceneNames();
        }
    }
    
    /// <summary>
    /// Verifica si una escena existe en Build Settings
    /// </summary>
    bool DoesSceneExist(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneNameFromPath = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            
            if (sceneNameFromPath.Equals(sceneName, System.StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }
    
    /// <summary>
    /// Intenta nombres alternativos para la escena PostGameplay
    /// </summary>
    void TryAlternativeSceneNames()
    {
        string[] alternativeNames = {
            "PostGameplay",
            "Post Gameplay", 
            "PostGame",
            "Post Game",
            "Results",
            "GameResults",
            "Score",
            "EndGame",
            "Finish"
        };
        
        foreach (string altName in alternativeNames)
        {
            if (DoesSceneExist(altName))
            {
                Debug.Log($"🔄 Usando escena alternativa: {altName}");
                SceneManager.LoadScene(altName);
                return;
            }
        }
        
        Debug.LogError("❌ No se encontró ninguna escena de PostGameplay");
        Debug.LogError("💡 Agrega la escena PostGameplay a Build Settings");
    }
    
    /// <summary>
    /// Forzar transición manual
    /// </summary>
    [ContextMenu("Force Transition to PostGameplay")]
    public void ForceTransitionToPostGameplay()
    {
        Debug.Log("🔧 Forzando transición a PostGameplay...");
        TriggerPostGameplayTransition("Transición manual");
    }
    
    /// <summary>
    /// Mostrar información del sistema
    /// </summary>
    [ContextMenu("Show Transition Info")]
    public void ShowTransitionInfo()
    {
        Debug.Log("📊 INFORMACIÓN DEL SISTEMA DE TRANSICIÓN:");
        Debug.Log("═══════════════════════════════════════");
        
        Debug.Log($"Escena objetivo: {postGameplaySceneName}");
        Debug.Log($"Escena existe: {(DoesSceneExist(postGameplaySceneName) ? "✅ SÍ" : "❌ NO")}");
        Debug.Log($"Transición activada: {(enableAutoTransition ? "✅ SÍ" : "❌ NO")}");
        Debug.Log($"Transición ya ejecutada: {(transitionTriggered ? "✅ SÍ" : "❌ NO")}");
        
        if (audioSource != null)
        {
            Debug.Log($"AudioSource encontrado: ✅ SÍ");
            Debug.Log($"   Reproduciendo: {(audioSource.isPlaying ? "✅ SÍ" : "❌ NO")}");
            if (audioSource.clip != null)
            {
                Debug.Log($"   Duración: {audioSource.clip.length:F1}s");
                Debug.Log($"   Tiempo actual: {audioSource.time:F1}s");
            }
        }
        else
        {
            Debug.Log($"AudioSource encontrado: ❌ NO");
        }
        
        if (gameplayManager != null)
        {
            Debug.Log($"GameplayManager encontrado: ✅ SÍ");
        }
        else
        {
            Debug.Log($"GameplayManager encontrado: ❌ NO");
        }
        
        Note[] notes = FindObjectsByType<Note>(FindObjectsSortMode.None);
        Debug.Log($"Notas restantes: {notes.Length}");
        
        Debug.Log("\n💡 MÉTODOS DE DETECCIÓN:");
        Debug.Log($"- Por fin de canción: {(detectBySongEnd ? "✅ ACTIVO" : "❌ INACTIVO")}");
        Debug.Log($"- Por notas procesadas: {(detectByAllNotesHit ? "✅ ACTIVO" : "❌ INACTIVO")}");
        Debug.Log($"- Por tiempo transcurrido: {(detectByTimeElapsed ? "✅ ACTIVO" : "❌ INACTIVO")}");
    }
    
    /// <summary>
    /// Configurar escena de destino
    /// </summary>
    public void SetPostGameplayScene(string sceneName)
    {
        postGameplaySceneName = sceneName;
        Debug.Log($"🎯 Escena de destino configurada: {sceneName}");
    }
}
