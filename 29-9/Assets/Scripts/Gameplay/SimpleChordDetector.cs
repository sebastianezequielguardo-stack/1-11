using UnityEngine;
using System.Collections.Generic;

public class SimpleChordDetector : MonoBehaviour
{
    [Header("Chord Detection")]
    public KeyCode[] gameKeys = { KeyCode.D, KeyCode.F, KeyCode.J, KeyCode.K, KeyCode.L };
    public float chordTimeWindow = 0.1f; // 100ms window for simultaneous presses
    
    [Header("Testing")]
    public bool enableDebugLogs = true;
    public bool autoSpawnTestNotes = true;
    public float spawnInterval = 3f;
    
    private Dictionary<KeyCode, float> keyPressTime = new Dictionary<KeyCode, float>();
    private List<KeyCode> currentChord = new List<KeyCode>();
    private NoteSpawner2D noteSpawner;
    private float lastSpawnTime = 0f;
    
    void Start()
    {
        noteSpawner = FindFirstObjectByType<NoteSpawner2D>();
        
        // Initialize key press times
        foreach (var key in gameKeys)
        {
            keyPressTime[key] = -1f;
        }
        
        if (enableDebugLogs)
        {
            Debug.Log("🎮 SimpleChordDetector iniciado");
            Debug.Log("🎹 Teclas: D, F, J, K, L");
            Debug.Log("⏱️ Ventana de tiempo: " + chordTimeWindow + "s");
            Debug.Log("🧪 Auto spawn: " + autoSpawnTestNotes);
        }
    }
    
    void Update()
    {
        DetectChordInput();
        
        if (autoSpawnTestNotes)
        {
            AutoSpawnTestNotes();
        }
    }
    
    void DetectChordInput()
    {
        float currentTime = Time.time;
        bool anyKeyPressed = false;
        
        // Check for key presses
        foreach (var key in gameKeys)
        {
            if (Input.GetKeyDown(key))
            {
                keyPressTime[key] = currentTime;
                anyKeyPressed = true;
                
                if (enableDebugLogs)
                    Debug.Log($"🎹 Tecla presionada: {key} a las {currentTime:F3}s");
            }
        }
        
        if (anyKeyPressed)
        {
            // Find all keys pressed within the time window
            currentChord.Clear();
            
            foreach (var kvp in keyPressTime)
            {
                if (kvp.Value > 0 && (currentTime - kvp.Value) <= chordTimeWindow)
                {
                    currentChord.Add(kvp.Key);
                }
            }
            
            // Process chord if we have multiple keys
            if (currentChord.Count >= 2)
            {
                ProcessChord(currentChord);
                
                // Clear processed keys
                foreach (var key in currentChord)
                {
                    keyPressTime[key] = -1f;
                }
            }
            else if (currentChord.Count == 1)
            {
                // Single note
                ProcessSingleNote(currentChord[0]);
                keyPressTime[currentChord[0]] = -1f;
            }
        }
    }
    
    void ProcessChord(List<KeyCode> chord)
    {
        if (enableDebugLogs)
        {
            string chordStr = string.Join(" + ", chord);
            Debug.Log($"🎯 ACORDE DETECTADO: {chordStr} ({chord.Count} teclas)");
        }
        
        // Convert keys to lane indices
        List<int> lanes = new List<int>();
        foreach (var key in chord)
        {
            int laneIndex = System.Array.IndexOf(gameKeys, key);
            if (laneIndex >= 0)
            {
                lanes.Add(laneIndex);
            }
        }
        
        // Test different chord patterns
        if (IsPattern_2_Continuous_1_Separate(lanes))
        {
            Debug.Log("✅ PATRÓN DETECTADO: 2 continuas + 1 separada");
        }
        else if (lanes.Count == 4)
        {
            Debug.Log("✅ PATRÓN DETECTADO: 4 teclas juntas");
        }
        else if (lanes.Count >= 2)
        {
            Debug.Log($"✅ ACORDE GENÉRICO: {lanes.Count} teclas");
        }
        
        // Simulate successful hit
        ShowChordSuccess(chord);
    }
    
    void ProcessSingleNote(KeyCode key)
    {
        int laneIndex = System.Array.IndexOf(gameKeys, key);
        if (enableDebugLogs)
            Debug.Log($"🎵 Nota individual: {key} (Lane {laneIndex})");
    }
    
    bool IsPattern_2_Continuous_1_Separate(List<int> lanes)
    {
        if (lanes.Count != 3) return false;
        
        lanes.Sort();
        
        // Check if we have 2 continuous and 1 separate
        // Example: 0,1,3 (D+F continuous, K separate)
        // Example: 1,2,4 (F+J continuous, L separate)
        
        for (int i = 0; i < lanes.Count - 1; i++)
        {
            if (lanes[i + 1] - lanes[i] == 1) // Found continuous pair
            {
                // Check if the third note is separate (gap > 1)
                for (int j = 0; j < lanes.Count; j++)
                {
                    if (j != i && j != i + 1)
                    {
                        int gap1 = Mathf.Abs(lanes[j] - lanes[i]);
                        int gap2 = Mathf.Abs(lanes[j] - lanes[i + 1]);
                        if (gap1 > 1 && gap2 > 1)
                        {
                            return true;
                        }
                    }
                }
            }
        }
        
        return false;
    }
    
    void ShowChordSuccess(List<KeyCode> chord)
    {
        // Visual feedback
        string chordDisplay = string.Join(" + ", chord);
        
        // Create a temporary UI message (you can enhance this)
        GameObject messageObj = new GameObject("ChordMessage");
        var canvas = FindFirstObjectByType<Canvas>();
        if (canvas != null)
        {
            messageObj.transform.SetParent(canvas.transform);
            var text = messageObj.AddComponent<UnityEngine.UI.Text>();
            text.text = $"ACORDE: {chordDisplay}";
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 24;
            text.color = Color.green;
            text.alignment = TextAnchor.MiddleCenter;
            
            var rectTransform = messageObj.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(0, 100);
            rectTransform.sizeDelta = new Vector2(400, 50);
            
            // Destroy after 2 seconds
            Destroy(messageObj, 2f);
        }
        
        Debug.Log($"🎉 ÉXITO! Acorde ejecutado: {chordDisplay}");
    }
    
    void AutoSpawnTestNotes()
    {
        if (Time.time - lastSpawnTime > spawnInterval)
        {
            SpawnRandomTestPattern();
            lastSpawnTime = Time.time;
        }
    }
    
    void SpawnRandomTestPattern()
    {
        if (noteSpawner == null) return;
        
        int pattern = Random.Range(0, 3);
        
        switch (pattern)
        {
            case 0: // 2 continuous + 1 separate
                noteSpawner.SpawnChord(new int[] { 0, 1, 3 }); // D+F continuous, K separate
                Debug.Log("🎵 Spawned: 2 continuous + 1 separate (D+F+K)");
                break;
                
            case 1: // 4 keys together
                noteSpawner.SpawnChord(new int[] { 0, 1, 2, 3 }); // D+F+J+K
                Debug.Log("🎵 Spawned: 4 keys together (D+F+J+K)");
                break;
                
            case 2: // Another 2+1 pattern
                noteSpawner.SpawnChord(new int[] { 1, 2, 4 }); // F+J continuous, L separate
                Debug.Log("🎵 Spawned: 2 continuous + 1 separate (F+J+L)");
                break;
        }
    }
    
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(Screen.width - 350, 10, 340, 200));
        GUILayout.BeginVertical("box");
        
        GUILayout.Label("🎮 SIMPLE CHORD DETECTOR", GUI.skin.box);
        
        GUILayout.Label("Patrones a probar:");
        GUILayout.Label("• D+F+K (2 continuas + 1 separada)");
        GUILayout.Label("• D+F+J+K (4 teclas juntas)");
        GUILayout.Label("• F+J+L (2 continuas + 1 separada)");
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Spawn Test Pattern"))
        {
            SpawnRandomTestPattern();
        }
        
        GUILayout.Label($"Auto spawn: {(autoSpawnTestNotes ? "ON" : "OFF")}");
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
}
