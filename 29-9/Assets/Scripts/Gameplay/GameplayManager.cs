using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance;

    [Header("Audio")]
    public AudioSource musicSource;

    [Header("Spawner")]
    public NoteSpawner spawner;
    
    [Header("Managers")]
    public ScoreManager scoreManager;
    public InputManager inputManager;
    public BackgroundVideoSystem backgroundVideoSystem;
    
    [Header("Game State")]
    public bool isGameActive = false;
    public bool isPaused = false;
    public float songLength = 0f;
    
    [Header("Hit Detection")]
    public float hitWindow = 0.3f;
    public float perfectWindow = 0.08f;
    public float greatWindow = 0.11f;
    
    public ChartParser.ChartData chartData;
    public List<NoteData> selectedNotes;
    
    // Events
    public System.Action<NoteData, HitAccuracy> OnNoteHit;
    public System.Action<NoteData> OnNoteMissed;
    public System.Action OnSongFinished;
    
    // Active notes for hit detection
    public List<NoteData> activeNotes = new List<NoteData>();
    private Dictionary<int, INoteObject> spawnedNotes = new Dictionary<int, INoteObject>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        StartCoroutine(InitializeGameplay());
    }
    
    IEnumerator InitializeGameplay()
    {
        LoadChartSection();
        yield return StartCoroutine(LoadAudio());
        
        // Load background video (async, non-blocking)
        LoadBackgroundVideo();
        
        // Wait a moment for everything to initialize
        yield return new WaitForSeconds(0.5f);
        
        StartGameplay();
    }
    
    void StartGameplay()
    {
        // Check if there's a loading screen that should control the start
        // GameplayLoadingScreen loadingScreen = FindFirstObjectByType<GameplayLoadingScreen>();
        // if (loadingScreen != null)
        // {
        //     Debug.Log("🎬 Loading screen detected - gameplay will start after loading");
        //     return; // Let the loading screen control when to start
        // }
        
        isGameActive = true;
        if (musicSource.clip != null)
        {
            songLength = musicSource.clip.length;
        }
        
        // Subscribe to events
        if (spawner != null)
        {
            spawner.OnNoteSpawned += RegisterActiveNote;
        }
        
        // Subscribe to NoteSpawner2D events if available
        NoteSpawner2D spawner2D = FindFirstObjectByType<NoteSpawner2D>();
        if (spawner2D != null)
        {
            // spawner2D.OnNoteSpawned += RegisterActiveNote2D;
        }
    }

    IEnumerator LoadAudio()
    {
        string songFolder = Path.Combine(Application.streamingAssetsPath, "Songs", GameManager.Instance.selectedSongPath);
        string audioPath = Path.Combine(songFolder, "song.ogg");

        if (!File.Exists(audioPath))
        {
            Debug.LogError("❌ Audio no encontrado: " + audioPath);
            yield break;
        }

        UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + audioPath, AudioType.OGGVORBIS);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("❌ Error al cargar audio: " + www.error);
        }
        else
        {
            musicSource.clip = DownloadHandlerAudioClip.GetContent(www);
            musicSource.volume = 0.4f;
            musicSource.Play();
        }
    }


    void LoadChartSection()
    {
        string songFolder = Path.Combine(Application.streamingAssetsPath, "Songs", GameManager.Instance.selectedSongPath);
        string chartPath = Path.Combine(songFolder, "notes.chart");

        if (!File.Exists(chartPath))
        {
            Debug.LogError("❌ Chart no encontrado: " + chartPath);
            return;
        }

        // Parse the chart file using the new ChartParser
        chartData = ChartParser.ParseChartFile(chartPath);
        
        if (chartData == null)
        {
            Debug.LogError("❌ Error al parsear el chart: " + chartPath);
            return;
        }

        // Get notes for the selected difficulty
        selectedNotes = ChartParser.GetNotesForDifficulty(chartData, GameManager.Instance.selectedDifficulty);
        
        if (selectedNotes == null || selectedNotes.Count == 0)
        {
            // No se encontraron notas
            return;
        }
        
        // Chart parseado exitosamente
    }

    void LoadBackgroundVideo()
    {
        if (backgroundVideoSystem != null)
        {
            string songFolder = Path.Combine(Application.streamingAssetsPath, "Songs", GameManager.Instance.selectedSongPath);
            backgroundVideoSystem.LoadSongVideo(songFolder);
            // Iniciando carga de video
        }
    }

    public float GetSongTime()
    {
        if (musicSource != null && musicSource.clip != null && musicSource.isPlaying)
            return musicSource.time;
        else
            return 0f;
    }
    
    void Update()
    {
        if (!isGameActive || isPaused) return;
        
        // Check for missed notes
        CheckMissedNotes();
        
        // Check if song is finished
        if (musicSource != null && !musicSource.isPlaying && GetSongTime() > 0)
        {
            EndSong();
        }
    }
    
    void CheckMissedNotes()
    {
        float currentTime = GetSongTime();
        
        for (int i = activeNotes.Count - 1; i >= 0; i--)
        {
            NoteData note = activeNotes[i];
            
            if (note.ShouldBeMissed(currentTime, hitWindow + 0.05f))
            {
                note.MarkAsMissed();
                OnNoteMissed?.Invoke(note);
                scoreManager?.RegisterMiss();
                activeNotes.RemoveAt(i);
                
                // Nota perdida
            }
        }
    }
    
    public void RegisterActiveNote(NoteData noteData, Note noteObject)
    {
        if (!activeNotes.Contains(noteData))
        {
            activeNotes.Add(noteData);
            // Usar adaptador para convertir Note a INoteObject
            NoteAdapter adapter = new NoteAdapter(noteObject);
            adapter.OnDestroyed += OnNoteObjectDestroyed;
            spawnedNotes[noteData.GetHashCode()] = adapter;
        }
    }
    
    public void RegisterActiveNote2D(NoteData noteData, FallingNote2D noteObject)
    {
        if (!activeNotes.Contains(noteData))
        {
            activeNotes.Add(noteData);
            // Usar adaptador para convertir FallingNote2D a INoteObject
            FallingNote2DAdapter adapter = new FallingNote2DAdapter(noteObject);
            adapter.OnDestroyed += OnNoteObjectDestroyed;
            spawnedNotes[noteData.GetHashCode()] = adapter;
        }
    }
    
    void OnNoteObjectDestroyed(INoteObject destroyedNote)
    {
        if (destroyedNote == null) return;
        
        // Find and remove the corresponding note data safely
        NoteData noteToRemove = null;
        int noteHashCode = 0;
        
        for (int i = activeNotes.Count - 1; i >= 0; i--)
        {
            if (i < activeNotes.Count && spawnedNotes.TryGetValue(activeNotes[i].GetHashCode(), out INoteObject noteObject) && noteObject == destroyedNote)
            {
                noteToRemove = activeNotes[i];
                noteHashCode = noteToRemove.GetHashCode();
                break;
            }
        }
        
        // Remove safely
        if (noteToRemove != null)
        {
            activeNotes.Remove(noteToRemove);
            spawnedNotes.Remove(noteHashCode);
        }
    }
    
    public bool TryHitNote(int laneIndex, out HitAccuracy accuracy)
    {
        accuracy = HitAccuracy.Miss;
        float currentTime = GetSongTime();
        
        // Find the closest hittable note in the specified lane
        NoteData closestNote = null;
        float closestDistance = float.MaxValue;
        
        foreach (NoteData note in activeNotes)
        {
            if (note.laneIndex == laneIndex && note.CanBeHit(currentTime, hitWindow))
            {
                float distance = Mathf.Abs(currentTime - note.time);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestNote = note;
                }
            }
        }
        
        if (closestNote != null)
        {
            // Determine accuracy based on timing
            if (closestDistance <= perfectWindow)
                accuracy = HitAccuracy.Perfect;
            else if (closestDistance <= greatWindow)
                accuracy = HitAccuracy.Great;
            else
                accuracy = HitAccuracy.Good;
            
            closestNote.MarkAsHit(accuracy);
            OnNoteHit?.Invoke(closestNote, accuracy);
            scoreManager?.RegisterHit(accuracy);
            
            // Remove from active notes immediately
            activeNotes.Remove(closestNote);
            
            // Destroy the visual note immediately when hit in hitzone
            if (spawnedNotes.TryGetValue(closestNote.GetHashCode(), out INoteObject noteObject))
            {
                if (noteObject != null)
                {
                    noteObject.Hit(); // This will trigger immediate destruction
                }
                spawnedNotes.Remove(closestNote.GetHashCode());
            }
            
            // Nota acertada
            return true;
        }
        
        // No note to hit - register as a miss
        scoreManager?.RegisterMiss();
        // Fallo sin nota disponible
        return false;
    }
    
    public bool TryHitChord(System.Collections.Generic.List<int> chordLanes, out HitAccuracy accuracy)
    {
        accuracy = HitAccuracy.Miss;
        float currentTime = GetSongTime();
        
        // Find chord notes (notes with same time and chord ID, or notes very close in time)
        var chordNotes = new System.Collections.Generic.List<NoteData>();
        var potentialChordNotes = new System.Collections.Generic.List<NoteData>();
        
        // First, collect all hittable notes in the specified lanes
        foreach (int laneIndex in chordLanes)
        {
            foreach (NoteData note in activeNotes)
            {
                if (note.laneIndex == laneIndex && note.CanBeHit(currentTime, hitWindow))
                {
                    potentialChordNotes.Add(note);
                }
            }
        }
        
        if (potentialChordNotes.Count == 0)
        {
            return false; // No hittable notes found
        }
        
        // Group notes by time (within a small tolerance) to find chords
        const float chordTimeTolerance = 0.1f; // Notes within 0.1 seconds are considered part of the same chord
        var chordGroups = new System.Collections.Generic.Dictionary<float, System.Collections.Generic.List<NoteData>>();
        
        foreach (var note in potentialChordNotes)
        {
            bool foundGroup = false;
            foreach (var kvp in chordGroups)
            {
                if (Mathf.Abs(kvp.Key - note.time) <= chordTimeTolerance)
                {
                    kvp.Value.Add(note);
                    foundGroup = true;
                    break;
                }
            }
            
            if (!foundGroup)
            {
                chordGroups[note.time] = new System.Collections.Generic.List<NoteData> { note };
            }
        }
        
        // Find the best chord group (closest in time and matching the most lanes)
        System.Collections.Generic.List<NoteData> bestChordGroup = null;
        float bestDistance = float.MaxValue;
        int bestMatchCount = 0;
        
        foreach (var group in chordGroups.Values)
        {
            if (group.Count >= 2) // Must have at least 2 notes to be a chord
            {
                // Check how many of the pressed lanes match this chord
                int matchCount = 0;
                foreach (var note in group)
                {
                    if (chordLanes.Contains(note.laneIndex))
                    {
                        matchCount++;
                    }
                }
                
                // Calculate average distance for this group
                float avgDistance = 0f;
                foreach (var note in group)
                {
                    avgDistance += Mathf.Abs(currentTime - note.time);
                }
                avgDistance /= group.Count;
                
                // Prefer groups with more matches, then closer timing
                if (matchCount > bestMatchCount || (matchCount == bestMatchCount && avgDistance < bestDistance))
                {
                    bestChordGroup = group;
                    bestDistance = avgDistance;
                    bestMatchCount = matchCount;
                }
            }
        }
        
        // If we found a good chord group and the player pressed the right number of keys
        if (bestChordGroup != null && bestMatchCount >= 2 && chordLanes.Count >= bestMatchCount)
        {
            // Determine accuracy based on timing
            if (bestDistance <= perfectWindow)
                accuracy = HitAccuracy.Perfect;
            else if (bestDistance <= greatWindow)
                accuracy = HitAccuracy.Great;
            else
                accuracy = HitAccuracy.Good;
            
            // Hit all notes in the chord
            foreach (var note in bestChordGroup)
            {
                if (chordLanes.Contains(note.laneIndex))
                {
                    note.MarkAsHit(accuracy);
                    OnNoteHit?.Invoke(note, accuracy);
                    scoreManager?.RegisterHit(accuracy);
                    
                    // Remove from active notes
                    activeNotes.Remove(note);
                    
                    // Destroy the visual note
                    if (spawnedNotes.TryGetValue(note.GetHashCode(), out INoteObject noteObject))
                    {
                        if (noteObject != null)
                        {
                            noteObject.Hit();
                        }
                        spawnedNotes.Remove(note.GetHashCode());
                    }
                }
            }
            
            // Acorde acertado
            return true;
        }
        
        return false; // No valid chord found
    }
    
    public void PauseGame()
    {
        isPaused = true;
        if (musicSource.isPlaying)
            musicSource.Pause();
        
        // Pausar video de fondo
        if (backgroundVideoSystem != null)
            backgroundVideoSystem.PauseVideo();
            
        Time.timeScale = 0f;
    }
    
    public void ResumeGame()
    {
        isPaused = false;
        if (musicSource.clip != null)
            musicSource.UnPause();
            
        // Reanudar video de fondo
        if (backgroundVideoSystem != null)
            backgroundVideoSystem.PlayVideo();
            
        Time.timeScale = 1f;
    }
    
    public void EndSong()
    {
        isGameActive = false;
        OnSongFinished?.Invoke();
        
        // Detener video de fondo
        if (backgroundVideoSystem != null)
            backgroundVideoSystem.StopVideo();
        
        // Canción terminada
        
        // Create game results and go to PostGameplay scene
        CreateGameResults();
    }
    
    // Public method to start gameplay with test notes
    public void StartTestGameplay()
    {
        isGameActive = true;
        
        // Subscribe to events
        if (spawner != null)
        {
            spawner.OnNoteSpawned += RegisterActiveNote;
        }
        
        // Subscribe to NoteSpawner2D events if available
        NoteSpawner2D spawner2D = FindFirstObjectByType<NoteSpawner2D>();
        if (spawner2D != null)
        {
            // spawner2D.OnNoteSpawned += RegisterActiveNote2D;
        }
        
        // Test gameplay iniciado
    }
    
    /// <summary>
    /// Force start gameplay (called by loading screen)
    /// </summary>
    public void ForceStartGameplay()
    {
        isGameActive = true;
        if (musicSource.clip != null)
        {
            songLength = musicSource.clip.length;
        }
        
        // Subscribe to events
        if (spawner != null)
        {
            spawner.OnNoteSpawned += RegisterActiveNote;
        }
        
        // Subscribe to NoteSpawner2D events if available
        NoteSpawner2D spawner2D = FindFirstObjectByType<NoteSpawner2D>();
        if (spawner2D != null)
        {
            // spawner2D.OnNoteSpawned += RegisterActiveNote2D;
        }
        
        // Gameplay iniciado por loading screen
    }
    
    void CreateGameResults()
    {
        if (scoreManager == null)
        {
            Debug.LogError("ScoreManager not found - cannot create game results");
            StartCoroutine(ReturnToMainMenu());
            return;
        }
        
        // Get song information
        string songName = chartData?.songName ?? "Unknown Song";
        string artist = chartData?.artist ?? "Unknown Artist";
        string difficulty = GameManager.Instance?.selectedDifficulty ?? "Unknown";
        
        // Get performance data
        int finalScore = scoreManager.score;
        float accuracy = scoreManager.GetAccuracyPercentage();
        float completion = CalculateCompletionPercentage();
        
        // Get hit statistics
        int perfectHits = scoreManager.GetHitCount(HitAccuracy.Perfect);
        int greatHits = scoreManager.GetHitCount(HitAccuracy.Great);
        int goodHits = scoreManager.GetHitCount(HitAccuracy.Good);
        int missedHits = scoreManager.GetMissCount();
        int totalNotes = selectedNotes?.Count ?? 0;
        
        // Create results data
        GameResultsData.CreateResults(
            songName,
            artist,
            difficulty,
            finalScore,
            accuracy,
            completion,
            perfectHits,
            greatHits,
            goodHits,
            missedHits,
            totalNotes
        );
        
        Debug.Log($"🎯 Game results created: {songName} - Score: {finalScore}, Accuracy: {accuracy:F1}%");
        
        // Load PostGameplay scene
        SceneManager.LoadScene("PostGameplay");
    }
    
    float CalculateCompletionPercentage()
    {
        if (selectedNotes == null || selectedNotes.Count == 0)
            return 0f;
            
        if (scoreManager == null)
            return 0f;
        
        int totalHits = scoreManager.GetHitCount(HitAccuracy.Perfect) + 
                       scoreManager.GetHitCount(HitAccuracy.Great) + 
                       scoreManager.GetHitCount(HitAccuracy.Good);
        
        return ((float)totalHits / selectedNotes.Count) * 100f;
    }
    
    IEnumerator ReturnToMainMenu()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("MainMenu");
    }
    
    void OnDestroy()
    {
        if (spawner != null)
        {
            spawner.OnNoteSpawned -= RegisterActiveNote;
        }
        
        // Clear spawned notes dictionary
        spawnedNotes.Clear();
    }
}
