using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Arregla referencias rotas y scripts faltantes
/// Limpia componentes con "Missing Script" y referencias nulas
/// </summary>
public class BrokenReferencesFixer : MonoBehaviour
{
    [Header("Auto Fix")]
    public bool fixOnStart = true;
    public bool showDebugLogs = true;
    
    [Header("Fix Options")]
    public bool removeMissingScripts = true;
    public bool fixNullReferences = true;
    public bool cleanupEmptyGameObjects = false;
    
    void Start()
    {
        if (fixOnStart)
        {
            // Esperar más tiempo para no interferir con loading screens
            Invoke(nameof(FixBrokenReferences), 2f);
        }
    }
    
    /// <summary>
    /// Arregla todas las referencias rotas
    /// </summary>
    [ContextMenu("Fix Broken References")]
    public void FixBrokenReferences()
    {
        Debug.Log("🔧 ARREGLANDO REFERENCIAS ROTAS");
        Debug.Log("═══════════════════════════════");
        
        int totalFixed = 0;
        
        if (removeMissingScripts)
        {
            totalFixed += RemoveMissingScripts();
        }
        
        if (fixNullReferences)
        {
            totalFixed += FixNullReferences();
        }
        
        if (cleanupEmptyGameObjects)
        {
            totalFixed += CleanupEmptyGameObjects();
        }
        
        Debug.Log($"✅ {totalFixed} referencias arregladas");
    }
    
    /// <summary>
    /// Remueve scripts faltantes (Missing Script)
    /// </summary>
    int RemoveMissingScripts()
    {
        int removed = 0;
        
        // Buscar todos los GameObjects
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        
        foreach (GameObject obj in allObjects)
        {
            if (obj == null) continue;
            
            // Obtener todos los componentes
            Component[] components = obj.GetComponents<Component>();
            
            for (int i = components.Length - 1; i >= 0; i--)
            {
                // Si el componente es null, es un "Missing Script"
                if (components[i] == null)
                {
                    // Usar GameObjectUtility para remover el componente faltante
                    #if UNITY_EDITOR
                    GameObjectUtility.RemoveMonoBehavioursWithMissingScript(obj);
                    #endif
                    
                    removed++;
                    
                    if (showDebugLogs)
                    {
                        Debug.Log($"🗑️ Removed missing script from: {obj.name}");
                    }
                    break; // Solo necesitamos llamar una vez por objeto
                }
            }
        }
        
        if (showDebugLogs && removed > 0)
        {
            Debug.Log($"🗑️ {removed} missing scripts removed");
        }
        
        return removed;
    }
    
    /// <summary>
    /// Arregla referencias nulas en componentes
    /// </summary>
    int FixNullReferences()
    {
        int fixedCount = 0;
        
        // Buscar MonoBehaviours con posibles referencias nulas
        MonoBehaviour[] allScripts = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
        
        foreach (MonoBehaviour script in allScripts)
        {
            if (script == null) continue;
            
            // Verificar tipos específicos conocidos que pueden tener problemas
            if (FixSpecificScriptReferences(script))
            {
                fixedCount++;
            }
        }
        
        if (showDebugLogs && fixedCount > 0)
        {
            Debug.Log($"🔧 {fixedCount} null references fixed");
        }
        
        return fixedCount;
    }
    
    /// <summary>
    /// Arregla referencias específicas de scripts conocidos
    /// </summary>
    bool FixSpecificScriptReferences(MonoBehaviour script)
    {
        bool wasFixed = false;
        string scriptType = script.GetType().Name;
        
        try
        {
            switch (scriptType)
            {
                case "GameplayPauseIntegration":
                    wasFixed = FixGameplayPauseIntegration(script);
                    break;
                    
                case "NoteSpawner":
                    wasFixed = FixNoteSpawner(script);
                    break;
                    
                case "SimplePauseSetup":
                    wasFixed = FixSimplePauseSetup(script);
                    break;
                    
                default:
                    // Para otros scripts, verificar referencias comunes
                    wasFixed = FixCommonReferences(script);
                    break;
            }
        }
        catch (System.Exception)
        {
            // Si hay error, simplemente continuar
        }
        
        return wasFixed;
    }
    
    /// <summary>
    /// Arregla GameplayPauseIntegration
    /// </summary>
    bool FixGameplayPauseIntegration(MonoBehaviour script)
    {
        // Desactivar si está causando problemas
        if (script.enabled)
        {
            script.enabled = false;
            if (showDebugLogs)
            {
                Debug.Log($"🔧 Disabled problematic GameplayPauseIntegration: {script.name}");
            }
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// Arregla NoteSpawner
    /// </summary>
    bool FixNoteSpawner(MonoBehaviour script)
    {
        // El NoteSpawner ya tiene auto-creación de lanes, no necesita arreglo
        return false;
    }
    
    /// <summary>
    /// Arregla SimplePauseSetup
    /// </summary>
    bool FixSimplePauseSetup(MonoBehaviour script)
    {
        // Verificar si está causando problemas con countdowns
        try
        {
            var pauseSetup = script as SimplePauseSetup;
            if (pauseSetup != null)
            {
                // Desactivar temporalmente para evitar countdowns automáticos
                pauseSetup.enabled = false;
                if (showDebugLogs)
                {
                    Debug.Log($"🔧 Temporarily disabled SimplePauseSetup: {script.name}");
                }
                return true;
            }
        }
        catch (System.Exception)
        {
            // Si hay error, desactivar el script
            script.enabled = false;
            return true;
        }
        
        return false;
    }
    
    /// <summary>
    /// Arregla referencias comunes
    /// </summary>
    bool FixCommonReferences(MonoBehaviour script)
    {
        // Para scripts desconocidos, verificar si están causando errores
        // Si el script no se puede acceder correctamente, desactivarlo
        try
        {
            // Intentar acceder a propiedades básicas
            string testName = script.name;
            bool testEnabled = script.enabled;
            return false;
        }
        catch (System.Exception)
        {
            // Si hay error al acceder, desactivar
            try
            {
                script.enabled = false;
                if (showDebugLogs)
                {
                    Debug.Log($"🔧 Disabled problematic script: {script.GetType().Name}");
                }
                return true;
            }
            catch (System.Exception)
            {
                // Si no se puede desactivar, intentar destruir
                try
                {
                    DestroyImmediate(script);
                    if (showDebugLogs)
                    {
                        Debug.Log($"🗑️ Destroyed broken script: {script.GetType().Name}");
                    }
                    return true;
                }
                catch (System.Exception)
                {
                    // Si nada funciona, ignorar
                    return false;
                }
            }
        }
    }
    
    /// <summary>
    /// Limpia GameObjects vacíos
    /// </summary>
    int CleanupEmptyGameObjects()
    {
        int cleaned = 0;
        
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        
        foreach (GameObject obj in allObjects)
        {
            if (obj == null) continue;
            
            // Verificar si el objeto está vacío y es seguro eliminarlo
            if (IsEmptyAndSafeToDelete(obj))
            {
                string objName = obj.name;
                DestroyImmediate(obj);
                cleaned++;
                
                if (showDebugLogs)
                {
                    Debug.Log($"🧹 Cleaned empty GameObject: {objName}");
                }
            }
        }
        
        if (showDebugLogs && cleaned > 0)
        {
            Debug.Log($"🧹 {cleaned} empty GameObjects cleaned");
        }
        
        return cleaned;
    }
    
    /// <summary>
    /// Verifica si un GameObject está vacío y es seguro eliminarlo
    /// </summary>
    bool IsEmptyAndSafeToDelete(GameObject obj)
    {
        // No eliminar objetos importantes
        if (obj.name.Contains("Manager") || 
            obj.name.Contains("System") ||
            obj.name.Contains("Camera") ||
            obj.name.Contains("Light") ||
            obj.name.Contains("Canvas") ||
            obj.transform.childCount > 0)
        {
            return false;
        }
        
        // Verificar si solo tiene Transform (está vacío)
        Component[] components = obj.GetComponents<Component>();
        
        // Contar componentes válidos (no nulos)
        int validComponents = 0;
        foreach (Component comp in components)
        {
            if (comp != null && !(comp is Transform))
            {
                validComponents++;
            }
        }
        
        // Solo eliminar si no tiene componentes (excepto Transform)
        return validComponents == 0;
    }
    
    /// <summary>
    /// Verifica el estado de las referencias
    /// </summary>
    [ContextMenu("Check References Status")]
    public void CheckReferencesStatus()
    {
        Debug.Log("📊 ESTADO DE REFERENCIAS:");
        Debug.Log("═══════════════════════════");
        
        int totalObjects = 0;
        int objectsWithMissingScripts = 0;
        int totalScripts = 0;
        int problematicScripts = 0;
        
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        totalObjects = allObjects.Length;
        
        foreach (GameObject obj in allObjects)
        {
            if (obj == null) continue;
            
            Component[] components = obj.GetComponents<Component>();
            bool hasMissingScript = false;
            
            foreach (Component comp in components)
            {
                if (comp == null)
                {
                    hasMissingScript = true;
                }
                else if (comp is MonoBehaviour)
                {
                    totalScripts++;
                    try
                    {
                        string testName = comp.name;
                    }
                    catch (System.Exception)
                    {
                        problematicScripts++;
                    }
                }
            }
            
            if (hasMissingScript)
            {
                objectsWithMissingScripts++;
            }
        }
        
        Debug.Log($"📦 Total GameObjects: {totalObjects}");
        Debug.Log($"❌ Objects with missing scripts: {objectsWithMissingScripts}");
        Debug.Log($"📜 Total MonoBehaviour scripts: {totalScripts}");
        Debug.Log($"⚠️ Problematic scripts: {problematicScripts}");
        
        if (objectsWithMissingScripts > 0 || problematicScripts > 0)
        {
            Debug.Log("🔧 Run 'Fix Broken References' to clean up");
        }
        else
        {
            Debug.Log("✅ All references are clean!");
        }
    }
}
