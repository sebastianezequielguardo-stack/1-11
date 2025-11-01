using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;

/// <summary>
/// Gestiona los records reales del juego
/// Actualiza automáticamente el ScrollView con records guardados
/// </summary>
public class RecordsManager : MonoBehaviour
{
    [Header("ScrollView Configuration")]
    public ScrollRect recordsScrollView;
    public Transform contentParent;
    public GameObject recordItemPrefab;
    
    [Header("Auto Setup")]
    public bool setupOnStart = true;
    public bool autoFindScrollView = true;
    public bool createPrefabIfNeeded = true;
    
    [Header("Records Settings")]
    public int maxRecordsToShow = 10;
    public bool sortByScore = true;
    
    private List<GameRecord> gameRecords = new List<GameRecord>();
    
    [System.Serializable]
    public class GameRecord
    {
        public string songName;
        public string artist;
        public string difficulty;
        public int score;
        public float completion;
        public int perfect;
        public int good;
        public int missed;
        public string date;
        
        public GameRecord(string song, string art, string diff, int sc, float comp, int perf, int gd, int miss, string dt)
        {
            songName = song;
            artist = art;
            difficulty = diff;
            score = sc;
            completion = comp;
            perfect = perf;
            good = gd;
            missed = miss;
            date = dt;
        }
    }
    
    void Start()
    {
        if (setupOnStart)
        {
            SetupRecordsManager();
        }
    }
    
    /// <summary>
    /// Configura el gestor de records
    /// </summary>
    [ContextMenu("Setup Records Manager")]
    public void SetupRecordsManager()
    {
        Debug.Log("📊 Configurando gestor de records...");
        
        // Buscar ScrollView automáticamente
        if (autoFindScrollView && recordsScrollView == null)
        {
            FindRecordsScrollView();
        }
        
        // Configurar content parent
        if (contentParent == null && recordsScrollView != null)
        {
            contentParent = recordsScrollView.content;
        }
        
        // Crear prefab si es necesario
        if (createPrefabIfNeeded && recordItemPrefab == null)
        {
            CreateRecordItemPrefab();
        }
        
        // Cargar y mostrar records
        LoadAllRecords();
        UpdateScrollView();
        
        Debug.Log("✅ Gestor de records configurado");
    }
    
    /// <summary>
    /// Busca automáticamente el ScrollView de records
    /// </summary>
    void FindRecordsScrollView()
    {
        Debug.Log("🔍 Buscando ScrollView de records...");
        
        // Buscar por nombres comunes
        string[] scrollViewNames = {
            "RecordsScrollView", "Records ScrollView", "Records Scroll View",
            "ScrollView Records", "Scroll View Records", "HighScores",
            "High Scores", "Leaderboard", "ScoreScrollView"
        };
        
        foreach (string name in scrollViewNames)
        {
            GameObject scrollObj = GameObject.Find(name);
            if (scrollObj != null)
            {
                recordsScrollView = scrollObj.GetComponent<ScrollRect>();
                if (recordsScrollView != null)
                {
                    Debug.Log($"✅ ScrollView encontrado: {name}");
                    return;
                }
            }
        }
        
        // Buscar cualquier ScrollRect disponible
        ScrollRect[] allScrollRects = FindObjectsByType<ScrollRect>(FindObjectsSortMode.None);
        if (allScrollRects.Length > 0)
        {
            recordsScrollView = allScrollRects[0];
            Debug.Log($"⚠️ Usando primer ScrollRect disponible: {recordsScrollView.name}");
        }
        else
        {
            Debug.LogWarning("⚠️ No se encontró ningún ScrollRect");
        }
    }
    
    /// <summary>
    /// Crea un prefab para los items de record
    /// </summary>
    void CreateRecordItemPrefab()
    {
        Debug.Log("🔨 Creando prefab de record item...");
        
        // Crear GameObject base
        GameObject prefab = new GameObject("RecordItem");
        RectTransform rectTransform = prefab.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(500f, 70f);
        
        // Agregar fondo
        Image bgImage = prefab.AddComponent<Image>();
        bgImage.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);
        
        // Crear layout horizontal
        HorizontalLayoutGroup layout = prefab.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 10f;
        layout.padding = new RectOffset(15, 15, 10, 10);
        layout.childControlWidth = false;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;
        
        // Crear elementos de texto
        CreateTextElement(prefab, "RankText", "1.", 40f, Color.yellow, TextAnchor.MiddleCenter, 18);
        CreateTextElement(prefab, "SongText", "Song Name", 200f, Color.white, TextAnchor.MiddleLeft, 16);
        CreateTextElement(prefab, "ScoreText", "999,999", 100f, Color.cyan, TextAnchor.MiddleCenter, 16);
        CreateTextElement(prefab, "CompletionText", "100%", 80f, Color.green, TextAnchor.MiddleCenter, 14);
        CreateTextElement(prefab, "DateText", "2024-10-13", 100f, Color.gray, TextAnchor.MiddleCenter, 12);
        
        recordItemPrefab = prefab;
        Debug.Log("✅ Prefab de record creado");
    }
    
    /// <summary>
    /// Crea un elemento de texto para el prefab
    /// </summary>
    void CreateTextElement(GameObject parent, string name, string text, float width, Color color, TextAnchor alignment, int fontSize)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent.transform);
        
        // Intentar usar TextMeshPro primero
        TextMeshProUGUI tmpText = textObj.AddComponent<TextMeshProUGUI>();
        tmpText.text = text;
        tmpText.color = color;
        tmpText.fontSize = fontSize;
        tmpText.alignment = ConvertTextAnchor(alignment);
        
        // Configurar RectTransform
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.sizeDelta = new Vector2(width, 0f);
        
        // Agregar LayoutElement
        LayoutElement layoutElement = textObj.AddComponent<LayoutElement>();
        layoutElement.preferredWidth = width;
        layoutElement.flexibleWidth = 0f;
    }
    
    /// <summary>
    /// Convierte TextAnchor a TextAlignmentOptions para TextMeshPro
    /// </summary>
    TextAlignmentOptions ConvertTextAnchor(TextAnchor anchor)
    {
        switch (anchor)
        {
            case TextAnchor.MiddleLeft: return TextAlignmentOptions.MidlineLeft;
            case TextAnchor.MiddleCenter: return TextAlignmentOptions.Midline;
            case TextAnchor.MiddleRight: return TextAlignmentOptions.MidlineRight;
            default: return TextAlignmentOptions.Midline;
        }
    }
    
    /// <summary>
    /// Carga todos los records guardados
    /// </summary>
    void LoadAllRecords()
    {
        Debug.Log("📈 Cargando records guardados...");
        
        gameRecords.Clear();
        
        // Cargar records desde PlayerPrefs
        string recordsList = PlayerPrefs.GetString("RecordsList", "");
        
        if (!string.IsNullOrEmpty(recordsList))
        {
            string[] recordKeys = recordsList.Split(',');
            
            foreach (string key in recordKeys)
            {
                if (PlayerPrefs.HasKey($"{key}_Song"))
                {
                    GameRecord record = new GameRecord(
                        PlayerPrefs.GetString($"{key}_Song", "Unknown"),
                        PlayerPrefs.GetString($"{key}_Artist", "Unknown Artist"),
                        PlayerPrefs.GetString($"{key}_Difficulty", "Medium"),
                        PlayerPrefs.GetInt($"{key}_Score", 0),
                        PlayerPrefs.GetFloat($"{key}_Completion", 0f),
                        PlayerPrefs.GetInt($"{key}_Perfect", 0),
                        PlayerPrefs.GetInt($"{key}_Good", 0),
                        PlayerPrefs.GetInt($"{key}_Missed", 0),
                        PlayerPrefs.GetString($"{key}_Date", "2024-10-13")
                    );
                    
                    gameRecords.Add(record);
                }
            }
        }
        
        // Si no hay records, agregar algunos de ejemplo
        if (gameRecords.Count == 0)
        {
            AddSampleRecords();
        }
        
        // Ordenar records
        if (sortByScore)
        {
            gameRecords = gameRecords.OrderByDescending(r => r.score).ToList();
        }
        
        // Limitar cantidad
        if (gameRecords.Count > maxRecordsToShow)
        {
            gameRecords = gameRecords.Take(maxRecordsToShow).ToList();
        }
        
        Debug.Log($"✅ {gameRecords.Count} records cargados");
    }
    
    /// <summary>
    /// Agrega records de ejemplo
    /// </summary>
    void AddSampleRecords()
    {
        gameRecords.Add(new GameRecord("Baile Inolvidable", "Artista Latino", "Medium", 187500, 94.2f, 142, 38, 12, "2024-10-13"));
        gameRecords.Add(new GameRecord("Rock Anthem", "Metal Band", "Hard", 165800, 89.7f, 128, 45, 18, "2024-10-12"));
        gameRecords.Add(new GameRecord("Dance Beat", "Electronic Artist", "Easy", 143200, 96.1f, 156, 22, 8, "2024-10-11"));
        gameRecords.Add(new GameRecord("Jazz Fusion", "Jazz Master", "Expert", 201300, 87.4f, 134, 52, 25, "2024-10-10"));
        gameRecords.Add(new GameRecord("Pop Hit", "Pop Star", "Medium", 129600, 92.8f, 118, 35, 15, "2024-10-09"));
    }
    
    /// <summary>
    /// Actualiza el ScrollView con los records
    /// </summary>
    void UpdateScrollView()
    {
        if (contentParent == null || recordItemPrefab == null)
        {
            Debug.LogError("❌ No se puede actualizar ScrollView - faltan componentes");
            return;
        }
        
        Debug.Log("📝 Actualizando ScrollView con records...");
        
        // Limpiar items existentes
        foreach (Transform child in contentParent)
        {
            DestroyImmediate(child.gameObject);
        }
        
        // Crear items para cada record
        for (int i = 0; i < gameRecords.Count; i++)
        {
            GameRecord record = gameRecords[i];
            
            // Instanciar prefab
            GameObject recordItem = Instantiate(recordItemPrefab, contentParent);
            recordItem.name = $"Record_{i + 1}";
            
            // Configurar textos
            ConfigureRecordItem(recordItem, record, i + 1);
            
            // Configurar colores especiales para top 3
            Image bgImage = recordItem.GetComponent<Image>();
            if (bgImage != null)
            {
                if (i == 0) bgImage.color = new Color(1f, 0.8f, 0f, 0.9f); // Oro
                else if (i == 1) bgImage.color = new Color(0.7f, 0.7f, 0.7f, 0.9f); // Plata
                else if (i == 2) bgImage.color = new Color(0.8f, 0.5f, 0.2f, 0.9f); // Bronce
                else bgImage.color = new Color(0.1f, 0.1f, 0.1f, 0.9f); // Normal
            }
        }
        
        // Ajustar tamaño del content
        AdjustContentSize();
        
        Debug.Log($"✅ ScrollView actualizado con {gameRecords.Count} records");
    }
    
    /// <summary>
    /// Configura un item de record individual
    /// </summary>
    void ConfigureRecordItem(GameObject item, GameRecord record, int rank)
    {
        // Buscar elementos de texto por nombre
        Transform rankText = item.transform.Find("RankText");
        Transform songText = item.transform.Find("SongText");
        Transform scoreText = item.transform.Find("ScoreText");
        Transform completionText = item.transform.Find("CompletionText");
        Transform dateText = item.transform.Find("DateText");
        
        // Configurar textos
        if (rankText != null)
        {
            TextMeshProUGUI tmp = rankText.GetComponent<TextMeshProUGUI>();
            if (tmp != null) tmp.text = $"{rank}.";
        }
        
        if (songText != null)
        {
            TextMeshProUGUI tmp = songText.GetComponent<TextMeshProUGUI>();
            if (tmp != null) tmp.text = record.songName;
        }
        
        if (scoreText != null)
        {
            TextMeshProUGUI tmp = scoreText.GetComponent<TextMeshProUGUI>();
            if (tmp != null) tmp.text = record.score.ToString("N0");
        }
        
        if (completionText != null)
        {
            TextMeshProUGUI tmp = completionText.GetComponent<TextMeshProUGUI>();
            if (tmp != null) tmp.text = $"{record.completion:F1}%";
        }
        
        if (dateText != null)
        {
            TextMeshProUGUI tmp = dateText.GetComponent<TextMeshProUGUI>();
            if (tmp != null) tmp.text = record.date;
        }
    }
    
    /// <summary>
    /// Ajusta el tamaño del content del ScrollView
    /// </summary>
    void AdjustContentSize()
    {
        if (contentParent != null)
        {
            RectTransform contentRect = contentParent.GetComponent<RectTransform>();
            if (contentRect != null)
            {
                float itemHeight = 75f; // Altura de cada item
                float totalHeight = gameRecords.Count * itemHeight;
                contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, totalHeight);
            }
        }
    }
    
    /// <summary>
    /// Agrega un nuevo record
    /// </summary>
    public void AddNewRecord(string songName, string artist, string difficulty, int score, float completion, int perfect, int good, int missed)
    {
        GameRecord newRecord = new GameRecord(
            songName, artist, difficulty, score, completion, 
            perfect, good, missed, System.DateTime.Now.ToString("yyyy-MM-dd")
        );
        
        gameRecords.Add(newRecord);
        
        // Reordenar
        if (sortByScore)
        {
            gameRecords = gameRecords.OrderByDescending(r => r.score).ToList();
        }
        
        // Limitar
        if (gameRecords.Count > maxRecordsToShow)
        {
            gameRecords = gameRecords.Take(maxRecordsToShow).ToList();
        }
        
        // Actualizar UI
        UpdateScrollView();
        
        Debug.Log($"✅ Nuevo record agregado: {songName} - {score:N0}");
    }
    
    /// <summary>
    /// Refrescar records desde PlayerPrefs
    /// </summary>
    [ContextMenu("Refresh Records")]
    public void RefreshRecords()
    {
        LoadAllRecords();
        UpdateScrollView();
        Debug.Log("🔄 Records refrescados desde PlayerPrefs");
    }
    
    /// <summary>
    /// Mostrar información del sistema
    /// </summary>
    [ContextMenu("Show Records Info")]
    public void ShowRecordsInfo()
    {
        Debug.Log("📊 INFORMACIÓN DEL SISTEMA DE RECORDS:");
        Debug.Log("═══════════════════════════════════════");
        
        Debug.Log($"ScrollView: {(recordsScrollView != null ? "✅ ENCONTRADO" : "❌ FALTANTE")}");
        Debug.Log($"Content Parent: {(contentParent != null ? "✅ ENCONTRADO" : "❌ FALTANTE")}");
        Debug.Log($"Record Prefab: {(recordItemPrefab != null ? "✅ ENCONTRADO" : "❌ FALTANTE")}");
        Debug.Log($"Records cargados: {gameRecords.Count}");
        
        Debug.Log("\n📈 TOP RECORDS:");
        for (int i = 0; i < Mathf.Min(5, gameRecords.Count); i++)
        {
            GameRecord record = gameRecords[i];
            Debug.Log($"{i + 1}. {record.songName} - {record.score:N0} ({record.completion:F1}%)");
        }
    }
}
