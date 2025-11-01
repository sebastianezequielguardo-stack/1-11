using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Handles audio and video calibration for precise timing in the rhythm game
/// </summary>
public class CalibrationManager : MonoBehaviour
{
    [Header("UI Components")]
    public GameObject calibrationPanel;
    public Button startCalibrationButton;
    public Button finishCalibrationButton;
    public Button resetCalibrationButton;
    public Slider audioOffsetSlider;
    public TextMeshProUGUI audioOffsetText;
    public TextMeshProUGUI instructionsText;
    
    [Header("Audio Components")]
    public AudioSource metronomeSource;
    public AudioClip calibrationClick;
    
    [Header("Calibration Settings")]
    public int calibrationBeats = 8;
    public float calibrationBPM = 120f;
    
    [Header("Visual Feedback")]
    public GameObject beatIndicator;
    public Color onBeatColor = Color.green;
    public Color offBeatColor = Color.red;
    
    private bool isCalibrating = false;
    private float currentOffset = 0f;
    private int beatCount = 0;
    private float[] inputTimes;
    private float[] expectedTimes;
    private Coroutine calibrationCoroutine;
    
    void Start()
    {
        InitializeCalibration();
        LoadCalibrationSettings();
    }
    
    void InitializeCalibration()
    {
        if (calibrationPanel != null) calibrationPanel.SetActive(false);
        
        // Setup UI listeners
        if (startCalibrationButton != null)
            startCalibrationButton.onClick.AddListener(StartCalibration);
            
        if (finishCalibrationButton != null)
            finishCalibrationButton.onClick.AddListener(FinishCalibration);
            
        if (resetCalibrationButton != null)
            resetCalibrationButton.onClick.AddListener(ResetCalibration);
            
        if (audioOffsetSlider != null)
        {
            audioOffsetSlider.onValueChanged.AddListener(OnOffsetChanged);
            audioOffsetSlider.minValue = -0.5f;
            audioOffsetSlider.maxValue = 0.5f;
        }
        
        // Initialize arrays
        inputTimes = new float[calibrationBeats];
        expectedTimes = new float[calibrationBeats];
    }
    
    void LoadCalibrationSettings()
    {
        currentOffset = PlayerPrefs.GetFloat("AudioOffset", 0f);
        
        if (audioOffsetSlider != null)
            audioOffsetSlider.value = currentOffset;
            
        UpdateOffsetDisplay();
    }
    
    public void ShowCalibrationMenu()
    {
        if (calibrationPanel != null)
            calibrationPanel.SetActive(true);
            
        UpdateInstructions("Ajusta el offset de audio o inicia la calibración automática");
    }
    
    public void HideCalibrationMenu()
    {
        if (calibrationPanel != null)
            calibrationPanel.SetActive(false);
            
        if (isCalibrating)
            StopCalibration();
    }
    
    void StartCalibration()
    {
        if (isCalibrating) return;
        
        if (metronomeSource == null)
        {
            UpdateInstructions("Error: No se encontró AudioSource para el metrónomo");
            return;
        }
        
        if (calibrationClick == null)
        {
            UpdateInstructions("Error: No se asignó el clip de calibración");
            return;
        }
        
        isCalibrating = true;
        beatCount = 0;
        
        UpdateInstructions($"Presiona ESPACIO al ritmo del metrónomo ({calibrationBeats} beats)");
        
        if (startCalibrationButton != null)
            startCalibrationButton.interactable = false;
            
        calibrationCoroutine = StartCoroutine(CalibrationSequence());
    }
    
    void StopCalibration()
    {
        isCalibrating = false;
        
        if (calibrationCoroutine != null)
        {
            StopCoroutine(calibrationCoroutine);
            calibrationCoroutine = null;
        }
        
        if (metronomeSource != null && metronomeSource.isPlaying)
            metronomeSource.Stop();
            
        if (startCalibrationButton != null)
            startCalibrationButton.interactable = true;
    }
    
    void FinishCalibration()
    {
        if (isCalibrating)
        {
            StopCalibration();
            CalculateOffset();
        }
        
        SaveCalibrationSettings();
        HideCalibrationMenu();
    }
    
    IEnumerator CalibrationSequence()
    {
        float beatInterval = 60f / calibrationBPM;
        float startTime = Time.time;
        
        // Wait a moment before starting
        UpdateInstructions("Preparándose... 3");
        yield return new WaitForSeconds(1f);
        UpdateInstructions("Preparándose... 2");
        yield return new WaitForSeconds(1f);
        UpdateInstructions("Preparándose... 1");
        yield return new WaitForSeconds(1f);
        
        UpdateInstructions($"¡Presiona ESPACIO al ritmo! ({beatCount}/{calibrationBeats})");
        
        for (int i = 0; i < calibrationBeats; i++)
        {
            // Play metronome click
            if (metronomeSource != null && calibrationClick != null)
            {
                metronomeSource.PlayOneShot(calibrationClick);
            }
            
            // Record expected time
            expectedTimes[i] = Time.time - startTime;
            
            // Visual feedback
            if (beatIndicator != null)
            {
                StartCoroutine(BeatIndicatorFlash());
            }
            
            yield return new WaitForSeconds(beatInterval);
        }
        
        // Finish calibration
        UpdateInstructions("Calibración completada. Calculando offset...");
        yield return new WaitForSeconds(1f);
        
        CalculateOffset();
        isCalibrating = false;
        
        if (startCalibrationButton != null)
            startCalibrationButton.interactable = true;
    }
    
    IEnumerator BeatIndicatorFlash()
    {
        if (beatIndicator == null) yield break;
        
        Image indicator = beatIndicator.GetComponent<Image>();
        if (indicator == null) yield break;
        
        indicator.color = onBeatColor;
        yield return new WaitForSeconds(0.1f);
        indicator.color = offBeatColor;
    }
    
    void Update()
    {
        if (isCalibrating && Input.GetKeyDown(KeyCode.Space))
        {
            RecordInput();
        }
    }
    
    void RecordInput()
    {
        if (beatCount < calibrationBeats)
        {
            inputTimes[beatCount] = Time.time;
            beatCount++;
            
            UpdateInstructions($"¡Presiona ESPACIO al ritmo! ({beatCount}/{calibrationBeats})");
            
            // Visual feedback for input
            if (beatIndicator != null)
            {
                StartCoroutine(InputFeedback());
            }
        }
    }
    
    IEnumerator InputFeedback()
    {
        if (beatIndicator == null) yield break;
        
        Image indicator = beatIndicator.GetComponent<Image>();
        if (indicator == null) yield break;
        
        Color originalColor = indicator.color;
        indicator.color = Color.yellow;
        yield return new WaitForSeconds(0.05f);
        indicator.color = originalColor;
    }
    
    void CalculateOffset()
    {
        if (beatCount < calibrationBeats / 2) // Need at least half the beats
        {
            UpdateInstructions("No hay suficientes datos. Intenta de nuevo.");
            return;
        }
        
        float totalOffset = 0f;
        int validInputs = 0;
        
        for (int i = 0; i < beatCount; i++)
        {
            if (inputTimes[i] > 0 && expectedTimes[i] > 0)
            {
                float offset = inputTimes[i] - expectedTimes[i];
                totalOffset += offset;
                validInputs++;
            }
        }
        
        if (validInputs > 0)
        {
            float averageOffset = totalOffset / validInputs;
            currentOffset = -averageOffset; // Negative because we want to compensate
            
            if (audioOffsetSlider != null)
                audioOffsetSlider.value = currentOffset;
                
            UpdateOffsetDisplay();
            UpdateInstructions($"Offset calculado: {currentOffset:F3}s");
        }
        else
        {
            UpdateInstructions("Error en el cálculo. Intenta de nuevo.");
        }
    }
    
    void OnOffsetChanged(float value)
    {
        currentOffset = value;
        UpdateOffsetDisplay();
        
        // Apply to AudioManager if available
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetAudioOffset(currentOffset);
        }
    }
    
    void UpdateOffsetDisplay()
    {
        if (audioOffsetText != null)
        {
            audioOffsetText.text = $"Audio Offset: {currentOffset:F3}s";
        }
    }
    
    void UpdateInstructions(string text)
    {
        if (instructionsText != null)
        {
            instructionsText.text = text;
        }
    }
    
    void SaveCalibrationSettings()
    {
        PlayerPrefs.SetFloat("AudioOffset", currentOffset);
        PlayerPrefs.Save();
        
        // Apply to AudioManager
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetAudioOffset(currentOffset);
            if (AudioManager.Instance.GetComponent<MonoBehaviour>().GetType().GetMethod("SaveAudioSettings") != null)
            {
                AudioManager.Instance.GetComponent<MonoBehaviour>().Invoke("SaveAudioSettings", 0f);
            }
        }
        
        Debug.Log($"Calibration settings saved. Audio Offset: {currentOffset:F3}s");
    }
    
    void ResetCalibration()
    {
        currentOffset = 0f;
        
        if (audioOffsetSlider != null)
            audioOffsetSlider.value = 0f;
            
        UpdateOffsetDisplay();
        SaveCalibrationSettings();
        
        UpdateInstructions("Calibración restablecida a valores por defecto");
    }
    
    // Public methods for external access
    public float GetCurrentOffset()
    {
        return currentOffset;
    }
    
    public void SetOffset(float offset)
    {
        currentOffset = Mathf.Clamp(offset, -0.5f, 0.5f);
        if (audioOffsetSlider != null)
            audioOffsetSlider.value = currentOffset;
        UpdateOffsetDisplay();
    }
}
