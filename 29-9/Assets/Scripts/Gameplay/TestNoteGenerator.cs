using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Generates test notes for gameplay testing without needing chart files
/// </summary>
public class TestNoteGenerator : MonoBehaviour
{
    [Header("Test Settings")]
    public bool generateTestNotes = true;
    public float noteInterval = 1f; // Seconds between notes
    public int totalTestNotes = 50;
    
    private GameplayManager gameplayManager;
    private List<NoteData> testNotes = new List<NoteData>();
    
    void Start()
    {
        gameplayManager = GameplayManager.Instance;
        
        if (generateTestNotes && gameplayManager != null)
        {
            StartCoroutine(InitializeTestNotes());
        }
    }
    
    IEnumerator InitializeTestNotes()
    {
        // Wait for GameplayManager to initialize
        yield return new WaitForSeconds(1f);
        
        GenerateTestNotes();
        gameplayManager.selectedNotes = testNotes;
        
        // Start test gameplay
        gameplayManager.StartTestGameplay();
        
        Debug.Log("🎮 Test gameplay started!");
    }
    
    void GenerateTestNotes()
    {
        Debug.Log("🎵 Generating test notes...");
        
        for (int i = 0; i < totalTestNotes; i++)
        {
            float time = i * noteInterval + 2f; // Start after 2 seconds
            int lane = i % 5; // Cycle through all lanes
            
            NoteData note = new NoteData(time, lane);
            note.noteType = NoteType.Normal;
            note.duration = 0f; // Regular note, not sustained
            
            testNotes.Add(note);
        }
        
        // Sort by time
        testNotes.Sort((a, b) => a.time.CompareTo(b.time));
        
        Debug.Log($"✅ Generated {testNotes.Count} test notes");
    }
    
    void Update()
    {
        // Press T to generate new test notes during gameplay
        if (Input.GetKeyDown(KeyCode.T) && gameplayManager != null)
        {
            testNotes.Clear();
            GenerateTestNotes();
            gameplayManager.selectedNotes = testNotes;
            Debug.Log("🔄 Regenerated test notes");
        }
        
        // Press R to restart gameplay
        if (Input.GetKeyDown(KeyCode.R) && gameplayManager != null)
        {
            StartCoroutine(RestartGameplay());
        }
    }
    
    IEnumerator RestartGameplay()
    {
        // Reset game state
        gameplayManager.isGameActive = false;
        yield return new WaitForSeconds(0.1f);
        
        // Regenerate and restart
        testNotes.Clear();
        GenerateTestNotes();
        gameplayManager.selectedNotes = testNotes;
        gameplayManager.StartTestGameplay();
        
        Debug.Log("🔄 Restarted gameplay");
    }
}
