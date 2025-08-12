using UnityEngine;

public class LevelTestSetup : MonoBehaviour
{
    [Header("Test Setup")]
    [SerializeField] private bool autoSetup = true;
    [SerializeField] private GameObject testPlayerPrefab;
    [SerializeField] private GameObject testChunkPrefab;
    
    void Start()
    {
        if (autoSetup)
        {
            SetupTestLevel();
        }
    }
    
    void SetupTestLevel()
    {
        // Create a simple test player if none exists
        if (GameObject.FindGameObjectWithTag("Player") == null)
        {
            CreateTestPlayer();
        }
        
        // Create a simple chunk prefab if none exists
        if (testChunkPrefab == null)
        {
            CreateTestChunkPrefab();
        }
        
        // Initialize the procedural level
        ProceduralLevelInitializer initializer = gameObject.AddComponent<ProceduralLevelInitializer>();
        SetPrivateField(initializer, "playerPrefab", testPlayerPrefab);
        SetPrivateField(initializer, "chunkPrefab", testChunkPrefab);
        
        // Call the initialization
        initializer.SendMessage("InitializeLevel");
    }
    
    void CreateTestPlayer()
    {
        // Create a simple player cube
        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        player.name = "TestPlayer";
        player.tag = "Player";
        player.transform.position = new Vector3(0, 2, 0);
        
        // Add player movement script if it exists
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        if (playerMovement == null)
        {
            playerMovement = player.AddComponent<PlayerMovement>();
        }
        
        // Add rigidbody for physics
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = player.AddComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }
        
        testPlayerPrefab = player;
    }
    
    void CreateTestChunkPrefab()
    {
        // Create a simple chunk prefab
        GameObject chunk = new GameObject("TestChunkPrefab");
        
        // Add visual representation
        GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cube);
        visual.transform.SetParent(chunk.transform);
        visual.transform.localPosition = Vector3.zero;
        visual.transform.localScale = new Vector3(16, 1, 16);
        
        // Remove collider from visual
        DestroyImmediate(visual.GetComponent<Collider>());
        
        // Add collider to chunk
        BoxCollider chunkCollider = chunk.AddComponent<BoxCollider>();
        chunkCollider.size = new Vector3(16, 0.1f, 16);
        chunkCollider.center = new Vector3(0, -0.05f, 0);
        
        // Add chunk component
        Chunk chunkComponent = chunk.AddComponent<Chunk>();
        
        testChunkPrefab = chunk;
    }
    
    void SetPrivateField(object obj, string fieldName, object value)
    {
        var field = obj.GetType().GetField(fieldName, 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field != null)
        {
            field.SetValue(obj, value);
        }
    }
}
