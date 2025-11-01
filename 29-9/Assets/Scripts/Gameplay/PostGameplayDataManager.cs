using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Gestiona los datos reales del PostGameplay
/// Reemplaza los datos de prueba con información real del gameplay
/// </summary>
public class PostGameplayDataManager : MonoBehaviour
{
    [Header("UI Elements - Text")]
    public Text songNameText;
    public Text artistText;
    public Text difficultyText;
    public Text scoreText;
    public Text completionText;
    public Text perfectText;
    public Text goodText;
    public Text missedText;
    
    [Header("UI Elements - TextMeshPro")]
    public TextMeshProUGUI songNameTMP;
    public TextMeshProUGUI artistTMP;
    public TextMeshProUGUI difficultyTMP;
    public TextMeshProUGUI scoreTMP;
    public TextMeshProUGUI completionTMP;
    public TextMeshProUGUI perfectTMP;
    public TextMeshProUGUI goodTMP;
    public TextMeshProUGUI missedTMP;
    
    [Header("Auto Setup")]
    public bool setupOnStart = true;
    public bool autoFindUIElements = true;
    
    [Header("Data Sources")]
    public bool usePlayerPrefs = true;
    public bool useStaticData = true;
    
    void Start()
    {
        if (setupOnStart)
        {
            SetupPostGameplayData();
        }
    }
    
    /// <summary>
    /// Configura los datos del PostGameplay
    /// </summary>
    [ContextMenu("Setup PostGameplay Data")]
    public void SetupPostGameplayData()
    {
        Debug.Log("📊 Configurando datos del PostGameplay...");
        
        // Buscar elementos UI automáticamente
        if (autoFindUIElements)
        {
            FindUIElements();
        }
        
        // Cargar datos reales
        LoadGameplayData();
        
        // Configurar botón Return to Menu
        SetupReturnToMenuButton();
        
        Debug.Log("✅ Datos del PostGameplay configurados");
    }
    
    /// <summary>
    /// Busca automáticamente los elementos UI
    /// </summary>
    void FindUIElements()
    {
        Debug.Log("🔍 Buscando elementos UI automáticamente...");
        
        // Buscar TextMeshPro components (más común en UI moderna)
        TextMeshProUGUI[] allTMPTexts = FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None);
        
        foreach (TextMeshProUGUI tmpComponent in allTMPTexts)
        {
            string name = tmpComponent.name.ToLower();
            string text = tmpComponent.text.ToLower();
            
            Debug.Log($"🔍 Analizando TMP: {tmpComponent.name} - Texto: '{tmpComponent.text}'");
            
            // Identificar por nombre o contenido
            if (name.Contains("song") || text.Contains("test song") || text.Contains("song"))
            {
                songNameTMP = tmpComponent;
                Debug.Log($"✅ Song Name TMP encontrado: {tmpComponent.name}");
            }
            else if (name.Contains("artist") || text.Contains("test artist") || text.Contains("artist"))
            {
                artistTMP = tmpComponent;
                Debug.Log($"✅ Artist TMP encontrado: {tmpComponent.name}");
            }
            else if (name.Contains("difficulty") || text.Contains("medium") || text.Contains("difficulty"))
            {
                difficultyTMP = tmpComponent;
                Debug.Log($"✅ Difficulty TMP encontrado: {tmpComponent.name}");
            }
            else if (name.Contains("score") || text.Contains("15000") || text.Contains("15,000") || text.Contains("score"))
            {
                scoreTMP = tmpComponent;
                Debug.Log($"✅ Score TMP encontrado: {tmpComponent.name}");
            }
            else if (name.Contains("completion") || text.Contains("82.3%") || text.Contains("completion") || text.Contains("%"))
            {
                completionTMP = tmpComponent;
                Debug.Log($"✅ Completion TMP encontrado: {tmpComponent.name}");
            }
            else if (name.Contains("perfect") || text.Contains("150") || text.Contains("perfect"))
            {
                perfectTMP = tmpComponent;
                Debug.Log($"✅ Perfect TMP encontrado: {tmpComponent.name}");
            }
            else if (name.Contains("good") || text.Contains("45") || text.Contains("good"))
            {
                goodTMP = tmpComponent;
                Debug.Log($"✅ Good TMP encontrado: {tmpComponent.name}");
            }
            else if (name.Contains("miss") || text.Contains("35") || text.Contains("miss"))
            {
                missedTMP = tmpComponent;
                Debug.Log($"✅ Missed TMP encontrado: {tmpComponent.name}");
            }
        }
        
        // También buscar Text components normales como backup
        Text[] allTexts = FindObjectsByType<Text>(FindObjectsSortMode.None);
        
        foreach (Text textComponent in allTexts)
        {
            string name = textComponent.name.ToLower();
            string text = textComponent.text.ToLower();
            
            // Solo asignar si no se encontró TMP equivalente
            if ((name.Contains("song") || text.Contains("test song")) && songNameTMP == null)
            {
                songNameText = textComponent;
                Debug.Log($"✅ Song Name Text encontrado: {textComponent.name}");
            }
            else if ((name.Contains("artist") || text.Contains("test artist")) && artistTMP == null)
            {
                artistText = textComponent;
                Debug.Log($"✅ Artist Text encontrado: {textComponent.name}");
            }
            // ... continuar para otros campos si es necesario
        }
    }
    
    /// <summary>
    /// Carga los datos reales del gameplay
    /// </summary>
    void LoadGameplayData()
    {
        Debug.Log("📈 Cargando datos reales del gameplay...");
        
        // Datos por defecto
        string songName = "Baile Inolvidable";
        string artist = "Artista Desconocido";
        string difficulty = "Medium";
        int score = 0;
        float completion = 0f;
        int perfect = 0;
        int good = 0;
        int missed = 0;
        
        // Cargar desde PlayerPrefs si está disponible
        if (usePlayerPrefs)
        {
            songName = PlayerPrefs.GetString("LastSongName", songName);
            artist = PlayerPrefs.GetString("LastArtist", artist);
            difficulty = PlayerPrefs.GetString("LastDifficulty", difficulty);
            score = PlayerPrefs.GetInt("LastScore", score);
            completion = PlayerPrefs.GetFloat("LastCompletion", completion);
            perfect = PlayerPrefs.GetInt("LastPerfect", perfect);
            good = PlayerPrefs.GetInt("LastGood", good);
            missed = PlayerPrefs.GetInt("LastMissed", missed);
        }
        
        // Cargar desde datos estáticos si está disponible
        if (useStaticData)
        {
            if (GameplayData.HasData())
            {
                songName = GameplayData.songName;
                artist = GameplayData.artist;
                difficulty = GameplayData.difficulty;
                score = GameplayData.score;
                completion = GameplayData.completion;
                perfect = GameplayData.perfect;
                good = GameplayData.good;
                missed = GameplayData.missed;
            }
        }
        
        // Aplicar datos a la UI
        ApplyDataToUI(songName, artist, difficulty, score, completion, perfect, good, missed);
    }
    
    /// <summary>
    /// Aplica los datos a los elementos UI
    /// </summary>
    void ApplyDataToUI(string songName, string artist, string difficulty, int score, float completion, int perfect, int good, int missed)
    {
        // Aplicar a TextMeshPro (prioridad)
        if (songNameTMP != null)
        {
            songNameTMP.text = songName;
            Debug.Log($"📝 Song Name TMP actualizado: {songName}");
        }
        else if (songNameText != null)
        {
            songNameText.text = songName;
            Debug.Log($"📝 Song Name Text actualizado: {songName}");
        }
        
        if (artistTMP != null)
        {
            artistTMP.text = $"by {artist}";
            Debug.Log($"📝 Artist TMP actualizado: by {artist}");
        }
        else if (artistText != null)
        {
            artistText.text = $"by {artist}";
        }
        
        if (difficultyTMP != null)
        {
            difficultyTMP.text = $"Difficulty: {difficulty}";
            Debug.Log($"📝 Difficulty TMP actualizado: {difficulty}");
        }
        else if (difficultyText != null)
        {
            difficultyText.text = $"Difficulty: {difficulty}";
        }
        
        if (scoreTMP != null)
        {
            scoreTMP.text = $"Score: {score:N0}";
            Debug.Log($"📝 Score TMP actualizado: {score:N0}");
        }
        else if (scoreText != null)
        {
            scoreText.text = $"Score: {score:N0}";
        }
        
        if (completionTMP != null)
        {
            completionTMP.text = $"Completion: {completion:F1}%";
            Debug.Log($"📝 Completion TMP actualizado: {completion:F1}%");
        }
        else if (completionText != null)
        {
            completionText.text = $"Completion: {completion:F1}%";
        }
        
        if (perfectTMP != null)
        {
            perfectTMP.text = $"Perfect: {perfect}";
            Debug.Log($"📝 Perfect TMP actualizado: {perfect}");
        }
        else if (perfectText != null)
        {
            perfectText.text = $"Perfect: {perfect}";
        }
        
        if (goodTMP != null)
        {
            goodTMP.text = $"Good: {good}";
            Debug.Log($"📝 Good TMP actualizado: {good}");
        }
        else if (goodText != null)
        {
            goodText.text = $"Good: {good}";
        }
        
        if (missedTMP != null)
        {
            missedTMP.text = $"Missed: {missed}";
            Debug.Log($"📝 Missed TMP actualizado: {missed}");
        }
        else if (missedText != null)
        {
            missedText.text = $"Missed: {missed}";
        }
        
        Debug.Log($"✅ Datos aplicados - {songName} by {artist} - Score: {score:N0}");
    }
    
    /// <summary>
    /// Configura el botón Return to Menu
    /// </summary>
    void SetupReturnToMenuButton()
    {
        Debug.Log("🔧 Configurando botón Return to Menu...");
        
        // Buscar el botón
        Button returnButton = null;
        Button[] allButtons = FindObjectsByType<Button>(FindObjectsSortMode.None);
        
        foreach (Button button in allButtons)
        {
            string name = button.name.ToLower();
            
            // Buscar por nombre
            if (name.Contains("return") || name.Contains("menu") || name.Contains("back"))
            {
                returnButton = button;
                break;
            }
            
            // Buscar por texto (Text normal)
            Text buttonText = button.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                string text = buttonText.text.ToLower();
                if (text.Contains("return") || text.Contains("menu") || text.Contains("back"))
                {
                    returnButton = button;
                    Debug.Log($"✅ Botón encontrado por Text: {button.name} ('{buttonText.text}')");
                    break;
                }
            }
            
            // Buscar por texto (TextMeshPro)
            TextMeshProUGUI buttonTMP = button.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonTMP != null)
            {
                string text = buttonTMP.text.ToLower();
                if (text.Contains("return") || text.Contains("menu") || text.Contains("back"))
                {
                    returnButton = button;
                    Debug.Log($"✅ Botón encontrado por TextMeshPro: {button.name} ('{buttonTMP.text}')");
                    break;
                }
            }
        }
        
        if (returnButton != null)
        {
            // Limpiar listeners existentes
            returnButton.onClick.RemoveAllListeners();
            
            // Agregar función para volver al menú
            returnButton.onClick.AddListener(() => {
                Debug.Log("🏠 Volviendo al MainMenu...");
                UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
            });
            
            Debug.Log($"✅ Botón Return to Menu configurado: {returnButton.name}");
        }
        else
        {
            Debug.LogWarning("⚠️ No se encontró botón Return to Menu");
        }
    }
    
    /// <summary>
    /// Configurar datos manualmente
    /// </summary>
    public void SetGameplayData(string songName, string artist, string difficulty, int score, float completion, int perfect, int good, int missed)
    {
        ApplyDataToUI(songName, artist, difficulty, score, completion, perfect, good, missed);
        
        // Guardar en PlayerPrefs para próxima vez
        PlayerPrefs.SetString("LastSongName", songName);
        PlayerPrefs.SetString("LastArtist", artist);
        PlayerPrefs.SetString("LastDifficulty", difficulty);
        PlayerPrefs.SetInt("LastScore", score);
        PlayerPrefs.SetFloat("LastCompletion", completion);
        PlayerPrefs.SetInt("LastPerfect", perfect);
        PlayerPrefs.SetInt("LastGood", good);
        PlayerPrefs.SetInt("LastMissed", missed);
        PlayerPrefs.Save();
        
        Debug.Log("✅ Datos configurados manualmente y guardados");
    }
    
    /// <summary>
    /// Mostrar información del sistema
    /// </summary>
    [ContextMenu("Show PostGameplay Info")]
    public void ShowPostGameplayInfo()
    {
        Debug.Log("📊 INFORMACIÓN DEL POSTGAMEPLAY:");
        Debug.Log("═══════════════════════════════");
        
        Debug.Log($"Song Name Text: {(songNameText != null ? "✅ ENCONTRADO" : "❌ FALTANTE")}");
        Debug.Log($"Artist Text: {(artistText != null ? "✅ ENCONTRADO" : "❌ FALTANTE")}");
        Debug.Log($"Difficulty Text: {(difficultyText != null ? "✅ ENCONTRADO" : "❌ FALTANTE")}");
        Debug.Log($"Score Text: {(scoreText != null ? "✅ ENCONTRADO" : "❌ FALTANTE")}");
        Debug.Log($"Completion Text: {(completionText != null ? "✅ ENCONTRADO" : "❌ FALTANTE")}");
        Debug.Log($"Perfect Text: {(perfectText != null ? "✅ ENCONTRADO" : "❌ FALTANTE")}");
        Debug.Log($"Good Text: {(goodText != null ? "✅ ENCONTRADO" : "❌ FALTANTE")}");
        Debug.Log($"Missed Text: {(missedText != null ? "✅ ENCONTRADO" : "❌ FALTANTE")}");
        
        // Mostrar datos actuales
        Debug.Log("\n📈 DATOS ACTUALES:");
        if (usePlayerPrefs)
        {
            Debug.Log($"Song: {PlayerPrefs.GetString("LastSongName", "N/A")}");
            Debug.Log($"Artist: {PlayerPrefs.GetString("LastArtist", "N/A")}");
            Debug.Log($"Score: {PlayerPrefs.GetInt("LastScore", 0):N0}");
            Debug.Log($"Completion: {PlayerPrefs.GetFloat("LastCompletion", 0f):F1}%");
        }
        
        // Mostrar botones disponibles
        Button[] buttons = FindObjectsByType<Button>(FindObjectsSortMode.None);
        Debug.Log($"\n🔘 Botones disponibles: {buttons.Length}");
        foreach (Button btn in buttons)
        {
            Text btnText = btn.GetComponentInChildren<Text>();
            string text = btnText != null ? btnText.text : "Sin texto";
            Debug.Log($"  🔘 {btn.name} ('{text}')");
        }
    }
}

/// <summary>
/// Clase estática para compartir datos entre escenas
/// </summary>
public static class GameplayData
{
    public static string songName = "";
    public static string artist = "";
    public static string difficulty = "";
    public static int score = 0;
    public static float completion = 0f;
    public static int perfect = 0;
    public static int good = 0;
    public static int missed = 0;
    
    public static bool HasData()
    {
        return !string.IsNullOrEmpty(songName);
    }
    
    public static void SetData(string song, string art, string diff, int sc, float comp, int perf, int gd, int miss)
    {
        songName = song;
        artist = art;
        difficulty = diff;
        score = sc;
        completion = comp;
        perfect = perf;
        good = gd;
        missed = miss;
    }
    
    public static void Clear()
    {
        songName = "";
        artist = "";
        difficulty = "";
        score = 0;
        completion = 0f;
        perfect = 0;
        good = 0;
        missed = 0;
    }
}
