using UnityEngine;

public class TestImprovedSystem : MonoBehaviour
{
    [Header("Test Settings")]
    [SerializeField] private bool testOnStart = true;
    [SerializeField] private bool createTerrainSettings = true;
    
    void Start()
    {
        if (testOnStart)
        {
            TestSystem();
        }
    }
    
    [ContextMenu("Test Improved System")]
    public void TestSystem()
    {
        Debug.Log("=== Testing Improved Procedural Generation System ===");
        
        // Test 1: Check if ProceduralLevelManager exists
        ProceduralLevelManager levelManager = FindObjectOfType<ProceduralLevelManager>();
        if (levelManager != null)
        {
            Debug.Log("✅ ProceduralLevelManager found and working");
            Debug.Log($"   - Active Chunks: {levelManager.ActiveChunkCount}");
            Debug.Log($"   - Pooled Chunks: {levelManager.PooledChunkCount}");
            Debug.Log($"   - Chunk Size: {levelManager.ChunkSize}");
            Debug.Log($"   - Render Distance: {levelManager.RenderDistance}");
        }
        else
        {
            Debug.LogWarning("❌ ProceduralLevelManager not found");
        }
        
        // Test 2: Check if TerrainSettings exists
        TerrainSettings terrainSettings = FindObjectOfType<TerrainSettings>();
        if (terrainSettings == null && createTerrainSettings)
        {
            Debug.Log("Creating default TerrainSettings...");
            terrainSettings = ScriptableObject.CreateInstance<TerrainSettings>();
        }
        
        if (terrainSettings != null)
        {
            Debug.Log("✅ TerrainSettings found and working");
            Debug.Log($"   - Noise Scale: {terrainSettings.NoiseScale}");
            Debug.Log($"   - Octaves: {terrainSettings.Octaves}");
            Debug.Log($"   - Biomes: {terrainSettings.Biomes.Length}");
        }
        else
        {
            Debug.LogWarning("❌ TerrainSettings not found");
        }
        
        // Test 3: Check if TerrainGenerator exists
        TerrainGenerator terrainGenerator = FindObjectOfType<TerrainGenerator>();
        if (terrainGenerator != null)
        {
            Debug.Log("✅ TerrainGenerator found and working");
        }
        else
        {
            Debug.LogWarning("❌ TerrainGenerator not found");
        }
        
        // Test 4: Check if ChunkPersistenceManager exists
        ChunkPersistenceManager persistenceManager = FindObjectOfType<ChunkPersistenceManager>();
        if (persistenceManager != null)
        {
            Debug.Log("✅ ChunkPersistenceManager found and working");
            Debug.Log($"   - Saved Chunks: {persistenceManager.GetSavedChunkCount()}");
        }
        else
        {
            Debug.LogWarning("❌ ChunkPersistenceManager not found");
        }
        
        // Test 5: Check if PerformanceMonitor exists
        PerformanceMonitor performanceMonitor = FindObjectOfType<PerformanceMonitor>();
        if (performanceMonitor != null)
        {
            Debug.Log("✅ PerformanceMonitor found and working");
        }
        else
        {
            Debug.LogWarning("❌ PerformanceMonitor not found");
        }
        
        // Test 6: Check if Player exists
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Debug.Log("✅ Player found and working");
            Debug.Log($"   - Position: {player.transform.position}");
        }
        else
        {
            Debug.LogWarning("❌ Player not found (make sure it has 'Player' tag)");
        }
        
        Debug.Log("=== Test Complete ===");
    }
    
    [ContextMenu("Create Complete Setup")]
    public void CreateCompleteSetup()
    {
        Debug.Log("Creating complete procedural generation setup...");
        
        // Create ProceduralLevelManager if it doesn't exist
        ProceduralLevelManager levelManager = FindObjectOfType<ProceduralLevelManager>();
        if (levelManager == null)
        {
            GameObject managerGO = new GameObject("ProceduralLevelManager");
            levelManager = managerGO.AddComponent<ProceduralLevelManager>();
            Debug.Log("Created ProceduralLevelManager");
        }
        
        // Create TerrainSettings if it doesn't exist
        TerrainSettings terrainSettings = FindObjectOfType<TerrainSettings>();
        if (terrainSettings == null)
        {
            terrainSettings = ScriptableObject.CreateInstance<TerrainSettings>();
            Debug.Log("Created TerrainSettings");
        }
        
        // Create PerformanceMonitor if it doesn't exist
        PerformanceMonitor performanceMonitor = FindObjectOfType<PerformanceMonitor>();
        if (performanceMonitor == null)
        {
            GameObject monitorGO = new GameObject("PerformanceMonitor");
            performanceMonitor = monitorGO.AddComponent<PerformanceMonitor>();
            Debug.Log("Created PerformanceMonitor");
        }
        
        Debug.Log("Complete setup created successfully!");
    }
}
