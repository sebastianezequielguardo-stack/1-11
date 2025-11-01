using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Sistema de ScrollView para mostrar records en MainMenu
/// Carga y muestra automáticamente los mejores puntajes
/// </summary>
public class MainMenuRecordsScrollView : MonoBehaviour
{
    [Header("ScrollView Configuration")]
    public ScrollRect recordsScrollView;
    public GameObject recordItemPrefab;
    public Transform contentParent;
    
    [Header("Auto Setup")]
    public bool setupOnStart = true;
    public bool autoFindScrollView = true;
    
    [Header("Records Settings")]
    public int maxRecordsToShow = 10;
    public bool sortByScore = true; // true = mayor a menor, false = menor a mayor
    
    [Header("Sample Data")]
    public bool useSampleData = true;
    public bool clearExistingRecords = true;
    
    // Lista de records
    private List<GameRecord> gameRecords = new List<GameRecord>();
    
    [System.Serializable]
    public class GameRecord
    {
        public string playerName;
        public int score;
        public float accuracy;
        public string songName;
        public string date;
        
        public GameRecord(string name, int sc, float acc, string song, string dt)
        {
            playerName = name;
            score = sc;
            accuracy = acc;
            songName = song;
            date = dt;
        }
    }
    
    void Start()
    {
        if (setupOnStart)
        {
            SetupRecordsScrollView();
        }
    }
    
    /// <summary>
    /// Configura automáticamente el ScrollView de records
    /// </summary>
    [ContextMenu("Setup Records ScrollView")]
    public void SetupRecordsScrollView()
    {
        Debug.Log("📜 Configurando ScrollView de records...");
        
        // Buscar ScrollView automáticamente si no está asignado
        if (recordsScrollView == null && autoFindScrollView)
        {
            FindRecordsScrollView();
        }
        
        if (recordsScrollView == null)
        {
            Debug.LogError("❌ No se encontró ScrollView de records");
            return;
        }
        
        // Configurar content parent
        if (contentParent == null)
        {
            contentParent = recordsScrollView.content;
        }
        
        // Crear prefab si no existe
        if (recordItemPrefab == null)
        {
            CreateRecordItemPrefab();
        }
        
        // Cargar records
        LoadGameRecords();
        
        // Poblar ScrollView
        PopulateScrollView();
        
        Debug.Log("✅ ScrollView de records configurado exitosamente");
    }
    
    /// <summary>
    /// Busca automáticamente el ScrollView de records
    /// </summary>
    void FindRecordsScrollView()
    {
        Debug.Log("🔍 Buscando ScrollView de records automáticamente...");
        
        // Buscar por nombre
        GameObject scrollViewObj = GameObject.Find("RecordsScrollView");
        if (scrollViewObj == null) scrollViewObj = GameObject.Find("Records ScrollView");
        if (scrollViewObj == null) scrollViewObj = GameObject.Find("Records Scroll View");
        if (scrollViewObj == null) scrollViewObj = GameObject.Find("ScrollView Records");
        if (scrollViewObj == null) scrollViewObj = GameObject.Find("Scroll View Records");
        if (scrollViewObj == null) scrollViewObj = GameObject.Find("HighScores");
        if (scrollViewObj == null) scrollViewObj = GameObject.Find("High Scores");
        if (scrollViewObj == null) scrollViewObj = GameObject.Find("Leaderboard");
        
        if (scrollViewObj != null)
        {
            recordsScrollView = scrollViewObj.GetComponent<ScrollRect>();
            if (recordsScrollView != null)
            {
                Debug.Log($"✅ ScrollView encontrado: {scrollViewObj.name}");
                return;
            }
        }
        
        // Buscar cualquier ScrollRect en la escena
        ScrollRect[] allScrollRects = FindObjectsByType<ScrollRect>(FindObjectsSortMode.None);
        
        foreach (ScrollRect scroll in allScrollRects)
        {
            string name = scroll.name.ToLower();
            if (name.Contains("record") || name.Contains("score") || name.Contains("leader"))
            {
                recordsScrollView = scroll;
                Debug.Log($"✅ ScrollView encontrado por contenido: {scroll.name}");
                return;
            }
        }
        
        // Si no encuentra ninguno específico, usar el primero disponible
        if (allScrollRects.Length > 0)
        {
            recordsScrollView = allScrollRects[0];
            Debug.Log($"⚠️ Usando primer ScrollRect disponible: {recordsScrollView.name}");
        }
        else
        {
            Debug.LogWarning("⚠️ No se encontró ningún ScrollRect en la escena");
        }
    }
    
    /// <summary>
    /// Crea un prefab básico para los items de record
    /// </summary>
    void CreateRecordItemPrefab()
    {
        Debug.Log("🔨 Creando prefab de record item...");
        
        // Crear GameObject base
        GameObject prefab = new GameObject("RecordItem");
        prefab.AddComponent<RectTransform>();
        
        // Agregar Image de fondo
        Image bgImage = prefab.AddComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        
        // Configurar RectTransform
        RectTransform rectTransform = prefab.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(400f, 60f);
        
        // Crear texto para el nombre del jugador
        GameObject playerNameObj = new GameObject("PlayerName");
        playerNameObj.transform.SetParent(prefab.transform);
        Text playerNameText = playerNameObj.AddComponent<Text>();
        playerNameText.text = "Player Name";
        playerNameText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        playerNameText.fontSize = 16;
        playerNameText.color = Color.white;
        
        RectTransform playerNameRect = playerNameObj.GetComponent<RectTransform>();
        playerNameRect.anchorMin = new Vector2(0f, 0.5f);
        playerNameRect.anchorMax = new Vector2(0.4f, 0.5f);
        playerNameRect.anchoredPosition = Vector2.zero;
        playerNameRect.sizeDelta = new Vector2(0f, 30f);
        
        // Crear texto para el puntaje
        GameObject scoreObj = new GameObject("Score");
        scoreObj.transform.SetParent(prefab.transform);
        Text scoreText = scoreObj.AddComponent<Text>();
        scoreText.text = "999,999";
        scoreText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        scoreText.fontSize = 16;
        scoreText.color = Color.yellow;
        scoreText.alignment = TextAnchor.MiddleCenter;
        
        RectTransform scoreRect = scoreObj.GetComponent<RectTransform>();
        scoreRect.anchorMin = new Vector2(0.4f, 0.5f);
        scoreRect.anchorMax = new Vector2(0.7f, 0.5f);
        scoreRect.anchoredPosition = Vector2.zero;
        scoreRect.sizeDelta = new Vector2(0f, 30f);
        
        // Crear texto para la precisión
        GameObject accuracyObj = new GameObject("Accuracy");
        accuracyObj.transform.SetParent(prefab.transform);
        Text accuracyText = accuracyObj.AddComponent<Text>();
        accuracyText.text = "100%";
        accuracyText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        accuracyText.fontSize = 14;
        accuracyText.color = Color.green;
        accuracyText.alignment = TextAnchor.MiddleCenter;
        
        RectTransform accuracyRect = accuracyObj.GetComponent<RectTransform>();
        accuracyRect.anchorMin = new Vector2(0.7f, 0.5f);
        accuracyRect.anchorMax = new Vector2(1f, 0.5f);
        accuracyRect.anchoredPosition = Vector2.zero;
        accuracyRect.sizeDelta = new Vector2(0f, 30f);
        
        recordItemPrefab = prefab;
        Debug.Log("✅ Prefab de record item creado");
    }
    
    /// <summary>
    /// Carga los records del juego
    /// </summary>
    void LoadGameRecords()
    {
        Debug.Log("📊 Cargando records del juego...");
        
        gameRecords.Clear();
        
        if (useSampleData)
        {
            // Crear datos de ejemplo
            gameRecords.Add(new GameRecord("Player1", 245680, 98.5f, "Baile Inolvidable", "2024-10-13"));
            gameRecords.Add(new GameRecord("ProGamer", 198750, 95.2f, "Epic Song", "2024-10-12"));
            gameRecords.Add(new GameRecord("MusicMaster", 187340, 92.8f, "Rock Anthem", "2024-10-11"));
            gameRecords.Add(new GameRecord("RhythmKing", 176890, 89.4f, "Dance Beat", "2024-10-10"));
            gameRecords.Add(new GameRecord("BeatHero", 165420, 87.1f, "Metal Storm", "2024-10-09"));
            gameRecords.Add(new GameRecord("NoteNinja", 154780, 84.6f, "Jazz Fusion", "2024-10-08"));
            gameRecords.Add(new GameRecord("SoundWave", 143210, 82.3f, "Electronic", "2024-10-07"));
            gameRecords.Add(new GameRecord("Melody", 132650, 79.8f, "Classical", "2024-10-06"));
            gameRecords.Add(new GameRecord("Harmony", 121980, 77.2f, "Pop Hit", "2024-10-05"));
            gameRecords.Add(new GameRecord("Tempo", 110340, 74.5f, "Indie Rock", "2024-10-04"));
        }
        
        // TODO: Aquí se cargarían los records reales desde PlayerPrefs o archivo
        // LoadRealGameRecords();
        
        // Ordenar records
        if (sortByScore)
        {
            gameRecords = gameRecords.OrderByDescending(r => r.score).ToList();
        }
        else
        {
            gameRecords = gameRecords.OrderBy(r => r.score).ToList();
        }
        
        // Limitar cantidad
        if (gameRecords.Count > maxRecordsToShow)
        {
            gameRecords = gameRecords.Take(maxRecordsToShow).ToList();
        }
        
        Debug.Log($"✅ {gameRecords.Count} records cargados");
    }
    
    /// <summary>
    /// Puebla el ScrollView con los records
    /// </summary>
    void PopulateScrollView()
    {
        if (contentParent == null || recordItemPrefab == null)
        {
            Debug.LogError("❌ No se puede poblar ScrollView - faltan componentes");
            return;
        }
        
        Debug.Log("📝 Poblando ScrollView con records...");
        
        // Limpiar items existentes si es necesario
        if (clearExistingRecords)
        {
            foreach (Transform child in contentParent)
            {
                DestroyImmediate(child.gameObject);
            }
        }
        
        // Crear items para cada record
        for (int i = 0; i < gameRecords.Count; i++)
        {
            GameRecord record = gameRecords[i];
            
            // Instanciar prefab
            GameObject recordItem = Instantiate(recordItemPrefab, contentParent);
            recordItem.name = $"Record_{i + 1}";
            
            // Configurar textos
            Text[] texts = recordItem.GetComponentsInChildren<Text>();
            
            foreach (Text text in texts)
            {
                switch (text.name)
                {
                    case "PlayerName":
                        text.text = $"{i + 1}. {record.playerName}";
                        break;
                    case "Score":
                        text.text = record.score.ToString("N0");
                        break;
                    case "Accuracy":
                        text.text = $"{record.accuracy:F1}%";
                        break;
                }
            }
            
            // Alternar colores de fondo
            Image bgImage = recordItem.GetComponent<Image>();
            if (bgImage != null)
            {
                if (i % 2 == 0)
                {
                    bgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
                }
                else
                {
                    bgImage.color = new Color(0.15f, 0.15f, 0.15f, 0.8f);
                }
                
                // Destacar top 3
                if (i == 0) bgImage.color = new Color(1f, 0.8f, 0f, 0.8f); // Oro
                else if (i == 1) bgImage.color = new Color(0.7f, 0.7f, 0.7f, 0.8f); // Plata
                else if (i == 2) bgImage.color = new Color(0.8f, 0.5f, 0.2f, 0.8f); // Bronce
            }
        }
        
        // Ajustar tamaño del content
        RectTransform contentRect = contentParent.GetComponent<RectTransform>();
        if (contentRect != null)
        {
            float itemHeight = 65f; // Altura de cada item + spacing
            float totalHeight = gameRecords.Count * itemHeight;
            contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, totalHeight);
        }
        
        Debug.Log($"✅ ScrollView poblado con {gameRecords.Count} records");
    }
    
    /// <summary>
    /// Agregar un nuevo record
    /// </summary>
    public void AddNewRecord(string playerName, int score, float accuracy, string songName)
    {
        GameRecord newRecord = new GameRecord(
            playerName, 
            score, 
            accuracy, 
            songName, 
            System.DateTime.Now.ToString("yyyy-MM-dd")
        );
        
        gameRecords.Add(newRecord);
        
        // Reordenar y limitar
        if (sortByScore)
        {
            gameRecords = gameRecords.OrderByDescending(r => r.score).ToList();
        }
        
        if (gameRecords.Count > maxRecordsToShow)
        {
            gameRecords = gameRecords.Take(maxRecordsToShow).ToList();
        }
        
        // Actualizar ScrollView
        PopulateScrollView();
        
        Debug.Log($"✅ Nuevo record agregado: {playerName} - {score}");
    }
    
    /// <summary>
    /// Refrescar el ScrollView
    /// </summary>
    [ContextMenu("Refresh Records")]
    public void RefreshRecords()
    {
        LoadGameRecords();
        PopulateScrollView();
        Debug.Log("🔄 Records refrescados");
    }
    
    /// <summary>
    /// Mostrar información del sistema
    /// </summary>
    [ContextMenu("Show ScrollView Info")]
    public void ShowScrollViewInfo()
    {
        Debug.Log("📊 INFORMACIÓN DEL SCROLLVIEW DE RECORDS:");
        Debug.Log("═══════════════════════════════════════");
        
        Debug.Log($"ScrollView asignado: {(recordsScrollView != null ? "✅ SÍ" : "❌ NO")}");
        if (recordsScrollView != null)
        {
            Debug.Log($"   Nombre: {recordsScrollView.name}");
            Debug.Log($"   Activo: {recordsScrollView.gameObject.activeInHierarchy}");
        }
        
        Debug.Log($"Content Parent: {(contentParent != null ? "✅ SÍ" : "❌ NO")}");
        Debug.Log($"Record Prefab: {(recordItemPrefab != null ? "✅ SÍ" : "❌ NO")}");
        Debug.Log($"Records cargados: {gameRecords.Count}");
        Debug.Log($"Max records a mostrar: {maxRecordsToShow}");
        
        if (contentParent != null)
        {
            Debug.Log($"Items en ScrollView: {contentParent.childCount}");
        }
    }
}
