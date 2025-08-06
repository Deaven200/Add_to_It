using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentPlayer : MonoBehaviour
{
    public static PersistentPlayer Instance;
    
    [Header("Spawn Settings")]
    [SerializeField] private string playerRoomSceneName = "PlayerRoom";
    [SerializeField] private string spawnPointName = "PlayerSpawnPoint";
    
    [Header("Death Settings")]
    [SerializeField] private float respawnDelay = 2f;
    
    private bool isDead = false;
    private Vector3 lastSpawnPosition;
    private PlayerHealth playerHealth;
    
    void Awake()
    {
        // Singleton pattern - only one persistent player
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Subscribe to scene loading
            SceneManager.sceneLoaded += OnSceneLoaded;
            
            Debug.Log("Persistent Player created and will survive scene changes");
        }
        else
        {
            // Destroy duplicate players
            Debug.Log("Duplicate player found and destroyed");
            Destroy(gameObject);
            return;
        }
        
        // Get player health component
        playerHealth = GetComponent<PlayerHealth>();
        if (playerHealth == null)
        {
            Debug.LogError("PersistentPlayer requires a PlayerHealth component!");
        }
    }
    
    void Start()
    {
        // Position player at spawn point if we're in PlayerRoom
        if (SceneManager.GetActiveScene().name == playerRoomSceneName)
        {
            MoveToSpawnPoint();
        }
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene loaded: {scene.name}");
        
        // Reset death state when entering new scene
        isDead = false;
        
        // Enable player movement if it was disabled
        EnablePlayerControls();
        
        // Move to spawn point if we're in PlayerRoom
        if (scene.name == playerRoomSceneName)
        {
            MoveToSpawnPoint();
            
            // Restore player health when respawning
            if (playerHealth != null)
            {
                playerHealth.RestoreFullHealth();
            }
        }
        else
        {
            // For other scenes, try to find a spawn point or use default position
            Transform spawnPoint = GameObject.Find(spawnPointName)?.transform;
            if (spawnPoint != null)
            {
                transform.position = spawnPoint.position;
                transform.rotation = spawnPoint.rotation;
                lastSpawnPosition = spawnPoint.position;
            }
        }
    }
    
    private void MoveToSpawnPoint()
    {
        Transform spawnPoint = GameObject.Find(spawnPointName)?.transform;
        
        if (spawnPoint != null)
        {
            transform.position = spawnPoint.position;
            transform.rotation = spawnPoint.rotation;
            lastSpawnPosition = spawnPoint.position;
            Debug.Log($"Player moved to spawn point: {spawnPoint.name}");
        }
        else
        {
            Debug.LogWarning($"Spawn point '{spawnPointName}' not found in scene '{SceneManager.GetActiveScene().name}'");
        }
    }
    
    public void OnPlayerDeath()
    {
        if (isDead) return; // Prevent multiple death calls
        
        isDead = true;
        Debug.Log("Player died! Respawning in PlayerRoom...");
        
        // Disable player controls
        DisablePlayerControls();
        
        // Start respawn process after delay
        Invoke(nameof(RespawnInPlayerRoom), respawnDelay);
    }
    
    private void RespawnInPlayerRoom()
    {
        // Load the PlayerRoom scene
        SceneManager.LoadScene(playerRoomSceneName);
    }
    
    private void DisablePlayerControls()
    {
        // Disable movement
        PlayerMovement movement = GetComponent<PlayerMovement>();
        if (movement != null)
        {
            movement.enabled = false;
        }
        
        // Disable shooting
        PlayerShooting shooting = GetComponent<PlayerShooting>();
        if (shooting != null)
        {
            shooting.enabled = false;
        }
        
        // Disable camera
        CameraController camera = GetComponentInChildren<CameraController>();
        if (camera != null)
        {
            camera.enabled = false;
        }
        
    }
    
    private void EnablePlayerControls()
    {
        // Enable movement
        PlayerMovement movement = GetComponent<PlayerMovement>();
        if (movement != null)
        {
            movement.enabled = true;
        }
        
        // Enable shooting
        PlayerShooting shooting = GetComponent<PlayerShooting>();
        if (shooting != null)
        {
            shooting.enabled = true;
        }
        
        // Enable camera
        CameraController camera = GetComponentInChildren<CameraController>();
        if (camera != null)
        {
            camera.enabled = true;
        }

        // Reset cursor state (mouse look won't work without this!)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    // Public method to force respawn (for testing or special cases)
    public void ForceRespawn()
    {
        OnPlayerDeath();
    }
    
    // Method to change spawn point name if needed
    public void SetSpawnPointName(string newSpawnPointName)
    {
        spawnPointName = newSpawnPointName;
    }
    
    // Method to change player room scene name if needed
    public void SetPlayerRoomScene(string newSceneName)
    {
        playerRoomSceneName = newSceneName;
    }
    
    void OnDestroy()
    {
        // Unsubscribe from events to prevent memory leaks
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}