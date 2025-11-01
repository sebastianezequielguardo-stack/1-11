using UnityEngine;
using UnityEngine.UI;

public class UIButtonSpriteSwitcher : MonoBehaviour
{
    [System.Serializable]
    public class ButtonSpritePair
    {
        public KeyCode key;
        public Image imageComponent;
        public Sprite normalSprite;
        public Sprite pressedSprite;
    }

    public ButtonSpritePair[] buttons;

    void Update()
    {
        foreach (var button in buttons)
        {
            if (button.imageComponent == null) continue;

            if (Input.GetKeyDown(button.key) && button.pressedSprite != null)
            {
                button.imageComponent.sprite = button.pressedSprite;
            }
            else if (Input.GetKeyUp(button.key) && button.normalSprite != null)
            {
                button.imageComponent.sprite = button.normalSprite;
            }
        }
    }
}