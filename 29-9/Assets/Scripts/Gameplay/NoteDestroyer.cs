using UnityEngine;

/// <summary>
/// Component that automatically destroys notes when they go beyond camera bounds
/// This provides an additional safety net for note cleanup
/// </summary>
public class NoteDestroyer : MonoBehaviour
{
    [Header("Destruction Settings")]
    public float behindCameraOffset = 5f; // Extra distance behind camera before destroying
    public float farFromCameraOffset = 50f; // Extra distance in front of camera before destroying
    public float sideBoundOffset = 2f; // Extra distance on sides before destroying
    
    [Header("Debug")]
    public bool showBounds = false; // Show destruction bounds in scene view
    
    private Camera mainCamera;
    private float destroyCheckInterval = 0.1f; // Check every 0.1 seconds for performance
    private float lastCheckTime = 0f;
    private bool isDestroyed = false; // Prevent multiple destruction calls
    
    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = FindFirstObjectByType<Camera>();
        }
        
        if (mainCamera == null)
        {
            Debug.LogWarning("⚠️ NoteDestroyer: No camera found! Notes may not be destroyed properly.");
        }
    }
    
    void Update()
    {
        // Don't check if already destroyed
        if (isDestroyed) return;
        
        // Only check bounds periodically for performance
        if (Time.time - lastCheckTime >= destroyCheckInterval)
        {
            CheckBounds();
            lastCheckTime = Time.time;
        }
    }
    
    void CheckBounds()
    {
        if (mainCamera == null) return;
        
        Vector3 worldPos = transform.position;
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(worldPos);
        
        // Check if note is completely outside camera bounds with buffer
        bool outsideBounds = false;
        string reason = "";
        
        // Check if behind camera (past the hit zone)
        if (worldPos.z < mainCamera.transform.position.z - behindCameraOffset)
        {
            outsideBounds = true;
            reason = "behind camera";
        }
        // Check if too far in front of camera
        else if (worldPos.z > mainCamera.transform.position.z + farFromCameraOffset)
        {
            outsideBounds = true;
            reason = "too far from camera";
        }
        // Check left bound
        else if (screenPoint.x < -0.1f)
        {
            outsideBounds = true;
            reason = "left of camera";
        }
        // Check right bound
        else if (screenPoint.x > 1.1f)
        {
            outsideBounds = true;
            reason = "right of camera";
        }
        
        if (outsideBounds)
        {
            // Prevent multiple destruction calls
            if (isDestroyed) return;
            isDestroyed = true;
            
            Debug.Log($"🗑️ NoteDestroyer: Destroying note {reason}");
            
            // Try to get Note component for proper cleanup
            Note noteComponent = GetComponent<Note>();
            if (noteComponent != null)
            {
                // Let the Note component handle its own destruction
                noteComponent.OnNoteDestroyed?.Invoke(noteComponent);
            }
            
            Destroy(gameObject);
        }
    }
    
    // Get world bounds for the destruction area
    public Bounds GetDestructionBounds()
    {
        if (mainCamera == null) return new Bounds();
        
        float height = mainCamera.orthographicSize * 2f;
        float width = height * mainCamera.aspect;
        
        // Add offsets
        height += behindCameraOffset + farFromCameraOffset;
        width += sideBoundOffset * 2f;
        
        return new Bounds(mainCamera.transform.position, new Vector3(width, height, 10f));
    }
    
    void OnDrawGizmos()
    {
        if (!showBounds || mainCamera == null) return;
        
        Bounds bounds = GetDestructionBounds();
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(bounds.center, bounds.size);
        
        // Draw camera bounds for reference
        float camHeight = mainCamera.orthographicSize * 2f;
        float camWidth = camHeight * mainCamera.aspect;
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(mainCamera.transform.position, new Vector3(camWidth, camHeight, 1f));
    }
}
