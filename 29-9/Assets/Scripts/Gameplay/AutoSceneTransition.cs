using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Transición automática simple a PostGameplay cuando termina la canción
/// Versión simplificada que funciona con cualquier setup
/// </summary>
public class AutoSceneTransition : MonoBehaviour
{
    [Header("Simple Transition")]
    public bool enableTransition = true;
    public float delayAfterSongEnd = 3f;
    
    private AudioSource audioSource;
    private bool transitionStarted = false;
    private float songEndTime = 0f;
    
    void Start()
    {
        // Buscar AudioSource en la escena
        audioSource = FindFirstObjectByType<AudioSource>();
        
        if (audioSource == null)
        {
            // AudioSource no encontrado
        }
        else
        {
            // AutoSceneTransition inicializado
        }
    }
    
    void Update()
    {
        if (!enableTransition || transitionStarted || audioSource == null) return;
        
        // Detectar si la canción terminó
        if (!audioSource.isPlaying && audioSource.time > 0)
        {
            if (songEndTime == 0f)
            {
                songEndTime = Time.time;
                // Canción terminada
            }
            
            // Esperar el delay y cambiar escena
            if (Time.time - songEndTime >= delayAfterSongEnd)
            {
                StartTransition();
            }
        }
    }
    
    /// <summary>
    /// Inicia la transición a PostGameplay
    /// </summary>
    void StartTransition()
    {
        transitionStarted = true;
        // Cambiando a PostGameplay
        
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
                // Cargando escena
                return;
            }
            catch
            {
                continue;
            }
        }
        
        // Escena PostGameplay no encontrada
    }
    
    /// <summary>
    /// Forzar transición inmediata
    /// </summary>
    [ContextMenu("Go to PostGameplay Now")]
    public void GoToPostGameplayNow()
    {
        // Transición manual
        StartTransition();
    }
}
