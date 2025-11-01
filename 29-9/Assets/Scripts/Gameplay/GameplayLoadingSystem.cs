using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Sistema completo de carga para el gameplay
/// Muestra pantalla negra con contador de 3 segundos
/// </summary>
public class GameplayLoadingSystem : MonoBehaviour
{
    [Header("Loading Configuration")]
    public bool autoStartOnAwake = true;
    public float countdownDuration = 3f;
    public Color backgroundColor = Color.black;
    public Color textColor = Color.white;
    public float textSize = 120f;
    
    [Header("Animation")]
    public bool animateNumbers = true;
    public float scaleMultiplier = 1.5f;
    public float animationSpeed = 0.3f;
    
    // UI Components
    private Canvas loadingCanvas;
    private Image backgroundImage;
    private TextMeshProUGUI countdownText;
    
    // State
    private bool isLoading = false;
    
    void Awake()
    {
        if (autoStartOnAwake)
        {
            StartLoadingSequence();
        }
    }
    
    /// <summary>
    /// Inicia la secuencia de carga
    /// </summary>
    [ContextMenu("Start Loading")]
    public void StartLoadingSequence()
    {
        if (isLoading) return;
        
        Debug.Log("🎬 Iniciando pantalla de carga del gameplay");
        
        CreateLoadingUI();
        StartCoroutine(LoadingCountdown());
    }
    
    /// <summary>
    /// Crea la interfaz de carga
    /// </summary>
    void CreateLoadingUI()
    {
        // Crear Canvas principal
        GameObject canvasObj = new GameObject("GameplayLoadingCanvas");
        canvasObj.transform.SetParent(transform);
        
        loadingCanvas = canvasObj.AddComponent<Canvas>();
        loadingCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        loadingCanvas.sortingOrder = 1000; // Muy alto para estar encima de todo
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        canvasObj.AddComponent<GraphicRaycaster>();
        
        // Crear fondo negro
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(canvasObj.transform, false);
        
        backgroundImage = bgObj.AddComponent<Image>();
        backgroundImage.color = backgroundColor;
        
        // Configurar para pantalla completa
        RectTransform bgRect = bgObj.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        bgRect.anchoredPosition = Vector2.zero;
        
        // Crear texto del contador
        GameObject textObj = new GameObject("CountdownText");
        textObj.transform.SetParent(canvasObj.transform, false);
        
        countdownText = textObj.AddComponent<TextMeshProUGUI>();
        countdownText.text = "";
        countdownText.fontSize = textSize;
        countdownText.color = textColor;
        countdownText.alignment = TextAlignmentOptions.Center;
        countdownText.fontStyle = FontStyles.Bold;
        
        // Outline para mejor visibilidad
        countdownText.outlineWidth = 0.2f;
        countdownText.outlineColor = Color.black;
        
        // Centrar el texto
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.5f, 0.5f);
        textRect.anchorMax = new Vector2(0.5f, 0.5f);
        textRect.sizeDelta = new Vector2(400, 200);
        textRect.anchoredPosition = Vector2.zero;
        
        Debug.Log("✅ UI de carga creada");
    }
    
    /// <summary>
    /// Ejecuta el countdown de carga
    /// </summary>
    IEnumerator LoadingCountdown()
    {
        isLoading = true;
        
        // Pausar el juego durante la carga
        Time.timeScale = 0f;
        
        Debug.Log("🔢 Iniciando countdown de carga");
        
        // Countdown de 3 a 1
        for (int i = 3; i >= 1; i--)
        {
            if (countdownText != null)
            {
                countdownText.text = i.ToString();
                
                if (animateNumbers)
                {
                    yield return StartCoroutine(AnimateNumber());
                }
                else
                {
                    yield return new WaitForSecondsRealtime(1f);
                }
            }
        }
        
        // Limpiar texto
        if (countdownText != null)
        {
            countdownText.text = "";
        }
        
        yield return new WaitForSecondsRealtime(0.2f);
        
        // Finalizar carga
        FinishLoading();
    }
    
    /// <summary>
    /// Anima los números del countdown
    /// </summary>
    IEnumerator AnimateNumber()
    {
        if (countdownText == null) yield break;
        
        Vector3 originalScale = countdownText.transform.localScale;
        Vector3 targetScale = originalScale * scaleMultiplier;
        
        float elapsed = 0f;
        
        // Escalar hacia arriba
        while (elapsed < animationSpeed)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / animationSpeed;
            
            if (countdownText != null)
            {
                countdownText.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            }
            
            yield return null;
        }
        
        // Mantener un momento
        yield return new WaitForSecondsRealtime(0.4f);
        
        elapsed = 0f;
        
        // Escalar hacia abajo
        while (elapsed < animationSpeed)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / animationSpeed;
            
            if (countdownText != null)
            {
                countdownText.transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            }
            
            yield return null;
        }
        
        if (countdownText != null)
        {
            countdownText.transform.localScale = originalScale;
        }
        
        yield return new WaitForSecondsRealtime(0.3f);
    }
    
    /// <summary>
    /// Finaliza la carga e inicia el gameplay
    /// </summary>
    void FinishLoading()
    {
        Debug.Log("🎮 Finalizando carga, iniciando gameplay");
        
        // Restaurar time scale
        Time.timeScale = 1f;
        
        // Fade out y destruir UI
        StartCoroutine(FadeOutAndDestroy());
        
        // Notificar que el gameplay puede comenzar
        NotifyGameplayStart();
        
        isLoading = false;
    }
    
    /// <summary>
    /// Fade out de la pantalla de carga
    /// </summary>
    IEnumerator FadeOutAndDestroy()
    {
        if (loadingCanvas == null) yield break;
        
        float fadeDuration = 0.5f;
        float elapsed = 0f;
        
        Color originalBg = backgroundColor;
        Color originalText = textColor;
        
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            
            if (backgroundImage != null)
            {
                backgroundImage.color = new Color(originalBg.r, originalBg.g, originalBg.b, alpha);
            }
            
            if (countdownText != null)
            {
                countdownText.color = new Color(originalText.r, originalText.g, originalText.b, alpha);
            }
            
            yield return null;
        }
        
        // Destruir la UI
        if (loadingCanvas != null)
        {
            Destroy(loadingCanvas.gameObject);
        }
        
        Debug.Log("✅ Pantalla de carga eliminada");
    }
    
    /// <summary>
    /// Notifica a otros sistemas que el gameplay ha comenzado
    /// </summary>
    void NotifyGameplayStart()
    {
        // Buscar GameplayManager y notificar
        GameplayManager gameplayManager = FindFirstObjectByType<GameplayManager>();
        if (gameplayManager != null)
        {
            // Si tiene método para iniciar, llamarlo
            gameplayManager.SendMessage("OnLoadingComplete", SendMessageOptions.DontRequireReceiver);
        }
        
        // Buscar AudioSource y asegurar que esté reproduciéndose
        AudioSource audioSource = FindFirstObjectByType<AudioSource>();
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
            Debug.Log("🎵 Audio iniciado después de la carga");
        }
        
        Debug.Log("📢 Gameplay iniciado correctamente");
    }
    
    /// <summary>
    /// Fuerza el fin de la carga (para emergencias)
    /// </summary>
    [ContextMenu("Force Finish Loading")]
    public void ForceFinishLoading()
    {
        if (isLoading)
        {
            StopAllCoroutines();
            FinishLoading();
        }
    }
    
    /// <summary>
    /// Verifica si está cargando
    /// </summary>
    public bool IsLoading()
    {
        return isLoading;
    }
}
