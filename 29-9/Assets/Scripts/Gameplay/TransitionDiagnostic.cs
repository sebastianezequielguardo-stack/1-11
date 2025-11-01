using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Diagnóstico completo del sistema de transición
/// Identifica por qué no está funcionando la transición a PostGameplay
/// </summary>
public class TransitionDiagnostic : MonoBehaviour
{
    [Header("Diagnostic Settings")]
    public bool runOnStart = true;
    public bool continuousMonitoring = true;
    public float monitoringInterval = 2f;
    
    private float lastMonitorTime = 0f;
    
    void Start()
    {
        if (runOnStart)
        {
            RunCompleteDiagnostic();
        }
    }
    
    void Update()
    {
        if (continuousMonitoring && Time.time - lastMonitorTime >= monitoringInterval)
        {
            lastMonitorTime = Time.time;
            MonitorTransitionSystems();
        }
    }
    
    /// <summary>
    /// Ejecuta diagnóstico completo
    /// </summary>
    [ContextMenu("Run Complete Diagnostic")]
    public void RunCompleteDiagnostic()
    {
        Debug.Log("🔍 DIAGNÓSTICO COMPLETO DE TRANSICIÓN:");
        Debug.Log("═══════════════════════════════════════");
        
        CheckCurrentScene();
        CheckBuildSettings();
        CheckTransitionSystems();
        CheckAudioSystem();
        CheckGameplayState();
        ProvideSolution();
    }
    
    /// <summary>
    /// Verifica la escena actual
    /// </summary>
    void CheckCurrentScene()
    {
        Debug.Log("\n📍 ESCENA ACTUAL:");
        string currentScene = SceneManager.GetActiveScene().name;
        Debug.Log($"Escena: {currentScene}");
        
        if (currentScene.ToLower().Contains("gameplay"))
        {
            Debug.Log("✅ Estás en una escena de Gameplay");
        }
        else
        {
            Debug.LogWarning("⚠️ No pareces estar en una escena de Gameplay");
        }
    }
    
    /// <summary>
    /// Verifica Build Settings
    /// </summary>
    void CheckBuildSettings()
    {
        Debug.Log("\n🏗️ BUILD SETTINGS:");
        
        string[] postGameplayNames = {
            "PostGameplay", "Post Gameplay", "PostGame", "Post Game",
            "Results", "GameResults", "Score", "EndGame"
        };
        
        bool foundPostGameplay = false;
        
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            
            Debug.Log($"   {i}: {sceneName}");
            
            foreach (string postName in postGameplayNames)
            {
                if (sceneName.Equals(postName, System.StringComparison.OrdinalIgnoreCase))
                {
                    foundPostGameplay = true;
                    Debug.Log($"✅ Escena PostGameplay encontrada: {sceneName}");
                }
            }
        }
        
        if (!foundPostGameplay)
        {
            Debug.LogError("❌ NO se encontró escena PostGameplay en Build Settings");
            Debug.LogError("💡 SOLUCIÓN: Agregar escena PostGameplay a Build Settings");
        }
    }
    
    /// <summary>
    /// Verifica sistemas de transición
    /// </summary>
    void CheckTransitionSystems()
    {
        Debug.Log("\n🔄 SISTEMAS DE TRANSICIÓN:");
        
        // Verificar AutoSceneTransition
        AutoSceneTransition autoTransition = FindFirstObjectByType<AutoSceneTransition>();
        if (autoTransition != null)
        {
            Debug.Log($"AutoSceneTransition: ✅ ENCONTRADO");
            Debug.Log($"   Habilitado: {(autoTransition.enableTransition ? "✅ SÍ" : "❌ NO")}");
            Debug.Log($"   GameObject: {autoTransition.gameObject.name}");
        }
        else
        {
            Debug.Log("AutoSceneTransition: ❌ NO ENCONTRADO");
        }
        
        // Verificar PauseAwareTransition
        PauseAwareTransition pauseTransition = FindFirstObjectByType<PauseAwareTransition>();
        if (pauseTransition != null)
        {
            Debug.Log($"PauseAwareTransition: ✅ ENCONTRADO");
            Debug.Log($"   Habilitado: {(pauseTransition.enableTransition ? "✅ SÍ" : "❌ NO")}");
            Debug.Log($"   GameObject: {pauseTransition.gameObject.name}");
        }
        else
        {
            Debug.Log("PauseAwareTransition: ❌ NO ENCONTRADO");
        }
        
        // Verificar SimpleGameplayTransition
        SimpleGameplayTransition simpleTransition = FindFirstObjectByType<SimpleGameplayTransition>();
        if (simpleTransition != null)
        {
            Debug.Log($"SimpleGameplayTransition: ✅ ENCONTRADO");
            Debug.Log($"   Habilitado: {(simpleTransition.enableTransition ? "✅ SÍ" : "❌ NO")}");
            Debug.Log($"   GameObject: {simpleTransition.gameObject.name}");
        }
        else
        {
            Debug.Log("SimpleGameplayTransition: ❌ NO ENCONTRADO");
        }
        
        // Verificar PostGameplayTransition
        PostGameplayTransition postTransition = FindFirstObjectByType<PostGameplayTransition>();
        if (postTransition != null)
        {
            Debug.Log($"PostGameplayTransition: ✅ ENCONTRADO");
            Debug.Log($"   Habilitado: {(postTransition.enableAutoTransition ? "✅ SÍ" : "❌ NO")}");
            Debug.Log($"   GameObject: {postTransition.gameObject.name}");
        }
        else
        {
            Debug.Log("PostGameplayTransition: ❌ NO ENCONTRADO");
        }
    }
    
    /// <summary>
    /// Verifica sistema de audio
    /// </summary>
    void CheckAudioSystem()
    {
        Debug.Log("\n🎵 SISTEMA DE AUDIO:");
        
        AudioSource[] audioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        Debug.Log($"AudioSources encontrados: {audioSources.Length}");
        
        foreach (AudioSource audio in audioSources)
        {
            Debug.Log($"   🔊 {audio.gameObject.name}:");
            Debug.Log($"      Reproduciendo: {(audio.isPlaying ? "✅ SÍ" : "❌ NO")}");
            Debug.Log($"      Clip asignado: {(audio.clip != null ? "✅ SÍ" : "❌ NO")}");
            
            if (audio.clip != null)
            {
                Debug.Log($"      Duración: {audio.clip.length:F1}s");
                Debug.Log($"      Tiempo actual: {audio.time:F1}s");
                Debug.Log($"      Progreso: {(audio.time / audio.clip.length):P1}");
            }
        }
        
        if (audioSources.Length == 0)
        {
            Debug.LogError("❌ NO se encontró ningún AudioSource");
            Debug.LogError("💡 El sistema de transición necesita un AudioSource para detectar el fin de la canción");
        }
    }
    
    /// <summary>
    /// Verifica estado del gameplay
    /// </summary>
    void CheckGameplayState()
    {
        Debug.Log("\n🎮 ESTADO DEL GAMEPLAY:");
        
        // Verificar GameplayManager
        GameplayManager gameplayManager = FindFirstObjectByType<GameplayManager>();
        if (gameplayManager != null)
        {
            Debug.Log("GameplayManager: ✅ ENCONTRADO");
            Debug.Log($"   GameObject: {gameplayManager.gameObject.name}");
        }
        else
        {
            Debug.Log("GameplayManager: ❌ NO ENCONTRADO");
        }
        
        // Verificar Time.timeScale
        Debug.Log($"Time.timeScale: {Time.timeScale}");
        if (Time.timeScale == 0f)
        {
            Debug.LogWarning("⚠️ Time.timeScale = 0 (juego pausado)");
        }
        
        // Verificar notas en escena
        Note[] notes = FindObjectsByType<Note>(FindObjectsSortMode.None);
        Debug.Log($"Notas en escena: {notes.Length}");
    }
    
    /// <summary>
    /// Monitoreo continuo
    /// </summary>
    void MonitorTransitionSystems()
    {
        AudioSource audio = FindFirstObjectByType<AudioSource>();
        if (audio != null && audio.clip != null)
        {
            float progress = audio.time / audio.clip.length;
            
            if (progress >= 0.9f && !audio.isPlaying)
            {
                Debug.Log($"⚠️ CANCIÓN TERMINADA ({progress:P1}) - ¿Por qué no hay transición?");
                
                // Verificar qué sistema debería estar funcionando
                SimpleGameplayTransition simple = FindFirstObjectByType<SimpleGameplayTransition>();
                if (simple != null && simple.enableTransition)
                {
                    Debug.Log("SimpleGameplayTransition está activo - debería funcionar");
                }
                
                AutoSceneTransition auto = FindFirstObjectByType<AutoSceneTransition>();
                if (auto != null && auto.enableTransition)
                {
                    Debug.Log("AutoSceneTransition está activo - debería funcionar");
                }
            }
        }
    }
    
    /// <summary>
    /// Proporciona solución
    /// </summary>
    void ProvideSolution()
    {
        Debug.Log("\n💡 SOLUCIÓN RECOMENDADA:");
        Debug.Log("═══════════════════════");
        
        Debug.Log("1. 🏗️ VERIFICAR BUILD SETTINGS:");
        Debug.Log("   - Abrir File → Build Settings");
        Debug.Log("   - Verificar que 'PostGameplay' esté en la lista");
        Debug.Log("   - Si no está, arrastrar la escena PostGameplay");
        
        Debug.Log("\n2. 🔄 AGREGAR SISTEMA DE TRANSICIÓN:");
        Debug.Log("   - Agregar SimpleGameplayTransition a cualquier GameObject");
        Debug.Log("   - O usar el botón 'Create Emergency Transition' abajo");
        
        Debug.Log("\n3. 🧪 PROBAR MANUALMENTE:");
        Debug.Log("   - Usar 'Force Transition Now' en cualquier sistema");
        Debug.Log("   - O presionar Enter durante el gameplay (si está habilitado)");
        
        Debug.Log("\n4. 🔍 VERIFICAR AUDIO:");
        Debug.Log("   - Asegurar que hay un AudioSource con clip asignado");
        Debug.Log("   - Verificar que la canción se reproduce correctamente");
    }
    
    /// <summary>
    /// Crear transición de emergencia
    /// </summary>
    [ContextMenu("Create Emergency Transition")]
    public void CreateEmergencyTransition()
    {
        Debug.Log("🚨 Creando sistema de transición de emergencia...");
        
        // Crear GameObject para la transición
        GameObject transitionObj = new GameObject("EmergencyTransition");
        SimpleGameplayTransition transition = transitionObj.AddComponent<SimpleGameplayTransition>();
        
        // Configurar para máxima compatibilidad
        transition.enableTransition = true;
        transition.delayAfterSongEnd = 2f;
        transition.useAudioSource = true;
        transition.useTimer = true;
        transition.useManualTrigger = true; // Permitir Enter para forzar
        transition.showDebugLogs = true;
        
        Debug.Log("✅ Sistema de transición de emergencia creado");
        Debug.Log("🎮 Ahora debería funcionar la transición al PostGameplay");
        Debug.Log("⌨️ También puedes presionar Enter para forzar la transición");
    }
    
    /// <summary>
    /// Test de transición inmediata
    /// </summary>
    [ContextMenu("Test Transition Now")]
    public void TestTransitionNow()
    {
        Debug.Log("🧪 Probando transición inmediata...");
        
        try
        {
            SceneManager.LoadScene("PostGameplay");
            Debug.Log("✅ Transición exitosa a PostGameplay");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"❌ Error en transición: {ex.Message}");
            Debug.LogError("💡 Verificar que PostGameplay esté en Build Settings");
        }
    }
}
