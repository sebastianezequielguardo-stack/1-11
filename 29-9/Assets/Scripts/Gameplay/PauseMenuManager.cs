using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Maneja el menú de pausa visual y funcional
/// </summary>
public class PauseMenuManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pauseMenuPanel;
    public Button resumeButton;
    public Button restartButton;
    public Button mainMenuButton;
    public Text pauseTitle;
    
    [Header("Auto Setup")]
    public bool createUIAutomatically = true;
    public string mainMenuSceneName = "MainMenu";
    
    private GameplayManager gameplayManager;
    private Canvas gameCanvas;
    // Variable menuCreated eliminada - no se utilizaba
    
    void Start()
    {
        gameplayManager = GameplayManager.Instance;
        
        if (createUIAutomatically && pauseMenuPanel == null)
        {
            CreatePauseMenuUI();
        }
        
        // Inicialmente ocultar el menú
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
        
        // PauseMenuManager iniciado
    }
    
    void Update()
    {
        // Verificar estado de pausa y mostrar/ocultar menú
        if (gameplayManager != null)
        {
            bool shouldShowMenu = gameplayManager.isPaused;
            
            if (pauseMenuPanel != null && pauseMenuPanel.activeSelf != shouldShowMenu)
            {
                pauseMenuPanel.SetActive(shouldShowMenu);
                
                if (shouldShowMenu)
                {
                    // Menú mostrado
                }
                else
                {
                    // Menú ocultado
                }
            }
        }
    }
    
    void CreatePauseMenuUI()
    {
        // Buscar o crear Canvas
        gameCanvas = FindFirstObjectByType<Canvas>();
        if (gameCanvas == null)
        {
            GameObject canvasObj = new GameObject("GameCanvas");
            gameCanvas = canvasObj.AddComponent<Canvas>();
            gameCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }
        
        // Crear panel principal
        GameObject panelObj = new GameObject("PauseMenuPanel");
        panelObj.transform.SetParent(gameCanvas.transform, false);
        
        // Configurar panel
        pauseMenuPanel = panelObj;
        Image panelImage = panelObj.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.8f); // Fondo semi-transparente
        
        RectTransform panelRect = panelObj.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        
        // Crear contenedor central
        GameObject containerObj = new GameObject("Container");
        containerObj.transform.SetParent(panelObj.transform, false);
        
        RectTransform containerRect = containerObj.GetComponent<RectTransform>();
        containerRect.anchoredPosition = Vector2.zero;
        containerRect.sizeDelta = new Vector2(400, 300);
        
        // Crear fondo del contenedor
        Image containerImage = containerObj.AddComponent<Image>();
        containerImage.color = new Color(0.2f, 0.2f, 0.2f, 0.9f);
        
        // Crear título
        CreateTitle(containerObj);
        
        // Crear botones
        CreateButtons(containerObj);
        
        // Menú creado automáticamente
    }
    
    void CreateTitle(GameObject parent)
    {
        GameObject titleObj = new GameObject("PauseTitle");
        titleObj.transform.SetParent(parent.transform, false);
        
        pauseTitle = titleObj.AddComponent<Text>();
        pauseTitle.text = "JUEGO PAUSADO";
        pauseTitle.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        pauseTitle.fontSize = 32;
        pauseTitle.color = Color.white;
        pauseTitle.alignment = TextAnchor.MiddleCenter;
        pauseTitle.fontStyle = FontStyle.Bold;
        
        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchoredPosition = new Vector2(0, 80);
        titleRect.sizeDelta = new Vector2(350, 50);
    }
    
    void CreateButtons(GameObject parent)
    {
        // Botón Reanudar
        resumeButton = CreateButton(parent, "Reanudar", new Vector2(0, 20), OnResumeClicked);
        
        // Botón Reiniciar
        restartButton = CreateButton(parent, "Reiniciar", new Vector2(0, -30), OnRestartClicked);
        
        // Botón Menú Principal
        mainMenuButton = CreateButton(parent, "Menú Principal", new Vector2(0, -80), OnMainMenuClicked);
    }
    
    Button CreateButton(GameObject parent, string text, Vector2 position, UnityEngine.Events.UnityAction onClick)
    {
        GameObject buttonObj = new GameObject(text + "Button");
        buttonObj.transform.SetParent(parent.transform, false);
        
        // Configurar RectTransform
        RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
        buttonRect.anchoredPosition = position;
        buttonRect.sizeDelta = new Vector2(200, 40);
        
        // Agregar componentes del botón
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(0.4f, 0.4f, 0.4f, 1f);
        
        Button button = buttonObj.AddComponent<Button>();
        button.targetGraphic = buttonImage;
        
        // Configurar colores del botón
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.4f, 0.4f, 0.4f, 1f);
        colors.highlightedColor = new Color(0.6f, 0.6f, 0.6f, 1f);
        colors.pressedColor = new Color(0.2f, 0.2f, 0.2f, 1f);
        button.colors = colors;
        
        // Crear texto del botón
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        
        Text buttonText = textObj.AddComponent<Text>();
        buttonText.text = text;
        buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        buttonText.fontSize = 18;
        buttonText.color = Color.white;
        buttonText.alignment = TextAnchor.MiddleCenter;
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        // Agregar evento
        button.onClick.AddListener(onClick);
        
        return button;
    }
    
    public void OnResumeClicked()
    {
        Debug.Log("▶️ Botón Reanudar presionado");
        if (gameplayManager != null)
        {
            gameplayManager.ResumeGame();
        }
    }
    
    public void OnRestartClicked()
    {
        Debug.Log("🔄 Botón Reiniciar presionado");
        Time.timeScale = 1f; // Restaurar tiempo antes de recargar
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void OnMainMenuClicked()
    {
        Debug.Log("🏠 Botón Menú Principal presionado");
        Time.timeScale = 1f; // Restaurar tiempo antes de cambiar escena
        SceneManager.LoadScene(mainMenuSceneName);
    }
    
    // Método público para mostrar/ocultar manualmente
    public void ShowPauseMenu(bool show)
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(show);
        }
    }
    
    // Método para verificar si el menú está visible
    public bool IsMenuVisible()
    {
        return pauseMenuPanel != null && pauseMenuPanel.activeSelf;
    }
}
