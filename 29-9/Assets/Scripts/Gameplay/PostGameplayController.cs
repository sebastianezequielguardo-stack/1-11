using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Maneja la escena PostGameplay completa
/// Carga datos reales, actualiza UI y maneja navegación
/// </summary>
public class PostGameplayController : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI songNameText;
    public TextMeshProUGUI artistText;
    public TextMeshProUGUI difficultyText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI completionText;
    public TextMeshProUGUI perfectText;
    public TextMeshProUGUI goodText;
    public TextMeshProUGUI missedText;
    public Button returnButton;
    
    [Header("Auto Detection")]
    public bool autoFindUIElements = true;
    public bool showDebugLogs = true;
    
    [Header("Data")]
    public GameResultsData gameResults;
    
    // Diccionario de traducciones al español
    private Dictionary<string, string> difficultyTranslations = new Dictionary<string, string>()
    {
        {"Easy", "Fácil"},
        {"Medium", "Medio"},
        {"Hard", "Difícil"},
        {"Expert", "Experto"},
        {"Master", "Maestro"},
        {"Beginner", "Principiante"},
        {"Normal", "Normal"},
        {"Extreme", "Extremo"}
    };
    
    void Start()
    {
        InitializePostGameplay();
    }
    
    /// <summary>
    /// Inicializa el sistema PostGameplay
    /// </summary>
    void InitializePostGameplay()
    {
        if (showDebugLogs)
        {
            Debug.Log("🎯 Inicializando PostGameplay Controller");
        }
        
        // Auto-detectar elementos UI si está habilitado
        if (autoFindUIElements)
        {
            FindUIElements();
        }
        
        // Cargar datos del gameplay
        LoadGameplayData();
        
        // Actualizar UI con los datos
        UpdateUI();
        
        // Configurar botón de retorno
        SetupReturnButton();
        
        if (showDebugLogs)
        {
            Debug.Log("✅ PostGameplay Controller inicializado correctamente");
        }
    }
    
    /// <summary>
    /// Busca automáticamente elementos UI en la escena
    /// </summary>
    void FindUIElements()
    {
        if (showDebugLogs)
        {
            Debug.Log("🔍 Buscando elementos UI automáticamente...");
        }
        
        // Buscar elementos por nombre
        if (songNameText == null)
            songNameText = GameObject.Find("SongNameText")?.GetComponent<TextMeshProUGUI>();
            
        if (artistText == null)
            artistText = GameObject.Find("ArtistText")?.GetComponent<TextMeshProUGUI>();
            
        if (difficultyText == null)
            difficultyText = GameObject.Find("DifficultyText")?.GetComponent<TextMeshProUGUI>();
            
        if (scoreText == null)
            scoreText = GameObject.Find("ScoreText")?.GetComponent<TextMeshProUGUI>();
            
        if (completionText == null)
            completionText = GameObject.Find("CompletionText")?.GetComponent<TextMeshProUGUI>();
            
        if (perfectText == null)
            perfectText = GameObject.Find("PerfectText")?.GetComponent<TextMeshProUGUI>();
            
        if (goodText == null)
            goodText = GameObject.Find("GoodText")?.GetComponent<TextMeshProUGUI>();
            
        if (missedText == null)
            missedText = GameObject.Find("MissedText")?.GetComponent<TextMeshProUGUI>();
            
        if (returnButton == null)
            returnButton = GameObject.Find("ReturnButton")?.GetComponent<Button>();
        
        if (showDebugLogs)
        {
            Debug.Log("✅ Búsqueda de elementos UI completada");
        }
    }
    
    /// <summary>
    /// Carga los datos del gameplay desde GameResultsData
    /// </summary>
    void LoadGameplayData()
    {
        // Buscar GameResultsData en la escena
        gameResults = FindFirstObjectByType<GameResultsData>();
        
        if (gameResults == null)
        {
            if (showDebugLogs)
            {
                Debug.LogWarning("⚠️ No se encontró GameResultsData, creando datos de prueba");
            }
            CreateTestData();
        }
        else
        {
            if (showDebugLogs)
            {
                Debug.Log($"✅ Datos cargados: {gameResults.songName} - Score: {gameResults.score}");
            }
        }
    }
    
    /// <summary>
    /// Crea datos de prueba para testing
    /// </summary>
    void CreateTestData()
    {
        GameObject testDataObj = new GameObject("TestGameResults");
        gameResults = testDataObj.AddComponent<GameResultsData>();
        
        gameResults.songName = "Canción de Prueba";
        gameResults.artist = "Artista de Prueba";
        gameResults.difficulty = "Medium";
        gameResults.score = 85000;
        gameResults.accuracy = 87.5f;
        gameResults.completionPercentage = 92.3f;
        gameResults.perfectHits = 150;
        gameResults.goodHits = 45;
        gameResults.okHits = 20;
        gameResults.missedHits = 35;
        gameResults.totalNotes = 250;
    }
    
    /// <summary>
    /// Actualiza todos los elementos UI con los datos
    /// </summary>
    void UpdateUI()
    {
        if (gameResults == null) return;
        
        // Actualizar información de la canción
        if (songNameText != null)
            songNameText.text = gameResults.songName;
            
        if (artistText != null)
            artistText.text = $"por {gameResults.artist}";
            
        if (difficultyText != null)
        {
            string spanishDifficulty = TranslateDifficulty(gameResults.difficulty);
            difficultyText.text = $"Dificultad: {spanishDifficulty}";
        }
        
        // Actualizar estadísticas de rendimiento
        if (scoreText != null)
            scoreText.text = $"Puntuación: {gameResults.score:N0}";
            
        if (completionText != null)
            completionText.text = $"Completado: {gameResults.completionPercentage:F1}%";
            
        if (perfectText != null)
            perfectText.text = $"Perfectos: {gameResults.perfectHits}";
            
        if (goodText != null)
            goodText.text = $"Buenos: {gameResults.goodHits}";
            
        if (missedText != null)
            missedText.text = $"Fallados: {gameResults.missedHits}";
        
        if (showDebugLogs)
        {
            Debug.Log("✅ UI actualizada con los datos del gameplay");
        }
    }
    
    /// <summary>
    /// Traduce la dificultad al español
    /// </summary>
    string TranslateDifficulty(string englishDifficulty)
    {
        if (difficultyTranslations.ContainsKey(englishDifficulty))
        {
            return difficultyTranslations[englishDifficulty];
        }
        
        return englishDifficulty;
    }
    
    /// <summary>
    /// Configura el botón de retorno al menú
    /// </summary>
    void SetupReturnButton()
    {
        if (returnButton != null)
        {
            returnButton.onClick.AddListener(ReturnToMainMenu);
            
            if (showDebugLogs)
            {
                Debug.Log("✅ Botón de retorno configurado");
            }
        }
        else if (showDebugLogs)
        {
            Debug.LogWarning("⚠️ No se encontró botón de retorno");
        }
    }
    
    /// <summary>
    /// Regresa al menú principal
    /// </summary>
    public void ReturnToMainMenu()
    {
        if (showDebugLogs)
        {
            Debug.Log("🔄 Regresando al menú principal...");
        }
        
        // Limpiar datos del gameplay
        if (gameResults != null && gameResults.gameObject != null)
        {
            Destroy(gameResults.gameObject);
        }
        
        // Cargar escena del menú principal
        SceneManager.LoadScene("MainMenu");
    }
    
    /// <summary>
    /// Información en pantalla para debug
    /// </summary>
    void OnGUI()
    {
        if (!showDebugLogs) return;
        
        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        
        GUILayout.Label("🎯 POST GAMEPLAY CONTROLLER", GUI.skin.box);
        GUILayout.Space(5);
        
        if (gameResults != null)
        {
            GUILayout.Label($"Canción: {gameResults.songName}");
            GUILayout.Label($"Artista: {gameResults.artist}");
            GUILayout.Label($"Score: {gameResults.score:N0}");
            GUILayout.Label($"Accuracy: {gameResults.accuracy:F1}%");
        }
        else
        {
            GUILayout.Label("Sin datos de gameplay");
        }
        
        GUILayout.Space(10);
        if (GUILayout.Button("🔄 Volver al Menú"))
        {
            ReturnToMainMenu();
        }
        
        GUILayout.EndArea();
    }
}
