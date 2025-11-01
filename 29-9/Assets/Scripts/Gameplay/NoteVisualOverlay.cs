using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Sistema de overlay visual para mostrar sprites 2D sobre las notas 3D existentes
/// </summary>
public class NoteVisualOverlay : MonoBehaviour
{
    [Header("Canvas Settings")]
    public Canvas overlayCanvas;
    public GameObject noteUIPrefab; // Prefab con Image component
    
    [Header("Note Sprites")]
    public Sprite[] noteSprites = new Sprite[5];
    
    [Header("Settings")]
    public float uiScale = 1f;
    public Vector2 spriteSize = new Vector2(80f, 80f);
    
    [Header("Perspective Scaling")]
    public float scaleAtHitZone = 1.5f; // Tamaño cuando llega a los botones
    public float scaleAtSpawn = 0.3f; // Tamaño cuando aparece arriba
    public float hitZoneZ = -8f; // Posición Z de los botones
    public float maxSpawnZ = 25f; // Posición Z máxima donde aparecen las notas
    public float scaleSmoothing = 5f; // Velocidad de transición de escala
    
    // Pool de objetos UI para optimización
    private Queue<GameObject> uiNotePool = new Queue<GameObject>();
    private Dictionary<Note, GameObject> activeUIElements = new Dictionary<Note, GameObject>();
    
    // Referencias
    private Camera mainCamera;
    private RectTransform canvasRect;
    
    // Nombres de sprites en Resources
    private string[] spriteNames = {
        "nota verde",
        "nota roja", 
        "nota amarilla",
        "nota azul",
        "nota rosa"
    };
    
    void Start()
    {
        mainCamera = Camera.main;
        SetupCanvas();
        LoadSprites();
        InitializePool();
        
        // Buscar notas existentes
        StartCoroutine(TrackExistingNotes());
    }
    
    void SetupCanvas()
    {
        if (overlayCanvas == null)
        {
            // Crear canvas automáticamente
            GameObject canvasObj = new GameObject("NoteOverlayCanvas");
            overlayCanvas = canvasObj.AddComponent<Canvas>();
            overlayCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            overlayCanvas.sortingOrder = 100; // Asegurar que esté encima
            
            // Añadir CanvasScaler para responsividad
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            // Añadir GraphicRaycaster
            canvasObj.AddComponent<GraphicRaycaster>();
        }
        
        canvasRect = overlayCanvas.GetComponent<RectTransform>();
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
    
    void InitializePool()
    {
        // Crear pool inicial de elementos UI
        for (int i = 0; i < 20; i++)
        {
            GameObject uiElement = CreateUIElement();
            uiElement.SetActive(false);
            uiNotePool.Enqueue(uiElement);
        }
    }
    
    GameObject CreateUIElement()
    {
        GameObject uiElement;
        
        if (noteUIPrefab != null)
        {
            uiElement = Instantiate(noteUIPrefab, overlayCanvas.transform);
        }
        else
        {
            // Crear elemento UI básico
            uiElement = new GameObject("NoteUI");
            uiElement.transform.SetParent(overlayCanvas.transform, false);
            
            // Añadir RectTransform
            RectTransform rectTransform = uiElement.AddComponent<RectTransform>();
            rectTransform.sizeDelta = spriteSize;
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            
            // Añadir Image component
            Image image = uiElement.AddComponent<Image>();
            image.raycastTarget = false;
        }
        
        return uiElement;
    }
    
    System.Collections.IEnumerator TrackExistingNotes()
    {
        while (true)
        {
            // Buscar todas las notas activas
            Note[] allNotes = FindObjectsByType<Note>(FindObjectsSortMode.None);
            
            // Añadir nuevas notas
            foreach (Note note in allNotes)
            {
                if (!activeUIElements.ContainsKey(note))
                {
                    CreateUIForNote(note);
                }
            }
            
            // Limpiar notas destruidas
            List<Note> notesToRemove = new List<Note>();
            foreach (var kvp in activeUIElements)
            {
                if (kvp.Key == null) // Nota fue destruida
                {
                    ReturnUIToPool(kvp.Value);
                    notesToRemove.Add(kvp.Key);
                }
            }
            
            foreach (Note note in notesToRemove)
            {
                activeUIElements.Remove(note);
            }
            
            yield return new WaitForSeconds(0.1f); // Verificar cada 0.1 segundos
        }
    }
    
    void CreateUIForNote(Note note)
    {
        GameObject uiElement = GetUIFromPool();
        if (uiElement == null) return;
        
        // Configurar el sprite
        Image image = uiElement.GetComponent<Image>();
        if (image != null && note.lane >= 0 && note.lane < noteSprites.Length && noteSprites[note.lane] != null)
        {
            image.sprite = noteSprites[note.lane];
            image.color = Color.white;
        }
        
        // Configurar escala
        uiElement.transform.localScale = Vector3.one * uiScale;
        
        // Activar y añadir al diccionario
        uiElement.SetActive(true);
        activeUIElements[note] = uiElement;
        
        Debug.Log($"UI creada para nota en carril {note.lane}");
    }
    
    GameObject GetUIFromPool()
    {
        if (uiNotePool.Count > 0)
        {
            return uiNotePool.Dequeue();
        }
        
        // Crear nuevo elemento si el pool está vacío
        return CreateUIElement();
    }
    
    void ReturnUIToPool(GameObject uiElement)
    {
        if (uiElement != null)
        {
            uiElement.SetActive(false);
            uiNotePool.Enqueue(uiElement);
        }
    }
    
    void Update()
    {
        UpdateUIPositions();
    }
    
    void UpdateUIPositions()
    {
        foreach (var kvp in activeUIElements)
        {
            Note note = kvp.Key;
            GameObject uiElement = kvp.Value;
            
            if (note != null && uiElement != null)
            {
                // Convertir posición 3D de la nota a posición de pantalla
                Vector3 screenPos = mainCamera.WorldToScreenPoint(note.transform.position);
                
                // Verificar si está en pantalla
                if (screenPos.z > 0 && screenPos.x >= 0 && screenPos.x <= Screen.width && 
                    screenPos.y >= 0 && screenPos.y <= Screen.height)
                {
                    // Convertir a coordenadas del canvas
                    Vector2 canvasPos;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        canvasRect, screenPos, null, out canvasPos);
                    
                    // Actualizar posición del elemento UI
                    RectTransform uiRect = uiElement.GetComponent<RectTransform>();
                    if (uiRect != null)
                    {
                        uiRect.anchoredPosition = canvasPos;
                        
                        // CORREGIR ESCALADO POR PERSPECTIVA
                        // Calcular escala basada en la distancia Z (más cerca = más grande)
                        float distanceFromCamera = Vector3.Distance(mainCamera.transform.position, note.transform.position);
                        float baseDistance = 20f; // Distancia de referencia
                        float perspectiveScale = baseDistance / distanceFromCamera;
                        
                        // Invertir el efecto: notas más cerca del jugador (Z menor) deberían ser más grandes
                        float noteZ = note.transform.position.z;
                        
                        // Escala inversa: cuando Z se acerca a hitZoneZ, la escala aumenta
                        float normalizedDistance = Mathf.Clamp01((noteZ - hitZoneZ) / (maxSpawnZ - hitZoneZ));
                        float targetScale = Mathf.Lerp(scaleAtHitZone, scaleAtSpawn, normalizedDistance); // Grande cerca, pequeño lejos
                        
                        // Aplicar escala con suavizado
                        Vector3 currentScale = uiElement.transform.localScale;
                        Vector3 newScale = Vector3.one * (uiScale * targetScale);
                        uiElement.transform.localScale = Vector3.Lerp(currentScale, newScale, Time.deltaTime * scaleSmoothing);
                    }
                    
                    // Asegurar que esté visible
                    if (!uiElement.activeInHierarchy)
                    {
                        uiElement.SetActive(true);
                    }
                }
                else
                {
                    // Ocultar si está fuera de pantalla
                    if (uiElement.activeInHierarchy)
                    {
                        uiElement.SetActive(false);
                    }
                }
            }
        }
    }
    
    // Método público para configurar sprites manualmente
    public void SetNoteSprite(int lane, Sprite sprite)
    {
        if (lane >= 0 && lane < noteSprites.Length)
        {
            noteSprites[lane] = sprite;
        }
    }
    
    // Método para limpiar todos los elementos UI
    public void ClearAllUI()
    {
        foreach (var kvp in activeUIElements)
        {
            ReturnUIToPool(kvp.Value);
        }
        activeUIElements.Clear();
    }
}
