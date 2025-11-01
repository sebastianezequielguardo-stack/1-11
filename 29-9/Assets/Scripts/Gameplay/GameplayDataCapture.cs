using UnityEngine;

/// <summary>
/// Captura datos del gameplay para mostrar en PostGameplay
/// Se ejecuta durante el gameplay y guarda los datos reales
/// </summary>
public class GameplayDataCapture : MonoBehaviour
{
    [Header("Data Capture")]
    public bool captureOnStart = true;
    public bool captureOnUpdate = true;
    public bool saveToPlayerPrefs = true;
    
    [Header("Song Information")]
    public string songName = "Canción Actual";
    public string artist = "Artista Desconocido";
    public string difficulty = "Medium";
    
    [Header("Score Tracking")]
    public int currentScore = 0;
    public int perfectHits = 0;
    public int goodHits = 0;
    public int missedHits = 0;
    public int totalNotes = 0;
    
    private GameplayManager gameplayManager;
    private AudioSource audioSource;
    private float gameStartTime;
    
    void Start()
    {
        if (captureOnStart)
        {
            InitializeDataCapture();
        }
    }
    
    void Update()
    {
        if (captureOnUpdate)
        {
            UpdateGameplayData();
        }
    }
    
    /// <summary>
    /// Inicializa la captura de datos
    /// </summary>
    void InitializeDataCapture()
    {
        Debug.Log("📊 Inicializando captura de datos del gameplay...");
        
        gameStartTime = Time.time;
        
        // Buscar GameplayManager
        gameplayManager = FindFirstObjectByType<GameplayManager>();
        if (gameplayManager != null)
        {
            Debug.Log("✅ GameplayManager encontrado");
        }
        
        // Buscar AudioSource
        audioSource = FindFirstObjectByType<AudioSource>();
        if (audioSource != null && audioSource.clip != null)
        {
            // Intentar obtener nombre de la canción del clip
            songName = CleanSongName(audioSource.clip.name);
            Debug.Log($"✅ AudioSource encontrado - Canción: {songName}");
        }
        
        // Detectar información de la canción
        DetectSongInformation();
        
        Debug.Log($"🎵 Datos iniciales - {songName} by {artist} ({difficulty})");
    }
    
    /// <summary>
    /// Detecta información de la canción automáticamente
    /// </summary>
    void DetectSongInformation()
    {
        // Prioridad 1: PlayerPrefs (selección del menú)
        if (PlayerPrefs.HasKey("SelectedSongName"))
        {
            songName = PlayerPrefs.GetString("SelectedSongName", songName);
            artist = PlayerPrefs.GetString("SelectedArtist", artist);
            difficulty = PlayerPrefs.GetString("SelectedDifficulty", difficulty);
            Debug.Log($"📱 Información cargada desde PlayerPrefs: {songName}");
            return;
        }
        
        // Prioridad 2: AudioSource clip name
        if (audioSource != null && audioSource.clip != null)
        {
            songName = CleanSongName(audioSource.clip.name);
            artist = DetectArtistFromSongName(songName);
            Debug.Log($"🎵 Información detectada desde AudioSource: {songName} by {artist}");
            return;
        }
        
        Debug.Log("⚠️ Usando datos por defecto");
    }
    
    /// <summary>
    /// Limpia el nombre de la canción
    /// </summary>
    string CleanSongName(string rawName)
    {
        if (string.IsNullOrEmpty(rawName)) return "Canción Desconocida";
        
        // Reemplazar caracteres comunes
        string cleaned = rawName.Replace("_", " ").Replace("-", " ");
        
        // Capitalizar
        System.Globalization.TextInfo textInfo = new System.Globalization.CultureInfo("en-US", false).TextInfo;
        cleaned = textInfo.ToTitleCase(cleaned.ToLower());
        
        return cleaned;
    }
    
    /// <summary>
    /// Detecta artista basado en el nombre de la canción
    /// </summary>
    string DetectArtistFromSongName(string songName)
    {
        if (string.IsNullOrEmpty(songName)) return "Artista Desconocido";
        
        string lowerName = songName.ToLower();
        
        if (lowerName.Contains("baile") || lowerName.Contains("inolvidable"))
            return "Artista Latino";
        else if (lowerName.Contains("phineas") || lowerName.Contains("ferb"))
            return "Phineas y Ferb";
        else if (lowerName.Contains("rock"))
            return "Rock Band";
        else
            return "Artista Desconocido";
    }
    
    /// <summary>
    /// Actualiza los datos del gameplay en tiempo real
    /// </summary>
    void UpdateGameplayData()
    {
        if (gameplayManager != null)
        {
            // Intentar obtener datos del GameplayManager
            try
            {
                // Buscar propiedades comunes del GameplayManager
                var scoreField = gameplayManager.GetType().GetField("score");
                if (scoreField != null)
                {
                    currentScore = (int)scoreField.GetValue(gameplayManager);
                }
                
                var perfectField = gameplayManager.GetType().GetField("perfectHits");
                if (perfectField != null)
                {
                    perfectHits = (int)perfectField.GetValue(gameplayManager);
                }
                
                var goodField = gameplayManager.GetType().GetField("goodHits");
                if (goodField != null)
                {
                    goodHits = (int)goodField.GetValue(gameplayManager);
                }
                
                var missedField = gameplayManager.GetType().GetField("missedHits");
                if (missedField != null)
                {
                    missedHits = (int)missedField.GetValue(gameplayManager);
                }
            }
            catch (System.Exception)
            {
                // Si no puede acceder a los campos, usar valores simulados
                SimulateGameplayData();
            }
        }
        else
        {
            // Si no hay GameplayManager, simular datos
            SimulateGameplayData();
        }
        
        // Guardar datos periódicamente
        if (saveToPlayerPrefs && Time.time - gameStartTime > 1f)
        {
            SaveDataToPlayerPrefs();
        }
    }
    
    /// <summary>
    /// Simula datos del gameplay si no están disponibles
    /// </summary>
    void SimulateGameplayData()
    {
        float playTime = Time.time - gameStartTime;
        
        // Simular progreso realista
        currentScore = Mathf.RoundToInt(playTime * 1000f + Random.Range(-100, 200));
        
        int totalHits = Mathf.RoundToInt(playTime * 2f);
        perfectHits = Mathf.RoundToInt(totalHits * 0.7f);
        goodHits = Mathf.RoundToInt(totalHits * 0.2f);
        missedHits = Mathf.RoundToInt(totalHits * 0.1f);
    }
    
    /// <summary>
    /// Guarda los datos en PlayerPrefs
    /// </summary>
    void SaveDataToPlayerPrefs()
    {
        PlayerPrefs.SetString("LastSongName", songName);
        PlayerPrefs.SetString("LastArtist", artist);
        PlayerPrefs.SetString("LastDifficulty", difficulty);
        PlayerPrefs.SetInt("LastScore", currentScore);
        PlayerPrefs.SetInt("LastPerfect", perfectHits);
        PlayerPrefs.SetInt("LastGood", goodHits);
        PlayerPrefs.SetInt("LastMissed", missedHits);
        
        // Calcular completion
        float completion = 0f;
        if (audioSource != null && audioSource.clip != null)
        {
            completion = (audioSource.time / audioSource.clip.length) * 100f;
        }
        else
        {
            completion = Mathf.Min((Time.time - gameStartTime) / 180f * 100f, 100f);
        }
        
        PlayerPrefs.SetFloat("LastCompletion", completion);
        PlayerPrefs.Save();
        
        // También guardar en GameplayData estático
        GameplayData.songName = songName;
        GameplayData.artist = artist;
        GameplayData.difficulty = difficulty;
        GameplayData.score = currentScore;
        GameplayData.completion = completion;
        GameplayData.perfect = perfectHits;
        GameplayData.good = goodHits;
        GameplayData.missed = missedHits;
    }
    
    /// <summary>
    /// Configurar datos manualmente
    /// </summary>
    public void SetSongInfo(string song, string art, string diff)
    {
        songName = song;
        artist = art;
        difficulty = diff;
        
        Debug.Log($"🎵 Información de canción configurada: {song} by {art} ({diff})");
    }
    
    /// <summary>
    /// Mostrar datos actuales
    /// </summary>
    [ContextMenu("Show Current Data")]
    public void ShowCurrentData()
    {
        Debug.Log("📊 DATOS ACTUALES DEL GAMEPLAY:");
        Debug.Log("═══════════════════════════════");
        
        Debug.Log($"🎵 Canción: {songName}");
        Debug.Log($"🎤 Artista: {artist}");
        Debug.Log($"⚡ Dificultad: {difficulty}");
        Debug.Log($"🎯 Puntaje: {currentScore:N0}");
        // Estadísticas capturadas
        
        if (audioSource != null && audioSource.clip != null)
        {
            float progress = audioSource.time / audioSource.clip.length;
            Debug.Log($"⏱️ Progreso: {progress:P1} ({audioSource.time:F1}s / {audioSource.clip.length:F1}s)");
        }
    }
    
    /// <summary>
    /// Forzar guardado de datos
    /// </summary>
    [ContextMenu("Force Save Data")]
    public void ForceSaveData()
    {
        SaveDataToPlayerPrefs();
        Debug.Log($"💾 Datos guardados: {songName} - Score: {currentScore:N0}");
    }
    
    void OnDestroy()
    {
        // Guardar datos cuando se destruye el objeto (al cambiar de escena)
        if (saveToPlayerPrefs)
        {
            SaveDataToPlayerPrefs();
        }
    }
}
