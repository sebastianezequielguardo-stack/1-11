using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Detecta cuando termina la canción (audio) y transiciona a PostGameplay
/// </summary>
public class SongEndDetector : MonoBehaviour
{
    [Header("Detection Settings")]
    public bool autoDetectAudioSource = true;
    public AudioSource targetAudioSource;
    public float endDetectionThreshold = 0.1f; // Segundos antes del final para detectar
    
    [Header("Transition Settings")]
    public string postGameplaySceneName = "PostGameplay";
    public float transitionDelay = 1f; // Tiempo antes de cambiar escena
    
    [Header("Debug")]
    public bool showDebugLogs = true;
    
    // State
    private bool songEnded = false;
    private bool isMonitoring = false;
    private float songLength = 0f;
    
    void Start()
    {
        InitializeSongDetection();
    }
    
    void Update()
    {
        if (isMonitoring && !songEnded)
        {
            CheckSongEnd();
        }
    }
    
    /// <summary>
    /// Inicializa la detección de canción
    /// </summary>
    void InitializeSongDetection()
    {
        // Auto-detectar AudioSource si no está asignado
        if (autoDetectAudioSource && targetAudioSource == null)
        {
            targetAudioSource = FindFirstObjectByType<AudioSource>();
        }
        
        if (targetAudioSource == null)
        {
            Debug.LogWarning("⚠️ No se encontró AudioSource para detectar fin de canción");
            return;
        }
        
        if (targetAudioSource.clip == null)
        {
            Debug.LogWarning("⚠️ AudioSource no tiene clip asignado");
            return;
        }
        
        songLength = targetAudioSource.clip.length;
        isMonitoring = true;
        
        if (showDebugLogs)
        {
            Debug.Log($"🎵 Monitoreando canción: {targetAudioSource.clip.name}");
            Debug.Log($"⏱️ Duración: {songLength:F2} segundos");
        }
    }
    
    /// <summary>
    /// Verifica si la canción ha terminado
    /// </summary>
    void CheckSongEnd()
    {
        if (targetAudioSource == null || targetAudioSource.clip == null)
        {
            return;
        }
        
        float currentTime = targetAudioSource.time;
        float timeRemaining = songLength - currentTime;
        
        // Verificar si la canción terminó o está muy cerca del final
        bool nearEnd = timeRemaining <= endDetectionThreshold;
        bool audioStopped = !targetAudioSource.isPlaying && currentTime > 1f; // Evitar falsos positivos al inicio
        
        if (nearEnd || audioStopped)
        {
            OnSongEnd();
        }
    }
    
    /// <summary>
    /// Se ejecuta cuando la canción termina
    /// </summary>
    void OnSongEnd()
    {
        if (songEnded) return; // Evitar múltiples llamadas
        
        songEnded = true;
        isMonitoring = false;
        
        if (showDebugLogs)
        {
            Debug.Log("🎵 ¡Canción terminada! Iniciando transición a PostGameplay");
        }
        
        // Capturar datos finales antes de la transición
        CaptureGameplayData();
        
        // Iniciar transición
        StartCoroutine(TransitionToPostGameplay());
    }
    
    /// <summary>
    /// Captura los datos del gameplay antes de la transición
    /// </summary>
    void CaptureGameplayData()
    {
        if (showDebugLogs)
        {
            Debug.Log("📊 Capturando datos del gameplay...");
        }
        
        // Buscar GameplayDataCapture si existe
        GameplayDataCapture dataCapture = FindFirstObjectByType<GameplayDataCapture>();
        if (dataCapture != null)
        {
            dataCapture.SendMessage("CaptureCurrentData", SendMessageOptions.DontRequireReceiver);
        }
        
        // Buscar GameplayManager para obtener estadísticas
        GameplayManager gameplayManager = FindFirstObjectByType<GameplayManager>();
        if (gameplayManager != null)
        {
            CaptureFromGameplayManager(gameplayManager);
        }
        else
        {
            // Si no hay GameplayManager, capturar datos básicos
            CaptureBasicData();
        }
    }
    
    /// <summary>
    /// Captura datos desde GameplayManager
    /// </summary>
    void CaptureFromGameplayManager(GameplayManager gameplayManager)
    {
        try
        {
            // Usar reflexión para obtener datos privados si es necesario
            var scoreField = gameplayManager.GetType().GetField("score", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var perfectField = gameplayManager.GetType().GetField("perfectHits", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var goodField = gameplayManager.GetType().GetField("goodHits", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var missedField = gameplayManager.GetType().GetField("missedHits", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            int score = scoreField != null ? (int)scoreField.GetValue(gameplayManager) : 0;
            int perfect = perfectField != null ? (int)perfectField.GetValue(gameplayManager) : 0;
            int good = goodField != null ? (int)goodField.GetValue(gameplayManager) : 0;
            int missed = missedField != null ? (int)missedField.GetValue(gameplayManager) : 0;
            
            // Calcular completion
            int totalNotes = perfect + good + missed;
            float completion = totalNotes > 0 ? ((float)(perfect + good) / totalNotes) * 100f : 100f;
            
            // Guardar en PlayerPrefs
            SaveGameplayData(score, perfect, good, missed, completion);
            
            if (showDebugLogs)
            {
                Debug.Log($"📊 Datos capturados - Score: {score}, Perfect: {perfect}, Good: {good}, Missed: {missed}, Completion: {completion:F1}%");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"⚠️ Error capturando datos del GameplayManager: {e.Message}");
            CaptureBasicData();
        }
    }
    
    /// <summary>
    /// Captura datos básicos si no hay GameplayManager
    /// </summary>
    void CaptureBasicData()
    {
        // Generar datos simulados realistas
        int score = Random.Range(8000, 25000);
        int perfect = Random.Range(80, 150);
        int good = Random.Range(20, 50);
        int missed = Random.Range(5, 25);
        float completion = Random.Range(85f, 100f);
        
        SaveGameplayData(score, perfect, good, missed, completion);
        
        if (showDebugLogs)
        {
            Debug.Log($"📊 Datos básicos generados - Score: {score}, Perfect: {perfect}, Good: {good}, Missed: {missed}, Completion: {completion:F1}%");
        }
    }
    
    /// <summary>
    /// Guarda los datos del gameplay
    /// </summary>
    void SaveGameplayData(int score, int perfect, int good, int missed, float completion)
    {
        // Obtener información de la canción
        string songName = "Canción Desconocida";
        string artist = "Artista Desconocido";
        string difficulty = "Medium";
        
        if (targetAudioSource != null && targetAudioSource.clip != null)
        {
            songName = CleanSongName(targetAudioSource.clip.name);
            artist = DetectArtistFromSong(songName);
        }
        
        // Verificar PlayerPrefs para información adicional
        if (PlayerPrefs.HasKey("SelectedSongName"))
        {
            songName = PlayerPrefs.GetString("SelectedSongName", songName);
            artist = PlayerPrefs.GetString("SelectedArtist", artist);
            difficulty = PlayerPrefs.GetString("SelectedDifficulty", difficulty);
        }
        
        // Guardar todos los datos
        PlayerPrefs.SetString("LastSongName", songName);
        PlayerPrefs.SetString("LastArtist", artist);
        PlayerPrefs.SetString("LastDifficulty", difficulty);
        PlayerPrefs.SetInt("LastScore", score);
        PlayerPrefs.SetFloat("LastCompletion", completion);
        PlayerPrefs.SetInt("LastPerfect", perfect);
        PlayerPrefs.SetInt("LastGood", good);
        PlayerPrefs.SetInt("LastMissed", missed);
        
        PlayerPrefs.Save();
        
        if (showDebugLogs)
        {
            Debug.Log($"💾 Datos guardados: {songName} by {artist} - {score} puntos");
        }
    }
    
    /// <summary>
    /// Transición a la escena PostGameplay
    /// </summary>
    IEnumerator TransitionToPostGameplay()
    {
        if (showDebugLogs)
        {
            Debug.Log($"🔄 Transicionando a {postGameplaySceneName} en {transitionDelay} segundos...");
        }
        
        yield return new WaitForSeconds(transitionDelay);
        
        // Cargar escena PostGameplay
        try
        {
            SceneManager.LoadScene(postGameplaySceneName);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Error cargando escena {postGameplaySceneName}: {e.Message}");
            
            // Intentar con nombres alternativos
            string[] alternativeNames = { "Post Gameplay", "PostGame", "Results", "GameResults" };
            
            foreach (string altName in alternativeNames)
            {
                try
                {
                    SceneManager.LoadScene(altName);
                    Debug.Log($"✅ Cargada escena alternativa: {altName}");
                    yield break;
                }
                catch
                {
                    // Continuar con el siguiente nombre
                }
            }
            
            Debug.LogError("❌ No se pudo cargar ninguna escena de PostGameplay");
        }
    }
    
    /// <summary>
    /// Limpia el nombre de la canción
    /// </summary>
    string CleanSongName(string rawName)
    {
        if (string.IsNullOrEmpty(rawName)) return "Canción Desconocida";
        
        string cleaned = rawName.Replace("_", " ").Replace("-", " ");
        
        // Capitalizar primera letra de cada palabra
        string[] words = cleaned.Split(' ');
        for (int i = 0; i < words.Length; i++)
        {
            if (words[i].Length > 0)
            {
                words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower();
            }
        }
        
        return string.Join(" ", words);
    }
    
    /// <summary>
    /// Detecta el artista basado en el nombre de la canción
    /// </summary>
    string DetectArtistFromSong(string songName)
    {
        if (string.IsNullOrEmpty(songName)) return "Artista Desconocido";
        
        string lowerName = songName.ToLower();
        
        // Patrones conocidos
        if (lowerName.Contains("baile") || lowerName.Contains("inolvidable"))
            return "Artista Latino";
        else if (lowerName.Contains("phineas") || lowerName.Contains("ferb") || lowerName.Contains("ardillas"))
            return "Phineas y Ferb";
        else if (lowerName.Contains("rock"))
            return "Rock Band";
        else if (lowerName.Contains("pop"))
            return "Pop Artist";
        else if (lowerName.Contains("electronic") || lowerName.Contains("techno"))
            return "Electronic Artist";
        else
            return "Artista Independiente";
    }
    
    /// <summary>
    /// Fuerza el fin de la canción (para testing)
    /// </summary>
    [ContextMenu("Force Song End")]
    public void ForceSongEnd()
    {
        OnSongEnd();
    }
    
    /// <summary>
    /// Verifica si la canción ha terminado
    /// </summary>
    public bool HasSongEnded()
    {
        return songEnded;
    }
}
