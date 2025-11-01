using UnityEngine;

[System.Serializable]
public class SongData
{
    [Header("Identification")]
    public string songID; // NUEVO: ID único para el ranking

    [Header("Basic Information")]
    public string songName;
    public string artist;
    public string album;
    public int year;

    [Header("File Paths")]
    public string oggPath;
    public string chartPath;
    public string iniPath;
    public string rb3conPath;

    [Header("Additional Metadata")]
    public string genre;
    public int length; // in seconds
    public string charter;
    public float previewStart;
    public float previewEnd;

    [Header("Difficulty Information")]
    public bool hasEasy;
    public bool hasMedium;
    public bool hasHard;
    public bool hasExpert;

    [Header("Chart Statistics")]
    public int easyNoteCount;
    public int mediumNoteCount;
    public int hardNoteCount;
    public int expertNoteCount;

    // Default constructor
    public SongData()
    {
        songID = System.Guid.NewGuid().ToString(); // NUEVO: generar ID único automáticamente
        songName = "Unknown Song";
        artist = "Unknown Artist";
        album = "";
        year = 0;
        genre = "";
        length = 0;
        charter = "";
        previewStart = 0f;
        previewEnd = 30f;

        oggPath = "";
        chartPath = "";
        iniPath = "";
        rb3conPath = "";

        hasEasy = false;
        hasMedium = false;
        hasHard = false;
        hasExpert = false;

        easyNoteCount = 0;
        mediumNoteCount = 0;
        hardNoteCount = 0;
        expertNoteCount = 0;
    }

    // Basic constructor
    public SongData(string name, string artist, string ogg, string chart)
    {
        this.songID = GenerateSongID(name, artist); // NUEVO: generar ID basado en nombre y artista
        this.songName = name;
        this.artist = artist;
        this.oggPath = ogg;
        this.chartPath = chart;

        // Set defaults for other fields
        album = "";
        year = 0;
        genre = "";
        length = 0;
        charter = "";
        previewStart = 0f;
        previewEnd = 30f;

        iniPath = "";
        rb3conPath = "";

        hasEasy = false;
        hasMedium = false;
        hasHard = false;
        hasExpert = false;

        easyNoteCount = 0;
        mediumNoteCount = 0;
        hardNoteCount = 0;
        expertNoteCount = 0;
    }

    // Extended constructor
    public SongData(string name, string artist, string ogg, string chart, string album, int year)
    {
        this.songID = GenerateSongID(name, artist); // NUEVO
        this.songName = name;
        this.artist = artist;
        this.oggPath = ogg;
        this.chartPath = chart;
        this.album = album;
        this.year = year;

        // Set defaults for other fields
        genre = "";
        length = 0;
        charter = "";
        previewStart = 0f;
        previewEnd = 30f;

        iniPath = "";
        rb3conPath = "";

        hasEasy = false;
        hasMedium = false;
        hasHard = false;
        hasExpert = false;

        easyNoteCount = 0;
        mediumNoteCount = 0;
        hardNoteCount = 0;
        expertNoteCount = 0;
    }

    // Full constructor
    public SongData(string name, string artist, string ogg, string chart, string ini, string rb3con = "")
    {
        songID = GenerateSongID(name, artist); // NUEVO
        songName = name;
        this.artist = artist;
        oggPath = ogg;
        chartPath = chart;
        iniPath = ini;
        rb3conPath = rb3con;

        // Set defaults for other fields
        album = "";
        year = 0;
        genre = "";
        length = 0;
        charter = "";
        previewStart = 0f;
        previewEnd = 30f;

        hasEasy = false;
        hasMedium = false;
        hasHard = false;
        hasExpert = false;

        easyNoteCount = 0;
        mediumNoteCount = 0;
        hardNoteCount = 0;
        expertNoteCount = 0;
    }

    // NUEVO: Método para generar un ID único basado en nombre y artista
    private string GenerateSongID(string name, string artist)
    {
        // Crear un ID legible basado en el nombre y artista
        string cleanName = name.ToLower().Replace(" ", "_").Replace("-", "_");
        string cleanArtist = artist.ToLower().Replace(" ", "_").Replace("-", "_");

        // Remover caracteres especiales
        cleanName = System.Text.RegularExpressions.Regex.Replace(cleanName, @"[^a-z0-9_]", "");
        cleanArtist = System.Text.RegularExpressions.Regex.Replace(cleanArtist, @"[^a-z0-9_]", "");

        return $"{cleanArtist}_{cleanName}";
    }

    // NUEVO: Método público para establecer un ID personalizado
    public void SetSongID(string customID)
    {
        songID = customID;
    }

    // Utility methods
    public bool IsValid()
    {
        return !string.IsNullOrEmpty(songName) &&
               !string.IsNullOrEmpty(artist) &&
               !string.IsNullOrEmpty(oggPath) &&
               !string.IsNullOrEmpty(chartPath) &&
               System.IO.File.Exists(oggPath) &&
               System.IO.File.Exists(chartPath);
    }

    public bool HasAnyDifficulty()
    {
        return hasEasy || hasMedium || hasHard || hasExpert;
    }

    public int GetTotalNoteCount()
    {
        return easyNoteCount + mediumNoteCount + hardNoteCount + expertNoteCount;
    }

    public string GetDifficultyString()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        if (hasEasy) sb.Append("Easy ");
        if (hasMedium) sb.Append("Medium ");
        if (hasHard) sb.Append("Hard ");
        if (hasExpert) sb.Append("Expert ");

        return sb.ToString().Trim();
    }

    public string GetFormattedLength()
    {
        if (length <= 0) return "Unknown";

        int minutes = length / 60;
        int seconds = length % 60;
        return $"{minutes}:{seconds:D2}";
    }

    public string GetDisplayName()
    {
        if (string.IsNullOrEmpty(artist))
            return songName;
        return $"{songName} - {artist}";
    }

    public string GetFolderPath()
    {
        if (string.IsNullOrEmpty(oggPath))
            return "";
        return System.IO.Path.GetDirectoryName(oggPath);
    }

    public void SetDifficultyAvailability(string difficulty, bool available, int noteCount = 0)
    {
        switch (difficulty.ToLower())
        {
            case "easy":
            case "facil":
                hasEasy = available;
                if (available) easyNoteCount = noteCount;
                break;
            case "medium":
                hasMedium = available;
                if (available) mediumNoteCount = noteCount;
                break;
            case "hard":
            case "dificil":
                hasHard = available;
                if (available) hardNoteCount = noteCount;
                break;
            case "expert":
                hasExpert = available;
                if (available) expertNoteCount = noteCount;
                break;
        }
    }

    public int GetNoteCountForDifficulty(string difficulty)
    {
        switch (difficulty.ToLower())
        {
            case "easy":
            case "facil":
                return easyNoteCount;
            case "medium":
                return mediumNoteCount;
            case "hard":
            case "dificil":
                return hardNoteCount;
            case "expert":
                return expertNoteCount;
            default:
                return 0;
        }
    }

    public bool HasDifficulty(string difficulty)
    {
        switch (difficulty.ToLower())
        {
            case "easy":
            case "facil":
                return hasEasy;
            case "medium":
                return hasMedium;
            case "hard":
            case "dificil":
                return hasHard;
            case "expert":
                return hasExpert;
            default:
                return false;
        }
    }

    public override string ToString()
    {
        return $"{GetDisplayName()} ({GetFormattedLength()}) - {GetDifficultyString()}";
    }

    // Clone method for creating copies
    public SongData Clone()
    {
        SongData clone = new SongData();

        clone.songID = this.songID; // NUEVO: copiar el ID también
        clone.songName = this.songName;
        clone.artist = this.artist;
        clone.album = this.album;
        clone.year = this.year;
        clone.genre = this.genre;
        clone.length = this.length;
        clone.charter = this.charter;
        clone.previewStart = this.previewStart;
        clone.previewEnd = this.previewEnd;

        clone.oggPath = this.oggPath;
        clone.chartPath = this.chartPath;
        clone.iniPath = this.iniPath;
        clone.rb3conPath = this.rb3conPath;

        clone.hasEasy = this.hasEasy;
        clone.hasMedium = this.hasMedium;
        clone.hasHard = this.hasHard;
        clone.hasExpert = this.hasExpert;

        clone.easyNoteCount = this.easyNoteCount;
        clone.mediumNoteCount = this.mediumNoteCount;
        clone.hardNoteCount = this.hardNoteCount;
        clone.expertNoteCount = this.expertNoteCount;

        return clone;
    }
}