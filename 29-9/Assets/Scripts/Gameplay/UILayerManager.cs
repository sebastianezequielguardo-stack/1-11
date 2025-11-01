using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Gestiona las capas de UI para que las imágenes aparezcan en el orden correcto
/// Coloca automáticamente imágenes de fondo detrás de botones
/// </summary>
public class UILayerManager : MonoBehaviour
{
    [Header("Layer Configuration")]
    public bool organizeOnStart = true;
    
    [Header("Layer Orders")]
    public int backgroundImageOrder = 0;
    public int trackBaseOrder = 1;
    public int buttonOrder = 2;
    public int topUIOrder = 3;
    
    [Header("Target Objects")]
    public GameObject trackBaseObject;
    public Image[] backgroundImages;
    public Button[] buttons;
    
    void Start()
    {
        if (organizeOnStart)
        {
            OrganizeUILayers();
        }
    }
    
    /// <summary>
    /// Organiza automáticamente las capas de UI
    /// </summary>
    [ContextMenu("Organize UI Layers")]
    public void OrganizeUILayers()
    {
        Debug.Log("🎨 Organizando capas de UI...");
        
        // Buscar objetos automáticamente si no están asignados
        if (trackBaseObject == null)
        {
            trackBaseObject = GameObject.Find("TrackBase");
        }
        
        // Organizar imágenes de fondo
        OrganizeBackgroundImages();
        
        // Organizar TrackBase
        OrganizeTrackBase();
        
        // Organizar botones
        OrganizeButtons();
        
        // Organizar por jerarquía también
        OrganizeHierarchy();
        
        Debug.Log("✅ Capas de UI organizadas correctamente");
    }
    
    /// <summary>
    /// Organiza las imágenes de fondo
    /// </summary>
    void OrganizeBackgroundImages()
    {
        // Buscar imágenes de fondo si no están asignadas
        if (backgroundImages == null || backgroundImages.Length == 0)
        {
            backgroundImages = FindBackgroundImages();
        }
        
        foreach (Image bgImage in backgroundImages)
        {
            if (bgImage != null)
            {
                // Configurar Order in Layer
                Canvas canvas = bgImage.GetComponentInParent<Canvas>();
                if (canvas != null)
                {
                    bgImage.canvas.overrideSorting = true;
                    bgImage.canvas.sortingOrder = backgroundImageOrder;
                }
                
                // Configurar componente Image
                bgImage.raycastTarget = false; // No bloquear clicks
                
                Debug.Log($"🖼️ Imagen de fondo configurada: {bgImage.name} (Order: {backgroundImageOrder})");
            }
        }
    }
    
    /// <summary>
    /// Organiza el TrackBase
    /// </summary>
    void OrganizeTrackBase()
    {
        if (trackBaseObject != null)
        {
            // Configurar Canvas del TrackBase
            Canvas trackCanvas = trackBaseObject.GetComponent<Canvas>();
            if (trackCanvas == null)
            {
                trackCanvas = trackBaseObject.AddComponent<Canvas>();
            }
            
            trackCanvas.overrideSorting = true;
            trackCanvas.sortingOrder = trackBaseOrder;
            
            // Configurar GraphicRaycaster si no existe
            GraphicRaycaster raycaster = trackBaseObject.GetComponent<GraphicRaycaster>();
            if (raycaster == null)
            {
                trackBaseObject.AddComponent<GraphicRaycaster>();
            }
            
            Debug.Log($"🎯 TrackBase configurado (Order: {trackBaseOrder})");
        }
    }
    
    /// <summary>
    /// Organiza los botones
    /// </summary>
    void OrganizeButtons()
    {
        // Buscar botones si no están asignados
        if (buttons == null || buttons.Length == 0)
        {
            buttons = FindButtonsInTrackBase();
        }
        
        for (int i = 0; i < buttons.Length; i++)
        {
            Button button = buttons[i];
            if (button != null)
            {
                // Configurar Order in Layer para cada botón
                Image buttonImage = button.GetComponent<Image>();
                if (buttonImage != null)
                {
                    Canvas buttonCanvas = button.GetComponentInParent<Canvas>();
                    if (buttonCanvas != null)
                    {
                        // Crear canvas individual para el botón si es necesario
                        Canvas individualCanvas = button.GetComponent<Canvas>();
                        if (individualCanvas == null)
                        {
                            individualCanvas = button.gameObject.AddComponent<Canvas>();
                        }
                        
                        individualCanvas.overrideSorting = true;
                        individualCanvas.sortingOrder = buttonOrder + i; // Cada botón con orden diferente
                        
                        // Asegurar que el botón sea clickeable
                        buttonImage.raycastTarget = true;
                    }
                }
                
                Debug.Log($"🔘 Botón configurado: {button.name} (Order: {buttonOrder + i})");
            }
        }
    }
    
    /// <summary>
    /// Organiza la jerarquía para mejor orden visual
    /// </summary>
    void OrganizeHierarchy()
    {
        Debug.Log("📋 Organizando jerarquía...");
        
        // Mover imágenes de fondo al principio de la jerarquía
        foreach (Image bgImage in backgroundImages)
        {
            if (bgImage != null)
            {
                bgImage.transform.SetAsFirstSibling();
            }
        }
        
        // Mover TrackBase después de las imágenes de fondo
        if (trackBaseObject != null)
        {
            trackBaseObject.transform.SetSiblingIndex(backgroundImages.Length);
        }
        
        Debug.Log("✅ Jerarquía organizada");
    }
    
    /// <summary>
    /// Busca automáticamente imágenes de fondo
    /// </summary>
    Image[] FindBackgroundImages()
    {
        Image[] allImages = FindObjectsByType<Image>(FindObjectsSortMode.None);
        System.Collections.Generic.List<Image> backgrounds = new System.Collections.Generic.List<Image>();
        
        foreach (Image img in allImages)
        {
            string name = img.name.ToLower();
            if (name.Contains("background") || 
                name.Contains("fondo") || 
                name.Contains("bg") ||
                name.Contains("highway") ||
                name.Contains("katana"))
            {
                backgrounds.Add(img);
            }
        }
        
        Debug.Log($"🔍 Encontradas {backgrounds.Count} imágenes de fondo automáticamente");
        return backgrounds.ToArray();
    }
    
    /// <summary>
    /// Busca botones en TrackBase
    /// </summary>
    Button[] FindButtonsInTrackBase()
    {
        if (trackBaseObject != null)
        {
            Button[] buttons = trackBaseObject.GetComponentsInChildren<Button>();
            Debug.Log($"🔍 Encontrados {buttons.Length} botones en TrackBase");
            return buttons;
        }
        
        // Si no hay TrackBase, buscar todos los botones
        Button[] allButtons = FindObjectsByType<Button>(FindObjectsSortMode.None);
        Debug.Log($"🔍 Encontrados {allButtons.Length} botones en total");
        return allButtons;
    }
    
#if UNITY_EDITOR
    /// <summary>
    /// Configurar una imagen específica como fondo
    /// </summary>
    [ContextMenu("Set Selected As Background")]
    public void SetSelectedAsBackground()
    {
        GameObject selected = UnityEditor.Selection.activeGameObject;
        if (selected != null)
        {
            Image image = selected.GetComponent<Image>();
            if (image != null)
            {
                // Configurar como fondo
                Canvas canvas = image.GetComponentInParent<Canvas>();
                if (canvas != null)
                {
                    Canvas individualCanvas = image.GetComponent<Canvas>();
                    if (individualCanvas == null)
                    {
                        individualCanvas = image.gameObject.AddComponent<Canvas>();
                    }
                    
                    individualCanvas.overrideSorting = true;
                    individualCanvas.sortingOrder = backgroundImageOrder;
                    image.raycastTarget = false;
                    
                    // Mover al principio de la jerarquía
                    image.transform.SetAsFirstSibling();
                    
                    Debug.Log($"✅ {selected.name} configurado como imagen de fondo");
                }
            }
        }
    }
#endif
    
#if UNITY_EDITOR
    /// <summary>
    /// Configurar una imagen específica como botón
    /// </summary>
    [ContextMenu("Set Selected As Button")]
    public void SetSelectedAsButton()
    {
        GameObject selected = UnityEditor.Selection.activeGameObject;
        if (selected != null)
        {
            Image image = selected.GetComponent<Image>();
            if (image != null)
            {
                // Configurar como botón
                Canvas individualCanvas = image.GetComponent<Canvas>();
                if (individualCanvas == null)
                {
                    individualCanvas = image.gameObject.AddComponent<Canvas>();
                }
                
                individualCanvas.overrideSorting = true;
                individualCanvas.sortingOrder = buttonOrder;
                image.raycastTarget = true;
                
                // Agregar Button component si no existe
                Button button = image.GetComponent<Button>();
                if (button == null)
                {
                    image.gameObject.AddComponent<Button>();
                }
                
                Debug.Log($"✅ {selected.name} configurado como botón");
            }
        }
    }
#endif
    
    /// <summary>
    /// Mostrar información de las capas actuales
    /// </summary>
    [ContextMenu("Show Layer Info")]
    public void ShowLayerInfo()
    {
        Debug.Log("📊 INFORMACIÓN DE CAPAS UI:");
        Debug.Log("═══════════════════════════");
        
        // Mostrar imágenes de fondo
        if (backgroundImages != null)
        {
            Debug.Log($"Imágenes de fondo: {backgroundImages.Length}");
            foreach (Image bg in backgroundImages)
            {
                if (bg != null)
                {
                    Canvas canvas = bg.GetComponent<Canvas>();
                    int order = canvas != null ? canvas.sortingOrder : -1;
                    Debug.Log($"  🖼️ {bg.name} (Order: {order})");
                }
            }
        }
        
        // Mostrar TrackBase
        if (trackBaseObject != null)
        {
            Canvas trackCanvas = trackBaseObject.GetComponent<Canvas>();
            int trackOrder = trackCanvas != null ? trackCanvas.sortingOrder : -1;
            Debug.Log($"TrackBase: {trackBaseObject.name} (Order: {trackOrder})");
        }
        
        // Mostrar botones
        if (buttons != null)
        {
            Debug.Log($"Botones: {buttons.Length}");
            foreach (Button btn in buttons)
            {
                if (btn != null)
                {
                    Canvas canvas = btn.GetComponent<Canvas>();
                    int order = canvas != null ? canvas.sortingOrder : -1;
                    Debug.Log($"  🔘 {btn.name} (Order: {order})");
                }
            }
        }
    }
}
