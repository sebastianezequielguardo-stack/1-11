using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class SongLoader : MonoBehaviour
{
    [Header("UI Components")]
    public Transform contentParent;
    public GameObject songButtonPrefab;
    public GameObject loadingIndicator;
    
    [Header("Song Selection")]
    public Color selectedColor = Color.green;
    public Color defaultColor = Color.white;
    
    [Header("Events")]
    public System.Action<SongData> OnSongSelected;
    
    private List<SongData> availableSongs = new List<SongData>();
    private GameObject currentSelectedButton;
    
    void Start()
    {
        LoadSongs();
    }
    
    void LoadSongs()
    {
        if (loadingIndicator != null)
            loadingIndicator.SetActive(true);
            
        string songsPath = Path.Combine(Application.streamingAssetsPath, "Songs");

        if (!Directory.Exists(songsPath))
        {
            Debug.LogError("❌ Carpeta 'Songs' no encontrada en StreamingAssets.");
            CreateNoSongsMessage();
            return;
        }

        // Clear previous content
        ClearSongList();
        availableSongs.Clear();

        string[] songFolders = Directory.GetDirectories(songsPath);

        if (songFolders.Length == 0)
        {
            CreateNoSongsMessage();
            return;
        }

        foreach (string folder in songFolders)
        {
            SongData songData = LoadSongData(folder);
            if (songData != null)
            {
                availableSongs.Add(songData);
                CreateSongButton(songData);
            }
        }
        
        if (loadingIndicator != null)
            loadingIndicator.SetActive(false);
            
        Debug.Log($"🎵 Loaded {availableSongs.Count} songs");
    }
    
    void ClearSongList()
    {
        if (contentParent == null) return;
        
        foreach (Transform child in contentParent)
        {
            if (Application.isPlaying)
                Destroy(child.gameObject);
            else
                DestroyImmediate(child.gameObject);
        }
        currentSelectedButton = null;
    }
    
    SongData LoadSongData(string folderPath)
    {
        string songName = Path.GetFileName(folderPath);
        string artist = "Unknown Artist";
        string album = "";
        int year = 0;
        
        // Try to load song.ini for metadata
        string iniPath = Path.Combine(folderPath, "song.ini");
        if (File.Exists(iniPath))
        {
            try
            {
                string iniContent = File.ReadAllText(iniPath);
                
                // Extract song name
                Match nameMatch = Regex.Match(iniContent, @"name\s*=\s*(.+)", RegexOptions.IgnoreCase);
                if (nameMatch.Success)
                    songName = nameMatch.Groups[1].Value.Trim().Trim('"');
                
                // Extract artist
                Match artistMatch = Regex.Match(iniContent, @"artist\s*=\s*(.+)", RegexOptions.IgnoreCase);
                if (artistMatch.Success)
                    artist = artistMatch.Groups[1].Value.Trim().Trim('"');
                    
                // Extract album
                Match albumMatch = Regex.Match(iniContent, @"album\s*=\s*(.+)", RegexOptions.IgnoreCase);
                if (albumMatch.Success)
                    album = albumMatch.Groups[1].Value.Trim().Trim('"');
                    
                // Extract year
                Match yearMatch = Regex.Match(iniContent, @"year\s*=\s*(\d+)", RegexOptions.IgnoreCase);
                if (yearMatch.Success && int.TryParse(yearMatch.Groups[1].Value, out int parsedYear))
                    year = parsedYear;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"⚠️ Error reading song.ini for {songName}: {e.Message}");
            }
        }
        
        // Check for required files
        string audioPath = FindAudioFile(folderPath);
        string chartPath = Path.Combine(folderPath, "notes.chart");
        
        if (string.IsNullOrEmpty(audioPath))
        {
            Debug.LogWarning($"⚠️ Missing audio file (song.ogg, song.mp3, song.wav) for {songName}");
            return null;
        }
        
        if (!File.Exists(chartPath))
        {
            Debug.LogWarning($"⚠️ Missing notes.chart for {songName}");
            return null;
        }
        
        // Create SongData
        return new SongData(songName, artist, audioPath, chartPath, album, year);
    }
    
    void CreateSongButton(SongData songData)
    {
        if (songButtonPrefab == null || contentParent == null)
        {
            Debug.LogError("SongLoader: Missing required components (songButtonPrefab or contentParent)");
            return;
        }
        
        GameObject buttonObj = Instantiate(songButtonPrefab, contentParent);
        buttonObj.transform.localScale = Vector3.one;

        // Set up text components
        TextMeshProUGUI[] textComponents = buttonObj.GetComponentsInChildren<TextMeshProUGUI>();
        
        if (textComponents.Length >= 1)
        {
            textComponents[0].text = songData.songName;
        }
        
        if (textComponents.Length >= 2)
        {
            textComponents[1].text = $"by {songData.artist}";
        }
        
        if (textComponents.Length >= 3 && !string.IsNullOrEmpty(songData.album))
        {
            textComponents[2].text = songData.album;
        }
        
        // Set up button functionality
        Button btn = buttonObj.GetComponent<Button>();
        if (btn != null)
        {
            // Store song data reference
            SongButtonData buttonData = buttonObj.GetComponent<SongButtonData>();
            if (buttonData == null)
                buttonData = buttonObj.AddComponent<SongButtonData>();
            buttonData.songData = songData;
            
            btn.onClick.AddListener(() => {
                SelectSong(songData, buttonObj);
            });
        }
        else
        {
            Debug.LogWarning("⚠️ El prefab no tiene componente Button.");
        }
    }
    
    void SelectSong(SongData songData, GameObject buttonObj)
    {
        // Update visual selection
        UpdateButtonSelection(buttonObj);
        
        // Update game manager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SelectSong(Path.GetFileName(Path.GetDirectoryName(songData.oggPath)), songData);
        }
        
        // Trigger event
        OnSongSelected?.Invoke(songData);
        
        Debug.Log($"🎵 Canción seleccionada: {songData.songName} by {songData.artist}");
    }
    
    void UpdateButtonSelection(GameObject selectedButton)
    {
        // Reset previous selection
        if (currentSelectedButton != null)
        {
            Image prevImage = currentSelectedButton.GetComponent<Image>();
            if (prevImage != null)
                prevImage.color = defaultColor;
        }
        
        // Set new selection
        currentSelectedButton = selectedButton;
        if (selectedButton != null)
        {
            Image newImage = selectedButton.GetComponent<Image>();
            if (newImage != null)
                newImage.color = selectedColor;
        }
    }
    
    void CreateNoSongsMessage()
    {
        if (contentParent == null) return;
        
        GameObject messageObj = new GameObject("NoSongsMessage");
        messageObj.transform.SetParent(contentParent, false);
        
        TextMeshProUGUI text = messageObj.AddComponent<TextMeshProUGUI>();
        text.text = "No songs found!\nPlace song folders in StreamingAssets/Songs/\n\nEach folder should contain:\n• song.ogg (audio file)\n• notes.chart (chart file)\n• song.ini (metadata, optional)";
        text.fontSize = 18;
        text.color = Color.gray;
        text.alignment = TextAlignmentOptions.Center;
        
        RectTransform rect = text.rectTransform;
        rect.sizeDelta = new Vector2(400, 200);
        
        if (loadingIndicator != null)
            loadingIndicator.SetActive(false);
    }
    
    public void RefreshSongList()
    {
        LoadSongs();
    }
    
    public SongData GetSelectedSong()
    {
        if (currentSelectedButton != null)
        {
            SongButtonData buttonData = currentSelectedButton.GetComponent<SongButtonData>();
            return buttonData?.songData;
        }
        return null;
    }
    
    public List<SongData> GetAllSongs()
    {
        return new List<SongData>(availableSongs);
    }
    
    public int GetSongCount()
    {
        return availableSongs.Count;
    }
    
    public void SelectSongByIndex(int index)
    {
        if (index >= 0 && index < availableSongs.Count)
        {
            SongData songData = availableSongs[index];
            
            // Find the corresponding button
            foreach (Transform child in contentParent)
            {
                SongButtonData buttonData = child.GetComponent<SongButtonData>();
                if (buttonData != null && buttonData.songData == songData)
                {
                    SelectSong(songData, child.gameObject);
                    break;
                }
            }
        }
    }
    
    public void SelectSongByName(string songName)
    {
        SongData foundSong = availableSongs.Find(s => s.songName.Equals(songName, System.StringComparison.OrdinalIgnoreCase));
        if (foundSong != null)
        {
            int index = availableSongs.IndexOf(foundSong);
            SelectSongByIndex(index);
        }
    }
    
    /// <summary>
    /// Busca archivos de audio en múltiples formatos
    /// </summary>
    string FindAudioFile(string folderPath)
    {
        string[] audioFormats = { "song.ogg", "song.mp3", "song.wav", "song.m4a" };
        
        foreach (string format in audioFormats)
        {
            string audioPath = Path.Combine(folderPath, format);
            if (File.Exists(audioPath))
            {
                return audioPath;
            }
        }
        
        return null;
    }
}

// Helper component to store song data reference in buttons
public class SongButtonData : MonoBehaviour
{
    public SongData songData;
}
