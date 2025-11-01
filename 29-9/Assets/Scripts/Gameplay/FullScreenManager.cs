using UnityEngine;

/// <summary>
/// Maneja el modo fullscreen del juego
/// Script de reemplazo para el GameObject FullScreen
/// </summary>
public class FullScreenManager : MonoBehaviour
{
    [Header("Fullscreen Settings")]
    public bool startInFullscreen = true;
    public bool allowToggle = true;
    public KeyCode toggleKey = KeyCode.F11;
    
    [Header("Debug")]
    public bool showDebugInfo = false;
    
    void Start()
    {
        // Aplicar configuración inicial
        if (startInFullscreen)
        {
            SetFullscreen(true);
        }
        
        if (showDebugInfo)
        {
            Debug.Log($"🖥️ FullScreenManager iniciado - Fullscreen: {Screen.fullScreen}");
        }
    }
    
    void Update()
    {
        // Permitir toggle con tecla
        if (allowToggle && Input.GetKeyDown(toggleKey))
        {
            ToggleFullscreen();
        }
    }
    
    /// <summary>
    /// Alterna entre fullscreen y windowed
    /// </summary>
    public void ToggleFullscreen()
    {
        SetFullscreen(!Screen.fullScreen);
    }
    
    /// <summary>
    /// Establece el modo fullscreen
    /// </summary>
    public void SetFullscreen(bool fullscreen)
    {
        Screen.fullScreen = fullscreen;
        
        if (showDebugInfo)
        {
            Debug.Log($"🖥️ Fullscreen cambiado a: {fullscreen}");
        }
    }
    
    /// <summary>
    /// Obtiene el estado actual de fullscreen
    /// </summary>
    public bool IsFullscreen()
    {
        return Screen.fullScreen;
    }
    
    /// <summary>
    /// Configurar desde UI (para uso con Toggle)
    /// </summary>
    public void OnFullscreenToggleChanged(bool value)
    {
        SetFullscreen(value);
    }
}
