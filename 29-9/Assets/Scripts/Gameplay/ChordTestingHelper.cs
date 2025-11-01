using UnityEngine;
using System.Collections.Generic;

public class ChordTestingHelper : MonoBehaviour
{
    [Header("Testing Controls")]
    [Tooltip("Enable testing mode with keyboard shortcuts")]
    public bool enableTesting = true;
    
    [Header("Test Chord Patterns")]
    [Tooltip("Test different chord combinations")]
    public bool showTestInstructions = true;
    
    private NoteSpawner2D noteSpawner;
    private InputManager inputManager;
    private GameplayManager gameplayManager;
    
    void Start()
    {
        noteSpawner = FindFirstObjectByType<NoteSpawner2D>();
        inputManager = FindFirstObjectByType<InputManager>();
        gameplayManager = GameplayManager.Instance;
        
        if (showTestInstructions)
        {
            ShowTestInstructions();
        }
    }
    
    void Update()
    {
        if (!enableTesting) return;
        
        // Test chord spawning
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SpawnTestChord(new int[] { 0, 2 }); // D + J (Green + Yellow)
            Debug.Log("🎵 Test: Spawned 2-note chord (D + J)");
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SpawnTestChord(new int[] { 1, 3 }); // F + K (Red + Blue)
            Debug.Log("🎵 Test: Spawned 2-note chord (F + K)");
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SpawnTestChord(new int[] { 0, 2, 4 }); // D + J + L (Green + Yellow + Orange)
            Debug.Log("🎵 Test: Spawned 3-note chord (D + J + L)");
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SpawnTestChord(new int[] { 0, 1, 2, 3 }); // D + F + J + K (4-note chord)
            Debug.Log("🎵 Test: Spawned 4-note chord (D + F + J + K)");
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SpawnTestChord(new int[] { 0, 1, 2, 3, 4 }); // All 5 keys
            Debug.Log("🎵 Test: Spawned 5-note chord (ALL KEYS)");
        }
        
        // Test continuous notes + separate note pattern
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            StartCoroutine(SpawnContinuousPattern());
        }
        
        // Display current input state
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            DisplayInputState();
        }
        
        // Reset all active notes
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            ClearAllNotes();
        }
    }
    
    void SpawnTestChord(int[] laneIndices)
    {
        if (noteSpawner != null)
        {
            noteSpawner.SpawnChord(laneIndices);
        }
        else
        {
            Debug.LogWarning("⚠️ NoteSpawner2D not found! Make sure it's in the scene.");
        }
    }
    
    System.Collections.IEnumerator SpawnContinuousPattern()
    {
        Debug.Log("🎵 Test: Starting continuous + separate pattern");
        
        // Spawn 2 continuous notes
        SpawnTestChord(new int[] { 0, 1 }); // D + F
        yield return new WaitForSeconds(1f);
        
        // Spawn separate note
        SpawnTestChord(new int[] { 3 }); // K
        yield return new WaitForSeconds(1f);
        
        // Spawn another pattern
        SpawnTestChord(new int[] { 2, 4 }); // J + L
        yield return new WaitForSeconds(0.5f);
        SpawnTestChord(new int[] { 1 }); // F
        
        Debug.Log("🎵 Test: Continuous pattern completed");
    }
    
    void DisplayInputState()
    {
        if (inputManager == null) return;
        
        string heldKeys = "";
        for (int i = 0; i < inputManager.laneKeys.Length; i++)
        {
            if (inputManager.IsKeyHeld(i))
            {
                heldKeys += $"{inputManager.laneKeys[i]} ";
            }
        }
        
        Debug.Log($"🎮 Currently held keys: {(string.IsNullOrEmpty(heldKeys) ? "None" : heldKeys)}");
        
        if (gameplayManager != null)
        {
            Debug.Log($"🎯 Active notes count: {gameplayManager.activeNotes?.Count ?? 0}");
            Debug.Log($"🎵 Song time: {gameplayManager.GetSongTime():F2}s");
        }
    }
    
    void ClearAllNotes()
    {
        if (noteSpawner != null)
        {
            // This would need to be implemented in NoteSpawner2D
            Debug.Log("🧹 Clearing all active notes (feature needs implementation in NoteSpawner2D)");
        }
    }
    
    void ShowTestInstructions()
    {
        Debug.Log("🎮 CHORD TESTING HELPER ACTIVATED");
        Debug.Log("📋 Test Controls:");
        Debug.Log("   1 - Spawn 2-note chord (D + J)");
        Debug.Log("   2 - Spawn 2-note chord (F + K)");
        Debug.Log("   3 - Spawn 3-note chord (D + J + L)");
        Debug.Log("   4 - Spawn 4-note chord (D + F + J + K)");
        Debug.Log("   5 - Spawn 5-note chord (ALL KEYS)");
        Debug.Log("   6 - Spawn continuous + separate pattern");
        Debug.Log("   Tab - Display current input state");
        Debug.Log("   Backspace - Clear all notes");
        Debug.Log("🎯 Game Controls: D, F, J, K, L");
        Debug.Log("⚡ Try pressing multiple keys simultaneously to test chord detection!");
    }
    
    void OnGUI()
    {
        if (!enableTesting || !showTestInstructions) return;
        
        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.BeginVertical("box");
        
        GUILayout.Label("🎮 CHORD TESTING", GUI.skin.box);
        
        if (GUILayout.Button("Spawn 2-Note Chord (D+J)"))
            SpawnTestChord(new int[] { 0, 2 });
            
        if (GUILayout.Button("Spawn 3-Note Chord (D+J+L)"))
            SpawnTestChord(new int[] { 0, 2, 4 });
            
        if (GUILayout.Button("Spawn 4-Note Chord"))
            SpawnTestChord(new int[] { 0, 1, 2, 3 });
            
        if (GUILayout.Button("Clear All Notes"))
            ClearAllNotes();
            
        GUILayout.Space(10);
        GUILayout.Label("Press multiple keys simultaneously!", GUI.skin.box);
        GUILayout.Label("Keys: D, F, J, K, L", GUI.skin.box);
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
}
