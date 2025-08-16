using UnityEngine;

/// <summary>
/// ScriptableObject that defines an enemy type with all its properties
/// Used by the DynamicEnemySpawner to determine what enemies to spawn
/// </summary>
[CreateAssetMenu(fileName = "New Enemy Type", menuName = "Enemy System/Enemy Type")]
public class EnemyType : ScriptableObject
{
    [Header("Basic Info")]
    public string enemyName = "Basic Enemy";
    public GameObject enemyPrefab;
    public Sprite icon; // For UI display
    
    [Header("Spawn Settings")]
    [Range(1, 100)]
    public int spawnCost = 10; // How many points this enemy costs to spawn
    [Range(0.1f, 10f)]
    public float spawnWeight = 1f; // Relative chance to spawn compared to other enemies
    [Range(1, 20)]
    public int minWaveToAppear = 1; // Earliest wave this enemy can appear
    [Range(1, 50)]
    public int maxWaveToAppear = 999; // Latest wave this enemy can appear (999 = unlimited)
    
    [Header("Base Stats")]
    [Range(1f, 100f)]
    public float baseHealth = 10f;
    [Range(1f, 20f)]
    public float baseSpeed = 3f;
    [Range(1f, 50f)]
    public float baseDamage = 5f;
    [Range(0.5f, 5f)]
    public float baseDamageCooldown = 1f;
    [Range(0.5f, 10f)]
    public float baseDamageRange = 1.5f;
    
    [Header("Stat Scaling")]
    [Range(0f, 2f)]
    public float healthScalingPerWave = 0.1f; // Additional health per wave
    [Range(0f, 1f)]
    public float speedScalingPerWave = 0.05f; // Additional speed per wave
    [Range(0f, 2f)]
    public float damageScalingPerWave = 0.1f; // Additional damage per wave
    
    [Header("Special Properties")]
    public bool isArmored = false; // Takes reduced damage
    public bool isFast = false; // Moves quickly
    public bool isTank = false; // High health, low speed
    public bool isBoss = false; // Special boss enemy
    public bool canSpawnInGroups = true; // Can multiple of this enemy spawn together
    
    [Header("Visual/Behavior")]
    public Color enemyColor = Color.red; // Color tint for this enemy type
    public float scaleMultiplier = 1f; // Size multiplier
    public bool hasSpecialDeathEffect = false;
    
    /// <summary>
    /// Get the scaled health for a specific wave
    /// </summary>
    public float GetHealthForWave(int waveNumber)
    {
        return baseHealth + (healthScalingPerWave * (waveNumber - 1));
    }
    
    /// <summary>
    /// Get the scaled speed for a specific wave
    /// </summary>
    public float GetSpeedForWave(int waveNumber)
    {
        return baseSpeed + (speedScalingPerWave * (waveNumber - 1));
    }
    
    /// <summary>
    /// Get the scaled damage for a specific wave
    /// </summary>
    public float GetDamageForWave(int waveNumber)
    {
        return baseDamage + (damageScalingPerWave * (waveNumber - 1));
    }
    
    /// <summary>
    /// Check if this enemy type can spawn in the given wave
    /// </summary>
    public bool CanSpawnInWave(int waveNumber)
    {
        return waveNumber >= minWaveToAppear && waveNumber <= maxWaveToAppear;
    }
    
    /// <summary>
    /// Get the effective spawn weight considering wave restrictions
    /// </summary>
    public float GetEffectiveSpawnWeight(int waveNumber)
    {
        return CanSpawnInWave(waveNumber) ? spawnWeight : 0f;
    }
}
