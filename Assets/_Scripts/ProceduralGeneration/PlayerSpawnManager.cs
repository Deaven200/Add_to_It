using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages player spawning when teleporting into procedural generation scenes.
/// This ensures the player can play the procedural level when teleporting in.
/// </summary>
public class PlayerSpawnManager : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Vector3 spawnPosition = new Vector3(0, 10, 0); // Spawn above ground
    [SerializeField] private bool spawnOnSceneLoad = true;
    [SerializeField] private Transform spawnPoint; // Optional spawn point with freeze script
    
    [Header("Scene Settings")]
    [SerializeField] private string[] proceduralScenes = { "Main_level" };
    
    private GameObject currentPlayer;
    private ProceduralLevelManager levelManager;
    
    void Awake()
    {
        // Subscribe to scene loading events
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    void Start()
    {
        // If we're already in a procedural scene, spawn the player
        if (spawnOnSceneLoad && IsProceduralScene(SceneManager.GetActiveScene().name))
        {
            SpawnPlayer();
        }
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (IsProceduralScene(scene.name))
        {
            // Wait a frame to ensure the ProceduralLevelManager is initialized
            Invoke(nameof(SpawnPlayer), 0.1f);
        }
    }
    
    void SpawnPlayer()
    {
        if (currentPlayer != null)
        {
            Destroy(currentPlayer);
        }
        
        if (playerPrefab == null)
        {
            Debug.LogError("PlayerSpawnManager: No player prefab assigned!");
            return;
        }
        
        // Find the ProceduralLevelManager
        levelManager = FindObjectOfType<ProceduralLevelManager>();
        if (levelManager == null)
        {
            Debug.LogError("PlayerSpawnManager: No ProceduralLevelManager found in scene!");
            return;
        }
        
        // Spawn the player at spawn point or default position
        Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : spawnPosition;
        currentPlayer = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
        
        // Set the player reference in the ProceduralLevelManager
        levelManager.SetPlayer(currentPlayer);
        
        // Enable player movement after a short delay to ensure terrain is generated
        StartCoroutine(EnablePlayerAfterDelay());
        
        Debug.Log($"Player spawned at {spawnPosition} in {SceneManager.GetActiveScene().name}");
    }
    
    System.Collections.IEnumerator EnablePlayerAfterDelay()
    {
        // Wait for the procedural level to initialize
        yield return new WaitForSeconds(2f);
        
        if (currentPlayer != null)
        {
            // Enable player movement components
            var playerMovement = currentPlayer.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.enabled = true;
            }
            
            // Enable camera controller
            var cameraController = currentPlayer.GetComponent<CameraController>();
            if (cameraController != null)
            {
                cameraController.enabled = true;
            }
            
            Debug.Log("Player movement enabled!");
        }
    }
    
    bool IsProceduralScene(string sceneName)
    {
        foreach (string proceduralScene in proceduralScenes)
        {
            if (sceneName.Equals(proceduralScene, System.StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }
    
    // Public method to manually spawn player
    public void SpawnPlayerManually()
    {
        SpawnPlayer();
    }
    
    // Public method to set spawn position
    public void SetSpawnPosition(Vector3 position)
    {
        spawnPosition = position;
    }
    
    // Public method to get current player
    public GameObject GetCurrentPlayer()
    {
        return currentPlayer;
    }
}
