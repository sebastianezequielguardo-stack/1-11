using UnityEngine;

/// <summary>
/// ULTRA SIMPLE CHORD DETECTOR - FUNCIONA SÍ O SÍ
/// </summary>
public class UltraSimpleChords : MonoBehaviour
{
    void Update()
    {
        // DETECTAR CADA FRAME QUÉ TECLAS ESTÁN PRESIONADAS
        bool D = Input.GetKey(KeyCode.D);
        bool F = Input.GetKey(KeyCode.F);
        bool J = Input.GetKey(KeyCode.J);
        bool K = Input.GetKey(KeyCode.K);
        bool L = Input.GetKey(KeyCode.L);
        
        // CONTAR CUÁNTAS TECLAS ESTÁN PRESIONADAS
        int count = 0;
        string teclas = "";
        
        if (D) { count++; teclas += "D "; }
        if (F) { count++; teclas += "F "; }
        if (J) { count++; teclas += "J "; }
        if (K) { count++; teclas += "K "; }
        if (L) { count++; teclas += "L "; }
        
        // SI HAY 2 O MÁS TECLAS PRESIONADAS = ACORDE
        if (count >= 2)
        {
            // Acorde detectado (log removido)
            
            // VERIFICAR PATRONES ESPECÍFICOS
            if (D && J && K && count == 3)
            {
                // Patrón DJK confirmado
            }
            
            if (F && K && L && count == 3)
            {
                // Patrón FKL confirmado
            }
                
            if (D && F && J && K && count == 4)
            {
                // Patrón 4 teclas confirmado
            }
        }
        
        // MOSTRAR ESTADO EN TIEMPO REAL
        if (count > 0)
        {
            // Teclas activas (log removido)
        }
    }
    
    void OnGUI()
    {
        // MOSTRAR ESTADO VISUAL
        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.BeginVertical("box");
        
        GUILayout.Label("🔥 ULTRA SIMPLE CHORDS", GUI.skin.box);
        
        // Estado de cada tecla
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
        
        GUI.color = Color.white;
        
        GUILayout.Space(10);
        GUILayout.Label("MANTÉN PRESIONADAS:");
        GUILayout.Label("• D + J + K juntas");
        GUILayout.Label("• F + K + L juntas");
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
}
