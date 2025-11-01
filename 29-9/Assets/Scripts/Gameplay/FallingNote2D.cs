using UnityEngine;
using UnityEngine.UI;

public class FallingNote2D : MonoBehaviour
{
    [Header("Note Settings")]
    public int laneIndex = 0;
    public float fallSpeed = 5f;
    public float noteValue = 1f; // Para diferentes tipos de notas
    
    [Header("Visual")]
    public Image noteImage;
    public Sprite noteSprite;
    
    [Header("Hit Detection")]
    public float hitZoneY = -3f; // Posición Y donde se puede golpear la nota
    public float hitTolerance = 0.5f; // Tolerancia para el hit
    
    private RectTransform rectTransform;
    private bool hasBeenHit = false;
    private bool hasPassed = false;
    
    // Referencias al sistema de juego
    private NoteSpawner2D spawner;
    
    // Compatibilidad con GameplayManager
    public System.Action<FallingNote2D> OnNoteDestroyed;
    public NoteData noteData; // Para compatibilidad con el sistema 3D
    
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        spawner = FindFirstObjectByType<NoteSpawner2D>();
        
        // Configurar el sprite si está asignado
        if (noteImage != null && noteSprite != null)
        {
            noteImage.sprite = noteSprite;
        }
    }
    
    void Update()
    {
        // Mover la nota hacia abajo
        Vector3 currentPos = rectTransform.anchoredPosition;
        currentPos.y -= fallSpeed * Time.deltaTime * 100f; // Multiplicar por 100 para UI
        rectTransform.anchoredPosition = currentPos;
        
        // Verificar si la nota ha pasado la zona de hit
        if (!hasPassed && currentPos.y < hitZoneY - hitTolerance)
        {
            hasPassed = true;
            OnNoteMissed();
        }
        
        // Destruir la nota si está muy abajo
        if (currentPos.y < -600f)
        {
            DestroyNote();
        }
        
        // Input detection is now handled by InputManager to support chords
        // CheckForInput();
    }
    
    void CheckForInput()
    {
        if (hasBeenHit || hasPassed) return;
        
        // Verificar si estamos en la zona de hit
        float currentY = rectTransform.anchoredPosition.y;
        if (Mathf.Abs(currentY - hitZoneY) <= hitTolerance * 100f)
        {
            KeyCode[] keys = { KeyCode.D, KeyCode.F, KeyCode.J, KeyCode.K, KeyCode.L };
            
            if (laneIndex >= 0 && laneIndex < keys.Length)
            {
                if (Input.GetKeyDown(keys[laneIndex]))
                {
                    OnNoteHit();
                }
            }
        }
    }
    
    void OnNoteHit()
    {
        if (hasBeenHit) return;
        
        hasBeenHit = true;
        
        // Calcular precisión del hit
        float currentY = rectTransform.anchoredPosition.y;
        float accuracy = 1f - (Mathf.Abs(currentY - hitZoneY) / (hitTolerance * 100f));
        
        // Notificar al spawner o game manager
        if (spawner != null)
        {
            spawner.OnNoteHit(laneIndex, accuracy, noteValue);
        }
        
        // Efecto visual de hit (opcional)
        ShowHitEffect();
        
        // Destruir la nota
        DestroyNote();
    }
    
    void OnNoteMissed()
    {
        // Notificar al spawner o game manager
        if (spawner != null)
        {
            spawner.OnNoteMissed(laneIndex);
        }
    }
    
    void ShowHitEffect()
    {
        // Aquí puedes añadir efectos visuales como:
        // - Cambio de color
        // - Animación de escala
        // - Partículas
        
        if (noteImage != null)
        {
            noteImage.color = Color.yellow; // Efecto simple
        }
    }
    
    void DestroyNote()
    {
        // Notificar destrucción para compatibilidad con GameplayManager
        OnNoteDestroyed?.Invoke(this);
        
        if (spawner != null)
        {
            spawner.ReturnNoteToPool(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // Métodos para compatibilidad con GameplayManager
    public void Hit()
    {
        OnNoteHit();
    }
    
    public void Miss()
    {
        OnNoteMissed();
    }
    
    // Método para configurar la nota desde el spawner
    public void SetupNote(int lane, Sprite sprite, float speed = 5f)
    {
        laneIndex = lane;
        fallSpeed = speed;
        
        if (noteImage != null && sprite != null)
        {
            noteImage.sprite = sprite;
            noteSprite = sprite;
        }
        
        hasBeenHit = false;
        hasPassed = false;
        
        // Crear NoteData si no existe
        if (noteData == null)
        {
            noteData = new NoteData(Time.time, lane);
        }
    }
}
