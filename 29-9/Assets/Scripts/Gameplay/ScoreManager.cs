using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [Header("Valores de Puntuación")]
    public int perfectScore = 100;
    public int greatScore = 80;
    public int goodScore = 60;
    public int okScore = 40;
    public int missScore = 0; // Fallo no resta puntos

    [Header("Configuración del Multiplicador")]
    public int maxMultiplier = 4;
    public int notesForMultiplier = 10; // Notas necesarias para aumentar el multiplicador

    [Header("Referencias de UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI accuracyText;
    public TextMeshProUGUI multiplierText;
    public TextMeshProUGUI comboText;

    // Seguimiento de puntuación
    public int score = 0;
    public int totalNotes = 0;
    public int hitNotes = 0;
    public int perfectHits = 0;
    public int greatHits = 0;
    public int goodHits = 0;
    public int okHits = 0;
    public int missedNotes = 0;

    // Combo y multiplicador
    public int currentCombo = 0;
    public int maxCombo = 0;
    public int currentMultiplier = 1;
    public int consecutiveHits = 0;

    // Eventos
    public System.Action<int> OnScoreChanged;
    public System.Action<int> OnComboChanged;
    public System.Action<int> OnMultiplierChanged;

    void Start()
    {
        UpdateUI();

        // Suscripción a eventos del GameplayManager
        if (GameplayManager.Instance != null)
        {
            GameplayManager.Instance.OnNoteHit += OnNoteHit;
            GameplayManager.Instance.OnNoteMissed += OnNoteMiss;
        }
    }

    // Método usado por GameplayManager: RegisterHit(HitAccuracy)
    public void RegisterHit(HitAccuracy accuracy)
    {
        totalNotes++;
        hitNotes++;
        consecutiveHits++;
        currentCombo++;

        // Registrar tipo de acierto y sumar puntaje (con multiplicador)
        switch (accuracy)
        {
            case HitAccuracy.Perfect:
                perfectHits++;
                score += perfectScore * currentMultiplier;
                break;
            case HitAccuracy.Great:
                greatHits++;
                score += greatScore * currentMultiplier;
                break;
            case HitAccuracy.Good:
                goodHits++;
                score += goodScore * currentMultiplier;
                break;
            case HitAccuracy.Ok:
                okHits++;
                score += okScore * currentMultiplier;
                break;
        }

        // Actualizar combo máximo
        if (currentCombo > maxCombo)
            maxCombo = currentCombo;

        // Actualizar multiplicador según aciertos consecutivos
        UpdateMultiplier();

        // Disparar eventos públicos
        OnScoreChanged?.Invoke(score);
        OnComboChanged?.Invoke(currentCombo);

        UpdateUI();

        Debug.Log($"🎯 Acierto registrado - Precisión: {accuracy}, Puntos: +{GetScoreForAccuracy(accuracy) * currentMultiplier}, Combo: {currentCombo}");
    }

    // Método usado por GameplayManager: RegisterMiss()
    public void RegisterMiss()
    {
        totalNotes++;
        missedNotes++;
        consecutiveHits = 0;
        currentCombo = 0;
        currentMultiplier = 1;

        score += missScore; // Por defecto 0
        if (score < 0) score = 0;

        // Disparar eventos públicos
        OnScoreChanged?.Invoke(score);
        OnComboChanged?.Invoke(currentCombo);
        OnMultiplierChanged?.Invoke(currentMultiplier);

        UpdateUI();

        Debug.Log($"❌ Fallo registrado - Puntos: {missScore}, Combo reiniciado");
    }

    void UpdateMultiplier()
    {
        int newMultiplier = Mathf.Min(1 + (consecutiveHits / notesForMultiplier), maxMultiplier);

        if (newMultiplier != currentMultiplier)
        {
            currentMultiplier = newMultiplier;
            OnMultiplierChanged?.Invoke(currentMultiplier);
            Debug.Log($"🔥 ¡Multiplicador aumentado a {currentMultiplier}x!");
        }
    }

    // Método auxiliar con nombre original (compatible)
    int GetScoreForAccuracy(HitAccuracy accuracy)
    {
        switch (accuracy)
        {
            case HitAccuracy.Perfect: return perfectScore;
            case HitAccuracy.Great: return greatScore;
            case HitAccuracy.Good: return goodScore;
            case HitAccuracy.Ok: return okScore;
            default: return 0;
        }
    }

    // Eventos recibidos desde GameplayManager (se dejaron vacíos por si querés lógica adicional)
    void OnNoteHit(NoteData noteData, HitAccuracy accuracy)
    {
        // Lógica adicional al acertar una nota (opcional)
    }

    void OnNoteMiss(NoteData noteData)
    {
        // Lógica adicional al fallar una nota (opcional)
    }

    // --- Métodos públicos que otros scripts llaman en tu proyecto ---
    public float GetAccuracy()
    {
        return totalNotes > 0 ? (hitNotes / (float)totalNotes) * 100f : 0f;
    }

    public float GetAccuracyPercentage()
    {
        return GetAccuracy();
    }

    public float GetPerfectPercentage()
    {
        return totalNotes > 0 ? (perfectHits / (float)totalNotes) * 100f : 0f;
    }

    public int GetHitCount(HitAccuracy accuracy)
    {
        switch (accuracy)
        {
            case HitAccuracy.Perfect: return perfectHits;
            case HitAccuracy.Great: return greatHits;
            case HitAccuracy.Good: return goodHits;
            case HitAccuracy.Ok: return okHits;
            default: return 0;
        }
    }

    public int GetMissCount()
    {
        return missedNotes;
    }

    public ScoreData GetFinalScore()
    {
        return new ScoreData
        {
            finalScore = score,
            accuracy = GetAccuracy(),
            maxCombo = maxCombo,
            perfectHits = perfectHits,
            greatHits = greatHits,
            goodHits = goodHits,
            missedNotes = missedNotes,
            totalNotes = totalNotes
        };
    }
    // -------------------------------------------------------------

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = $"Puntaje: {score:N0}";

        if (accuracyText != null)
            accuracyText.text = $"Precisión: {GetAccuracy():F1}%";

        if (multiplierText != null)
            multiplierText.text = $"x{currentMultiplier}";

        if (comboText != null)
            comboText.text = $"Combo: {currentCombo}";
    }

    void OnDestroy()
    {
        // Desuscribir eventos del GameplayManager
        if (GameplayManager.Instance != null)
        {
            GameplayManager.Instance.OnNoteHit -= OnNoteHit;
            GameplayManager.Instance.OnNoteMissed -= OnNoteMiss;
        }
    }
}

[System.Serializable]
public class ScoreData
{
    public int finalScore;
    public float accuracy;
    public int maxCombo;
    public int perfectHits;
    public int greatHits;
    public int goodHits;
    public int missedNotes;
    public int totalNotes;
}
