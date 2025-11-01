using UnityEngine;

/// <summary>
/// Script de diagnóstico para verificar el sistema de pausa
/// </summary>
public class PauseDiagnostic : MonoBehaviour
{
    [Header("Diagnóstico")]
    public bool showDiagnostic = true;
    public KeyCode diagnosticKey = KeyCode.F1;
    
    void Update()
    {
        if (Input.GetKeyDown(diagnosticKey))
        {
            RunDiagnostic();
        }
    }
    
    void RunDiagnostic()
    {
        Debug.Log("🔍 DIAGNÓSTICO DE PAUSA");
        
        // Verificar SimplePauseSetup
        SimplePauseSetup simplePause = FindFirstObjectByType<SimplePauseSetup>();
        if (simplePause != null)
        {
            Debug.Log($"✅ SimplePauseSetup - Pausado: {simplePause.IsPaused}");
        }
        else
        {
            Debug.Log("❌ SimplePauseSetup NO encontrado");
        }
        
        // Verificar GameplayManager
        GameplayManager gameplayManager = GameplayManager.Instance;
        if (gameplayManager != null)
        {
            Debug.Log($"✅ GameplayManager - Pausado: {gameplayManager.isPaused}");
        }
        else
        {
            Debug.Log("❌ GameplayManager NO encontrado");
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
        
        // Verificar estado del sistema
        Debug.Log($"⛱️ Time.timeScale: {Time.timeScale}");
        Debug.Log($"🔊 AudioListener.pause: {AudioListener.pause}");
        
        // Verificar audio sources
        AudioSource[] audioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        // Audio sources verificados
        
        int playingCount = 0;
        foreach (var source in audioSources)
        {
            if (source.isPlaying) playingCount++;
        }
        Debug.Log($"🎵 Audio reproduciendo: {playingCount}/{audioSources.Length}");
        
        Debug.Log("🔍 FIN DIAGNÓSTICO");
    }
    
    void OnGUI()
    {
        if (!showDiagnostic) return;
        
        GUILayout.BeginArea(new Rect(10, Screen.height - 200, 400, 190));
        GUILayout.BeginVertical("box");
        
        GUILayout.Label("🔍 PAUSE DIAGNOSTIC", GUI.skin.box);
        
        // Estado actual
        SimplePauseSetup simplePause = FindFirstObjectByType<SimplePauseSetup>();
        if (simplePause != null)
        {
            GUI.color = simplePause.IsPaused ? Color.red : Color.green;
            GUILayout.Label($"Estado: {(simplePause.IsPaused ? "PAUSADO" : "JUGANDO")}");
            GUI.color = Color.white;
        }
        
        GUILayout.Label($"Time.timeScale: {Time.timeScale:F1}");
        GUILayout.Label($"AudioListener.pause: {AudioListener.pause}");
        
        // Controles
        GUILayout.Space(5);
        GUILayout.Label("CONTROLES:");
        GUILayout.Label("ESC / Options = Pausa");
        GUILayout.Label("F1 = Diagnóstico completo");
        
        // Botones de emergencia
        GUILayout.Space(5);
        if (GUILayout.Button("FORCE RESUME"))
        {
            ForceResumeEverything();
        }
        
        if (GUILayout.Button("RUN DIAGNOSTIC"))
        {
            RunDiagnostic();
        }
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
    
    void ForceResumeEverything()
    {
        Debug.Log("🚨 FORZANDO REANUDACIÓN");
        
        // Resetear tiempo
        Time.timeScale = 1f;
        AudioListener.pause = false;
        
        // Buscar y forzar resume en SimplePauseSetup
        SimplePauseSetup simplePause = FindFirstObjectByType<SimplePauseSetup>();
        if (simplePause != null)
        {
            simplePause.ForceResumeGame();
        }
        
        // Forzar resume en GameplayManager
        GameplayManager gameplayManager = GameplayManager.Instance;
        if (gameplayManager != null && gameplayManager.isPaused)
        {
            gameplayManager.ResumeGame();
        }
        
        // Reanudar todo el audio
        AudioSource[] sources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        foreach (var source in sources)
        {
            if (source != null)
            {
                source.UnPause();
            }
        }
        
        Debug.Log("✅ Reanudación forzada completada");
    }
}
