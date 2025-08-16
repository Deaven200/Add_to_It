using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ScriptableObject that tracks player performance metrics
/// Used by the DynamicEnemySpawner to adjust difficulty dynamically
/// </summary>
[CreateAssetMenu(fileName = "Player Performance Tracker", menuName = "Enemy System/Player Performance Tracker")]
public class PlayerPerformanceTracker : ScriptableObject
{
    [Header("Performance Metrics")]
    [SerializeField] private float averageWaveClearTime = 30f;
    [SerializeField] private float averagePlayerHealth = 100f;
    [SerializeField] private float averageDamageTaken = 0f;
    [SerializeField] private float averageKillsPerMinute = 10f;
    [SerializeField] private float averageAccuracy = 0.7f; // TODO: Implement accuracy tracking - currently unused
    
    [Header("Difficulty Adjustment")]
    [Range(0.5f, 2f)]
    public float healthScalingMultiplier = 1f;
    [Range(0.5f, 2f)]
    public float speedScalingMultiplier = 1f;
    [Range(0.5f, 2f)]
    public float damageScalingMultiplier = 1f;
    [Range(0.5f, 2f)]
    public float spawnRateMultiplier = 1f;
    
    [Header("Performance History")]
    [SerializeField] private List<float> waveClearTimes = new List<float>();
    [SerializeField] private List<float> playerHealthHistory = new List<float>();
    [SerializeField] private List<float> damageTakenHistory = new List<float>();
    [SerializeField] private List<float> killsPerMinuteHistory = new List<float>();
    
    [Header("Settings")]
    [Range(5, 20)]
    public int historySize = 10; // How many waves to remember for averaging
    [Range(0.1f, 1f)]
    public float difficultyAdjustmentSpeed = 0.2f; // How quickly difficulty adjusts
    
    // Events for other systems to subscribe to
    public System.Action<float> OnDifficultyChanged;
    public System.Action<float> OnPerformanceUpdated;
    
    /// <summary>
    /// Record the completion of a wave with performance data
    /// </summary>
    public void RecordWaveCompletion(float clearTime, float finalHealth, float damageTaken, float killsPerMinute)
    {
        // Add to history
        waveClearTimes.Add(clearTime);
        playerHealthHistory.Add(finalHealth);
        damageTakenHistory.Add(damageTaken);
        killsPerMinuteHistory.Add(killsPerMinute);
        
        // Keep history size manageable
        if (waveClearTimes.Count > historySize)
        {
            waveClearTimes.RemoveAt(0);
            playerHealthHistory.RemoveAt(0);
            damageTakenHistory.RemoveAt(0);
            killsPerMinuteHistory.RemoveAt(0);
        }
        
        // Calculate new averages
        CalculateAverages();
        
        // Adjust difficulty based on performance
        AdjustDifficulty();
        
        // Notify subscribers
        OnPerformanceUpdated?.Invoke(GetOverallPerformance());
    }
    
    /// <summary>
    /// Calculate averages from the performance history
    /// </summary>
    private void CalculateAverages()
    {
        if (waveClearTimes.Count == 0) return;
        
        averageWaveClearTime = CalculateAverage(waveClearTimes);
        averagePlayerHealth = CalculateAverage(playerHealthHistory);
        averageDamageTaken = CalculateAverage(damageTakenHistory);
        averageKillsPerMinute = CalculateAverage(killsPerMinuteHistory);
    }
    
    /// <summary>
    /// Calculate the average of a list of floats
    /// </summary>
    private float CalculateAverage(List<float> values)
    {
        if (values.Count == 0) return 0f;
        
        float sum = 0f;
        foreach (float value in values)
        {
            sum += value;
        }
        return sum / values.Count;
    }
    
    /// <summary>
    /// Adjust difficulty multipliers based on player performance
    /// </summary>
    private void AdjustDifficulty()
    {
        float performance = GetOverallPerformance();
        float adjustment = (performance - 0.5f) * difficultyAdjustmentSpeed; // -0.5 to 0.5 range
        
        // Adjust multipliers (higher performance = higher difficulty)
        healthScalingMultiplier = Mathf.Clamp(healthScalingMultiplier + adjustment, 0.5f, 2f);
        speedScalingMultiplier = Mathf.Clamp(speedScalingMultiplier + adjustment, 0.5f, 2f);
        damageScalingMultiplier = Mathf.Clamp(damageScalingMultiplier + adjustment, 0.5f, 2f);
        spawnRateMultiplier = Mathf.Clamp(spawnRateMultiplier + adjustment, 0.5f, 2f);
        
        OnDifficultyChanged?.Invoke(performance);
    }
    
    /// <summary>
    /// Get overall performance score (0-1, where 1 is excellent performance)
    /// </summary>
    public float GetOverallPerformance()
    {
        if (waveClearTimes.Count == 0) return 0.5f; // Neutral performance if no data
        
        // Calculate performance factors (0-1 each)
        float timePerformance = Mathf.Clamp01(1f - (averageWaveClearTime - 20f) / 40f); // 20s = 1.0, 60s = 0.0
        float healthPerformance = Mathf.Clamp01(averagePlayerHealth / 100f); // 100% health = 1.0
        float damagePerformance = Mathf.Clamp01(1f - (averageDamageTaken / 50f)); // 0 damage = 1.0, 50+ damage = 0.0
        float killPerformance = Mathf.Clamp01(averageKillsPerMinute / 20f); // 20+ kills/min = 1.0
        
        // Weighted average
        return (timePerformance * 0.3f + healthPerformance * 0.3f + damagePerformance * 0.2f + killPerformance * 0.2f);
    }
    
    /// <summary>
    /// Get the current difficulty level (0-1, where 1 is maximum difficulty)
    /// </summary>
    public float GetCurrentDifficulty()
    {
        return (healthScalingMultiplier + speedScalingMultiplier + damageScalingMultiplier + spawnRateMultiplier) / 4f;
    }
    
    /// <summary>
    /// Reset all performance tracking data
    /// </summary>
    public void ResetPerformance()
    {
        waveClearTimes.Clear();
        playerHealthHistory.Clear();
        damageTakenHistory.Clear();
        killsPerMinuteHistory.Clear();
        
        healthScalingMultiplier = 1f;
        speedScalingMultiplier = 1f;
        damageScalingMultiplier = 1f;
        spawnRateMultiplier = 1f;
        
        CalculateAverages();
    }
    
    /// <summary>
    /// Get a summary of current performance metrics
    /// </summary>
    public string GetPerformanceSummary()
    {
        return $"Performance: {GetOverallPerformance():F2}\n" +
               $"Avg Clear Time: {averageWaveClearTime:F1}s\n" +
               $"Avg Health: {averagePlayerHealth:F0}%\n" +
               $"Avg Damage: {averageDamageTaken:F1}\n" +
               $"Avg Kills/min: {averageKillsPerMinute:F1}\n" +
               $"Difficulty: {GetCurrentDifficulty():F2}";
    }
}
