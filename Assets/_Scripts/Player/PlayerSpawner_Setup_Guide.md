# PlayerSpawner Setup Guide

## Overview
The PlayerSpawner is a comprehensive system for handling player spawning across different scenes and scenarios. It supports spawn points, fallback spawning, and proper player initialization.

## Features

### 🎯 **Core Functionality**
- **Automatic spawning** at the best available spawn point
- **Fallback spawning** when no spawn point is found
- **Scene persistence** - can persist between scene loads
- **Player initialization** - properly sets up all player components
- **Invulnerability** - temporary invulnerability after spawning
- **Event system** - notifies when player is spawned/despawned

### 🔧 **Spawn Point Detection**
The spawner automatically finds spawn points in this order:
1. **Assigned default spawn point** (in inspector)
2. **Objects with "PlayerSpawnPoint" tag**
3. **Objects with "SpawnPoint" or "PlayerSpawn" in name**
4. **Fallback position** (if enabled)

## Setup Instructions

### 1. Create PlayerSpawner GameObject
1. Create an empty GameObject in your scene
2. Name it "PlayerSpawner"
3. Add the `PlayerSpawner` script to it

### 2. Configure PlayerSpawner Settings

#### **Player Settings**
- **Player Prefab**: Assign your player prefab
- **Spawn On Start**: Enable to spawn player when scene starts
- **Destroy Existing Player**: Enable to replace existing players

#### **Spawn Point Settings**
- **Default Spawn Point**: Assign a specific spawn point transform
- **Spawn Point Tag**: Tag for automatic spawn point detection ("PlayerSpawnPoint")
- **Fallback Spawn Position**: Position to spawn if no spawn point found
- **Use Fallback If No Spawn Point**: Enable fallback spawning

#### **Spawn Behavior**
- **Spawn Delay**: Time to wait before spawning (0.1s default)
- **Reset Player Stats On Spawn**: Reset health/money on spawn
- **Enable Invulnerability On Spawn**: Temporary invulnerability
- **Invulnerability Duration**: How long invulnerability lasts (2s default)

#### **Scene Management**
- **Persist Between Scenes**: Keep spawner alive between scenes
- **Auto Spawn On Scene Load**: Automatically spawn when new scene loads

### 3. Create Spawn Points
You can create spawn points in several ways:

#### **Method 1: Tagged GameObject**
1. Create an empty GameObject
2. Name it "PlayerSpawnPoint"
3. Set its tag to "PlayerSpawnPoint"
4. Position it where you want the player to spawn

#### **Method 2: Named GameObject**
1. Create an empty GameObject
2. Name it "SpawnPoint" or "PlayerSpawn"
3. Position it where you want the player to spawn

#### **Method 3: Assigned Transform**
1. Create an empty GameObject
2. Position it where you want the player to spawn
3. Assign it to the PlayerSpawner's "Default Spawn Point" field

### 4. Player Prefab Requirements
Your player prefab should have these components:
- `PlayerHealth` - Health system with invulnerability support
- `PlayerMoney` - Money/currency system
- `WeaponManager` - Weapon management
- `CameraController` - Camera control
- `AuraSystem` - Aura effects (optional)

## Usage Examples

### **Basic Usage**
```csharp
// Get the PlayerSpawner instance
PlayerSpawner spawner = PlayerSpawner.Instance;

// Spawn player at best available location
spawner.SpawnPlayer();

// Spawn player at specific position
spawner.SpawnPlayerAtPosition(new Vector3(0, 1, 0));

// Spawn player at specific spawn point
Transform spawnPoint = GameObject.Find("MySpawnPoint").transform;
spawner.SpawnPlayerAtSpawnPoint(spawnPoint);

// Despawn current player
spawner.DespawnPlayer();
```

### **Event Handling**
```csharp
void Start()
{
    PlayerSpawner spawner = PlayerSpawner.Instance;
    
    // Subscribe to spawn events
    spawner.OnPlayerSpawned += OnPlayerSpawned;
    spawner.OnPlayerDespawned += OnPlayerDespawned;
}

void OnPlayerSpawned(GameObject player)
{
    Debug.Log("Player spawned: " + player.name);
    // Do something when player spawns
}

void OnPlayerDespawned(GameObject player)
{
    Debug.Log("Player despawned: " + player.name);
    // Do something when player despawns
}
```

### **Scene Integration**
```csharp
// In your scene manager or game manager
public void LoadGameScene()
{
    // The PlayerSpawner will automatically spawn the player
    // when the new scene loads (if autoSpawnOnSceneLoad is enabled)
    SceneManager.LoadScene("GameScene");
}
```

## Advanced Features

### **Invulnerability System**
The spawner automatically makes the player invulnerable for a short time after spawning. This prevents immediate death from nearby enemies.

### **Component Initialization**
The spawner automatically initializes all player components:
- **PlayerHealth**: Restores full health
- **PlayerMoney**: Optionally resets money
- **WeaponManager**: Reloads weapons
- **CameraController**: Resets camera position and locks cursor
- **AuraSystem**: Ready for use

### **Error Handling**
The spawner includes comprehensive error handling:
- Warns if no spawn point is found
- Warns if player prefab is missing
- Prevents multiple spawn attempts
- Graceful fallback to default position

## Troubleshooting

### **Player Not Spawning**
1. Check if PlayerSpawner is in the scene
2. Verify player prefab is assigned
3. Check if spawn points exist
4. Enable fallback spawning

### **Player Spawning in Wrong Location**
1. Check spawn point positions
2. Verify spawn point tags/names
3. Check fallback spawn position
4. Ensure spawn points are active

### **Components Not Initializing**
1. Verify player prefab has required components
2. Check component initialization methods exist
3. Look for error messages in console

## Context Menu Commands
Right-click on the PlayerSpawner component in the inspector to access:
- **Spawn Player**: Manually spawn player
- **Despawn Player**: Manually despawn player
- **Spawn Player at Fallback Position**: Spawn at fallback location

This system provides a robust foundation for player spawning across your entire game! 🎮
