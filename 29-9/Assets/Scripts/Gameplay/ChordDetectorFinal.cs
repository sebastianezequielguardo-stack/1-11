using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// DETECTOR DE ACORDES FINAL - SI LAS TECLAS INDIVIDUALES FUNCIONAN, ESTO TAMBIÉN
/// </summary>
public class ChordDetectorFinal : MonoBehaviour
{
    [Header("CONFIGURACIÓN")]
    public bool mostrarDebugContinuo = false;
    
    private bool ultimoEstadoD = false;
    private bool ultimoEstadoF = false;
    private bool ultimoEstadoJ = false;
    private bool ultimoEstadoK = false;
    private bool ultimoEstadoL = false;
    
    void Update()
    {
        // OBTENER ESTADO ACTUAL DE CADA TECLA
        bool D = Input.GetKey(KeyCode.D);
        bool F = Input.GetKey(KeyCode.F);
        bool J = Input.GetKey(KeyCode.J);
        bool K = Input.GetKey(KeyCode.K);
        bool L = Input.GetKey(KeyCode.L);
        
        // MOSTRAR DEBUG CONTINUO SI ESTÁ ACTIVADO
        if (mostrarDebugContinuo)
        {
            // Estado de teclas (log removido)
        }
        
        // DETECTAR CAMBIOS DE ESTADO
        if (D != ultimoEstadoD)
        {
            ultimoEstadoD = D;
        }
        
        if (F != ultimoEstadoF)
        {
            ultimoEstadoF = F;
        }
        
        if (J != ultimoEstadoJ)
        {
            ultimoEstadoJ = J;
        }
        
        if (K != ultimoEstadoK)
        {
            ultimoEstadoK = K;
        }
        
        if (L != ultimoEstadoL)
        {
            ultimoEstadoL = L;
        }
        
        // CONTAR TECLAS PRESIONADAS
        int teclasPresionadas = 0;
        List<string> teclas = new List<string>();
        
        if (D) { teclasPresionadas++; teclas.Add("D"); }
        if (F) { teclasPresionadas++; teclas.Add("F"); }
        if (J) { teclasPresionadas++; teclas.Add("J"); }
        if (K) { teclasPresionadas++; teclas.Add("K"); }
        if (L) { teclasPresionadas++; teclas.Add("L"); }
        
        // DETECTAR ACORDES
        if (teclasPresionadas >= 2)
        {
            string acordeTexto = string.Join(" + ", teclas);
            // Acorde activo detectado
            
            // VERIFICAR PATRONES ESPECÍFICOS
            VerificarPatrones(D, F, J, K, L, teclasPresionadas);
        }
        
        // CASOS ESPECÍFICOS MUY EXPLÍCITOS
        if (D && J && K && !F && !L)
        {
            // Patrón DJK detectado
        }
        
        if (F && K && L && !D && !J)
        {
            // Patrón FKL detectado
        }
        
        if (D && F && J && K && !L)
        {
            // Patrón DFJK detectado
        }
    }
    
    void VerificarPatrones(bool D, bool F, bool J, bool K, bool L, int count)
    {
        if (count == 2)
        {
            // Patrones de 2 teclas detectados (logs removidos)
        }
        else if (count == 3)
        {
            // Patrones de 3 teclas detectados (logs removidos)
        }
        else if (count == 4)
        {
            // Patrón de 4 teclas detectado
        }
        else if (count == 5)
        {
            // Todas las teclas presionadas
        }
    }
    
    void OnGUI()
    {
        // PANEL PRINCIPAL
        GUILayout.BeginArea(new Rect(10, 10, 350, 400));
        GUILayout.BeginVertical("box");
        
        GUILayout.Label("🔥 CHORD DETECTOR FINAL", GUI.skin.box);
        
        // ESTADO VISUAL DE CADA TECLA
        GUI.color = Input.GetKey(KeyCode.D) ? Color.green : Color.red;
        GUILayout.Label($"D: {(Input.GetKey(KeyCode.D) ? "PRESIONADA" : "libre")}");
        
        GUI.color = Input.GetKey(KeyCode.F) ? Color.green : Color.red;
        GUILayout.Label($"F: {(Input.GetKey(KeyCode.F) ? "PRESIONADA" : "libre")}");
        
        GUI.color = Input.GetKey(KeyCode.J) ? Color.green : Color.red;
        GUILayout.Label($"J: {(Input.GetKey(KeyCode.J) ? "PRESIONADA" : "libre")}");
        
        GUI.color = Input.GetKey(KeyCode.K) ? Color.green : Color.red;
        GUILayout.Label($"K: {(Input.GetKey(KeyCode.K) ? "PRESIONADA" : "libre")}");
        
        GUI.color = Input.GetKey(KeyCode.L) ? Color.green : Color.red;
        GUILayout.Label($"L: {(Input.GetKey(KeyCode.L) ? "PRESIONADA" : "libre")}");
        
        GUI.color = Color.white;
        
        GUILayout.Space(10);
        
        // CONTADOR DE TECLAS ACTIVAS
        int count = 0;
        if (Input.GetKey(KeyCode.D)) count++;
        if (Input.GetKey(KeyCode.F)) count++;
        if (Input.GetKey(KeyCode.J)) count++;
        if (Input.GetKey(KeyCode.K)) count++;
        if (Input.GetKey(KeyCode.L)) count++;
        
        GUI.color = count >= 2 ? Color.yellow : Color.white;
        GUILayout.Label($"TECLAS ACTIVAS: {count}");
        GUI.color = Color.white;
        
        if (count >= 2)
        {
            GUILayout.Label("🔥 ACORDE DETECTADO 🔥");
        }
        
        GUILayout.Space(10);
        GUILayout.Label("INSTRUCCIONES:");
        GUILayout.Label("1. Mantén presionada D");
        GUILayout.Label("2. Sin soltar D, presiona J");
        GUILayout.Label("3. Sin soltar D ni J, presiona K");
        GUILayout.Label("4. Deberías ver 'ACORDE DETECTADO'");
        
        GUILayout.Space(10);
        mostrarDebugContinuo = GUILayout.Toggle(mostrarDebugContinuo, "Debug continuo");
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
        
        // PANEL DE ESTADO GRANDE
        GUILayout.BeginArea(new Rect(Screen.width - 200, 10, 190, 100));
        GUILayout.BeginVertical("box");
        
        if (count >= 2)
        {
            GUI.color = Color.red;
            GUILayout.Label("🔥 ACORDE ACTIVO 🔥", GUI.skin.box);
            GUILayout.Label($"Teclas: {count}");
        }
        else
        {
            GUI.color = Color.white;
            GUILayout.Label("Esperando acorde...", GUI.skin.box);
        }
        
        GUI.color = Color.white;
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
}
