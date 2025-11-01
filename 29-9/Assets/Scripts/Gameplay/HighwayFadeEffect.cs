using UnityEngine;

/// <summary>
/// Creates a fade effect at the top of the highway to simulate notes coming from distance
/// </summary>
public class HighwayFadeEffect : MonoBehaviour
{
    [Header("Fade Settings")]
    public float fadeDistance = 10f; // Distance over which fade occurs
    public float fadeStartZ = 15f; // Z position where fade starts
    public AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    
    [Header("Fog Settings")]
    public bool useFog = true;
    public Color fogColor = new Color(0.5f, 0.7f, 1f, 1f);
    public float fogDensity = 0.02f;
    
    [Header("Fade Plane")]
    public GameObject fadePlane;
    public Material fadeMaterial;
    
    void Start()
    {
        SetupFadeEffect();
        SetupFog();
    }
    
    void SetupFadeEffect()
    {
        if (fadePlane == null)
        {
            CreateFadePlane();
        }
        
        if (fadeMaterial != null && fadePlane != null)
        {
            Renderer renderer = fadePlane.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = fadeMaterial;
            }
        }
    }
    
    void CreateFadePlane()
    {
        // Create a plane at the top of the highway for fade effect
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.name = "HighwayFadePlane";
        plane.transform.parent = transform;
        
        // Position at the top of the highway
        plane.transform.position = new Vector3(0, 1f, fadeStartZ);
        plane.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        plane.transform.localScale = new Vector3(2f, 1f, fadeDistance / 10f);
        
        // Remove collider
        Collider collider = plane.GetComponent<Collider>();
        if (collider != null)
        {
            DestroyImmediate(collider);
        }
        
        // Create fade material if none exists
        if (fadeMaterial == null)
        {
            CreateFadeMaterial(plane);
        }
        
        fadePlane = plane;
    }
    
    void CreateFadeMaterial(GameObject plane)
    {
        // Create a material with transparency for fade effect
        Material material = new Material(Shader.Find("Standard"));
        material.SetFloat("_Mode", 3); // Transparent mode
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = 3000;
        
        // Set gradient colors (transparent to opaque)
        Color fadeColor = fogColor;
        fadeColor.a = 0.8f;
        material.color = fadeColor;
        
        Renderer renderer = plane.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = material;
        }
        
        fadeMaterial = material;
    }
    
    void SetupFog()
    {
        if (useFog)
        {
            RenderSettings.fog = true;
            RenderSettings.fogColor = fogColor;
            RenderSettings.fogMode = FogMode.Exponential;
            RenderSettings.fogDensity = fogDensity;
        }
    }
    
    /// <summary>
    /// Apply fade effect to a note based on its Z position
    /// </summary>
    public float GetNoteFadeAlpha(float noteZ)
    {
        if (noteZ >= fadeStartZ)
        {
            // Note is in fade zone
            float fadeProgress = (noteZ - fadeStartZ) / fadeDistance;
            fadeProgress = Mathf.Clamp01(fadeProgress);
            
            // Use curve for smooth fade
            return 1f - fadeCurve.Evaluate(fadeProgress);
        }
        
        return 1f; // Full opacity
    }
    
    /// <summary>
    /// Apply fade effect to a note renderer
    /// </summary>
    public void ApplyFadeToNote(Renderer noteRenderer, float noteZ)
    {
        if (noteRenderer == null) return;
        
        float alpha = GetNoteFadeAlpha(noteZ);
        
        Material material = noteRenderer.material;
        Color color = material.color;
        color.a = alpha;
        material.color = color;
        
        // Enable transparency if needed
        if (alpha < 1f && material.renderQueue < 3000)
        {
            material.SetFloat("_Mode", 3); // Transparent mode
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.DisableKeyword("_ALPHATEST_ON");
            material.EnableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = 3000;
        }
    }
    
    void OnDrawGizmosSelected()
    {
        // Draw fade zone in editor
        Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
        Vector3 center = new Vector3(0, 0, fadeStartZ + fadeDistance / 2f);
        Vector3 size = new Vector3(10f, 2f, fadeDistance);
        Gizmos.DrawCube(center, size);
        
        // Draw fade start line
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(-5f, -1f, fadeStartZ), new Vector3(5f, 1f, fadeStartZ));
    }
}
