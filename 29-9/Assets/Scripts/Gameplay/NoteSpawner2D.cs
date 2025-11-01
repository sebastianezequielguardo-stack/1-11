using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class NoteSpawner2D : MonoBehaviour
{
    [Header("Spawning Settings")]
    public GameObject notePrefab; // Prefab con FallingNote2D
    public Transform[] lanePositions; // Posiciones X de cada carril
    public float spawnY = 400f; // Altura donde aparecen las notas
    public float noteSpeed = 5f;
    
    [Header("Note Sprites")]
    public Sprite[] noteSprites = new Sprite[5]; // Sprites para cada carril
    
    [Header("Gameplay")]
    public bool autoSpawn = false;
    public float spawnInterval = 1f;
    
    [Header("Note Spacing")]
    [Range(0.1f, 5f)]
    public float noteSpacing = 1f; // Multiplicador de espaciado temporal entre notas
    public bool useCustomSpacing = false;
    public float customSpacingSeconds = 0.5f; // Espaciado personalizado en segundos
    
    [Header("Chord Spacing (Horizontal)")]
    public bool enableChords = true; // Permitir múltiples notas simultáneas
    [Range(0f, 100f)]
    public float horizontalSpacing = 15f; // Espaciado horizontal adicional entre notas del mismo acorde
    public int maxNotesPerChord = 5; // Máximo de notas simultáneas
    [Range(0f, 1f)]
    public float chordProbability = 0.4f; // Probabilidad de generar acordes (40%)
    
    // Pool de notas para optimización
    private Queue<FallingNote2D> notePool = new Queue<FallingNote2D>();
    private List<FallingNote2D> activeNotes = new List<FallingNote2D>();
    
    // Referencias
    private Canvas worldCanvas;
    private GameManager gameManager;
    private GameplayManager gameplayManager;
    
    // Eventos para integración con GameplayManager
    public System.Action<NoteData, FallingNote2D> OnNoteSpawned;
    
    // Estadísticas
    private int notesHit = 0;
    private int notesMissed = 0;
    private float totalAccuracy = 0f;
    
    void Start()
    {
        worldCanvas = GetComponentInParent<Canvas>();
        gameManager = FindFirstObjectByType<GameManager>();
        gameplayManager = GameplayManager.Instance;
        
        // Cargar sprites desde Resources si no están asignados
        LoadNoteSprites();
        
        // Inicializar pool de notas
        InitializeNotePool(10);
        
        // Auto spawn para testing
        if (autoSpawn)
        {
            float actualInterval = GetActualSpawnInterval();
            InvokeRepeating(nameof(SpawnRandomNote), 1f, actualInterval);
        }
    }
    
    void LoadNoteSprites()
    {
        string[] spriteNames = {
            "nota verde",
            "nota roja", 
            "nota amarilla",
            "nota azul",
            "nota rosa"
        };
        
        for (int i = 0; i < spriteNames.Length && i < noteSprites.Length; i++)
        {
            if (noteSprites[i] == null)
            {
                noteSprites[i] = Resources.Load<Sprite>(spriteNames[i]);
                if (noteSprites[i] == null)
                {
                    // No se pudo cargar sprite
                }
            }
        }
    }
    
    void InitializeNotePool(int poolSize)
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject noteObj = Instantiate(notePrefab, transform);
            FallingNote2D note = noteObj.GetComponent<FallingNote2D>();
            noteObj.SetActive(false);
            notePool.Enqueue(note);
        }
    }
    
    public void SpawnNote(int laneIndex)
    {
        if (laneIndex < 0 || laneIndex >= lanePositions.Length) return;
        
        FallingNote2D note = GetNoteFromPool();
        if (note == null) return;
        
        // Configurar posición
        RectTransform noteRect = note.GetComponent<RectTransform>();
        Vector2 spawnPos = new Vector2(GetLaneXPosition(laneIndex), spawnY);
        noteRect.anchoredPosition = spawnPos;
        
        // Configurar la nota
        Sprite noteSprite = (laneIndex < noteSprites.Length) ? noteSprites[laneIndex] : null;
        note.SetupNote(laneIndex, noteSprite, noteSpeed);
        
        // Activar la nota
        note.gameObject.SetActive(true);
        activeNotes.Add(note);
        
        // Crear NoteData para integración con GameplayManager
        if (gameplayManager != null)
        {
            float noteTime = Time.time + (spawnY / (noteSpeed * 100f));
            NoteData noteData = new NoteData(noteTime, laneIndex);
            OnNoteSpawned?.Invoke(noteData, note);
        }
    }
    
    public void SpawnRandomNote()
    {
        if (enableChords && Random.Range(0f, 1f) < chordProbability) // Chance de acorde configurable
        {
            SpawnRandomChord();
        }
        else
        {
            int randomLane = Random.Range(0, lanePositions.Length);
            SpawnNote(randomLane);
        }
    }
    
    public void SpawnRandomChord()
    {
        int chordSize = Random.Range(2, Mathf.Min(maxNotesPerChord + 1, lanePositions.Length + 1));
        int[] lanes = new int[chordSize];
        
        // Seleccionar carriles únicos para el acorde
        for (int i = 0; i < chordSize; i++)
        {
            int lane;
            do
            {
                lane = Random.Range(0, lanePositions.Length);
            } while (System.Array.IndexOf(lanes, lane, 0, i) >= 0);
            
            lanes[i] = lane;
        }
        
        SpawnChord(lanes);
    }
    
    public void SpawnChord(int[] laneIndices)
    {
        for (int i = 0; i < laneIndices.Length; i++)
        {
            SpawnNoteWithOffset(laneIndices[i], i * horizontalSpacing);
        }
    }
    
    void SpawnNoteWithOffset(int laneIndex, float horizontalOffset)
    {
        if (laneIndex < 0 || laneIndex >= lanePositions.Length) return;
        
        FallingNote2D note = GetNoteFromPool();
        if (note == null) return;
        
        // Configurar posición con offset horizontal
        RectTransform noteRect = note.GetComponent<RectTransform>();
        float baseX = GetLaneXPosition(laneIndex);
        Vector2 spawnPos = new Vector2(baseX + horizontalOffset, spawnY);
        noteRect.anchoredPosition = spawnPos;
        
        // Configurar la nota
        Sprite noteSprite = (laneIndex < noteSprites.Length) ? noteSprites[laneIndex] : null;
        note.SetupNote(laneIndex, noteSprite, noteSpeed);
        
        // Activar la nota
        note.gameObject.SetActive(true);
        activeNotes.Add(note);
        
        // Crear NoteData para integración con GameplayManager (acordes)
        if (gameplayManager != null)
        {
            float noteTime = Time.time + (spawnY / (noteSpeed * 100f));
            NoteData noteData = new NoteData(noteTime, laneIndex);
            noteData.isChord = true;
            noteData.chordId = System.DateTime.Now.Millisecond; // ID único para el acorde
            OnNoteSpawned?.Invoke(noteData, note);
        }
        
        // Nota spawneada
    }
    
    float GetLaneXPosition(int laneIndex)
    {
        if (laneIndex >= 0 && laneIndex < lanePositions.Length && lanePositions[laneIndex] != null)
        {
            // Convertir posición 3D a posición UI
            Vector3 worldPos = lanePositions[laneIndex].position;
            Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPos);
            
            // Convertir a coordenadas del canvas
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                worldCanvas.GetComponent<RectTransform>(),
                screenPos,
                worldCanvas.worldCamera,
                out Vector2 localPos
            );
            
            // Posición calculada
            return localPos.x;
        }
        
        // Posiciones de respaldo mejoradas basadas en los botones UI
        ButtonSpriteController buttonController = FindFirstObjectByType<ButtonSpriteController>();
        if (buttonController != null && laneIndex < buttonController.buttons.Length)
        {
            var button = buttonController.buttons[laneIndex];
            if (button.imageComponent != null)
            {
                RectTransform buttonRect = button.imageComponent.GetComponent<RectTransform>();
                return buttonRect.anchoredPosition.x;
            }
        }
        
        // Posiciones de respaldo finales (ajustadas para mejor alineación)
        float[] fallbackPositions = { -240f, -120f, 0f, 120f, 240f };
        return (laneIndex < fallbackPositions.Length) ? fallbackPositions[laneIndex] : 0f;
    }
    
    FallingNote2D GetNoteFromPool()
    {
        if (notePool.Count > 0)
        {
            return notePool.Dequeue();
        }
        
        // Crear nueva nota si el pool está vacío
        GameObject noteObj = Instantiate(notePrefab, transform);
        return noteObj.GetComponent<FallingNote2D>();
    }
    
    public void ReturnNoteToPool(FallingNote2D note)
    {
        if (note == null) return;
        
        activeNotes.Remove(note);
        note.gameObject.SetActive(false);
        notePool.Enqueue(note);
    }
    
    // Callbacks del sistema de notas
    public void OnNoteHit(int laneIndex, float accuracy, float noteValue)
    {
        notesHit++;
        totalAccuracy += accuracy;
        
        // Calcular puntuación
        int score = Mathf.RoundToInt(noteValue * accuracy * 100f);
        
        // Notificar al GameManager si existe
        if (gameManager != null)
        {
            // gameManager.AddScore(score);
        }
        
        // Nota golpeada
    }
    
    public void OnNoteMissed(int laneIndex)
    {
        notesMissed++;
        // Nota perdida
        
        // Notificar al GameManager si existe
        if (gameManager != null)
        {
            // gameManager.OnNoteMissed();
        }
    }
    
    // Métodos públicos para control externo
    public void SetNoteSpeed(float speed)
    {
        noteSpeed = speed;
    }
    
    public void ClearAllNotes()
    {
        foreach (var note in activeNotes.ToArray())
        {
            ReturnNoteToPool(note);
        }
    }
    
    public float GetAccuracy()
    {
        if (notesHit == 0) return 0f;
        return totalAccuracy / notesHit;
    }
    
    public int GetNotesHit() => notesHit;
    public int GetNotesMissed() => notesMissed;
    
    // Métodos de control de espaciado
    float GetActualSpawnInterval()
    {
        if (useCustomSpacing)
        {
            return customSpacingSeconds;
        }
        return spawnInterval * noteSpacing;
    }
    
    public void SetNoteSpacing(float spacing)
    {
        noteSpacing = Mathf.Clamp(spacing, 0.1f, 5f);
        UpdateSpawnInterval();
    }
    
    public void SetCustomSpacing(float seconds)
    {
        customSpacingSeconds = Mathf.Max(0.1f, seconds);
        if (useCustomSpacing)
        {
            UpdateSpawnInterval();
        }
    }
    
    void UpdateSpawnInterval()
    {
        if (autoSpawn)
        {
            CancelInvoke(nameof(SpawnRandomNote));
            float actualInterval = GetActualSpawnInterval();
            InvokeRepeating(nameof(SpawnRandomNote), 0f, actualInterval);
            // Espaciado actualizado
        }
    }
    
    // Métodos de acceso rápido
    [ContextMenu("Spacing: Very Fast")]
    public void SetVeryFastSpacing()
    {
        SetNoteSpacing(0.2f);
    }
    
    [ContextMenu("Spacing: Fast")]
    public void SetFastSpacing()
    {
        SetNoteSpacing(0.5f);
    }
    
    [ContextMenu("Spacing: Normal")]
    public void SetNormalSpacing()
    {
        SetNoteSpacing(1f);
    }
    
    [ContextMenu("Spacing: Slow")]
    public void SetSlowSpacing()
    {
        SetNoteSpacing(2f);
    }
    
    [ContextMenu("Spacing: Very Slow")]
    public void SetVerySlowSpacing()
    {
        SetNoteSpacing(3f);
    }
    
    // Para testing en el editor
    void Update()
    {
        if (Application.isEditor)
        {
            // Spawn manual con teclas numéricas
            for (int i = 0; i < 5; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    SpawnNote(i);
                }
            }
            
            // Controles de espaciado temporal con teclas
            if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                SetNoteSpacing(noteSpacing + 0.2f); // Más lento
                // Espaciado aumentado
            }
            if (Input.GetKeyDown(KeyCode.Equals) || Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                SetNoteSpacing(noteSpacing - 0.2f); // Más rápido
                // Espaciado reducido
            }
            
            // Controles de espaciado horizontal
            if (Input.GetKeyDown(KeyCode.LeftBracket))
            {
                horizontalSpacing = Mathf.Max(0f, horizontalSpacing - 10f);
                // Espaciado horizontal reducido
            }
            if (Input.GetKeyDown(KeyCode.RightBracket))
            {
                horizontalSpacing = Mathf.Min(100f, horizontalSpacing + 10f);
                // Espaciado horizontal aumentado
            }
            
            // Testing de acordes
            if (Input.GetKeyDown(KeyCode.C))
            {
                SpawnRandomChord();
                // Acorde spawneado
            }
            
            // Spawn acordes específicos
            if (Input.GetKeyDown(KeyCode.Q))
            {
                SpawnChord(new int[] { 0, 2, 4 }); // Verde, Amarillo, Rosa
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                SpawnChord(new int[] { 1, 3 }); // Rojo, Azul
            }
            
            // Reset espaciado
            if (Input.GetKeyDown(KeyCode.R) && Input.GetKey(KeyCode.LeftControl))
            {
                SetNormalSpacing();
                horizontalSpacing = 0f;
                // Espaciados reseteados
            }
        }
    }
}
