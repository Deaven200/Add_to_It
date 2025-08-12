using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Threading.Tasks;

public class ProceduralLevelManager : MonoBehaviour
{
    [Header("Player Setup")]
    [SerializeField] private bool autoSetup = true;
    [SerializeField] private GameObject playerPrefab;
    
    [Header("Chunk Settings")]
    [SerializeField] private int chunkSize = 10;
    [SerializeField] private int renderDistance = 8; // Reduced from 16 to prevent too many chunks
    [SerializeField] private Transform player;
    [SerializeField] private GameObject chunkPrefab;
    
    [Header("Performance Settings")]
    [SerializeField] private int maxChunksInPool = 50; // Reduced from 100
    [SerializeField] private bool useAsyncGeneration = true;
    [SerializeField] private int chunksPerFrame = 1; // Reduced from 2
    [SerializeField] private float maxGenerationTime = 0.016f; // Max 16ms per frame
    
    [Header("Terrain Settings")]
    [SerializeField] private TerrainSettings terrainSettings;
    
    [Header("Fog Settings")]
    [SerializeField] private bool enableFog = true;
    [SerializeField] private float fogDistance = 50f;
    [SerializeField] private Color fogColor = Color.gray;
    
    [Header("Persistence")]
    [SerializeField] private bool enablePersistence = true;
    
    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = false;
    [SerializeField] private bool showPerformanceStats = false;
    [SerializeField] private bool showVerboseLogs = false;
    
    // Performance tracking
    private float lastFrameTime;
    private int totalChunksGenerated;
    private bool isGenerating = false;
    private bool isInitialized = false;
    
    // Chunk management
    private Dictionary<Vector2Int, Chunk> activeChunks = new Dictionary<Vector2Int, Chunk>();
    private Queue<Chunk> chunkPool = new Queue<Chunk>();
    private HashSet<Vector2Int> chunksToGenerate = new HashSet<Vector2Int>();
    private Vector2Int lastPlayerChunk;
    
    // Components
    private ChunkPersistenceManager persistenceManager;
    private TerrainGenerator terrainGenerator;
    
    // Public properties
    public int ChunkSize => chunkSize;
    public int RenderDistance => renderDistance;
    public bool EnablePersistence => enablePersistence;
    public int ActiveChunkCount => activeChunks.Count;
    public int PooledChunkCount => chunkPool.Count;
    
    void Start()
    {
        if (autoSetup)
        {
            SetupProceduralLevel();
        }
    }
    
    void SetupProceduralLevel()
    {
        try
        {
            // Create player if none exists
            if (GameObject.FindGameObjectWithTag("Player") == null)
            {
                CreatePlayer();
            }
            
            // Find the player
            if (player == null)
            {
                player = GameObject.FindGameObjectWithTag("Player")?.transform;
                if (player == null)
                {
                    Debug.LogError("ProceduralLevelManager: No player found! Make sure the player has the 'Player' tag.");
                    return;
                }
            }
            
            // Set up fog
            SetupFog();
            
            // Set up terrain generator
            SetupTerrainGenerator();
            
            // Set up persistence manager
            SetupPersistenceManager();
            
            // Initialize chunk pool
            InitializeChunkPool();
            
            // Generate initial chunks around player
            lastPlayerChunk = GetChunkPosition(player.position);
            GenerateChunksAroundPlayer();
            
            // Wait for initial chunks to be generated
            StartCoroutine(WaitForInitialGeneration());
            
            if (showDebugInfo)
            {
                Debug.Log($"ProceduralLevelManager initialized - Chunk Size: {chunkSize}, Render Distance: {renderDistance}");
            }
            else
            {
                Debug.Log("ProceduralLevelManager initialized successfully");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in SetupProceduralLevel: {e.Message}");
        }
    }
    
    void SetupFog()
    {
        if (enableFog)
        {
            RenderSettings.fog = true;
            RenderSettings.fogColor = fogColor;
            RenderSettings.fogMode = FogMode.Linear;
            RenderSettings.fogStartDistance = 0f;
            RenderSettings.fogEndDistance = fogDistance;
        }
    }
    
    void SetupTerrainGenerator()
    {
        terrainGenerator = GetComponent<TerrainGenerator>();
        if (terrainGenerator == null)
        {
            terrainGenerator = gameObject.AddComponent<TerrainGenerator>();
        }
        
        if (terrainSettings == null)
        {
            terrainSettings = ScriptableObject.CreateInstance<TerrainSettings>();
            Debug.LogWarning("No TerrainSettings assigned. Using default settings.");
        }
        
        terrainGenerator.Initialize(terrainSettings, chunkSize);
    }
    
    void SetupPersistenceManager()
    {
        if (enablePersistence)
        {
            persistenceManager = FindObjectOfType<ChunkPersistenceManager>();
            if (persistenceManager == null)
            {
                GameObject persistenceGO = new GameObject("ChunkPersistenceManager");
                persistenceManager = persistenceGO.AddComponent<ChunkPersistenceManager>();
            }
            
            persistenceManager.SetChunkManager(this);
        }
    }
    
    void InitializeChunkPool()
    {
        // Pre-populate the chunk pool with a smaller number
        int initialPoolSize = Mathf.Min(maxChunksInPool / 4, 10);
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreatePooledChunk();
        }
    }
    
    void CreatePooledChunk()
    {
        GameObject chunkGO = new GameObject("PooledChunk");
        chunkGO.transform.SetParent(transform);
        chunkGO.SetActive(false);
        
        Chunk chunk = chunkGO.AddComponent<Chunk>();
        chunkPool.Enqueue(chunk);
    }
    
    void CreatePlayer()
    {
        GameObject playerObject;
        
        if (playerPrefab != null)
        {
            playerObject = Instantiate(playerPrefab, new Vector3(0, 2, 0), Quaternion.identity);
        }
        else
        {
            // Create a simple test player
            playerObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            playerObject.name = "Player";
            playerObject.tag = "Player";
            playerObject.transform.position = new Vector3(0, 2, 0);
            
            // Add necessary components
            Rigidbody rb = playerObject.AddComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            
            // Add player movement script if it exists
            if (playerObject.GetComponent<PlayerMovement>() == null)
            {
                PlayerMovement movement = playerObject.AddComponent<PlayerMovement>();
                // Set up camera reference
                Camera mainCamera = Camera.main;
                if (mainCamera != null)
                {
                    movement.cameraTransform = mainCamera.transform;
                }
                
                // Disable movement until initialization is complete
                if (movement != null)
                {
                    movement.enabled = false;
                }
            }
        }
        
        player = playerObject.transform;
    }
    
    void Update()
    {
        if (player == null || !isInitialized) return;
        
        // Performance tracking
        lastFrameTime = Time.deltaTime;
        
        Vector2Int currentPlayerChunk = GetChunkPosition(player.position);
        
        // Only regenerate chunks if player moved to a new chunk
        if (currentPlayerChunk != lastPlayerChunk)
        {
            lastPlayerChunk = currentPlayerChunk;
            GenerateChunksAroundPlayer();
        }
        
        // Process chunk generation queue with time limit
        if (useAsyncGeneration && !isGenerating)
        {
            ProcessChunkGenerationQueue();
        }
        
        // Update performance stats
        if (showPerformanceStats && Time.frameCount % 60 == 0)
        {
            LogPerformanceStats();
        }
    }
    
    void ProcessChunkGenerationQueue()
    {
        if (chunksToGenerate.Count == 0) return;
        
        isGenerating = true;
        float startTime = Time.realtimeSinceStartup;
        int chunksProcessed = 0;
        
        try
        {
            while (chunksToGenerate.Count > 0 && 
                   chunksProcessed < chunksPerFrame && 
                   (Time.realtimeSinceStartup - startTime) < maxGenerationTime)
            {
                // Get the first chunk position from the set
                Vector2Int chunkPos = new Vector2Int();
                bool foundChunk = false;
                
                foreach (var pos in chunksToGenerate)
                {
                    chunkPos = pos;
                    foundChunk = true;
                    break;
                }
                
                if (!foundChunk) break;
                
                chunksToGenerate.Remove(chunkPos);
                
                if (!activeChunks.ContainsKey(chunkPos))
                {
                    CreateChunk(chunkPos);
                    chunksProcessed++;
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in ProcessChunkGenerationQueue: {e.Message}");
        }
        finally
        {
            isGenerating = false;
        }
    }
    
    Vector2Int GetChunkPosition(Vector3 worldPosition)
    {
        int chunkX = Mathf.FloorToInt(worldPosition.x / chunkSize);
        int chunkZ = Mathf.FloorToInt(worldPosition.z / chunkSize);
        return new Vector2Int(chunkX, chunkZ);
    }
    
    void GenerateChunksAroundPlayer()
    {
        if (isGenerating) return; // Prevent recursive calls
        
        try
        {
            Vector2Int playerChunk = GetChunkPosition(player.position);
            HashSet<Vector2Int> chunksToKeep = new HashSet<Vector2Int>();
            
            // Generate chunks in render distance (reduced range for performance)
            int actualRenderDistance = Mathf.Min(renderDistance, 8); // Cap at 8 for safety
            
            for (int x = -actualRenderDistance; x <= actualRenderDistance; x++)
            {
                for (int z = -actualRenderDistance; z <= actualRenderDistance; z++)
                {
                    Vector2Int chunkPos = new Vector2Int(
                        playerChunk.x + x,
                        playerChunk.y + z
                    );
                    
                    chunksToKeep.Add(chunkPos);
                    
                    if (!activeChunks.ContainsKey(chunkPos))
                    {
                        if (useAsyncGeneration)
                        {
                            chunksToGenerate.Add(chunkPos);
                        }
                        else
                        {
                            CreateChunk(chunkPos);
                        }
                    }
                }
            }
            
            // Remove chunks outside render distance
            List<Vector2Int> chunksToRemove = new List<Vector2Int>();
            foreach (var chunk in activeChunks.Keys)
            {
                if (!chunksToKeep.Contains(chunk))
                {
                    chunksToRemove.Add(chunk);
                }
            }
            
            foreach (var chunkPos in chunksToRemove)
            {
                RemoveChunk(chunkPos);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in GenerateChunksAroundPlayer: {e.Message}");
        }
    }
    
    void CreateChunk(Vector2Int chunkPos, bool wasPreviouslySaved = false)
    {
        try
        {
            Vector3 worldPos = new Vector3(chunkPos.x * chunkSize, 0, chunkPos.y * chunkSize);
            Chunk chunk;
            
            // Get chunk from pool or create new one
            if (chunkPool.Count > 0)
            {
                chunk = chunkPool.Dequeue();
                chunk.gameObject.SetActive(true);
                chunk.transform.position = worldPos;
            }
            else
            {
                GameObject chunkGO = new GameObject($"Chunk_{chunkPos.x}_{chunkPos.y}");
                chunkGO.transform.SetParent(transform);
                chunkGO.transform.position = worldPos;
                chunk = chunkGO.AddComponent<Chunk>();
            }
            
            // Initialize chunk with terrain generation
            chunk.Initialize(chunkPos, chunkSize, terrainGenerator);
            
            activeChunks.Add(chunkPos, chunk);
            totalChunksGenerated++;
            
            // Save chunk data if persistence is enabled
            if (enablePersistence && persistenceManager != null)
            {
                persistenceManager.SaveChunk(chunkPos, worldPos, true);
            }
            
            if (showDebugInfo && wasPreviouslySaved)
            {
                Debug.Log($"Restored previously saved chunk at {chunkPos}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error creating chunk at {chunkPos}: {e.Message}");
        }
    }
    
    void RemoveChunk(Vector2Int chunkPos)
    {
        try
        {
            if (activeChunks.TryGetValue(chunkPos, out Chunk chunk))
            {
                // Save chunk data before destroying (for persistence)
                if (enablePersistence && persistenceManager != null)
                {
                    persistenceManager.SaveChunk(chunkPos, chunk.transform.position, true);
                }
                
                // Return to pool instead of destroying
                if (chunkPool.Count < maxChunksInPool)
                {
                    chunk.gameObject.SetActive(false);
                    chunkPool.Enqueue(chunk);
                }
                else
                {
                    Destroy(chunk.gameObject);
                }
                
                activeChunks.Remove(chunkPos);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error removing chunk at {chunkPos}: {e.Message}");
        }
    }
    
    void LogPerformanceStats()
    {
        Debug.Log($"Performance Stats - FPS: {1f/lastFrameTime:F1}, Active Chunks: {activeChunks.Count}, " +
                  $"Pooled Chunks: {chunkPool.Count}, Total Generated: {totalChunksGenerated}, " +
                  $"Chunks in Queue: {chunksToGenerate.Count}");
    }
    
    public void StartNewGame()
    {
        try
        {
            if (persistenceManager != null)
            {
                persistenceManager.StartNewGame();
            }
            
            // Clear all active chunks
            foreach (var chunk in activeChunks.Values)
            {
                if (chunk != null)
                {
                    if (chunkPool.Count < maxChunksInPool)
                    {
                        chunk.gameObject.SetActive(false);
                        chunkPool.Enqueue(chunk);
                    }
                    else
                    {
                        Destroy(chunk.gameObject);
                    }
                }
            }
            activeChunks.Clear();
            chunksToGenerate.Clear();
            
            // Generate initial chunks
            if (player != null)
            {
                lastPlayerChunk = GetChunkPosition(player.position);
                GenerateChunksAroundPlayer();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in StartNewGame: {e.Message}");
        }
    }
    
    public Chunk GetChunkAt(Vector2Int position)
    {
        activeChunks.TryGetValue(position, out Chunk chunk);
        return chunk;
    }
    
    System.Collections.IEnumerator WaitForInitialGeneration()
    {
        if (showVerboseLogs)
        {
            Debug.Log("Waiting for initial chunk generation...");
        }
        
        // Wait a few frames to allow initial chunks to be queued
        yield return new WaitForSeconds(0.1f);
        
        // Process chunks until we have enough around the player
        int maxWaitFrames = 60; // Wait up to 1 second at 60fps
        int framesWaited = 0;
        
        while (chunksToGenerate.Count > 0 && framesWaited < maxWaitFrames)
        {
            // Process all chunks in the queue
            ProcessChunkGenerationQueue();
            
            // Wait a frame
            yield return null;
            framesWaited++;
        }
        
        // Ensure player is on solid ground
        EnsurePlayerOnGround();
        
        isInitialized = true;
        
        // Enable player movement
        EnablePlayerMovement();
        
        if (showDebugInfo)
        {
            Debug.Log($"Initialization complete! Active chunks: {activeChunks.Count}, Pooled chunks: {chunkPool.Count}");
        }
        else
        {
            Debug.Log("World ready! Player can now move.");
        }
    }
    
    void EnsurePlayerOnGround()
    {
        if (player == null) return;
        
        // Check if player is on a chunk
        Vector2Int playerChunk = GetChunkPosition(player.position);
        Chunk playerChunkObj = GetChunkAt(playerChunk);
        
        if (playerChunkObj == null)
        {
            Debug.LogWarning($"Player not on a chunk! Creating emergency chunk at {playerChunk}");
            CreateChunk(playerChunk);
        }
        
        // Move player slightly up to ensure they're on the ground
        Vector3 playerPos = player.position;
        playerPos.y = Mathf.Max(playerPos.y, 1f); // Ensure player is at least 1 unit above ground
        player.position = playerPos;
        
        if (showVerboseLogs)
        {
            Debug.Log($"Player positioned at {player.position}");
        }
    }
    
    void EnablePlayerMovement()
    {
        if (player == null) return;
        
        // Enable player movement script
        PlayerMovement movement = player.GetComponent<PlayerMovement>();
        if (movement != null)
        {
            movement.enabled = true;
            if (showVerboseLogs)
            {
                Debug.Log("Player movement enabled");
            }
        }
        
        // Enable player rigidbody if it was disabled
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }
    }
    
    void OnDrawGizmosSelected()
    {
        if (!showDebugInfo) return;
        
        // Draw chunk boundaries
        Gizmos.color = Color.yellow;
        foreach (var chunk in activeChunks.Values)
        {
            if (chunk != null)
            {
                Vector3 center = chunk.transform.position + new Vector3(chunkSize * 0.5f, 0, chunkSize * 0.5f);
                Gizmos.DrawWireCube(center, new Vector3(chunkSize, 1, chunkSize));
            }
        }
        
        // Draw player position
        if (player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(player.position, 0.5f);
        }
    }
    
    /// <summary>
    /// Sets the player reference for the procedural level manager.
    /// Called by PlayerSpawnManager when spawning a player.
    /// </summary>
    /// <param name="playerObject">The player GameObject to track</param>
    public void SetPlayer(GameObject playerObject)
    {
        if (playerObject == null)
        {
            Debug.LogError("ProceduralLevelManager: Cannot set null player!");
            return;
        }
        
        player = playerObject.transform;
        
        // Disable player movement until initialization is complete
        var playerMovement = playerObject.GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }
        
        // Disable camera controller until initialization is complete
        var cameraController = playerObject.GetComponent<CameraController>();
        if (cameraController != null)
        {
            cameraController.enabled = false;
        }
        
        if (showDebugInfo)
        {
            Debug.Log($"Player set to: {playerObject.name}");
        }
    }
}
