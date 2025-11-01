using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Maneja el countdown al inicio del juego (para reinicio)
/// </summary>
public class GameStartCountdown : MonoBehaviour
{
    [Header("Countdown Settings")]
    public bool showCountdownOnStart = true;
    public float countdownDuration = 3f;
    
    [Header("Auto Detection")]
    public bool pauseGameplayOnStart = true;
    
    private bool hasShownCountdown = false;
    
    void Start()
    {
        if (showCountdownOnStart && !hasShownCountdown)
        {
            StartCoroutine(StartGameCountdown());
        }
    }
    
    System.Collections.IEnumerator StartGameCountdown()
    {
        hasShownCountdown = true;
        
        if (pauseGameplayOnStart)
        {
            // Pause everything at start
            Time.timeScale = 0f;
            
            // Pause audio
            AudioSource[] sources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
            foreach (var source in sources)
            {
                if (source.isPlaying)
                {
                    source.Pause();
                }
            }
            
            // Disable gameplay scripts temporarily
            DisableGameplayScripts();
        }
        
        // Create countdown UI
        GameObject countdownObj = CreateCountdownUI();
        TextMeshProUGUI countdownText = countdownObj.GetComponent<TextMeshProUGUI>();
        
        // Countdown from 3 to 1
        for (int i = 3; i >= 1; i--)
        {
            countdownText.text = i.ToString();
            countdownText.fontSize = 150f;
            countdownText.color = Color.white;
            
            // Scale animation
            StartCoroutine(AnimateCountdownNumber(countdownText));
            
            yield return new WaitForSecondsRealtime(1f);
        }
        
        // Show "GO!" or "¡VAMOS!"
        countdownText.text = "¡VAMOS!";
        countdownText.fontSize = 120f;
        countdownText.color = Color.green;
        StartCoroutine(AnimateCountdownNumber(countdownText));
        
        yield return new WaitForSecondsRealtime(0.8f);
        
        if (pauseGameplayOnStart)
        {
            // Resume everything
            EnableGameplayScripts();
            
            // Resume audio
            AudioSource[] sources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
            foreach (var source in sources)
            {
                source.UnPause();
            }
            
            Time.timeScale = 1f;
        }
        
        // Fade out countdown
        yield return StartCoroutine(FadeOutCountdown(countdownObj));
        
        // Destroy countdown UI
        Destroy(countdownObj);
        
        Debug.Log("🎮 Game started after countdown!");
    }
    
    GameObject CreateCountdownUI()
    {
        // Create canvas
        GameObject canvasObj = new GameObject("StartCountdownCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 3000; // Highest priority
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        canvasObj.AddComponent<GraphicRaycaster>();
        
        // Create countdown text
        GameObject countdownObj = new GameObject("StartCountdownText");
        countdownObj.transform.SetParent(canvasObj.transform, false);
        
        RectTransform rect = countdownObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(400f, 300f);
        rect.anchoredPosition = Vector2.zero;
        
        TextMeshProUGUI text = countdownObj.AddComponent<TextMeshProUGUI>();
        text.text = "3";
        text.fontSize = 150f;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.Center;
        text.fontStyle = FontStyles.Bold;
        
        // Add outline and shadow for better visibility
        text.outlineWidth = 0.4f;
        text.outlineColor = Color.black;
        
        return canvasObj;
    }
    
    System.Collections.IEnumerator AnimateCountdownNumber(TextMeshProUGUI text)
    {
        Vector3 originalScale = text.transform.localScale;
        Vector3 bigScale = originalScale * 1.8f;
        
        float duration = 0.4f;
        float elapsed = 0f;
        
        // Scale up with bounce
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            float bounceT = Mathf.Sin(t * Mathf.PI * 0.5f); // Smooth bounce
            text.transform.localScale = Vector3.Lerp(originalScale, bigScale, bounceT);
            yield return null;
        }
        
        elapsed = 0f;
        
        // Scale down
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            text.transform.localScale = Vector3.Lerp(bigScale, originalScale, t);
            yield return null;
        }
        
        text.transform.localScale = originalScale;
    }
    
    System.Collections.IEnumerator FadeOutCountdown(GameObject countdownObj)
    {
        TextMeshProUGUI text = countdownObj.GetComponentInChildren<TextMeshProUGUI>();
        Color originalColor = text.color;
        
        float duration = 0.8f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            
            Color newColor = originalColor;
            newColor.a = Mathf.Lerp(1f, 0f, t);
            text.color = newColor;
            
            // Scale down while fading
            float scale = Mathf.Lerp(1f, 0.5f, t);
            text.transform.localScale = Vector3.one * scale;
            
            yield return null;
        }
    }
    
    void DisableGameplayScripts()
    {
        // Disable note spawners
        NoteSpawner[] spawners = FindObjectsByType<NoteSpawner>(FindObjectsSortMode.None);
        foreach (var spawner in spawners)
        {
            spawner.enabled = false;
        }
        
        NoteSpawner2D[] spawners2D = FindObjectsByType<NoteSpawner2D>(FindObjectsSortMode.None);
        foreach (var spawner in spawners2D)
        {
            spawner.enabled = false;
        }
        
        // Disable gameplay manager
        GameplayManager[] managers = FindObjectsByType<GameplayManager>(FindObjectsSortMode.None);
        foreach (var manager in managers)
        {
            manager.enabled = false;
        }
    }
    
    void EnableGameplayScripts()
    {
        // Enable note spawners
        NoteSpawner[] spawners = FindObjectsByType<NoteSpawner>(FindObjectsSortMode.None);
        foreach (var spawner in spawners)
        {
            spawner.enabled = true;
        }
        
        NoteSpawner2D[] spawners2D = FindObjectsByType<NoteSpawner2D>(FindObjectsSortMode.None);
        foreach (var spawner in spawners2D)
        {
            spawner.enabled = true;
        }
        
        // Enable gameplay manager
        GameplayManager[] managers = FindObjectsByType<GameplayManager>(FindObjectsSortMode.None);
        foreach (var manager in managers)
        {
            manager.enabled = true;
        }
    }
    
    // Public method to trigger countdown manually
    public void StartCountdown()
    {
        if (!hasShownCountdown)
        {
            StartCoroutine(StartGameCountdown());
        }
    }
    
    // Method to skip countdown
    [ContextMenu("Skip Countdown")]
    public void SkipCountdown()
    {
        StopAllCoroutines();
        
        if (pauseGameplayOnStart)
        {
            EnableGameplayScripts();
            Time.timeScale = 1f;
            
            AudioSource[] sources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
            foreach (var source in sources)
            {
                source.UnPause();
            }
        }
        
        // Clean up any countdown UI
        GameObject[] countdownObjects = GameObject.FindGameObjectsWithTag("Untagged");
        foreach (var obj in countdownObjects)
        {
            if (obj.name.Contains("Countdown"))
            {
                Destroy(obj);
            }
        }
        
        hasShownCountdown = true;
        Debug.Log("⏭️ Countdown skipped!");
    }
}
