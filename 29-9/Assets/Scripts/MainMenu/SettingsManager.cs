using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages game settings including audio, video, input, and gameplay preferences
/// </summary>
public class SettingsManager : MonoBehaviour
{
    [Header("Settings UI")]
    public GameObject settingsPanel;
    public Button openSettingsButton;
    public Button closeSettingsButton;
    public Button resetSettingsButton;
    
    [Header("Audio Settings")]
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Slider metronomeVolumeSlider;
    public TextMeshProUGUI musicVolumeText;
    public TextMeshProUGUI sfxVolumeText;
    public TextMeshProUGUI metronomeVolumeText;
    public Toggle metronomeToggle;
    
    [Header("Gameplay Settings")]
    public Slider noteSpeedSlider;
    public TextMeshProUGUI noteSpeedText;
    public Slider hitWindowSlider;
    public TextMeshProUGUI hitWindowText;
    public Toggle showFPSToggle;
    public Toggle pauseOnFocusLossToggle;
    
    [Header("Visual Settings")]
    public Dropdown qualityDropdown;
    public Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    public Toggle vsyncToggle;
    
    [Header("Input Settings")]
    public Button[] keyBindButtons = new Button[5];
    public TextMeshProUGUI[] keyBindTexts = new TextMeshProUGUI[5];
    public KeyCode[] defaultKeys = { KeyCode.D, KeyCode.F, KeyCode.J, KeyCode.K, KeyCode.L };
    
    private KeyCode[] currentKeys = new KeyCode[5];
    private bool isWaitingForKey = false;
    private int keyToRebind = -1;
    
    // Default values
    private const float DEFAULT_MUSIC_VOLUME = 0.7f;
    private const float DEFAULT_SFX_VOLUME = 1.0f;
    private const float DEFAULT_METRONOME_VOLUME = 0.5f;
    private const float DEFAULT_NOTE_SPEED = 5.0f;
    private const float DEFAULT_HIT_WINDOW = 0.1f;
    
    void Start()
    {
        InitializeSettings();
        LoadSettings();
        SetupUIListeners();
    }
    
    void InitializeSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        
        // Initialize key bindings
        for (int i = 0; i < currentKeys.Length; i++)
        {
            currentKeys[i] = defaultKeys[i];
        }
        
        // Setup resolution dropdown
        SetupResolutionDropdown();
    }
    
    void SetupUIListeners()
    {
        // Main buttons
        if (openSettingsButton != null)
            openSettingsButton.onClick.AddListener(ShowSettings);
            
        if (closeSettingsButton != null)
            closeSettingsButton.onClick.AddListener(HideSettings);
            
        if (resetSettingsButton != null)
            resetSettingsButton.onClick.AddListener(ResetToDefaults);
        
        // Audio sliders
        if (musicVolumeSlider != null)
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
            
        if (metronomeVolumeSlider != null)
            metronomeVolumeSlider.onValueChanged.AddListener(OnMetronomeVolumeChanged);
        
        // Gameplay sliders
        if (noteSpeedSlider != null)
            noteSpeedSlider.onValueChanged.AddListener(OnNoteSpeedChanged);
            
        if (hitWindowSlider != null)
            hitWindowSlider.onValueChanged.AddListener(OnHitWindowChanged);
        
        // Toggles
        if (metronomeToggle != null)
            metronomeToggle.onValueChanged.AddListener(OnMetronomeToggleChanged);
            
        if (showFPSToggle != null)
            showFPSToggle.onValueChanged.AddListener(OnShowFPSChanged);
            
        if (pauseOnFocusLossToggle != null)
            pauseOnFocusLossToggle.onValueChanged.AddListener(OnPauseOnFocusLossChanged);
            
        if (fullscreenToggle != null)
            fullscreenToggle.onValueChanged.AddListener(OnFullscreenChanged);
            
        if (vsyncToggle != null)
            vsyncToggle.onValueChanged.AddListener(OnVSyncChanged);
        
        // Dropdowns
        if (qualityDropdown != null)
            qualityDropdown.onValueChanged.AddListener(OnQualityChanged);
            
        if (resolutionDropdown != null)
            resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
        
        // Key binding buttons
        for (int i = 0; i < keyBindButtons.Length; i++)
        {
            int index = i; // Capture for closure
            if (keyBindButtons[i] != null)
            {
                keyBindButtons[i].onClick.AddListener(() => StartKeyRebind(index));
            }
        }
    }
    
    void SetupResolutionDropdown()
    {
        if (resolutionDropdown == null) return;
        
        resolutionDropdown.ClearOptions();
        
        Resolution[] resolutions = Screen.resolutions;
        System.Collections.Generic.List<string> options = new System.Collections.Generic.List<string>();
        
        foreach (Resolution res in resolutions)
        {
            string option = res.width + " x " + res.height;
            options.Add(option);
        }
        
        resolutionDropdown.AddOptions(options);
    }
    
    public void ShowSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }
    
    public void HideSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
            
        SaveSettings();
    }
    
    void LoadSettings()
    {
        // Audio settings
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", DEFAULT_MUSIC_VOLUME);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", DEFAULT_SFX_VOLUME);
        float metronomeVolume = PlayerPrefs.GetFloat("MetronomeVolume", DEFAULT_METRONOME_VOLUME);
        bool useMetronome = PlayerPrefs.GetInt("UseMetronome", 0) == 1;
        
        // Gameplay settings
        float noteSpeed = PlayerPrefs.GetFloat("NoteSpeed", DEFAULT_NOTE_SPEED);
        float hitWindow = PlayerPrefs.GetFloat("HitWindow", DEFAULT_HIT_WINDOW);
        bool showFPS = PlayerPrefs.GetInt("ShowFPS", 0) == 1;
        bool pauseOnFocusLoss = PlayerPrefs.GetInt("PauseOnFocusLoss", 1) == 1;
        
        // Visual settings
        bool fullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        bool vsync = PlayerPrefs.GetInt("VSync", 1) == 1;
        
        // Key bindings
        for (int i = 0; i < currentKeys.Length; i++)
        {
            string keyName = PlayerPrefs.GetString($"Key_{i}", defaultKeys[i].ToString());
            if (System.Enum.TryParse(keyName, out KeyCode key))
            {
                currentKeys[i] = key;
            }
        }
        
        // Apply to UI
        ApplySettingsToUI(musicVolume, sfxVolume, metronomeVolume, useMetronome,
                         noteSpeed, hitWindow, showFPS, pauseOnFocusLoss,
                         fullscreen, vsync);
        
        // Apply to game systems
        ApplySettings();
    }
    
    void ApplySettingsToUI(float musicVolume, float sfxVolume, float metronomeVolume, bool useMetronome,
                          float noteSpeed, float hitWindow, bool showFPS, bool pauseOnFocusLoss,
                          bool fullscreen, bool vsync)
    {
        if (musicVolumeSlider != null) musicVolumeSlider.value = musicVolume;
        if (sfxVolumeSlider != null) sfxVolumeSlider.value = sfxVolume;
        if (metronomeVolumeSlider != null) metronomeVolumeSlider.value = metronomeVolume;
        if (metronomeToggle != null) metronomeToggle.isOn = useMetronome;
        
        if (noteSpeedSlider != null) noteSpeedSlider.value = noteSpeed;
        if (hitWindowSlider != null) hitWindowSlider.value = hitWindow;
        if (showFPSToggle != null) showFPSToggle.isOn = showFPS;
        if (pauseOnFocusLossToggle != null) pauseOnFocusLossToggle.isOn = pauseOnFocusLoss;
        
        if (fullscreenToggle != null) fullscreenToggle.isOn = fullscreen;
        if (vsyncToggle != null) vsyncToggle.isOn = vsync;
        
        // Update key binding texts
        UpdateKeyBindingTexts();
        
        // Update value displays
        UpdateVolumeDisplays();
        UpdateGameplayDisplays();
    }
    
    void ApplySettings()
    {
        // Apply audio settings
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(musicVolumeSlider?.value ?? DEFAULT_MUSIC_VOLUME);
            AudioManager.Instance.SetSFXVolume(sfxVolumeSlider?.value ?? DEFAULT_SFX_VOLUME);
            AudioManager.Instance.SetMetronomeVolume(metronomeVolumeSlider?.value ?? DEFAULT_METRONOME_VOLUME);
        }
        
        // Apply visual settings
        Screen.fullScreen = fullscreenToggle?.isOn ?? true;
        QualitySettings.vSyncCount = (vsyncToggle?.isOn ?? true) ? 1 : 0;
        
        // Apply input settings to InputManager
        InputManager inputManager = FindFirstObjectByType<InputManager>();
        if (inputManager != null)
        {
            inputManager.laneKeys = currentKeys;
        }
    }
    
    void SaveSettings()
    {
        // Save audio settings
        PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider?.value ?? DEFAULT_MUSIC_VOLUME);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolumeSlider?.value ?? DEFAULT_SFX_VOLUME);
        PlayerPrefs.SetFloat("MetronomeVolume", metronomeVolumeSlider?.value ?? DEFAULT_METRONOME_VOLUME);
        PlayerPrefs.SetInt("UseMetronome", (metronomeToggle?.isOn ?? false) ? 1 : 0);
        
        // Save gameplay settings
        PlayerPrefs.SetFloat("NoteSpeed", noteSpeedSlider?.value ?? DEFAULT_NOTE_SPEED);
        PlayerPrefs.SetFloat("HitWindow", hitWindowSlider?.value ?? DEFAULT_HIT_WINDOW);
        PlayerPrefs.SetInt("ShowFPS", (showFPSToggle?.isOn ?? false) ? 1 : 0);
        PlayerPrefs.SetInt("PauseOnFocusLoss", (pauseOnFocusLossToggle?.isOn ?? true) ? 1 : 0);
        
        // Save visual settings
        PlayerPrefs.SetInt("Fullscreen", (fullscreenToggle?.isOn ?? true) ? 1 : 0);
        PlayerPrefs.SetInt("VSync", (vsyncToggle?.isOn ?? true) ? 1 : 0);
        
        // Save key bindings
        for (int i = 0; i < currentKeys.Length; i++)
        {
            PlayerPrefs.SetString($"Key_{i}", currentKeys[i].ToString());
        }
        
        PlayerPrefs.Save();
        
        // Apply settings immediately
        ApplySettings();
    }
    
    void ResetToDefaults()
    {
        // Reset to default values
        if (musicVolumeSlider != null) musicVolumeSlider.value = DEFAULT_MUSIC_VOLUME;
        if (sfxVolumeSlider != null) sfxVolumeSlider.value = DEFAULT_SFX_VOLUME;
        if (metronomeVolumeSlider != null) metronomeVolumeSlider.value = DEFAULT_METRONOME_VOLUME;
        if (metronomeToggle != null) metronomeToggle.isOn = false;
        
        if (noteSpeedSlider != null) noteSpeedSlider.value = DEFAULT_NOTE_SPEED;
        if (hitWindowSlider != null) hitWindowSlider.value = DEFAULT_HIT_WINDOW;
        if (showFPSToggle != null) showFPSToggle.isOn = false;
        if (pauseOnFocusLossToggle != null) pauseOnFocusLossToggle.isOn = true;
        
        if (fullscreenToggle != null) fullscreenToggle.isOn = true;
        if (vsyncToggle != null) vsyncToggle.isOn = true;
        
        // Reset key bindings
        for (int i = 0; i < currentKeys.Length; i++)
        {
            currentKeys[i] = defaultKeys[i];
        }
        
        UpdateKeyBindingTexts();
        UpdateVolumeDisplays();
        UpdateGameplayDisplays();
        
        SaveSettings();
    }
    
    // Event handlers
    void OnMusicVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetMusicVolume(value);
        UpdateVolumeDisplays();
    }
    
    void OnSFXVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetSFXVolume(value);
        UpdateVolumeDisplays();
    }
    
    void OnMetronomeVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetMetronomeVolume(value);
        UpdateVolumeDisplays();
    }
    
    void OnMetronomeToggleChanged(bool value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.useMetronome = value;
    }
    
    void OnNoteSpeedChanged(float value)
    {
        UpdateGameplayDisplays();
    }
    
    void OnHitWindowChanged(float value)
    {
        UpdateGameplayDisplays();
    }
    
    void OnShowFPSChanged(bool value)
    {
        // Toggle FPS display if you have one
    }
    
    void OnPauseOnFocusLossChanged(bool value)
    {
        // Handle pause on focus loss setting
    }
    
    void OnFullscreenChanged(bool value)
    {
        Screen.fullScreen = value;
    }
    
    void OnVSyncChanged(bool value)
    {
        QualitySettings.vSyncCount = value ? 1 : 0;
    }
    
    void OnQualityChanged(int value)
    {
        QualitySettings.SetQualityLevel(value);
    }
    
    void OnResolutionChanged(int value)
    {
        Resolution[] resolutions = Screen.resolutions;
        if (value < resolutions.Length)
        {
            Screen.SetResolution(resolutions[value].width, resolutions[value].height, Screen.fullScreen);
        }
    }
    
    void StartKeyRebind(int keyIndex)
    {
        if (isWaitingForKey) return;
        
        keyToRebind = keyIndex;
        isWaitingForKey = true;
        
        if (keyBindTexts[keyIndex] != null)
        {
            keyBindTexts[keyIndex].text = "Press key...";
        }
    }
    
    void Update()
    {
        if (isWaitingForKey)
        {
            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(key) && key != KeyCode.Escape)
                {
                    currentKeys[keyToRebind] = key;
                    isWaitingForKey = false;
                    UpdateKeyBindingTexts();
                    break;
                }
            }
            
            // Cancel rebinding with Escape
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                isWaitingForKey = false;
                UpdateKeyBindingTexts();
            }
        }
    }
    
    void UpdateKeyBindingTexts()
    {
        for (int i = 0; i < keyBindTexts.Length && i < currentKeys.Length; i++)
        {
            if (keyBindTexts[i] != null)
            {
                keyBindTexts[i].text = currentKeys[i].ToString();
            }
        }
    }
    
    void UpdateVolumeDisplays()
    {
        if (musicVolumeText != null && musicVolumeSlider != null)
            musicVolumeText.text = $"Music: {(musicVolumeSlider.value * 100):F0}%";
            
        if (sfxVolumeText != null && sfxVolumeSlider != null)
            sfxVolumeText.text = $"SFX: {(sfxVolumeSlider.value * 100):F0}%";
            
        if (metronomeVolumeText != null && metronomeVolumeSlider != null)
            metronomeVolumeText.text = $"Metronome: {(metronomeVolumeSlider.value * 100):F0}%";
    }
    
    void UpdateGameplayDisplays()
    {
        if (noteSpeedText != null && noteSpeedSlider != null)
            noteSpeedText.text = $"Note Speed: {noteSpeedSlider.value:F1}";
            
        if (hitWindowText != null && hitWindowSlider != null)
            hitWindowText.text = $"Hit Window: {(hitWindowSlider.value * 1000):F0}ms";
    }
}
