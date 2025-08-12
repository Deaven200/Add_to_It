using UnityEngine;

public class ProceduralLevelInitializer : MonoBehaviour
{
    [Header("Level Setup")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject chunkPrefab;
    [SerializeField] private Vector3 playerSpawnPosition = new Vector3(0, 2, 0);
    
    [Header("Chunk Settings")]
    [SerializeField] private int chunkSize = 16;
    [SerializeField] private int renderDistance = 3;
    
    void Start()
    {
        InitializeLevel();
    }
    
    void InitializeLevel()
    {
        // Create ProceduralLevelManager
        GameObject levelManagerGO = new GameObject("ProceduralLevelManager");
        ProceduralLevelManager levelManager = levelManagerGO.AddComponent<ProceduralLevelManager>();
        
        // Set up level manager through reflection since fields are private
        SetPrivateField(levelManager, "chunkPrefab", chunkPrefab);
        SetPrivateField(levelManager, "chunkSize", chunkSize);
        SetPrivateField(levelManager, "renderDistance", renderDistance);
        SetPrivateField(levelManager, "enableFog", true);
        SetPrivateField(levelManager, "fogDistance", 50f);
        SetPrivateField(levelManager, "fogColor", Color.gray);
        
        // Spawn player if not present
        if (GameObject.FindGameObjectWithTag("Player") == null && playerPrefab != null)
        {
            GameObject player = Instantiate(playerPrefab, playerSpawnPosition, Quaternion.identity);
            player.tag = "Player";
            
            // Set player reference in level manager
            SetPrivateField(levelManager, "player", player.transform);
        }
        else if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            // Set existing player reference
            Transform player = GameObject.FindGameObjectWithTag("Player").transform;
            SetPrivateField(levelManager, "player", player);
        }
        
        // Set up camera
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
                // Position camera above and behind player
                mainCamera.transform.position = player.transform.position + new Vector3(0, 5, -10);
                mainCamera.transform.LookAt(player.transform);
                
                // Add camera controller if it doesn't exist
                CameraController cameraController = mainCamera.GetComponent<CameraController>();
                if (cameraController == null)
                {
                    cameraController = mainCamera.gameObject.AddComponent<CameraController>();
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
