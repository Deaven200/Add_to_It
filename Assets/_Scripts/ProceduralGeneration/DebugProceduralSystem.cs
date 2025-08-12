using UnityEngine;

public class DebugProceduralSystem : MonoBehaviour
{
    [Header("Debug Controls")]
    [SerializeField] private bool enableDebugMode = false;
    [SerializeField] private bool showPerformanceInfo = false;
    
    [Header("Test Settings")]
    [SerializeField] private bool testChunkCreation = false;
    [SerializeField] private bool clearAllChunks = false;
    [SerializeField] private bool resetSystem = false;
    
    private ProceduralLevelManager levelManager;
    private PerformanceMonitor performanceMonitor;
    
    void Start()
    {
        // Find the procedural level manager
        levelManager = FindObjectOfType<ProceduralLevelManager>();
        if (levelManager == null)
        {
            Debug.LogError("DebugProceduralSystem: No ProceduralLevelManager found in scene!");
            return;
        }
        
        // Find or create performance monitor
        performanceMonitor = FindObjectOfType<PerformanceMonitor>();
        if (performanceMonitor == null)
        {
            GameObject monitorGO = new GameObject("PerformanceMonitor");
            performanceMonitor = monitorGO.AddComponent<PerformanceMonitor>();
        }
        
        Debug.Log("DebugProceduralSystem initialized. Check the console for system status.");
        LogSystemStatus();
    }
    
    void Update()
    {
        if (!enableDebugMode) return;
        
        // Handle test controls
        if (testChunkCreation)
        {
            testChunkCreation = false;
            TestChunkCreation();
        }
        
        if (clearAllChunks)
        {
            clearAllChunks = false;
            ClearAllChunks();
        }
        
        if (resetSystem)
        {
            resetSystem = false;
            ResetSystem();
        }
        
        // Log performance info periodically
        if (showPerformanceInfo && Time.frameCount % 120 == 0)
        {
            LogPerformanceInfo();
        }
    }
    
    void TestChunkCreation()
    {
        if (levelManager == null) return;
        
        Debug.Log("=== Testing Chunk Creation ===");
        
        // Test creating a chunk at origin
        Vector2Int testPos = new Vector2Int(0, 0);
        Chunk testChunk = levelManager.GetChunkAt(testPos);
        
        if (testChunk == null)
        {
            Debug.Log("No chunk at origin - this is normal if chunks are generated around player");
        }
        else
        {
            Debug.Log($"Found chunk at origin: {testChunk.name}");
        }
        
        Debug.Log($"Active Chunks: {levelManager.ActiveChunkCount}");
        Debug.Log($"Pooled Chunks: {levelManager.PooledChunkCount}");
    }
    
    void ClearAllChunks()
    {
        if (levelManager == null) return;
        
        Debug.Log("=== Clearing All Chunks ===");
        levelManager.StartNewGame();
        Debug.Log("All chunks cleared and system reset");
    }
    
    void ResetSystem()
    {
        if (levelManager == null) return;
        
        Debug.Log("=== Resetting Procedural System ===");
        
        // Disable the level manager temporarily
        levelManager.enabled = false;
        
        // Wait a frame
        StartCoroutine(ResetAfterDelay());
    }
    
    System.Collections.IEnumerator ResetAfterDelay()
    {
        yield return new WaitForEndOfFrame();
        
        // Re-enable the level manager
        levelManager.enabled = true;
        
        // Clear and regenerate
        levelManager.StartNewGame();
        
        Debug.Log("System reset complete");
    }
    
    void LogSystemStatus()
    {
        if (levelManager == null) return;
        
        Debug.Log("=== Procedural System Status ===");
        Debug.Log($"Level Manager Active: {levelManager.enabled}");
        Debug.Log($"Chunk Size: {levelManager.ChunkSize}");
        Debug.Log($"Render Distance: {levelManager.RenderDistance}");
        Debug.Log($"Active Chunks: {levelManager.ActiveChunkCount}");
        Debug.Log($"Pooled Chunks: {levelManager.PooledChunkCount}");
        Debug.Log($"Persistence Enabled: {levelManager.EnablePersistence}");
        
        // Check for player
        Transform player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player != null)
        {
            Debug.Log($"Player Found: {player.name} at {player.position}");
        }
        else
        {
            Debug.LogWarning("No player found with 'Player' tag!");
        }
        
        // Check for terrain generator
        TerrainGenerator terrainGen = levelManager.GetComponent<TerrainGenerator>();
        if (terrainGen != null)
        {
            Debug.Log("TerrainGenerator component found");
        }
        else
        {
            Debug.LogWarning("No TerrainGenerator component found!");
        }
    }
    
    void LogPerformanceInfo()
    {
        if (performanceMonitor == null) return;
        
        Debug.Log("=== Performance Info ===");
        Debug.Log($"FPS: {performanceMonitor.GetFPS():F1}");
        Debug.Log($"Avg FPS: {performanceMonitor.GetAverageFPS():F1}");
        Debug.Log($"Active Chunks: {performanceMonitor.GetActiveChunks()}");
        Debug.Log($"Pooled Chunks: {performanceMonitor.GetPooledChunks()}");
        Debug.Log($"Memory Used: {performanceMonitor.GetUsedMemory()}");
    }
    
    void OnGUI()
    {
        if (!enableDebugMode) return;
        
        GUILayout.BeginArea(new Rect(10, 10, 300, 400));
        GUILayout.BeginVertical("box");
        
        GUILayout.Label("Procedural System Debug", GUI.skin.box);
        
        if (levelManager != null)
        {
            GUILayout.Label($"Active Chunks: {levelManager.ActiveChunkCount}");
            GUILayout.Label($"Pooled Chunks: {levelManager.PooledChunkCount}");
            GUILayout.Label($"Chunk Size: {levelManager.ChunkSize}");
            GUILayout.Label($"Render Distance: {levelManager.RenderDistance}");
        }
        else
        {
            GUILayout.Label("No Level Manager Found!");
        }
        
        if (performanceMonitor != null)
        {
            GUILayout.Space(10);
            GUILayout.Label($"FPS: {performanceMonitor.GetFPS():F1}");
            GUILayout.Label($"Memory: {performanceMonitor.GetUsedMemory()}");
        }
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Test Chunk Creation"))
        {
            TestChunkCreation();
        }
        
        if (GUILayout.Button("Clear All Chunks"))
        {
            ClearAllChunks();
        }
        
        if (GUILayout.Button("Reset System"))
        {
            ResetSystem();
        }
        
        if (GUILayout.Button("Log Status"))
        {
            LogSystemStatus();
        }
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
}
