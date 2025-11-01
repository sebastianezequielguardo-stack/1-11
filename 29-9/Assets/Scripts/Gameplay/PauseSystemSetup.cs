using UnityEngine;

/// <summary>
/// Script simple para configurar automáticamente el sistema de pausa
/// Agrega este script a cualquier GameObject en la escena de gameplay
/// </summary>
public class PauseSystemSetup : MonoBehaviour
{
    [Header("Configuración")]
    public bool setupAutomatically = true;
    public string mainMenuSceneName = "MainMenu";
    
    void Start()
    {
        if (setupAutomatically)
        {
            SetupPauseSystem();
        }
    }
    
    void SetupPauseSystem()
    {
        // Verificar si ya existe PauseMenuManager
        PauseMenuManager existingPauseManager = FindFirstObjectByType<PauseMenuManager>();
        if (existingPauseManager == null)
        {
            // Crear GameObject para PauseMenuManager
            GameObject pauseManagerObj = new GameObject("PauseMenuManager");
            PauseMenuManager pauseManager = pauseManagerObj.AddComponent<PauseMenuManager>();
            pauseManager.createUIAutomatically = true;
            pauseManager.mainMenuSceneName = mainMenuSceneName;
            
            // PauseMenuManager creado
        }
        else
        {
            // PauseMenuManager ya existe
        }
        
        // Verificar InputManager
        InputManager inputManager = FindFirstObjectByType<InputManager>();
        if (inputManager != null)
        {
            // InputManager encontrado
        }
        else
        {
            // InputManager no encontrado
        }
        
        // Verificar GameplayManager
        GameplayManager gameplayManager = GameplayManager.Instance;
        if (gameplayManager != null)
        {
            // GameplayManager encontrado
        }
        else
        {
            // GameplayManager no encontrado
        }
        
        // Sistema de pausa configurado
    }
    
    void OnGUI()
    {
        // Panel de información
        GUILayout.BeginArea(new Rect(Screen.width - 300, 10, 290, 150));
        GUILayout.BeginVertical("box");
        
        GUILayout.Label("🎮 PAUSE SYSTEM", GUI.skin.box);
        
        GameplayManager gm = GameplayManager.Instance;
        if (gm != null)
        {
            GUI.color = gm.isPaused ? Color.red : Color.green;
            GUILayout.Label($"Estado: {(gm.isPaused ? "PAUSADO" : "JUGANDO")}");
            GUI.color = Color.white;
        }
        
        GUILayout.Space(5);
        GUILayout.Label("CONTROLES:");
        GUILayout.Label("ESC / Options = Pausa");
        GUILayout.Label("R / Share = Reiniciar");
        
        PauseMenuManager pmm = FindFirstObjectByType<PauseMenuManager>();
        if (pmm != null)
        {
            GUILayout.Label($"Menú visible: {(pmm.IsMenuVisible() ? "SÍ" : "NO")}");
        }
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
}
