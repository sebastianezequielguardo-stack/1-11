using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class RecordDisplay : MonoBehaviour
{
    [Header("UI Components")]
    public Transform recordContentPanel;
    public GameObject recordEntryPrefab;
    
    [Header("Display Settings")]
    public int maxRecordsToShow = 5;
    
    private List<RecordData> currentRecords = new List<RecordData>();
    
    void Start()
    {
        // Initialize with empty records
        if (currentRecords.Count == 0)
        {
            LoadRecords(new List<RecordData>());
        }
        
        // Debug: Force create empty entries
        Debug.Log($"RecordDisplay Start: recordContentPanel = {(recordContentPanel != null ? "OK" : "NULL")}, recordEntryPrefab = {(recordEntryPrefab != null ? "OK" : "NULL")}");
    }
    
    public void LoadRecords(List<RecordData> newRecords)
    {
        if (recordContentPanel == null)
        {
            Debug.LogError("RecordDisplay: recordContentPanel is not assigned!");
            return;
        }
        
        if (recordEntryPrefab == null)
        {
            Debug.LogError("RecordDisplay: recordEntryPrefab is not assigned!");
            return;
        }
        
        // Clear previous records
        ClearRecordEntries();
        
        // Sort records using custom comparison (points > accuracy > completion)
        var sortedRecords = newRecords.OrderByDescending(r => r.points)
                                     .ThenByDescending(r => r.accuracy)
                                     .ThenByDescending(r => r.completionPercentage)
                                     .Take(maxRecordsToShow)
                                     .ToList();
        
        currentRecords = sortedRecords;
        
        // Create record entries
        for (int i = 0; i < currentRecords.Count; i++)
        {
            CreateRecordEntry(i, currentRecords[i]);
        }
        
        // Fill remaining slots with empty entries if needed
        for (int i = currentRecords.Count; i < maxRecordsToShow; i++)
        {
            CreateEmptyRecordEntry(i);
        }
    }
    
    void ClearRecordEntries()
    {
        foreach (Transform child in recordContentPanel)
        {
            if (Application.isPlaying)
                Destroy(child.gameObject);
            else
                DestroyImmediate(child.gameObject);
        }
    }
    
    void CreateRecordEntry(int position, RecordData record)
    {
        GameObject entry = Instantiate(recordEntryPrefab, recordContentPanel);
        
        // Try to get TextMeshPro components first, then fallback to Text
        TextMeshProUGUI[] tmpTexts = entry.GetComponentsInChildren<TextMeshProUGUI>();
        Text[] legacyTexts = entry.GetComponentsInChildren<Text>();
        
        if (tmpTexts.Length >= 4)
        {
            // Using TextMeshPro - 4 components: Position, Initials, Points, Accuracy, Completion
            tmpTexts[0].text = $"{position + 1}°";
            tmpTexts[1].text = record.playerInitials;
            tmpTexts[2].text = $"{record.points:N0} pts";
            tmpTexts[3].text = $"{record.accuracy:F1}% | {record.completionPercentage:F1}%";
        }
        else if (tmpTexts.Length >= 3)
        {
            // Using TextMeshPro - 3 components: Position, Initials+Points, Accuracy+Completion
            tmpTexts[0].text = $"{position + 1}°";
            tmpTexts[1].text = $"{record.playerInitials} - {record.points:N0} pts";
            tmpTexts[2].text = $"{record.accuracy:F1}% | {record.completionPercentage:F1}%";
        }
        else if (legacyTexts.Length >= 4)
        {
            // Using legacy Text - 4 components
            legacyTexts[0].text = $"{position + 1}°";
            legacyTexts[1].text = record.playerInitials;
            legacyTexts[2].text = $"{record.points:N0} pts";
            legacyTexts[3].text = $"{record.accuracy:F1}% | {record.completionPercentage:F1}%";
        }
        else if (legacyTexts.Length >= 3)
        {
            // Using legacy Text - 3 components
            legacyTexts[0].text = $"{position + 1}°";
            legacyTexts[1].text = $"{record.playerInitials} - {record.points:N0} pts";
            legacyTexts[2].text = $"{record.accuracy:F1}% | {record.completionPercentage:F1}%";
        }
        else
        {
            Debug.LogWarning($"RecordDisplay: Record entry prefab doesn't have enough text components. Found TMP: {tmpTexts.Length}, Text: {legacyTexts.Length}");
        }
    }
    
    void CreateEmptyRecordEntry(int position)
    {
        GameObject entry = Instantiate(recordEntryPrefab, recordContentPanel);
        
        // Try to get TextMeshPro components first, then fallback to Text
        TextMeshProUGUI[] tmpTexts = entry.GetComponentsInChildren<TextMeshProUGUI>();
        Text[] legacyTexts = entry.GetComponentsInChildren<Text>();
        
        if (tmpTexts.Length >= 3)
        {
            // Using TextMeshPro
            tmpTexts[0].text = $"{position + 1}°";
            tmpTexts[1].text = "--- pts";
            tmpTexts[2].text = "---%";
        }
        else if (legacyTexts.Length >= 3)
        {
            // Using legacy Text
            legacyTexts[0].text = $"{position + 1}°";
            legacyTexts[1].text = "--- pts";
            legacyTexts[2].text = "---%";
        }
    }
    
    public void AddNewRecord(RecordData newRecord)
    {
        if (newRecord == null)
        {
            Debug.LogWarning("RecordDisplay: Attempted to add null record");
            return;
        }
        
        // Check for duplicate initials
        var existingRecord = currentRecords.FirstOrDefault(r => r.HasSameInitials(newRecord.playerInitials));
        
        if (existingRecord != null)
        {
            // Found duplicate initials - update if new record is better
            if (newRecord.IsBetterThan(existingRecord))
            {
                currentRecords.Remove(existingRecord);
                currentRecords.Add(newRecord);
                Debug.Log($"RecordDisplay: Updated record for {newRecord.playerInitials} - {newRecord.GetFormattedRecord()}");
            }
            else
            {
                Debug.Log($"RecordDisplay: New record for {newRecord.playerInitials} is not better than existing record");
                return;
            }
        }
        else
        {
            // No duplicate initials - add new record
            currentRecords.Add(newRecord);
            Debug.Log($"RecordDisplay: Added new record - {newRecord.GetFormattedRecord()}");
        }
        
        LoadRecords(currentRecords);
    }
    
    public bool CheckForDuplicateInitials(string initials, out RecordData existingRecord)
    {
        existingRecord = currentRecords.FirstOrDefault(r => r.HasSameInitials(initials));
        return existingRecord != null;
    }
    
    public void AddNewRecordWithInitialsCheck(RecordData newRecord, System.Action<string> onDuplicateInitials = null)
    {
        if (newRecord == null)
        {
            Debug.LogWarning("RecordDisplay: Attempted to add null record");
            return;
        }
        
        var existingRecord = currentRecords.FirstOrDefault(r => r.HasSameInitials(newRecord.playerInitials));
        
        if (existingRecord != null)
        {
            // Trigger callback for duplicate initials handling
            onDuplicateInitials?.Invoke(newRecord.playerInitials);
        }
        else
        {
            // No duplicates - add directly
            AddNewRecord(newRecord);
        }
    }
    
    public void ClearAllRecords()
    {
        currentRecords.Clear();
        LoadRecords(currentRecords);
        Debug.Log("RecordDisplay: Cleared all records");
    }
    
    public List<RecordData> GetCurrentRecords()
    {
        return new List<RecordData>(currentRecords);
    }
    
    public bool IsNewHighScore(int points)
    {
        if (currentRecords.Count == 0) return true;
        return points > currentRecords.Max(r => r.points);
    }
    
    public int GetRecordPosition(int points)
    {
        var sortedRecords = currentRecords.OrderByDescending(r => r.points).ToList();
        for (int i = 0; i < sortedRecords.Count; i++)
        {
            if (points > sortedRecords[i].points)
                return i + 1;
        }
        return sortedRecords.Count + 1;
    }
}

[System.Serializable]
public class RecordData
{
    [Header("Player Information")]
    public string playerInitials = "AAA";
    public string playerName = "Player";
    
    [Header("Score Information")]
    public int points;
    public float accuracy;
    public float completionPercentage;
    public string difficulty = "Medium";
    
    [Header("Performance Details")]
    public int perfectHits;
    public int goodHits;
    public int okHits;
    public int missedHits;
    public int totalNotes;
    public int notesHit;
    
    [Header("Metadata")]
    public string songName;
    public string songPath;
    public System.DateTime dateAchieved;
    
    public RecordData()
    {
        dateAchieved = System.DateTime.Now;
        playerInitials = "AAA";
        completionPercentage = 0f;
    }
    
    public RecordData(string initials, int points, float accuracy, float completion = 0f)
    {
        this.playerInitials = initials.ToUpper().Substring(0, Mathf.Min(3, initials.Length)).PadRight(3, 'A');
        this.points = points;
        this.accuracy = accuracy;
        this.completionPercentage = completion;
        this.dateAchieved = System.DateTime.Now;
    }
    
    public RecordData(string initials, int points, float accuracy, float completion, int perfect, int good, int ok, int missed, int total)
    {
        this.playerInitials = initials.ToUpper().Substring(0, Mathf.Min(3, initials.Length)).PadRight(3, 'A');
        this.points = points;
        this.accuracy = accuracy;
        this.completionPercentage = completion;
        this.perfectHits = perfect;
        this.goodHits = good;
        this.okHits = ok;
        this.missedHits = missed;
        this.totalNotes = total;
        this.notesHit = perfect + good + ok;
        this.dateAchieved = System.DateTime.Now;
    }
    
    public bool HasSameInitials(string otherInitials)
    {
        return playerInitials.Equals(otherInitials.ToUpper().Substring(0, Mathf.Min(3, otherInitials.Length)).PadRight(3, 'A'));
    }
    
    public bool IsBetterThan(RecordData other)
    {
        // Primary sort: Points (higher is better)
        if (points != other.points)
            return points > other.points;
            
        // Secondary sort: Accuracy (higher is better)
        if (Mathf.Abs(accuracy - other.accuracy) > 0.01f)
            return accuracy > other.accuracy;
            
        // Tertiary sort: Completion percentage (higher is better)
        return completionPercentage > other.completionPercentage;
    }
    
    public string GetFormattedRecord()
    {
        return $"{playerInitials} - {points:N0} pts - {accuracy:F1}% - {completionPercentage:F1}%";
    }
    
    public override string ToString()
    {
        return GetFormattedRecord() + $" ({dateAchieved:dd/MM/yyyy})";
    }
}
