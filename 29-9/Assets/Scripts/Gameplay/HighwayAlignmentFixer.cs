using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Script completo para solucionar problemas de alineación en el highway
/// - Elimina lane dividers
/// - Alinea botones de colores con carriles
/// - Oculta texto molesto en pantalla
/// - Corrige posicionamiento de notas
/// </summary>
public class HighwayAlignmentFixer : MonoBehaviour
{
    [Header("Alignment Settings")]
    public bool fixOnStart = true;
    public bool hideDebugText = true;
    public bool removeLaneDividers = true;
    public bool alignHitZones = true;
    
    [Header("Lane Configuration")]
    public float laneSpacing = 0.8f; // Debe coincidir con HighwaySetup
    public Vector3 laneStartPosition = new Vector3(-1.6f, 0f, -8f); // Posición del primer carril
    
    [Header("Debug")]
    public bool showDebugInfo = false;
    
    private HighwaySetup highwaySetup;
    private HitZone hitZone;
    private NoteSpawner noteSpawner;
    
    void Start()
    {
        if (fixOnStart)
        {
            Invoke("PerformAlignment", 0.2f); // Pequeño delay para asegurar inicialización
        }
    }
    
    [ContextMenu("Perform Alignment Fix")]
    public void PerformAlignment()
    {
        Debug.Log("🔧 Iniciando corrección de alineación del highway...");
        
        FindComponents();
        
        if (hideDebugText)
        {
            HideDebugText();
        }
        
        if (removeLaneDividers)
        {
            RemoveLaneDividers();
        }
        
        if (alignHitZones)
        {
            AlignHitZones();
        }
        
        AlignNoteSpawner();
        
        Debug.Log("✅ Corrección de alineación completada");
    }
    
    void FindComponents()
    {
        highwaySetup = FindFirstObjectByType<HighwaySetup>();
        hitZone = FindFirstObjectByType<HitZone>();
        noteSpawner = FindFirstObjectByType<NoteSpawner>();
        
        if (showDebugInfo)
        {
            // Componentes encontrados
        }
    }
    
    void HideDebugText()
    {
        // Buscar y ocultar texto que contenga "Streak", "Error", porcentajes, etc.
        TextMeshProUGUI[] allTexts = FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None);
        int hiddenCount = 0;
        
        foreach (TextMeshProUGUI text in allTexts)
        {
            string textContent = text.text.ToLower();
            
            // Ocultar texto de debug común
            if (textContent.Contains("streak") || 
                textContent.Contains("error") ||
                textContent.Contains("0,0%") ||
                textContent.Contains("debug") ||
                textContent.Contains("test") ||
                (textContent.Contains("%") && textContent.Length < 10))
            {
                text.gameObject.SetActive(false);
                hiddenCount++;
                Debug.Log($"🔇 Texto de debug oculto: {text.text}");
            }
        }
        
        // También buscar UI Text
        Text[] uiTexts = FindObjectsByType<Text>(FindObjectsSortMode.None);
        foreach (Text text in uiTexts)
        {
            string textContent = text.text.ToLower();
            
            if (textContent.Contains("streak") || 
                textContent.Contains("error") ||
                textContent.Contains("0,0%") ||
                textContent.Contains("debug") ||
                textContent.Contains("test"))
            {
                text.gameObject.SetActive(false);
                hiddenCount++;
                Debug.Log($"🔇 UI Text de debug oculto: {text.text}");
            }
        }
        
        Debug.Log($"🧹 {hiddenCount} textos de debug ocultados");
    }
    
    void RemoveLaneDividers()
    {
        // Buscar y eliminar lane dividers
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        int removedCount = 0;
        
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("LaneDivider") || obj.name.Contains("Divider"))
            {
                obj.SetActive(false);
                removedCount++;
                Debug.Log($"🗑️ Lane divider eliminado: {obj.name}");
            }
        }
        
        Debug.Log($"🧹 {removedCount} lane dividers eliminados");
    }
    
    void AlignHitZones()
    {
        if (hitZone == null || hitZone.lanePositions == null) return;
        
        // Calcular posiciones correctas de los carriles
        Vector3[] correctPositions = CalculateLanePositions();
        
        // Alinear HitZone lane positions
        for (int i = 0; i < hitZone.lanePositions.Length && i < correctPositions.Length; i++)
        {
            if (hitZone.lanePositions[i] != null)
            {
                hitZone.lanePositions[i].position = correctPositions[i];
                // Lane alineado
            }
        }
        
        // Alinear indicadores visuales de HitZone
        if (hitZone.laneIndicators != null)
        {
            for (int i = 0; i < hitZone.laneIndicators.Length && i < correctPositions.Length; i++)
            {
                if (hitZone.laneIndicators[i] != null)
                {
                    hitZone.laneIndicators[i].transform.position = correctPositions[i];
                    // Indicador alineado
                }
            }
        }
        
        // Actualizar visual feedback
        if (hitZone.GetComponent<HitZone>() != null)
        {
            hitZone.SendMessage("UpdateVisualFeedback", SendMessageOptions.DontRequireReceiver);
        }
    }
    
    void AlignNoteSpawner()
    {
        if (noteSpawner == null || noteSpawner.lanes == null) return;
        
        // Calcular posiciones correctas de los carriles
        Vector3[] correctPositions = CalculateLanePositions();
        
        // Alinear lanes del NoteSpawner
        for (int i = 0; i < noteSpawner.lanes.Length && i < correctPositions.Length; i++)
        {
            if (noteSpawner.lanes[i] != null)
            {
                // Ajustar solo X, mantener Y y Z del spawner
                Vector3 spawnerPos = noteSpawner.lanes[i].position;
                spawnerPos.x = correctPositions[i].x;
                noteSpawner.lanes[i].position = spawnerPos;
                
                // NoteSpawner alineado
            }
        }
    }
    
    Vector3[] CalculateLanePositions()
    {
        Vector3[] positions = new Vector3[5];
        
        // Calcular posiciones centradas basadas en laneSpacing
        float startX = -(laneSpacing * 2f); // Para 5 carriles centrados
        
        for (int i = 0; i < 5; i++)
        {
            float laneX = startX + (i * laneSpacing);
            positions[i] = new Vector3(laneX, laneStartPosition.y, laneStartPosition.z);
        }
        
        return positions;
    }
    
    [ContextMenu("Show Lane Positions")]
    public void ShowLanePositions()
    {
        Vector3[] positions = CalculateLanePositions();
        
        Debug.Log("📋 POSICIONES DE CARRILES CALCULADAS:");
        for (int i = 0; i < positions.Length; i++)
        {
            Debug.Log($"   Lane {i}: {positions[i]}");
        }
        
        // Mostrar posiciones actuales para comparación
        if (hitZone != null && hitZone.lanePositions != null)
        {
            Debug.Log("📋 POSICIONES ACTUALES DE HITZONES:");
            for (int i = 0; i < hitZone.lanePositions.Length; i++)
            {
                if (hitZone.lanePositions[i] != null)
                {
                    Debug.Log($"   Lane {i}: {hitZone.lanePositions[i].position}");
                }
            }
        }
    }
    
    [ContextMenu("Hide All Debug Text")]
    public void ForceHideDebugText()
    {
        HideDebugText();
    }
    
    [ContextMenu("Remove All Lane Dividers")]
    public void ForceRemoveLaneDividers()
    {
        RemoveLaneDividers();
    }
    
    void Update()
    {
        // Monitoreo continuo para ocultar texto de debug que aparezca
        if (hideDebugText && Time.frameCount % 120 == 0) // Cada 2 segundos aproximadamente
        {
            HideDebugText();
        }
    }
    
    void OnDrawGizmosSelected()
    {
        // Dibujar posiciones calculadas de carriles en el editor
        Vector3[] positions = CalculateLanePositions();
        
        Gizmos.color = Color.green;
        for (int i = 0; i < positions.Length; i++)
        {
            Gizmos.DrawWireCube(positions[i], new Vector3(0.4f, 0.1f, 0.4f));
            
            // Dibujar línea del carril
            Vector3 start = positions[i] + Vector3.forward * 10f;
            Vector3 end = positions[i] - Vector3.forward * 10f;
            Gizmos.DrawLine(start, end);
        }
    }
}
