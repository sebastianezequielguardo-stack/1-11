using UnityEngine;
using System.IO.Ports;
using System.Collections.Generic;

public class ESPADA : MonoBehaviour
{
    [Header("Configuración Serial")]
    public string puertoSerial = "COM4";
    public int baudRate = 9600;

    private SerialPort puerto;
    private InputManager inputManager;

    private int GetLaneIndex(string buttonName)
    {
        switch (buttonName)
        {
            case "D": return 0; // ← Cambiado!
            case "F": return 1; // ← Cambiado!
            case "J": return 2; // ← Cambiado!
            case "K": return 3; // ← Cambiado!
            case "L": return 4; // ← Cambiado!
                                // También acepta el formato largo por si lo cambias luego
            case "BUTTON_D": return 0;
            case "BUTTON_F": return 1;
            case "BUTTON_J": return 2;
            case "BUTTON_K": return 3;
            case "BUTTON_L": return 4;
            default: return -1;
        }
    }

    void Start()
    {
        Debug.Log("🚀 ESPADA: Start() iniciando...");

        // Lista todos los puertos disponibles
        string[] puertos = SerialPort.GetPortNames();
        Debug.Log("📋 Puertos COM disponibles: " + string.Join(", ", puertos));

        try
        {
            Debug.Log("🔌 Intentando abrir puerto: " + puertoSerial + " a " + baudRate + " baud");
            puerto = new SerialPort(puertoSerial, baudRate);
            puerto.ReadTimeout = 50; // Timeout más corto
            puerto.NewLine = "\n"; // Especifica el terminador de línea
            puerto.Open();
            Debug.Log("✅ Puerto " + puertoSerial + " ABIERTO CORRECTAMENTE");
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("⚠️ Puerto " + puertoSerial + " no disponible: " + ex.Message);
            Debug.Log("🎮 Continuando con entrada de teclado únicamente");
            puerto = null; // Marcar como no disponible pero no desactivar el componente
        }

        Debug.Log("🔍 Buscando InputManager...");
        inputManager = FindFirstObjectByType<InputManager>();

        if (inputManager == null)
        {
            Debug.LogError("❌ NO SE ENCONTRÓ InputManager!");
            enabled = false;
        }
        else
        {
            Debug.Log("✅ InputManager ENCONTRADO Y CONECTADO!");
            
            // Mostrar estado del sistema de entrada
            if (puerto != null && puerto.IsOpen)
            {
                Debug.Log("🗡️ Sistema de entrada: ESPADA (prioridad) + Teclado (respaldo)");
            }
            else
            {
                Debug.Log("⌨️ Sistema de entrada: TECLADO únicamente");
            }
        }
    }

    void Update()
    {
        if (puerto == null || !puerto.IsOpen || inputManager == null)
            return;

        try
        {
            if (puerto.BytesToRead > 0)
            {
                string todosLosDatos = puerto.ReadExisting();
                string[] lineas = todosLosDatos.Split(new char[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);

                foreach (string linea in lineas)
                {
                    string comando = linea.Trim();

                    // Detectar si es DOWN o UP
                    if (comando.EndsWith("_DOWN"))
                    {
                        string tecla = comando.Replace("_DOWN", "");
                        int lane = GetLaneIndex(tecla);

                        if (lane >= 0)
                        {
                            Debug.Log($"✅ Botón {lane} PRESIONADO");
                            inputManager.SimulateButtonPress(lane, true);  // ← Presionado
                        }
                    }
                    else if (comando.EndsWith("_UP"))
                    {
                        string tecla = comando.Replace("_UP", "");
                        int lane = GetLaneIndex(tecla);

                        if (lane >= 0)
                        {
                            Debug.Log($"🔽 Botón {lane} LIBERADO");
                            inputManager.SimulateButtonPress(lane, false);  // ← Soltado
                        }
                    }
                    else
                    {
                        // Formato viejo (solo letra)
                        int lane = GetLaneIndex(comando);
                        if (lane >= 0)
                        {
                            Debug.Log($"✅ Formato antiguo - Lane {lane}");
                            inputManager.SimulateButtonPress(lane);  // Mantiene compatibilidad
                        }
                    }
                }
            }
        }
        catch (System.TimeoutException) { }
        catch (System.Exception ex)
        {
            Debug.LogError("❌ ERROR: " + ex.Message);
        }
    }
    
    /// <summary>
    /// Verifica si la espada está conectada y funcionando
    /// </summary>
    public bool IsEspadaConnected()
    {
        return puerto != null && puerto.IsOpen;
    }
    
    /// <summary>
    /// Intenta reconectar la espada
    /// </summary>
    [ContextMenu("Intentar Reconectar Espada")]
    public void TryReconnectEspada()
    {
        if (IsEspadaConnected())
        {
            Debug.Log("✅ La espada ya está conectada");
            return;
        }
        
        Debug.Log("🔄 Intentando reconectar espada...");
        
        // Cerrar puerto anterior si existe
        if (puerto != null)
        {
            try { puerto.Close(); } catch { }
            puerto = null;
        }
        
        // Intentar abrir nuevamente
        try
        {
            puerto = new SerialPort(puertoSerial, baudRate);
            puerto.ReadTimeout = 50;
            puerto.NewLine = "\n";
            puerto.Open();
            Debug.Log("✅ Espada reconectada exitosamente!");
            Debug.Log("🗡️ Sistema de entrada: ESPADA (prioridad) + Teclado (respaldo)");
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("⚠️ No se pudo reconectar: " + ex.Message);
            Debug.Log("⌨️ Continuando con teclado únicamente");
            puerto = null;
        }
    }
    
    /// <summary>
    /// Obtiene información del estado actual
    /// </summary>
    [ContextMenu("Mostrar Estado del Sistema")]
    public void ShowSystemStatus()
    {
        Debug.Log("📊 ESTADO DEL SISTEMA DE ENTRADA:");
        Debug.Log("═══════════════════════════════");
        
        if (IsEspadaConnected())
        {
            Debug.Log("🗡️ Espada: CONECTADA");
            Debug.Log($"📡 Puerto: {puertoSerial} @ {baudRate} baud");
            Debug.Log("⌨️ Teclado: Disponible como respaldo");
            Debug.Log("🎮 Modo: HÍBRIDO (Espada tiene prioridad)");
        }
        else
        {
            Debug.Log("🗡️ Espada: DESCONECTADA");
            Debug.Log("⌨️ Teclado: ACTIVO (modo principal)");
            Debug.Log("🎮 Modo: SOLO TECLADO");
        }
        
        Debug.Log($"🎯 InputManager: {(inputManager != null ? "Conectado" : "No encontrado")}");
        
        // Mostrar puertos disponibles
        string[] puertos = SerialPort.GetPortNames();
        Debug.Log($"📋 Puertos COM disponibles: {string.Join(", ", puertos)}");
    }
    
    void OnDestroy()
    {
        // Cerrar puerto al destruir el objeto
        if (puerto != null && puerto.IsOpen)
        {
            try
            {
                puerto.Close();
                Debug.Log("🔌 Puerto cerrado correctamente");
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("⚠️ Error cerrando puerto: " + ex.Message);
            }
        }
    }
}