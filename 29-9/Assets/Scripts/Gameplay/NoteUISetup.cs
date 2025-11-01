using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script helper para configurar el prefab de nota UI
/// </summary>
public class NoteUISetup : MonoBehaviour
{
    [Header("Auto Setup")]
    public bool setupOnStart = true;
    
    void Start()
    {
        if (setupOnStart)
        {
            SetupNotePrefab();
        }
    }
    
    public void SetupNotePrefab()
    {
        // Asegurar que tenga los componentes necesarios
        RectTransform rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            rectTransform = gameObject.AddComponent<RectTransform>();
        }
        
        // Configurar tamaño por defecto
        rectTransform.sizeDelta = new Vector2(80f, 80f);
        
        // Asegurar que tenga Image component
        Image image = GetComponent<Image>();
        if (image == null)
        {
            image = gameObject.AddComponent<Image>();
        }
        
        // Configurar propiedades de la imagen
        image.raycastTarget = false; // No necesita recibir clicks
        
        // Asegurar que tenga FallingNote2D component
        FallingNote2D fallingNote = GetComponent<FallingNote2D>();
        if (fallingNote == null)
        {
            fallingNote = gameObject.AddComponent<FallingNote2D>();
        }
        
        // Asignar referencia de imagen
        fallingNote.noteImage = image;
        
        // Prefab configurado
    }
}
