# Textured Hit Zone Buttons

## Overview
This system replaces the simple colored cylinder indicators with textured 2D button sprites that work in the 3D gameplay environment, matching the visual style shown in the reference image.

## Components

### TexturedHitZoneIndicators.cs
- **Purpose**: Creates textured visual indicators for hit zone buttons using 2D sprites in 3D space
- **Features**:
  - Loads button textures from Resources folder automatically
  - Supports base and pressed states for each button
  - Handles visual feedback for key presses
  - Integrates with existing input system
  - Provides hit effects when notes are successfully hit

### HitZoneButtonIntegrator.cs
- **Purpose**: Integrates the textured buttons with the existing hit zone system
- **Features**:
  - Connects TexturedHitZoneIndicators with HitZone and InputManager
  - Provides methods for triggering button effects
  - Can be extended for additional integration features

## Button Assets
The system uses the following button textures from `Assets/Resources/`:

**Base States:**
- `boton verde base.png` - Green button (Lane 0)
- `boton rojo base.png` - Red button (Lane 1)  
- `boton amarillo base.png` - Yellow button (Lane 2)
- `boton azul base.png` - Blue button (Lane 3)
- `boton rosa base.png` - Pink button (Lane 4)

**Pressed States:**
- `boton verde pulsado.png` - Green button pressed
- `boton rojo pulsado.png` - Red button pressed
- `boton amarillo pulsado.png` - Yellow button pressed
- `boton azul pulsado.png` - Blue button pressed
- `boton rosa pulsado.png` - Pink button pressed

## Setup Instructions

1. **Scene Configuration**: The `Gameplay.unity` scene has been updated to use `TexturedHitZoneIndicators` instead of the old `HitZoneIndicators`

2. **Key Bindings**: The system uses the same key bindings as the original:
   - Lane 0 (Green): D key
   - Lane 1 (Red): F key
   - Lane 2 (Yellow): J key
   - Lane 3 (Blue): K key
   - Lane 4 (Pink): L key

3. **Visual Settings**:
   - `indicatorSize`: Controls the size of the button indicators (default: 1.2)
   - `buttonDepth`: Controls the thickness of the 3D buttons (default: 0.1)
   - `showKeyLabels`: Enable/disable key letter labels on buttons (default: false)

## Integration with Existing Systems

### HitZone Integration
- The `HitZone.cs` script has been updated to work with the new textured indicators
- When a note is hit, it triggers the appropriate button's hit effect
- Visual feedback is synchronized between the old sprite system and new textured buttons

### Input System
- The textured buttons respond to the same input as the original system
- Key presses switch between base and pressed button textures
- Visual feedback includes subtle scaling effects

## Technical Details

### 3D Sprite Rendering
- Uses cylinder primitives as base shapes for the buttons
- Applies 2D textures to 3D objects using Standard shader
- Maintains proper depth sorting and transparency

### Material Management
- Creates separate materials for base and pressed states
- Automatically loads textures from Resources folder
- Handles material cleanup on destroy

### Performance Considerations
- Textures are loaded once at startup
- Materials are reused between button states
- Minimal impact on frame rate during gameplay

## Customization

### Adding New Button Styles
1. Add new texture files to `Assets/Resources/`
2. Update the `baseTextureNames` and `pressedTextureNames` arrays in `TexturedHitZoneIndicators.cs`
3. Adjust the `indicatorSize` and `buttonDepth` values as needed

### Modifying Visual Effects
- Edit the `HandleInputFeedback()` method for key press effects
- Modify `TriggerHitEffect()` for note hit animations
- Adjust shader properties in `CreateButtonMaterials()` for different visual styles

## Troubleshooting

### Buttons Not Appearing
- Check that texture files are in the correct `Assets/Resources/` folder
- Verify that texture names match the arrays in the script
- Ensure the `TexturedHitZoneIndicators` component is active in the scene

### Input Not Working
- Verify that the `InputManager` component is present in the scene
- Check that key bindings match between `InputManager` and `TexturedHitZoneIndicators`
- Ensure the component is receiving Update calls

### Visual Issues
- Check that lane positions are properly assigned in the inspector
- Verify that the camera can see the button positions
- Adjust `indicatorSize` and `buttonDepth` values if buttons appear too small/large
