using UnityEngine;

public class SetupHitEffects : MonoBehaviour
{
    void Start()
    {
        InputManager inputManager = FindFirstObjectByType<InputManager>();

        if (inputManager == null)
        {
            Debug.LogError("❌ InputManager no encontrado");
            return;
        }

        // Busca los botones por nombre
        GameObject[] effects = new GameObject[5];
        effects[0] = GameObject.Find("BotonVerde");
        effects[1] = GameObject.Find("BotonRojo");
        effects[2] = GameObject.Find("BotonAzul");
        effects[3] = GameObject.Find("BotonAmarillo");
        effects[4] = GameObject.Find("BotonRosa");

        // Verifica que todos existan
        bool todosEncontrados = true;
        for (int i = 0; i < effects.Length; i++)
        {
            if (effects[i] == null)
            {
                Debug.LogError($"❌ No se encontró el botón en índice {i}");
                todosEncontrados = false;
            }
            else
            {
                Debug.Log($"✅ Botón {i} encontrado: {effects[i].name}");
            }
        }

        if (!todosEncontrados)
        {
            Debug.LogError("❌ No se encontraron todos los botones");
            return;
        }

        // Asigna al InputManager
        var field = typeof(InputManager).GetField("hitEffects",
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance);

        if (field != null)
        {
            field.SetValue(inputManager, effects);
            Debug.Log("🎉 ¡Hit Effects asignados correctamente!");
        }
        else
        {
            Debug.LogError("❌ No se pudo acceder al campo hitEffects");
        }

        // Auto-destruye este script
        Destroy(this);
    }
}