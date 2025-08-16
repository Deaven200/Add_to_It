using UnityEngine;
using UnityEditor;

/// <summary>
/// Setup script to help configure the DynamicEnemySpawner system
/// This script provides easy setup and configuration options
/// </summary>
public class DynamicSpawnerSetup : MonoBehaviour
{
    [Header("Setup Instructions")]
    [TextArea(5, 10)]
    public string setupInstructions = 
        "SETUP INSTRUCTIONS:\n\n" +
        "1. Replace your existing EnemySpawner with DynamicEnemySpawner\n" +
        "2. Assign the example EnemyType ScriptableObjects to 'Available Enemy Types'\n" +
        "3. Assign the example SpecialWave ScriptableObjects to 'Available Special Waves'\n" +
        "4. Create a PlayerPerformanceTracker ScriptableObject\n" +
        "5. Configure spawner settings as desired\n\n" +
        "The system will automatically handle dynamic difficulty, wave phases, and performance tracking!";

    [Header("Quick Setup")]
    public bool autoSetupOnStart = true;
    
    [Header("Example Configurations")]
    public EnemyType[] exampleEnemyTypes;
    public SpecialWave[] exampleSpecialWaves;
    public PlayerPerformanceTracker examplePerformanceTracker;
    
    void Start()
    {
        if (autoSetupOnStart)
        {
            SetupDynamicSpawner();
        }
    }
    
    /// <summary>
    /// Automatically set up the DynamicEnemySpawner with example configurations
    /// </summary>
    [ContextMenu("Setup Dynamic Spawner")]
    public void SetupDynamicSpawner()
    {
        // Find or create the DynamicEnemySpawner
        DynamicEnemySpawner spawner = FindObjectOfType<DynamicEnemySpawner>();
        
        if (spawner == null)
        {
            // Create a new spawner if none exists
            GameObject spawnerGO = new GameObject("DynamicEnemySpawner");
            spawner = spawnerGO.AddComponent<DynamicEnemySpawner>();
            Debug.Log("Created new DynamicEnemySpawner");
        }
        
        // Load example configurations if not already set
        if (spawner.availableEnemyTypes == null || spawner.availableEnemyTypes.Length == 0)
        {
            LoadExampleEnemyTypes(spawner);
        }
        
        if (spawner.availableSpecialWaves == null || spawner.availableSpecialWaves.Length == 0)
        {
            LoadExampleSpecialWaves(spawner);
        }
        
        if (spawner.performanceTracker == null)
        {
            LoadExamplePerformanceTracker(spawner);
        }
        
        Debug.Log("DynamicEnemySpawner setup complete!");
    }
    
    /// <summary>
    /// Load example enemy types from ScriptableObjects
    /// </summary>
    private void LoadExampleEnemyTypes(DynamicEnemySpawner spawner)
    {
        // Try to load from Resources folder first
        EnemyType[] loadedTypes = Resources.LoadAll<EnemyType>("ScriptableObjects/EnemyTypes");
        
        if (loadedTypes.Length > 0)
        {
            spawner.availableEnemyTypes = loadedTypes;
            Debug.Log($"Loaded {loadedTypes.Length} enemy types from Resources");
        }
        else
        {
            // Create default enemy types if none found
            CreateDefaultEnemyTypes(spawner);
        }
    }
    
    /// <summary>
    /// Load example special waves from ScriptableObjects
    /// </summary>
    private void LoadExampleSpecialWaves(DynamicEnemySpawner spawner)
    {
        // Try to load from Resources folder first
        SpecialWave[] loadedWaves = Resources.LoadAll<SpecialWave>("ScriptableObjects/SpecialWaves");
        
        if (loadedWaves.Length > 0)
        {
            spawner.availableSpecialWaves = loadedWaves;
            Debug.Log($"Loaded {loadedWaves.Length} special waves from Resources");
        }
        else
        {
            // Create default special waves if none found
            CreateDefaultSpecialWaves(spawner);
        }
    }
    
    /// <summary>
    /// Load or create example performance tracker
    /// </summary>
    private void LoadExamplePerformanceTracker(DynamicEnemySpawner spawner)
    {
        // Try to load from Resources folder first
        PlayerPerformanceTracker tracker = Resources.Load<PlayerPerformanceTracker>("ScriptableObjects/PlayerPerformanceTracker");
        
        if (tracker != null)
        {
            spawner.performanceTracker = tracker;
            Debug.Log("Loaded performance tracker from Resources");
        }
        else
        {
            // Create a new performance tracker
            spawner.performanceTracker = ScriptableObject.CreateInstance<PlayerPerformanceTracker>();
            Debug.Log("Created new performance tracker");
        }
    }
    
    /// <summary>
    /// Create default enemy types if none are found
    /// </summary>
    private void CreateDefaultEnemyTypes(DynamicEnemySpawner spawner)
    {
        Debug.Log("Creating default enemy types...");
        
        // Basic Enemy
        EnemyType basicEnemy = ScriptableObject.CreateInstance<EnemyType>();
        basicEnemy.enemyName = "Basic Enemy";
        basicEnemy.spawnCost = 10;
        basicEnemy.spawnWeight = 1f;
        basicEnemy.baseHealth = 10f;
        basicEnemy.baseSpeed = 3f;
        basicEnemy.baseDamage = 5f;
        basicEnemy.enemyColor = Color.red;
        
        // Fast Enemy
        EnemyType fastEnemy = ScriptableObject.CreateInstance<EnemyType>();
        fastEnemy.enemyName = "Fast Enemy";
        fastEnemy.spawnCost = 15;
        fastEnemy.spawnWeight = 0.8f;
        fastEnemy.minWaveToAppear = 3;
        fastEnemy.baseHealth = 5f;
        fastEnemy.baseSpeed = 6f;
        fastEnemy.baseDamage = 3f;
        fastEnemy.isFast = true;
        fastEnemy.enemyColor = Color.green;
        fastEnemy.scaleMultiplier = 0.8f;
        
        // Tank Enemy
        EnemyType tankEnemy = ScriptableObject.CreateInstance<EnemyType>();
        tankEnemy.enemyName = "Tank Enemy";
        tankEnemy.spawnCost = 25;
        tankEnemy.spawnWeight = 0.6f;
        tankEnemy.minWaveToAppear = 5;
        tankEnemy.baseHealth = 30f;
        tankEnemy.baseSpeed = 1.5f;
        tankEnemy.baseDamage = 8f;
        tankEnemy.isTank = true;
        tankEnemy.enemyColor = new Color(0.5f, 0.5f, 1f);
        tankEnemy.scaleMultiplier = 1.3f;
        
        spawner.availableEnemyTypes = new EnemyType[] { basicEnemy, fastEnemy, tankEnemy };
    }
    
    /// <summary>
    /// Create default special waves if none are found
    /// </summary>
    private void CreateDefaultSpecialWaves(DynamicEnemySpawner spawner)
    {
        Debug.Log("Creating default special waves...");
        
        // Fast Rush Wave
        SpecialWave fastRush = ScriptableObject.CreateInstance<SpecialWave>();
        fastRush.waveName = "Fast Rush";
        fastRush.description = "A wave of fast enemies that rush the player";
        fastRush.minWaveToAppear = 5;
        fastRush.selectionWeight = 1.5f;
        fastRush.enemySpeedMultiplier = 1.5f;
        fastRush.enemyHealthMultiplier = 0.8f;
        
        // Create phases for fast rush
        WavePhase[] fastRushPhases = new WavePhase[3];
        
        fastRushPhases[0] = new WavePhase();
        fastRushPhases[0].phaseName = "Preparation";
        fastRushPhases[0].duration = 3f;
        fastRushPhases[0].spawnPattern = SpawnPattern.Steady;
        fastRushPhases[0].spawnRate = 0.5f;
        fastRushPhases[0].preferFastEnemies = true;
        fastRushPhases[0].isCalmPhase = true;
        fastRushPhases[0].phaseMessage = "Fast enemies approaching...";
        
        fastRushPhases[1] = new WavePhase();
        fastRushPhases[1].phaseName = "Rush";
        fastRushPhases[1].duration = 12f;
        fastRushPhases[1].spawnPattern = SpawnPattern.Burst;
        fastRushPhases[1].spawnRate = 2f;
        fastRushPhases[1].preferFastEnemies = true;
        fastRushPhases[1].isChaoticPhase = true;
        fastRushPhases[1].phaseMessage = "RUSH!";
        
        fastRushPhases[2] = new WavePhase();
        fastRushPhases[2].phaseName = "Relief";
        fastRushPhases[2].duration = 5f;
        fastRushPhases[2].spawnPattern = SpawnPattern.Steady;
        fastRushPhases[2].spawnRate = 0.2f;
        fastRushPhases[2].isReliefPhase = true;
        fastRushPhases[2].phaseMessage = "Catch your breath...";
        
        fastRush.phases = fastRushPhases;
        
        spawner.availableSpecialWaves = new SpecialWave[] { fastRush };
    }
    
    /// <summary>
    /// Get setup status information
    /// </summary>
    [ContextMenu("Check Setup Status")]
    public void CheckSetupStatus()
    {
        DynamicEnemySpawner spawner = FindObjectOfType<DynamicEnemySpawner>();
        
        if (spawner == null)
        {
            Debug.LogWarning("No DynamicEnemySpawner found in scene!");
            return;
        }
        
        string status = "Dynamic Spawner Setup Status:\n";
        status += $"Enemy Types: {(spawner.availableEnemyTypes != null ? spawner.availableEnemyTypes.Length : 0)}\n";
        status += $"Special Waves: {(spawner.availableSpecialWaves != null ? spawner.availableSpecialWaves.Length : 0)}\n";
        status += $"Performance Tracker: {(spawner.performanceTracker != null ? "✓" : "✗")}\n";
        status += $"Base Point Budget: {spawner.basePointBudget}\n";
        status += $"Point Budget Increase: {spawner.pointBudgetIncreasePerWave}\n";
        
        Debug.Log(status);
    }
}

#if UNITY_EDITOR
/// <summary>
/// Editor extension for easy setup
/// </summary>
[CustomEditor(typeof(DynamicSpawnerSetup))]
public class DynamicSpawnerSetupEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        DynamicSpawnerSetup setup = (DynamicSpawnerSetup)target;
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Setup Dynamic Spawner"))
        {
            setup.SetupDynamicSpawner();
        }
        
        if (GUILayout.Button("Check Setup Status"))
        {
            setup.CheckSetupStatus();
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Use the buttons above to quickly set up the Dynamic Enemy Spawner system.", MessageType.Info);
    }
}
#endif
