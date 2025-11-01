using UnityEngine;

/// <summary>
/// Script para limpiar automáticamente los componentes problemáticos del sistema de pausa
/// </summary>
public class PauseSystemCleaner : MonoBehaviour
{
    [Header("Auto Clean")]
    public bool cleanOnStart = true;

    void Start()
    {
        if (cleanOnStart)
        {
            CleanupPauseSystem();
        }
    }

    [ContextMenu("Clean Pause System")]
    public void CleanupPauseSystem()
    {
        Debug.Log("🧹 Starting pause system cleanup...");

        // Remove all GameStartCountdown components
        GameStartCountdown[] startCountdowns = FindObjectsByType<GameStartCountdown>(FindObjectsSortMode.None);
        foreach (var countdown in startCountdowns)
        {
            Debug.Log($"🗑️ Removing GameStartCountdown from {countdown.gameObject.name}");
            DestroyImmediate(countdown);
        }

        // Remove all GameplayPauseIntegration components
        GameplayPauseIntegration[] integrations = FindObjectsByType<GameplayPauseIntegration>(FindObjectsSortMode.None);
        foreach (var integration in integrations)
        {
            Debug.Log($"🗑️ Removing GameplayPauseIntegration from {integration.gameObject.name}");
            DestroyImmediate(integration);
        }

        // Clean up any countdown UI objects
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (var obj in allObjects)
        {
            if (obj.name.Contains("Countdown") || obj.name.Contains("PauseMenu"))
            {
                if (obj.GetComponent<SimplePauseSetup>() == null) // Don't destroy SimplePauseSetup objects
                {
                    Debug.Log($"🗑️ Cleaning up UI object: {obj.name}");
                    DestroyImmediate(obj);
                }
            }
        }

        // Ensure only one SimplePauseSetup exists
        SimplePauseSetup[] pauseSetups = FindObjectsByType<SimplePauseSetup>(FindObjectsSortMode.None);
        if (pauseSetups.Length > 1)
        {
            Debug.Log($"⚠️ Found {pauseSetups.Length} SimplePauseSetup components, keeping only the first one");
            for (int i = 1; i < pauseSetups.Length; i++)
            {
                DestroyImmediate(pauseSetups[i].gameObject);
            }
        }
        else if (pauseSetups.Length == 0)
        {
            Debug.Log("✅ No SimplePauseSetup found, creating one...");
            CreateCleanPauseSetup();
        }

        // Reset Time.timeScale and AudioListener
        Time.timeScale = 1f;
        AudioListener.pause = false;

        Debug.Log("✅ Pause system cleanup completed!");
    }

    void CreateCleanPauseSetup()
    {
        GameObject pauseObj = new GameObject("CleanPauseSystem");
        SimplePauseSetup pauseSetup = pauseObj.AddComponent<SimplePauseSetup>();

        Debug.Log("✅ Clean SimplePauseSetup created");
    }

    [ContextMenu("Force Resume Everything")]
    public void ForceResumeEverything()
    {
        Debug.Log("🚨 FORCE RESUMING EVERYTHING");

        // Stop all coroutines in the scene
        MonoBehaviour[] allMonoBehaviours = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
        foreach (var mb in allMonoBehaviours)
        {
            if (mb != null && mb != this)
            {
                mb.StopAllCoroutines();
            }
        }

        // Reset time and audio
        Time.timeScale = 1f;
        AudioListener.pause = false;

        // Resume all audio sources
        AudioSource[] sources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        foreach (var source in sources)
        {
            if (source != null)
            {
                source.volume = 1f;
                source.UnPause();
                if (!source.isPlaying && source.clip != null)
                {
                    source.Play();
                }
            }
        }

        // Enable all disabled scripts
        MonoBehaviour[] scripts = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
        foreach (var script in scripts)
        {
            if (script != null && script != this)
            {
                script.enabled = true;
            }
        }

        // Clean up any remaining countdown UI
        GameObject[] countdownObjects = GameObject.FindGameObjectsWithTag("Untagged");
        foreach (var obj in countdownObjects)
        {
            if (obj.name.Contains("Countdown"))
            {
                DestroyImmediate(obj);
            }
        }

        Debug.Log("✅ Everything force resumed!");
    }

    void OnGUI()
    {
        // Emergency buttons in game view
        if (GUI.Button(new Rect(10, 10, 150, 30), "Clean Pause System"))
        {
            CleanupPauseSystem();
        }

        if (GUI.Button(new Rect(10, 50, 150, 30), "Force Resume All"))
        {
            ForceResumeEverything();
        }

        // Show current state
        GUI.Label(new Rect(10, 90, 200, 20), $"Time Scale: {Time.timeScale}");
        GUI.Label(new Rect(10, 110, 200, 20), $"Audio Paused: {AudioListener.pause}");
    }
}