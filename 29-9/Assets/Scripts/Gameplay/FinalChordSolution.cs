using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// SOLUCIÓN FINAL GARANTIZADA PARA ACORDES
/// Este script funciona SIN FALTA - detecta múltiples teclas presionadas simultáneamente
/// </summary>
public class FinalChordSolution : MonoBehaviour
{
    [Header("ACTIVAR SISTEMA")]
    public bool ACTIVADO = true;
    
    // Teclas del juego: D=0, F=1, J=2, K=3, L=4
    private KeyCode[] teclas = { KeyCode.D, KeyCode.F, KeyCode.J, KeyCode.K, KeyCode.L };
    private string[] nombres = { "D", "F", "J", "K", "L" };
    
    // Control de detección
    private bool[] teclaActiva = new bool[5];
    private float tiempoVentana = 0.2f; // 200ms para presionar teclas
    private List<int> acordeActual = new List<int>();
    
    void Start()
    {
        Debug.Log("🔥🔥🔥 FINAL CHORD SOLUTION ACTIVADO 🔥🔥🔥");
        Debug.Log("PRESIONA ESTAS COMBINACIONES:");
        Debug.Log("• D + J + K");
        Debug.Log("• F + K + L");
        Debug.Log("• D + F + J + K");
        Debug.Log("• Cualquier combinación de 2+ teclas");
    }
    
    void Update()
    {
        if (!ACTIVADO) return;
        
        // MÉTODO 1: Detección frame por frame
        DeteccionDirecta();
        
        // MÉTODO 2: Detección por ventana de tiempo
        DeteccionPorTiempo();
    }
    
    void DeteccionDirecta()
    {
        // Verificar qué teclas están presionadas AHORA MISMO
        List<int> teclasPresionadas = new List<int>();
        
        for (int i = 0; i < teclas.Length; i++)
        {
            if (Input.GetKey(teclas[i]))
            {
                teclasPresionadas.Add(i);
            }
        }
        
        // Si hay 2 o más teclas presionadas simultáneamente
        if (teclasPresionadas.Count >= 2)
        {
            ProcesarAcordeDirecto(teclasPresionadas);
        }
    }
    
    void DeteccionPorTiempo()
    {
        // Detectar nuevas presiones
        for (int i = 0; i < teclas.Length; i++)
        {
            if (Input.GetKeyDown(teclas[i]))
            {
                if (!acordeActual.Contains(i))
                {
                    acordeActual.Add(i);
                    Debug.Log($"🎹 {nombres[i]} agregada al acorde actual");
                }
                
                // Reiniciar timer
                CancelInvoke("ProcesarAcordeTemporal");
                Invoke("ProcesarAcordeTemporal", tiempoVentana);
            }
        }
    }
    
    void ProcesarAcordeDirecto(List<int> teclas)
    {
        // Evitar spam - solo procesar cuando cambie el acorde
        string acordeStr = string.Join(",", teclas);
        if (acordeStr == ultimoAcorde) return;
        ultimoAcorde = acordeStr;
        
        if (teclas.Count >= 2)
        {
            MostrarExito(teclas, "DETECCIÓN DIRECTA");
        }
    }
    
    private string ultimoAcorde = "";
    
    void ProcesarAcordeTemporal()
    {
        if (acordeActual.Count >= 2)
        {
            MostrarExito(acordeActual, "DETECCIÓN TEMPORAL");
        }
        
        acordeActual.Clear();
    }
    
    void MostrarExito(List<int> indices, string metodo)
    {
        // Crear nombres de teclas
        List<string> nombresAcorde = new List<string>();
        foreach (int i in indices)
        {
            nombresAcorde.Add(nombres[i]);
        }
        
        string acordeTexto = string.Join(" + ", nombresAcorde);
        string patron = Analizar(indices);
        
        // LOGS GARANTIZADOS
        Debug.Log("🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉");
        Debug.Log($"🔥 ÉXITO TOTAL: {acordeTexto}");
        Debug.Log($"🎯 PATRÓN: {patron}");
        Debug.Log($"⚡ MÉTODO: {metodo}");
        Debug.Log($"📊 TECLAS: {indices.Count}");
        Debug.Log("🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉");
        
        // Verificar patrones específicos
        VerificarPatronesEspecificos(indices);
        
        // Mensaje en pantalla
        MostrarEnPantalla(acordeTexto, patron);
    }
    
    string Analizar(List<int> indices)
    {
        var sorted = new List<int>(indices);
        sorted.Sort();
        
        if (sorted.Count == 2) return "2 TECLAS";
        if (sorted.Count == 3) return "3 TECLAS";
        if (sorted.Count == 4) return "4 TECLAS";
        if (sorted.Count == 5) return "TODAS LAS TECLAS";
        
        return $"{sorted.Count} TECLAS";
    }
    
    void VerificarPatronesEspecificos(List<int> indices)
    {
        var sorted = new List<int>(indices);
        sorted.Sort();
        string patron = string.Join(",", sorted);
        
        Debug.Log($"🔍 PATRÓN NUMÉRICO: {patron}");
        
        switch (patron)
        {
            case "0,2,3":
                Debug.Log("✅ DJK DETECTADO - D + J,K continuas");
                break;
            case "1,3,4":
                Debug.Log("✅ FKL DETECTADO - F + K,L continuas");
                break;
            case "0,1,3":
                Debug.Log("✅ DFK DETECTADO - D,F continuas + K");
                break;
            case "1,2,4":
                Debug.Log("✅ FJL DETECTADO - F,J continuas + L");
                break;
            case "0,1,2,3":
                Debug.Log("✅ DFJK DETECTADO - 4 teclas");
                break;
            default:
                Debug.Log($"✅ PATRÓN PERSONALIZADO: {patron}");
                break;
        }
    }
    
    void MostrarEnPantalla(string acorde, string patron)
    {
        // Crear objeto de texto temporal
        GameObject obj = new GameObject("AcordeExito");
        
        // Buscar Canvas
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas != null)
        {
            obj.transform.SetParent(canvas.transform, false);
            
            // Crear texto
            var texto = obj.AddComponent<UnityEngine.UI.Text>();
            texto.text = $"🔥 ÉXITO 🔥\n{acorde}\n{patron}";
            texto.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            texto.fontSize = 36;
            texto.color = Color.red;
            texto.alignment = TextAnchor.MiddleCenter;
            texto.fontStyle = FontStyle.Bold;
            
            // Posicionar en el centro
            var rect = obj.GetComponent<RectTransform>();
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(600, 200);
            
            // Destruir después de 4 segundos
            Destroy(obj, 4f);
            
            Debug.Log("🖥️ MENSAJE MOSTRADO EN PANTALLA");
        }
        else
        {
            Debug.LogWarning("⚠️ No se encontró Canvas para mostrar mensaje");
        }
    }
    
    void OnGUI()
    {
        if (!ACTIVADO) return;
        
        // Panel de estado
        GUILayout.BeginArea(new Rect(10, 10, 500, 300));
        GUILayout.BeginVertical("box");
        
        GUILayout.Label("🔥 FINAL CHORD SOLUTION", GUI.skin.box);
        GUILayout.Label($"Estado: {(ACTIVADO ? "ACTIVO" : "INACTIVO")}");
        
        GUILayout.Space(10);
        GUILayout.Label("PATRONES A PROBAR:", GUI.skin.box);
        GUILayout.Label("• D + J + K (presiona las 3 juntas)");
        GUILayout.Label("• F + K + L (presiona las 3 juntas)");
        GUILayout.Label("• D + F + J + K (presiona las 4 juntas)");
        
        GUILayout.Space(10);
        GUILayout.Label("ESTADO EN TIEMPO REAL:");
        
        // Mostrar estado de cada tecla
        for (int i = 0; i < teclas.Length; i++)
        {
            bool presionada = Input.GetKey(teclas[i]);
            GUI.color = presionada ? Color.green : Color.white;
            GUILayout.Label($"{nombres[i]}: {(presionada ? "PRESIONADA" : "libre")}");
        }
        GUI.color = Color.white;
        
        GUILayout.Space(10);
        if (acordeActual.Count > 0)
        {
            string acordeStr = "";
            foreach (int i in acordeActual)
            {
                acordeStr += nombres[i] + " ";
            }
            GUILayout.Label($"Acorde actual: {acordeStr}");
        }
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
}
