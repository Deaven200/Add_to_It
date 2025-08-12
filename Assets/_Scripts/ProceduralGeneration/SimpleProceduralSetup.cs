using UnityEngine;

public class SimpleProceduralSetup : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private bool autoSetup = true;
    [SerializeField] private GameObject playerPrefab; // Assign your player prefab here
    
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
        
        // Create chunk generator
        CreateChunkGenerator();
        
        // Set up camera
        SetupCamera();
    }
    
    void CreatePlayer()
    {
        GameObject player;
        
        if (playerPrefab != null)
        {
            // Use the assigned player prefab
            player = Instantiate(playerPrefab, new Vector3(0, 2, 0), Quaternion.identity);
            Debug.Log("Spawned player from prefab");
        }
        else
        {
            // Fallback to creating a simple player
            player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "Player";
            player.tag = "Player";
            player.transform.position = new Vector3(0, 2, 0);
            
            // Remove the default collider from the primitive
            DestroyImmediate(player.GetComponent<Collider>());
            
            // Add CharacterController (required by PlayerMovement script)
            CharacterController characterController = player.AddComponent<CharacterController>();
            characterController.height = 2f;
            characterController.radius = 0.5f;
            characterController.center = Vector3.zero;
            
            // Add player movement script
            if (player.GetComponent<PlayerMovement>() == null)
            {
                player.AddComponent<PlayerMovement>();
            }
            
            Debug.Log("Created test player with CharacterController");
        }
    }
    
    void CreateChunkGenerator()
    {
        // Create chunk generator GameObject
        GameObject chunkGeneratorGO = new GameObject("ChunkGenerator");
        ChunkGenerator chunkGenerator = chunkGeneratorGO.AddComponent<ChunkGenerator>();
        
        // Set up the chunk generator through reflection
        SetPrivateField(chunkGenerator, "chunkSize", 16);
        SetPrivateField(chunkGenerator, "renderDistance", 3);
        SetPrivateField(chunkGenerator, "enableFog", true);
        SetPrivateField(chunkGenerator, "fogDistance", 50f);
        SetPrivateField(chunkGenerator, "fogColor", Color.gray);
        
        // Set player reference
        Transform player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player != null)
        {
            SetPrivateField(chunkGenerator, "player", player);
        }
        
        Debug.Log("Created chunk generator");
    }
    
    void SetupCamera()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                // Position camera above and behind player
                mainCamera.transform.position = player.transform.position + new Vector3(0, 5, -10);
                mainCamera.transform.LookAt(player.transform);
                
                // Add camera controller if it exists
                if (mainCamera.GetComponent<CameraController>() == null)
                {
                    mainCamera.gameObject.AddComponent<CameraController>();
                }
            }
        }
    }
    
    void SetPrivateField(object obj, string fieldName, object value)
    {
        var field = obj.GetType().GetField(fieldName, 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field != null)
        {
            field.SetValue(obj, value);
        }
        else
        {
            Debug.LogWarning($"Field {fieldName} not found on {obj.GetType().Name}");
        }
    }
}
