using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// DETECTOR DE ACORDES CON CONTROL DE PLAYSTATION Y TECLADO
/// </summary>
public class ControllerChordTest : MonoBehaviour
{
    [Header("CONFIGURACIÓN")]
    public bool usarControl = true;
    public bool usarTeclado = true;
    public bool debugContinuo = false;
    
    void Start()
    {
        Debug.Log("🎮 CONTROLLER CHORD TEST INICIADO");
        Debug.Log("🎮 Control: X, Square, Triangle, Circle, R1");
        Debug.Log("⌨️ Teclado: D, F, J, K, L");
        Debug.Log("🔧 Conecta tu control de PS y prueba ambos métodos");
    }
    
    void Update()
    {
        // DETECTAR INPUT DEL CONTROL
        bool[] controlInputs = new bool[5];
        if (usarControl)
        {
            controlInputs[0] = Input.GetKey(KeyCode.Joystick1Button0); // X
            controlInputs[1] = Input.GetKey(KeyCode.Joystick1Button2); // Square  
            controlInputs[2] = Input.GetKey(KeyCode.Joystick1Button3); // Triangle
            controlInputs[3] = Input.GetKey(KeyCode.Joystick1Button1); // Circle
            controlInputs[4] = Input.GetKey(KeyCode.Joystick1Button5); // R1
        }
        
        // DETECTAR INPUT DEL TECLADO
        bool[] keyboardInputs = new bool[5];
        if (usarTeclado)
        {
            keyboardInputs[0] = Input.GetKey(KeyCode.D);
            keyboardInputs[1] = Input.GetKey(KeyCode.F);
            keyboardInputs[2] = Input.GetKey(KeyCode.J);
            keyboardInputs[3] = Input.GetKey(KeyCode.K);
            keyboardInputs[4] = Input.GetKey(KeyCode.L);
        }
        
        // COMBINAR AMBOS INPUTS
        bool[] finalInputs = new bool[5];
        for (int i = 0; i < 5; i++)
        {
            finalInputs[i] = controlInputs[i] || keyboardInputs[i];
        }
        
        // CONTAR INPUTS ACTIVOS
        int count = 0;
        List<string> activeInputs = new List<string>();
        string[] inputNames = { "1", "2", "3", "4", "5" };
        
        for (int i = 0; i < 5; i++)
        {
            if (finalInputs[i])
            {
                count++;
                activeInputs.Add(inputNames[i]);
            }
        }
        
        // DEBUG CONTINUO
        if (debugContinuo && count > 0)
        {
            Debug.Log($"Inputs activos: {string.Join(", ", activeInputs)} (Total: {count})");
        }
        
        // DETECTAR ACORDES
        if (count >= 2)
        {
            string acordeTexto = string.Join(" + ", activeInputs);
            Debug.Log($"🔥 ACORDE DETECTADO: {acordeTexto} ({count} inputs)");
            
            // PATRONES ESPECÍFICOS
            DetectarPatrones(finalInputs, count);
        }
        
        // DETECTAR CAMBIOS INDIVIDUALES
        DetectarCambiosIndividuales(controlInputs, keyboardInputs);
    }
    
    void DetectarPatrones(bool[] inputs, int count)
    {
        // Patrón 1+3+4 (equivalente a DJK)
        if (inputs[0] && inputs[2] && inputs[3] && count == 3)
        {
            Debug.Log("✅ PATRÓN 1+3+4 DETECTADO (equivalente a DJK)");
        }
        
        // Patrón 2+4+5 (equivalente a FKL)
        if (inputs[1] && inputs[3] && inputs[4] && count == 3)
        {
            Debug.Log("✅ PATRÓN 2+4+5 DETECTADO (equivalente a FKL)");
        }
        
        // Patrón de 4 inputs
        if (count == 4)
        {
            Debug.Log("✅ PATRÓN DE 4 INPUTS DETECTADO");
        }
        
        // Todos los inputs
        if (count == 5)
        {
            Debug.Log("✅ TODOS LOS INPUTS ACTIVOS");
        }
    }
    
    private bool[] lastControlInputs = new bool[5];
    private bool[] lastKeyboardInputs = new bool[5];
    
    void DetectarCambiosIndividuales(bool[] controlInputs, bool[] keyboardInputs)
    {
        string[] controlNames = { "X", "Square", "Triangle", "Circle", "R1" };
        string[] keyNames = { "D", "F", "J", "K", "L" };
        
        // Detectar cambios en control
        for (int i = 0; i < 5; i++)
        {
            if (controlInputs[i] != lastControlInputs[i])
            {
                Debug.Log($"🎮 Control {controlNames[i]}: {(controlInputs[i] ? "PRESIONADO" : "LIBERADO")}");
                lastControlInputs[i] = controlInputs[i];
            }
        }
        
        // Detectar cambios en teclado
        for (int i = 0; i < 5; i++)
        {
            if (keyboardInputs[i] != lastKeyboardInputs[i])
            {
                Debug.Log($"⌨️ Teclado {keyNames[i]}: {(keyboardInputs[i] ? "PRESIONADO" : "LIBERADO")}");
                lastKeyboardInputs[i] = keyboardInputs[i];
            }
        }
    }
    
    void OnGUI()
    {
        // PANEL PRINCIPAL
        GUILayout.BeginArea(new Rect(10, 10, 400, 500));
        GUILayout.BeginVertical("box");
        
        GUILayout.Label("🎮 CONTROLLER + KEYBOARD TEST", GUI.skin.box);
        
        usarControl = GUILayout.Toggle(usarControl, "Usar Control de PS");
        usarTeclado = GUILayout.Toggle(usarTeclado, "Usar Teclado");
        debugContinuo = GUILayout.Toggle(debugContinuo, "Debug Continuo");
        
        GUILayout.Space(10);
        
        // ESTADO DEL CONTROL
        if (usarControl)
        {
            GUILayout.Label("🎮 ESTADO DEL CONTROL:", GUI.skin.box);
            
            GUI.color = Input.GetKey(KeyCode.Joystick1Button0) ? Color.green : Color.white;
            GUILayout.Label("X (Button 0): " + (Input.GetKey(KeyCode.Joystick1Button0) ? "PRESIONADO" : "libre"));
            
            GUI.color = Input.GetKey(KeyCode.Joystick1Button2) ? Color.green : Color.white;
            GUILayout.Label("Square (Button 2): " + (Input.GetKey(KeyCode.Joystick1Button2) ? "PRESIONADO" : "libre"));
            
            GUI.color = Input.GetKey(KeyCode.Joystick1Button3) ? Color.green : Color.white;
            GUILayout.Label("Triangle (Button 3): " + (Input.GetKey(KeyCode.Joystick1Button3) ? "PRESIONADO" : "libre"));
            
            GUI.color = Input.GetKey(KeyCode.Joystick1Button1) ? Color.green : Color.white;
            GUILayout.Label("Circle (Button 1): " + (Input.GetKey(KeyCode.Joystick1Button1) ? "PRESIONADO" : "libre"));
            
            GUI.color = Input.GetKey(KeyCode.Joystick1Button5) ? Color.green : Color.white;
            GUILayout.Label("R1 (Button 5): " + (Input.GetKey(KeyCode.Joystick1Button5) ? "PRESIONADO" : "libre"));
        }
        
        GUI.color = Color.white;
        GUILayout.Space(10);
        
        // ESTADO DEL TECLADO
        if (usarTeclado)
        {
            GUILayout.Label("⌨️ ESTADO DEL TECLADO:", GUI.skin.box);
            
            GUI.color = Input.GetKey(KeyCode.D) ? Color.green : Color.white;
            GUILayout.Label("D: " + (Input.GetKey(KeyCode.D) ? "PRESIONADA" : "libre"));
            
            GUI.color = Input.GetKey(KeyCode.F) ? Color.green : Color.white;
            GUILayout.Label("F: " + (Input.GetKey(KeyCode.F) ? "PRESIONADA" : "libre"));
            
            GUI.color = Input.GetKey(KeyCode.J) ? Color.green : Color.white;
            GUILayout.Label("J: " + (Input.GetKey(KeyCode.J) ? "PRESIONADA" : "libre"));
            
            GUI.color = Input.GetKey(KeyCode.K) ? Color.green : Color.white;
            GUILayout.Label("K: " + (Input.GetKey(KeyCode.K) ? "PRESIONADA" : "libre"));
            
            GUI.color = Input.GetKey(KeyCode.L) ? Color.green : Color.white;
            GUILayout.Label("L: " + (Input.GetKey(KeyCode.L) ? "PRESIONADA" : "libre"));
        }
        
        GUI.color = Color.white;
        GUILayout.Space(10);
        
        // CONTADOR TOTAL
        int totalCount = 0;
        if (usarControl)
        {
            if (Input.GetKey(KeyCode.Joystick1Button0)) totalCount++;
            if (Input.GetKey(KeyCode.Joystick1Button1)) totalCount++;
            if (Input.GetKey(KeyCode.Joystick1Button2)) totalCount++;
            if (Input.GetKey(KeyCode.Joystick1Button3)) totalCount++;
            if (Input.GetKey(KeyCode.Joystick1Button5)) totalCount++;
        }
        if (usarTeclado)
        {
            if (Input.GetKey(KeyCode.D)) totalCount++;
            if (Input.GetKey(KeyCode.F)) totalCount++;
            if (Input.GetKey(KeyCode.J)) totalCount++;
            if (Input.GetKey(KeyCode.K)) totalCount++;
            if (Input.GetKey(KeyCode.L)) totalCount++;
        }
        
        GUI.color = totalCount >= 2 ? Color.yellow : Color.white;
        GUILayout.Label($"INPUTS TOTALES: {totalCount}");
        
        if (totalCount >= 2)
        {
            GUI.color = Color.red;
            GUILayout.Label("🔥 ACORDE DETECTADO 🔥", GUI.skin.box);
        }
        
        GUI.color = Color.white;
        GUILayout.EndVertical();
        GUILayout.EndArea();
        
        // INSTRUCCIONES
        GUILayout.BeginArea(new Rect(Screen.width - 350, 10, 340, 200));
        GUILayout.BeginVertical("box");
        
        GUILayout.Label("📋 INSTRUCCIONES", GUI.skin.box);
        GUILayout.Label("CONTROL DE PS:");
        GUILayout.Label("• X + Triangle + Circle");
        GUILayout.Label("• Square + Circle + R1");
        GUILayout.Label("");
        GUILayout.Label("TECLADO:");
        GUILayout.Label("• D + J + K");
        GUILayout.Label("• F + K + L");
        GUILayout.Label("");
        GUILayout.Label("Mantén los botones presionados");
        GUILayout.Label("y mira la consola!");
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
}
