using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class SongListUI : MonoBehaviour
{
    [Header("UI Components")]
    public Transform contentPanel;
    public GameObject songButtonPrefab;
    public ScrollRect scrollRect;
    public GameObject loadingIndicator;
    
    [Header("Visual Settings")]
    public Color selectedColor = Color.green;
    public Color defaultColor = Color.white;
    public float buttonHeight = 80f;
    
    [Header("Events")]
    public System.Action<string, SongData> OnSongSelected;
    
    private string selectedSongPath = "";
    private GameObject currentSelectedButton;
    private List<SongData> loadedSongs = new List<SongData>();

    void Start()
    {
        LoadSongList();
    }
    
    void LoadSongList()
    {
        if (loadingIndicator != null)
            loadingIndicator.SetActive(true);
            
        string songsFolderPath = Path.Combine(Application.streamingAssetsPath, "Songs");

        if (!Directory.Exists(songsFolderPath))
        {
            Debug.LogWarning("❌ No se encontró la carpeta Songs en StreamingAssets.");
            CreateNoSongsMessage();
            return;
        }

        // Clear previous buttons
        ClearSongButtons();

        string[] songFolders = Directory.GetDirectories(songsFolderPath);
        Debug.Log($"📁 Cantidad de carpetas encontradas: {songFolders.Length}");

        int validSongs = 0;
        foreach (string folder in songFolders)
        {
            string folderName = Path.GetFileName(folder);
            Debug.Log($"📁 Procesando carpeta: {folderName}");

            // Ignore system folders
            if (folderName.StartsWith(".") || folderName == "__MACOSX")
            {
                Debug.Log($"⏭️ Ignorando carpeta del sistema: {folderName}");
                continue;
            }

            SongData songData = LoadSongFromFolder(folder);
            if (songData != null)
            {
                loadedSongs.Add(songData);
                CreateSongButton(songData, folder);
                validSongs++;
            }
        }
        
        if (validSongs == 0)
        {
            CreateNoSongsMessage();
        }
        
        if (loadingIndicator != null)
            loadingIndicator.SetActive(false);
            
        Debug.Log($"✅ Cargadas {validSongs} canciones válidas");
    }
    
    void ClearSongButtons()
    {
        if (contentPanel == null) return;
        
        loadedSongs.Clear();
        selectedSongPath = "";
        currentSelectedButton = null;
        
        foreach (Transform child in contentPanel)
        {
            if (Application.isPlaying)
                Destroy(child.gameObject);
            else
                DestroyImmediate(child.gameObject);
        }
    }
    
    SongData LoadSongFromFolder(string folderPath)
    {
        string iniPath = Path.Combine(folderPath, "song.ini");
        string chartPath = Path.Combine(folderPath, "notes.chart");
        string oggPath = Path.Combine(folderPath, "song.ogg");
        
        // Check for required files
        if (!File.Exists(chartPath))
        {
            Debug.LogWarning($"⚠️ Falta notes.chart en: {Path.GetFileName(folderPath)}");
            return null;
        }
        
        if (!File.Exists(oggPath))
        {
            Debug.LogWarning($"⚠️ Falta song.ogg en: {Path.GetFileName(folderPath)}");
            return null;
        }
        
        // Load song metadata
        string songName = Path.GetFileName(folderPath);
        string artist = "Unknown Artist";
        string album = "";
        int year = 0;
        
        if (File.Exists(iniPath))
        {
            try
            {
                string[] lines = File.ReadAllLines(iniPath);
                foreach (string line in lines)
                {
                    if (line.StartsWith("name"))
                    {
                        string[] parts = line.Split('=');
                        if (parts.Length > 1)
                            songName = parts[1].Trim().Trim('"');
                    }
                    else if (line.StartsWith("artist"))
                    {
                        string[] parts = line.Split('=');
                        if (parts.Length > 1)
                            artist = parts[1].Trim().Trim('"');
                    }
                    else if (line.StartsWith("album"))
                    {
                        string[] parts = line.Split('=');
                        if (parts.Length > 1)
                            album = parts[1].Trim().Trim('"');
                    }
                    else if (line.StartsWith("year"))
                    {
                        string[] parts = line.Split('=');
                        if (parts.Length > 1 && int.TryParse(parts[1].Trim(), out int parsedYear))
                            year = parsedYear;
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"⚠️ Error leyendo song.ini: {e.Message}");
            }
        }
        
        return new SongData(songName, artist, oggPath, chartPath, album, year);
    }
    
    void CreateSongButton(SongData songData, string folderPath)
    {
        if (songButtonPrefab == null || contentPanel == null)
        {
            Debug.LogError("❌ SongListUI: Faltan componentes requeridos");
            return;
        }
        
        GameObject button = Instantiate(songButtonPrefab, contentPanel);

        // Set button size
        RectTransform rt = button.GetComponent<RectTransform>();
        if (rt != null)
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, buttonHeight);

        // Set text content
        SetButtonText(button, songData);

        // Set button functionality
        Button buttonComponent = button.GetComponent<Button>();
        if (buttonComponent != null)
        {
            buttonComponent.onClick.AddListener(() => {
                SelectSong(folderPath, songData, button);
            });
        }
        else
        {
            Debug.LogWarning("⚠️ El prefab del botón no tiene componente Button");
        }
    }
    
    void SetButtonText(GameObject button, SongData songData)
    {
        // Try TextMeshPro first, then fallback to Text
        TextMeshProUGUI tmpText = button.GetComponentInChildren<TextMeshProUGUI>();
        if (tmpText != null)
        {
            tmpText.text = $"{songData.songName}\n<size=70%>by {songData.artist}</size>";
            return;
        }
        
        Text legacyText = button.GetComponentInChildren<Text>();
        if (legacyText != null)
        {
            legacyText.text = $"{songData.songName}\nby {songData.artist}";
        }
        else
        {
            Debug.LogWarning("⚠️ El prefab del botón no tiene componente de texto");
        }
    }
    
    void SelectSong(string folderPath, SongData songData, GameObject buttonObj)
    {
        // Update visual selection
        UpdateButtonSelection(buttonObj);
        
        // Update selected song
        selectedSongPath = folderPath;
        
        // Update GameManager
        if (GameManager.Instance != null)
        {
            string folderName = Path.GetFileName(folderPath);
            GameManager.Instance.SelectSong(folderName, songData);
        }
        
        // Trigger event
        OnSongSelected?.Invoke(folderPath, songData);
        
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
        if (contentPanel == null) return;
        
        GameObject messageObj = new GameObject("NoSongsMessage");
        messageObj.transform.SetParent(contentPanel, false);
        
        // Add RectTransform
        RectTransform rect = messageObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(400, 200);
        
        // Try to add TextMeshPro, fallback to Text
        TextMeshProUGUI tmpText = messageObj.AddComponent<TextMeshProUGUI>();
        if (tmpText != null)
        {
            tmpText.text = "No songs found!\n\nPlace song folders in:\nStreamingAssets/Songs/\n\nEach folder needs:\n• song.ogg (audio)\n• notes.chart (chart)\n• song.ini (metadata, optional)";
            tmpText.fontSize = 18;
            tmpText.color = Color.gray;
            tmpText.alignment = TextAlignmentOptions.Center;
        }
        
        if (loadingIndicator != null)
            loadingIndicator.SetActive(false);
    }
    
    // Public methods
    public void RefreshSongList()
    {
        LoadSongList();
    }
    
    public string GetSelectedSongPath()
    {
        return selectedSongPath;
    }
    
    public SongData GetSelectedSongData()
    {
        if (string.IsNullOrEmpty(selectedSongPath)) return null;
        
        string folderName = Path.GetFileName(selectedSongPath);
        return loadedSongs.Find(s => Path.GetFileName(s.GetFolderPath()) == folderName);
    }
    
    public List<SongData> GetAllSongs()
    {
        return new List<SongData>(loadedSongs);
    }
    
    public int GetSongCount()
    {
        return loadedSongs.Count;
    }
    
    public void SelectSongByIndex(int index)
    {
        if (index >= 0 && index < loadedSongs.Count)
        {
            SongData songData = loadedSongs[index];
            string folderPath = songData.GetFolderPath();
            
            // Find corresponding button
            Transform buttonTransform = contentPanel.GetChild(index);
            if (buttonTransform != null)
            {
                SelectSong(folderPath, songData, buttonTransform.gameObject);
            }
        }
    }
    
    public void ScrollToSelected()
    {
        if (scrollRect != null && currentSelectedButton != null)
        {
            // Calculate scroll position to center the selected button
            RectTransform content = scrollRect.content;
            RectTransform selectedRect = currentSelectedButton.GetComponent<RectTransform>();
            
            if (content != null && selectedRect != null)
            {
                float normalizedPosition = 1f - (selectedRect.anchoredPosition.y / content.sizeDelta.y);
                scrollRect.verticalNormalizedPosition = Mathf.Clamp01(normalizedPosition);
            }
        }
    }
}
