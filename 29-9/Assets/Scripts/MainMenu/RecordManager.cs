using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class RecordManager : MonoBehaviour
{
    [Header("UI Components")]
    public RecordDisplay recordDisplay;
    public TextMeshProUGUI songNameText;
    public Button clearRecordsButton;
    
    [Header("Current Song")]
    public string currentSongPath = "";
    public SongData currentSongData;
    
    private Dictionary<string, List<RecordData>> songRecords = new Dictionary<string, List<RecordData>>();
    
    void Start()
    {
        InitializeRecordManager();
        SubscribeToEvents();
    }
    
    void InitializeRecordManager()
    {
        // Clear display initially
        ClearRecordsDisplay();
        
        // Setup clear records button
        if (clearRecordsButton != null)
            clearRecordsButton.onClick.AddListener(ClearCurrentSongRecords);
    }
    
    void SubscribeToEvents()
    {
        // Subscribe to GameManager events
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnSongSelected += HandleSongSelected;
        }
        
        // Subscribe to SongLoader events
        SongLoader songLoader = FindFirstObjectByType<SongLoader>();
        if (songLoader != null)
        {
            songLoader.OnSongSelected += HandleSongSelectedFromLoader;
        }
        
        // Subscribe to SongListUI events
        SongListUI songListUI = FindFirstObjectByType<SongListUI>();
        if (songListUI != null)
        {
            songListUI.OnSongSelected += HandleSongSelectedFromUI;
        }
    }
    
    void HandleSongSelected(string songPath)
    {
        LoadRecordsForSong(songPath, null);
    }
    
    void HandleSongSelectedFromLoader(SongData songData)
    {
        string songPath = Path.GetFileName(songData.GetFolderPath());
        LoadRecordsForSong(songPath, songData);
    }
    
    void HandleSongSelectedFromUI(string fullPath, SongData songData)
    {
        string songPath = Path.GetFileName(fullPath);
        LoadRecordsForSong(songPath, songData);
    }
    
    public void LoadRecordsForSong(string songPath, SongData songData = null)
    {
        if (string.IsNullOrEmpty(songPath))
        {
            ClearRecordsDisplay();
            return;
        }
        
        currentSongPath = songPath;
        currentSongData = songData;
        
        // Update song name display
        if (songNameText != null)
        {
            string displayName = songData?.GetDisplayName() ?? songPath;
            songNameText.text = $"Records de: {displayName}";
        }
        
        // Load records for this song
        List<RecordData> records = GetRecordsForSong(songPath);
        
        // Display records
        if (recordDisplay != null)
        {
            recordDisplay.LoadRecords(records);
        }
        
        Debug.Log($"RecordManager: Loaded {records.Count} records for song: {songPath}");
    }
    
    List<RecordData> GetRecordsForSong(string songPath)
    {
        if (songRecords.ContainsKey(songPath))
        {
            return songRecords[songPath];
        }
        
        // Try to load from file
        List<RecordData> loadedRecords = LoadRecordsFromFile(songPath);
        songRecords[songPath] = loadedRecords;
        
        return loadedRecords;
    }
    
    List<RecordData> LoadRecordsFromFile(string songPath)
    {
        List<RecordData> records = new List<RecordData>();
        
        string recordsPath = GetRecordsFilePath(songPath);
        
        if (!File.Exists(recordsPath))
        {
            return records;
        }
        
        try
        {
            string jsonData = File.ReadAllText(recordsPath);
            RecordDataList recordList = JsonUtility.FromJson<RecordDataList>(jsonData);
            
            if (recordList != null && recordList.records != null)
            {
                records = recordList.records.ToList();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"RecordManager: Error loading records from file: {e.Message}");
        }
        
        return records;
    }
    
    public void SaveNewRecord(RecordData newRecord)
    {
        if (string.IsNullOrEmpty(currentSongPath))
        {
            Debug.LogWarning("RecordManager: No current song selected");
            return;
        }
        
        // Set song metadata
        newRecord.songName = currentSongData?.songName ?? currentSongPath;
        newRecord.songPath = currentSongPath;
        
        // Get current records for song
        List<RecordData> records = GetRecordsForSong(currentSongPath);
        
        // Add new record with duplicate checking
        if (recordDisplay != null)
        {
            recordDisplay.AddNewRecord(newRecord);
            records = recordDisplay.GetCurrentRecords();
        }
        else
        {
            records.Add(newRecord);
            records = records.OrderByDescending(r => r.points)
                           .ThenByDescending(r => r.accuracy)
                           .ThenByDescending(r => r.completionPercentage)
                           .Take(5)
                           .ToList();
        }
        
        // Update cached records
        songRecords[currentSongPath] = records;
        
        // Save to file
        SaveRecordsToFile(currentSongPath, records);
        
        Debug.Log($"RecordManager: Saved new record for {currentSongPath}: {newRecord.GetFormattedRecord()}");
    }
    
    void SaveRecordsToFile(string songPath, List<RecordData> records)
    {
        try
        {
            string recordsPath = GetRecordsFilePath(songPath);
            
            // Ensure directory exists
            string directory = Path.GetDirectoryName(recordsPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            RecordDataList recordList = new RecordDataList { records = records.ToArray() };
            string jsonData = JsonUtility.ToJson(recordList, true);
            
            File.WriteAllText(recordsPath, jsonData);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"RecordManager: Error saving records to file: {e.Message}");
        }
    }
    
    string GetRecordsFilePath(string songPath)
    {
        string recordsDir = Path.Combine(Application.persistentDataPath, "Records");
        return Path.Combine(recordsDir, $"{songPath}_records.json");
    }
    
    public void ClearCurrentSongRecords()
    {
        if (string.IsNullOrEmpty(currentSongPath))
        {
            Debug.LogWarning("RecordManager: No current song selected");
            return;
        }
        
        // Clear from memory
        if (songRecords.ContainsKey(currentSongPath))
        {
            songRecords[currentSongPath].Clear();
        }
        
        // Clear from display
        if (recordDisplay != null)
        {
            recordDisplay.ClearAllRecords();
        }
        
        // Delete file
        try
        {
            string recordsPath = GetRecordsFilePath(currentSongPath);
            if (File.Exists(recordsPath))
            {
                File.Delete(recordsPath);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"RecordManager: Error deleting records file: {e.Message}");
        }
        
        Debug.Log($"RecordManager: Cleared all records for {currentSongPath}");
    }
    
    void ClearRecordsDisplay()
    {
        if (songNameText != null)
            songNameText.text = "Records de: ---";
            
        if (recordDisplay != null)
            recordDisplay.ClearAllRecords();
    }
    
    public bool HasRecordsForCurrentSong()
    {
        if (string.IsNullOrEmpty(currentSongPath))
            return false;
            
        List<RecordData> records = GetRecordsForSong(currentSongPath);
        return records.Count > 0;
    }
    
    public List<RecordData> GetCurrentSongRecords()
    {
        if (string.IsNullOrEmpty(currentSongPath))
            return new List<RecordData>();
            
        return GetRecordsForSong(currentSongPath);
    }
    
    void OnDestroy()
    {
        // Unsubscribe from events
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnSongSelected -= HandleSongSelected;
        }
    }
}

[System.Serializable]
public class RecordDataList
{
    public RecordData[] records;
}
