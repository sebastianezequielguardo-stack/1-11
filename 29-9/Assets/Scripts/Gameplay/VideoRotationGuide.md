# 🔄 Guía de Rotación de Videos de Fondo

## ✅ Nueva Funcionalidad Agregada

He agregado **controles completos de rotación** al BackgroundVideoSystem para que puedas rotar los videos de fondo según necesites.

## 🎛️ **Controles en el Inspector**

### **En BackgroundVideoSystem:**
```
Display Settings:
├── Video Position: (X, Y, Z)
├── Video Rotation: (X, Y, Z)  ← ¡NUEVO!
└── Video Scale: (X, Y, Z)
```

### **Valores de Rotación:**
- **X**: Rotación hacia adelante/atrás (pitch)
- **Y**: Rotación izquierda/derecha (yaw)  
- **Z**: Rotación horaria/antihoraria (roll) - **Más común**

## 🎮 **Uso en el Inspector**

### **Rotaciones Comunes:**
```
Sin rotación:     (0, 0, 0)
90° horario:      (0, 0, 90)
90° antihorario:  (0, 0, -90)
180° volteado:    (0, 0, 180)
Vertical:         (90, 0, 0)
```

### **Para tu caso específico:**
Basándome en tu imagen, puedes usar:
```
Video Rotation: (30, 0, 0)  // Inclinación hacia adelante
```

## 💻 **Control por Código**

### **Métodos Disponibles:**
```csharp
BackgroundVideoSystem videoSystem = FindObjectOfType<BackgroundVideoSystem>();

// Configurar rotación completa
videoSystem.SetVideoRotation(30f, 0f, 0f);

// O usar Vector3
videoSystem.SetVideoRotation(new Vector3(30f, 0f, 0f));

// Solo rotar en Z (más común)
videoSystem.SetVideoRotationZ(90f);

// Actualizar todo el transform
videoSystem.UpdateVideoTransform();
```

### **Ejemplo de Uso:**
```csharp
void Start()
{
    BackgroundVideoSystem videoSystem = FindObjectOfType<BackgroundVideoSystem>();
    
    // Rotar video 30 grados en X como en tu imagen
    videoSystem.SetVideoRotation(30f, 0f, 0f);
    
    // O configurar todo junto
    videoSystem.videoPosition = new Vector3(0f, -16f, 30f);
    videoSystem.videoRotation = new Vector3(30f, 0f, 0f);
    videoSystem.videoScale = new Vector3(92f, 60f, 1f);
    videoSystem.UpdateVideoTransform();
}
```

## 🔄 **Actualización en Tiempo Real**

### **Cambios en el Inspector:**
- ✅ **Automático** - Los cambios se aplican inmediatamente
- ✅ **Durante gameplay** - Puedes ajustar mientras juegas
- ✅ **Sin reiniciar** - No necesitas parar el video

### **Animación de Rotación:**
```csharp
// Ejemplo para rotar gradualmente
IEnumerator RotateVideo()
{
    BackgroundVideoSystem videoSystem = FindObjectOfType<BackgroundVideoSystem>();
    
    for (float angle = 0f; angle <= 360f; angle += 1f)
    {
        videoSystem.SetVideoRotationZ(angle);
        yield return new WaitForSeconds(0.01f);
    }
}
```

## 🎯 **Casos de Uso Comunes**

### **📱 Videos Verticales (Portrait):**
```csharp
videoSystem.SetVideoRotation(0f, 0f, 90f);  // Rotar 90°
videoSystem.videoScale = new Vector3(40f, 60f, 1f);  // Ajustar escala
```

### **🔄 Videos Volteados:**
```csharp
videoSystem.SetVideoRotation(0f, 0f, 180f);  // Voltear completamente
```

### **📐 Videos Inclinados:**
```csharp
videoSystem.SetVideoRotation(15f, 0f, 0f);  // Inclinación sutil
```

### **🎬 Efecto Cinematográfico:**
```csharp
videoSystem.SetVideoRotation(5f, 0f, -2f);  // Rotación ligera para dinamismo
```

## 🛠️ **Configuración Recomendada**

### **Para tu Setup Actual:**
Basándome en tu imagen:
```
Position: (0, -16, 30)
Rotation: (30, 0, 0)     ← Inclinación hacia adelante
Scale: (92, 60, 1)
```

### **Alternativas Interesantes:**
```
Clásico:      Rotation: (0, 0, 0)
Dinámico:     Rotation: (10, 0, -5)
Cinematográfico: Rotation: (5, 0, 2)
Vertical:     Rotation: (0, 0, 90)
```

## 🔧 **Solución de Problemas**

### **Video se ve cortado después de rotar:**
- ✅ Aumentar la escala: `videoScale = new Vector3(100f, 80f, 1f)`
- ✅ Ajustar posición: `videoPosition.z` más lejos

### **Rotación no se aplica:**
- ✅ Verificar que `videoQuad` esté creado
- ✅ Llamar `UpdateVideoTransform()` manualmente
- ✅ Verificar que `enableBackgroundVideo = true`

### **Performance con rotación:**
- ✅ **Sin impacto** - La rotación es solo transform
- ✅ **Optimizado** - Se actualiza solo cuando cambia
- ✅ **Tiempo real** - Cambios instantáneos

## 🎮 **Integración Completa**

El sistema de rotación está **completamente integrado**:
- ✅ **Inspector** - Controles visuales
- ✅ **Código** - Métodos públicos
- ✅ **Tiempo real** - Actualización automática
- ✅ **Persistente** - Se mantiene durante gameplay

¡Ahora puedes rotar tus videos de fondo exactamente como necesites!
