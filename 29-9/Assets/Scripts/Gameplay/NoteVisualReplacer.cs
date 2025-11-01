using UnityEngine;

/// <summary>
/// Reemplaza las notas 3D con sprites 2D personalizados
/// </summary>
public class NoteVisualReplacer : MonoBehaviour
{
    [Header("Note Sprites")]
    public Sprite[] noteSprites = new Sprite[5]; // Sprites para cada carril
    
    [Header("Settings")]
    public bool replaceOnStart = true;
    public float spriteScale = 1f;
    
    // Nombres de los sprites en Resources
    private string[] spriteNames = {
        "nota verde",
        "nota roja", 
        "nota amarilla",
        "nota azul",
        "nota rosa"
    };
    
    void Start()
    {
        if (replaceOnStart)
        {
            LoadSprites();
            ReplaceExistingNotes();
        }
    }
    
    void LoadSprites()
    {
        for (int i = 0; i < spriteNames.Length && i < noteSprites.Length; i++)
        {
            if (noteSprites[i] == null)
            {
                noteSprites[i] = Resources.Load<Sprite>(spriteNames[i]);
                if (noteSprites[i] == null)
                {
                    Debug.LogWarning($"No se pudo cargar el sprite: {spriteNames[i]}");
                }
            }
        }
    }
    
    void ReplaceExistingNotes()
    {
        // Buscar todas las notas existentes en la escena
        Note[] existingNotes = FindObjectsByType<Note>(FindObjectsSortMode.None);
        
        foreach (Note note in existingNotes)
        {
            ReplaceNoteVisual(note);
        }
    }
    
    public void ReplaceNoteVisual(Note note)
    {
        if (note == null) return;
        
        SpriteRenderer spriteRenderer = note.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) return;
        
        // Obtener el carril de la nota
        int lane = note.lane;
        
        // Asignar el sprite correspondiente
        if (lane >= 0 && lane < noteSprites.Length && noteSprites[lane] != null)
        {
            spriteRenderer.sprite = noteSprites[lane];
            
            // Ajustar escala si es necesario
            if (spriteScale != 1f)
            {
                note.transform.localScale = Vector3.one * spriteScale;
            }
            
            // Asegurar que el sprite sea visible
            spriteRenderer.sortingLayerName = "Default";
            spriteRenderer.sortingOrder = 10;
            
            // Mantener el color blanco para que se vea el sprite original
            spriteRenderer.color = Color.white;
            
            Debug.Log($"Sprite reemplazado para nota en carril {lane}");
        }
        else
        {
            Debug.LogWarning($"No hay sprite disponible para el carril {lane}");
        }
    }
    
    // Método para ser llamado cuando se spawnen nuevas notas
    void OnEnable()
    {
        // Suscribirse al evento de spawn de notas si existe
        NoteSpawner spawner = FindFirstObjectByType<NoteSpawner>();
        if (spawner != null)
        {
            spawner.OnNoteSpawned += OnNoteSpawned;
        }
    }
    
    void OnDisable()
    {
        // Desuscribirse del evento
        NoteSpawner spawner = FindFirstObjectByType<NoteSpawner>();
        if (spawner != null)
        {
            spawner.OnNoteSpawned -= OnNoteSpawned;
        }
    }
    
    void OnNoteSpawned(NoteData noteData, Note note)
    {
        // Reemplazar el visual de la nota recién spawneada
        ReplaceNoteVisual(note);
    }
}
