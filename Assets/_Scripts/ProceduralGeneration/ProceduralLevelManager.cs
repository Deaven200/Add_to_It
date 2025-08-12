using System.Collections.Generic;
using UnityEngine;

public class ProceduralLevelManager : MonoBehaviour
{
    [Header("Player Setup")]
    [SerializeField] private bool autoSetup = true;
    [SerializeField] private GameObject playerPrefab; // Assign your player prefab here
    
    [Header("Chunk Settings")]
    [Tooltip("Size of each chunk in world units")]
    [SerializeField] private int chunkSize = 10;
    [Tooltip("How many chunks to load around the player")]
    [SerializeField] private int renderDistance = 16;
    [Tooltip("The player transform to track for chunk generation")]
    [SerializeField] private Transform player;
    [Tooltip("Prefab to use for chunks (leave empty for dynamic generation)")]
    [SerializeField] private GameObject chunkPrefab;
    
    [Header("Fog Settings")]
    [Tooltip("Enable fog to hide ungenerated areas")]
    [SerializeField] private bool enableFog = true;
    [Tooltip("Distance where fog starts to appear")]
    [SerializeField] private float fogDistance = 50f;
    [Tooltip("Color of the fog")]
    [SerializeField] private Color fogColor = Color.gray;
    
    [Header("Persistence")]
    [Tooltip("Enable chunk persistence across game sessions")]
    [SerializeField] private bool enablePersistence = true;
    
    [Header("Debug")]
    [Tooltip("Show debug information in console")]
    [SerializeField] private bool showDebugInfo = true;
    
    private Dictionary<Vector2Int, GameObject> activeChunks = new Dictionary<Vector2Int, GameObject>();
    private Vector2Int lastPlayerChunk;
    private ChunkPersistenceManager persistenceManager;
    
    // Public properties for easy access
    public int ChunkSize => chunkSize;
    public int RenderDistance => renderDistance;
    public bool EnablePersistence => enablePersistence;
    
    void Start()
    {
        if (autoSetup)
        {
            SetupProceduralLevel();
        }
    }
    
    void SetupProceduralLevel()
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
        if (enableFog)
        {
            RenderSettings.fog = true;
            RenderSettings.fogColor = fogColor;
            RenderSettings.fogMode = FogMode.Linear;
            RenderSettings.fogStartDistance = 0f;
            RenderSettings.fogEndDistance = fogDistance;
        }
        
        // Set up persistence manager
        SetupPersistenceManager();
        
        // Generate initial chunks around player
        lastPlayerChunk = GetChunkPosition(player.position);
        GenerateChunksAroundPlayer();
        
        if (showDebugInfo)
        {
            Debug.Log($"ProceduralLevelManager initialized - Chunk Size: {chunkSize}, Render Distance: {renderDistance}");
        }
    }
    
    void CreatePlayer()
    {
        GameObject playerObject;
        
        if (playerPrefab != null)
        {
            // Use provided player prefab
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
            }
        }
        
        player = playerObject.transform;
    }
    
    void SetupPersistenceManager()
    {
        if (enablePersistence)
        {
            // Find or create persistence manager
            persistenceManager = FindObjectOfType<ChunkPersistenceManager>();
            if (persistenceManager == null)
            {
                GameObject persistenceGO = new GameObject("ChunkPersistenceManager");
                persistenceManager = persistenceGO.AddComponent<ChunkPersistenceManager>();
            }
            
            persistenceManager.SetChunkManager(this);
        }
    }
    
    void Update()
    {
        if (player == null) return;
        
        Vector2Int currentPlayerChunk = GetChunkPosition(player.position);
        
        // Only regenerate chunks if player moved to a new chunk
        if (currentPlayerChunk != lastPlayerChunk)
        {
            lastPlayerChunk = currentPlayerChunk;
            GenerateChunksAroundPlayer();
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
        Vector2Int playerChunk = GetChunkPosition(player.position);
        HashSet<Vector2Int> chunksToKeep = new HashSet<Vector2Int>();
        
        // Generate chunks in render distance
        for (int x = -renderDistance; x <= renderDistance; x++)
        {
            for (int z = -renderDistance; z <= renderDistance; z++)
            {
                Vector2Int chunkPos = new Vector2Int(
                    playerChunk.x + x,
                    playerChunk.y + z
                );
                
                chunksToKeep.Add(chunkPos);
                
                if (!activeChunks.ContainsKey(chunkPos))
                {
                    CreateChunk(chunkPos);
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
    
    void CreateChunk(Vector2Int chunkPos, bool wasPreviouslySaved = false)
    {
        Vector3 worldPos = new Vector3(chunkPos.x * chunkSize, -0.5f, chunkPos.y * chunkSize);
        GameObject chunk;
        
        // Always create chunk dynamically for now (more reliable)
        chunk = new GameObject($"Chunk_{chunkPos.x}_{chunkPos.y}");
        chunk.transform.SetParent(transform);
        chunk.transform.position = worldPos;
        
        // Add visual representation
        GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cube);
        visual.transform.SetParent(chunk.transform);
        visual.transform.localPosition = Vector3.zero;
        visual.transform.localScale = new Vector3(chunkSize, 1, chunkSize);
        
        // Set material to make it visible
        Renderer renderer = visual.GetComponent<Renderer>();
        if (renderer != null)
        {
            // Create a simple material if none exists
            Material material = new Material(Shader.Find("Standard"));
            material.color = new Color(0.5f, 0.5f, 0.5f, 1f); // Gray color
            renderer.material = material;
        }
        
        // Use the visual's collider instead of creating a separate one
        // This ensures the collider matches the visual exactly
        BoxCollider visualCollider = visual.GetComponent<BoxCollider>();
        if (visualCollider != null)
        {
            // Make sure the collider is properly sized
            visualCollider.size = new Vector3(chunkSize, 1, chunkSize);
            visualCollider.center = Vector3.zero;
        }
        
        // Add chunk component if it doesn't exist
        Chunk chunkComponent = chunk.GetComponent<Chunk>();
        if (chunkComponent == null)
        {
            chunkComponent = chunk.AddComponent<Chunk>();
        }
        chunkComponent.Initialize(chunkPos, chunkSize);
        
        activeChunks.Add(chunkPos, chunk);
        
        // Save chunk data if persistence is enabled
        if (enablePersistence && persistenceManager != null)
        {
            persistenceManager.SaveChunk(chunkPos, worldPos, true);
        }
        
        // Only log if this was a previously saved chunk (less spam)
        if (showDebugInfo && wasPreviouslySaved)
        {
            Debug.Log($"Restored previously saved chunk at {chunkPos}");
        }
    }
    
    void RemoveChunk(Vector2Int chunkPos)
    {
        if (activeChunks.TryGetValue(chunkPos, out GameObject chunk))
        {
            // Save chunk data before destroying (for persistence)
            if (enablePersistence && persistenceManager != null)
            {
                persistenceManager.SaveChunk(chunkPos, chunk.transform.position, true);
            }
            
            Destroy(chunk);
            activeChunks.Remove(chunkPos);
        }
    }
    
    public void StartNewGame()
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
                Destroy(chunk);
            }
        }
        activeChunks.Clear();
        
        // Regenerate chunks around current player position
        if (player != null)
        {
            lastPlayerChunk = GetChunkPosition(player.position);
            GenerateChunksAroundPlayer();
        }
        
        Debug.Log("Started new game - all chunks cleared and regenerated");
    }
    
    // Public method to get active chunk count
    public int GetActiveChunkCount()
    {
        return activeChunks.Count;
    }
    
    // Public method to get all active chunk positions
    public List<Vector2Int> GetActiveChunkPositions()
    {
        return new List<Vector2Int>(activeChunks.Keys);
    }
    
    void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            Vector2Int playerChunk = GetChunkPosition(player.position);
            Vector3 chunkCenter = new Vector3(
                playerChunk.x * chunkSize + chunkSize / 2f,
                0,
                playerChunk.y * chunkSize + chunkSize / 2f
            );
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(chunkCenter, new Vector3(chunkSize, 1, chunkSize));
            
            // Draw render distance
            Gizmos.color = Color.green;
            float renderSize = renderDistance * chunkSize;
            Gizmos.DrawWireCube(chunkCenter, new Vector3(renderSize * 2, 1, renderSize * 2));
        }
    }
}
