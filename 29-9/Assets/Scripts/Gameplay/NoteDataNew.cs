using System;
using UnityEngine;

[System.Serializable]
public class NoteData
{
    [Header("Timing Information")]
    public float time;              // Time in seconds when the note should be hit
    public int tick;                // Original tick position from chart file
    
    [Header("Note Properties")]
    public int laneIndex;           // Lane/fret number (0-4 for 5-fret guitar)
    public NoteType noteType;       // Type of note (normal, HOPO, tap, etc.)
    public float duration;          // Duration for sustained notes (0 for regular notes)
    
    [Header("State Management")]
    public bool spawned;            // Has this note been spawned in the game world?
    public bool hit;                // Has this note been successfully hit?
    public bool missed;             // Has this note been missed?
    public HitAccuracy accuracy;    // Hit accuracy (perfect, great, good, miss)
    
    [Header("Visual Properties")]
    public Color noteColor = Color.white;  // Color of the note
    public bool isChord;            // Is this part of a chord?
    public int chordId;             // ID to group chord notes together
    
    // Constructor for basic notes (maintains compatibility with existing code)
    public NoteData(float time, int laneIndex)
    {
        this.time = time;
        this.laneIndex = laneIndex;
        this.noteType = NoteType.Normal;
        this.duration = 0f;
        this.spawned = false;
        this.hit = false;
        this.missed = false;
        this.accuracy = HitAccuracy.None;
        this.isChord = false;
        this.chordId = -1;
        SetNoteColor();
    }
    
    // Constructor for sustained notes
    public NoteData(float time, int laneIndex, float duration) : this(time, laneIndex)
    {
        this.duration = duration;
    }
    
    // Constructor with full parameters
    public NoteData(float time, int tick, int laneIndex, NoteType noteType, float duration = 0f)
    {
        this.time = time;
        this.tick = tick;
        this.laneIndex = laneIndex;
        this.noteType = noteType;
        this.duration = duration;
        this.spawned = false;
        this.hit = false;
        this.missed = false;
        this.accuracy = HitAccuracy.None;
        this.isChord = false;
        this.chordId = -1;
        SetNoteColor();
    }
    
    // Default constructor for serialization
    public NoteData()
    {
        this.time = 0f;
        this.laneIndex = 0;
        this.noteType = NoteType.Normal;
        this.duration = 0f;
        this.spawned = false;
        this.hit = false;
        this.missed = false;
        this.accuracy = HitAccuracy.None;
        this.isChord = false;
        this.chordId = -1;
        SetNoteColor();
    }
    
    // Set note color based on lane (Clone Hero standard colors)
    private void SetNoteColor()
    {
        switch (laneIndex)
        {
            case 0: noteColor = Color.green; break;      // Green fret
            case 1: noteColor = Color.red; break;        // Red fret
            case 2: noteColor = Color.yellow; break;     // Yellow fret
            case 3: noteColor = Color.blue; break;       // Blue fret
            case 4: noteColor = Color.magenta; break;    // Orange fret
            default: noteColor = Color.white; break;
        }
    }
    
    // Check if this is a sustained note
    public bool IsSustained()
    {
        return duration > 0f;
    }
    
    // Get the end time for sustained notes
    public float GetEndTime()
    {
        return time + duration;
    }
    
    // Check if the note can be hit at the given time with tolerance
    public bool CanBeHit(float currentTime, float hitWindow = 0.1f)
    {
        return !hit && !missed && Mathf.Abs(currentTime - time) <= hitWindow;
    }
    
    // Check if the note should be considered missed
    public bool ShouldBeMissed(float currentTime, float missWindow = 0.15f)
    {
        return !hit && !missed && currentTime > time + missWindow;
    }
    
    // Mark the note as hit with accuracy
    public void MarkAsHit(HitAccuracy hitAccuracy)
    {
        hit = true;
        accuracy = hitAccuracy;
    }
    
    // Mark the note as missed
    public void MarkAsMissed()
    {
        missed = true;
        accuracy = HitAccuracy.Miss;
    }
    
    // Reset note state (useful for practice mode)
    public void Reset()
    {
        spawned = false;
        hit = false;
        missed = false;
        accuracy = HitAccuracy.None;
    }
    
    // Get score value based on accuracy
    public int GetScoreValue()
    {
        switch (accuracy)
        {
            case HitAccuracy.Perfect: return 100;
            case HitAccuracy.Great: return 75;
            case HitAccuracy.Good: return 50;
            case HitAccuracy.Miss: return 0;
            default: return 0;
        }
    }
    
    // String representation for debugging
    public override string ToString()
    {
        return $"Note[Time:{time:F2}, Lane:{laneIndex}, Type:{noteType}, Duration:{duration:F2}, Hit:{hit}, Missed:{missed}]";
    }
}

// Enum for different note types in Clone Hero
public enum NoteType
{
    Normal,     // Regular note
    HOPO,       // Hammer-on/Pull-off
    Tap,        // Tap note
    Open,       // Open note (no fret)
    StarPower   // Star power note
}

// Enum for hit accuracy
public enum HitAccuracy
{
    None,       // Not hit yet
    Perfect,    // Perfect timing
    Great,      // Great timing
    Good,       // Good timing
    Ok,         // Ok timing
    Miss        // Missed
}
