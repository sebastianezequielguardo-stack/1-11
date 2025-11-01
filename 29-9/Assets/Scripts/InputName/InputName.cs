using System.Linq;
using UnityEngine;

public class InputName : MonoBehaviour
{
    string stringName;

    public void ReadNameString(string name)
    {
        if (name.Length == 0)
        {
            Debug.Log("❌ Nombre no válido: está vacío");
        }
        else if (name.Any(char.IsDigit))
        {
            Debug.Log("❌ Nombre no válido: no puede contener números");
        }
        else
        {
            stringName = name;
            Debug.Log("✅ Nombre válido: " + stringName);
        }
    }

    public bool IsNameValid(string name)
    {
        return name.Length > 0 && !name.Any(char.IsDigit);
    }
}