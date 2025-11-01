using UnityEngine;

/// <summary>
/// Script para corregir automáticamente los warnings de FindObjectOfType obsoleto
/// </summary>
public static class DeprecationFixer
{
    // Métodos de reemplazo para FindObjectOfType
    public static T FindFirstObjectByType<T>() where T : Object
    {
        #if UNITY_2023_1_OR_NEWER
            return Object.FindFirstObjectByType<T>();
        #else
            return Object.FindFirstObjectByType<T>();
        #endif
    }
    
    public static T FindAnyObjectByType<T>() where T : Object
    {
        #if UNITY_2023_1_OR_NEWER
            return Object.FindAnyObjectByType<T>();
        #else
            return Object.FindFirstObjectByType<T>();
        #endif
    }
    
    // Métodos de reemplazo para FindObjectsOfType
    public static T[] FindObjectsByType<T>(FindObjectsSortMode sortMode = FindObjectsSortMode.None) where T : Object
    {
        #if UNITY_2023_1_OR_NEWER
            return Object.FindObjectsByType<T>(sortMode);
        #else
            return Object.FindObjectsByType<T>(FindObjectsSortMode.None);
        #endif
    }
}
