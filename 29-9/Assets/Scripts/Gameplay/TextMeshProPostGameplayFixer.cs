using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Arreglador específico para TextMeshPro en PostGameplay
/// Versión simplificada que funciona directamente con TMP
/// </summary>
public class TextMeshProPostGameplayFixer : MonoBehaviour
{
    [Header("Auto Fix")]
    public bool fixOnStart = true;
    public bool showDebugLogs = true;
    
    [Header("Sample Data")]
    public string realSongName = "Baile Inolvidable";
    public string realArtist = "Artista Latino";
    public string realDifficulty = "Medium";
    public int realScore = 187500;
    public float realCompletion = 94.2f;
    public int realPerfect = 142;
    public int realGood = 38;
    public int realMissed = 12;
    
    void Start()
    {
        if (fixOnStart)
        {
            FixPostGameplayData();
        }
    }
    
    /// <summary>
    /// Arregla todos los datos del PostGameplay
    /// </summary>
    [ContextMenu("Fix PostGameplay Data")]
    public void FixPostGameplayData()
    {
        Debug.Log("🔧 Arreglando datos del PostGameplay con TextMeshPro...");
        
        // Cargar datos reales si están disponibles
        LoadRealData();
        
        // Buscar y actualizar todos los TextMeshPro
        UpdateAllTextMeshPro();
        
        // Arreglar botón Return to Menu
        FixReturnToMenuButton();
        
        Debug.Log("✅ PostGameplay arreglado completamente");
    }
    
    /// <summary>
    /// Carga datos reales desde PlayerPrefs o GameplayData
    /// </summary>
    void LoadRealData()
    {
        // Prioridad 1: GameplayData (más reciente)
        if (GameplayData.HasData())
        {
            realSongName = GameplayData.songName;
            realArtist = GameplayData.artist;
            realDifficulty = GameplayData.difficulty;
            realScore = GameplayData.score;
            realCompletion = GameplayData.completion;
            realPerfect = GameplayData.perfect;
            realGood = GameplayData.good;
            realMissed = GameplayData.missed;
            
            if (showDebugLogs)
            {
                Debug.Log($"📊 Datos cargados desde GameplayData: {realSongName} - {realScore:N0}");
            }
            return;
        }
        
        // Prioridad 2: PlayerPrefs (backup)
        if (PlayerPrefs.HasKey("LastSongName"))
        {
            realSongName = PlayerPrefs.GetString("LastSongName", realSongName);
            realArtist = PlayerPrefs.GetString("LastArtist", realArtist);
            realDifficulty = PlayerPrefs.GetString("LastDifficulty", realDifficulty);
            realScore = PlayerPrefs.GetInt("LastScore", realScore);
            realCompletion = PlayerPrefs.GetFloat("LastCompletion", realCompletion);
            realPerfect = PlayerPrefs.GetInt("LastPerfect", realPerfect);
            realGood = PlayerPrefs.GetInt("LastGood", realGood);
            realMissed = PlayerPrefs.GetInt("LastMissed", realMissed);
            
            if (showDebugLogs)
            {
                Debug.Log($"📱 Datos cargados desde PlayerPrefs: {realSongName} - {realScore:N0}");
            }
            return;
        }
        
        // Si no hay datos, usar valores por defecto mejorados
        if (showDebugLogs)
        {
            Debug.Log("⚠️ No se encontraron datos guardados, usando valores por defecto");
        }
    }
    
    /// <summary>
    /// Actualiza todos los TextMeshPro con datos reales
    /// </summary>
    void UpdateAllTextMeshPro()
    {
        TextMeshProUGUI[] allTMPs = FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None);
        
        if (showDebugLogs)
        {
            Debug.Log($"🔍 Encontrados {allTMPs.Length} TextMeshPro components");
        }
        
        foreach (TextMeshProUGUI tmp in allTMPs)
        {
            string currentText = tmp.text.ToLower();
            string name = tmp.name.ToLower();
            
            if (showDebugLogs)
            {
                Debug.Log($"🔍 Analizando: {tmp.name} - '{tmp.text}'");
            }
            
            // Reemplazar texto basado en contenido actual
            if (currentText.Contains("test song") || name.Contains("song"))
            {
                tmp.text = realSongName;
                if (showDebugLogs) Debug.Log($"✅ Song actualizado: {realSongName}");
            }
            else if (currentText.Contains("test artist") || name.Contains("artist"))
            {
                tmp.text = $"by {realArtist}";
                if (showDebugLogs) Debug.Log($"✅ Artist actualizado: by {realArtist}");
            }
            else if (currentText.Contains("medium") || currentText.Contains("difficulty") || name.Contains("difficulty"))
            {
                tmp.text = $"Difficulty: {realDifficulty}";
                if (showDebugLogs) Debug.Log($"✅ Difficulty actualizado: {realDifficulty}");
            }
            else if (currentText.Contains("15000") || currentText.Contains("15,000") || name.Contains("score"))
            {
                tmp.text = $"Score: {realScore:N0}";
                if (showDebugLogs) Debug.Log($"✅ Score actualizado: {realScore:N0}");
            }
            else if (currentText.Contains("82.3%") || currentText.Contains("%") || name.Contains("completion"))
            {
                tmp.text = $"Completion: {realCompletion:F1}%";
                if (showDebugLogs) Debug.Log($"✅ Completion actualizado: {realCompletion:F1}%");
            }
            else if (currentText.Contains("150") || name.Contains("perfect"))
            {
                tmp.text = $"Perfect: {realPerfect}";
                if (showDebugLogs) Debug.Log($"✅ Perfect actualizado: {realPerfect}");
            }
            else if (currentText.Contains("45") || name.Contains("good"))
            {
                tmp.text = $"Good: {realGood}";
                if (showDebugLogs) Debug.Log($"✅ Good actualizado: {realGood}");
            }
            else if (currentText.Contains("35") || name.Contains("miss"))
            {
                tmp.text = $"Missed: {realMissed}";
                if (showDebugLogs) Debug.Log($"✅ Missed actualizado: {realMissed}");
            }
        }
    }
    
    /// <summary>
    /// Arregla el botón Return to Menu
    /// </summary>
    void FixReturnToMenuButton()
    {
        Button[] allButtons = FindObjectsByType<Button>(FindObjectsSortMode.None);
        
        foreach (Button button in allButtons)
        {
            // Buscar por TextMeshPro del botón
            TextMeshProUGUI buttonTMP = button.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonTMP != null)
            {
                string text = buttonTMP.text.ToLower();
                if (text.Contains("return") || text.Contains("menu") || text.Contains("back"))
                {
                    // Limpiar listeners existentes
                    button.onClick.RemoveAllListeners();
                    
                    // Agregar función para volver al menú
                    button.onClick.AddListener(() => {
                        if (showDebugLogs) Debug.Log("🏠 Volviendo al MainMenu...");
                        SceneManager.LoadScene("MainMenu");
                    });
                    
                    if (showDebugLogs)
                    {
                        Debug.Log($"✅ Botón Return to Menu configurado: {button.name}");
                    }
                    break;
                }
            }
            
            // También buscar por Text normal como backup
            Text buttonText = button.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                string text = buttonText.text.ToLower();
                if (text.Contains("return") || text.Contains("menu") || text.Contains("back"))
                {
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() => {
                        if (showDebugLogs) Debug.Log("🏠 Volviendo al MainMenu...");
                        SceneManager.LoadScene("MainMenu");
                    });
                    
                    if (showDebugLogs)
                    {
                        Debug.Log($"✅ Botón Return to Menu configurado (Text): {button.name}");
                    }
                    break;
                }
            }
        }
    }
    
    /// <summary>
    /// Configurar datos manualmente
    /// </summary>
    public void SetGameplayData(string song, string artist, string difficulty, int score, float completion, int perfect, int good, int missed)
    {
        realSongName = song;
        realArtist = artist;
        realDifficulty = difficulty;
        realScore = score;
        realCompletion = completion;
        realPerfect = perfect;
        realGood = good;
        realMissed = missed;
        
        UpdateAllTextMeshPro();
        
        if (showDebugLogs)
        {
            Debug.Log($"✅ Datos configurados manualmente: {song} - {score:N0}");
        }
    }
    
    /// <summary>
    /// Test con datos de ejemplo
    /// </summary>
    [ContextMenu("Test With Sample Data")]
    public void TestWithSampleData()
    {
        SetGameplayData(
            "Baile Inolvidable",
            "Artista Latino", 
            "Medium",
            187500,
            94.2f,
            142,
            38,
            12
        );
    }
    
    /// <summary>
    /// Mostrar todos los TextMeshPro disponibles
    /// </summary>
    [ContextMenu("Show All TextMeshPro")]
    public void ShowAllTextMeshPro()
    {
        TextMeshProUGUI[] allTMPs = FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None);
        
        Debug.Log("📊 TODOS LOS TEXTMESHPRO EN LA ESCENA:");
        Debug.Log("═══════════════════════════════════════");
        
        for (int i = 0; i < allTMPs.Length; i++)
        {
            TextMeshProUGUI tmp = allTMPs[i];
            Debug.Log($"{i + 1}. {tmp.name} - '{tmp.text}'");
        }
        
        // También mostrar botones
        Button[] allButtons = FindObjectsByType<Button>(FindObjectsSortMode.None);
        Debug.Log($"\n🔘 BOTONES DISPONIBLES: {allButtons.Length}");
        
        foreach (Button btn in allButtons)
        {
            TextMeshProUGUI btnTMP = btn.GetComponentInChildren<TextMeshProUGUI>();
            Text btnText = btn.GetComponentInChildren<Text>();
            
            string text = "";
            if (btnTMP != null) text = btnTMP.text;
            else if (btnText != null) text = btnText.text;
            
            Debug.Log($"  🔘 {btn.name} - '{text}'");
        }
    }
    
    /// <summary>
    /// Arreglo de emergencia - reemplaza todo texto que parezca de prueba
    /// </summary>
    [ContextMenu("Emergency Fix All Test Data")]
    public void EmergencyFixAllTestData()
    {
        Debug.Log("🚨 ARREGLO DE EMERGENCIA - Reemplazando todos los datos de prueba...");
        
        TextMeshProUGUI[] allTMPs = FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None);
        
        foreach (TextMeshProUGUI tmp in allTMPs)
        {
            string text = tmp.text;
            
            // Reemplazar cualquier texto que contenga "Test"
            if (text.Contains("Test Song"))
            {
                tmp.text = realSongName;
                Debug.Log($"🔧 Reemplazado: '{text}' → '{realSongName}'");
            }
            else if (text.Contains("Test Artist"))
            {
                tmp.text = $"by {realArtist}";
                Debug.Log($"🔧 Reemplazado: '{text}' → 'by {realArtist}'");
            }
            else if (text == "15000" || text == "15,000")
            {
                tmp.text = realScore.ToString("N0");
                Debug.Log($"🔧 Reemplazado: '{text}' → '{realScore:N0}'");
            }
            else if (text == "82.3%")
            {
                tmp.text = $"{realCompletion:F1}%";
                Debug.Log($"🔧 Reemplazado: '{text}' → '{realCompletion:F1}%'");
            }
            else if (text == "150")
            {
                tmp.text = realPerfect.ToString();
                Debug.Log($"🔧 Reemplazado: '{text}' → '{realPerfect}'");
            }
            else if (text == "45")
            {
                tmp.text = realGood.ToString();
                Debug.Log($"🔧 Reemplazado: '{text}' → '{realGood}'");
            }
            else if (text == "35")
            {
                tmp.text = realMissed.ToString();
                Debug.Log($"🔧 Reemplazado: '{text}' → '{realMissed}'");
            }
        }
        
        // También arreglar botón
        FixReturnToMenuButton();
        
        Debug.Log("✅ Arreglo de emergencia completado");
    }
}
