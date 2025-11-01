using UnityEngine;

/// <summary>
/// Script para limpiar HitZones duplicados y KeyLabels no deseados
/// Ejecuta al inicio para evitar la creación de elementos duplicados
/// </summary>
public class HitZoneCleanup : MonoBehaviour
{
    [Header("Cleanup Settings")]
    public bool cleanupOnStart = true;
    public bool preventDuplicateHitZones = true;
    public bool removeAllKeyLabels = true;
    
    void Awake()
    {
        // Ejecutar en Awake para que sea lo primero que se ejecute
        if (cleanupOnStart)
        {
            PerformCleanup();
        }
    }
    
    void Start()
    {
        // Ejecutar también en Start por si algo se crea después
        if (cleanupOnStart)
        {
            Invoke("PerformCleanup", 0.1f); // Pequeño delay para asegurar que todo esté creado
        }
    }
    
    [ContextMenu("Perform Cleanup")]
    public void PerformCleanup()
    {
        Debug.Log("🧹 Iniciando limpieza de HitZones...");
        
        if (preventDuplicateHitZones)
        {
            PreventDuplicateHitZones();
        }
        
        if (removeAllKeyLabels)
        {
            RemoveAllKeyLabels();
        }
        
        CleanupDuplicateHitZones();
        
        Debug.Log("✅ Limpieza de HitZones completada");
    }
    
    void PreventDuplicateHitZones()
    {
        // Desactivar HitZoneIndicators para evitar que cree duplicados
        HitZoneIndicators[] hitZoneIndicators = FindObjectsByType<HitZoneIndicators>(FindObjectsSortMode.None);
        foreach (HitZoneIndicators indicator in hitZoneIndicators)
        {
            indicator.showKeyLabels = false;
            indicator.enabled = false;
            Debug.Log("🔇 HitZoneIndicators desactivado para prevenir duplicados");
        }
    }
    
    void RemoveAllKeyLabels()
    {
        // Buscar y destruir todos los KeyLabels
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        int removedCount = 0;
        
        foreach (GameObject obj in allObjects)
        {
            // Buscar por nombre
            if (obj.name.StartsWith("KeyLabel_") || obj.name.Contains("KeyLabel"))
            {
                DestroyImmediate(obj);
                removedCount++;
                Debug.Log($"🗑️ KeyLabel destruido: {obj.name}");
                continue;
            }
            
            // Buscar TextMesh con letras de teclas
            TextMesh textMesh = obj.GetComponent<TextMesh>();
            if (textMesh != null)
            {
                string text = textMesh.text.ToUpper().Trim();
                if (text == "D" || text == "F" || text == "J" || text == "K" || text == "L")
                {
                    DestroyImmediate(obj);
                    removedCount++;
                    Debug.Log($"🗑️ TextMesh con letra de tecla destruido: {text}");
                }
            }
        }
        
        Debug.Log($"🧹 {removedCount} KeyLabels removidos");
    }
    
    void CleanupDuplicateHitZones()
    {
        // Buscar HitZones duplicados
        GameObject[] hitZones = GameObject.FindGameObjectsWithTag("Untagged");
        System.Collections.Generic.Dictionary<string, GameObject> uniqueHitZones = 
            new System.Collections.Generic.Dictionary<string, GameObject>();
        
        int duplicatesRemoved = 0;
        
        foreach (GameObject obj in hitZones)
        {
            if (obj.name.StartsWith("HitZone_Lane_"))
            {
                string laneName = obj.name;
                
                if (uniqueHitZones.ContainsKey(laneName))
                {
                    // Es un duplicado, verificar cuál mantener
                    GameObject existing = uniqueHitZones[laneName];
                    GameObject toKeep = ChooseBetterHitZone(existing, obj);
                    GameObject toRemove = (toKeep == existing) ? obj : existing;
                    
                    // Actualizar el diccionario si cambiamos la elección
                    if (toKeep != existing)
                    {
                        uniqueHitZones[laneName] = toKeep;
                    }
                    
                    // Destruir el duplicado
                    DestroyImmediate(toRemove);
                    duplicatesRemoved++;
                    Debug.Log($"🗑️ HitZone duplicado destruido: {toRemove.name}");
                }
                else
                {
                    uniqueHitZones[laneName] = obj;
                }
            }
        }
        
        Debug.Log($"🧹 {duplicatesRemoved} HitZones duplicados removidos");
    }
    
    GameObject ChooseBetterHitZone(GameObject hitZone1, GameObject hitZone2)
    {
        // Priorizar el que NO tenga KeyLabels como hijos
        bool hasKeyLabels1 = HasKeyLabelsAsChildren(hitZone1);
        bool hasKeyLabels2 = HasKeyLabelsAsChildren(hitZone2);
        
        if (hasKeyLabels1 && !hasKeyLabels2)
        {
            return hitZone2; // Elegir el que no tiene KeyLabels
        }
        else if (!hasKeyLabels1 && hasKeyLabels2)
        {
            return hitZone1; // Elegir el que no tiene KeyLabels
        }
        
        // Si ambos tienen o no tienen KeyLabels, elegir el primero
        return hitZone1;
    }
    
    bool HasKeyLabelsAsChildren(GameObject parent)
    {
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            Transform child = parent.transform.GetChild(i);
            if (child.name.StartsWith("KeyLabel_") || child.name.Contains("KeyLabel"))
            {
                return true;
            }
        }
        return false;
    }
    
    [ContextMenu("Force Remove All KeyLabels")]
    public void ForceRemoveAllKeyLabels()
    {
        RemoveAllKeyLabels();
    }
    
    [ContextMenu("Show HitZone Hierarchy")]
    public void ShowHitZoneHierarchy()
    {
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        
        Debug.Log("📋 JERARQUÍA DE HITZONES:");
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.StartsWith("HitZone_Lane_"))
            {
                Debug.Log($"  🎯 {obj.name}");
                for (int i = 0; i < obj.transform.childCount; i++)
                {
                    Transform child = obj.transform.GetChild(i);
                    Debug.Log($"    └── {child.name}");
                }
            }
        }
    }
}
