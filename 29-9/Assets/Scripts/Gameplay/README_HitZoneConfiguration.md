# Configuración de HitZones - Guía de Uso

## Descripción General
Se han creado nuevos scripts para facilitar la configuración del tamaño de los HitZones con estilos predefinidos similares a Fortnite Festival y otros juegos de ritmo populares.

## Scripts Incluidos

### 1. HitZoneSizer.cs
Script principal para configurar tamaños de HitZone con presets predefinidos.

**Características:**
- 5 presets predefinidos: Fortnite Style, Clone Hero, Guitar Hero, Compact, Large
- Configuración personalizada
- Ajuste automático al tamaño de las notas
- Visualización con Gizmos en el editor

### 2. HitZoneConfigurator.cs
Script avanzado con configuración en tiempo real y ajuste de ventanas de hit.

**Características:**
- 7 estilos predefinidos incluyendo RockBand
- Configuración de ventanas de hit (Perfect, Great, Good, OK, Miss)
- Preview en tiempo real
- Escalado de indicadores visuales

### 3. HitZone.cs (Modificado)
Script original mejorado con valores por defecto estilo Fortnite.

**Cambios:**
- Valores por defecto: width=0.4f, height=0.3f (más compactos)
- Método público `UpdateVisualFeedback()` para actualización externa
- Tooltips informativos

## Instrucciones de Uso

### Configuración Rápida (Recomendado)

1. **Agregar HitZoneConfigurator:**
   ```
   - Selecciona el GameObject que tiene el componente HitZone
   - Add Component → HitZoneConfigurator
   - En "Current Style" selecciona "Fortnite"
   - Marca "Auto Configure On Start" y "Live Preview"
   ```

2. **Aplicar estilo Fortnite:**
   ```
   - En el Inspector, cambia "Current Style" a "Fortnite"
   - O usa el botón "Apply Fortnite Style" en el menú contextual
   ```

### Configuración Avanzada

1. **Usar HitZoneSizer para presets simples:**
   ```
   - Add Component → HitZoneSizer
   - Selecciona preset en "Selected Preset Index" (0 = Fortnite)
   - Marca "Auto Apply On Start"
   ```

2. **Configuración personalizada:**
   ```
   - En HitZoneConfigurator, selecciona "Custom" en Current Style
   - Ajusta los valores en "Custom Style"
   - Los cambios se aplican automáticamente con Live Preview
   ```

## Presets Disponibles

### Fortnite Style
- **Tamaño:** 0.35 x 0.25
- **Escala:** 0.7x
- **Descripción:** HitZones compactos similares a Fortnite Festival
- **Ventanas de hit:** Muy precisas (Perfect: 0.04s)

### Clone Hero
- **Tamaño:** 0.6 x 0.6
- **Escala:** 1.0x
- **Descripción:** Tamaño estándar de Clone Hero
- **Ventanas de hit:** Estándar (Perfect: 0.05s)

### Guitar Hero
- **Tamaño:** 0.5 x 0.4
- **Escala:** 0.9x
- **Descripción:** Estilo clásico de Guitar Hero
- **Ventanas de hit:** Clásicas (Perfect: 0.045s)

### Compact
- **Tamaño:** 0.3 x 0.2
- **Escala:** 0.6x
- **Descripción:** HitZones muy pequeños para expertos
- **Ventanas de hit:** Muy estrictas (Perfect: 0.03s)

### Large
- **Tamaño:** 0.8 x 0.8
- **Escala:** 1.3x
- **Descripción:** HitZones grandes para principiantes
- **Ventanas de hit:** Generosas (Perfect: 0.07s)

## Comandos del Menú Contextual

### HitZoneConfigurator
- `Apply Current Style` - Aplica el estilo seleccionado
- `Apply Fortnite Style` - Aplica directamente el estilo Fortnite
- `Apply Clone Hero Style` - Aplica directamente el estilo Clone Hero
- `Apply Compact Style` - Aplica directamente el estilo compacto

### HitZoneSizer
- `Apply Selected Preset` - Aplica el preset seleccionado
- `Apply Fortnite Style` - Aplica preset Fortnite
- `Apply Clone Hero Style` - Aplica preset Clone Hero
- `Apply Compact Style` - Aplica preset compacto

## Configuración Recomendada para Fortnite

```csharp
// Valores recomendados para estilo Fortnite
hitZoneWidth = 0.35f;
hitZoneHeight = 0.25f;
indicatorScale = 0.7f;
perfectWindow = 0.04f;
greatWindow = 0.07f;
goodWindow = 0.1f;
```

## Solución de Problemas

### Los cambios no se aplican
- Verifica que el GameObject tenga el componente HitZone
- Asegúrate de que "Live Preview" esté activado
- Usa "Apply Current Style" manualmente

### Los indicadores visuales no cambian de tamaño
- Verifica que laneIndicators esté asignado en HitZone
- Los GameObjects de laneIndicators deben existir en la escena

### Las ventanas de hit no funcionan
- Verifica que GameplayManager esté configurado correctamente
- Las ventanas de hit se validan automáticamente (Perfect < Great < Good < OK < Miss)

## Integración con Sistema Existente

Los nuevos scripts son completamente compatibles con:
- ✅ Sistema de detección de notas existente
- ✅ GameplayManager y eventos de hit/miss
- ✅ Sistema de destrucción de notas
- ✅ ChartParser y NoteSpawner
- ✅ Efectos visuales y de audio

## Notas Técnicas

- Los valores se validan automáticamente en OnValidate()
- Los Gizmos muestran el área de los HitZones en el editor
- Los cambios se aplican en tiempo real durante el gameplay
- Compatible con el sistema de memoria existente
