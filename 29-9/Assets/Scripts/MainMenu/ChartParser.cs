using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System;

/// <summary>
/// Handles parsing of .chart files for Clone Hero-style rhythm games
/// Supports multiple difficulty levels and proper timing calculations
/// </summary>
public class ChartParser
{
    [System.Serializable]
    public class ChartData
    {
        public string songName;
        public string artist;
        public string album;
        public string charter;
        public float offset;
        public float resolution;
        public string player2;
        public string difficulty;
        public string previewStart;
        public string previewEnd;
        public string genre;
        public string mediaType;
        public string musicStream;
        
        public List<BPMChange> bpmChanges = new List<BPMChange>();
        public List<TimeSignature> timeSignatures = new List<TimeSignature>();
        public Dictionary<string, List<NoteData>> difficultyTracks = new Dictionary<string, List<NoteData>>();
        
        public ChartData()
        {
            bpmChanges = new List<BPMChange>();
            timeSignatures = new List<TimeSignature>();
            difficultyTracks = new Dictionary<string, List<NoteData>>();
        }
    }
    
    [System.Serializable]
    public class BPMChange
    {
        public int tick;
        public float bpm;
        public float timeInSeconds;
        
        public BPMChange(int tick, float bpm)
        {
            this.tick = tick;
            this.bpm = bpm;
        }
    }
    
    [System.Serializable]
    public class TimeSignature
    {
        public int tick;
        public int numerator;
        public int denominator;
        
        public TimeSignature(int tick, int numerator, int denominator = 4)
        {
            this.tick = tick;
            this.numerator = numerator;
            this.denominator = denominator;
        }
    }
    
    // Supported difficulty sections
    private readonly Dictionary<string, string[]> difficultyMappings = new Dictionary<string, string[]>
    {
        { "Easy", new[] { "[EasySingle]", "[EasyGuitar]" } },
        { "Medium", new[] { "[MediumSingle]", "[MediumGuitar]" } },
        { "Hard", new[] { "[HardSingle]", "[HardGuitar]" } },
        { "Expert", new[] { "[ExpertSingle]", "[ExpertGuitar]" } },
        { "Facil", new[] { "[EasySingle]", "[EasyGuitar]", "[Single]" } },
        { "Dificil", new[] { "[HardSingle]", "[HardGuitar]", "[ExpertSingle]", "[ExpertGuitar]" } }
    };
    
    /// <summary>
    /// Parses a .chart file and returns structured chart data
    /// </summary>
    /// <param name="chartFilePath">Path to the .chart file</param>
    /// <returns>Parsed chart data or null if parsing fails</returns>
    public static ChartData ParseChartFile(string chartFilePath)
    {
        if (!File.Exists(chartFilePath))
        {
            Debug.LogError($"❌ Chart file not found: {chartFilePath}");
            return null;
        }
        
        try
        {
            string chartContent = File.ReadAllText(chartFilePath);
            return ParseChartContent(chartContent);
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ Error reading chart file {chartFilePath}: {e.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// Parses chart content from a string
    /// </summary>
    /// <param name="chartContent">Raw chart file content</param>
    /// <returns>Parsed chart data or null if parsing fails</returns>
    public static ChartData ParseChartContent(string chartContent)
    {
        if (string.IsNullOrEmpty(chartContent))
        {
            Debug.LogError("❌ Chart content is empty");
            return null;
        }
        
        ChartData chartData = new ChartData();
        
        try
        {
            // Parse metadata section
            ParseSongSection(chartContent, chartData);
            
            // Parse sync track (BPM changes, time signatures)
            ParseSyncTrack(chartContent, chartData);
            
            // Parse difficulty tracks
            ParseDifficultyTracks(chartContent, chartData);
            
            // Calculate timing for all events
            CalculateTiming(chartData);
            
            Debug.Log($"✅ Successfully parsed chart: {chartData.songName} by {chartData.artist}");
            Debug.Log($"📊 Found {chartData.bpmChanges.Count} BPM changes, {chartData.difficultyTracks.Count} difficulty tracks");
            
            return chartData;
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ Error parsing chart content: {e.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// Extracts notes for a specific difficulty level
    /// </summary>
    /// <param name="chartData">Parsed chart data</param>
    /// <param name="difficulty">Difficulty name (Easy, Medium, Hard, Expert, Facil, Dificil)</param>
    /// <returns>List of notes for the specified difficulty</returns>
    public static List<NoteData> GetNotesForDifficulty(ChartData chartData, string difficulty)
    {
        if (chartData == null)
        {
            Debug.LogError("❌ Chart data is null");
            return new List<NoteData>();
        }
        
        var parser = new ChartParser();
        if (!parser.difficultyMappings.ContainsKey(difficulty))
        {
            Debug.LogWarning($"⚠️ Unknown difficulty: {difficulty}. Available: {string.Join(", ", parser.difficultyMappings.Keys)}");
            return new List<NoteData>();
        }
        
        string[] possibleSections = parser.difficultyMappings[difficulty];
        
        foreach (string section in possibleSections)
        {
            if (chartData.difficultyTracks.ContainsKey(section))
            {
                Debug.Log($"🎯 Found difficulty section: {section} for {difficulty}");
                return chartData.difficultyTracks[section];
            }
        }
        
        Debug.LogWarning($"⚠️ No notes found for difficulty: {difficulty}");
        return new List<NoteData>();
    }
    
    private static void ParseSongSection(string chartContent, ChartData chartData)
    {
        Match songSection = Regex.Match(chartContent, @"\[Song\](.*?)(?=\[|$)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
        
        if (songSection.Success)
        {
            string sectionContent = songSection.Groups[1].Value;
            
            chartData.songName = ExtractValue(sectionContent, "Name") ?? "Unknown Song";
            chartData.artist = ExtractValue(sectionContent, "Artist") ?? "Unknown Artist";
            chartData.album = ExtractValue(sectionContent, "Album") ?? "";
            chartData.charter = ExtractValue(sectionContent, "Charter") ?? "";
            chartData.genre = ExtractValue(sectionContent, "Genre") ?? "";
            chartData.mediaType = ExtractValue(sectionContent, "MediaType") ?? "";
            chartData.musicStream = ExtractValue(sectionContent, "MusicStream") ?? "";
            
            // Parse numeric values
            string offsetStr = ExtractValue(sectionContent, "Offset");
            if (float.TryParse(offsetStr, out float offset))
                chartData.offset = offset;
                
            string resolutionStr = ExtractValue(sectionContent, "Resolution");
            if (float.TryParse(resolutionStr, out float resolution))
                chartData.resolution = resolution > 0 ? resolution : 192f; // Default to 192 if invalid
            else
                chartData.resolution = 192f; // Default resolution
                
            chartData.previewStart = ExtractValue(sectionContent, "PreviewStart") ?? "0";
            chartData.previewEnd = ExtractValue(sectionContent, "PreviewEnd") ?? "0";
        }
        else
        {
            Debug.LogWarning("⚠️ No [Song] section found, using defaults");
            chartData.resolution = 192f; // Ensure we have a default resolution
        }
    }
    
    private static void ParseSyncTrack(string chartContent, ChartData chartData)
    {
        Match syncSection = Regex.Match(chartContent, @"\[SyncTrack\](.*?)(?=\[|$)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
        
        if (syncSection.Success)
        {
            string sectionContent = syncSection.Groups[1].Value;
            string[] lines = sectionContent.Split('\n');
            
            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();
                if (string.IsNullOrEmpty(trimmedLine)) continue;
                
                // Parse BPM changes: tick = B bpm
                Match bpmMatch = Regex.Match(trimmedLine, @"(\d+)\s*=\s*B\s+(\d+)");
                if (bpmMatch.Success)
                {
                    int tick = int.Parse(bpmMatch.Groups[1].Value);
                    float bpm = float.Parse(bpmMatch.Groups[2].Value) / 1000f; // BPM is stored as micro-BPM
                    chartData.bpmChanges.Add(new BPMChange(tick, bpm));
                    continue;
                }
                
                // Parse time signatures: tick = TS numerator denominator
                Match tsMatch = Regex.Match(trimmedLine, @"(\d+)\s*=\s*TS\s+(\d+)\s*(\d*)");
                if (tsMatch.Success)
                {
                    int tick = int.Parse(tsMatch.Groups[1].Value);
                    int numerator = int.Parse(tsMatch.Groups[2].Value);
                    int denominator = string.IsNullOrEmpty(tsMatch.Groups[3].Value) ? 4 : int.Parse(tsMatch.Groups[3].Value);
                    chartData.timeSignatures.Add(new TimeSignature(tick, numerator, denominator));
                }
            }
        }
        
        // Ensure we have at least one BPM change
        if (chartData.bpmChanges.Count == 0)
        {
            Debug.LogWarning("⚠️ No BPM changes found, using default 120 BPM");
            chartData.bpmChanges.Add(new BPMChange(0, 120f));
        }
        
        // Sort BPM changes by tick
        chartData.bpmChanges.Sort((a, b) => a.tick.CompareTo(b.tick));
        chartData.timeSignatures.Sort((a, b) => a.tick.CompareTo(b.tick));
    }
    
    private static void ParseDifficultyTracks(string chartContent, ChartData chartData)
    {
        // Find all difficulty sections
        MatchCollection sections = Regex.Matches(chartContent, @"(\[(?:Easy|Medium|Hard|Expert)(?:Single|Guitar|Bass|Drums|Keys)?\])(.*?)(?=\[|$)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
        
        foreach (Match section in sections)
        {
            string sectionName = section.Groups[1].Value;
            string sectionContent = section.Groups[2].Value;
            
            List<NoteData> notes = ParseNoteSection(sectionContent, chartData.resolution);
            if (notes.Count > 0)
            {
                chartData.difficultyTracks[sectionName] = notes;
                Debug.Log($"📝 Parsed {notes.Count} notes for {sectionName}");
            }
        }
        
        // Also check for generic [Single] section for compatibility
        Match singleSection = Regex.Match(chartContent, @"(\[Single\])(.*?)(?=\[|$)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
        if (singleSection.Success)
        {
            string sectionContent = singleSection.Groups[2].Value;
            List<NoteData> notes = ParseNoteSection(sectionContent, chartData.resolution);
            if (notes.Count > 0)
            {
                chartData.difficultyTracks["[Single]"] = notes;
                Debug.Log($"📝 Parsed {notes.Count} notes for [Single]");
            }
        }
    }
    
    private static List<NoteData> ParseNoteSection(string sectionContent, float resolution)
    {
        List<NoteData> notes = new List<NoteData>();
        string[] lines = sectionContent.Split('\n');
        
        foreach (string line in lines)
        {
            string trimmedLine = line.Trim();
            if (string.IsNullOrEmpty(trimmedLine)) continue;
            
            // Parse notes: tick = N lane length
            Match noteMatch = Regex.Match(trimmedLine, @"(\d+)\s*=\s*N\s+(\d+)\s+(\d+)");
            if (noteMatch.Success)
            {
                int tick = int.Parse(noteMatch.Groups[1].Value);
                int lane = int.Parse(noteMatch.Groups[2].Value);
                int length = int.Parse(noteMatch.Groups[3].Value);
                
                // Validate lane (0-4 for 5-fret guitar)
                if (lane >= 0 && lane <= 4)
                {
                    NoteData note = new NoteData(0f, lane); // Time will be calculated later
                    note.tick = tick;
                    note.duration = length; // Using duration instead of length
                    notes.Add(note);
                }
            }
        }
        
        // Sort notes by tick
        notes.Sort((a, b) => a.tick.CompareTo(b.tick));
        return notes;
    }
    
    private static void CalculateTiming(ChartData chartData)
    {
        if (chartData.bpmChanges.Count == 0) return;
        
        // Calculate time in seconds for each BPM change
        float currentTime = chartData.offset;
        int currentTick = 0;
        float currentBPM = chartData.bpmChanges[0].bpm;
        
        for (int i = 0; i < chartData.bpmChanges.Count; i++)
        {
            BPMChange bpmChange = chartData.bpmChanges[i];
            
            if (i > 0)
            {
                // Calculate time elapsed since last BPM change
                int tickDifference = bpmChange.tick - currentTick;
                float timeElapsed = (tickDifference / chartData.resolution) * (60f / currentBPM);
                currentTime += timeElapsed;
            }
            
            bpmChange.timeInSeconds = currentTime;
            currentTick = bpmChange.tick;
            currentBPM = bpmChange.bpm;
        }
        
        // Calculate timing for all notes in all difficulty tracks
        foreach (var track in chartData.difficultyTracks.Values)
        {
            foreach (NoteData note in track)
            {
                note.time = TickToSeconds(note.tick, chartData);
            }
        }
    }
    
    private static float TickToSeconds(int tick, ChartData chartData)
    {
        if (chartData.bpmChanges.Count == 0) return 0f;
        
        // Find the appropriate BPM change for this tick
        BPMChange activeBPMChange = chartData.bpmChanges[0];
        
        for (int i = chartData.bpmChanges.Count - 1; i >= 0; i--)
        {
            if (chartData.bpmChanges[i].tick <= tick)
            {
                activeBPMChange = chartData.bpmChanges[i];
                break;
            }
        }
        
        // Calculate time based on the active BPM change
        int tickDifference = tick - activeBPMChange.tick;
        float timeFromBPMChange = (tickDifference / chartData.resolution) * (60f / activeBPMChange.bpm);
        
        return activeBPMChange.timeInSeconds + timeFromBPMChange;
    }
    
    private static string ExtractValue(string content, string key)
    {
        Match match = Regex.Match(content, $@"{key}\s*=\s*(.+)", RegexOptions.IgnoreCase);
        return match.Success ? match.Groups[1].Value.Trim().Trim('"') : null;
    }
}
