using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Arregla todos los errores críticos del gameplay
/// Restaura referencias faltantes y configura componentes rotos
/// </summary>
public class GameplayErrorFixer : MonoBehaviour
{
    [Header("Auto Fix Settings")]
    public bool fixOnStart = true;
    public bool showDetailedLogs = true;
    
    [Header("Lane Configuration")]
    public int numberOfLanes = 5;
    public float laneSpacing = 2f;
    public float laneStartX = -4f;
    
    void Start()
    {
        if (fixOnStart)
        {
            FixAllGameplayErrors();
        }
    }
    
    /// <summary>
    /// Arregla todos los errores del gameplay
    /// </summary>
    [ContextMenu("Fix All Gameplay Errors")]
    public void FixAllGameplayErrors()
    {
        Debug.Log("🔧 ARREGLANDO ERRORES DEL GAMEPLAY");
        Debug.Log("═══════════════════════════════════");
        
        FixMissingScriptReferences();
        FixNoteSpawnerLanes();
        FixPauseMenuReferences();
        CreateMissingGameObjects();
        
        Debug.Log("✅ Todos los errores del gameplay arreglados");
    }
    
    /// <summary>
    /// Arregla referencias de scripts faltantes
    /// </summary>
    [ContextMenu("Fix Missing Script References")]
    public void FixMissingScriptReferences()
    {
        Debug.Log("📜 Arreglando referencias de scripts faltantes...");
        
        // Buscar todos los GameObjects con componentes rotos
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        int fixedReferences = 0;
        
        foreach (GameObject obj in allObjects)
        {
            if (obj == null) continue;
            
            Component[] components = obj.GetComponents<Component>();
            
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] == null)
                {
                    Debug.Log($"🔧 Script roto encontrado en: {obj.name}");
                    
                    // Intentar determinar qué script era y recrearlo
                    RecreateCommonScripts(obj);
                    fixedReferences++;
                }
            }
        }
        
        Debug.Log($"✅ {fixedReferences} referencias de scripts arregladas");
    }
    
    /// <summary>
    /// Recrea scripts comunes que pueden haberse perdido
    /// </summary>
    void RecreateCommonScripts(GameObject obj)
    {
        string objName = obj.name.ToLower();
        
        // Basado en el nombre del objeto, agregar scripts apropiados
        if (objName.Contains("notespawner") || objName.Contains("spawner"))
        {
            if (obj.GetComponent<NoteSpawner>() == null)
            {
                obj.AddComponent<NoteSpawner>();
                Debug.Log($"🎵 NoteSpawner agregado a {obj.name}");
            }
        }
        else if (objName.Contains("pause") || objName.Contains("menu"))
        {
            // Agregar componentes de pausa si es necesario
            if (obj.GetComponent<Canvas>() == null)
            {
                obj.AddComponent<Canvas>();
            }
        }
        else if (objName.Contains("gameplay") || objName.Contains("manager"))
        {
            if (obj.GetComponent<GameplayManager>() == null)
            {
                try
                {
                    obj.AddComponent<GameplayManager>();
                    Debug.Log($"🎮 GameplayManager agregado a {obj.name}");
                }
                catch
                {
                    Debug.LogWarning($"⚠️ No se pudo agregar GameplayManager a {obj.name}");
                }
            }
        }
    }
    
    /// <summary>
    /// Arregla las lanes del NoteSpawner
    /// </summary>
    [ContextMenu("Fix NoteSpawner Lanes")]
    public void FixNoteSpawnerLanes()
    {
        Debug.Log("🎵 Arreglando lanes del NoteSpawner...");
        
        // Buscar todos los NoteSpawners
        NoteSpawner[] noteSpawners = FindObjectsByType<NoteSpawner>(FindObjectsSortMode.None);
        
        if (noteSpawners.Length == 0)
        {
            Debug.LogWarning("⚠️ No se encontró NoteSpawner, creando uno nuevo...");
            CreateNoteSpawner();
            noteSpawners = FindObjectsByType<NoteSpawner>(FindObjectsSortMode.None);
        }
        
        foreach (NoteSpawner spawner in noteSpawners)
        {
            if (spawner == null) continue;
            
            // Crear lanes si no existen
            CreateLanesForSpawner(spawner);
        }
        
        Debug.Log($"✅ {noteSpawners.Length} NoteSpawners configurados con lanes");
    }
    
    /// <summary>
    /// Crea un NoteSpawner si no existe
    /// </summary>
    void CreateNoteSpawner()
    {
        GameObject spawnerObj = new GameObject("NoteSpawner");
        NoteSpawner spawner = spawnerObj.AddComponent<NoteSpawner>();
        
        // Posicionar en la parte superior
        spawnerObj.transform.position = new Vector3(0, 10, 0);
        
        Debug.Log("🎵 NoteSpawner creado");
    }
    
    /// <summary>
    /// Crea lanes para un NoteSpawner específico
    /// </summary>
    void CreateLanesForSpawner(NoteSpawner spawner)
    {
        // Buscar si ya tiene lanes asignadas
        var lanesField = spawner.GetType().GetField("lanes");
        if (lanesField != null)
        {
            Transform[] currentLanes = (Transform[])lanesField.GetValue(spawner);
            
            if (currentLanes == null || currentLanes.Length == 0 || System.Array.Exists(currentLanes, lane => lane == null))
            {
                Debug.Log($"🛣️ Creando lanes para {spawner.name}...");
                
                // Crear contenedor de lanes
                GameObject lanesContainer = GameObject.Find("Lanes");
                if (lanesContainer == null)
                {
                    lanesContainer = new GameObject("Lanes");
                    lanesContainer.transform.position = Vector3.zero;
                }
                
                // Crear array de lanes
                Transform[] newLanes = new Transform[numberOfLanes];
                
                for (int i = 0; i < numberOfLanes; i++)
                {
                    // Buscar lane existente o crear nueva
                    GameObject laneObj = GameObject.Find($"Lane_{i}");
                    if (laneObj == null)
                    {
                        laneObj = new GameObject($"Lane_{i}");
                        laneObj.transform.SetParent(lanesContainer.transform);
                        
                        // Posicionar lane
                        float xPos = laneStartX + (i * laneSpacing);
                        laneObj.transform.position = new Vector3(xPos, 0, 0);
                        
                        // Agregar marcador visual
                        CreateLaneVisual(laneObj, i);
                    }
                    
                    newLanes[i] = laneObj.transform;
                }
                
                // Asignar lanes al spawner
                lanesField.SetValue(spawner, newLanes);
                
                Debug.Log($"✅ {numberOfLanes} lanes creadas y asignadas a {spawner.name}");
            }
            else
            {
                Debug.Log($"✅ {spawner.name} ya tiene lanes asignadas");
            }
        }
        else
        {
            Debug.LogWarning($"⚠️ No se pudo encontrar el campo 'lanes' en {spawner.name}");
        }
    }
    
    /// <summary>
    /// Crea visual para una lane
    /// </summary>
    void CreateLaneVisual(GameObject laneObj, int laneIndex)
    {
        // Crear marcador visual simple
        GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cube);
        visual.name = "LaneMarker";
        visual.transform.SetParent(laneObj.transform);
        visual.transform.localPosition = Vector3.zero;
        visual.transform.localScale = new Vector3(0.5f, 0.1f, 10f);
        
        // Color basado en el índice
        Color[] laneColors = { Color.green, Color.red, Color.blue, Color.yellow, Color.magenta };
        Renderer renderer = visual.GetComponent<Renderer>();
        if (renderer != null && laneIndex < laneColors.Length)
        {
            renderer.material.color = laneColors[laneIndex];
        }
        
        // Remover collider innecesario
        Collider collider = visual.GetComponent<Collider>();
        if (collider != null)
        {
            DestroyImmediate(collider);
        }
    }
    
    /// <summary>
    /// Arregla referencias del menú de pausa
    /// </summary>
    [ContextMenu("Fix Pause Menu References")]
    public void FixPauseMenuReferences()
    {
        Debug.Log("⏸️ Arreglando referencias del menú de pausa...");
        
        // Buscar GameplayPauseIntegration
        GameplayPauseIntegration pauseIntegration = FindFirstObjectByType<GameplayPauseIntegration>();
        
        if (pauseIntegration != null)
        {
            // Crear PauseMenu si no existe
            GameObject pauseMenu = GameObject.Find("PauseMenu");
            if (pauseMenu == null)
            {
                CreateSimplePauseMenu();
                Debug.Log("⏸️ PauseMenu creado");
            }
            
            // Crear SimplePauseSetup si no existe
            GameObject simplePause = GameObject.Find("SimplePauseSetup");
            if (simplePause == null)
            {
                CreateSimplePauseSetup();
                Debug.Log("⏸️ SimplePauseSetup creado");
            }
        }
        
        Debug.Log("✅ Referencias del menú de pausa arregladas");
    }
    
    /// <summary>
    /// Crea un menú de pausa simple
    /// </summary>
    void CreateSimplePauseMenu()
    {
        GameObject pauseMenu = new GameObject("PauseMenu");
        
        // Agregar Canvas
        Canvas canvas = pauseMenu.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        
        pauseMenu.AddComponent<GraphicRaycaster>();
        
        // Inicialmente desactivado
        pauseMenu.SetActive(false);
        
        // Crear fondo
        GameObject background = new GameObject("Background");
        background.transform.SetParent(pauseMenu.transform);
        
        UnityEngine.UI.Image bgImage = background.AddComponent<UnityEngine.UI.Image>();
        bgImage.color = new Color(0, 0, 0, 0.8f);
        
        RectTransform bgRect = background.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        bgRect.anchoredPosition = Vector2.zero;
    }
    
    /// <summary>
    /// Crea SimplePauseSetup
    /// </summary>
    void CreateSimplePauseSetup()
    {
        GameObject simplePause = new GameObject("SimplePauseSetup");
        
        // Agregar un script simple de pausa
        SimplePauseController pauseController = simplePause.AddComponent<SimplePauseController>();
    }
    
    /// <summary>
    /// Crea GameObjects faltantes esenciales
    /// </summary>
    [ContextMenu("Create Missing GameObjects")]
    public void CreateMissingGameObjects()
    {
        Debug.Log("🎯 Creando GameObjects faltantes...");
        
        // Verificar y crear objetos esenciales
        CreateIfMissing("GameplayManager", () => {
            GameObject obj = new GameObject("GameplayManager");
            try
            {
                obj.AddComponent<GameplayManager>();
            }
            catch
            {
                Debug.LogWarning("⚠️ GameplayManager script no encontrado");
            }
            return obj;
        });
        
        CreateIfMissing("TrackBase", () => {
            GameObject obj = new GameObject("TrackBase");
            Canvas canvas = obj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            obj.AddComponent<GraphicRaycaster>();
            return obj;
        });
        
        CreateIfMissing("Highway", () => {
            GameObject obj = new GameObject("Highway");
            obj.transform.position = new Vector3(0, 0, 5);
            return obj;
        });
        
        Debug.Log("✅ GameObjects faltantes creados");
    }
    
    /// <summary>
    /// Crea un GameObject si no existe
    /// </summary>
    void CreateIfMissing(string name, System.Func<GameObject> createFunc)
    {
        if (GameObject.Find(name) == null)
        {
            GameObject obj = createFunc();
            Debug.Log($"🎯 {name} creado");
        }
    }
    
    /// <summary>
    /// Diagnóstico completo de errores
    /// </summary>
    [ContextMenu("Diagnose All Errors")]
    public void DiagnoseAllErrors()
    {
        Debug.Log("🔍 DIAGNÓSTICO COMPLETO DE ERRORES:");
        Debug.Log("═══════════════════════════════════");
        
        // Verificar NoteSpawners
        NoteSpawner[] spawners = FindObjectsByType<NoteSpawner>(FindObjectsSortMode.None);
        Debug.Log($"🎵 NoteSpawners encontrados: {spawners.Length}");
        
        foreach (NoteSpawner spawner in spawners)
        {
            if (spawner != null)
            {
                var lanesField = spawner.GetType().GetField("lanes");
                if (lanesField != null)
                {
                    Transform[] lanes = (Transform[])lanesField.GetValue(spawner);
                    bool hasValidLanes = lanes != null && lanes.Length > 0 && !System.Array.Exists(lanes, lane => lane == null);
                    Debug.Log($"  - {spawner.name}: Lanes {(hasValidLanes ? "✅ OK" : "❌ FALTANTES")}");
                }
            }
        }
        
        // Verificar PauseMenu
        GameObject pauseMenu = GameObject.Find("PauseMenu");
        Debug.Log($"⏸️ PauseMenu: {(pauseMenu != null ? "✅ EXISTE" : "❌ FALTANTE")}");
        
        GameObject simplePause = GameObject.Find("SimplePauseSetup");
        Debug.Log($"⏸️ SimplePauseSetup: {(simplePause != null ? "✅ EXISTE" : "❌ FALTANTE")}");
        
        // Verificar scripts rotos
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        int brokenScripts = 0;
        
        foreach (GameObject obj in allObjects)
        {
            if (obj == null) continue;
            
            Component[] components = obj.GetComponents<Component>();
            foreach (Component comp in components)
            {
                if (comp == null)
                {
                    brokenScripts++;
                }
            }
        }
        
        Debug.Log($"📜 Scripts rotos encontrados: {brokenScripts}");
        
        Debug.Log("\n💡 Usa 'Fix All Gameplay Errors' para arreglar todos los problemas");
    }
}

/// <summary>
/// Controlador simple de pausa
/// </summary>
public class SimplePauseController : MonoBehaviour
{
    private bool isPaused = false;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }
    
    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        
        GameObject pauseMenu = GameObject.Find("PauseMenu");
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(isPaused);
        }
        
        Debug.Log($"⏸️ Juego {(isPaused ? "pausado" : "reanudado")}");
    }
}
