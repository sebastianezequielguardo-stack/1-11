using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class RankingData
{
    public string playerName;
    public int score;

    public RankingData(string name, int score)
    {
        this.playerName = name;
        this.score = score;
    }
}

[System.Serializable]
public class SongRanking
{
    public string songID;
    public List<RankingData> entries = new List<RankingData>();
}

[System.Serializable]
public class RankingCollection
{
    public List<SongRanking> songRankings = new List<SongRanking>();
}

public class RankingManager : MonoBehaviour
{
    public static RankingManager Instance;

    [Header("UI References")]
    public GameObject rankingEntryPrefab;
    public Transform contentParent;
    public GameObject rankingPanel;
    public TextMeshProUGUI songTitleText; // Texto para mostrar el nombre de la canción

    [Header("Input Panel")]
    public GameObject inputPanel;
    public TMP_InputField nameInputField;

    [Header("Settings")]
    public int maxEntries = 10;

    private RankingCollection allRankings = new RankingCollection();
    private string currentSongID = "";
    private string currentSongName = "";
    private int currentScore = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Opcional: mantener entre escenas
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        LoadAllRankings();

        if (inputPanel != null) inputPanel.SetActive(false);
        if (rankingPanel != null) rankingPanel.SetActive(false);
    }

    // Establecer la canción actual antes de mostrar el ranking
    public void SetCurrentSong(string songID, string songName)
    {
        currentSongID = songID;
        currentSongName = songName;

        // Actualizar el título en la UI
        if (songTitleText != null)
        {
            songTitleText.text = songName;
        }
    }

    // Sobrecarga para usar SongData directamente
    public void SetCurrentSong(SongData song)
    {
        if (song != null)
        {
            SetCurrentSong(song.songID, song.songName);
        }
    }

    // Obtener o crear el ranking de una canción específica
    SongRanking GetSongRanking(string songID)
    {
        SongRanking ranking = allRankings.songRankings.Find(x => x.songID == songID);

        if (ranking == null)
        {
            ranking = new SongRanking();
            ranking.songID = songID;
            allRankings.songRankings.Add(ranking);
        }

        return ranking;
    }

    // Verificar si la puntuación califica para el ranking de la canción actual
    public bool IsHighScore(int score)
    {
        if (string.IsNullOrEmpty(currentSongID))
        {
            Debug.LogWarning("No se ha establecido una canción actual");
            return false;
        }

        SongRanking ranking = GetSongRanking(currentSongID);

        if (ranking.entries.Count < maxEntries)
            return true;

        return score > ranking.entries[ranking.entries.Count - 1].score;
    }

    // Mostrar panel para ingresar nombre
    public void ShowNameInput(int score)
    {
        currentScore = score;

        // Obtener el nombre guardado del jugador
        string playerName = PlayerPrefs.GetString("PlayerName", "Player");

        // Agregar directamente el score con el nombre guardado
        AddScore(playerName, currentScore);

        // Mostrar el ranking
        ShowRanking();
    }

    // Llamar cuando el usuario confirma su nombre
    public void OnNameSubmitted()
    {
        string playerName = PlayerPrefs.GetString("PlayerName", "Player");

        AddScore(playerName, currentScore);

        if (inputPanel != null) inputPanel.SetActive(false);
        ShowRanking();
    }

    // Añadir nueva puntuación al ranking de la canción actual
    public void AddScore(string playerName, int score)
    {
        if (string.IsNullOrEmpty(currentSongID))
        {
            Debug.LogError("No se puede añadir puntuación sin establecer una canción");
            return;
        }

        SongRanking ranking = GetSongRanking(currentSongID);

        RankingData newEntry = new RankingData(playerName, score);
        ranking.entries.Add(newEntry);

        // Ordenar por puntuación descendente
        ranking.entries = ranking.entries.OrderByDescending(x => x.score).ToList();

        // Limitar al máximo de entradas
        if (ranking.entries.Count > maxEntries)
        {
            ranking.entries.RemoveAt(ranking.entries.Count - 1);
        }

        SaveAllRankings();
        UpdateRankingUI();
    }

    // Actualizar la UI del ranking de la canción actual
    void UpdateRankingUI()
    {
        // Limpiar entradas anteriores
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        if (string.IsNullOrEmpty(currentSongID))
        {
            Debug.LogWarning("No hay canción seleccionada para mostrar ranking");
            return;
        }

        SongRanking ranking = GetSongRanking(currentSongID);

        // Crear nuevas entradas
        for (int i = 0; i < ranking.entries.Count; i++)
        {
            GameObject entryObj = Instantiate(rankingEntryPrefab, contentParent);
            RankingEntryUI entryUI = entryObj.GetComponent<RankingEntryUI>();

            if (entryUI != null)
            {
                entryUI.SetData(i + 1, ranking.entries[i].playerName, ranking.entries[i].score);
            }
        }

        // Si no hay entradas, mostrar mensaje
        if (ranking.entries.Count == 0)
        {
            // Opcional: crear un texto que diga "No hay puntuaciones aún"
        }
    }

    // Mostrar el ranking de la canción actual
    public void ShowRanking()
    {
        UpdateRankingUI();

        if (rankingPanel != null)
        {
            rankingPanel.SetActive(true);
        }
    }

    // Mostrar ranking de una canción específica
    public void ShowRankingForSong(string songID, string songName)
    {
        SetCurrentSong(songID, songName);
        ShowRanking();
    }

    // Sobrecarga para usar SongData
    public void ShowRankingForSong(SongData song)
    {
        if (song != null)
        {
            ShowRankingForSong(song.songID, song.songName);
        }
    }

    // Ocultar el ranking
    public void HideRanking()
    {
        if (rankingPanel != null)
        {
            rankingPanel.SetActive(false);
        }
    }

    // Obtener el mejor score de una canción específica
    public int GetBestScore(string songID)
    {
        SongRanking ranking = GetSongRanking(songID);

        if (ranking.entries.Count > 0)
        {
            return ranking.entries[0].score;
        }

        return 0;
    }

    // Guardar todos los rankings en PlayerPrefs
    void SaveAllRankings()
    {
        string json = JsonUtility.ToJson(allRankings);
        PlayerPrefs.SetString("AllRankings", json);
        PlayerPrefs.Save();
    }

    // Cargar todos los rankings desde PlayerPrefs
    void LoadAllRankings()
    {
        if (PlayerPrefs.HasKey("AllRankings"))
        {
            string json = PlayerPrefs.GetString("AllRankings");
            allRankings = JsonUtility.FromJson<RankingCollection>(json);
        }
        else
        {
            allRankings = new RankingCollection();
        }
    }

    // Limpiar ranking de una canción específica
    public void ClearSongRanking(string songID)
    {
        SongRanking ranking = allRankings.songRankings.Find(x => x.songID == songID);

        if (ranking != null)
        {
            ranking.entries.Clear();
            SaveAllRankings();

            if (currentSongID == songID)
            {
                UpdateRankingUI();
            }
        }
    }

    // Limpiar todos los rankings (útil para testing)
    public void ClearAllRankings()
    {
        allRankings.songRankings.Clear();
        PlayerPrefs.DeleteKey("AllRankings");
        UpdateRankingUI();
    }
}