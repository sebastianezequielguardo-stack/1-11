using UnityEngine;
using System.Collections.Generic;

public class HitZone : MonoBehaviour
{
    [Header("Hit Zone Configuration")]
    public Transform[] lanePositions; // Positions for each lane's hit zone
    [Tooltip("Hit zone height - use HitZoneSizer component for easy presets")]
    public float hitZoneHeight = 0.3f; // Fortnite-style compact size
    [Tooltip("Hit zone width - use HitZoneSizer component for easy presets")]
    public float hitZoneWidth = 0.4f; // Fortnite-style compact size
    
    [Header("Visual Feedback")]
    public GameObject[] laneIndicators; // Visual indicators for each lane
    public Color normalColor = Color.white;
    public Color pressedColor = Color.yellow;
    public Color hitColor = Color.green;
    public Color missColor = Color.red;
    
    [Header("Hit Detection")]
    public float perfectWindow = 0.05f;
    public float greatWindow = 0.08f;
    public float goodWindow = 0.1f;
    public float okWindow = 0.12f;
    public float missWindow = 0.15f;
    
    [Header("3D Hit Zone")]
    public float hitZoneZ = -8f; // Z position where notes should be hit (moved to match actual detection)
    
    private GameplayManager gameplayManager;
    private InputManager inputManager;
    private TexturedHitZoneIndicators texturedIndicators;
    private Dictionary<int, SpriteRenderer> laneRenderers = new Dictionary<int, SpriteRenderer>();
    private Dictionary<int, float> laneEffectTimers = new Dictionary<int, float>();

    void Start()
    {
        InitializeHitZone();
        SetupVisualFeedback();
    }
    
    void InitializeHitZone()
    {
        gameplayManager = GameplayManager.Instance;
        inputManager = FindFirstObjectByType<InputManager>();
        texturedIndicators = FindFirstObjectByType<TexturedHitZoneIndicators>();
        
        // Set up lane renderers for visual feedback
        for (int i = 0; i < lanePositions.Length; i++)
        {
            if (laneIndicators != null && i < laneIndicators.Length && laneIndicators[i] != null)
            {
                SpriteRenderer renderer = laneIndicators[i].GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    laneRenderers[i] = renderer;
                    renderer.color = normalColor;
                }
            }
            
            laneEffectTimers[i] = 0f;
        }
        
        // Subscribe to events
        if (gameplayManager != null)
        {
            gameplayManager.OnNoteHit += OnNoteHit;
            gameplayManager.OnNoteMissed += OnNoteMissed;
        }
    }
    
    void SetupVisualFeedback()
    {
        UpdateVisualFeedback();
    }
    
    // Public method to update visual feedback (called by HitZoneSizer)
    public void UpdateVisualFeedback()
    {
        // Create hit zone visual boundaries if they don't exist
        for (int i = 0; i < lanePositions.Length; i++)
        {
            if (laneIndicators == null || i >= laneIndicators.Length || laneIndicators[i] == null)
                continue;
                
            Transform laneTransform = laneIndicators[i].transform;
            
            // Set position to match lane
            if (i < lanePositions.Length)
            {
                Vector3 pos = lanePositions[i].position;
                pos.y = transform.position.y; // Keep hit zone Y position
                laneTransform.position = pos;
            }
            
            // Set scale to match hit zone size (preserve any custom scaling)
            Vector3 currentScale = laneTransform.localScale;
            laneTransform.localScale = new Vector3(
                hitZoneWidth * (currentScale.x / Mathf.Max(currentScale.x, currentScale.y)), 
                hitZoneHeight * (currentScale.y / Mathf.Max(currentScale.x, currentScale.y)), 
                1f
            );
        }
    }

    void Update()
    {
        UpdateVisualEffects();
        HandleInputFeedback();
    }
    
    void UpdateVisualEffects()
    {
        // Update lane effect timers and colors
        for (int i = 0; i < lanePositions.Length; i++)
        {
            if (laneEffectTimers.ContainsKey(i) && laneEffectTimers[i] > 0f)
            {
                laneEffectTimers[i] -= Time.deltaTime;
                
                if (laneEffectTimers[i] <= 0f)
                {
                    // Reset to normal color
                    SetLaneColor(i, normalColor);
                }
            }
        }
    }
    
    void HandleInputFeedback()
    {
        if (inputManager == null) return;
        
        // Check for key presses and update visual feedback
        for (int i = 0; i < lanePositions.Length && i < inputManager.laneKeys.Length; i++)
        {
            if (Input.GetKeyDown(inputManager.laneKeys[i]))
            {
                OnLanePressed(i);
            }
            else if (Input.GetKeyUp(inputManager.laneKeys[i]))
            {
                OnLaneReleased(i);
            }
        }
    }
    
    void OnLanePressed(int laneIndex)
    {
        SetLaneColor(laneIndex, pressedColor);
        
        // Add visual effect (scale pulse, etc.)
        if (laneIndicators != null && laneIndex < laneIndicators.Length && laneIndicators[laneIndex] != null)
        {
            StartCoroutine(PulseLane(laneIndex));
        }
    }
    
    void OnLaneReleased(int laneIndex)
    {
        // Only reset color if no effect timer is running
        if (!laneEffectTimers.ContainsKey(laneIndex) || laneEffectTimers[laneIndex] <= 0f)
        {
            SetLaneColor(laneIndex, normalColor);
        }
    }
    
    System.Collections.IEnumerator PulseLane(int laneIndex)
    {
        if (laneIndicators == null || laneIndex >= laneIndicators.Length || laneIndicators[laneIndex] == null)
            yield break;
            
        Transform laneTransform = laneIndicators[laneIndex].transform;
        Vector3 originalScale = laneTransform.localScale;
        Vector3 targetScale = originalScale * 1.2f;
        
        float duration = 0.1f;
        float elapsed = 0f;
        
        // Scale up
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            laneTransform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }
        
        elapsed = 0f;
        
        // Scale back down
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            laneTransform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            yield return null;
        }
        
        laneTransform.localScale = originalScale;
    }
    
    void OnNoteHit(NoteData noteData, HitAccuracy accuracy)
    {
        Color effectColor = GetAccuracyColor(accuracy);
        SetLaneColor(noteData.laneIndex, effectColor);
        laneEffectTimers[noteData.laneIndex] = 0.3f; // Show effect for 0.3 seconds
        
        // Trigger textured button hit effect
        if (texturedIndicators != null)
        {
            texturedIndicators.TriggerHitEffect(noteData.laneIndex);
        }
        
        // Create hit particle effect
        CreateHitEffect(noteData.laneIndex, effectColor);
    }
    
    void OnNoteMissed(NoteData noteData)
    {
        SetLaneColor(noteData.laneIndex, missColor);
        laneEffectTimers[noteData.laneIndex] = 0.5f; // Show miss effect longer
    }
    
    Color GetAccuracyColor(HitAccuracy accuracy)
    {
        switch (accuracy)
        {
            case HitAccuracy.Perfect: return Color.cyan;
            case HitAccuracy.Great: return Color.green;
            case HitAccuracy.Good: return Color.yellow;
            case HitAccuracy.Miss: return missColor;
            default: return normalColor;
        }
    }
    
    void SetLaneColor(int laneIndex, Color color)
    {
        if (laneRenderers.ContainsKey(laneIndex) && laneRenderers[laneIndex] != null)
        {
            laneRenderers[laneIndex].color = color;
        }
    }
    
    void CreateHitEffect(int laneIndex, Color effectColor)
    {
        if (laneIndex < 0 || laneIndex >= lanePositions.Length) return;
        
        Vector3 effectPosition = lanePositions[laneIndex].position;
        
        // Create a simple particle effect
        GameObject effect = new GameObject("HitEffect");
        effect.transform.position = effectPosition;
        
        // Add a sprite renderer for visual effect
        SpriteRenderer effectRenderer = effect.AddComponent<SpriteRenderer>();
        effectRenderer.color = effectColor;
        effectRenderer.sortingOrder = 20; // Above notes
        
        // Create a simple circle sprite (you can replace with your own)
        effectRenderer.sprite = CreateCircleSprite();
        
        // Animate the effect
        StartCoroutine(AnimateHitEffect(effect, effectColor));
    }
    
    Sprite CreateCircleSprite()
    {
        // Create a simple circle texture
        int size = 64;
        Texture2D texture = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];
        
        Vector2 center = new Vector2(size / 2f, size / 2f);
        float radius = size / 2f - 2f;
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                if (distance <= radius)
                {
                    float alpha = 1f - (distance / radius);
                    pixels[y * size + x] = new Color(1f, 1f, 1f, alpha);
                }
                else
                {
                    pixels[y * size + x] = Color.clear;
                }
            }
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }
    
    System.Collections.IEnumerator AnimateHitEffect(GameObject effect, Color startColor)
    {
        SpriteRenderer renderer = effect.GetComponent<SpriteRenderer>();
        Transform effectTransform = effect.transform;
        
        float duration = 0.5f;
        float elapsed = 0f;
        Vector3 startScale = Vector3.one * 0.5f;
        Vector3 endScale = Vector3.one * 2f;
        
        effectTransform.localScale = startScale;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // Scale up and fade out
            effectTransform.localScale = Vector3.Lerp(startScale, endScale, t);
            
            Color color = startColor;
            color.a = Mathf.Lerp(1f, 0f, t);
            renderer.color = color;
            
            yield return null;
        }
        
        Destroy(effect);
    }
    
    // Public method to get hit zone position for a lane
    public Vector3 GetLaneHitPosition(int laneIndex)
    {
        if (laneIndex >= 0 && laneIndex < lanePositions.Length)
        {
            return lanePositions[laneIndex].position;
        }
        return Vector3.zero;
    }
    
    // Public method to check if a position is within hit zone
    public bool IsInHitZone(Vector3 position, int laneIndex)
    {
        if (laneIndex < 0 || laneIndex >= lanePositions.Length) return false;
        
        Vector3 lanePos = lanePositions[laneIndex].position;
        float distance = Vector3.Distance(position, lanePos);
        
        return distance <= (hitZoneHeight / 2f);
    }
    
    void OnDestroy()
    {
        // Unsubscribe from events
        if (gameplayManager != null)
        {
            gameplayManager.OnNoteHit -= OnNoteHit;
            gameplayManager.OnNoteMissed -= OnNoteMissed;
        }
    }
    
    // Gizmos for editor visualization
    void OnDrawGizmos()
    {
        if (lanePositions == null) return;
        
        Gizmos.color = Color.yellow;
        for (int i = 0; i < lanePositions.Length; i++)
        {
            if (lanePositions[i] != null)
            {
                Vector3 pos = lanePositions[i].position;
                Gizmos.DrawWireCube(pos, new Vector3(hitZoneWidth, hitZoneHeight, 0.1f));
            }
        }
    }
}
