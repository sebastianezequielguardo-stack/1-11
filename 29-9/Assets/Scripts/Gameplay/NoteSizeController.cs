using UnityEngine;
using System.Collections;

/// <summary>
/// Controla el tamaño de las notas 3D originales
/// </summary>
public class NoteSizeController : MonoBehaviour
{
    [Header("Note Size Settings")]
    [Range(0.1f, 2f)]
    public float noteScale = 0.5f; // Escala general de las notas
    
    [Header("Auto Resize")]
    public bool autoResizeExistingNotes = true;
    public float checkInterval = 0.2f;
    
    [Header("Debug")]
    public bool showDebugLogs = false;
    
    void Start()
    {
        if (autoResizeExistingNotes)
        {
            StartCoroutine(ResizeExistingNotes());
        }
    }
    
    System.Collections.IEnumerator ResizeExistingNotes()
    {
        while (true)
        {
            // Buscar todas las notas activas
            Note[] allNotes = FindObjectsByType<Note>(FindObjectsSortMode.None);
            
            foreach (Note note in allNotes)
            {
                ResizeNote(note);
            }
            
            yield return new WaitForSeconds(checkInterval);
        }
    }
    
    void ResizeNote(Note note)
    {
        if (note == null) return;
        
        // Cambiar la escala de la nota
        Vector3 targetScale = Vector3.one * noteScale;
        
        // Solo cambiar si es diferente para evitar updates innecesarios
        if (Vector3.Distance(note.transform.localScale, targetScale) > 0.01f)
        {
            note.transform.localScale = targetScale;
            
            if (showDebugLogs)
            {
                Debug.Log($"Nota redimensionada en carril {note.lane} - Nueva escala: {targetScale}");
            }
        }
    }
    
    // Método público para cambiar el tamaño en tiempo real
    public void SetNoteScale(float newScale)
    {
        noteScale = Mathf.Clamp(newScale, 0.1f, 2f);
        
        // Aplicar inmediatamente a todas las notas existentes
        Note[] allNotes = FindObjectsByType<Note>(FindObjectsSortMode.None);
        foreach (Note note in allNotes)
        {
            ResizeNote(note);
        }
        
        Debug.Log($"Escala de notas cambiada a: {noteScale}");
    }
    
    // Métodos de acceso rápido
    [ContextMenu("Make Notes Smaller")]
    public void MakeNotesSmaller()
    {
        SetNoteScale(noteScale * 0.8f);
    }
    
    [ContextMenu("Make Notes Bigger")]
    public void MakeNotesBigger()
    {
        SetNoteScale(noteScale * 1.2f);
    }
    
    [ContextMenu("Reset Note Size")]
    public void ResetNoteSize()
    {
        SetNoteScale(1f);
    }
    
    // Configuraciones predefinidas
    [ContextMenu("Tiny Notes")]
    public void SetTinyNotes()
    {
        SetNoteScale(0.3f);
    }
    
    [ContextMenu("Small Notes")]
    public void SetSmallNotes()
    {
        SetNoteScale(0.5f);
    }
    
    [ContextMenu("Normal Notes")]
    public void SetNormalNotes()
    {
        SetNoteScale(1f);
    }
    
    [ContextMenu("Large Notes")]
    public void SetLargeNotes()
    {
        SetNoteScale(1.5f);
    }
}
