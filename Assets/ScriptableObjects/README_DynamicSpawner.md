# Dynamic Enemy Spawner System

A comprehensive enemy spawning system that creates dynamic, emotionally varied waves with point budgets, phases, and performance-based difficulty adjustment.

## Features

### 🎯 Point Budget System
- Each wave has a point budget that grows over time
- Enemy types have different spawn costs
- Spawner intelligently chooses enemies based on available points

### 🌊 Wave Phases
- Waves can have multiple phases (calm start, chaotic middle, relief end)
- Different spawn patterns for each phase
- Dynamic pacing that varies between steady trickle, bursts, and all-at-once spawns

### 🎮 Special Event Waves
- Fast enemy rushes
- Armored enemy assaults
- Endless rush waves
- Boss waves
- Survival challenges

### 📊 Performance Tracking
- Tracks player performance metrics
- Adjusts difficulty dynamically based on:
  - Wave clear time
  - Player health
  - Damage taken
  - Kills per minute
- Maintains history for consistent difficulty progression

### 🎨 Spawn Location Variety
- Random spawning around player
- Directional spawning (front, behind, sides)
- Surrounding spawns for maximum pressure
- Configurable spawn distances

### ⚙️ ScriptableObject Configuration
- All enemy stats, costs, and special wave rules are editable via ScriptableObjects
- Easy to add new enemy types without rewriting code
- Visual editor support for all configurations

## Quick Setup

### 1. Replace Existing Spawner
Replace your current `EnemySpawner` component with `DynamicEnemySpawner`:

1. Select your existing EnemySpawner GameObject
2. Remove the `EnemySpawner` component
3. Add the `DynamicEnemySpawner` component

### 2. Auto-Setup (Recommended)
Add the `DynamicSpawnerSetup` component to any GameObject in your scene:

1. Create an empty GameObject called "SpawnerSetup"
2. Add the `DynamicSpawnerSetup` component
3. The system will automatically configure itself on start

### 3. Manual Setup
If you prefer manual setup:

1. **Enemy Types**: Assign the example EnemyType ScriptableObjects to the "Available Enemy Types" array
2. **Special Waves**: Assign the example SpecialWave ScriptableObjects to the "Available Special Waves" array
3. **Performance Tracker**: Create a PlayerPerformanceTracker ScriptableObject and assign it

## Configuration

### Enemy Types
Each enemy type defines:
- **Basic Info**: Name, prefab, icon
- **Spawn Settings**: Cost, weight, wave restrictions
- **Base Stats**: Health, speed, damage, cooldowns
- **Stat Scaling**: How stats increase per wave
- **Special Properties**: Armored, fast, tank, boss flags
- **Visual Properties**: Color tint, scale multiplier

### Special Waves
Each special wave defines:
- **Wave Structure**: Multiple phases with different behaviors
- **Spawn Patterns**: Steady, burst, all-at-once, pulsing, escalating, random
- **Enemy Preferences**: Prefer fast, armored, or tank enemies
- **Spawn Locations**: Random, front, behind, sides, surrounding
- **Environmental Effects**: Darkness, fog, wind, lighting tint
- **Rewards**: Bonus money, experience, special items

### Performance Tracking
The system tracks:
- **Wave Clear Time**: How long each wave takes
- **Player Health**: Final health percentage
- **Damage Taken**: Total damage received
- **Kills Per Minute**: Combat efficiency
- **Difficulty Adjustment**: Automatic scaling based on performance

## Example Configurations

### Basic Enemy
```csharp
EnemyName: "Basic Enemy"
SpawnCost: 10
SpawnWeight: 1.0
BaseHealth: 10
BaseSpeed: 3
BaseDamage: 5
EnemyColor: Red
```

### Fast Enemy
```csharp
EnemyName: "Fast Enemy"
SpawnCost: 15
SpawnWeight: 0.8
MinWaveToAppear: 3
BaseHealth: 5
BaseSpeed: 6
BaseDamage: 3
IsFast: true
EnemyColor: Green
ScaleMultiplier: 0.8
```

### Tank Enemy
```csharp
EnemyName: "Tank Enemy"
SpawnCost: 25
SpawnWeight: 0.6
MinWaveToAppear: 5
BaseHealth: 30
BaseSpeed: 1.5
BaseDamage: 8
IsTank: true
EnemyColor: Blue
ScaleMultiplier: 1.3
```

### Fast Rush Special Wave
```csharp
WaveName: "Fast Rush"
Phases:
1. Preparation (3s) - Calm phase with fast enemies
2. Rush (12s) - Chaotic burst spawning
3. Relief (5s) - Reduced spawn rate
EnemySpeedMultiplier: 1.5
EnemyHealthMultiplier: 0.8
```

## Spawn Patterns

### Steady
Constant trickle of enemies at regular intervals

### Burst
Groups of enemies spawn together periodically

### All At Once
All enemies for the phase spawn simultaneously

### Pulsing
Alternating between fast and slow spawn rates

### Escalating
Gradually increasing spawn rate throughout the phase

### Random
Random timing between spawns

## Spawn Locations

### Random
Enemies spawn in random directions around the player

### Front
Enemies spawn in front of the player

### Behind
Enemies spawn behind the player

### Sides
Enemies spawn to the left and right of the player

### Surrounding
Enemies spawn in all directions around the player

## Performance Metrics

The system calculates an overall performance score (0-1) based on:

- **Time Performance**: How quickly waves are cleared
- **Health Performance**: How much health is maintained
- **Damage Performance**: How little damage is taken
- **Kill Performance**: How efficiently enemies are killed

This score is used to adjust difficulty multipliers for:
- Enemy health scaling
- Enemy speed scaling
- Enemy damage scaling
- Spawn rate scaling

## Events

The spawner provides UnityEvents for integration with other systems:

- `OnWaveStart(int waveNumber)`: Called when a wave begins
- `OnWaveComplete(int waveNumber)`: Called when a wave ends
- `OnPhaseMessage(string message)`: Called when a phase message should be displayed
- `OnSpecialWaveStart(SpecialWave wave)`: Called when a special wave begins

## Debug Information

Enable `showDebugInfo` to see real-time information about:
- Current wave and phase
- Active enemy count
- Remaining point budget
- Performance score
- Difficulty level

## Tips for Best Results

1. **Balance Enemy Costs**: Ensure enemy costs are balanced with their effectiveness
2. **Use Wave Restrictions**: Prevent overpowered enemies from appearing too early
3. **Create Phase Variety**: Mix different spawn patterns and enemy preferences
4. **Test Performance Tracking**: Monitor how the difficulty adjusts to player skill
5. **Use Special Waves Sparingly**: Don't make every wave special - variety comes from mixing regular and special waves

## Troubleshooting

### No Enemies Spawning
- Check that enemy types are assigned
- Verify enemy prefabs are set
- Ensure point budget is sufficient for enemy costs

### Performance Issues
- Reduce max enemies alive per phase
- Lower spawn rates
- Simplify special wave phases

### Difficulty Too High/Low
- Adjust performance tracker settings
- Modify difficulty adjustment speed
- Tune enemy stat scaling

## Extending the System

### Adding New Enemy Types
1. Create a new EnemyType ScriptableObject
2. Configure stats, costs, and properties
3. Assign to spawner's available enemy types

### Adding New Special Waves
1. Create a new SpecialWave ScriptableObject
2. Define phases with spawn patterns and preferences
3. Assign to spawner's available special waves

### Custom Spawn Patterns
Extend the `SpawnPattern` enum and add logic to `HandleSpawning()` method

### Custom Performance Metrics
Extend `PlayerPerformanceTracker` to track additional metrics

## File Structure

```
Assets/
├── ScriptableObjects/
│   ├── EnemyType.cs
│   ├── SpecialWave.cs
│   ├── PlayerPerformanceTracker.cs
│   ├── EnemyTypes/
│   │   ├── BasicEnemy.asset
│   │   ├── FastEnemy.asset
│   │   ├── TankEnemy.asset
│   │   └── ArmoredEnemy.asset
│   └── SpecialWaves/
│       ├── FastRushWave.asset
│       ├── TankAssaultWave.asset
│       └── EndlessRushWave.asset
└── _Scripts/Enemy/
    ├── DynamicEnemySpawner.cs
    └── DynamicSpawnerSetup.cs
```

This system provides a solid foundation for creating engaging, dynamic enemy encounters that adapt to player skill and provide varied, emotionally engaging experiences.
