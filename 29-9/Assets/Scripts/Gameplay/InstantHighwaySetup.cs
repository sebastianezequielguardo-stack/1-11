using UnityEngine;

/// <summary>
/// Configuración instantánea del highway metálico
/// Solo agregar a la escena y funciona inmediatamente
/// </summary>
public class InstantHighwaySetup : MonoBehaviour
{
    void Start()
    {
        SetupInstantHighway();
    }
    
    /// <summary>
    /// Configuración instantánea del highway
    /// </summary>
    void SetupInstantHighway()
    {
        Debug.Log("⚡ Configuración instantánea del highway metálico...");
        
        // Verificar si ya existe MetallicHighwayCreator
        MetallicHighwayCreator existingCreator = FindFirstObjectByType<MetallicHighwayCreator>();
        if (existingCreator != null)
        {
            Debug.Log("✅ MetallicHighwayCreator ya existe, creando highway...");
            existingCreator.CreateMetallicHighway();
        }
        else
        {
            // Crear nuevo MetallicHighwayCreator
            GameObject creatorObj = new GameObject("MetallicHighwayCreator");
            MetallicHighwayCreator creator = creatorObj.AddComponent<MetallicHighwayCreator>();
            
            // Configurar parámetros para que coincida con tu imagen
            creator.createOnStart = false; // Ya lo crearemos manualmente
            creator.replaceExistingHighway = true;
            
            // Configurar posición y escala
            creator.highwayPosition = new Vector3(0f, -0.1f, 0f);
            creator.highwayRotation = new Vector3(90f, 0f, 0f);
            creator.highwayScale = new Vector3(10f, 50f, 1f);
            
            // Configurar colores metálicos basados en tu imagen
            creator.mainMetalColor = new Color(0.7f, 0.7f, 0.75f, 1f); // Gris metálico principal
            creator.edgeColor = new Color(0.4f, 0.4f, 0.45f, 1f); // Bordes más oscuros
            creator.centerLineColor = new Color(0.3f, 0.3f, 0.35f, 1f); // Línea central oscura
            creator.metallic = 0.8f;
            creator.smoothness = 0.6f;
            
            // Configurar detalles
            creator.addCenterLine = true;
            creator.addEdgeLines = true;
            creator.centerLineWidth = 0.15f;
            creator.edgeLineWidth = 0.08f;
            
            Debug.Log("✅ MetallicHighwayCreator configurado");
            
            // Crear highway inmediatamente
            creator.CreateMetallicHighway();
        }
        
        Debug.Log("🎉 ¡Highway metálico creado instantáneamente!");
        Debug.Log("🛣️ Highway rectangular con apariencia metálica aplicado");
        Debug.Log("💡 Basado en la imagen que proporcionaste");
        
        // Auto-destruir este script después de la configuración
        Destroy(this);
    }
}
