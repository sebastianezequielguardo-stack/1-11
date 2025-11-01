using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

/// <summary>
/// Restaura específicamente los elementos importantes del gameplay que se borraron
/// Recrea highway, botones, video de fondo y todo lo esencial
/// </summary>
public class RestoreGameplayElements : MonoBehaviour
{
    [Header("Restoration Settings")]
    public bool showDebugLogs = true;
    
    [Header("Highway Settings")]
    public Material highwayMaterial;
    public Texture2D highwayTexture;
    
    [Header("Button Settings")]
    public Sprite buttonSprite;
    public Color[] buttonColors = { Color.green, Color.red, Color.blue, Color.yellow, Color.magenta };
    
    void Start()
    {
        // Auto-restaurar al iniciar
        RestoreAllGameplayElements();
    }
    
    /// <summary>
    /// Restaura todos los elementos del gameplay
    /// </summary>
    [ContextMenu("Restore All Gameplay Elements")]
    public void RestoreAllGameplayElements()
    {
        Debug.Log("🔧 RESTAURANDO ELEMENTOS DEL GAMEPLAY");
        Debug.Log("═══════════════════════════════════════");
        
        RestoreTrackBase();
        RestoreHighwayImage();
        RestoreButtonImages();
        RestoreBackgroundVideo();
        RestoreCanvas();
        
        Debug.Log("✅ Restauración completa del gameplay terminada");
    }
    
    /// <summary>
    /// Restaura el TrackBase
    /// </summary>
    [ContextMenu("Restore TrackBase")]
    public void RestoreTrackBase()
    {
        GameObject trackBase = GameObject.Find("TrackBase");
        if (trackBase == null)
        {
            trackBase = new GameObject("TrackBase");
            
            // Agregar Canvas
            Canvas canvas = trackBase.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = Camera.main;
            
            // Configurar RectTransform
            RectTransform rectTransform = trackBase.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(1000, 600);
            rectTransform.position = new Vector3(0, 0, 0);
            
            // Agregar GraphicRaycaster
            trackBase.AddComponent<GraphicRaycaster>();
            
            Debug.Log("🎯 TrackBase restaurado");
        }
        else
        {
            Debug.Log("🎯 TrackBase ya existe");
        }
    }
    
    /// <summary>
    /// Restaura la imagen del highway
    /// </summary>
    [ContextMenu("Restore Highway Image")]
    public void RestoreHighwayImage()
    {
        GameObject trackBase = GameObject.Find("TrackBase");
        if (trackBase == null)
        {
            RestoreTrackBase();
            trackBase = GameObject.Find("TrackBase");
        }
        
        // Buscar si ya existe highway image
        Transform existingHighway = trackBase.transform.Find("HighwayImage");
        if (existingHighway == null)
        {
            // Crear highway image
            GameObject highwayImage = new GameObject("HighwayImage");
            highwayImage.transform.SetParent(trackBase.transform);
            
            // Agregar Image component
            Image image = highwayImage.AddComponent<Image>();
            
            // Crear textura metálica programáticamente
            Texture2D metallicTexture = CreateMetallicTexture();
            Sprite metallicSprite = Sprite.Create(metallicTexture, 
                new Rect(0, 0, metallicTexture.width, metallicTexture.height), 
                new Vector2(0.5f, 0.5f));
            
            image.sprite = metallicSprite;
            image.color = new Color(0.7f, 0.7f, 0.8f, 1f); // Color metálico
            
            // Configurar RectTransform para que cubra toda el área
            RectTransform rectTransform = highwayImage.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchoredPosition = Vector2.zero;
            
            // Configurar para que esté atrás
            image.raycastTarget = false;
            
            // Agregar Canvas para control de orden
            Canvas highwayCanvas = highwayImage.AddComponent<Canvas>();
            highwayCanvas.overrideSorting = true;
            highwayCanvas.sortingOrder = -1; // Atrás de todo
            
            Debug.Log("🛣️ Highway Image restaurada con textura metálica");
        }
        else
        {
            Debug.Log("🛣️ Highway Image ya existe");
        }
    }
    
    /// <summary>
    /// Crea una textura metálica programáticamente
    /// </summary>
    Texture2D CreateMetallicTexture()
    {
        int width = 512;
        int height = 512;
        Texture2D texture = new Texture2D(width, height);
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Crear patrón metálico con líneas
                float linePattern = Mathf.Sin(y * 0.1f) * 0.1f + 0.5f;
                float noisePattern = Mathf.PerlinNoise(x * 0.01f, y * 0.01f) * 0.2f;
                
                float intensity = linePattern + noisePattern;
                Color pixelColor = new Color(intensity, intensity, intensity + 0.1f, 1f);
                
                texture.SetPixel(x, y, pixelColor);
            }
        }
        
        texture.Apply();
        return texture;
    }
    
    /// <summary>
    /// Restaura las imágenes de los botones
    /// </summary>
    [ContextMenu("Restore Button Images")]
    public void RestoreButtonImages()
    {
        GameObject trackBase = GameObject.Find("TrackBase");
        if (trackBase == null)
        {
            RestoreTrackBase();
            trackBase = GameObject.Find("TrackBase");
        }
        
        // Crear contenedor para botones si no existe
        Transform buttonContainer = trackBase.transform.Find("ButtonContainer");
        if (buttonContainer == null)
        {
            GameObject container = new GameObject("ButtonContainer");
            container.transform.SetParent(trackBase.transform);
            buttonContainer = container.transform;
            
            // Configurar RectTransform
            RectTransform containerRect = container.AddComponent<RectTransform>();
            containerRect.anchorMin = new Vector2(0f, 0f);
            containerRect.anchorMax = new Vector2(1f, 0.3f);
            containerRect.sizeDelta = Vector2.zero;
            containerRect.anchoredPosition = Vector2.zero;
        }
        
        // Crear 5 botones (típico de Guitar Hero)
        string[] buttonNames = { "GreenButton", "RedButton", "BlueButton", "YellowButton", "MagentaButton" };
        
        for (int i = 0; i < buttonNames.Length; i++)
        {
            Transform existingButton = buttonContainer.Find(buttonNames[i]);
            if (existingButton == null)
            {
                // Crear botón
                GameObject button = new GameObject(buttonNames[i]);
                button.transform.SetParent(buttonContainer);
                
                // Agregar Image
                Image buttonImage = button.AddComponent<Image>();
                
                // Crear sprite circular para el botón
                Texture2D buttonTexture = CreateCircularButtonTexture(buttonColors[i]);
                Sprite buttonSprite = Sprite.Create(buttonTexture,
                    new Rect(0, 0, buttonTexture.width, buttonTexture.height),
                    new Vector2(0.5f, 0.5f));
                
                buttonImage.sprite = buttonSprite;
                buttonImage.color = buttonColors[i];
                
                // Configurar posición
                RectTransform buttonRect = button.GetComponent<RectTransform>();
                buttonRect.sizeDelta = new Vector2(80, 80);
                
                // Posicionar horizontalmente
                float spacing = 100f;
                float startX = -200f;
                buttonRect.anchoredPosition = new Vector2(startX + (i * spacing), 0);
                
                // Agregar Button component
                Button buttonComp = button.AddComponent<Button>();
                buttonComp.targetGraphic = buttonImage;
                
                // Configurar Canvas para orden
                Canvas buttonCanvas = button.AddComponent<Canvas>();
                buttonCanvas.overrideSorting = true;
                buttonCanvas.sortingOrder = 1; // Encima del highway
                
                Debug.Log($"🔘 {buttonNames[i]} restaurado");
            }
        }
        
        Debug.Log("🎮 Todas las imágenes de botones restauradas");
    }
    
    /// <summary>
    /// Crea una textura circular para botones
    /// </summary>
    Texture2D CreateCircularButtonTexture(Color baseColor)
    {
        int size = 128;
        Texture2D texture = new Texture2D(size, size);
        Vector2 center = new Vector2(size / 2f, size / 2f);
        float radius = size / 2f - 4f;
        
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                Vector2 pos = new Vector2(x, y);
                float distance = Vector2.Distance(pos, center);
                
                if (distance <= radius)
                {
                    // Dentro del círculo
                    float intensity = 1f - (distance / radius) * 0.3f;
                    Color pixelColor = baseColor * intensity;
                    pixelColor.a = 1f;
                    texture.SetPixel(x, y, pixelColor);
                }
                else
                {
                    // Fuera del círculo - transparente
                    texture.SetPixel(x, y, Color.clear);
                }
            }
        }
        
        texture.Apply();
        return texture;
    }
    
    /// <summary>
    /// Restaura el video de fondo
    /// </summary>
    [ContextMenu("Restore Background Video")]
    public void RestoreBackgroundVideo()
    {
        // Buscar si ya existe video player
        VideoPlayer existingPlayer = FindFirstObjectByType<VideoPlayer>();
        if (existingPlayer == null)
        {
            // Crear GameObject para video
            GameObject videoObj = new GameObject("BackgroundVideo");
            
            // Agregar VideoPlayer
            VideoPlayer videoPlayer = videoObj.AddComponent<VideoPlayer>();
            videoPlayer.playOnAwake = false;
            videoPlayer.isLooping = true;
            videoPlayer.renderMode = VideoRenderMode.RenderTexture;
            
            // Crear RenderTexture
            RenderTexture renderTexture = new RenderTexture(1920, 1080, 16);
            videoPlayer.targetTexture = renderTexture;
            
            // Crear quad para mostrar el video
            GameObject videoQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            videoQuad.name = "VideoQuad";
            videoQuad.transform.SetParent(videoObj.transform);
            
            // Posicionar detrás de todo
            videoQuad.transform.position = new Vector3(0, 0, 50f);
            videoQuad.transform.rotation = Quaternion.identity;
            videoQuad.transform.localScale = new Vector3(60f, 40f, 1f);
            
            // Configurar material
            Renderer quadRenderer = videoQuad.GetComponent<Renderer>();
            Material videoMaterial = new Material(Shader.Find("Unlit/Texture"));
            videoMaterial.mainTexture = renderTexture;
            quadRenderer.material = videoMaterial;
            
            // Remover collider innecesario
            Collider quadCollider = videoQuad.GetComponent<Collider>();
            if (quadCollider != null)
            {
                DestroyImmediate(quadCollider);
            }
            
            // Agregar script de gestión de video
            BackgroundVideoSystem videoSystem = videoObj.AddComponent<BackgroundVideoSystem>();
            
            Debug.Log("🎬 Sistema de video de fondo restaurado");
        }
        else
        {
            Debug.Log("🎬 Video player ya existe");
        }
    }
    
    /// <summary>
    /// Restaura Canvas principal si no existe
    /// </summary>
    [ContextMenu("Restore Canvas")]
    public void RestoreCanvas()
    {
        Canvas mainCanvas = FindFirstObjectByType<Canvas>();
        if (mainCanvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            canvasObj.AddComponent<GraphicRaycaster>();
            
            Debug.Log("🖼️ Canvas principal restaurado");
        }
        else
        {
            Debug.Log("🖼️ Canvas ya existe");
        }
    }
    
    /// <summary>
    /// Restaura elementos específicos por separado
    /// </summary>
    [ContextMenu("Quick Fix - Restore Highway Only")]
    public void QuickRestoreHighway()
    {
        RestoreTrackBase();
        RestoreHighwayImage();
        Debug.Log("⚡ Highway restaurado rápidamente");
    }
    
    [ContextMenu("Quick Fix - Restore Buttons Only")]
    public void QuickRestoreButtons()
    {
        RestoreTrackBase();
        RestoreButtonImages();
        Debug.Log("⚡ Botones restaurados rápidamente");
    }
    
    [ContextMenu("Quick Fix - Restore Video Only")]
    public void QuickRestoreVideo()
    {
        RestoreBackgroundVideo();
        Debug.Log("⚡ Video de fondo restaurado rápidamente");
    }
    
    /// <summary>
    /// Diagnóstico de elementos faltantes
    /// </summary>
    [ContextMenu("Diagnose Missing Elements")]
    public void DiagnoseMissingElements()
    {
        Debug.Log("🔍 DIAGNÓSTICO DE ELEMENTOS DEL GAMEPLAY:");
        Debug.Log("═══════════════════════════════════════");
        
        GameObject trackBase = GameObject.Find("TrackBase");
        Debug.Log($"🎯 TrackBase: {(trackBase != null ? "✅ EXISTE" : "❌ FALTANTE")}");
        
        if (trackBase != null)
        {
            Transform highway = trackBase.transform.Find("HighwayImage");
            Debug.Log($"🛣️ Highway Image: {(highway != null ? "✅ EXISTE" : "❌ FALTANTE")}");
            
            Transform buttons = trackBase.transform.Find("ButtonContainer");
            Debug.Log($"🎮 Button Container: {(buttons != null ? "✅ EXISTE" : "❌ FALTANTE")}");
            
            if (buttons != null)
            {
                Debug.Log($"🔘 Botones encontrados: {buttons.childCount}");
            }
        }
        
        VideoPlayer videoPlayer = FindFirstObjectByType<VideoPlayer>();
        Debug.Log($"🎬 Video Player: {(videoPlayer != null ? "✅ EXISTE" : "❌ FALTANTE")}");
        
        Canvas canvas = FindFirstObjectByType<Canvas>();
        Debug.Log($"🖼️ Canvas: {(canvas != null ? "✅ EXISTE" : "❌ FALTANTE")}");
        
        Debug.Log("\n💡 Usa 'Restore All Gameplay Elements' para restaurar todo lo faltante");
    }
}
