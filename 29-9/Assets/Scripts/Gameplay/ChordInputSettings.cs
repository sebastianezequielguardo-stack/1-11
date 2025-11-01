using UnityEngine;

[CreateAssetMenu(fileName = "ChordInputSettings", menuName = "Gameplay/Chord Input Settings")]
public class ChordInputSettings : ScriptableObject
{
    [Header("Chord Detection")]
    [Tooltip("Time window in seconds to detect simultaneous key presses")]
    [Range(0.01f, 0.2f)]
    public float chordDetectionWindow = 0.05f;
    
    [Tooltip("Maximum time difference between notes to be considered part of the same chord")]
    [Range(0.05f, 0.3f)]
    public float chordTimeTolerance = 0.1f;
    
    [Header("Input Sensitivity")]
    [Tooltip("Minimum number of keys that must be pressed to trigger chord detection")]
    [Range(2, 5)]
    public int minimumChordSize = 2;
    
    [Tooltip("Allow partial chord hits (hitting some but not all notes in a chord)")]
    public bool allowPartialChordHits = true;
    
    [Header("Feedback")]
    [Tooltip("Enable debug logging for chord detection")]
    public bool enableChordDebugLogging = true;
    
    [Tooltip("Different hit sound pitch for chords")]
    [Range(0.5f, 2f)]
    public float chordHitSoundPitch = 1.3f;
    
    [Header("Scoring")]
    [Tooltip("Score multiplier for successful chord hits")]
    [Range(1f, 3f)]
    public float chordScoreMultiplier = 1.5f;
    
    [Tooltip("Bonus points for perfect chord timing")]
    [Range(0, 100)]
    public int perfectChordBonus = 25;
}
