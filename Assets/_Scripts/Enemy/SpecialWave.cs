using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ScriptableObject that defines a special wave with unique rules and behaviors
/// Used by the DynamicEnemySpawner to create varied and interesting wave experiences
/// </summary>
[CreateAssetMenu(fileName = "New Special Wave", menuName = "Enemy System/Special Wave")]
public class SpecialWave : ScriptableObject
{
    [Header("Basic Info")]
    public string waveName = "Special Wave";
    public string description = "A special wave with unique mechanics";
    public Sprite icon; // For UI display
    
    [Header("Wave Selection")]
    [Range(1, 50)]
    public int minWaveToAppear = 5; // Earliest wave this special wave can appear
    [Range(0.1f, 10f)]
    public float selectionWeight = 1f; // Relative chance to be selected
    [Range(1, 10)]
    public int maxOccurrences = 3; // Maximum times this wave can appear
    
    [Header("Wave Structure")]
    public WavePhase[] phases; // Array of phases for this wave
    
    [Header("Special Rules")]
    public bool isEndlessRush = false; // Enemies keep spawning until time runs out
    public bool isBossWave = false; // Special boss wave
    public bool isSurvivalWave = false; // Player must survive for a set time
    public bool isEscortWave = false; // Protect something while enemies attack
    
    [Header("Modifiers")]
    [Range(0.5f, 3f)]
    public float enemyHealthMultiplier = 1f;
    [Range(0.5f, 3f)]
    public float enemySpeedMultiplier = 1f;
    [Range(0.5f, 3f)]
    public float enemyDamageMultiplier = 1f;
    [Range(0.5f, 3f)]
    public float spawnRateMultiplier = 1f;
    
    [Header("Environmental Effects")]
    public bool spawnInDarkness = false; // Reduce visibility
    public bool spawnWithFog = false; // Add fog effect
    public bool spawnWithWind = false; // Add wind effect
    public Color lightingTint = Color.white; // Tint the lighting
    
    [Header("Rewards")]
    public int bonusMoney = 0; // Extra money for completing this wave
    public int bonusExperience = 0; // Extra experience for completing this wave
    public GameObject specialRewardPrefab; // Special item that drops
    
    /// <summary>
    /// Check if this special wave can appear in the given wave
    /// </summary>
    public bool CanAppearInWave(int waveNumber)
    {
        return waveNumber >= minWaveToAppear;
    }
    
    /// <summary>
    /// Get the effective selection weight considering wave restrictions
    /// </summary>
    public float GetEffectiveSelectionWeight(int waveNumber)
    {
        return CanAppearInWave(waveNumber) ? selectionWeight : 0f;
    }
}

/// <summary>
/// Defines a single phase within a special wave
/// </summary>
[System.Serializable]
public class WavePhase
{
    [Header("Phase Info")]
    public string phaseName = "Phase";
    [Range(1f, 60f)]
    public float duration = 10f; // How long this phase lasts
    
    [Header("Spawn Settings")]
    public SpawnPattern spawnPattern = SpawnPattern.Steady;
    [Range(0.1f, 10f)]
    public float spawnRate = 1f; // Enemies per second
    [Range(1, 20)]
    public int maxEnemiesAlive = 10; // Maximum enemies alive during this phase
    
    [Header("Enemy Selection")]
    public EnemyType[] allowedEnemyTypes; // Only these enemy types can spawn
    public bool preferFastEnemies = false;
    public bool preferArmoredEnemies = false;
    public bool preferTankEnemies = false;
    
    [Header("Spawn Locations")]
    public SpawnLocation[] spawnLocations = { SpawnLocation.Random };
    [Range(5f, 50f)]
    public float minSpawnDistance = 10f;
    [Range(10f, 100f)]
    public float maxSpawnDistance = 30f;
    
    [Header("Phase Effects")]
    public bool isCalmPhase = false; // Reduced intensity
    public bool isChaoticPhase = false; // Increased intensity
    public bool isReliefPhase = false; // Brief respite
    public string phaseMessage = ""; // Message to display when phase starts
}

/// <summary>
/// Different patterns for spawning enemies
/// </summary>
public enum SpawnPattern
{
    Steady,     // Constant trickle of enemies
    Burst,      // Groups of enemies at once
    AllAtOnce,  // All enemies spawn simultaneously
    Pulsing,    // Alternating between fast and slow spawns
    Escalating, // Gradually increasing spawn rate
    Random      // Random timing between spawns
}

/// <summary>
/// Different locations where enemies can spawn
/// </summary>
public enum SpawnLocation
{
    Random,     // Random direction around player
    Front,      // In front of player
    Behind,     // Behind player
    Sides,      // To the left and right of player
    Surrounding, // All around player
    Specific    // Specific spawn points
}
