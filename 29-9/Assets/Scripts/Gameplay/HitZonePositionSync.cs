using UnityEngine;

/// <summary>
/// Sincroniza automáticamente las posiciones del hit zone en todos los componentes
/// para asegurar que los botones visuales coincidan con la zona de detección real
/// </summary>
public class HitZonePositionSync : MonoBehaviour
{
    [Header("Hit Zone Position")]
    [Range(-15f, -2f)]
    public float hitZoneZ = -8f; // Posición Z donde realmente funciona la detección
    
    [Header("Components to Sync")]
    public HighwaySetup highwaySetup;
    public HitZone hitZone;
    public NoteSpawner noteSpawner;
    
    [Header("Auto Sync")]
    public bool syncOnStart = true;
    public bool syncInRealTime = false;
    
    private float lastHitZoneZ;
    
    void Start()
    {
        FindComponents();
        
        if (syncOnStart)
        {
            SyncAllComponents();
        }
        
        lastHitZoneZ = hitZoneZ;
    }
    
    void Update()
    {
        if (syncInRealTime && Mathf.Abs(hitZoneZ - lastHitZoneZ) > 0.01f)
        {
            SyncAllComponents();
            lastHitZoneZ = hitZoneZ;
        }
    }
    
    void FindComponents()
    {
        if (highwaySetup == null)
            highwaySetup = FindFirstObjectByType<HighwaySetup>();
        
        if (hitZone == null)
            hitZone = FindFirstObjectByType<HitZone>();
        
        if (noteSpawner == null)
            noteSpawner = FindFirstObjectByType<NoteSpawner>();
    }
    
    [ContextMenu("Sync All Hit Zone Positions")]
    public void SyncAllComponents()
    {
        Debug.Log($"🎯 Sincronizando posiciones del hit zone a Z = {hitZoneZ}");
        
        // Sincronizar HighwaySetup
        if (highwaySetup != null)
        {
            highwaySetup.hitZoneZ = hitZoneZ;
            Debug.Log($"✅ HighwaySetup sincronizado");
            
            // Forzar recreación de indicadores si está en modo play
            if (Application.isPlaying)
            {
                RecreateHitZoneIndicators();
            }
        }
        
        // Sincronizar HitZone
        if (hitZone != null)
        {
            hitZone.hitZoneZ = hitZoneZ;
            Debug.Log($"✅ HitZone sincronizado");
        }
        
        // Sincronizar NoteSpawner
        if (noteSpawner != null)
        {
            noteSpawner.hitZoneZ = hitZoneZ;
            Debug.Log($"✅ NoteSpawner sincronizado");
        }
        
        // Sincronizar todas las notas existentes
        SyncExistingNotes();
        
        Debug.Log($"🎸 Sincronización completa - Los botones ahora deberían estar en la posición correcta");
    }
    
    void RecreateHitZoneIndicators()
    {
        if (highwaySetup == null) return;
        
        // Buscar y mover los indicadores existentes
        for (int i = 0; i < 5; i++)
        {
            GameObject indicator = GameObject.Find($"HitZone_Lane_{i}");
            if (indicator != null)
            {
                Vector3 newPos = indicator.transform.position;
                newPos.z = hitZoneZ;
                indicator.transform.position = newPos;
                
                Debug.Log($"🎯 Movido indicador Lane {i} a Z = {hitZoneZ}");
            }
        }
    }
    
    void SyncExistingNotes()
    {
        Note[] existingNotes = FindObjectsByType<Note>(FindObjectsSortMode.None);
        
        foreach (Note note in existingNotes)
        {
            if (note != null)
            {
                note.hitZoneZ = hitZoneZ;
                note.missThresholdZ = hitZoneZ - 2f;
                note.destroyThresholdZ = hitZoneZ - 7f;
            }
        }
        
        if (existingNotes.Length > 0)
        {
            Debug.Log($"🎵 Sincronizadas {existingNotes.Length} notas existentes");
        }
    }
    
    /// <summary>
    /// Encuentra la posición Z donde realmente funciona la detección
    /// analizando las notas que se han detectado exitosamente
    /// </summary>
    [ContextMenu("Auto Detect Hit Zone Position")]
    public void AutoDetectHitZonePosition()
    {
        Debug.Log("🔍 Intentando detectar automáticamente la posición del hit zone...");
        Debug.Log("💡 Toca algunas notas y observa dónde desaparecen para determinar la posición correcta");
        Debug.Log("📝 Luego ajusta manualmente el valor 'Hit Zone Z' en este componente");
        Debug.Log("🎯 Valor actual: " + hitZoneZ);
        
        // En el futuro, esto podría analizar estadísticas de hits para determinar la posición óptima
    }
    
    void OnDrawGizmosSelected()
    {
        // Dibujar la posición del hit zone en el editor
        Gizmos.color = Color.red;
        
        // Dibujar línea horizontal en la posición del hit zone
        Vector3 center = new Vector3(0f, 0f, hitZoneZ);
        Vector3 leftPoint = center + Vector3.left * 3f;
        Vector3 rightPoint = center + Vector3.right * 3f;
        
        Gizmos.DrawLine(leftPoint, rightPoint);
        
        // Dibujar indicadores para cada lane
        for (int i = 0; i < 5; i++)
        {
            float laneX = -1.6f + (i * 0.8f); // Basado en laneSpacing
            Vector3 lanePos = new Vector3(laneX, 0f, hitZoneZ);
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(lanePos, new Vector3(0.6f, 0.1f, 0.5f));
        }
        
        // Etiqueta
        Gizmos.color = Color.white;
        Vector3 labelPos = new Vector3(0f, 0.5f, hitZoneZ);
        Gizmos.DrawWireCube(labelPos, Vector3.one * 0.1f);
    }
}
