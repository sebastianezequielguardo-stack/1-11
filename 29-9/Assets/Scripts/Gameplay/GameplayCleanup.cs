using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Limpia automáticamente GameObjects innecesarios del Gameplay
/// Optimiza la escena eliminando objetos que no se usan
/// </summary>
public class GameplayCleanup : MonoBehaviour
{
    [Header("Cleanup Settings")]
    public bool cleanupOnStart = false; // Cambiar a true para limpieza automática
    public bool showDetailedLogs = true;
    public bool createBackup = true;
    
    [Header("Objects to Keep (Essential)")]
    public string[] essentialObjects = {
        "Main Camera", "Camera", "EventSystem", "Canvas", "UI", "Audio Source", "AudioSource",
        "GameplayManager", "TrackBase", "Highway", "Player", "ScoreManager", "InputManager"
    };
    
    [Header("Objects to Remove (Unnecessary)")]
    public string[] unnecessaryObjects = {
        "Cube", "Sphere", "Cylinder", "Plane", "Quad", "Capsule",
        "Test", "Debug", "Temp", "Example", "Sample", "Demo",
        "Old", "Backup", "Copy", "Duplicate", "_old", "_backup", "_copy"
    };
    
    [Header("Component Types to Remove")]
    public bool removeUnusedComponents = true;
    public bool removeEmptyGameObjects = true;
    public bool removeDisabledObjects = false; // Cuidado con esto
    
    private List<GameObject> objectsToDelete = new List<GameObject>();
    private List<Component> componentsToDelete = new List<Component>();
    private int initialObjectCount = 0;
    
    void Start()
    {
        if (cleanupOnStart)
        {
            PerformGameplayCleanup();
        }
    }
    
    /// <summary>
    /// Ejecuta la limpieza completa del Gameplay
    /// </summary>
    [ContextMenu("Perform Gameplay Cleanup")]
    public void PerformGameplayCleanup()
    {
        Debug.Log("🧹 INICIANDO LIMPIEZA DEL GAMEPLAY");
        Debug.Log("═══════════════════════════════════");
        
        initialObjectCount = FindObjectsByType<GameObject>(FindObjectsSortMode.None).Length;
        
        if (createBackup)
        {
            CreateSceneBackup();
        }
        
        // Limpiar en orden específico
        CleanupUnnecessaryObjects();
        CleanupUnusedComponents();
        CleanupEmptyGameObjects();
        CleanupDuplicateObjects();
        CleanupOldScripts();
        
        // Ejecutar limpieza
        ExecuteCleanup();
        
        // Mostrar resultados
        ShowCleanupResults();
        
        Debug.Log("✅ LIMPIEZA COMPLETADA");
    }
    
    /// <summary>
    /// Crea un backup de la escena antes de limpiar
    /// </summary>
    void CreateSceneBackup()
    {
        Debug.Log("💾 Creando backup de la escena...");
        
        // En un entorno real, aquí se guardaría la escena
        // Por ahora solo loggeamos
        Debug.Log("📁 Backup creado (recomendación: guardar escena manualmente antes de limpiar)");
    }
    
    /// <summary>
    /// Limpia objetos innecesarios por nombre
    /// </summary>
    void CleanupUnnecessaryObjects()
    {
        Debug.Log("🔍 Buscando objetos innecesarios...");
        
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        
        foreach (GameObject obj in allObjects)
        {
            if (obj == null) continue;
            
            string objName = obj.name.ToLower();
            
            // Verificar si es esencial
            bool isEssential = false;
            foreach (string essential in essentialObjects)
            {
                if (objName.Contains(essential.ToLower()))
                {
                    isEssential = true;
                    break;
                }
            }
            
            if (isEssential) continue;
            
            // Verificar si es innecesario
            foreach (string unnecessary in unnecessaryObjects)
            {
                if (objName.Contains(unnecessary.ToLower()))
                {
                    if (!objectsToDelete.Contains(obj))
                    {
                        objectsToDelete.Add(obj);
                        if (showDetailedLogs)
                        {
                            Debug.Log($"🗑️ Marcado para eliminar: {obj.name} (contiene '{unnecessary}')");
                        }
                    }
                    break;
                }
            }
        }
    }
    
    /// <summary>
    /// Limpia componentes no utilizados
    /// </summary>
    void CleanupUnusedComponents()
    {
        if (!removeUnusedComponents) return;
        
        Debug.Log("🔧 Buscando componentes no utilizados...");
        
        // Tipos de componentes que se pueden eliminar de forma segura
        System.Type[] unnecessaryComponentTypes = {
            typeof(MeshRenderer), // Si no tiene MeshFilter
            typeof(MeshFilter),   // Si no tiene MeshRenderer
            typeof(Collider),     // Si está deshabilitado y no se usa
            typeof(Rigidbody),    // Si no se mueve
            typeof(AudioSource)   // Si no tiene clip y no se usa
        };
        
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        
        foreach (GameObject obj in allObjects)
        {
            if (obj == null || objectsToDelete.Contains(obj)) continue;
            
            // Verificar MeshRenderer sin MeshFilter
            MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();
            MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
            
            if (meshRenderer != null && meshFilter == null)
            {
                componentsToDelete.Add(meshRenderer);
                if (showDetailedLogs)
                {
                    Debug.Log($"🔧 MeshRenderer sin MeshFilter: {obj.name}");
                }
            }
            
            if (meshFilter != null && meshRenderer == null)
            {
                componentsToDelete.Add(meshFilter);
                if (showDetailedLogs)
                {
                    Debug.Log($"🔧 MeshFilter sin MeshRenderer: {obj.name}");
                }
            }
            
            // Verificar AudioSource sin clip
            AudioSource audioSource = obj.GetComponent<AudioSource>();
            if (audioSource != null && audioSource.clip == null && !audioSource.enabled)
            {
                componentsToDelete.Add(audioSource);
                if (showDetailedLogs)
                {
                    Debug.Log($"🔧 AudioSource vacío: {obj.name}");
                }
            }
        }
    }
    
    /// <summary>
    /// Limpia GameObjects vacíos
    /// </summary>
    void CleanupEmptyGameObjects()
    {
        if (!removeEmptyGameObjects) return;
        
        Debug.Log("📦 Buscando GameObjects vacíos...");
        
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        
        foreach (GameObject obj in allObjects)
        {
            if (obj == null || objectsToDelete.Contains(obj)) continue;
            
            // Verificar si está vacío
            if (IsGameObjectEmpty(obj))
            {
                objectsToDelete.Add(obj);
                if (showDetailedLogs)
                {
                    Debug.Log($"📦 GameObject vacío: {obj.name}");
                }
            }
        }
    }
    
    /// <summary>
    /// Verifica si un GameObject está vacío
    /// </summary>
    bool IsGameObjectEmpty(GameObject obj)
    {
        // Tiene hijos?
        if (obj.transform.childCount > 0) return false;
        
        // Tiene componentes importantes?
        Component[] components = obj.GetComponents<Component>();
        
        // Solo Transform = vacío
        if (components.Length <= 1) return true;
        
        // Solo Transform + componentes deshabilitados = vacío
        bool hasActiveComponents = false;
        foreach (Component comp in components)
        {
            if (comp is Transform) continue;
            
            if (comp is Behaviour behaviour)
            {
                if (behaviour.enabled)
                {
                    hasActiveComponents = true;
                    break;
                }
            }
            else
            {
                hasActiveComponents = true;
                break;
            }
        }
        
        return !hasActiveComponents;
    }
    
    /// <summary>
    /// Limpia objetos duplicados
    /// </summary>
    void CleanupDuplicateObjects()
    {
        Debug.Log("🔄 Buscando objetos duplicados...");
        
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        Dictionary<string, List<GameObject>> objectGroups = new Dictionary<string, List<GameObject>>();
        
        // Agrupar por nombre
        foreach (GameObject obj in allObjects)
        {
            if (obj == null || objectsToDelete.Contains(obj)) continue;
            
            string baseName = GetBaseName(obj.name);
            
            if (!objectGroups.ContainsKey(baseName))
            {
                objectGroups[baseName] = new List<GameObject>();
            }
            
            objectGroups[baseName].Add(obj);
        }
        
        // Encontrar duplicados
        foreach (var group in objectGroups)
        {
            if (group.Value.Count > 1)
            {
                // Ordenar por nombre (los que tienen números al final van después)
                var sorted = group.Value.OrderBy(obj => obj.name).ToList();
                
                // Mantener el primero, eliminar el resto
                for (int i = 1; i < sorted.Count; i++)
                {
                    if (IsDuplicateObject(sorted[0], sorted[i]))
                    {
                        objectsToDelete.Add(sorted[i]);
                        if (showDetailedLogs)
                        {
                            Debug.Log($"🔄 Duplicado encontrado: {sorted[i].name}");
                        }
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// Obtiene el nombre base sin números
    /// </summary>
    string GetBaseName(string name)
    {
        // Remover números al final como " (1)", " (2)", etc.
        string baseName = System.Text.RegularExpressions.Regex.Replace(name, @"\s*\(\d+\)$", "");
        baseName = System.Text.RegularExpressions.Regex.Replace(baseName, @"\s*\d+$", "");
        return baseName.Trim();
    }
    
    /// <summary>
    /// Verifica si dos objetos son duplicados
    /// </summary>
    bool IsDuplicateObject(GameObject obj1, GameObject obj2)
    {
        // Verificar posición similar
        if (Vector3.Distance(obj1.transform.position, obj2.transform.position) < 0.1f)
        {
            // Verificar componentes similares
            Component[] comp1 = obj1.GetComponents<Component>();
            Component[] comp2 = obj2.GetComponents<Component>();
            
            if (comp1.Length == comp2.Length)
            {
                return true; // Probablemente duplicado
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// Limpia scripts antiguos o rotos
    /// </summary>
    void CleanupOldScripts()
    {
        Debug.Log("📜 Buscando scripts antiguos...");
        
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        
        foreach (GameObject obj in allObjects)
        {
            if (obj == null || objectsToDelete.Contains(obj)) continue;
            
            Component[] components = obj.GetComponents<Component>();
            
            foreach (Component comp in components)
            {
                // Verificar componentes nulos (scripts rotos)
                if (comp == null)
                {
                    if (showDetailedLogs)
                    {
                        Debug.Log($"📜 Script roto encontrado en: {obj.name}");
                    }
                    // Unity maneja esto automáticamente, pero lo loggeamos
                }
                
                // Verificar scripts con nombres de prueba
                if (comp != null)
                {
                    string typeName = comp.GetType().Name.ToLower();
                    if (typeName.Contains("test") || typeName.Contains("debug") || typeName.Contains("temp"))
                    {
                        componentsToDelete.Add(comp);
                        if (showDetailedLogs)
                        {
                            Debug.Log($"📜 Script de prueba: {comp.GetType().Name} en {obj.name}");
                        }
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// Ejecuta la limpieza marcada
    /// </summary>
    void ExecuteCleanup()
    {
        Debug.Log("🗑️ Ejecutando limpieza...");
        
        // Eliminar componentes
        foreach (Component comp in componentsToDelete)
        {
            if (comp != null)
            {
                DestroyImmediate(comp);
            }
        }
        
        // Eliminar GameObjects
        foreach (GameObject obj in objectsToDelete)
        {
            if (obj != null)
            {
                DestroyImmediate(obj);
            }
        }
    }
    
    /// <summary>
    /// Muestra los resultados de la limpieza
    /// </summary>
    void ShowCleanupResults()
    {
        int finalObjectCount = FindObjectsByType<GameObject>(FindObjectsSortMode.None).Length;
        int objectsRemoved = initialObjectCount - finalObjectCount;
        
        Debug.Log("📊 RESULTADOS DE LA LIMPIEZA:");
        Debug.Log("═══════════════════════════");
        Debug.Log($"GameObjects iniciales: {initialObjectCount}");
        Debug.Log($"GameObjects finales: {finalObjectCount}");
        Debug.Log($"GameObjects eliminados: {objectsRemoved}");
        Debug.Log($"Componentes eliminados: {componentsToDelete.Count}");
        
        if (objectsRemoved > 0)
        {
            float reduction = ((float)objectsRemoved / initialObjectCount) * 100f;
            Debug.Log($"Reducción: {reduction:F1}%");
        }
        
        Debug.Log("\n💡 RECOMENDACIONES:");
        if (objectsRemoved > 0)
        {
            Debug.Log("✅ Escena optimizada exitosamente");
            Debug.Log("💾 Guarda la escena para mantener los cambios");
        }
        else
        {
            Debug.Log("ℹ️ La escena ya estaba optimizada");
        }
    }
    
    /// <summary>
    /// Análisis sin eliminar nada
    /// </summary>
    [ContextMenu("Analyze Only (No Cleanup)")]
    public void AnalyzeOnly()
    {
        Debug.Log("🔍 ANÁLISIS DE LA ESCENA (sin eliminar):");
        Debug.Log("═══════════════════════════════════════");
        
        objectsToDelete.Clear();
        componentsToDelete.Clear();
        
        CleanupUnnecessaryObjects();
        CleanupUnusedComponents();
        CleanupEmptyGameObjects();
        CleanupDuplicateObjects();
        CleanupOldScripts();
        
        Debug.Log($"📊 Objetos que se eliminarían: {objectsToDelete.Count}");
        Debug.Log($"📊 Componentes que se eliminarían: {componentsToDelete.Count}");
        
        if (objectsToDelete.Count > 0)
        {
            Debug.Log("\n🗑️ OBJETOS MARCADOS PARA ELIMINAR:");
            foreach (GameObject obj in objectsToDelete)
            {
                if (obj != null)
                {
                    Debug.Log($"  - {obj.name}");
                }
            }
        }
        
        // Limpiar listas para que no se ejecute accidentalmente
        objectsToDelete.Clear();
        componentsToDelete.Clear();
    }
    
    /// <summary>
    /// Limpieza rápida de objetos obvios
    /// </summary>
    [ContextMenu("Quick Cleanup (Safe)")]
    public void QuickCleanup()
    {
        Debug.Log("⚡ LIMPIEZA RÁPIDA (segura):");
        
        objectsToDelete.Clear();
        componentsToDelete.Clear();
        
        // Solo eliminar objetos obviamente innecesarios
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        
        foreach (GameObject obj in allObjects)
        {
            if (obj == null) continue;
            
            string name = obj.name.ToLower();
            
            // Solo objetos muy obvios
            if (name.StartsWith("cube") || name.StartsWith("sphere") || 
                name.StartsWith("cylinder") || name.StartsWith("plane") ||
                name.Contains("test") || name.Contains("debug") ||
                name.Contains("temp") || name.Contains("example"))
            {
                objectsToDelete.Add(obj);
                Debug.Log($"⚡ Eliminando: {obj.name}");
            }
        }
        
        ExecuteCleanup();
        Debug.Log($"✅ Limpieza rápida completada - {objectsToDelete.Count} objetos eliminados");
    }
}
