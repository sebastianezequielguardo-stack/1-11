using UnityEngine;

/// <summary>
/// TEST BÁSICO PARA VERIFICAR SI UNITY DETECTA LAS TECLAS
/// </summary>
public class BasicInputTest : MonoBehaviour
{
    void Update()
    {
        // TEST MUY BÁSICO - CUALQUIER TECLA
        if (Input.anyKeyDown)
        {
            // Tecla detectada
        }
        
        // TEST INDIVIDUAL DE CADA TECLA
        if (Input.GetKeyDown(KeyCode.D))
        {
            // D detectada
        }
        
        if (Input.GetKeyDown(KeyCode.F))
        {
            // F detectada
        }
        
        if (Input.GetKeyDown(KeyCode.J))
        {
            // J detectada
        }
        
        if (Input.GetKeyDown(KeyCode.K))
        {
            // K detectada
        }
        
        if (Input.GetKeyDown(KeyCode.L))
        {
            // L detectada
        }
        
        // TEST DE TECLAS ALTERNATIVAS
        if (Input.GetKeyDown(KeyCode.A))
        {
            // A detectada
        }
        
        if (Input.GetKeyDown(KeyCode.S))
        {
            // S detectada
        }
        
        // TEST DE NÚMEROS
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // 1 detectada
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            // 2 detectada
        }
        
        // TEST DE ESPACIO
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Espacio detectado
        }
        
        // MOSTRAR TODAS LAS TECLAS PRESIONADAS
        string inputString = Input.inputString;
        if (!string.IsNullOrEmpty(inputString))
        {
            // Input string detectado
        }
    }
    
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 400, 300));
        GUILayout.BeginVertical("box");
        
        GUILayout.Label("🔧 BASIC INPUT TEST", GUI.skin.box);
        GUILayout.Label("PRUEBA ESTAS TECLAS UNA POR UNA:");
        
        GUILayout.Space(10);
        GUILayout.Label("1. Presiona D (debe aparecer log)");
        GUILayout.Label("2. Presiona F (debe aparecer log)");
        GUILayout.Label("3. Presiona J (debe aparecer log)");
        GUILayout.Label("4. Presiona K (debe aparecer log)");
        GUILayout.Label("5. Presiona L (debe aparecer log)");
        
        GUILayout.Space(10);
        GUILayout.Label("SI NO VES LOGS:");
        GUILayout.Label("• Prueba A, S, 1, 2, ESPACIO");
        GUILayout.Label("• Verifica que la consola esté abierta");
        GUILayout.Label("• Verifica que el juego tenga foco");
        
        GUILayout.Space(10);
        if (GUILayout.Button("CLICK AQUÍ PARA DAR FOCO"))
        {
            Debug.Log("🎯 Botón clickeado - juego tiene foco");
        }
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
}
