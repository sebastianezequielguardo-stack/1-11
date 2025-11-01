using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ControllerButtonContinuar : MonoBehaviour
{
    [Header("Referencias")]
    public TMP_InputField inputField;   // Arrastrá tu TMP_InputField
    public Button miBoton;              // Arrastrá tu Botón
    public InputName inputNameValidator; // NUEVO: Referencia al script InputName
    public string nombreEscena = "TuEscena"; // Cambiá por el nombre de tu escena

    [Header("Colores del botón")]
    public Color colorHabilitado = Color.white;
    public Color colorDeshabilitado = Color.gray;

    private Image botonImagen;

    void Start()
    {
        // Verificación de referencias
        if (inputField == null || miBoton == null)
        {
            Debug.LogError("⚠️ Asigná el InputField y el Botón en el Inspector.");
            return;
        }

        botonImagen = miBoton.GetComponent<Image>();

        // Desactivar el botón al inicio
        VerificarInput(inputField.text);

        // Escuchar cambios de texto
        inputField.onValueChanged.RemoveAllListeners();
        inputField.onValueChanged.AddListener(VerificarInput);

        // Asignar acción del botón
        miBoton.onClick.RemoveAllListeners();
        miBoton.onClick.AddListener(CambiarEscena);
    }

    public void VerificarInput(string texto)
    {
        bool hayTextoValido = !string.IsNullOrWhiteSpace(texto);
        miBoton.interactable = hayTextoValido;

        if (botonImagen != null)
            botonImagen.color = hayTextoValido ? colorHabilitado : colorDeshabilitado;
    }

    private void CambiarEscena()
    {
        if (!miBoton.interactable)
        {
            Debug.Log("❌ El botón está desactivado, no se puede cambiar de escena.");
            return;
        }

        if (string.IsNullOrEmpty(nombreEscena))
        {
            Debug.LogWarning("⚠️ No se especificó el nombre de la escena.");
            return;
        }

        // NUEVO: Guardar el nombre antes de cambiar de escena
        string nombreJugador = inputField.text.Trim();

        if (!string.IsNullOrEmpty(nombreJugador))
        {
            PlayerPrefs.SetString("PlayerName", nombreJugador);
            PlayerPrefs.Save();
            Debug.Log("✅ Nombre guardado: " + nombreJugador);
        }

        Debug.Log("➡️ Cargando escena: " + nombreEscena);
        SceneManager.LoadScene(nombreEscena);
    }
}