using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIButtonController : MonoBehaviour
{
    [System.Serializable]
    public class ButtonInfo
    {
        public KeyCode key;
        public Image buttonImage;
        public Sprite normalSprite;
        public Sprite pressedSprite;
        public Color pressedColor = new Color(0.8f, 0.8f, 0.8f, 1f);
    }

    public List<ButtonInfo> buttons = new List<ButtonInfo>();
    private Dictionary<KeyCode, ButtonInfo> buttonMap = new Dictionary<KeyCode, ButtonInfo>();

    void Start()
    {
        // Crear el mapeo de teclas
        foreach (var btn in buttons)
        {
            if (btn.buttonImage != null && btn.normalSprite != null)
            {
                buttonMap[btn.key] = btn;
                btn.buttonImage.sprite = btn.normalSprite;
            }
        }
    }

    void Update()
    {
        foreach (var btn in buttons)
        {
            if (Input.GetKeyDown(btn.key))
            {
                if (buttonMap.TryGetValue(btn.key, out var buttonInfo))
                {
                    buttonInfo.buttonImage.sprite = buttonInfo.pressedSprite;
                    buttonInfo.buttonImage.color = buttonInfo.pressedColor;
                }
            }
            else if (Input.GetKeyUp(btn.key))
            {
                if (buttonMap.TryGetValue(btn.key, out var buttonInfo))
                {
                    buttonInfo.buttonImage.sprite = buttonInfo.normalSprite;
                    buttonInfo.buttonImage.color = Color.white;
                }
            }
        }
    }
}