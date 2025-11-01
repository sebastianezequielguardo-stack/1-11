using UnityEngine;

public class QuickChordTest : MonoBehaviour
{
    [Header("Quick Test")]
    public bool enableQuickTest = true;
    
    void Update()
    {
        if (!enableQuickTest) return;
        
        // Test básico - presiona ESPACIO para generar un acorde de prueba
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TestChordSystem();
        }
        
        // Mostrar estado del sistema
        if (Input.GetKeyDown(KeyCode.I))
        {
            ShowSystemInfo();
        }
    }
    
    void TestChordSystem()
    {
        Debug.Log("🧪 === QUICK CHORD TEST ===");
        
        // Buscar componentes
        var gameplayManager = GameplayManager.Instance;
        var inputManager = FindFirstObjectByType<InputManager>();
        var noteSpawner2D = FindFirstObjectByType<NoteSpawner2D>();
        
        if (gameplayManager == null)
        {
            Debug.LogError("❌ GameplayManager no encontrado!");
            return;
        }
        
        if (inputManager == null)
        {
            Debug.LogError("❌ InputManager no encontrado!");
            return;
        }
        
        if (noteSpawner2D == null)
        {
            Debug.LogError("❌ NoteSpawner2D no encontrado!");
            return;
        }
        
        // Verificar que el juego esté activo
        if (!gameplayManager.isGameActive)
        {
            Debug.LogWarning("⚠️ GameplayManager no está activo. Activando...");
            gameplayManager.StartTestGameplay();
        }
        
        // Generar acorde de prueba
        Debug.Log("🎵 Generando acorde de prueba (D + J)...");
        noteSpawner2D.SpawnChord(new int[] { 0, 2 }); // D + J
        
        Debug.Log("✅ Acorde generado! Presiona D + J simultáneamente para probarlo.");
        Debug.Log("🎮 Controles: D, F, J, K, L");
        Debug.Log("📊 Presiona I para ver info del sistema");
    }
    
    void ShowSystemInfo()
    {
        Debug.Log("📊 === SYSTEM INFO ===");
        
        var gameplayManager = GameplayManager.Instance;
        var inputManager = FindFirstObjectByType<InputManager>();
        var noteSpawner2D = FindFirstObjectByType<NoteSpawner2D>();
        
        if (gameplayManager != null)
        {
            Debug.Log($"🎮 GameplayManager - Activo: {gameplayManager.isGameActive}");
            Debug.Log($"📝 Notas activas: {gameplayManager.activeNotes?.Count ?? 0}");
            Debug.Log($"⏱️ Tiempo de canción: {gameplayManager.GetSongTime():F2}s");
        }
        
        if (inputManager != null)
        {
            Debug.Log($"⌨️ InputManager - Encontrado: ✅");
            Debug.Log($"⚙️ Chord settings: {(inputManager.chordSettings != null ? "✅" : "❌")}");
        }
        
        if (noteSpawner2D != null)
        {
            Debug.Log($"🎵 NoteSpawner2D - Acordes habilitados: {noteSpawner2D.enableChords}");
            Debug.Log($"🎲 Probabilidad de acordes: {noteSpawner2D.chordProbability * 100}%");
        }
        
        Debug.Log("📊 === END SYSTEM INFO ===");
    }
    
    void OnGUI()
    {
        if (!enableQuickTest) return;
        
        GUILayout.BeginArea(new Rect(10, Screen.height - 150, 400, 140));
        GUILayout.BeginVertical("box");
        
        GUILayout.Label("🧪 QUICK CHORD TEST", GUI.skin.box);
        
        if (GUILayout.Button("Test Chord System (SPACE)"))
            TestChordSystem();
            
        if (GUILayout.Button("Show System Info (I)"))
            ShowSystemInfo();
        
        GUILayout.Space(5);
        GUILayout.Label("Presiona D + J simultáneamente después del test", GUI.skin.box);
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
}
