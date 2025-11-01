using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Arregla las capas dentro de un Panel específico
/// Coloca la imagen del highway detrás de los botones automáticamente
/// </summary>
public class PanelLayerFixer : MonoBehaviour
{
    [Header("Target Panel")]
    public GameObject targetPanel; // El panel que contiene highway y botones
    
    [Header("Layer Orders")]
    public int highwayImageOrder = -1;  // Highway atrás
    public int buttonImageOrder = 1;    // Botones encima
    
    [Header("Auto Detection")]
    public bool autoFindPanel = true;
    public bool fixOnStart = true;
    
    void Start()
    {
        if (fixOnStart)
        {
            FixPanelLayers();
        }
    }
    
    /// <summary>
    /// Arregla las capas dentro del panel
    /// </summary>
    [ContextMenu("Fix Panel Layers")]
    public void FixPanelLayers()
    {
        Debug.Log("🔧 Arreglando capas dentro del panel...");
        
        // Buscar panel automáticamente si no está asignado
        if (targetPanel == null && autoFindPanel)
        {
            FindTargetPanel();
        }
        
        if (targetPanel == null)
        {
            Debug.LogError("❌ No se encontró el panel objetivo");
            return;
        }
        
        // Arreglar highway image
        FixHighwayImage();
        
        // Arreglar button images
        FixButtonImages();
        
        // Organizar jerarquía
        OrganizeHierarchy();
        
        Debug.Log("✅ Capas del panel arregladas correctamente");
    }
    
    /// <summary>
    /// Busca automáticamente el panel que contiene highway y botones
    /// </summary>
    void FindTargetPanel()
    {
        Debug.Log("🔍 Buscando panel automáticamente...");
        
        // Buscar por nombres comunes
        string[] panelNames = { "Panel", "TrackBase", "UI Panel", "ButtonPanel", "GamePanel" };
        
        foreach (string name in panelNames)
        {
            GameObject panel = GameObject.Find(name);
            if (panel != null)
            {
                // Verificar si tiene imágenes dentro
                Image[] images = panel.GetComponentsInChildren<Image>();
                if (images.Length > 1) // Debe tener highway + botones
                {
                    targetPanel = panel;
                    Debug.Log($"✅ Panel encontrado: {name}");
                    return;
                }
            }
        }
        
        // Buscar cualquier objeto con múltiples imágenes
        Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        foreach (Canvas canvas in canvases)
        {
            Transform[] children = canvas.GetComponentsInChildren<Transform>();
            foreach (Transform child in children)
            {
                Image[] images = child.GetComponentsInChildren<Image>();
                if (images.Length > 1)
                {
                    targetPanel = child.gameObject;
                    Debug.Log($"✅ Panel encontrado automáticamente: {child.name}");
                    return;
                }
            }
        }
        
        Debug.LogWarning("⚠️ No se pudo encontrar panel automáticamente");
    }
    
    /// <summary>
    /// Arregla la imagen del highway para que esté atrás
    /// </summary>
    void FixHighwayImage()
    {
        if (targetPanel == null) return;
        
        Image[] images = targetPanel.GetComponentsInChildren<Image>();
        
        foreach (Image img in images)
        {
            string name = img.name.ToLower();
            
            // Detectar imagen del highway
            if (name.Contains("highway") || 
                name.Contains("katana") || 
                name.Contains("road") || 
                name.Contains("track") ||
                name.Contains("background") ||
                name.Contains("fondo"))
            {
                // Configurar como imagen de fondo
                SetImageLayer(img, highwayImageOrder, false);
                
                // Mover al principio de la jerarquía (atrás)
                img.transform.SetAsFirstSibling();
                
                Debug.Log($"🛣️ Highway image configurada: {img.name} (Order: {highwayImageOrder})");
            }
        }
    }
    
    /// <summary>
    /// Arregla las imágenes de los botones para que estén encima
    /// </summary>
    void FixButtonImages()
    {
        if (targetPanel == null) return;
        
        // Buscar botones
        Button[] buttons = targetPanel.GetComponentsInChildren<Button>();
        
        for (int i = 0; i < buttons.Length; i++)
        {
            Button btn = buttons[i];
            Image btnImage = btn.GetComponent<Image>();
            
            if (btnImage != null)
            {
                // Configurar como botón (encima)
                SetImageLayer(btnImage, buttonImageOrder + i, true);
                
                Debug.Log($"🔘 Button image configurada: {btn.name} (Order: {buttonImageOrder + i})");
            }
        }
        
        // También buscar imágenes que parezcan botones
        Image[] images = targetPanel.GetComponentsInChildren<Image>();
        
        foreach (Image img in images)
        {
            string name = img.name.ToLower();
            
            if (name.Contains("button") || 
                name.Contains("btn") || 
                name.Contains("boton") ||
                name.Contains("note") ||
                name.Contains("hit"))
            {
                // Solo si no es highway
                if (!name.Contains("highway") && !name.Contains("katana"))
                {
                    SetImageLayer(img, buttonImageOrder, true);
                    Debug.Log($"🔘 Button-like image configurada: {img.name}");
                }
            }
        }
    }
    
    /// <summary>
    /// Configura la capa de una imagen específica
    /// </summary>
    void SetImageLayer(Image image, int order, bool raycastTarget)
    {
        if (image == null) return;
        
        // Método 1: Canvas individual
        Canvas individualCanvas = image.GetComponent<Canvas>();
        if (individualCanvas == null)
        {
            individualCanvas = image.gameObject.AddComponent<Canvas>();
        }
        
        individualCanvas.overrideSorting = true;
        individualCanvas.sortingOrder = order;
        
        // Configurar raycast
        image.raycastTarget = raycastTarget;
        
        // Método 2: Graphic component (backup)
        Graphic graphic = image.GetComponent<Graphic>();
        if (graphic != null)
        {
            graphic.raycastTarget = raycastTarget;
        }
    }
    
    /// <summary>
    /// Organiza la jerarquía para mejor orden visual
    /// </summary>
    void OrganizeHierarchy()
    {
        if (targetPanel == null) return;
        
        Debug.Log("📋 Organizando jerarquía del panel...");
        
        Transform panelTransform = targetPanel.transform;
        
        // Primero: mover highway images al principio
        for (int i = 0; i < panelTransform.childCount; i++)
        {
            Transform child = panelTransform.GetChild(i);
            string name = child.name.ToLower();
            
            if (name.Contains("highway") || name.Contains("katana"))
            {
                child.SetAsFirstSibling();
                Debug.Log($"🛣️ {child.name} movido al principio");
            }
        }
        
        // Segundo: mover botones al final
        for (int i = 0; i < panelTransform.childCount; i++)
        {
            Transform child = panelTransform.GetChild(i);
            
            if (child.GetComponent<Button>() != null)
            {
                child.SetAsLastSibling();
                Debug.Log($"🔘 {child.name} movido al final");
            }
        }
    }
    
#if UNITY_EDITOR
    /// <summary>
    /// Configurar manualmente una imagen como highway
    /// </summary>
    [ContextMenu("Set Selected As Highway")]
    public void SetSelectedAsHighway()
    {
        GameObject selected = UnityEditor.Selection.activeGameObject;
        if (selected != null)
        {
            Image image = selected.GetComponent<Image>();
            if (image != null)
            {
                SetImageLayer(image, highwayImageOrder, false);
                image.transform.SetAsFirstSibling();
                Debug.Log($"✅ {selected.name} configurado como highway (atrás)");
            }
        }
    }
#endif
    
#if UNITY_EDITOR
    /// <summary>
    /// Configurar manualmente una imagen como botón
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
                SetImageLayer(image, buttonImageOrder, true);
                image.transform.SetAsLastSibling();
                Debug.Log($"✅ {selected.name} configurado como botón (encima)");
            }
        }
    }
#endif
    
    /// <summary>
    /// Mostrar información del panel
    /// </summary>
    [ContextMenu("Show Panel Info")]
    public void ShowPanelInfo()
    {
        if (targetPanel == null)
        {
            Debug.LogWarning("⚠️ No hay panel asignado");
            return;
        }
        
        Debug.Log("📊 INFORMACIÓN DEL PANEL:");
        Debug.Log("═══════════════════════════");
        Debug.Log($"Panel: {targetPanel.name}");
        
        Image[] images = targetPanel.GetComponentsInChildren<Image>();
        Debug.Log($"Total imágenes: {images.Length}");
        
        foreach (Image img in images)
        {
            Canvas canvas = img.GetComponent<Canvas>();
            int order = canvas != null ? canvas.sortingOrder : 0;
            string type = img.raycastTarget ? "Clickeable" : "Fondo";
            
            Debug.Log($"  🖼️ {img.name} (Order: {order}, {type})");
        }
        
        Button[] buttons = targetPanel.GetComponentsInChildren<Button>();
        Debug.Log($"Total botones: {buttons.Length}");
        
        foreach (Button btn in buttons)
        {
            Debug.Log($"  🔘 {btn.name}");
        }
    }
    
    /// <summary>
    /// Arreglo rápido para casos específicos
    /// </summary>
    [ContextMenu("Quick Fix - Highway Behind Buttons")]
    public void QuickFixHighwayBehindButtons()
    {
        Debug.Log("⚡ Arreglo rápido: Highway atrás, botones encima");
        
        // Buscar todas las imágenes en la escena
        Image[] allImages = FindObjectsByType<Image>(FindObjectsSortMode.None);
        
        foreach (Image img in allImages)
        {
            string name = img.name.ToLower();
            
            // Si es highway, ponerlo atrás
            if (name.Contains("highway") || name.Contains("katana"))
            {
                SetImageLayer(img, -10, false);
                img.transform.SetAsFirstSibling();
                Debug.Log($"🛣️ {img.name} configurado atrás");
            }
            // Si es botón, ponerlo encima
            else if (img.GetComponent<Button>() != null || 
                     name.Contains("button") || 
                     name.Contains("btn"))
            {
                SetImageLayer(img, 10, true);
                img.transform.SetAsLastSibling();
                Debug.Log($"🔘 {img.name} configurado encima");
            }
        }
        
        Debug.Log("✅ Arreglo rápido completado");
    }
}
