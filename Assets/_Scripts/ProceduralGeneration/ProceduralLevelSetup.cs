using UnityEngine;

public class ProceduralLevelSetup : MonoBehaviour
{
    [Header("Scene Setup")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Vector3 playerSpawnPosition = new Vector3(0, 2, 0);
    
    [Header("Chunk Generator")]
    [SerializeField] private GameObject chunkPrefab;
    [SerializeField] private int chunkSize = 16;
    [SerializeField] private int renderDistance = 3;
    
    void Start()
    {
        SetupLevel();
    }
    
    void SetupLevel()
    {
        // Find or create ChunkGenerator
        ChunkGenerator chunkGenerator = FindObjectOfType<ChunkGenerator>();
        if (chunkGenerator == null)
        {
            GameObject generatorGO = new GameObject("ChunkGenerator");
            chunkGenerator = generatorGO.AddComponent<ChunkGenerator>();
        }
        
        // Set up chunk generator parameters
        var chunkGeneratorComponent = chunkGenerator.GetComponent<ChunkGenerator>();
        if (chunkGeneratorComponent != null)
        {
            // We'll set these through reflection since they're private
            SetPrivateField(chunkGeneratorComponent, "chunkPrefab", chunkPrefab);
            SetPrivateField(chunkGeneratorComponent, "chunkSize", chunkSize);
            SetPrivateField(chunkGeneratorComponent, "renderDistance", renderDistance);
        }
        
        // Spawn player if not already present
        if (GameObject.FindGameObjectWithTag("Player") == null && playerPrefab != null)
        {
            GameObject player = Instantiate(playerPrefab, playerSpawnPosition, Quaternion.identity);
            player.tag = "Player";
        }
        
        // Set up camera to follow player
        SetupCamera();
    }
    
    void SetupCamera()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                // Add camera controller if it doesn't exist
                CameraController cameraController = mainCamera.GetComponent<CameraController>();
                if (cameraController == null)
                {
                    cameraController = mainCamera.gameObject.AddComponent<CameraController>();
                }
                
                // Set camera position relative to player
                mainCamera.transform.position = player.transform.position + new Vector3(0, 5, -10);
                mainCamera.transform.LookAt(player.transform);
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
    }
}

