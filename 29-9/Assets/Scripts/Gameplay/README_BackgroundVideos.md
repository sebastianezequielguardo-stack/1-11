# Configuración de Videos de Fondo

## ✅ Videos WebM Cargados - ¿Cómo Activarlos?

Ya tienes los videos en formato WebM. Ahora necesitas configurar el sistema para que se reproduzcan automáticamente durante el gameplay.

## 🚀 Configuración Rápida (3 Pasos)

### Paso 1: Agregar Scripts a la Escena de Gameplay
1. **Agregar `BackgroundVideoManager`** a cualquier GameObject en la escena de gameplay
2. **Agregar `VideoSetupHelper`** al mismo GameObject (opcional, para ayuda)

### Paso 2: Configurar GameplayManager
1. En el Inspector del **GameplayManager**
2. Asignar el **BackgroundVideoManager** al campo correspondiente
3. ¡Listo! Los videos se cargarán automáticamente

### Paso 3: Organizar Archivos WebM
Coloca tus videos WebM en las carpetas de canciones:
```
StreamingAssets/
└── Songs/
    └── [NombreCancion]/
        ├── song.ogg
        ├── notes.chart
        └── background.webm  ← Tu video aquí
```

## 📂 Estructura de Archivos

### Nombres de Video Válidos
El sistema busca automáticamente estos nombres:
- `background.webm` (recomendado)
- `video.webm`
- `bg.webm` 
- `movie.webm`
- Cualquier archivo `.webm` en la carpeta

### Formatos Soportados
- ✅ **WebM** (recomendado - mejor rendimiento)
- ✅ MP4
- ✅ MOV
- ✅ AVI

## ⚙️ Configuración Avanzada

### BackgroundVideoManager Settings
```csharp
[Header("Video Configuration")]
public bool enableBackgroundVideo = true;     // Activar/desactivar videos
public float videoOpacity = 0.7f;            // Transparencia (0.1 - 1.0)
public Vector3 videoPosition = (0, 0, 20);   // Posición detrás del highway
public Vector3 videoScale = (20, 15, 1);     // Tamaño del video
```

### Configuración Recomendada
- **Opacidad**: 0.7 (70% visible, no interfiere con gameplay)
- **Posición Z**: 20 (bien detrás del highway)
- **Escala**: 20x15 (cubre toda la pantalla)

## 🎮 Funcionalidades Automáticas

### Sincronización con Gameplay
- ▶️ **Inicia** automáticamente con la canción
- ⏸️ **Pausa** cuando pausas el juego
- ▶️ **Resume** cuando reanudas
- ⏹️ **Para** cuando termina la canción

### Detección Automática
- Busca videos en la carpeta de la canción seleccionada
- Carga el primer video encontrado
- Fallback silencioso si no hay video

## 🛠️ Scripts Creados

### 1. BackgroundVideoManager.cs
**Funcionalidad principal:**
- Carga y reproduce videos de fondo
- Sincronización con GameplayManager
- Control de transparencia y posición
- Soporte para múltiples formatos

### 2. VideoSetupHelper.cs
**Herramientas de configuración:**
- Configuración automática
- Información de setup
- Testing de videos
- Diagnóstico de archivos

### 3. GameplayManager.cs (Modificado)
**Integración agregada:**
- Carga automática de videos
- Control de pausa/resume
- Sincronización con audio

## 🎯 Comandos de Testing

### VideoSetupHelper Commands
- **"Setup Background Video"** - Configuración automática
- **"Show Video Setup Info"** - Información detallada
- **"Test Video Loading"** - Probar carga de video
- **"Create Example Song Structure"** - Crear estructura de ejemplo

### Teclas de Acceso Rápido (en gameplay)
- **V** - Test video loading
- **I** - Show setup info

## 📋 Checklist de Configuración

### ✅ Verificaciones Necesarias
1. **BackgroundVideoManager** agregado a la escena
2. **GameplayManager.backgroundVideoManager** asignado
3. **Videos WebM** en carpetas de canciones correctas
4. **enableBackgroundVideo = true** en BackgroundVideoManager

### 🔧 Solución de Problemas

#### Video no aparece:
1. Verificar que el archivo esté en la carpeta correcta
2. Comprobar que `enableBackgroundVideo = true`
3. Revisar Console para errores de carga
4. Usar "Test Video Loading" para diagnóstico

#### Video muy opaco/transparente:
1. Ajustar `videoOpacity` (0.1 - 1.0)
2. Valores recomendados: 0.5-0.8

#### Video mal posicionado:
1. Ajustar `videoPosition` (Z debe ser > 15)
2. Ajustar `videoScale` según necesidad

#### Video no sincroniza:
1. Verificar que GameplayManager tenga referencia
2. Comprobar que los métodos de pausa/resume funcionen

## 🎨 Configuración Visual Recomendada

### Para Estilo Fortnite Festival
```csharp
videoOpacity = 0.6f;           // Sutil pero visible
videoPosition = (0, 0, 25);    // Bien atrás
videoScale = (25, 18, 1);      // Pantalla completa
```

### Para Estilo Clone Hero
```csharp
videoOpacity = 0.8f;           // Más prominente
videoPosition = (0, 0, 20);    // Posición estándar
videoScale = (20, 15, 1);      // Tamaño estándar
```

## 📝 Notas Importantes

- Los videos se reproducen en **loop** automáticamente
- **Sin audio** - solo el audio de la canción se reproduce
- **Render detrás** del highway y notas
- **Optimizado** para no afectar el rendimiento del juego
- **Compatible** con todos los sistemas existentes

## 🚀 Resultado Final

Con esta configuración tendrás:
- ✅ Videos de fondo automáticos durante gameplay
- ✅ Sincronización perfecta con audio
- ✅ Control de pausa/resume
- ✅ Transparencia configurable
- ✅ Detección automática de archivos
- ✅ Experiencia visual profesional similar a Fortnite Festival

¡Los videos WebM que cargaste se reproducirán automáticamente cuando juegues las canciones!
