using UnityEngine;

/// <summary>
/// Corrige problemas con DontDestroyOnLoad asegurando que los GameObjects estén en el root
/// </summary>
public class DontDestroyOnLoadFixer : MonoBehaviour
{
    [Header("Auto Fix Settings")]
    public bool fixOnAwake = true;
    public bool enableDebugLogs = true;
    
    void Awake()
    {
        if (fixOnAwake)
        {
            FixDontDestroyOnLoad();
        }
    }
    
    /// <summary>
    /// Corrige el GameObject para que DontDestroyOnLoad funcione correctamente
    /// </summary>
    [ContextMenu("Fix DontDestroyOnLoad")]
    public void FixDontDestroyOnLoad()
    {
        // Verificar si el GameObject tiene un parent
        if (transform.parent != null)
        {
            if (enableDebugLogs)
            {
                Debug.Log($"🔧 Moviendo {gameObject.name} al root para DontDestroyOnLoad");
            }
            
            // Mover al root de la jerarquía
            transform.SetParent(null);
        }
        
        // Aplicar DontDestroyOnLoad
        DontDestroyOnLoad(gameObject);
        
        if (enableDebugLogs)
        {
            Debug.Log($"✅ DontDestroyOnLoad aplicado correctamente a {gameObject.name}");
        }
    }
    
    /// <summary>
    /// Busca y corrige todos los GameObjects con problemas de DontDestroyOnLoad
    /// </summary>
    [ContextMenu("Fix All DontDestroyOnLoad Issues")]
    public static void FixAllDontDestroyOnLoadIssues()
    {
        // Buscar todos los componentes que podrían usar DontDestroyOnLoad
        MonoBehaviour[] allComponents = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
        
        int fixedCount = 0;
        
        foreach (MonoBehaviour component in allComponents)
        {
            // Verificar si el componente es un singleton que podría usar DontDestroyOnLoad
            if (IsSingletonComponent(component))
            {
                if (component.transform.parent != null)
                {
                    Debug.Log($"🔧 Corrigiendo {component.gameObject.name} para DontDestroyOnLoad");
                    component.transform.SetParent(null);
                    fixedCount++;
                }
            }
        }
        
        Debug.Log($"✅ Corregidos {fixedCount} GameObjects para DontDestroyOnLoad");
    }
    
    /// <summary>
    /// Verifica si un componente es probablemente un singleton
    /// </summary>
    static bool IsSingletonComponent(MonoBehaviour component)
    {
        string typeName = component.GetType().Name.ToLower();
        
        // Nombres comunes de singletons
        return typeName.Contains("manager") || 
               typeName.Contains("controller") || 
               typeName.Contains("system") || 
               typeName.Contains("service") ||
               typeName.Contains("ranking") ||
               typeName.Contains("record") ||
               typeName.Contains("settings") ||
               typeName.Contains("audio") ||
               typeName.Contains("game");
    }
}
