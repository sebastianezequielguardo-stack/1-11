using UnityEngine;
using System.Collections;

/// <summary>
/// Manages all audio aspects of the rhythm game including hit sounds, music, and audio calibration
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource metronomeSource;
    
    [Header("Hit Sounds")]
    public AudioClip perfectHitSound;
    public AudioClip greatHitSound;
    public AudioClip goodHitSound;
    public AudioClip missSound;
    public AudioClip clickSound;
    
    [Header("Audio Settings")]
    [Range(0f, 1f)]
    public float musicVolume = 0.7f;
    [Range(0f, 1f)]
    public float sfxVolume = 1f;
    [Range(0f, 1f)]
    public float metronomeVolume = 0.5f;
    
    [Header("Calibration")]
    public float audioOffset = 0f; // Audio calibration offset in seconds
    public bool useMetronome = false;
    
    private GameplayManager gameplayManager;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        InitializeAudio();
        LoadAudioSettings();
    }
    
    void InitializeAudio()
    {
        gameplayManager = GameplayManager.Instance;
        
        // Subscribe to gameplay events
        if (gameplayManager != null)
        {
            gameplayManager.OnNoteHit += OnNoteHit;
            gameplayManager.OnNoteMissed += OnNoteMissed;
        }
        
        // Configure audio sources
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
            musicSource.playOnAwake = false;
        }
        
        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
            sfxSource.playOnAwake = false;
        }
        
        if (metronomeSource != null)
        {
            metronomeSource.volume = metronomeVolume;
            metronomeSource.playOnAwake = false;
        }
    }
    
    void LoadAudioSettings()
    {
        // Load saved audio settings
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        metronomeVolume = PlayerPrefs.GetFloat("MetronomeVolume", 0.5f);
        audioOffset = PlayerPrefs.GetFloat("AudioOffset", 0f);
        useMetronome = PlayerPrefs.GetInt("UseMetronome", 0) == 1;
        
        ApplyAudioSettings();
    }
    
    public void ApplyAudioSettings()
    {
        if (musicSource != null) musicSource.volume = musicVolume;
        if (sfxSource != null) sfxSource.volume = sfxVolume;
        if (metronomeSource != null) metronomeSource.volume = metronomeVolume;
    }
    
    public void SaveAudioSettings()
    {
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.SetFloat("MetronomeVolume", metronomeVolume);
        PlayerPrefs.SetFloat("AudioOffset", audioOffset);
        PlayerPrefs.SetInt("UseMetronome", useMetronome ? 1 : 0);
        PlayerPrefs.Save();
    }
    
    void OnNoteHit(NoteData noteData, HitAccuracy accuracy)
    {
        // No sound feedback for hits - silent gameplay
    }
    
    void OnNoteMissed(NoteData noteData)
    {
        // No sound feedback for misses - silent gameplay
    }
    
    public void PlayHitSound(HitAccuracy accuracy)
    {
        AudioClip clipToPlay = null;
        
        switch (accuracy)
        {
            case HitAccuracy.Perfect:
                clipToPlay = perfectHitSound;
                break;
            case HitAccuracy.Great:
                clipToPlay = greatHitSound;
                break;
            case HitAccuracy.Good:
                clipToPlay = goodHitSound;
                break;
        }
        
        if (clipToPlay != null && sfxSource != null)
        {
            sfxSource.pitch = GetPitchForAccuracy(accuracy);
            sfxSource.PlayOneShot(clipToPlay);
        }
    }
    
    public void PlayMissSound()
    {
        if (missSound != null && sfxSource != null)
        {
            sfxSource.pitch = 1f;
            sfxSource.PlayOneShot(missSound);
        }
    }
    
    public void PlayClickSound()
    {
        if (clickSound != null && sfxSource != null)
        {
            sfxSource.pitch = 1f;
            sfxSource.PlayOneShot(clickSound);
        }
    }
    
    float GetPitchForAccuracy(HitAccuracy accuracy)
    {
        switch (accuracy)
        {
            case HitAccuracy.Perfect: return 1.2f;
            case HitAccuracy.Great: return 1.1f;
            case HitAccuracy.Good: return 1.0f;
            default: return 1.0f;
        }
    }
    
    public void StartMetronome(float bpm)
    {
        if (useMetronome && metronomeSource != null)
        {
            StartCoroutine(MetronomeCoroutine(bpm));
        }
    }
    
    public void StopMetronome()
    {
        StopAllCoroutines();
    }
    
    IEnumerator MetronomeCoroutine(float bpm)
    {
        float interval = 60f / bpm;
        
        while (useMetronome && gameplayManager != null && gameplayManager.isGameActive)
        {
            if (metronomeSource != null && clickSound != null)
            {
                metronomeSource.PlayOneShot(clickSound);
            }
            
            yield return new WaitForSeconds(interval);
        }
    }
    
    public float GetCalibratedTime()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            return musicSource.time + audioOffset;
        }
        return 0f;
    }
    
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null) musicSource.volume = musicVolume;
    }
    
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (sfxSource != null) sfxSource.volume = sfxVolume;
    }
    
    public void SetMetronomeVolume(float volume)
    {
        metronomeVolume = Mathf.Clamp01(volume);
        if (metronomeSource != null) metronomeSource.volume = metronomeVolume;
    }
    
    public void SetAudioOffset(float offset)
    {
        audioOffset = offset;
    }
    
    public void ToggleMetronome()
    {
        useMetronome = !useMetronome;
        
        if (!useMetronome)
        {
            StopMetronome();
        }
    }
    
    void OnDestroy()
    {
        if (gameplayManager != null)
        {
            gameplayManager.OnNoteHit -= OnNoteHit;
            gameplayManager.OnNoteMissed -= OnNoteMissed;
        }
    }
}
