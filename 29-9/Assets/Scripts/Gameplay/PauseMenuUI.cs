using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Helper script para configurar automáticamente la UI del menú de pausa
/// </summary>
public class PauseMenuUI : MonoBehaviour
{
    [Header("Auto Setup")]
    public bool setupOnStart = true;
    public bool createFromScratch = false;

    [Header("Visual Style")]
    public Color backgroundColor = new Color(0f, 0f, 0f, 0.8f);
    public Color buttonNormalColor = new Color(0.2f, 0.2f, 0.2f, 0.9f);
    public Color buttonHighlightColor = new Color(0.3f, 0.3f, 0.3f, 1f);
    public Color buttonPressedColor = new Color(0.1f, 0.1f, 0.1f, 1f);
    public Color textColor = Color.white;

    [Header("Layout")]
    public Vector2 menuSize = new Vector2(400f, 600f);
    public float buttonHeight = 50f;
    public float buttonSpacing = 10f;

    void Start()
    {
        if (setupOnStart)
        {
            if (createFromScratch)
                CreatePauseMenuFromScratch();
            else
                SetupExistingPauseMenu();
        }
    }

    void CreatePauseMenuFromScratch()
    {
        // Create main canvas if it doesn't exist
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("PauseMenuCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;

            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        // Create pause menu panel
        GameObject pausePanel = CreatePausePanel(canvas.transform);

        // Create background blur
        CreateBackgroundBlur(pausePanel.transform);

        // Create content area
        GameObject contentArea = CreateContentArea(pausePanel.transform);

        // Create song info section
        CreateSongInfoSection(contentArea.transform);

        // Create pause title
        CreatePauseTitle(contentArea.transform);

        // Create menu buttons
        CreateMenuButtons(contentArea.transform);

        // Setup PauseMenu component
        SetupPauseMenuComponent(pausePanel);

        Debug.Log("Pause Menu created from scratch!");
    }

    GameObject CreatePausePanel(Transform parent)
    {
        GameObject panel = new GameObject("PauseMenuPanel");
        panel.transform.SetParent(parent, false);

        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        // Initially inactive
        panel.SetActive(false);

        return panel;
    }

    void CreateBackgroundBlur(Transform parent)
    {
        GameObject blur = new GameObject("BackgroundBlur");
        blur.transform.SetParent(parent, false);

        RectTransform rect = blur.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Image image = blur.AddComponent<Image>();
        image.color = backgroundColor;
    }

    GameObject CreateContentArea(Transform parent)
    {
        GameObject content = new GameObject("ContentArea");
        content.transform.SetParent(parent, false);

        RectTransform rect = content.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = menuSize;
        rect.anchoredPosition = Vector2.zero;

        return content;
    }

    void CreateSongInfoSection(Transform parent)
    {
        // Song info container
        GameObject songInfo = new GameObject("SongInfo");
        songInfo.transform.SetParent(parent, false);

        RectTransform rect = songInfo.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 0.7f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.offsetMin = new Vector2(20f, 0f);
        rect.offsetMax = new Vector2(-20f, 0f);

        // Song title
        GameObject titleObj = new GameObject("SongTitle");
        titleObj.transform.SetParent(songInfo.transform, false);

        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0f, 0.6f);
        titleRect.anchorMax = new Vector2(1f, 1f);
        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;

        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "Song Title";
        titleText.fontSize = 24f;
        titleText.color = textColor;
        titleText.fontStyle = FontStyles.Bold;
        titleText.alignment = TextAlignmentOptions.TopLeft;

        // Artist
        GameObject artistObj = new GameObject("Artist");
        artistObj.transform.SetParent(songInfo.transform, false);

        RectTransform artistRect = artistObj.AddComponent<RectTransform>();
        artistRect.anchorMin = new Vector2(0f, 0.3f);
        artistRect.anchorMax = new Vector2(1f, 0.6f);
        artistRect.offsetMin = Vector2.zero;
        artistRect.offsetMax = Vector2.zero;

        TextMeshProUGUI artistText = artistObj.AddComponent<TextMeshProUGUI>();
        artistText.text = "Artist Name";
        artistText.fontSize = 18f;
        artistText.color = new Color(textColor.r, textColor.g, textColor.b, 0.8f);
        artistText.alignment = TextAlignmentOptions.TopLeft;

        // Current section
        GameObject sectionObj = new GameObject("CurrentSection");
        sectionObj.transform.SetParent(songInfo.transform, false);

        RectTransform sectionRect = sectionObj.AddComponent<RectTransform>();
        sectionRect.anchorMin = new Vector2(0f, 0f);
        sectionRect.anchorMax = new Vector2(1f, 0.3f);
        sectionRect.offsetMin = Vector2.zero;
        sectionRect.offsetMax = Vector2.zero;

        TextMeshProUGUI sectionText = sectionObj.AddComponent<TextMeshProUGUI>();
        sectionText.text = "CURRENT SECTION:\nINTRO";
        sectionText.fontSize = 14f;
        sectionText.color = new Color(textColor.r, textColor.g, textColor.b, 0.7f);
        sectionText.alignment = TextAlignmentOptions.TopLeft;
    }

    void CreatePauseTitle(Transform parent)
    {
        GameObject titleObj = new GameObject("PauseTitle");
        titleObj.transform.SetParent(parent, false);

        RectTransform rect = titleObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.7f, 0.8f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "PAUSA";
        titleText.fontSize = 36f;
        titleText.color = textColor;
        titleText.fontStyle = FontStyles.Bold;
        titleText.alignment = TextAlignmentOptions.Center;
    }

    void CreateMenuButtons(Transform parent)
    {
        string[] buttonNames = { "CONTINUAR", "REINICIAR", "NUEVA CANCIÓN", "PRÁCTICA", "OPCIONES", "OPCIONES DE CANCIÓN", "SALIR" };
        string[] buttonIds = { "Resume", "Restart", "MainMenu", "Practice", "Options", "SongOptions", "Exit" };

        GameObject buttonContainer = new GameObject("ButtonContainer");
        buttonContainer.transform.SetParent(parent, false);

        RectTransform containerRect = buttonContainer.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0.5f, 0f);
        containerRect.anchorMax = new Vector2(1f, 0.7f);
        containerRect.offsetMin = Vector2.zero;
        containerRect.offsetMax = Vector2.zero;

        // Add vertical layout group
        VerticalLayoutGroup layoutGroup = buttonContainer.AddComponent<VerticalLayoutGroup>();
        layoutGroup.spacing = buttonSpacing;
        layoutGroup.padding = new RectOffset(20, 20, 20, 20);
        layoutGroup.childAlignment = TextAnchor.UpperCenter;
        layoutGroup.childControlHeight = false;
        layoutGroup.childControlWidth = true;
        layoutGroup.childForceExpandHeight = false;
        layoutGroup.childForceExpandWidth = true;

        for (int i = 0; i < buttonNames.Length; i++)
        {
            CreateMenuButton(buttonContainer.transform, buttonNames[i], buttonIds[i]);
        }
    }

    void CreateMenuButton(Transform parent, string buttonText, string buttonId)
    {
        GameObject buttonObj = new GameObject($"Button_{buttonId}");
        buttonObj.transform.SetParent(parent, false);

        RectTransform rect = buttonObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(0f, buttonHeight);

        // Button component
        Button button = buttonObj.AddComponent<Button>();

        // Button image
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = buttonNormalColor;

        // Button colors
        ColorBlock colors = button.colors;
        colors.normalColor = buttonNormalColor;
        colors.highlightedColor = buttonHighlightColor;
        colors.pressedColor = buttonPressedColor;
        colors.selectedColor = buttonHighlightColor;
        button.colors = colors;

        // Button text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        TextMeshProUGUI buttonTextComp = textObj.AddComponent<TextMeshProUGUI>();
        buttonTextComp.text = buttonText;
        buttonTextComp.fontSize = 16f;
        buttonTextComp.color = textColor;
        buttonTextComp.alignment = TextAlignmentOptions.Center;

        // Special styling for selected button (SALIR in the example)
        if (buttonId == "Exit")
        {
            buttonImage.color = new Color(0.4f, 0.4f, 0.4f, 1f);
        }
    }

    void SetupPauseMenuComponent(GameObject pausePanel)
    {
        PauseMenu pauseMenu = pausePanel.GetComponent<PauseMenu>();
        if (pauseMenu == null)
        {
            pauseMenu = pausePanel.AddComponent<PauseMenu>();
        }

        // Auto-assign references
        pauseMenu.pauseMenuPanel = pausePanel;
        pauseMenu.backgroundBlur = pausePanel.transform.Find("BackgroundBlur")?.GetComponent<Image>();

        Transform contentArea = pausePanel.transform.Find("ContentArea");
        if (contentArea != null)
        {
            Transform songInfo = contentArea.Find("SongInfo");
            if (songInfo != null)
            {
                pauseMenu.songTitleText = songInfo.Find("SongTitle")?.GetComponent<TextMeshProUGUI>();
                pauseMenu.artistText = songInfo.Find("Artist")?.GetComponent<TextMeshProUGUI>();
                pauseMenu.currentSectionText = songInfo.Find("CurrentSection")?.GetComponent<TextMeshProUGUI>();
            }

            Transform buttonContainer = contentArea.Find("ButtonContainer");
            if (buttonContainer != null)
            {
                pauseMenu.resumeButton = buttonContainer.Find("Button_Resume")?.GetComponent<Button>();
                pauseMenu.restartButton = buttonContainer.Find("Button_Restart")?.GetComponent<Button>();
                pauseMenu.mainMenuButton = buttonContainer.Find("Button_MainMenu")?.GetComponent<Button>();
                pauseMenu.optionsButton = buttonContainer.Find("Button_Options")?.GetComponent<Button>();
                pauseMenu.songOptionsButton = buttonContainer.Find("Button_SongOptions")?.GetComponent<Button>();
                pauseMenu.exitButton = buttonContainer.Find("Button_Exit")?.GetComponent<Button>();
            }
        }

        Debug.Log("PauseMenu component configured!");
    }

    void SetupExistingPauseMenu()
    {
        // Apply visual styling to existing pause menu elements
        Debug.Log("Setting up existing pause menu styling...");
    }
}
