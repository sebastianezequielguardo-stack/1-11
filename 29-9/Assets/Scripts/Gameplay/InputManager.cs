using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    [Header("Input Configuration")]
    public KeyCode[] laneKeys = { KeyCode.D, KeyCode.F, KeyCode.J, KeyCode.K, KeyCode.L };
    public KeyCode pauseKey = KeyCode.Escape;
    public KeyCode restartKey = KeyCode.R;

    [Header("Controller Pause")]
    public KeyCode controllerPauseButton = KeyCode.Joystick1Button9;
    public KeyCode controllerRestartButton = KeyCode.Joystick1Button8;

    [Header("Controller Support")]
    public bool useController = true;
    public KeyCode[] controllerButtons = {
        KeyCode.Joystick1Button4,
        KeyCode.Joystick1Button2,
        KeyCode.Joystick1Button0,
        KeyCode.Joystick1Button1,
        KeyCode.Joystick1Button5
    };

    [Header("Input Feedback")]
    public GameObject[] hitEffects;
    public AudioSource hitSound;
    public AudioSource missSound;

    [Header("Chord Settings")]
    public ChordInputSettings chordSettings;

    private GameplayManager gameplayManager;
    private bool[] keyHeld;
    private bool[] keyPressedThisFrame;
    private float[] keyPressTime;

    // ✨ NUEVO: Estados para input externo (Arduino)
    private bool[] externalKeyHeld;

    private float chordDetectionWindow = 0.05f;
    private bool enableChordDebugLogging = true;

    void Start()
    {
        gameplayManager = GameplayManager.Instance;

        keyHeld = new bool[laneKeys.Length];
        keyPressedThisFrame = new bool[laneKeys.Length];
        keyPressTime = new float[laneKeys.Length];
        externalKeyHeld = new bool[laneKeys.Length]; // ✨ Inicializa array para Arduino

        LoadChordSettings();

        if (gameplayManager != null)
        {
            gameplayManager.OnNoteHit += OnNoteHitFeedback;
            gameplayManager.OnNoteMissed += OnNoteMissedFeedback;
        }
    }

    void Update()
    {
        if (gameplayManager == null || !gameplayManager.isGameActive)
            return;

        HandleGameplayInput();
        HandleSystemInput();
    }

    void HandleGameplayInput()
    {
        // Resetea los flags de "presionado este frame"
        for (int i = 0; i < keyPressedThisFrame.Length; i++)
            keyPressedThisFrame[i] = false;

        for (int i = 0; i < laneKeys.Length; i++)
        {
            // Detecta input de teclado/controlador
            bool keyboardPressed = Input.GetKeyDown(laneKeys[i]);
            bool controllerPressed = useController && i < controllerButtons.Length && Input.GetKeyDown(controllerButtons[i]);
            bool keyboardHeld = Input.GetKey(laneKeys[i]);
            bool controllerHeld = useController && i < controllerButtons.Length && Input.GetKey(controllerButtons[i]);
            bool keyboardReleased = Input.GetKeyUp(laneKeys[i]);
            bool controllerReleased = useController && i < controllerButtons.Length && Input.GetKeyUp(controllerButtons[i]);

            // Guarda el estado anterior
            bool wasHeld = keyHeld[i];

            // ✨ CLAVE: Combina input de teclado/controlador CON input externo (Arduino)
            bool currentlyHeld = keyboardHeld || controllerHeld || externalKeyHeld[i];
            bool justPressed = keyboardPressed || controllerPressed;

            // Actualiza estado
            if (justPressed)
            {
                keyPressedThisFrame[i] = true;
                keyHeld[i] = true;
                keyPressTime[i] = Time.time;
            }
            else
            {
                keyHeld[i] = currentlyHeld;
            }

            // Actualiza efecto visual solo si cambió el estado
            if (wasHeld != keyHeld[i])
            {
                TriggerLaneEffect(i, keyHeld[i]);
            }

            // Detecta liberación
            if ((keyboardReleased || controllerReleased) && !currentlyHeld)
            {
                keyHeld[i] = false;
                TriggerLaneEffect(i, false);
            }
        }

        ProcessChordInput();
    }

    void HandleSystemInput()
    {
        bool pausePressed = Input.GetKeyDown(pauseKey) || (useController && Input.GetKeyDown(controllerPauseButton));
        if (pausePressed)
        {
            if (gameplayManager.isPaused) gameplayManager.ResumeGame();
            else gameplayManager.PauseGame();
        }

        bool restartPressed = Input.GetKeyDown(restartKey) || (useController && Input.GetKeyDown(controllerRestartButton));
        if (gameplayManager.isPaused && restartPressed)
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    void ProcessChordInput()
    {
        var chordLanes = new System.Collections.Generic.List<int>();
        float currentTime = Time.time;

        for (int i = 0; i < laneKeys.Length; i++)
        {
            if (keyPressedThisFrame[i] || (keyHeld[i] && (currentTime - keyPressTime[i]) <= chordDetectionWindow))
                chordLanes.Add(i);
        }

        if (chordLanes.Count > 0)
        {
            if (chordLanes.Count >= GetMinimumChordSize())
                TryHitChord(chordLanes);
            else if (chordLanes.Count == 1 && keyPressedThisFrame[chordLanes[0]])
                TryHitNote(chordLanes[0]);
        }
    }

    public void TryHitNote(int laneIndex)
    {
        if (gameplayManager.TryHitNote(laneIndex, out HitAccuracy accuracy))
            PlayHitSound(accuracy);
        else
            PlayMissSound();
    }

    void TryHitChord(System.Collections.Generic.List<int> chordLanes)
    {
        if (gameplayManager.TryHitChord(chordLanes, out HitAccuracy accuracy))
        {
            PlayHitSound(accuracy);
            if (enableChordDebugLogging)
                Debug.Log($"Chord hit! Lanes: [{string.Join(", ", chordLanes)}]");
        }
        else
        {
            bool anyHit = false;
            foreach (int lane in chordLanes)
            {
                if (keyPressedThisFrame[lane] && gameplayManager.TryHitNote(lane, out HitAccuracy noteAccuracy))
                {
                    PlayHitSound(noteAccuracy);
                    anyHit = true;
                }
            }
            if (!anyHit) PlayMissSound();
        }
    }

    void TriggerLaneEffect(int laneIndex, bool pressed)
    {
        if (hitEffects == null || laneIndex >= hitEffects.Length || hitEffects[laneIndex] == null)
            return;

        GameObject botonImagen = hitEffects[laneIndex];
        botonImagen.SetActive(!pressed);

        if (botonImagen.TryGetComponent<Image>(out var image))
        {
            image.SetAllDirty();
            LayoutRebuilder.ForceRebuildLayoutImmediate(image.rectTransform);
        }
    }

    void PlayHitSound(HitAccuracy accuracy)
    {
        if (hitSound == null) return;
        switch (accuracy)
        {
            case HitAccuracy.Perfect: hitSound.pitch = 1.2f; break;
            case HitAccuracy.Great: hitSound.pitch = 1.1f; break;
            case HitAccuracy.Good: hitSound.pitch = 1.0f; break;
        }
        hitSound.Play();
    }

    void PlayMissSound()
    {
        if (missSound != null) missSound.Play();
    }

    void OnNoteHitFeedback(NoteData noteData, HitAccuracy accuracy) { }
    void OnNoteMissedFeedback(NoteData noteData) { }

    void LoadChordSettings()
    {
        if (chordSettings != null)
        {
            chordDetectionWindow = chordSettings.chordDetectionWindow;
            enableChordDebugLogging = chordSettings.enableChordDebugLogging;
        }
    }

    int GetMinimumChordSize()
    {
        return chordSettings != null ? chordSettings.minimumChordSize : 2;
    }

    void OnDestroy()
    {
        if (gameplayManager != null)
        {
            gameplayManager.OnNoteHit -= OnNoteHitFeedback;
            gameplayManager.OnNoteMissed -= OnNoteMissedFeedback;
        }
    }

    // ========== MÉTODOS PARA INPUT EXTERNO (ARDUINO) ==========

    /// <summary>
    /// Método principal para simular input desde Arduino
    /// </summary>
    public void SimulateButtonPress(int laneIndex, bool isPressed)
    {
        if (gameplayManager == null || !gameplayManager.isGameActive)
            return;

        if (laneIndex < 0 || laneIndex >= laneKeys.Length)
            return;

        Debug.Log($"🎮 Arduino Input - Lane: {laneIndex}, Pressed: {isPressed}");

        if (isPressed)
        {
            // PRESIONAR
            externalKeyHeld[laneIndex] = true;
            keyPressedThisFrame[laneIndex] = true;
            keyHeld[laneIndex] = true;
            keyPressTime[laneIndex] = Time.time;

            TriggerLaneEffect(laneIndex, true);

            // Procesa la nota
            if (gameplayManager.TryHitNote(laneIndex, out HitAccuracy accuracy))
                PlayHitSound(accuracy);
            else
                PlayMissSound();
        }
        else
        {
            // SOLTAR
            externalKeyHeld[laneIndex] = false;

            // Solo libera si no hay input de teclado/controlador
            bool keyboardHeld = Input.GetKey(laneKeys[laneIndex]);
            bool controllerHeld = useController && laneIndex < controllerButtons.Length && Input.GetKey(controllerButtons[laneIndex]);

            if (!keyboardHeld && !controllerHeld)
            {
                keyHeld[laneIndex] = false;
                TriggerLaneEffect(laneIndex, false);
            }
        }
    }

    /// <summary>
    /// Método de compatibilidad (simula press + auto-release)
    /// </summary>
    public void SimulateButtonPress(int laneIndex)
    {
        SimulateButtonPress(laneIndex, true);
        StartCoroutine(ReleaseLaneEffectAfterDelay(laneIndex, 0.1f));
    }

    private System.Collections.IEnumerator ReleaseLaneEffectAfterDelay(int laneIndex, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (laneIndex >= 0 && laneIndex < externalKeyHeld.Length)
        {
            externalKeyHeld[laneIndex] = false;

            // Solo libera si no hay input de teclado
            bool keyboardHeld = Input.GetKey(laneKeys[laneIndex]);
            bool controllerHeld = useController && laneIndex < controllerButtons.Length && Input.GetKey(controllerButtons[laneIndex]);

            if (!keyboardHeld && !controllerHeld)
            {
                keyHeld[laneIndex] = false;
                TriggerLaneEffect(laneIndex, false);
            }
        }
    }

    public void ReleaseButton(int laneIndex)
    {
        SimulateButtonPress(laneIndex, false);
    }

    public bool IsKeyHeld(int laneIndex)
    {
        if (laneIndex < 0 || laneIndex >= keyHeld.Length)
            return false;
        return keyHeld[laneIndex];
    }
}
