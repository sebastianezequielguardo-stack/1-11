using UnityEngine;

/// <summary>
/// DEBUGGER PARA VER EXACTAMENTE QUÉ TECLAS SE ESTÁN PRESIONANDO
/// </summary>
public class KeyboardDebugger : MonoBehaviour
{
    void Update()
    {
        // DETECTAR PRESIONES INDIVIDUALES
        if (Input.GetKeyDown(KeyCode.D))
        {
            // D presionada
        }
            
        if (Input.GetKeyDown(KeyCode.F))
        {
            // F presionada
        }
            
        if (Input.GetKeyDown(KeyCode.J))
        {
            // J presionada
        }
            
        if (Input.GetKeyDown(KeyCode.K))
        {
            // K presionada
        }
            
        if (Input.GetKeyDown(KeyCode.L))
        {
            // L presionada
        }
        
        // DETECTAR LIBERACIONES
        if (Input.GetKeyUp(KeyCode.D))
        {
            // D liberada
        }
            
        if (Input.GetKeyUp(KeyCode.F))
        {
            // F liberada
        }
            
        if (Input.GetKeyUp(KeyCode.J))
        {
            // J liberada
        }
            
        if (Input.GetKeyUp(KeyCode.K))
        {
            // K liberada
        }
            
        if (Input.GetKeyUp(KeyCode.L))
        {
            // L liberada
        }
        
        // DETECTAR CUALQUIER TECLA
        if (Input.anyKeyDown)
        {
            // Alguna tecla presionada
        }
        
        // TEST ESPECÍFICO PARA DJK
        if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.J) && Input.GetKey(KeyCode.K))
        {
            // DJK todas presionadas
        }
        
        // TEST ESPECÍFICO PARA FKL
        if (Input.GetKey(KeyCode.F) && Input.GetKey(KeyCode.K) && Input.GetKey(KeyCode.L))
        {
            // FKL todas presionadas
        }
    }
    
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(Screen.width - 350, 10, 340, 150));
        GUILayout.BeginVertical("box");
        
        GUILayout.Label("🔍 KEYBOARD DEBUGGER", GUI.skin.box);
        GUILayout.Label("Presiona teclas y mira la consola");
        
        GUILayout.Space(10);
        GUILayout.Label("INSTRUCCIONES:");
        GUILayout.Label("1. Presiona D, luego J, luego K");
        GUILayout.Label("2. Mantén D+J+K presionadas juntas");
        GUILayout.Label("3. Mira los logs en consola");
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
}
