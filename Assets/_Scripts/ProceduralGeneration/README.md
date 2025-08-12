# Improved Procedural Level Generation System

This is an enhanced version of your procedural level generation system with performance optimizations, terrain variety, and better code structure.

## 🚀 Key Improvements

### Performance Optimizations
- **Chunk Pooling**: Reuses chunks instead of creating/destroying them
- **Async Generation**: Generates chunks over multiple frames to prevent frame drops
- **Performance Monitoring**: Real-time FPS and memory tracking
- **Optimized Updates**: Only regenerates chunks when player moves to new chunk

### Terrain Variety
- **Noise-based Generation**: Uses Perlin noise for natural-looking terrain
- **Biome System**: Different terrain types (Plains, Hills, Mountains)
- **Height Variation**: Dynamic elevation with configurable ranges
- **Procedural Features**: Trees, rocks, and structures that spawn based on terrain

### Code Structure
- **Modular Design**: Separated concerns into different components
- **ScriptableObject Settings**: Easy configuration through TerrainSettings
- **Better Error Handling**: More robust initialization and error checking
- **Cleaner Architecture**: Removed duplicate code and improved organization

## 📁 File Structure

```
ProceduralGeneration/
├── ProceduralLevelManager.cs      # Main manager (improved)
├── TerrainGenerator.cs            # Handles terrain generation
├── TerrainSettings.cs             # ScriptableObject for configuration
├── Chunk.cs                       # Individual chunk behavior (updated)
├── ChunkPersistenceManager.cs     # Saves/loads chunk data
├── PerformanceMonitor.cs          # Performance tracking
├── SimpleTreeCreator.cs           # Utility for creating tree prefabs
├── SimpleRockCreator.cs           # Utility for creating rock prefabs
└── README.md                      # This file
```

## 🛠️ Setup Instructions

### 1. Basic Setup
1. Add `ProceduralLevelManager` component to an empty GameObject in your scene
2. Configure the settings in the inspector:
   - **Chunk Size**: Size of each chunk (default: 10)
   - **Render Distance**: How many chunks to load around player (default: 16)
   - **Player**: Assign your player transform (or leave empty for auto-detection)

### 2. Terrain Configuration
1. Create a TerrainSettings asset using one of these methods:
   - **Method 1**: Right-click in Project window → Create → Procedural Generation → Terrain Settings
   - **Method 2**: Add `TerrainSettingsCreator` component to any GameObject and use the context menu
   - **Method 3**: Use the `TestImprovedSystem` component's "Create Complete Setup" function
2. Configure the settings:
   - **Noise Settings**: Control terrain generation parameters
   - **Height Settings**: Set min/max elevation and curve
   - **Biome Settings**: Define different terrain types
   - **Feature Settings**: Enable/disable trees, rocks, structures

### 3. Performance Settings
Configure performance options in ProceduralLevelManager:
- **Max Chunks In Pool**: How many chunks to keep in memory (default: 100)
- **Use Async Generation**: Spread chunk generation over multiple frames
- **Chunks Per Frame**: How many chunks to generate per frame (default: 2)

### 4. Feature Prefabs (Optional)
1. Create tree prefabs using `SimpleTreeCreator`
2. Create rock prefabs using `SimpleRockCreator`
3. Assign them to the `TerrainGenerator` component

## 🎮 Usage

### Basic Usage
The system works automatically once set up. Just move your player around and chunks will generate dynamically.

### Performance Monitoring
Add `PerformanceMonitor` component to track:
- FPS and average FPS
- Active and pooled chunk counts
- Memory usage

### Customization
- **Terrain**: Modify `TerrainSettings` to change terrain appearance
- **Biomes**: Add new biomes or modify existing ones
- **Features**: Create custom prefabs and assign them to `TerrainGenerator`

## ⚙️ Configuration Examples

### Flat Terrain
```csharp
// In TerrainSettings
MaxHeight = 0f;
MinHeight = 0f;
NoiseScale = 100f; // Large scale = smoother terrain
```

### Mountainous Terrain
```csharp
// In TerrainSettings
MaxHeight = 20f;
MinHeight = -5f;
Octaves = 6; // More detail
Persistence = 0.6f; // More variation
```

### Dense Forest
```csharp
// In TerrainSettings
EnableTrees = true;
TreeDensity = 0.3f; // 30% chance per grid cell
```

## 🔧 Advanced Features

### Custom Terrain Generation
Extend `TerrainGenerator` to add custom generation logic:
```csharp
public class CustomTerrainGenerator : TerrainGenerator
{
    protected override void GenerateCustomFeatures(Chunk chunk, Vector2Int position)
    {
        // Your custom feature generation
    }
}
```

### Biome-Specific Features
Modify feature generation based on biome:
```csharp
if (biome.BiomeName == "Desert")
{
    // Generate cacti instead of trees
}
```

### Height-Based Generation
Use height data for feature placement:
```csharp
float height = GetHeightAt(worldPosition, chunkPosition);
if (height > 5f)
{
    // Place mountain features
}
```

## 📊 Performance Tips

1. **Adjust Render Distance**: Smaller values = better performance
2. **Use Chunk Pooling**: Keeps memory usage consistent
3. **Limit Features**: Reduce tree/rock density for better performance
4. **Monitor Performance**: Use PerformanceMonitor to track issues

## 🐛 Troubleshooting

### Chunks Not Generating
- Check if player has "Player" tag
- Verify ProceduralLevelManager is active
- Check console for error messages

### Poor Performance
- Reduce render distance
- Enable async generation
- Reduce feature density
- Check PerformanceMonitor for bottlenecks

### Terrain Looks Wrong
- Adjust noise settings in TerrainSettings
- Check biome configuration
- Verify height curve settings

### TerrainSettings Menu Not Showing
- Wait for Unity to finish compiling scripts
- Check console for compilation errors
- Use `TerrainSettingsCreator` component as alternative
- Use `TestImprovedSystem` to create setup automatically

### Compilation Errors
- Make sure all scripts are saved
- Check for missing `using` statements
- Restart Unity if errors persist

## 🔄 Migration from Old System

If you're upgrading from the old system:
1. Remove old `ChunkGenerator` component
2. Replace with `ProceduralLevelManager`
3. Create `TerrainSettings` asset
4. Configure settings as needed
5. Test and adjust performance settings

## 📝 Notes

- The system automatically handles chunk loading/unloading
- Chunks are saved/loaded automatically if persistence is enabled
- Performance monitoring helps identify optimization opportunities
- All settings can be adjusted at runtime for testing

## 🎯 Future Enhancements

Potential improvements you could add:
- LOD (Level of Detail) system
- Texture streaming
- Weather effects per biome
- Dynamic events and encounters
- Multi-threaded generation
- GPU-based terrain generation
