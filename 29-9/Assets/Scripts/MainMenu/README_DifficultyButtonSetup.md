# Configuración de Botones de Dificultad

## Problema Solucionado
Los botones de dificultad "FACIL" y "DIFICIL" aparecían seleccionados (verdes) desde el inicio del juego, cuando deberían empezar sin seleccionar y solo uno puede estar activo a la vez.

## Solución Implementada
Se creó un nuevo script `DifficultyButtonManager.cs` que maneja correctamente el comportamiento de los botones de dificultad.

## Pasos para Configurar en Unity

### 1. Agregar el Script DifficultyButtonManager
1. En la escena MainMenu, crea un GameObject vacío llamado "DifficultyButtonManager"
2. Agrega el componente `DifficultyButtonManager` a este GameObject

### 2. Configurar las Referencias en el Inspector
En el componente DifficultyButtonManager, asigna las siguientes referencias:

**Difficulty Buttons:**
- `Facil Button`: Arrastra el botón "FACIL" desde la jerarquía
- `Dificil Button`: Arrastra el botón "DIFICIL" desde la jerarquía

**Visual Settings:**
- `Selected Color`: Verde (Color.green) - Color cuando el botón está seleccionado
- `Unselected Color`: Blanco (Color.white) - Color cuando el botón NO está seleccionado
- `Hover Color`: Gris (Color.gray) - Color cuando el mouse está sobre el botón

**Play Button:**
- `Play Button`: Arrastra el botón "JUGAR" desde la jerarquía

### 3. Configurar los Botones en Unity
Para cada botón de dificultad (FACIL y DIFICIL):

1. Selecciona el botón en la jerarquía
2. En el componente Button, ve a la sección "On Click ()"
3. **IMPORTANTE**: Elimina cualquier referencia existente a `MainMenu.SetFacil()` o `MainMenu.SetDificil()`
4. Los eventos onClick ahora son manejados automáticamente por el DifficultyButtonManager

### 4. Verificar el Botón de Jugar
1. Selecciona el botón "JUGAR" 
2. Asegúrate de que su evento onClick esté configurado para llamar a `MainMenu.Jugar()`

## Comportamiento Esperado

### Al Iniciar el Juego:
- ✅ Ambos botones de dificultad aparecen en color blanco (no seleccionados)
- ✅ El botón "JUGAR" está deshabilitado (gris)

### Al Seleccionar Solo Canción:
- ✅ Los botones de dificultad siguen blancos (no seleccionados)
- ✅ El botón "JUGAR" permanece gris (CRÍTICO: NO se pone verde)

### Al Seleccionar Solo Dificultad:
- ✅ El botón seleccionado se vuelve verde
- ✅ El otro botón se vuelve blanco
- ✅ El botón "JUGAR" permanece gris (falta la canción)

### Al Seleccionar Canción Y Dificultad:
- ✅ El botón "JUGAR" se habilita (verde) - SOLO cuando ambas condiciones se cumplen
- ✅ Se puede iniciar el juego

## Archivos Modificados
- **Nuevo**: `DifficultyButtonManager.cs` - Maneja la lógica de los botones
- **Modificado**: `MainMenu.cs` - Comentarios de compatibilidad agregados
- **Nuevo**: `README_DifficultyButtonSetup.md` - Esta guía de configuración

## Notas Técnicas
- El script se integra automáticamente con el GameManager existente
- Mantiene compatibilidad con el sistema de eventos existente
- Los métodos `SetFacil()` y `SetDificil()` en MainMenu siguen funcionando pero muestran advertencias
- El estado de los botones se actualiza automáticamente cuando se selecciona una canción
- **CRÍTICO**: El script usa `LateUpdate()` para forzar el estado correcto del botón JUGAR y evitar interferencias de otros scripts
- Escucha eventos del GameManager para mantener sincronización pero siempre aplica la lógica correcta (canción Y dificultad)

## Solución de Problemas

### Si los botones siguen apareciendo verdes:
1. Verifica que el DifficultyButtonManager esté asignado correctamente
2. Asegúrate de que los eventos onClick de los botones no tengan referencias a los métodos antiguos
3. Reinicia la escena para ver los cambios

### Si el botón JUGAR no se habilita:
1. Verifica que la referencia al Play Button esté asignada
2. Asegúrate de que tanto la canción como la dificultad estén seleccionadas
3. Revisa la consola para mensajes de debug
