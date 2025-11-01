using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Arregla el inicio del gameplay eliminando pantallas de carga no deseadas
/// Desactiva contadores y pantallas negras que aparecen al iniciar
/// </summary>
public class GameplayStartFixer : MonoBehaviour
{
    [Header("Auto Fix")]
    public bool fixOnStart = true;
    public bool removeLoadingScreens = false; // Cambiado a false para mantener pantalla de carga
    public bool removeCountdowns = false; // Cambiado a false para mantener countdown legítimo
    public bool ensureNormalTimeScale = true;
    public bool showDebugLogs = true;
    
    [Header("Selective Removal")]
    public bool onlyRemoveUnwantedScreens = true;
    public bool preserveGameStartCountdown = true;
    
    [Header("Detection")]
    public bool findByName = true;
    public bool findByColor = true;
    public bool findByText = true;
    
    void Start()
    {
        if (fixOnStart)
        {
            // Solo ejecutar si no está configurado para preservar todo
            if (onlyRemoveUnwantedScreens)
            {
                // Ejecutar de forma más conservadora
                Invoke(nameof(FixGameplayStartConservative), 0.5f);
            }
            else
            {
                // Ejecutar inmediatamente
                FixGameplayStart();
                
                // También ejecutar después de un frame para asegurar
                Invoke(nameof(FixGameplayStartDelayed), 0.1f);
            }
        }
    }
    
    /// <summary>
    /// Versión conservadora que solo arregla problemas críticos
    /// </summary>
    void FixGameplayStartConservative()
    {
        Debug.Log("🛡️ Ejecutando limpieza conservadora...");
        
        // Solo asegurar time scale normal
        if (ensureNormalTimeScale)
        {
            Time.timeScale = 1f;
        }
        
        // Solo limpiar objetos claramente problemáticos
        CleanupOnlyProblematicObjects();
        
        Debug.Log("✅ Limpieza conservadora completada");
    }
    
    /// <summary>
    /// Limpia solo objetos claramente problemáticos
    /// </summary>
    void CleanupOnlyProblematicObjects()
    {
        int removed = 0;
        
        // Solo buscar objetos con nombres claramente problemáticos
        string[] problematicNames = {
            "ErrorCanvas", "BrokenScreen", "EmptyTransition", 
            "NullReference", "MissingScript", "DestroyedObject"
        };
        
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (GameObject obj in allObjects)
        {
            if (obj != null)
            {
                string objName = obj.name.ToLower();
                foreach (string problematicName in problematicNames)
                {
                    if (objName.Contains(problematicName.ToLower()))
                    {
                        string name = obj.name;
                        DestroyImmediate(obj);
                        removed++;
                        if (showDebugLogs)
                        {
                            Debug.Log($"🗑️ Eliminado objeto problemático: {name}");
                        }
                        break;
                    }
                }
            }
        }
        
        if (showDebugLogs)
        {
            Debug.Log($"🧹 {removed} objetos problemáticos eliminados");
        }
    }
    
    /// <summary>
    /// Arregla el inicio del gameplay
    /// </summary>
    [ContextMenu("Fix Gameplay Start")]
    public void FixGameplayStart()
    {
        Debug.Log("🚀 ARREGLANDO INICIO DEL GAMEPLAY");
        Debug.Log("═══════════════════════════════════");
        
        // Asegurar time scale normal
        if (ensureNormalTimeScale)
        {
            Time.timeScale = 1f;
            Debug.Log("⏱️ Time scale restaurado a 1.0");
        }
        
        // Remover pantallas de carga
        if (removeLoadingScreens)
        {
            RemoveLoadingScreens();
        }
        
        // Remover contadores
        if (removeCountdowns)
        {
            RemoveCountdowns();
        }
        
        // Limpiar objetos de transición
        CleanupTransitionObjects();
        
        Debug.Log("✅ Inicio del gameplay arreglado");
    }
    
    /// <summary>
    /// Versión retrasada para asegurar limpieza
    /// </summary>
    void FixGameplayStartDelayed()
    {
        RemoveLoadingScreens();
        RemoveCountdowns();
        CleanupTransitionObjects();
        
        if (showDebugLogs)
        {
            Debug.Log("🔄 Limpieza retrasada completada");
        }
    }
    
    /// <summary>
    /// Remueve pantallas de carga (solo las no deseadas)
    /// </summary>
    void RemoveLoadingScreens()
    {
        if (!onlyRemoveUnwantedScreens)
        {
            return; // No remover nada si está configurado para preservar
        }
        
        int removed = 0;
        
        // Buscar por nombres comunes - solo los problemáticos
        if (findByName)
        {
            string[] unwantedNames = {
                "ErrorScreen", "EmptyTransition", "BrokenCanvas"
            };
            
            foreach (string name in unwantedNames)
            {
                GameObject[] objects = GameObject.FindGameObjectsWithTag("Untagged");
                foreach (GameObject obj in objects)
                {
                    if (obj != null && obj.name.Contains(name))
                    {
                        DestroyImmediate(obj);
                        removed++;
                        if (showDebugLogs)
                        {
                            Debug.Log($"🗑️ Eliminado pantalla problemática: {obj.name}");
                        }
                    }
                }
            }
        }
        
        // Buscar por componentes de imagen negra - solo las problemáticas
        if (findByColor)
        {
            Image[] allImages = FindObjectsByType<Image>(FindObjectsSortMode.None);
            foreach (Image img in allImages)
            {
                if (img != null && img.color == Color.black && IsFullScreenImage(img))
                {
                    Canvas parentCanvas = img.GetComponentInParent<Canvas>();
                    if (parentCanvas != null && IsUnwantedBlackScreen(parentCanvas))
                    {
                        string canvasName = parentCanvas.name;
                        DestroyImmediate(parentCanvas.gameObject);
                        removed++;
                        if (showDebugLogs)
                        {
                            Debug.Log($"🖤 Eliminada pantalla negra problemática: {canvasName}");
                        }
                    }
                }
            }
        }
        
        if (showDebugLogs && removed > 0)
        {
            Debug.Log($"🗑️ {removed} pantallas de carga eliminadas");
        }
    }
    
    /// <summary>
    /// Remueve contadores
    /// </summary>
    void RemoveCountdowns()
    {
        int removed = 0;
        
        if (findByText)
        {
            // Buscar textos de countdown
            TextMeshProUGUI[] allTexts = FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None);
            foreach (TextMeshProUGUI text in allTexts)
            {
                if (IsCountdownText(text))
                {
                    // Eliminar el canvas padre si existe
                    Canvas parentCanvas = text.GetComponentInParent<Canvas>();
                    if (parentCanvas != null)
                    {
                        DestroyImmediate(parentCanvas.gameObject);
                        removed++;
                        if (showDebugLogs)
                        {
                            Debug.Log($"🔢 Eliminado contador: {parentCanvas.name}");
                        }
                    }
                    else
                    {
                        DestroyImmediate(text.gameObject);
                        removed++;
                        if (showDebugLogs)
                        {
                            Debug.Log($"🔢 Eliminado texto contador: {text.name}");
                        }
                    }
                }
            }
            
            // También buscar Text legacy
            Text[] legacyTexts = FindObjectsByType<Text>(FindObjectsSortMode.None);
            foreach (Text text in legacyTexts)
            {
                if (IsCountdownTextLegacy(text))
                {
                    Canvas parentCanvas = text.GetComponentInParent<Canvas>();
                    if (parentCanvas != null)
                    {
                        DestroyImmediate(parentCanvas.gameObject);
                        removed++;
                    }
                    else
                    {
                        DestroyImmediate(text.gameObject);
                        removed++;
                    }
                }
            }
        }
        
        if (showDebugLogs && removed > 0)
        {
            Debug.Log($"🔢 {removed} contadores eliminados");
        }
    }
    
    /// <summary>
    /// Limpia objetos de transición
    /// </summary>
    void CleanupTransitionObjects()
    {
        int removed = 0;
        
        // Buscar objetos con nombres sospechosos - con verificación de null
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (GameObject obj in allObjects)
        {
            if (obj != null && IsTransitionObject(obj))
            {
                string objName = obj.name; // Guardar nombre antes de destruir
                DestroyImmediate(obj);
                removed++;
                if (showDebugLogs)
                {
                    Debug.Log($"🧹 Limpiado objeto de transición: {objName}");
                }
            }
        }
        
        // Buscar Canvas con sorting order muy alto (probablemente overlays)
        Canvas[] allCanvas = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        foreach (Canvas canvas in allCanvas)
        {
            if (canvas != null && canvas.gameObject != null && canvas.sortingOrder > 1000 && IsLikelyTransitionCanvas(canvas))
            {
                string canvasName = canvas.name; // Guardar nombre antes de destruir
                DestroyImmediate(canvas.gameObject);
                removed++;
                if (showDebugLogs)
                {
                    Debug.Log($"🎨 Eliminado canvas de transición: {canvasName}");
                }
            }
        }
        
        if (showDebugLogs && removed > 0)
        {
            Debug.Log($"🧹 {removed} objetos de transición limpiados");
        }
    }
    
    /// <summary>
    /// Verifica si una imagen es de pantalla completa
    /// </summary>
    bool IsFullScreenImage(Image img)
    {
        RectTransform rect = img.GetComponent<RectTransform>();
        if (rect == null) return false;
        
        // Verificar si cubre toda la pantalla
        return rect.anchorMin == Vector2.zero && rect.anchorMax == Vector2.one;
    }
    
    /// <summary>
    /// Verifica si es texto de countdown (solo elimina si NO es countdown de inicio de juego)
    /// </summary>
    bool IsCountdownText(TextMeshProUGUI text)
    {
        if (text == null || string.IsNullOrEmpty(text.text)) return false;
        
        // Si queremos preservar el countdown de inicio, no eliminar ningún countdown
        if (preserveGameStartCountdown)
        {
            return false;
        }
        
        string textContent = text.text.Trim();
        
        // Verificar si es un número del 1 al 5 (típico countdown)
        if (int.TryParse(textContent, out int number))
        {
            if (number >= 1 && number <= 5)
            {
                // Solo eliminar si es claramente un countdown no deseado
                return IsUnwantedCountdown(text);
            }
        }
        
        // Verificar palabras de countdown problemáticas
        string lowerText = textContent.ToLower();
        return lowerText.Contains("countdown") && lowerText.Contains("error");
    }
    
    /// <summary>
    /// Verifica si es un countdown no deseado (no el de inicio de juego)
    /// </summary>
    bool IsUnwantedCountdown(TextMeshProUGUI text)
    {
        // Verificar el contexto del countdown
        Canvas parentCanvas = text.GetComponentInParent<Canvas>();
        if (parentCanvas == null) return false;
        
        string canvasName = parentCanvas.name.ToLower();
        
        // No eliminar si es parte del sistema de carga del juego
        if (canvasName.Contains("loading") || 
            canvasName.Contains("gamestart") ||
            canvasName.Contains("initialization") ||
            canvasName.Contains("startup"))
        {
            return false; // Mantener estos countdowns
        }
        
        // Eliminar si es de sistemas de pausa problemáticos
        if (canvasName.Contains("pause") && canvasName.Contains("resume"))
        {
            return true; // Eliminar countdowns de resume de pausa
        }
        
        return false; // Por defecto, mantener
    }
    
    /// <summary>
    /// Verifica si es una pantalla negra no deseada
    /// </summary>
    bool IsUnwantedBlackScreen(Canvas canvas)
    {
        if (canvas == null) return false;
        
        string canvasName = canvas.name.ToLower();
        
        // NO eliminar pantallas de carga legítimas
        if (canvasName.Contains("loading") ||
            canvasName.Contains("gamestart") ||
            canvasName.Contains("initialization") ||
            canvasName.Contains("startup") ||
            canvasName.Contains("background"))
        {
            return false; // Mantener estas pantallas
        }
        
        // Eliminar solo pantallas problemáticas
        if (canvasName.Contains("error") ||
            canvasName.Contains("broken") ||
            canvasName.Contains("empty") ||
            (canvasName.Contains("pause") && canvasName.Contains("resume")))
        {
            return true; // Eliminar estas pantallas
        }
        
        // Si tiene sorting order muy alto y no tiene propósito claro, podría ser problemática
        if (canvas.sortingOrder > 9000 && canvas.transform.childCount <= 1)
        {
            return true;
        }
        
        return false; // Por defecto, mantener
    }
    
    /// <summary>
    /// Verifica si es texto de countdown (Text legacy)
    /// </summary>
    bool IsCountdownTextLegacy(Text text)
    {
        if (text == null || string.IsNullOrEmpty(text.text)) return false;
        
        string textContent = text.text.Trim();
        
        if (int.TryParse(textContent, out int number))
        {
            if (number >= 1 && number <= 5)
            {
                return true;
            }
        }
        
        string lowerText = textContent.ToLower();
        return lowerText.Contains("countdown") || 
               lowerText.Contains("starting") || 
               lowerText.Contains("iniciando");
    }
    
    /// <summary>
    /// Verifica si es objeto de transición
    /// </summary>
    bool IsTransitionObject(GameObject obj)
    {
        if (obj == null) return false;
        
        string name = obj.name.ToLower();
        
        return name.Contains("transition") ||
               name.Contains("loading") ||
               name.Contains("countdown") ||
               name.Contains("fade") ||
               name.Contains("black") ||
               name.Contains("overlay") ||
               (name.Contains("canvas") && name.Contains("temp"));
    }
    
    /// <summary>
    /// Verifica si es canvas de transición
    /// </summary>
    bool IsLikelyTransitionCanvas(Canvas canvas)
    {
        if (canvas == null) return false;
        
        string name = canvas.name.ToLower();
        
        // Verificar nombre
        bool suspiciousName = name.Contains("countdown") ||
                             name.Contains("transition") ||
                             name.Contains("loading") ||
                             name.Contains("fade") ||
                             name.Contains("overlay");
        
        // Verificar si tiene pocos hijos (típico de overlays temporales)
        bool fewChildren = canvas.transform.childCount <= 2;
        
        // Verificar si tiene imagen negra
        bool hasBlackImage = canvas.GetComponentInChildren<Image>()?.color == Color.black;
        
        return suspiciousName || (fewChildren && hasBlackImage);
    }
    
    /// <summary>
    /// Desactiva sistemas de pausa que pueden causar pantallas
    /// </summary>
    [ContextMenu("Disable Pause Systems")]
    public void DisablePauseSystems()
    {
        // Buscar y desactivar SimplePauseSetup
        SimplePauseSetup[] pauseSetups = FindObjectsByType<SimplePauseSetup>(FindObjectsSortMode.None);
        foreach (SimplePauseSetup setup in pauseSetups)
        {
            // Desactivar countdown automático
            setup.enabled = false;
            if (showDebugLogs)
            {
                Debug.Log($"⏸️ SimplePauseSetup desactivado: {setup.name}");
            }
        }
        
        // Buscar otros sistemas de pausa
        MonoBehaviour[] allScripts = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
        foreach (MonoBehaviour script in allScripts)
        {
            string typeName = script.GetType().Name.ToLower();
            if (typeName.Contains("pause") && typeName.Contains("transition"))
            {
                script.enabled = false;
                if (showDebugLogs)
                {
                    Debug.Log($"⏸️ Sistema de pausa desactivado: {script.GetType().Name}");
                }
            }
        }
    }
    
    /// <summary>
    /// Fuerza inicio limpio del gameplay
    /// </summary>
    [ContextMenu("Force Clean Start")]
    public void ForceCleanStart()
    {
        Debug.Log("🧹 FORZANDO INICIO LIMPIO");
        
        // Restaurar time scale
        Time.timeScale = 1f;
        
        // Limpiar todo
        RemoveLoadingScreens();
        RemoveCountdowns();
        CleanupTransitionObjects();
        DisablePauseSystems();
        
        // Asegurar que el audio esté funcionando
        AudioListener.pause = false;
        
        Debug.Log("✅ Inicio limpio forzado completado");
    }
    
    void Update()
    {
        // Verificar continuamente por pantallas no deseadas
        if (Time.frameCount % 60 == 0) // Cada segundo aproximadamente
        {
            CheckForUnwantedScreens();
        }
    }
    
    /// <summary>
    /// Verifica continuamente por pantallas no deseadas
    /// </summary>
    void CheckForUnwantedScreens()
    {
        try
        {
            // Verificar si aparecieron nuevas pantallas de countdown
            Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
            foreach (Canvas canvas in canvases)
            {
                if (canvas != null && canvas.gameObject != null && canvas.sortingOrder > 5000 && IsLikelyTransitionCanvas(canvas))
                {
                    string canvasName = canvas.name; // Guardar nombre antes de destruir
                    DestroyImmediate(canvas.gameObject);
                    if (showDebugLogs)
                    {
                        Debug.Log($"🗑️ Eliminada pantalla emergente: {canvasName}");
                    }
                }
            }
        }
        catch (System.Exception)
        {
            // Silenciar errores de objetos destruidos
        }
    }
}
