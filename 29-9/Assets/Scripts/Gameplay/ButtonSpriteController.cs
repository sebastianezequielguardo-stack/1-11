using UnityEngine;
using UnityEngine.UI;

public class ButtonSpriteController : MonoBehaviour
{
    [System.Serializable]
    public class ButtonSprite
    {
        public KeyCode key;
        public Image imageComponent;
        public Sprite normalSprite;
        public Sprite pressedSprite;
        [Range(0.8f, 1.2f)]
        public float pressScale = 0.9f;
    }

    public ButtonSprite[] buttons;
    private Vector3[] originalScales;

    void Start()
    {
        originalScales = new Vector3[buttons.Length];

        // Guardar escalas originales
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i].imageComponent != null)
            {
                originalScales[i] = buttons[i].imageComponent.transform.localScale;

                // Asegurar que empiecen con el sprite normal
                if (buttons[i].normalSprite != null)
                {
                    buttons[i].imageComponent.sprite = buttons[i].normalSprite;
                }
            }
        }
    }

    void Update()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i].imageComponent == null) continue;

            if (Input.GetKeyDown(buttons[i].key))
            {
                // Cambiar a sprite presionado
                if (buttons[i].pressedSprite != null)
                {
                    buttons[i].imageComponent.sprite = buttons[i].pressedSprite;
                }
                // Efecto de escala
                buttons[i].imageComponent.transform.localScale = originalScales[i] * buttons[i].pressScale;
            }
            else if (Input.GetKeyUp(buttons[i].key))
            {
                // Volver a sprite normal
                if (buttons[i].normalSprite != null)
                {
                    buttons[i].imageComponent.sprite = buttons[i].normalSprite;
                }
                // Restaurar escala
                buttons[i].imageComponent.transform.localScale = originalScales[i];
            }
        }
    }
}