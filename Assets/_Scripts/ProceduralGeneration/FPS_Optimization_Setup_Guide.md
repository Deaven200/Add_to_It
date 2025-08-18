# FPS Optimization System Setup Guide

## Overview
This system automatically monitors your game's FPS and pauses chunk generation when performance drops below configurable thresholds. It helps maintain smooth gameplay by preventing the procedural generation system from causing frame rate drops.

## Components Added

### 1. FPSOptimizer
- **Location**: `Assets/_Scripts/ProceduralGeneration/FPSOptimizer.cs`
- **Purpose**: Monitors FPS and controls generation based on performance
- **Features**:
  - Real-time FPS monitoring with averaging
  - Configurable performance thresholds
  - Emergency mode for critical performance drops
  - Automatic render distance reduction
  - Enemy spawning pause/resume

### 2. FPSOptimizationUI
- **Location**: `Assets/_Scripts/UI/FPSOptimizationUI.cs`
- **Purpose**: Provides UI controls and status display
- **Features**:
  - Real-time FPS display with color coding
  - Optimization toggle
  - Emergency mode button
  - Resume generation button
  - Status indicators

### 3. Enhanced ProceduralLevelManager
- **Modifications**: Added FPS optimization support
- **New Methods**:
  - `PauseGeneration()` / `ResumeGeneration()`
  - `SetRenderDistance(int distance)`
  - Event handlers for FPS optimization

## Setup Instructions

### Step 1: Add FPSOptimizer to Your Scene
1. Create an empty GameObject named "FPSOptimizer"
2. Add the `FPSOptimizer` component to it
3. Configure the settings in the inspector:
   - **Min FPS Threshold**: 30 (pauses generation below this)
   - **Resume FPS Threshold**: 45 (resumes generation above this)
   - **Critical FPS Threshold**: 20 (triggers emergency mode)
   - **Emergency Render Distance**: 4 (reduced render distance in emergency mode)

### Step 2: Configure ProceduralLevelManager
1. Select your ProceduralLevelManager GameObject
2. In the inspector, find the "FPS Optimization" section
3. Enable "Enable FPS Optimization"
4. Assign the FPSOptimizer reference (drag from scene)

### Step 3: Optional UI Setup
1. Create a UI Canvas for the FPS optimization controls
2. Add UI elements (Text, Buttons, Toggle, Slider)
3. Add the `FPSOptimizationUI` component to a GameObject
4. Assign the UI references in the inspector

## Configuration Options

### FPS Thresholds
- **Min FPS Threshold**: When FPS drops below this, chunk generation pauses
- **Resume FPS Threshold**: When FPS goes above this, generation resumes
- **Critical FPS Threshold**: Triggers emergency mode with additional optimizations

### Emergency Mode
- **Duration**: How long to stay in emergency mode (default: 5 seconds)
- **Render Distance Reduction**: Reduces render distance to improve performance
- **Automatic Recovery**: Returns to normal mode after duration expires

### Performance Settings
- **FPS Sample Size**: Number of frames to average for FPS calculation (default: 30)
- **Update Interval**: How often to update FPS calculation (default: 0.5 seconds)

## How It Works

### Normal Operation
1. System continuously monitors FPS using a rolling average
2. When FPS drops below the minimum threshold, chunk generation pauses
3. Enemy spawning also pauses to reduce CPU load
4. When FPS recovers above the resume threshold, generation resumes

### Emergency Mode
1. Triggered when FPS drops below the critical threshold
2. Forces pause of all generation activities
3. Reduces render distance to minimum
4. Automatically recovers after the specified duration
5. Only resumes if FPS is above the resume threshold

### Performance Benefits
- **Reduced CPU Load**: Pausing generation frees up CPU cycles
- **Smoother Gameplay**: Prevents frame rate drops from chunk generation
- **Adaptive Rendering**: Reduces render distance when needed
- **Automatic Recovery**: System automatically optimizes and recovers

## Debug Features

### Console Logging
- FPS optimization events are logged to the console
- Performance statistics are displayed periodically
- Emergency mode activations are clearly marked

### On-Screen Display
- Real-time FPS counter
- Generation status indicator
- Emergency mode status
- Color-coded FPS display (Green/Yellow/Red)

## Troubleshooting

### System Not Working
1. Check that FPSOptimizer is in the scene
2. Verify ProceduralLevelManager has FPS optimization enabled
3. Ensure the FPSOptimizer reference is assigned
4. Check console for error messages

### Performance Still Poor
1. Lower the FPS thresholds
2. Reduce the emergency render distance
3. Increase the emergency mode duration
4. Check for other performance bottlenecks

### Too Aggressive Optimization
1. Increase the FPS thresholds
2. Reduce the emergency mode duration
3. Increase the emergency render distance
4. Disable emergency mode entirely

## Advanced Usage

### Custom Thresholds
```csharp
// Access the FPSOptimizer and set custom thresholds
FPSOptimizer optimizer = FindObjectOfType<FPSOptimizer>();
optimizer.SetFPSThresholds(25f, 40f, 15f); // min, resume, critical
```

### Manual Control
```csharp
// Force emergency mode
optimizer.ForceEmergencyMode();

// Force resume generation
optimizer.ForceResumeGeneration();

// Disable optimization entirely
optimizer.SetOptimizationEnabled(false);
```

### Event Handling
```csharp
// Subscribe to optimization events
optimizer.OnGenerationPausedChanged += (paused) => {
    Debug.Log($"Generation {(paused ? "paused" : "resumed")}");
};

optimizer.OnEmergencyModeChanged += (emergency) => {
    Debug.Log($"Emergency mode {(emergency ? "activated" : "deactivated")}");
};
```

## Performance Tips

1. **Start Conservative**: Begin with higher thresholds and adjust down
2. **Monitor Console**: Watch for optimization events to understand performance patterns
3. **Test Under Load**: Test with many enemies and complex terrain
4. **Balance Settings**: Find the right balance between performance and visual quality
5. **Profile Regularly**: Use Unity Profiler to identify other bottlenecks

## Integration Notes

- The system works automatically once set up
- No changes needed to existing chunk generation code
- Enemy spawning is automatically paused/resumed
- Render distance changes are handled automatically
- All optimizations are temporary and recover automatically

