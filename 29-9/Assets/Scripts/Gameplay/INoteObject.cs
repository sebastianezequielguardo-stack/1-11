using UnityEngine;

/// <summary>
/// Interfaz común para objetos de nota que pueden ser manejados por GameplayManager
/// </summary>
public interface INoteObject
{
    void Hit();
    void Miss();
    NoteData GetNoteData();
    GameObject GetGameObject();
    
    // Evento para notificar destrucción
    System.Action<INoteObject> OnDestroyed { get; set; }
}

/// <summary>
/// Adaptador para FallingNote2D para que implemente INoteObject
/// </summary>
public class FallingNote2DAdapter : INoteObject
{
    private FallingNote2D fallingNote;
    
    public System.Action<INoteObject> OnDestroyed { get; set; }
    
    public FallingNote2DAdapter(FallingNote2D note)
    {
        fallingNote = note;
        
        // Suscribirse al evento de destrucción de la nota original
        fallingNote.OnNoteDestroyed += OnFallingNoteDestroyed;
    }
    
    private void OnFallingNoteDestroyed(FallingNote2D note)
    {
        OnDestroyed?.Invoke(this);
    }
    
    public void Hit()
    {
        fallingNote.Hit();
    }
    
    public void Miss()
    {
        fallingNote.Miss();
    }
    
    public NoteData GetNoteData()
    {
        return fallingNote.noteData;
    }
    
    public GameObject GetGameObject()
    {
        return fallingNote.gameObject;
    }
    
    public void Cleanup()
    {
        if (fallingNote != null)
        {
            fallingNote.OnNoteDestroyed -= OnFallingNoteDestroyed;
        }
    }
}

/// <summary>
/// Adaptador para Note (sistema 3D) para que implemente INoteObject
/// </summary>
public class NoteAdapter : INoteObject
{
    private Note note;
    
    public System.Action<INoteObject> OnDestroyed { get; set; }
    
    public NoteAdapter(Note note)
    {
        this.note = note;
        
        // Suscribirse al evento de destrucción de la nota original
        note.OnNoteDestroyed += OnNoteDestroyed;
    }
    
    private void OnNoteDestroyed(Note note)
    {
        OnDestroyed?.Invoke(this);
    }
    
    public void Hit()
    {
        note.Hit();
    }
    
    public void Miss()
    {
        note.Miss();
    }
    
    public NoteData GetNoteData()
    {
        return note.noteData;
    }
    
    public GameObject GetGameObject()
    {
        return note.gameObject;
    }
    
    public void Cleanup()
    {
        if (note != null)
        {
            note.OnNoteDestroyed -= OnNoteDestroyed;
        }
    }
}
