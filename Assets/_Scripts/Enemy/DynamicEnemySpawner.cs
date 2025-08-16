using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Advanced enemy spawner system that creates dynamic, emotionally varied waves
/// Uses point budgets, phases, and performance tracking to create engaging experiences
/// </summary>
public class DynamicEnemySpawner : MonoBehaviour
{
    [Header("Enemy Configuration")]
    public EnemyType[] availableEnemyTypes;
    public SpecialWave[] availableSpecialWaves;
    
    [Header("Spawner Settings")]
    [Range(10, 1000)]
    public int basePointBudget = 100;
    [Range(10, 200)]
    public int pointBudgetIncreasePerWave = 20;
    [Range(1f, 10f)]
    public float timeBetweenWaves = 5f;
    [Range(5f, 50f)]
    public float minSpawnDistance = 10f;
    [Range(10f, 100f)]
    public float maxSpawnDistance = 30f;
    
    [Header("Performance Tracking")]
    public PlayerPerformanceTracker performanceTracker;
    
    [Header("Wave Events")]
    public UnityEvent<int> OnWaveStart;
    public UnityEvent<int> OnWaveComplete;
    public UnityEvent<string> OnPhaseMessage;
    public UnityEvent<SpecialWave> OnSpecialWaveStart;
    
    [Header("Debug")]
    public bool showDebugInfo = true;
    public bool pauseSpawning = false;
    
    // Private variables
    private int currentWave = 0;
    private int currentPointBudget;
    private List<GameObject> activeEnemies = new List<GameObject>();
    private bool waveInProgress = false;
    private float waveStartTime;
    private float totalDamageTakenThisWave = 0f;
    private int killsThisWave = 0;
    private float waveKillStartTime;
    
    // Performance tracking
    private PlayerHealth playerHealth;
    private PlayerDamageHandler playerDamageHandler;
    private Transform playerTransform;
    
    // Current wave state
    private SpecialWave currentSpecialWave;
    private WavePhase currentPhase;
    private int currentPhaseIndex = 0;
    private float phaseStartTime;
    private float lastSpawnTime;
    private int enemiesSpawnedThisPhase = 0;
    
    void Start()
    {
        // Find player components
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            playerHealth = player.GetComponent<PlayerHealth>();
            playerDamageHandler = player.GetComponent<PlayerDamageHandler>();
        }
        
        // Initialize performance tracker
        if (performanceTracker == null)
        {
            performanceTracker = ScriptableObject.CreateInstance<PlayerPerformanceTracker>();
        }
        
        // Subscribe to performance events
        performanceTracker.OnDifficultyChanged += OnDifficultyChanged;
        
        // Start the first wave
        StartCoroutine(StartNextWave());
    }
    
    void Update()
    {
        if (pauseSpawning) return;
        
        // Clean up dead enemies
        activeEnemies.RemoveAll(enemy => enemy == null);
        
        // Update wave progress
        if (waveInProgress)
        {
            UpdateWaveProgress();
        }
        
        // Check if wave is complete
        if (waveInProgress && activeEnemies.Count == 0 && !IsPhaseStillActive())
        {
            CompleteWave();
        }
    }
    
    /// <summary>
    /// Start the next wave with dynamic difficulty and special wave selection
    /// </summary>
    IEnumerator StartNextWave()
    {
        currentWave++;
        waveInProgress = true;
        waveStartTime = Time.time;
        waveKillStartTime = Time.time;
        totalDamageTakenThisWave = 0f;
        killsThisWave = 0;
        
        // Calculate point budget for this wave
        currentPointBudget = basePointBudget + (pointBudgetIncreasePerWave * (currentWave - 1));
        
        // Apply performance-based adjustments
        currentPointBudget = Mathf.RoundToInt(currentPointBudget * performanceTracker.spawnRateMultiplier);
        
        // Decide if this should be a special wave
        currentSpecialWave = ShouldUseSpecialWave() ? SelectSpecialWave() : null;
        
        if (currentSpecialWave != null)
        {
            OnSpecialWaveStart?.Invoke(currentSpecialWave);
            Debug.Log($"Starting Special Wave: {currentSpecialWave.waveName}");
        }
        
        OnWaveStart?.Invoke(currentWave);
        
        // Start the first phase
        currentPhaseIndex = 0;
        StartPhase();
        
        yield return null;
    }
    
    /// <summary>
    /// Determine if this wave should be a special wave
    /// </summary>
    private bool ShouldUseSpecialWave()
    {
        if (availableSpecialWaves == null || availableSpecialWaves.Length == 0) return false;
        
        // 20% chance for special wave after wave 5
        if (currentWave >= 5 && Random.Range(0f, 1f) < 0.2f) return true;
        
        // 40% chance for special wave after wave 10
        if (currentWave >= 10 && Random.Range(0f, 1f) < 0.4f) return true;
        
        return false;
    }
    
    /// <summary>
    /// Select a special wave based on weights and availability
    /// </summary>
    private SpecialWave SelectSpecialWave()
    {
        List<SpecialWave> availableWaves = new List<SpecialWave>();
        List<float> weights = new List<float>();
        
        foreach (SpecialWave wave in availableSpecialWaves)
        {
            if (wave.CanAppearInWave(currentWave))
            {
                availableWaves.Add(wave);
                weights.Add(wave.GetEffectiveSelectionWeight(currentWave));
            }
        }
        
        if (availableWaves.Count == 0) return null;
        
        // Weighted random selection
        float totalWeight = 0f;
        foreach (float weight in weights)
        {
            totalWeight += weight;
        }
        
        float randomValue = Random.Range(0f, totalWeight);
        float currentWeight = 0f;
        
        for (int i = 0; i < availableWaves.Count; i++)
        {
            currentWeight += weights[i];
            if (randomValue <= currentWeight)
            {
                return availableWaves[i];
            }
        }
        
        return availableWaves[0]; // Fallback
    }
    
    /// <summary>
    /// Start a new phase within the current wave
    /// </summary>
    private void StartPhase()
    {
        if (currentSpecialWave != null && currentSpecialWave.phases != null && currentPhaseIndex < currentSpecialWave.phases.Length)
        {
            currentPhase = currentSpecialWave.phases[currentPhaseIndex];
        }
        else
        {
            // Create a default phase for regular waves
            currentPhase = CreateDefaultPhase();
        }
        
        phaseStartTime = Time.time;
        lastSpawnTime = Time.time;
        enemiesSpawnedThisPhase = 0;
        
        // Display phase message
        if (!string.IsNullOrEmpty(currentPhase.phaseMessage))
        {
            OnPhaseMessage?.Invoke(currentPhase.phaseMessage);
        }
        
        Debug.Log($"Starting Phase: {currentPhase.phaseName} (Duration: {currentPhase.duration}s)");
    }
    
    /// <summary>
    /// Create a default phase for regular waves
    /// </summary>
    private WavePhase CreateDefaultPhase()
    {
        WavePhase phase = new WavePhase();
        phase.phaseName = "Standard Phase";
        phase.duration = 15f;
        phase.spawnPattern = SpawnPattern.Steady;
        phase.spawnRate = 1f;
        phase.maxEnemiesAlive = 10;
        phase.allowedEnemyTypes = availableEnemyTypes;
        phase.spawnLocations = new SpawnLocation[] { SpawnLocation.Random };
        phase.minSpawnDistance = minSpawnDistance;
        phase.maxSpawnDistance = maxSpawnDistance;
        
        return phase;
    }
    
    /// <summary>
    /// Update the current wave progress and handle spawning
    /// </summary>
    private void UpdateWaveProgress()
    {
        if (currentPhase == null) return;
        
        // Check if phase is complete
        if (Time.time - phaseStartTime >= currentPhase.duration)
        {
            currentPhaseIndex++;
            
            if (currentSpecialWave != null && currentPhaseIndex < currentSpecialWave.phases.Length)
            {
                StartPhase(); // Start next phase
            }
            else
            {
                // Wave is complete
                return;
            }
        }
        
        // Handle spawning based on pattern
        HandleSpawning();
    }
    
    /// <summary>
    /// Handle enemy spawning based on the current phase's spawn pattern
    /// </summary>
    private void HandleSpawning()
    {
        if (currentPhase == null) return;
        
        float timeSincePhaseStart = Time.time - phaseStartTime;
        float timeSinceLastSpawn = Time.time - lastSpawnTime;
        
        // Check if we can spawn more enemies
        if (activeEnemies.Count >= currentPhase.maxEnemiesAlive) return;
        
        // Determine spawn timing based on pattern
        bool shouldSpawn = false;
        float spawnInterval = 1f / currentPhase.spawnRate;
        
        switch (currentPhase.spawnPattern)
        {
            case SpawnPattern.Steady:
                shouldSpawn = timeSinceLastSpawn >= spawnInterval;
                break;
                
            case SpawnPattern.Burst:
                // Spawn in groups every few seconds
                shouldSpawn = timeSinceLastSpawn >= spawnInterval * 3f && Random.Range(0f, 1f) < 0.3f;
                break;
                
            case SpawnPattern.AllAtOnce:
                // Spawn all enemies at the start of the phase
                shouldSpawn = timeSinceLastSpawn >= spawnInterval && enemiesSpawnedThisPhase == 0;
                break;
                
            case SpawnPattern.Pulsing:
                // Alternating fast and slow spawns
                float pulsePhase = Mathf.Sin(timeSincePhaseStart * 2f) * 0.5f + 0.5f;
                shouldSpawn = timeSinceLastSpawn >= spawnInterval * (1f + pulsePhase);
                break;
                
            case SpawnPattern.Escalating:
                // Gradually increasing spawn rate
                float escalationFactor = 1f + (timeSincePhaseStart / currentPhase.duration);
                shouldSpawn = timeSinceLastSpawn >= spawnInterval / escalationFactor;
                break;
                
            case SpawnPattern.Random:
                // Random timing
                shouldSpawn = timeSinceLastSpawn >= spawnInterval * Random.Range(0.5f, 2f);
                break;
        }
        
        if (shouldSpawn)
        {
            SpawnEnemy();
            lastSpawnTime = Time.time;
            enemiesSpawnedThisPhase++;
        }
    }
    
    /// <summary>
    /// Spawn a single enemy based on current phase settings
    /// </summary>
    private void SpawnEnemy()
    {
        // Select enemy type
        EnemyType selectedEnemy = SelectEnemyType();
        if (selectedEnemy == null || selectedEnemy.enemyPrefab == null) return;
        
        // Check if we have enough points
        if (currentPointBudget < selectedEnemy.spawnCost) return;
        
        // Calculate spawn position
        Vector3 spawnPosition = CalculateSpawnPosition();
        
        // Spawn the enemy
        GameObject enemy = Instantiate(selectedEnemy.enemyPrefab, spawnPosition, Quaternion.identity);
        activeEnemies.Add(enemy);
        
        // Apply enemy stats
        ApplyEnemyStats(enemy, selectedEnemy);
        
        // Deduct points
        currentPointBudget -= selectedEnemy.spawnCost;
        
        // Track enemy death for performance
        EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            // Subscribe to death event
            StartCoroutine(TrackEnemyDeath(enemyHealth));
        }
        
        Debug.Log($"Spawned {selectedEnemy.enemyName} at {spawnPosition}. Points remaining: {currentPointBudget}");
    }
    
    /// <summary>
    /// Select an enemy type based on current phase preferences and weights
    /// </summary>
    private EnemyType SelectEnemyType()
    {
        List<EnemyType> availableEnemies = new List<EnemyType>();
        List<float> weights = new List<float>();
        
        // Determine which enemy types are available
        EnemyType[] enemyPool = currentPhase.allowedEnemyTypes != null && currentPhase.allowedEnemyTypes.Length > 0 
            ? currentPhase.allowedEnemyTypes 
            : availableEnemyTypes;
        
        foreach (EnemyType enemyType in enemyPool)
        {
            if (enemyType == null) continue;
            
            // Check if enemy can spawn in this wave
            if (!enemyType.CanSpawnInWave(currentWave)) continue;
            
            // Check if we have enough points
            if (currentPointBudget < enemyType.spawnCost) continue;
            
            // Check phase preferences
            bool matchesPreference = true;
            if (currentPhase.preferFastEnemies && !enemyType.isFast) matchesPreference = false;
            if (currentPhase.preferArmoredEnemies && !enemyType.isArmored) matchesPreference = false;
            if (currentPhase.preferTankEnemies && !enemyType.isTank) matchesPreference = false;
            
            if (matchesPreference)
            {
                availableEnemies.Add(enemyType);
                float weight = enemyType.GetEffectiveSpawnWeight(currentWave);
                
                // Boost weight for preferred enemies
                if (currentPhase.preferFastEnemies && enemyType.isFast) weight *= 2f;
                if (currentPhase.preferArmoredEnemies && enemyType.isArmored) weight *= 2f;
                if (currentPhase.preferTankEnemies && enemyType.isTank) weight *= 2f;
                
                weights.Add(weight);
            }
        }
        
        if (availableEnemies.Count == 0) return null;
        
        // Weighted random selection
        float totalWeight = 0f;
        foreach (float weight in weights)
        {
            totalWeight += weight;
        }
        
        float randomValue = Random.Range(0f, totalWeight);
        float currentWeight = 0f;
        
        for (int i = 0; i < availableEnemies.Count; i++)
        {
            currentWeight += weights[i];
            if (randomValue <= currentWeight)
            {
                return availableEnemies[i];
            }
        }
        
        return availableEnemies[0]; // Fallback
    }
    
    /// <summary>
    /// Calculate spawn position based on current phase settings
    /// </summary>
    private Vector3 CalculateSpawnPosition()
    {
        if (playerTransform == null) return transform.position;
        
        Vector3 playerPos = playerTransform.position;
        Vector3 spawnPos = playerPos;
        
        // Select spawn location type
        SpawnLocation locationType = currentPhase.spawnLocations[Random.Range(0, currentPhase.spawnLocations.Length)];
        
        float distance = Random.Range(currentPhase.minSpawnDistance, currentPhase.maxSpawnDistance);
        Vector2 direction = Vector2.zero;
        
        switch (locationType)
        {
            case SpawnLocation.Random:
                direction = Random.insideUnitCircle.normalized;
                break;
                
            case SpawnLocation.Front:
                direction = new Vector2(0, 1); // Forward
                break;
                
            case SpawnLocation.Behind:
                direction = new Vector2(0, -1); // Behind
                break;
                
            case SpawnLocation.Sides:
                direction = Random.Range(0, 2) == 0 ? new Vector2(-1, 0) : new Vector2(1, 0);
                break;
                
            case SpawnLocation.Surrounding:
                // Spawn in all directions
                float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
                direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                break;
        }
        
        spawnPos += new Vector3(direction.x, 0, direction.y) * distance;
        spawnPos.y = 0f; // Keep on ground level
        
        return spawnPos;
    }
    
    /// <summary>
    /// Apply stats to a spawned enemy based on its type and current wave
    /// </summary>
    private void ApplyEnemyStats(GameObject enemy, EnemyType enemyType)
    {
        // Apply health
        EnemyHealth health = enemy.GetComponent<EnemyHealth>();
        if (health != null)
        {
            float healthValue = enemyType.GetHealthForWave(currentWave);
            if (currentSpecialWave != null)
            {
                healthValue *= currentSpecialWave.enemyHealthMultiplier;
            }
            healthValue *= performanceTracker.healthScalingMultiplier;
            health.maxHealth = healthValue;
        }
        
        // Apply AI stats
        EnemyAI ai = enemy.GetComponent<EnemyAI>();
        if (ai != null)
        {
            float speedValue = enemyType.GetSpeedForWave(currentWave);
            if (currentSpecialWave != null)
            {
                speedValue *= currentSpecialWave.enemySpeedMultiplier;
            }
            speedValue *= performanceTracker.speedScalingMultiplier;
            ai.moveSpeed = speedValue;
            ai.damage = enemyType.GetDamageForWave(currentWave);
        }
        
        // Apply damage stats
        EnemyDamage damage = enemy.GetComponent<EnemyDamage>();
        if (damage != null)
        {
            float damageValue = enemyType.GetDamageForWave(currentWave);
            if (currentSpecialWave != null)
            {
                damageValue *= currentSpecialWave.enemyDamageMultiplier;
            }
            damageValue *= performanceTracker.damageScalingMultiplier;
            damage.damageAmount = Mathf.RoundToInt(damageValue);
            damage.damageCooldown = enemyType.baseDamageCooldown;
            damage.damageRange = enemyType.baseDamageRange;
        }
        
        // Apply visual changes
        Renderer renderer = enemy.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = enemyType.enemyColor;
        }
        
        // Apply scale
        enemy.transform.localScale *= enemyType.scaleMultiplier;
    }
    
    /// <summary>
    /// Track enemy death for performance metrics
    /// </summary>
    IEnumerator TrackEnemyDeath(EnemyHealth enemyHealth)
    {
        // Wait for enemy to die
        while (enemyHealth != null && enemyHealth.maxHealth > 0)
        {
            yield return null;
        }
        
        // Enemy died
        killsThisWave++;
    }
    
    /// <summary>
    /// Check if the current phase is still active
    /// </summary>
    private bool IsPhaseStillActive()
    {
        if (currentPhase == null) return false;
        return Time.time - phaseStartTime < currentPhase.duration;
    }
    
    /// <summary>
    /// Complete the current wave and record performance
    /// </summary>
    private void CompleteWave()
    {
        waveInProgress = false;
        float waveDuration = Time.time - waveStartTime;
        float killDuration = Time.time - waveKillStartTime;
        float killsPerMinute = killDuration > 0 ? (killsThisWave / killDuration) * 60f : 0f;
        
        // Record performance
        if (playerHealth != null)
        {
            float finalHealth = (float)playerHealth.currentHealth / playerHealth.maxHealth * 100f;
            performanceTracker.RecordWaveCompletion(waveDuration, finalHealth, totalDamageTakenThisWave, killsPerMinute);
        }
        
        OnWaveComplete?.Invoke(currentWave);
        
        Debug.Log($"Wave {currentWave} completed in {waveDuration:F1}s. Kills: {killsThisWave}, KPM: {killsPerMinute:F1}");
        
        // Start next wave after delay
        StartCoroutine(StartNextWaveAfterDelay());
    }
    
    /// <summary>
    /// Start the next wave after a delay
    /// </summary>
    IEnumerator StartNextWaveAfterDelay()
    {
        yield return new WaitForSeconds(timeBetweenWaves);
        StartCoroutine(StartNextWave());
    }
    
    /// <summary>
    /// Handle difficulty changes from performance tracker
    /// </summary>
    private void OnDifficultyChanged(float performance)
    {
        Debug.Log($"Difficulty adjusted based on performance: {performance:F2}");
    }
    
    /// <summary>
    /// Record damage taken by the player
    /// </summary>
    public void RecordPlayerDamage(float damage)
    {
        totalDamageTakenThisWave += damage;
    }
    
    /// <summary>
    /// Get current wave information for UI
    /// </summary>
    public string GetWaveInfo()
    {
        if (!waveInProgress) return $"Wave {currentWave} - Starting...";
        
        string info = $"Wave {currentWave}";
        if (currentSpecialWave != null)
        {
            info += $" - {currentSpecialWave.waveName}";
        }
        
        if (currentPhase != null)
        {
            float phaseTimeRemaining = currentPhase.duration - (Time.time - phaseStartTime);
            info += $"\nPhase: {currentPhase.phaseName} ({phaseTimeRemaining:F1}s remaining)";
        }
        
        info += $"\nEnemies: {activeEnemies.Count} | Points: {currentPointBudget}";
        
        return info;
    }
    
    void OnGUI()
    {
        if (!showDebugInfo) return;
        
        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.Label(GetWaveInfo());
        GUILayout.Label($"Performance: {performanceTracker.GetOverallPerformance():F2}");
        GUILayout.Label($"Difficulty: {performanceTracker.GetCurrentDifficulty():F2}");
        GUILayout.EndArea();
    }
    
    void OnDestroy()
    {
        if (performanceTracker != null)
        {
            performanceTracker.OnDifficultyChanged -= OnDifficultyChanged;
        }
    }
}
