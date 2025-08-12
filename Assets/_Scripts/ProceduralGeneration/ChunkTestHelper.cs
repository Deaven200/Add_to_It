using UnityEngine;

public class ChunkTestHelper : MonoBehaviour
{
    [Header("Test Settings")]
    [SerializeField] private bool testOnStart = true;
    [SerializeField] private Vector2Int testChunkPosition = Vector2Int.zero;
    [SerializeField] private int testChunkSize = 10;
    
    [Header("Debug Info")]
    [SerializeField] private bool showDebugInfo = false;
    
    private ProceduralLevelManager levelManager;
    private TerrainGenerator terrainGenerator;
    
    void Start()
    {
        if (testOnStart)
        {
            TestChunkCreation();
        }
    }
    
    void TestChunkCreation()
    {
        Debug.Log("=== Testing Chunk Creation ===");
        
        // Find the level manager
        levelManager = FindObjectOfType<ProceduralLevelManager>();
        if (levelManager == null)
        {
            Debug.LogError("No ProceduralLevelManager found!");
            return;
        }
        
        // Find the terrain generator
        terrainGenerator = levelManager.GetComponent<TerrainGenerator>();
        if (terrainGenerator == null)
        {
            Debug.LogWarning("No TerrainGenerator found on ProceduralLevelManager!");
        }
        
        // Create a test chunk
        CreateTestChunk();
        
        // Check if player exists
        Transform player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player != null)
        {
            Debug.Log($"Player found at: {player.position}");
            
            // Check if player is on a chunk
            Vector2Int playerChunk = GetChunkPosition(player.position);
            Debug.Log($"Player is in chunk: {playerChunk}");
            
            Chunk playerChunkObj = levelManager.GetChunkAt(playerChunk);
            if (playerChunkObj != null)
            {
                Debug.Log($"Chunk at player position exists: {playerChunkObj.name}");
            }
            else
            {
                Debug.LogWarning("No chunk at player position!");
            }
        }
        else
        {
            Debug.LogWarning("No player found with 'Player' tag!");
        }
        
        Debug.Log($"Total active chunks: {levelManager.ActiveChunkCount}");
        Debug.Log($"Total pooled chunks: {levelManager.PooledChunkCount}");
    }
    
    void CreateTestChunk()
    {
        // Create a test chunk at origin
        GameObject testChunkGO = new GameObject($"TestChunk_{testChunkPosition.x}_{testChunkPosition.y}");
        testChunkGO.transform.SetParent(transform);
        testChunkGO.transform.position = new Vector3(
            testChunkPosition.x * testChunkSize, 
            0, 
            testChunkPosition.y * testChunkSize
        );
        
        Chunk testChunk = testChunkGO.AddComponent<Chunk>();
        testChunk.Initialize(testChunkPosition, testChunkSize, terrainGenerator);
        
        Debug.Log($"Created test chunk at {testChunkPosition}");
        
        // Check if the chunk has a mesh
        MeshFilter meshFilter = testChunk.GetComponent<MeshFilter>();
        if (meshFilter != null && meshFilter.mesh != null)
        {
            Debug.Log($"Test chunk has mesh with {meshFilter.mesh.vertexCount} vertices");
        }
        else
        {
            Debug.LogError("Test chunk has no mesh!");
        }
        
        // Check if the chunk has a collider
        MeshCollider meshCollider = testChunk.GetComponent<MeshCollider>();
        if (meshCollider != null && meshCollider.sharedMesh != null)
        {
            Debug.Log("Test chunk has collider");
        }
        else
        {
            Debug.LogError("Test chunk has no collider!");
        }
    }
    
    Vector2Int GetChunkPosition(Vector3 worldPosition)
    {
        int chunkX = Mathf.FloorToInt(worldPosition.x / testChunkSize);
        int chunkZ = Mathf.FloorToInt(worldPosition.z / testChunkSize);
        return new Vector2Int(chunkX, chunkZ);
    }
    
    void OnGUI()
    {
        if (!showDebugInfo) return;
        
        GUILayout.BeginArea(new Rect(10, 450, 300, 200));
        GUILayout.BeginVertical("box");
        
        GUILayout.Label("Chunk Test Helper", GUI.skin.box);
        
        if (levelManager != null)
        {
            GUILayout.Label($"Active Chunks: {levelManager.ActiveChunkCount}");
            GUILayout.Label($"Pooled Chunks: {levelManager.PooledChunkCount}");
        }
        
        Transform player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player != null)
        {
            Vector2Int playerChunk = GetChunkPosition(player.position);
            GUILayout.Label($"Player Chunk: {playerChunk}");
            GUILayout.Label($"Player Position: {player.position}");
        }
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Test Chunk Creation"))
        {
            TestChunkCreation();
        }
        
        if (GUILayout.Button("Create Test Chunk"))
        {
            CreateTestChunk();
        }
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
}
