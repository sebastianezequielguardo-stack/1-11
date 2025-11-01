using UnityEngine;

/// <summary>
/// Sets up the camera for optimal 3D highway gameplay view
/// </summary>
public class CameraSetup : MonoBehaviour
{
    [Header("Camera Position")]
    public Vector3 cameraPosition = new Vector3(0f, 8f, -12f);
    public Vector3 cameraRotation = new Vector3(25f, 0f, 0f);
    
    [Header("Camera Settings")]
    public float fieldOfView = 60f;
    public float nearClipPlane = 0.1f;
    public float farClipPlane = 100f;
    
    [Header("Highway View")]
    public float hitZoneZ = -5f; // Where the hit zone should be (visible but near camera)
    public float highwayLength = 50f; // Length of visible highway
    
    private Camera gameCamera;
    
    void Start()
    {
        SetupCamera();
    }
    
    void SetupCamera()
    {
        gameCamera = GetComponent<Camera>();
        if (gameCamera == null)
        {
            gameCamera = Camera.main;
        }
        
        if (gameCamera != null)
        {
            // Set camera position and rotation
            transform.position = cameraPosition;
            transform.rotation = Quaternion.Euler(cameraRotation);
            
            // Configure camera settings
            gameCamera.fieldOfView = fieldOfView;
            gameCamera.nearClipPlane = nearClipPlane;
            gameCamera.farClipPlane = farClipPlane;
            
            // Ensure perspective projection
            gameCamera.orthographic = false;
            
            Debug.Log($"🎥 Camera positioned at: {transform.position}, rotation: {transform.rotation.eulerAngles}");
            Debug.Log($"🎯 Hit zone should appear at screen bottom (Z: {hitZoneZ})");
        }
    }
    
    /// <summary>
    /// Adjust camera to better frame the highway
    /// </summary>
    public void OptimizeForHighway()
    {
        if (gameCamera == null) return;
        
        // Calculate optimal position to see the full highway
        float optimalDistance = highwayLength * 0.4f;
        float optimalHeight = highwayLength * 0.3f;
        
        Vector3 newPosition = new Vector3(0f, optimalHeight, -optimalDistance);
        Vector3 newRotation = new Vector3(20f, 0f, 0f);
        
        transform.position = newPosition;
        transform.rotation = Quaternion.Euler(newRotation);
        
        Debug.Log($"🎥 Camera optimized for highway view");
    }
    
    /// <summary>
    /// Get the screen position of a world point
    /// </summary>
    public Vector3 WorldToScreenPoint(Vector3 worldPoint)
    {
        if (gameCamera != null)
        {
            return gameCamera.WorldToScreenPoint(worldPoint);
        }
        return Vector3.zero;
    }
    
    /// <summary>
    /// Check if the hit zone is visible at the bottom of the screen
    /// </summary>
    public bool IsHitZoneAtBottomOfScreen()
    {
        if (gameCamera == null) return false;
        
        // Check where the hit zone appears on screen
        Vector3 hitZoneWorldPos = new Vector3(0f, 0f, hitZoneZ);
        Vector3 screenPos = gameCamera.WorldToViewportPoint(hitZoneWorldPos);
        
        // Hit zone should be at the bottom of the screen (Y close to 0)
        bool isAtBottom = screenPos.y < 0.3f && screenPos.y > -0.1f;
        
        Debug.Log($"🎯 Hit zone screen position: {screenPos}, at bottom: {isAtBottom}");
        
        return isAtBottom;
    }
    
    void OnDrawGizmosSelected()
    {
        // Draw camera view frustum
        Gizmos.color = Color.yellow;
        
        // Draw line to hit zone
        Vector3 hitZonePos = new Vector3(0f, 0f, hitZoneZ);
        Gizmos.DrawLine(transform.position, hitZonePos);
        
        // Draw hit zone position
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(hitZonePos, new Vector3(8f, 0.5f, 1f));
        
        // Draw highway bounds
        Gizmos.color = Color.white;
        Vector3 highwayStart = new Vector3(0f, 0f, hitZoneZ);
        Vector3 highwayEnd = new Vector3(0f, 0f, hitZoneZ + highwayLength);
        Gizmos.DrawLine(highwayStart, highwayEnd);
    }
    
    void Update()
    {
        // Debug key to check hit zone position
        if (Input.GetKeyDown(KeyCode.C))
        {
            IsHitZoneAtBottomOfScreen();
        }
        
        // Debug key to optimize camera
        if (Input.GetKeyDown(KeyCode.O))
        {
            OptimizeForHighway();
        }
    }
}
