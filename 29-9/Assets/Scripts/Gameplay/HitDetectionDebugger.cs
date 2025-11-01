using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Debugger para diagnosticar problemas de detección de notas
/// Agrega este script a cualquier GameObject para diagnosticar el sistema
/// </summary>
public class HitDetectionDebugger : MonoBehaviour
{
    [Header("Debug Settings")]
    public bool enableDebugMode = false; // Desactivado por defecto para evitar UI en pantalla
    public bool showActiveNotes = false; // Deshabilitado para experiencia limpia
    public bool showInputDetection = false; // Deshabilitado para experiencia limpia
    public bool forceTestNotes = false;
    
    [Header("Test Note Generation")]
    public bool generateTestNotesOnStart = true;
    public float testNoteInterval = 2f;
    public int testNotesCount = 20;
    
    private GameplayManager gameplayManager;
    private InputManager inputManager;
    private NoteSpawner noteSpawner;
    
    void Start()
    {
        FindComponents();
        
        if (generateTestNotesOnStart)
        {
            StartCoroutine(InitializeTestMode());
        }
    }
    
    void FindComponents()
    {
        gameplayManager = GameplayManager.Instance;
        inputManager = FindFirstObjectByType<InputManager>();
        noteSpawner = FindFirstObjectByType<NoteSpawner>();
        
        Debug.Log($"🔍 COMPONENTES ENCONTRADOS:");
        Debug.Log($"   GameplayManager: {(gameplayManager != null ? "✅" : "❌")}");
        Debug.Log($"   InputManager: {(inputManager != null ? "✅" : "❌")}");
        Debug.Log($"   NoteSpawner: {(noteSpawner != null ? "✅" : "❌")}");
    }
    
    System.Collections.IEnumerator InitializeTestMode()
    {
        yield return new WaitForSeconds(1f);
        
        if (gameplayManager == null)
        {
            Debug.LogError("❌ GameplayManager no encontrado - no se pueden generar notas de prueba");
            yield break;
        }
        
        // Generar notas de prueba
        GenerateTestNotes();
        
        // Activar el gameplay
        gameplayManager.isGameActive = true;
        
        Debug.Log("🎮 MODO DE PRUEBA ACTIVADO - Presiona D, F, J, K, L para probar");
    }
    
    void GenerateTestNotes()
    {
        List<NoteData> testNotes = new List<NoteData>();
        
        for (int i = 0; i < testNotesCount; i++)
        {
            float time = i * testNoteInterval + 3f; // Empezar después de 3 segundos
            int lane = i % 5; // Rotar entre todos los lanes
            
            NoteData note = new NoteData(time, lane);
            note.noteType = NoteType.Normal;
            note.duration = 0f;
            
            testNotes.Add(note);
        }
        
        // Asignar las notas al GameplayManager
        gameplayManager.selectedNotes = testNotes;
        
        Debug.Log($"✅ Generadas {testNotes.Count} notas de prueba");
        Debug.Log($"   Primera nota en lane {testNotes[0].laneIndex} a los {testNotes[0].time:F1}s");
        Debug.Log($"   Última nota en lane {testNotes[testNotes.Count-1].laneIndex} a los {testNotes[testNotes.Count-1].time:F1}s");
    }
    
    void Update()
    {
        if (!enableDebugMode) return;
        
        // Mostrar información de debug
        if (showActiveNotes)
        {
            ShowActiveNotesInfo();
        }
        
        if (showInputDetection)
        {
            ShowInputInfo();
        }
        
        // Controles de debug
        HandleDebugControls();
    }
    
    void ShowActiveNotesInfo()
    {
        if (gameplayManager == null) return;
        
        // Usar reflexión para acceder a activeNotes (es privado)
        var activeNotesField = typeof(GameplayManager).GetField("activeNotes", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (activeNotesField != null)
        {
            var activeNotes = activeNotesField.GetValue(gameplayManager) as List<NoteData>;
            
            if (activeNotes != null && activeNotes.Count > 0)
            {
                float currentTime = gameplayManager.GetSongTime();
                Debug.Log($"🎵 NOTAS ACTIVAS ({activeNotes.Count}): Tiempo actual: {currentTime:F2}s");
                
                foreach (var note in activeNotes)
                {
                    float timeUntilHit = note.time - currentTime;
                    Debug.Log($"   Lane {note.laneIndex}: {timeUntilHit:F2}s hasta hit");
                }
            }
        }
    }
    
    void ShowInputInfo()
    {
        if (inputManager == null) return;
        
        KeyCode[] keys = { KeyCode.D, KeyCode.F, KeyCode.J, KeyCode.K, KeyCode.L };
        
        for (int i = 0; i < keys.Length; i++)
        {
            if (Input.GetKeyDown(keys[i]))
            {
                Debug.Log($"⌨️ TECLA PRESIONADA: {keys[i]} (Lane {i})");
                
                // Intentar hit
                if (gameplayManager != null)
                {
                    bool hitSuccess = gameplayManager.TryHitNote(i, out HitAccuracy accuracy);
                    Debug.Log($"   Resultado: {(hitSuccess ? "✅ HIT" : "❌ MISS")} - Precisión: {accuracy}");
                }
            }
        }
    }
    
    void HandleDebugControls()
    {
        // Presiona G para generar más notas de prueba
        if (Input.GetKeyDown(KeyCode.G))
        {
            GenerateTestNotes();
            Debug.Log("🔄 Nuevas notas de prueba generadas");
        }
        
        // Presiona H para mostrar información del sistema
        if (Input.GetKeyDown(KeyCode.H))
        {
            ShowSystemInfo();
        }
        
        // Presiona N para forzar spawn de una nota inmediata
        if (Input.GetKeyDown(KeyCode.N))
        {
            ForceSpawnTestNote();
        }
    }
    
    void ShowSystemInfo()
    {
        Debug.Log("🔧 INFORMACIÓN DEL SISTEMA:");
        
        if (gameplayManager != null)
        {
            Debug.Log($"   GameplayManager.isGameActive: {gameplayManager.isGameActive}");
            Debug.Log($"   GameplayManager.isPaused: {gameplayManager.isPaused}");
            Debug.Log($"   Tiempo actual: {gameplayManager.GetSongTime():F2}s");
            Debug.Log($"   Notas seleccionadas: {(gameplayManager.selectedNotes?.Count ?? 0)}");
        }
        
        if (noteSpawner != null)
        {
            Debug.Log($"   NoteSpawner encontrado: ✅");
            Debug.Log($"   NotePrefab asignado: {(noteSpawner.notePrefab != null ? "✅" : "❌")}");
            Debug.Log($"   Lanes configurados: {(noteSpawner.lanes?.Length ?? 0)}");
        }
        
        // Verificar si hay notas en la escena
        Note[] notesInScene = FindObjectsByType<Note>(FindObjectsSortMode.None);
        Debug.Log($"   Notas en escena: {notesInScene.Length}");
    }
    
    void ForceSpawnTestNote()
    {
        if (gameplayManager == null || noteSpawner == null) return;
        
        // Crear una nota que debería llegar al hit zone en 2 segundos
        float currentTime = gameplayManager.GetSongTime();
        float hitTime = currentTime + 2f;
        
        NoteData testNote = new NoteData(hitTime, 2); // Lane central
        testNote.noteType = NoteType.Normal;
        testNote.duration = 0f;
        
        // Agregar a las notas seleccionadas
        if (gameplayManager.selectedNotes == null)
        {
            gameplayManager.selectedNotes = new List<NoteData>();
        }
        
        gameplayManager.selectedNotes.Add(testNote);
        
        Debug.Log($"🎯 Nota de prueba forzada - Lane 2, llegará en 2 segundos (tiempo {hitTime:F2})");
    }
    
    void OnGUI()
    {
        if (!enableDebugMode) return;
        
        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.Label("🎸 HIT DETECTION DEBUGGER");
        GUILayout.Label($"Tiempo: {(gameplayManager?.GetSongTime() ?? 0):F2}s");
        GUILayout.Label($"Juego activo: {(gameplayManager?.isGameActive ?? false)}");
        
        if (gameplayManager?.selectedNotes != null)
        {
            GUILayout.Label($"Notas totales: {gameplayManager.selectedNotes.Count}");
        }
        
        Note[] notesInScene = FindObjectsByType<Note>(FindObjectsSortMode.None);
        GUILayout.Label($"Notas en escena: {notesInScene.Length}");
        
        GUILayout.Label("\nControles:");
        GUILayout.Label("G - Generar notas de prueba");
        GUILayout.Label("H - Mostrar info del sistema");
        GUILayout.Label("N - Forzar nota inmediata");
        GUILayout.Label("D,F,J,K,L - Tocar notas");
        
        GUILayout.EndArea();
    }
}
