using UnityEngine;
using System.Collections.Generic;

public class ChordSystemDiagnostic : MonoBehaviour
{
    [Header("Diagnostic Settings")]
    public bool enableDiagnostic = true;
    public bool verboseLogging = true;
    
    private GameplayManager gameplayManager;
    private InputManager inputManager;
    private NoteSpawner2D noteSpawner;
    
    void Start()
    {
        if (!enableDiagnostic) return;
        
        // Find components
        gameplayManager = GameplayManager.Instance;
        inputManager = FindFirstObjectByType<InputManager>();
        noteSpawner = FindFirstObjectByType<NoteSpawner2D>();
        
        // Run initial diagnostic
        RunInitialDiagnostic();
        
        // Show instructions
        Debug.Log("🔧 CHORD SYSTEM DIAGNOSTIC ACTIVATED");
        Debug.Log("Press SPACE to run full diagnostic");
        Debug.Log("Press G to show current game state");
        Debug.Log("Press T to test chord spawning");
    }
    
    void Update()
    {
        if (!enableDiagnostic) return;
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RunFullDiagnostic();
        }
        
        if (Input.GetKeyDown(KeyCode.G))
        {
            ShowGameState();
        }
        
        if (Input.GetKeyDown(KeyCode.T))
        {
            TestChordSpawning();
        }
        
        // Monitor input in real-time
        if (verboseLogging)
        {
            MonitorInput();
        }
    }
    
    void RunInitialDiagnostic()
    {
        Debug.Log("🔍 === INITIAL SYSTEM DIAGNOSTIC ===");
        
        // Check GameplayManager
        if (gameplayManager == null)
        {
            Debug.LogError("❌ GameplayManager.Instance is NULL!");
        }
        else
        {
            Debug.Log($"✅ GameplayManager found - Active: {gameplayManager.isGameActive}");
            Debug.Log($"📊 Active notes count: {gameplayManager.activeNotes?.Count ?? 0}");
        }
        
        // Check InputManager
        if (inputManager == null)
        {
            Debug.LogError("❌ InputManager not found!");
        }
        else
        {
            Debug.Log($"✅ InputManager found");
            Debug.Log($"🎹 Lane keys: [{string.Join(", ", inputManager.laneKeys)}]");
            Debug.Log($"⚙️ Chord settings: {(inputManager.chordSettings != null ? "Assigned" : "NULL")}");
        }
        
        // Check NoteSpawner2D
        if (noteSpawner == null)
        {
            Debug.LogError("❌ NoteSpawner2D not found!");
        }
        else
        {
            Debug.Log($"✅ NoteSpawner2D found");
            Debug.Log($"🎵 Chords enabled: {noteSpawner.enableChords}");
            Debug.Log($"🎲 Chord probability: {noteSpawner.chordProbability}");
            Debug.Log($"📏 Horizontal spacing: {noteSpawner.horizontalSpacing}");
        }
        
        Debug.Log("🔍 === END INITIAL DIAGNOSTIC ===");
    }
    
    void RunFullDiagnostic()
    {
        Debug.Log("🔧 === FULL SYSTEM DIAGNOSTIC ===");
        
        // Test component references
        RunInitialDiagnostic();
        
        // Test chord spawning
        if (noteSpawner != null)
        {
            Debug.Log("🧪 Testing chord spawning...");
            noteSpawner.SpawnChord(new int[] { 0, 2 }); // D + J
            Debug.Log("✅ Chord spawn command sent");
        }
        
        // Show current input state
        ShowInputState();
        
        // Show game state
        ShowGameState();
        
        Debug.Log("🔧 === END FULL DIAGNOSTIC ===");
    }
    
    void ShowGameState()
    {
        Debug.Log("🎮 === CURRENT GAME STATE ===");
        
        if (gameplayManager != null)
        {
            Debug.Log($"🎵 Game active: {gameplayManager.isGameActive}");
            Debug.Log($"⏸️ Game paused: {gameplayManager.isPaused}");
            Debug.Log($"⏱️ Song time: {gameplayManager.GetSongTime():F2}s");
            Debug.Log($"📝 Active notes: {gameplayManager.activeNotes?.Count ?? 0}");
            Debug.Log($"🎯 Hit window: {gameplayManager.hitWindow}");
        }
        
        Debug.Log("🎮 === END GAME STATE ===");
    }
    
    void ShowInputState()
    {
        if (inputManager == null) return;
        
        Debug.Log("⌨️ === INPUT STATE ===");
        
        string heldKeys = "";
        for (int i = 0; i < inputManager.laneKeys.Length; i++)
        {
            if (Input.GetKey(inputManager.laneKeys[i]))
            {
                heldKeys += $"{inputManager.laneKeys[i]} ";
            }
        }
        
        Debug.Log($"🎹 Currently held: {(string.IsNullOrEmpty(heldKeys) ? "None" : heldKeys)}");
        Debug.Log("⌨️ === END INPUT STATE ===");
    }
    
    void TestChordSpawning()
    {
        if (noteSpawner == null)
        {
            Debug.LogError("❌ Cannot test - NoteSpawner2D is null!");
            return;
        }
        
        Debug.Log("🧪 === TESTING CHORD SPAWNING ===");
        
        // Test different chord sizes
        Debug.Log("🎵 Spawning 2-note chord (D+J)...");
        noteSpawner.SpawnChord(new int[] { 0, 2 });
        
        // Wait and spawn another
        StartCoroutine(DelayedChordTest());
    }
    
    System.Collections.IEnumerator DelayedChordTest()
    {
        yield return new WaitForSeconds(2f);
        
        Debug.Log("🎵 Spawning 3-note chord (D+F+J)...");
        noteSpawner.SpawnChord(new int[] { 0, 1, 2 });
        
        yield return new WaitForSeconds(2f);
        
        Debug.Log("🎵 Spawning 4-note chord (D+F+J+K)...");
        noteSpawner.SpawnChord(new int[] { 0, 1, 2, 3 });
        
        Debug.Log("🧪 === END CHORD SPAWNING TEST ===");
    }
    
    void MonitorInput()
    {
        if (inputManager == null) return;
        
        // Check for simultaneous key presses
        List<KeyCode> pressedKeys = new List<KeyCode>();
        
        for (int i = 0; i < inputManager.laneKeys.Length; i++)
        {
            if (Input.GetKeyDown(inputManager.laneKeys[i]))
            {
                pressedKeys.Add(inputManager.laneKeys[i]);
            }
        }
        
        if (pressedKeys.Count > 1)
        {
            Debug.Log($"🎹 SIMULTANEOUS KEYS DETECTED: [{string.Join(", ", pressedKeys)}]");
        }
        else if (pressedKeys.Count == 1)
        {
            Debug.Log($"🎹 Single key pressed: {pressedKeys[0]}");
        }
    }
    
    void OnGUI()
    {
        if (!enableDiagnostic) return;
        
        GUILayout.BeginArea(new Rect(Screen.width - 320, 10, 300, 300));
        GUILayout.BeginVertical("box");
        
        GUILayout.Label("🔧 CHORD DIAGNOSTIC", GUI.skin.box);
        
        if (GUILayout.Button("Run Full Diagnostic (SPACE)"))
            RunFullDiagnostic();
            
        if (GUILayout.Button("Show Game State (G)"))
            ShowGameState();
            
        if (GUILayout.Button("Test Chord Spawning (T)"))
            TestChordSpawning();
        
        GUILayout.Space(10);
        
        // Show component status
        GUILayout.Label("Component Status:", GUI.skin.box);
        GUILayout.Label($"GameplayManager: {(gameplayManager != null ? "✅" : "❌")}");
        GUILayout.Label($"InputManager: {(inputManager != null ? "✅" : "❌")}");
        GUILayout.Label($"NoteSpawner2D: {(noteSpawner != null ? "✅" : "❌")}");
        
        if (gameplayManager != null)
        {
            GUILayout.Label($"Game Active: {gameplayManager.isGameActive}");
            GUILayout.Label($"Active Notes: {gameplayManager.activeNotes?.Count ?? 0}");
        }
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
}
