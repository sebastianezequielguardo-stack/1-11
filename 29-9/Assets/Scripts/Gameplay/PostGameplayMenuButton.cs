using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Configura automáticamente el botón "Return to Menu" en PostGameplay
/// para que vuelva a la escena MainMenu
/// </summary>
public class PostGameplayMenuButton : MonoBehaviour
{
    [Header("Scene Configuration")]
    public string mainMenuSceneName = "MainMenu";
    public bool setupOnStart = true;
    
    [Header("Button Detection")]
    public Button returnToMenuButton;
    public bool autoFindButton = true;
    
    [Header("Debug")]
    public bool showDebugInfo = true;
    
    void Start()
    {
        if (setupOnStart)
        {
            SetupReturnToMenuButton();
        }
    }
    
    /// <summary>
    /// Configura automáticamente el botón Return to Menu
    /// </summary>
    [ContextMenu("Setup Return to Menu Button")]
    public void SetupReturnToMenuButton()
    {
        Debug.Log("🔧 Configurando botón Return to Menu...");
        
        // Buscar botón automáticamente si no está asignado
        if (returnToMenuButton == null && autoFindButton)
        {
            FindReturnToMenuButton();
        }
        
        if (returnToMenuButton != null)
        {
            // Limpiar listeners existentes
            returnToMenuButton.onClick.RemoveAllListeners();
            
            // Agregar función para volver al menú
            returnToMenuButton.onClick.AddListener(ReturnToMainMenu);
            
            if (showDebugInfo)
            {
                Debug.Log($"✅ Botón '{returnToMenuButton.name}' configurado para volver a {mainMenuSceneName}");
            }
        }
        else
        {
            Debug.LogError("❌ No se encontró botón Return to Menu");
            Debug.LogError("💡 Asigna manualmente el botón o verifica que exista en la escena");
        }
    }
    
    /// <summary>
    /// Busca automáticamente el botón Return to Menu
    /// </summary>
    void FindReturnToMenuButton()
    {
        Debug.Log("🔍 Buscando botón Return to Menu automáticamente...");
        
        // Buscar todos los botones en la escena
        Button[] allButtons = FindObjectsByType<Button>(FindObjectsSortMode.None);
        
        foreach (Button button in allButtons)
        {
            string buttonName = button.name.ToLower();
            
            // Buscar por nombre del GameObject
            if (buttonName.Contains("return") && buttonName.Contains("menu") ||
                buttonName.Contains("back") && buttonName.Contains("menu") ||
                buttonName.Contains("main") && buttonName.Contains("menu") ||
                buttonName.Contains("menu") ||
                buttonName.Contains("home") ||
                buttonName.Contains("exit"))
            {
                returnToMenuButton = button;
                Debug.Log($"✅ Botón encontrado por nombre: {button.name}");
                return;
            }
            
            // Buscar por texto del botón
            Text buttonText = button.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                string text = buttonText.text.ToLower();
                if (text.Contains("return") && text.Contains("menu") ||
                    text.Contains("back") && text.Contains("menu") ||
                    text.Contains("main") && text.Contains("menu") ||
                    text.Contains("menu") ||
                    text.Contains("home") ||
                    text.Contains("exit"))
                {
                    returnToMenuButton = button;
                    Debug.Log($"✅ Botón encontrado por texto: {button.name} ('{buttonText.text}')");
                    return;
                }
            }
            
            // Buscar por TextMeshPro
            TMPro.TextMeshProUGUI tmpText = button.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (tmpText != null)
            {
                string text = tmpText.text.ToLower();
                if (text.Contains("return") && text.Contains("menu") ||
                    text.Contains("back") && text.Contains("menu") ||
                    text.Contains("main") && text.Contains("menu") ||
                    text.Contains("menu") ||
                    text.Contains("home") ||
                    text.Contains("exit"))
                {
                    returnToMenuButton = button;
                    Debug.Log($"✅ Botón encontrado por TextMeshPro: {button.name} ('{tmpText.text}')");
                    return;
                }
            }
        }
        
        Debug.LogWarning("⚠️ No se encontró botón Return to Menu automáticamente");
        Debug.LogWarning("💡 Asigna manualmente el botón en el Inspector");
    }
    
    /// <summary>
    /// Función que se ejecuta cuando se presiona el botón
    /// </summary>
    public void ReturnToMainMenu()
    {
        if (showDebugInfo)
        {
            Debug.Log($"🏠 Volviendo al menú principal: {mainMenuSceneName}");
        }
        
        // Verificar si la escena existe
        if (DoesSceneExist(mainMenuSceneName))
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
        else
        {
            Debug.LogError($"❌ Escena '{mainMenuSceneName}' no encontrada");
            TryAlternativeMainMenuScenes();
        }
    }
    
    /// <summary>
    /// Verifica si una escena existe en Build Settings
    /// </summary>
    bool DoesSceneExist(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneNameFromPath = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            
            if (sceneNameFromPath.Equals(sceneName, System.StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }
    
    /// <summary>
    /// Intenta nombres alternativos para MainMenu
    /// </summary>
    void TryAlternativeMainMenuScenes()
    {
        string[] alternativeNames = {
            "MainMenu",
            "Main Menu",
            "Menu",
            "StartMenu",
            "Start Menu",
            "Home",
            "Title",
            "TitleScreen",
            "Lobby"
        };
        
        foreach (string altName in alternativeNames)
        {
            if (DoesSceneExist(altName))
            {
                Debug.Log($"🔄 Usando escena alternativa: {altName}");
                SceneManager.LoadScene(altName);
                return;
            }
        }
        
        Debug.LogError("❌ No se encontró ninguna escena de MainMenu");
        Debug.LogError("💡 Agrega la escena MainMenu a Build Settings");
    }
    
    /// <summary>
    /// Configurar manualmente el nombre de la escena MainMenu
    /// </summary>
    public void SetMainMenuSceneName(string sceneName)
    {
        mainMenuSceneName = sceneName;
        Debug.Log($"🎯 Escena MainMenu configurada: {sceneName}");
    }
    
    /// <summary>
    /// Mostrar información del sistema
    /// </summary>
    [ContextMenu("Show Button Info")]
    public void ShowButtonInfo()
    {
        Debug.Log("📊 INFORMACIÓN DEL BOTÓN RETURN TO MENU:");
        Debug.Log("═══════════════════════════════════════");
        
        Debug.Log($"Escena objetivo: {mainMenuSceneName}");
        Debug.Log($"Escena existe: {(DoesSceneExist(mainMenuSceneName) ? "✅ SÍ" : "❌ NO")}");
        
        if (returnToMenuButton != null)
        {
            Debug.Log($"Botón asignado: ✅ {returnToMenuButton.name}");
            Debug.Log($"Botón activo: {(returnToMenuButton.gameObject.activeInHierarchy ? "✅ SÍ" : "❌ NO")}");
            Debug.Log($"Botón interactuable: {(returnToMenuButton.interactable ? "✅ SÍ" : "❌ NO")}");
            
            // Mostrar texto del botón
            Text buttonText = returnToMenuButton.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                Debug.Log($"Texto del botón: '{buttonText.text}'");
            }
            
            TMPro.TextMeshProUGUI tmpText = returnToMenuButton.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (tmpText != null)
            {
                Debug.Log($"TextMeshPro del botón: '{tmpText.text}'");
            }
        }
        else
        {
            Debug.Log("Botón asignado: ❌ NO");
        }
        
        // Mostrar todos los botones disponibles
        Button[] allButtons = FindObjectsByType<Button>(FindObjectsSortMode.None);
        Debug.Log($"\n🔘 Botones disponibles en la escena: {allButtons.Length}");
        
        foreach (Button btn in allButtons)
        {
            string text = "";
            Text textComp = btn.GetComponentInChildren<Text>();
            if (textComp != null) text = textComp.text;
            
            TMPro.TextMeshProUGUI tmpComp = btn.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (tmpComp != null) text = tmpComp.text;
            
            Debug.Log($"  🔘 {btn.name} {(string.IsNullOrEmpty(text) ? "" : $"('{text}')")}");
        }
    }
    
    /// <summary>
    /// Configurar todos los botones de menú automáticamente
    /// </summary>
    [ContextMenu("Setup All Menu Buttons")]
    public void SetupAllMenuButtons()
    {
        Debug.Log("🔧 Configurando todos los botones de menú...");
        
        Button[] allButtons = FindObjectsByType<Button>(FindObjectsSortMode.None);
        int buttonsConfigured = 0;
        
        foreach (Button button in allButtons)
        {
            string buttonName = button.name.ToLower();
            
            // Configurar botones que parezcan de menú
            if (buttonName.Contains("menu") || 
                buttonName.Contains("home") || 
                buttonName.Contains("return") ||
                buttonName.Contains("back") ||
                buttonName.Contains("exit"))
            {
                // Limpiar listeners existentes
                button.onClick.RemoveAllListeners();
                
                // Agregar función para volver al menú
                button.onClick.AddListener(ReturnToMainMenu);
                
                buttonsConfigured++;
                Debug.Log($"✅ Botón configurado: {button.name}");
            }
        }
        
        Debug.Log($"✅ {buttonsConfigured} botones configurados para volver al MainMenu");
    }
    
    /// <summary>
    /// Test manual del botón
    /// </summary>
    [ContextMenu("Test Return to Menu")]
    public void TestReturnToMenu()
    {
        Debug.Log("🧪 Probando Return to Menu...");
        ReturnToMainMenu();
    }
}
