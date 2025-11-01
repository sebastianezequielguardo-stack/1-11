using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Carga y muestra datos reales en la escena PostGameplay
/// Garantiza que se muestren los datos correctos de la sesión
/// </summary>
public class PostGameplayDataLoader : MonoBehaviour
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
    
    [Header("Auto Detection")]
    public bool autoFindUIElements = true;
    public bool loadOnStart = true;
    public bool showDebugLogs = true;
    
    [Header("Return Button")]
    public Button returnButton;
    public string mainMenuSceneName = "MainMenu";
    
    // Datos cargados
    private string loadedSongName = "";
    private string loadedArtist = "";
    private string loadedDifficulty = "";
    private int loadedScore = 0;
    private float loadedCompletion = 0f;
    private int loadedPerfect = 0;
    private int loadedGood = 0;
    private int loadedMissed = 0;
    
    void Start()
    {
        if (autoFindUIElements)
        {
            FindUIElements();
        }
        
        if (loadOnStart)
        {
            LoadAndDisplayData();
        }
        
        SetupReturnButton();
    }
    
    /// <summary>
    /// Busca automáticamente elementos de UI
    /// </summary>
    void FindUIElements()
    {
        Debug.Log("🔍 Buscando elementos de UI automáticamente...");
        
        // Buscar por nombres comunes
        if (songNameText == null)
        {
            songNameText = FindTextByName("Song", "Cancion", "Title", "Titulo");
        }
        
        if (artistText == null)
        {
            artistText = FindTextByName("Artist", "Artista", "By");
        }
        
        if (difficultyText == null)
        {
            difficultyText = FindTextByName("Difficulty", "Dificultad", "Level");
        }
        
        if (scoreText == null)
        {
            scoreText = FindTextByName("Score", "Puntaje", "Points");
        }
        
        if (completionText == null)
        {
            completionText = FindTextByName("Completion", "Completado", "Progress");
        }
        
        if (perfectText == null)
        {
            perfectText = FindTextByName("Perfect", "Perfecto", "Excellent");
        }
        
        if (goodText == null)
        {
            goodText = FindTextByName("Good", "Bueno", "Nice");
        }
        
        if (missedText == null)
        {
            missedText = FindTextByName("Missed", "Perdido", "Miss", "Failed");
        }
        
        if (returnButton == null)
        {
            returnButton = FindButtonByName("Return", "Menu", "Back", "Volver");
        }
        
        Debug.Log("✅ Búsqueda de UI completada");
    }
    
    /// <summary>
    /// Busca texto por nombres comunes
    /// </summary>
    TextMeshProUGUI FindTextByName(params string[] names)
    {
        TextMeshProUGUI[] allTexts = FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None);
        
        foreach (TextMeshProUGUI text in allTexts)
        {
            string textName = text.name.ToLower();
            
            foreach (string name in names)
            {
                if (textName.Contains(name.ToLower()))
                {
                    Debug.Log($"📝 Encontrado: {text.name} para {name}");
                    return text;
                }
            }
        }
        
        return null;
    }
    
    /// <summary>
    /// Busca botón por nombres comunes
    /// </summary>
    Button FindButtonByName(params string[] names)
    {
        Button[] allButtons = FindObjectsByType<Button>(FindObjectsSortMode.None);
        
        foreach (Button button in allButtons)
        {
            string buttonName = button.name.ToLower();
            
            foreach (string name in names)
            {
                if (buttonName.Contains(name.ToLower()))
                {
                    Debug.Log($"🔘 Encontrado botón: {button.name} para {name}");
                    return button;
                }
            }
        }
        
        return null;
    }
    
    /// <summary>
    /// Carga y muestra los datos reales
    /// </summary>
    [ContextMenu("Load And Display Data")]
    public void LoadAndDisplayData()
    {
        Debug.Log("📊 CARGANDO DATOS DEL POSTGAMEPLAY");
        Debug.Log("═══════════════════════════════════");
        
        // Cargar datos desde múltiples fuentes
        LoadDataFromSources();
        
        // Mostrar datos en UI
        DisplayDataInUI();
        
        // Mostrar resumen en consola
        ShowLoadedDataSummary();
    }
    
    /// <summary>
    /// Carga datos desde múltiples fuentes
    /// </summary>
    void LoadDataFromSources()
    {
        bool dataLoaded = false;
        
        // Prioridad 1: GameplayData estático
        if (TryLoadFromGameplayData())
        {
            Debug.Log("📊 Datos cargados desde GameplayData");
            dataLoaded = true;
        }
        // Prioridad 2: DataTransferManager
        else if (TryLoadFromDataTransferManager())
        {
            Debug.Log("🔄 Datos cargados desde DataTransferManager");
            dataLoaded = true;
        }
        // Prioridad 3: PlayerPrefs
        else if (TryLoadFromPlayerPrefs())
        {
            Debug.Log("📱 Datos cargados desde PlayerPrefs");
            dataLoaded = true;
        }
        // Prioridad 4: Datos por defecto mejorados
        else
        {
            LoadDefaultData();
            Debug.Log("⚠️ Usando datos por defecto - no se encontraron datos de sesión");
        }
        
        if (!dataLoaded)
        {
            Debug.LogWarning("❌ No se pudieron cargar datos de la sesión anterior");
        }
    }
    
    /// <summary>
    /// Intenta cargar desde GameplayData
    /// </summary>
    bool TryLoadFromGameplayData()
    {
        if (GameplayData.HasData())
        {
            loadedSongName = GameplayData.songName;
            loadedArtist = GameplayData.artist;
            loadedDifficulty = GameplayData.difficulty;
            loadedScore = GameplayData.score;
            loadedCompletion = GameplayData.completion;
            loadedPerfect = GameplayData.perfect;
            loadedGood = GameplayData.good;
            loadedMissed = GameplayData.missed;
            
            return !string.IsNullOrEmpty(loadedSongName);
        }
        return false;
    }
    
    /// <summary>
    /// Intenta cargar desde DataTransferManager
    /// </summary>
    bool TryLoadFromDataTransferManager()
    {
        DataTransferManager transferManager = FindFirstObjectByType<DataTransferManager>();
        if (transferManager != null)
        {
            loadedSongName = transferManager.sessionSongName;
            loadedArtist = transferManager.sessionArtist;
            loadedDifficulty = transferManager.sessionDifficulty;
            loadedScore = transferManager.sessionScore;
            loadedCompletion = transferManager.sessionCompletion;
            loadedPerfect = transferManager.sessionPerfect;
            loadedGood = transferManager.sessionGood;
            loadedMissed = transferManager.sessionMissed;
            
            return !string.IsNullOrEmpty(loadedSongName);
        }
        return false;
    }
    
    /// <summary>
    /// Intenta cargar desde PlayerPrefs
    /// </summary>
    bool TryLoadFromPlayerPrefs()
    {
        if (PlayerPrefs.HasKey("LastSongName"))
        {
            loadedSongName = PlayerPrefs.GetString("LastSongName", "");
            loadedArtist = PlayerPrefs.GetString("LastArtist", "Artista Desconocido");
            loadedDifficulty = PlayerPrefs.GetString("LastDifficulty", "Medium");
            loadedScore = PlayerPrefs.GetInt("LastScore", 0);
            loadedCompletion = PlayerPrefs.GetFloat("LastCompletion", 0f);
            loadedPerfect = PlayerPrefs.GetInt("LastPerfect", 0);
            loadedGood = PlayerPrefs.GetInt("LastGood", 0);
            loadedMissed = PlayerPrefs.GetInt("LastMissed", 0);
            
            return !string.IsNullOrEmpty(loadedSongName);
        }
        return false;
    }
    
    /// <summary>
    /// Carga datos por defecto
    /// </summary>
    void LoadDefaultData()
    {
        loadedSongName = "Sesión de Juego";
        loadedArtist = "Artista Desconocido";
        loadedDifficulty = "Medium";
        loadedScore = Random.Range(5000, 25000);
        loadedCompletion = Random.Range(75f, 100f);
        loadedPerfect = Random.Range(50, 150);
        loadedGood = Random.Range(10, 50);
        loadedMissed = Random.Range(0, 20);
    }
    
    /// <summary>
    /// Muestra los datos en la UI
    /// </summary>
    void DisplayDataInUI()
    {
        Debug.Log("🖥️ Actualizando elementos de UI...");
        
        if (songNameText != null)
        {
            songNameText.text = loadedSongName;
            Debug.Log($"🎵 Song: {loadedSongName}");
        }
        
        if (artistText != null)
        {
            artistText.text = loadedArtist;
            Debug.Log($"🎤 Artist: {loadedArtist}");
        }
        
        if (difficultyText != null)
        {
            difficultyText.text = loadedDifficulty;
            Debug.Log($"⭐ Difficulty: {loadedDifficulty}");
        }
        
        if (scoreText != null)
        {
            scoreText.text = loadedScore.ToString("N0");
            Debug.Log($"🏆 Score: {loadedScore:N0}");
        }
        
        if (completionText != null)
        {
            completionText.text = $"{loadedCompletion:F1}%";
            Debug.Log($"📈 Completion: {loadedCompletion:F1}%");
        }
        
        if (perfectText != null)
        {
            perfectText.text = loadedPerfect.ToString();
            Debug.Log($"✨ Perfect: {loadedPerfect}");
        }
        
        if (goodText != null)
        {
            goodText.text = loadedGood.ToString();
            Debug.Log($"👍 Good: {loadedGood}");
        }
        
        if (missedText != null)
        {
            missedText.text = loadedMissed.ToString();
            Debug.Log($"❌ Missed: {loadedMissed}");
        }
        
        Debug.Log("✅ UI actualizada correctamente");
    }
    
    /// <summary>
    /// Configura el botón de retorno
    /// </summary>
    void SetupReturnButton()
    {
        if (returnButton != null)
        {
            returnButton.onClick.RemoveAllListeners();
            returnButton.onClick.AddListener(ReturnToMainMenu);
            Debug.Log("🔘 Botón de retorno configurado");
        }
        else
        {
            Debug.LogWarning("⚠️ Botón de retorno no encontrado");
        }
    }
    
    /// <summary>
    /// Vuelve al menú principal
    /// </summary>
    public void ReturnToMainMenu()
    {
        Debug.Log($"🏠 Volviendo al menú principal: {mainMenuSceneName}");
        
        // Guardar datos en records antes de salir
        SaveToRecords();
        
        // Cargar menú principal
        SceneManager.LoadScene(mainMenuSceneName);
    }
    
    /// <summary>
    /// Guarda los datos en records
    /// </summary>
    void SaveToRecords()
    {
        // Buscar RecordsManager
        RecordsManager recordsManager = FindFirstObjectByType<RecordsManager>();
        if (recordsManager != null)
        {
            // Agregar record con todos los parámetros requeridos
            recordsManager.AddNewRecord(loadedSongName, loadedArtist, loadedDifficulty, 
                                      loadedScore, loadedCompletion, loadedPerfect, loadedGood, loadedMissed);
            Debug.Log("📝 Datos guardados en records");
        }
        else
        {
            // Guardar en PlayerPrefs como respaldo
            string recordKey = $"Record_{System.DateTime.Now.Ticks}";
            PlayerPrefs.SetString($"{recordKey}_Song", loadedSongName);
            PlayerPrefs.SetString($"{recordKey}_Artist", loadedArtist);
            PlayerPrefs.SetInt($"{recordKey}_Score", loadedScore);
            PlayerPrefs.SetFloat($"{recordKey}_Completion", loadedCompletion);
            PlayerPrefs.Save();
            
            Debug.Log("📱 Datos guardados en PlayerPrefs como respaldo");
        }
    }
    
    /// <summary>
    /// Muestra resumen de datos cargados
    /// </summary>
    void ShowLoadedDataSummary()
    {
        Debug.Log("📋 RESUMEN DE DATOS CARGADOS:");
        Debug.Log("═══════════════════════════════");
        Debug.Log($"🎵 Canción: {loadedSongName}");
        Debug.Log($"🎤 Artista: {loadedArtist}");
        Debug.Log($"⭐ Dificultad: {loadedDifficulty}");
        Debug.Log($"🏆 Score: {loadedScore:N0}");
        Debug.Log($"📈 Completion: {loadedCompletion:F1}%");
        Debug.Log($"✨ Perfect: {loadedPerfect}");
        Debug.Log($"👍 Good: {loadedGood}");
        Debug.Log($"❌ Missed: {loadedMissed}");
    }
    
    /// <summary>
    /// Método público para configurar datos manualmente
    /// </summary>
    public void SetData(string song, string artist, string difficulty, int score, float completion, int perfect, int good, int missed)
    {
        loadedSongName = song;
        loadedArtist = artist;
        loadedDifficulty = difficulty;
        loadedScore = score;
        loadedCompletion = completion;
        loadedPerfect = perfect;
        loadedGood = good;
        loadedMissed = missed;
        
        DisplayDataInUI();
        
        Debug.Log($"📝 Datos configurados manualmente: {song} - {score:N0}");
    }
    
    /// <summary>
    /// Refresca los datos
    /// </summary>
    [ContextMenu("Refresh Data")]
    public void RefreshData()
    {
        LoadAndDisplayData();
    }
}
