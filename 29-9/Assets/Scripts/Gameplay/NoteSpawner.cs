using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class NoteSpawner : MonoBehaviour
{
    public GameObject notePrefab;
    public Transform[] lanes;
    public float noteSpeed = 15f; // Increased speed for better gameplay
    
    [Header("Spawn Settings")]
    public float spawnDistance = 25f; // Distance ahead of hit zone to spawn notes
    public float hitZoneZ = -8f; // Z position of the hit zone (moved to match detection)
    public float lookAheadTime = 1.5f; // Time in seconds to look ahead for spawning
    
    [Header("Visual Effects")]
    public HighwayFadeEffect fadeEffect;
    public Material[] laneMaterials = new Material[5]; // Materials for each lane
    
    private List<NoteData> notes = new List<NoteData>();
    
    // Events
    public System.Action<NoteData, Note> OnNoteSpawned;

    IEnumerator Start()
    {
        // Auto-create lanes if not assigned
        if (lanes == null || lanes.Length == 0 || System.Array.Exists(lanes, lane => lane == null))
        {
            CreateLanes();
        }
        
        while (GameplayManager.Instance.selectedNotes == null || GameplayManager.Instance.selectedNotes.Count == 0)
            yield return null;

        LoadNotesFromGameplayManager();
    }

    void Update()
    {
        float songTime = GameplayManager.Instance.GetSongTime();
        
        // Calculate travel time for notes to reach hit zone
        float travelTime = spawnDistance / noteSpeed;

        foreach (NoteData note in notes)
        {
            // Spawn note early so it arrives at hitzone exactly at note.time
            // No additional lookAheadTime needed - just pure travel time
            float spawnTime = note.time - travelTime;
            
            if (!note.spawned && songTime >= spawnTime)
            {
                SpawnNote(note);
                note.spawned = true;
                
                // Note spawned
            }
        }
    }

    void LoadNotesFromGameplayManager()
    {
        // Get the parsed notes from GameplayManager
        notes = new List<NoteData>(GameplayManager.Instance.selectedNotes);
        
        // Notes loaded from GameplayManager
        
        // Sort notes by time to ensure proper spawning order
        notes.Sort((a, b) => a.time.CompareTo(b.time));
    }

    void SpawnNote(NoteData noteData)
    {
        if (noteData.laneIndex >= 0 && noteData.laneIndex < lanes.Length)
        {
            // Calculate spawn position (far ahead for fade effect)
            Vector3 lanePosition = lanes[noteData.laneIndex].position;
            Vector3 spawnPosition = new Vector3(lanePosition.x, lanePosition.y, hitZoneZ + spawnDistance);
            
            GameObject newNoteObject = Instantiate(notePrefab, spawnPosition, Quaternion.identity);

            Note noteScript = newNoteObject.GetComponent<Note>();
            if (noteScript != null)
            {
                // Set up the note script
                noteScript.lane = noteData.laneIndex;
                noteScript.speed = noteSpeed;
                noteScript.noteData = noteData;
                
                // Apply lane material and fade effect
                ApplyNoteVisuals(newNoteObject, noteData.laneIndex);
                
                // Subscribe to note destruction event
                noteScript.OnNoteDestroyed += OnNoteObjectDestroyed;
            }
            
            // Add NoteDestroyer component for automatic cleanup
            NoteDestroyer destroyer = newNoteObject.GetComponent<NoteDestroyer>();
            if (destroyer == null)
            {
                destroyer = newNoteObject.AddComponent<NoteDestroyer>();
                // Configure destroyer settings if needed
                destroyer.behindCameraOffset = 5f;
                destroyer.farFromCameraOffset = 50f;
            }
            
            // Notify that a note has been spawned
            OnNoteSpawned?.Invoke(noteData, noteScript);

            // Nota instanciada
        }
    }
    
    void OnNoteObjectDestroyed(Note note)
    {
        // Clean up when a note is destroyed
        if (note != null)
        {
            note.OnNoteDestroyed -= OnNoteObjectDestroyed;
        }
    }
    
    void ApplyNoteVisuals(GameObject noteObject, int laneIndex)
    {
        // Apply lane-specific material
        if (laneIndex >= 0 && laneIndex < laneMaterials.Length && laneMaterials[laneIndex] != null)
        {
            Renderer renderer = noteObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = laneMaterials[laneIndex];
                
                // Apply fade effect if available
                if (fadeEffect != null)
                {
                    fadeEffect.ApplyFadeToNote(renderer, noteObject.transform.position.z);
                }
            }
        }
        
        // Apply to both 3D Renderer and SpriteRenderer
        Renderer renderer3D = noteObject.GetComponent<Renderer>();
        SpriteRenderer sr = noteObject.GetComponent<SpriteRenderer>();
        
        // Set lane colors (Guitar Hero standard)
        Color[] laneColors = {
            Color.green,                    // Lane 0 - Green
            Color.red,                      // Lane 1 - Red  
            Color.yellow,                   // Lane 2 - Yellow
            Color.blue,                     // Lane 3 - Blue
            new Color(1f, 0.5f, 0f, 1f)   // Lane 4 - Orange
        };
        
        if (laneIndex >= 0 && laneIndex < laneColors.Length)
        {
            Color noteColor = laneColors[laneIndex];
            
            // Apply to 3D renderer
            if (renderer3D != null)
            {
                Material mat = renderer3D.material;
                mat.color = noteColor;
                
                // Apply fade effect if available
                if (fadeEffect != null)
                {
                    fadeEffect.ApplyFadeToNote(renderer3D, noteObject.transform.position.z);
                }
            }
            
            // Apply to sprite renderer
            if (sr != null)
            {
                sr.color = noteColor;
            }
        }
    }
    
    /// <summary>
    /// Creates lanes automatically if not assigned
    /// </summary>
    void CreateLanes()
    {
        Debug.Log("🛣️ Auto-creating lanes for NoteSpawner...");
        
        // Create lanes container
        GameObject lanesContainer = GameObject.Find("Lanes");
        if (lanesContainer == null)
        {
            lanesContainer = new GameObject("Lanes");
            lanesContainer.transform.position = Vector3.zero;
        }
        
        // Create 5 lanes (standard Guitar Hero)
        lanes = new Transform[5];
        float laneSpacing = 2f;
        float startX = -4f;
        
        for (int i = 0; i < 5; i++)
        {
            GameObject laneObj = GameObject.Find($"Lane_{i}");
            if (laneObj == null)
            {
                laneObj = new GameObject($"Lane_{i}");
                laneObj.transform.SetParent(lanesContainer.transform);
                
                // Position lane
                float xPos = startX + (i * laneSpacing);
                laneObj.transform.position = new Vector3(xPos, 0, 0);
                
                Debug.Log($"🛣️ Created Lane_{i} at position ({xPos}, 0, 0)");
            }
            
            lanes[i] = laneObj.transform;
        }
        
        Debug.Log($"✅ {lanes.Length} lanes created and assigned to NoteSpawner");
    }
}
