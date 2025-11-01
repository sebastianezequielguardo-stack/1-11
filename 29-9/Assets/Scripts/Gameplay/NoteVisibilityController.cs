using UnityEngine;
using System.Collections;

/// <summary>
/// Controla la visibilidad de las notas 3D originales
/// </summary>
public class NoteVisibilityController : MonoBehaviour
{
    [Header("Settings")]
    public bool hideOriginalNotes = true;
    public float checkInterval = 0.1f;
    
    void Start()
    {
        if (hideOriginalNotes)
        {
            StartCoroutine(HideOriginalNotes());
        }
    }
    
    System.Collections.IEnumerator HideOriginalNotes()
    {
        while (true)
        {
            // Buscar todas las notas activas
            Note[] allNotes = FindObjectsByType<Note>(FindObjectsSortMode.None);
            
            foreach (Note note in allNotes)
            {
                // Ocultar el SpriteRenderer de la nota original
                SpriteRenderer spriteRenderer = note.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null && spriteRenderer.enabled)
                {
                    spriteRenderer.enabled = false;
                    Debug.Log($"Nota 3D ocultada en carril {note.lane}");
                }
            }
            
            yield return new WaitForSeconds(checkInterval);
        }
    }
    
    // Método para mostrar/ocultar notas originales
    public void SetOriginalNotesVisible(bool visible)
    {
        hideOriginalNotes = !visible;
        
        Note[] allNotes = FindObjectsByType<Note>(FindObjectsSortMode.None);
        foreach (Note note in allNotes)
        {
            SpriteRenderer spriteRenderer = note.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = visible;
            }
        }
    }
}
