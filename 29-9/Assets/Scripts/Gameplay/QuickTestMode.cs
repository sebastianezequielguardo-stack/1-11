using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Script para activar rápidamente un modo de prueba con notas
/// Agrega este script a cualquier GameObject para probar inmediatamente
/// </summary>
public class QuickTestMode : MonoBehaviour
{
    [Header("Quick Test Settings")]
    public bool autoStartTest = true;
    public float firstNoteDelay = 2f;
    public float notesPerSecond = 0.5f; // Una nota cada 2 segundos
    public int totalNotes = 30;
    
    [Header("Debug Info")]
    public bool showDebugGUI = false; // Desactivado por defecto para evitar letras en pantalla
    
    private GameplayManager gameplayManager;
    private bool testStarted = false;
    
    void Start()
    {
        if (autoStartTest)
        {
            StartCoroutine(InitializeQuickTest());
        }
    }
    
    System.Collections.IEnumerator InitializeQuickTest()
    {
        Debug.Log("🚀 INICIANDO MODO DE PRUEBA RÁPIDO...");
        
        // Esperar a que se inicialicen los componentes
        yield return new WaitForSeconds(0.5f);
        
        // Encontrar GameplayManager
        gameplayManager = GameplayManager.Instance;
        if (gameplayManager == null)
        {
            Debug.LogError("❌ No se encontró GameplayManager");
            yield break;
        }
        
        // Generar notas de prueba
        GenerateQuickTestNotes();
        
        // Activar el juego
        gameplayManager.isGameActive = true;
        testStarted = true;
        
        Debug.Log("✅ MODO DE PRUEBA ACTIVADO");
        Debug.Log("🎮 Usa las teclas D, F, J, K, L para tocar las notas");
        Debug.Log("🎵 Las notas empezarán a aparecer en unos segundos");
    }
    
    void GenerateQuickTestNotes()
    {
        List<NoteData> testNotes = new List<NoteData>();
        
        // Generar patrón de notas simple
        for (int i = 0; i < totalNotes; i++)
        {
            float time = firstNoteDelay + (i / notesPerSecond);
            int lane = i % 5; // Rotar entre todos los lanes
            
            // Crear patrón más interesante
            if (i % 10 < 5)
            {
                // Primeras 5 notas: secuencial
                lane = i % 5;
            }
            else
            {
                // Siguientes 5 notas: patrón específico
                int[] pattern = { 2, 0, 4, 1, 3 }; // Centro, izquierda, derecha, etc.
                lane = pattern[i % 5];
            }
            
            NoteData note = new NoteData(time, lane);
            note.noteType = NoteType.Normal;
            note.duration = 0f;
            
            testNotes.Add(note);
        }
        
        // Asignar al GameplayManager
        gameplayManager.selectedNotes = testNotes;
        
        Debug.Log($"🎵 Generadas {testNotes.Count} notas de prueba");
        Debug.Log($"   Primera nota: Lane {testNotes[0].laneIndex} a los {testNotes[0].time:F1}s");
        Debug.Log($"   Patrón: Rotación por todos los lanes");
    }
    
    void Update()
    {
        // Controles adicionales
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!testStarted)
            {
                StartCoroutine(InitializeQuickTest());
            }
            else
            {
                RestartTest();
            }
        }
        
        // Información en tiempo real
        if (Input.GetKeyDown(KeyCode.I))
        {
            ShowGameInfo();
        }
    }
    
    void RestartTest()
    {
        Debug.Log("🔄 REINICIANDO PRUEBA...");
        
        // Limpiar notas existentes
        Note[] existingNotes = FindObjectsByType<Note>(FindObjectsSortMode.None);
        foreach (Note note in existingNotes)
        {
            if (note != null)
            {
                Destroy(note.gameObject);
            }
        }
        
        // Regenerar notas
        GenerateQuickTestNotes();
        
        Debug.Log("✅ Prueba reiniciada");
    }
    
    void ShowGameInfo()
    {
        if (gameplayManager == null) return;
        
        Debug.Log("📊 INFORMACIÓN DEL JUEGO:");
        Debug.Log($"   Tiempo actual: {gameplayManager.GetSongTime():F2}s");
        Debug.Log($"   Juego activo: {gameplayManager.isGameActive}");
        Debug.Log($"   Juego pausado: {gameplayManager.isPaused}");
        Debug.Log($"   Notas totales: {gameplayManager.selectedNotes?.Count ?? 0}");
        
        // Contar notas en escena
        Note[] notesInScene = FindObjectsByType<Note>(FindObjectsSortMode.None);
        Debug.Log($"   Notas visibles: {notesInScene.Length}");
        
        // Verificar componentes
        InputManager inputManager = FindFirstObjectByType<InputManager>();
        NoteSpawner spawner = FindFirstObjectByType<NoteSpawner>();
        
        Debug.Log($"   InputManager: {(inputManager != null ? "✅" : "❌")}");
        Debug.Log($"   NoteSpawner: {(spawner != null ? "✅" : "❌")}");
        
        if (spawner != null)
        {
            Debug.Log($"   NotePrefab: {(spawner.notePrefab != null ? "✅" : "❌")}");
            Debug.Log($"   Lanes configurados: {spawner.lanes?.Length ?? 0}");
        }
    }
    
    void OnGUI()
    {
        if (!showDebugGUI) return;
        
        // Panel de información
        GUILayout.BeginArea(new Rect(Screen.width - 250, 10, 240, 300));
        
        GUILayout.Label("🎸 QUICK TEST MODE", GUI.skin.box);
        
        if (gameplayManager != null)
        {
            GUILayout.Label($"Tiempo: {gameplayManager.GetSongTime():F1}s");
            GUILayout.Label($"Activo: {gameplayManager.isGameActive}");
            GUILayout.Label($"Notas: {gameplayManager.selectedNotes?.Count ?? 0}");
        }
        
        // Contar notas visibles
        Note[] visibleNotes = FindObjectsByType<Note>(FindObjectsSortMode.None);
        GUILayout.Label($"Visibles: {visibleNotes.Length}");
        
        GUILayout.Space(10);
        GUILayout.Label("Controles:", GUI.skin.box);
        GUILayout.Label("D F J K L - Tocar");
        GUILayout.Label("SPACE - Reiniciar");
        GUILayout.Label("I - Info detallada");
        
        GUILayout.Space(10);
        
        // Mostrar estado de las teclas
        GUILayout.Label("Estado teclas:", GUI.skin.box);
        KeyCode[] keys = { KeyCode.D, KeyCode.F, KeyCode.J, KeyCode.K, KeyCode.L };
        string[] keyNames = { "D", "F", "J", "K", "L" };
        
        for (int i = 0; i < keys.Length; i++)
        {
            bool pressed = Input.GetKey(keys[i]);
            GUILayout.Label($"{keyNames[i]}: {(pressed ? "🔴" : "⚪")}");
        }
        
        GUILayout.EndArea();
    }
}
