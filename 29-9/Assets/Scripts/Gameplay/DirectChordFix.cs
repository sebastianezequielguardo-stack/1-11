using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// SOLUCIÓN DIRECTA Y SIMPLE PARA ACORDES
/// Este script funciona independientemente de otros sistemas
/// </summary>
public class DirectChordFix : MonoBehaviour
{
    [Header("CONFIGURACIÓN SIMPLE")]
    public bool activarSistema = true;
    public float ventanaTiempo = 0.15f; // 150ms para presionar teclas juntas
    
    // Teclas del juego
    private KeyCode[] teclas = { KeyCode.D, KeyCode.F, KeyCode.J, KeyCode.K, KeyCode.L };
    private string[] nombresTeclas = { "D", "F", "J", "K", "L" };
    
    // Control de tiempo
    private float[] tiempoPresion = new float[5];
    private bool[] teclaPresionada = new bool[5];
    
    void Start()
    {
        // Direct Chord Fix iniciado
        
        // Inicializar arrays
        for (int i = 0; i < 5; i++)
        {
            tiempoPresion[i] = -1f;
            teclaPresionada[i] = false;
        }
    }
    
    void Update()
    {
        if (!activarSistema) return;
        
        DetectarTeclas();
        ProcesarAcordes();
    }
    
    void DetectarTeclas()
    {
        float tiempoActual = Time.time;
        
        // Detectar presiones de teclas
        for (int i = 0; i < teclas.Length; i++)
        {
            if (Input.GetKeyDown(teclas[i]))
            {
                tiempoPresion[i] = tiempoActual;
                teclaPresionada[i] = true;
            }
            
            if (Input.GetKeyUp(teclas[i]))
            {
                teclaPresionada[i] = false;
            }
        }
    }
    
    void ProcesarAcordes()
    {
        float tiempoActual = Time.time;
        List<int> teclasEnVentana = new List<int>();
        
        // Encontrar todas las teclas presionadas dentro de la ventana de tiempo
        for (int i = 0; i < teclas.Length; i++)
        {
            if (tiempoPresion[i] > 0 && (tiempoActual - tiempoPresion[i]) <= ventanaTiempo)
            {
                teclasEnVentana.Add(i);
            }
        }
        
        // Si tenemos 2 o más teclas, es un acorde
        if (teclasEnVentana.Count >= 2)
        {
            ProcesarAcorde(teclasEnVentana);
            
            // Limpiar las teclas procesadas
            foreach (int i in teclasEnVentana)
            {
                tiempoPresion[i] = -1f;
            }
        }
        // Si solo hay 1 tecla y ha pasado la ventana, procesarla como nota individual
        else if (teclasEnVentana.Count == 1)
        {
            float tiempoTecla = tiempoPresion[teclasEnVentana[0]];
            if ((tiempoActual - tiempoTecla) > ventanaTiempo)
            {
                ProcesarNotaIndividual(teclasEnVentana[0]);
                tiempoPresion[teclasEnVentana[0]] = -1f;
            }
        }
    }
    
    void ProcesarAcorde(List<int> indices)
    {
        // Crear string con los nombres de las teclas
        List<string> nombresAcorde = new List<string>();
        foreach (int i in indices)
        {
            nombresAcorde.Add(nombresTeclas[i]);
        }
        
        string acordeStr = string.Join(" + ", nombresAcorde);
        
        // Análisis del patrón
        string patron = AnalizarPatron(indices);
        
        // Acorde detectado
        // Mostrar mensaje en pantalla
        MostrarMensajeExito(acordeStr, patron);
        
        // Aquí puedes agregar la lógica del juego (puntuación, efectos, etc.)
        EjecutarLogicaJuego(indices);
    }
    
    void ProcesarNotaIndividual(int indice)
    {
    }
    
    string AnalizarPatron(List<int> indices)
    {
        indices.Sort();
        
        if (indices.Count == 2)
        {
            int diferencia = indices[1] - indices[0];
            if (diferencia == 1)
                return "2 TECLAS CONTINUAS";
            else
                return "2 TECLAS SEPARADAS";
        }
        else if (indices.Count == 3)
        {
            // Casos específicos primero
            if (EsPatronEspecifico(indices))
                return "2 CONTINUAS + 1 SEPARADA ";
            // Verificar patrón general 2 continuas + 1 separada
            else if (EsPatron2Continuas1Separada(indices))
                return "2 CONTINUAS + 1 SEPARADA ";
            else
                return "3 TECLAS MIXTAS";
        }
        else if (indices.Count == 4)
        {
            return "4 TECLAS JUNTAS ";
        }
        else if (indices.Count == 5)
        {
            return "!TODAS LAS TECLAS!";
        }
        
        return $"{indices.Count} TECLAS";
    }
    
    bool EsPatronEspecifico(List<int> indices)
    {
        if (indices.Count != 3) return false;
        
        indices.Sort();
        
        // Casos específicos que deben funcionar:
        // DJK = 0,2,3 -> D(0) + J,K(2,3) continuas
        // FKL = 1,3,4 -> F(1) + K,L(3,4) continuas
        // DFK = 0,1,3 -> D,F(0,1) continuas + K(3)
        // FJL = 1,2,4 -> F,J(1,2) continuas + L(4)
        
        string patron = string.Join(",", indices);
        
        switch (patron)
        {
            case "0,2,3": // DJK
                // DJK detectado
                return true;
            case "1,3,4": // FKL
                // FKL detectado
                return true;
            case "0,1,3": // DFK
                // DFK detectado
                return true;
            case "1,2,4": // FJL
                // FJL detectado
                return true;
            case "0,3,4": // DKL
                // DKL detectado
                return true;
            case "0,1,2": // DFJ
                // DFJ detectado
                return false; // Este es 3 continuas, no 2+1
            case "2,3,4": // JKL
                // JKL detectado
                return false; // Este es 3 continuas, no 2+1
        }
        
        return false;
    }
    
    bool EsPatron2Continuas1Separada(List<int> indices)
    {
        if (indices.Count != 3) return false;
        
        // Analizar todos los patrones posibles para 3 teclas
        // D=0, F=1, J=2, K=3, L=4
        
        // Casos específicos que deben funcionar:
        // DJK = 0,2,3 -> J,K continuas (2,3) + D separada (0)
        // FKL = 1,3,4 -> K,L continuas (3,4) + F separada (1)
        
        indices.Sort();
        
        // Verificar si hay exactamente un par continuo
        int paresContinuos = 0;
        int indicePar = -1;
        
        for (int i = 0; i < indices.Count - 1; i++)
        {
            if (indices[i + 1] - indices[i] == 1)
            {
                paresContinuos++;
                indicePar = i;
            }
        }
        
        // Debe haber exactamente 1 par continuo
        if (paresContinuos == 1)
        {
            // Encontrar la tecla separada
            int teclaSeparada = -1;
            for (int i = 0; i < indices.Count; i++)
            {
                if (i != indicePar && i != indicePar + 1)
                {
                    teclaSeparada = indices[i];
                    break;
                }
            }
            
            // Verificar que la tecla separada tenga gap > 1 con el par continuo
            if (teclaSeparada != -1)
            {
                int gap1 = Mathf.Abs(teclaSeparada - indices[indicePar]);
                int gap2 = Mathf.Abs(teclaSeparada - indices[indicePar + 1]);
                
                bool esValido = gap1 > 1 || gap2 > 1;
                return esValido;
            }
        }
        
        return false;
    }
    
    void MostrarMensajeExito(string acorde, string patron)
    {
        // Crear mensaje temporal en pantalla
        GameObject mensaje = new GameObject("MensajeAcorde");
        
        // Buscar Canvas
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas != null)
        {
            mensaje.transform.SetParent(canvas.transform, false);
            
            // Agregar componente de texto
            var texto = mensaje.AddComponent<UnityEngine.UI.Text>();
            texto.text = $"¡ÉXITO!\n{acorde}\n{patron}";
            texto.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            texto.fontSize = 32;
            texto.color = Color.green;
            texto.alignment = TextAnchor.MiddleCenter;
            
            // Configurar posición
            var rect = mensaje.GetComponent<RectTransform>();
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(500, 150);
            
            // Destruir después de 3 segundos
            Destroy(mensaje, 3f);
        }
    }
    
    void EjecutarLogicaJuego(List<int> indices)
    {
        // Aquí puedes agregar:
        // - Puntuación
        // - Efectos visuales
        // - Sonidos
        // - Lógica específica del juego
        
        // Lógica de juego ejecutada
    }
    
    void OnGUI()
    {
        if (!activarSistema) return;
        
        // Panel de información
        GUILayout.BeginArea(new Rect(10, 10, 400, 250));
        GUILayout.BeginVertical("box");
        
        GUILayout.Label("🔥 DIRECT CHORD FIX", GUI.skin.box);
        GUILayout.Label($"Estado: {(activarSistema ? "ACTIVO" : "INACTIVO")}");
        GUILayout.Label($"Ventana de tiempo: {ventanaTiempo * 1000:F0}ms");
        
        GUILayout.Space(10);
        GUILayout.Label("PATRONES A PROBAR:", GUI.skin.box);
        GUILayout.Label("• D + F + K (2 continuas + 1 separada)");
        GUILayout.Label("• D + F + J + K (4 teclas juntas)");
        GUILayout.Label("• F + J + L (2 continuas + 1 separada)");
        GUILayout.Label("• Cualquier combinación de 2+ teclas");
        
        GUILayout.Space(10);
        
        // Mostrar estado de teclas en tiempo real
        GUILayout.Label("Estado de teclas:");
        for (int i = 0; i < teclas.Length; i++)
        {
            bool presionada = Input.GetKey(teclas[i]);
            string estado = presionada ? "PRESIONADA" : "libre";
            Color color = presionada ? Color.green : Color.white;
            
            GUI.color = color;
            GUILayout.Label($"{nombresTeclas[i]}: {estado}");
        }
        GUI.color = Color.white;
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
}
