using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// TEST DEL CONTROL CON EL SISTEMA DE JUEGO REAL
/// </summary>
public class ControllerGameplayTest : MonoBehaviour
{
    [Header("Configuración")]
    public bool activarTest = true;
    public bool spawnearNotasAutomaticamente = true;
    public float intervaloSpawn = 3f;
    
    private InputManager inputManager;
    private GameplayManager gameplayManager;
    private NoteSpawner2D noteSpawner;
    private float ultimoSpawn = 0f;
    
    void Start()
    {
        // Buscar componentes del juego
        inputManager = FindFirstObjectByType<InputManager>();
        gameplayManager = GameplayManager.Instance;
        noteSpawner = FindFirstObjectByType<NoteSpawner2D>();
        
        // Controller test iniciado (logs removidos para limpiar consola)
        
        // Activar el juego si no está activo
        if (gameplayManager != null && !gameplayManager.isGameActive)
        {
            gameplayManager.StartTestGameplay();
        }
        
        // Configurar InputManager para usar control
        if (inputManager != null)
        {
            inputManager.useController = true;
        }
    }
    
    void Update()
    {
        if (!activarTest) return;
        
        // Spawn automático de notas de prueba
        if (spawnearNotasAutomaticamente && Time.time - ultimoSpawn > intervaloSpawn)
        {
            SpawnearNotasDePrueba();
            ultimoSpawn = Time.time;
        }
        
        // Mostrar estado del input en tiempo real
        MostrarEstadoInput();
    }
    
    void SpawnearNotasDePrueba()
    {
        if (noteSpawner == null) return;
        
        int patron = Random.Range(0, 4);
        
        switch (patron)
        {
            case 0: // Acorde L1 + X + Circle (0 + 2 + 3)
                noteSpawner.SpawnChord(new int[] { 0, 2, 3 });
                // Acorde spawneado
                break;
                
            case 1: // Acorde Square + Circle + R1 (1 + 3 + 4)
                noteSpawner.SpawnChord(new int[] { 1, 3, 4 });
                // Acorde spawneado
                break;
                
            case 2: // Acorde de 4 botones
                noteSpawner.SpawnChord(new int[] { 0, 1, 2, 3 });
                // Acorde de 4 botones spawneado
                break;
                
            case 3: // Nota individual
                int lane = Random.Range(0, 5);
                noteSpawner.SpawnNote(lane);
                // Nota individual spawneada
                break;
        }
    }
    
    void MostrarEstadoInput()
    {
        if (inputManager == null) return;
        
        // Verificar si alguna tecla está presionada
        List<int> teclasPresionadas = new List<int>();
        for (int i = 0; i < 5; i++)
        {
            if (inputManager.IsKeyHeld(i))
            {
                teclasPresionadas.Add(i);
            }
        }
        
        // Si hay múltiples teclas presionadas, mostrar acorde
        if (teclasPresionadas.Count >= 2)
        {
            string acordeTexto = string.Join(" + ", teclasPresionadas);
            // Acorde detectado con control
        }
    }
    
    void OnGUI()
    {
        if (!activarTest) return;
        
        GUILayout.BeginArea(new Rect(10, 10, 450, 400));
        GUILayout.BeginVertical("box");
        
        GUILayout.Label("🎮 CONTROLLER GAMEPLAY TEST", GUI.skin.box);
        
        // Estado de componentes
        GUILayout.Label("COMPONENTES:");
        GUILayout.Label($"InputManager: {(inputManager != null ? "✅" : "❌")}");
        GUILayout.Label($"GameplayManager: {(gameplayManager != null ? "✅" : "❌")}");
        GUILayout.Label($"NoteSpawner2D: {(noteSpawner != null ? "✅" : "❌")}");
        
        if (gameplayManager != null)
        {
            GUILayout.Label($"Juego activo: {(gameplayManager.isGameActive ? "✅" : "❌")}");
            GUILayout.Label($"Notas activas: {gameplayManager.activeNotes?.Count ?? 0}");
        }
        
        GUILayout.Space(10);
        
        // Estado del control
        GUILayout.Label("🎮 ESTADO DEL CONTROL:", GUI.skin.box);
        
        string[] buttonNames = { "L1 (Lane 0 - Verde)", "Square (Lane 1 - Rojo)", "X (Lane 2 - Azul)", "Circle (Lane 3 - Amarillo)", "R1 (Lane 4 - Rosa)" };
        KeyCode[] buttons = { 
            KeyCode.Joystick1Button4, // L1
            KeyCode.Joystick1Button2, // Square
            KeyCode.Joystick1Button0, // X
            KeyCode.Joystick1Button1, // Circle
            KeyCode.Joystick1Button5  // R1
        };
        
        for (int i = 0; i < buttons.Length; i++)
        {
            bool pressed = Input.GetKey(buttons[i]);
            GUI.color = pressed ? Color.green : Color.white;
            GUILayout.Label($"{buttonNames[i]}: {(pressed ? "PRESIONADO" : "libre")}");
        }
        
        GUI.color = Color.white;
        GUILayout.Space(10);
        
        // Controles
        activarTest = GUILayout.Toggle(activarTest, "Activar Test");
        spawnearNotasAutomaticamente = GUILayout.Toggle(spawnearNotasAutomaticamente, "Auto Spawn Notas");
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Spawn Acorde L1+X+Circle"))
        {
            if (noteSpawner != null)
            {
                noteSpawner.SpawnChord(new int[] { 0, 2, 3 });
                // Manual spawn ejecutado
            }
        }
        
        if (GUILayout.Button("Spawn Acorde Square+Circle+R1"))
        {
            if (noteSpawner != null)
            {
                noteSpawner.SpawnChord(new int[] { 1, 3, 4 });
                // Manual spawn ejecutado
            }
        }
        
        GUILayout.Space(10);
        GUILayout.Label("INSTRUCCIONES:", GUI.skin.box);
        GUILayout.Label("1. Espera a que aparezcan notas");
        GUILayout.Label("2. Presiona los botones correspondientes");
        GUILayout.Label("3. Para acordes, mantén múltiples botones");
        GUILayout.Label("4. Options = Pausa, Share = Reiniciar");
        GUILayout.Label("5. Mira los logs en consola");
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
}
