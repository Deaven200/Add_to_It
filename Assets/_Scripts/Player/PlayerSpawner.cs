using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Handles player spawning across different scenes and scenarios
/// Supports spawn points, fallback spawning, and proper initialization
/// </summary>
public class PlayerSpawner : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private bool spawnOnStart = true;
    [SerializeField] private bool destroyExistingPlayer = true;
    
    [Header("Spawn Point Settings")]
    [SerializeField] private Transform defaultSpawnPoint;
    [SerializeField] private string spawnPointTag = "PlayerSpawnPoint";
    [SerializeField] private Vector3 fallbackSpawnPosition = Vector3.zero;
    [SerializeField] private bool useFallbackIfNoSpawnPoint = true;
    
    [Header("Spawn Behavior")]
    [SerializeField] private float spawnDelay = 0.1f;
    [SerializeField] private bool resetPlayerStatsOnSpawn = false;
    [SerializeField] private bool enableInvulnerabilityOnSpawn = true;
    [SerializeField] private float invulnerabilityDuration = 2f;
    
    [Header("Scene Management")]
    [SerializeField] private bool persistBetweenScenes = true;
    [SerializeField] private bool autoSpawnOnSceneLoad = true;
    
    // Events
    public System.Action<GameObject> OnPlayerSpawned;
    public System.Action<GameObject> OnPlayerDespawned;
    
    // Singleton instance
    public static PlayerSpawner Instance { get; private set; }
    
    // Current player reference
    private GameObject currentPlayer;
    private bool isSpawning = false;
    
    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            if (persistBetweenScenes)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    void Start()
    {
        if (spawnOnStart)
        {
            SpawnPlayer();
        }
        
        // Subscribe to scene load events
        if (autoSpawnOnSceneLoad)
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }
    
    void OnDestroy()
    {
        // Unsubscribe from scene load events
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    /// <summary>
    /// Spawns the player at the best available spawn point
    /// </summary>
    public void SpawnPlayer()
    {
        if (isSpawning) return;
        
        StartCoroutine(SpawnPlayerCoroutine());
    }
    
    /// <summary>
    /// Spawns the player at a specific position
    /// </summary>
    public void SpawnPlayerAtPosition(Vector3 position, Quaternion rotation = default)
    {
        if (isSpawning) return;
        
        StartCoroutine(SpawnPlayerAtPositionCoroutine(position, rotation));
    }
    
    /// <summary>
    /// Spawns the player at a specific spawn point
    /// </summary>
    public void SpawnPlayerAtSpawnPoint(Transform spawnPoint)
    {
        if (isSpawning) return;
        
        StartCoroutine(SpawnPlayerAtSpawnPointCoroutine(spawnPoint));
    }
    
    /// <summary>
    /// Despawns the current player
    /// </summary>
    public void DespawnPlayer()
    {
        if (currentPlayer != null)
        {
            OnPlayerDespawned?.Invoke(currentPlayer);
            Destroy(currentPlayer);
            currentPlayer = null;
        }
    }
    
    /// <summary>
    /// Gets the current player GameObject
    /// </summary>
    public GameObject GetCurrentPlayer()
    {
        return currentPlayer;
    }
    
    /// <summary>
    /// Checks if a player is currently spawned
    /// </summary>
    public bool IsPlayerSpawned()
    {
        return currentPlayer != null;
    }
    
    private IEnumerator SpawnPlayerCoroutine()
    {
        isSpawning = true;
        
        // Find the best spawn point
        Transform spawnPoint = FindBestSpawnPoint();
        
        if (spawnPoint != null)
        {
            yield return StartCoroutine(SpawnPlayerAtSpawnPointCoroutine(spawnPoint));
        }
        else if (useFallbackIfNoSpawnPoint)
        {
            yield return StartCoroutine(SpawnPlayerAtPositionCoroutine(fallbackSpawnPosition));
        }
        else
        {
            Debug.LogWarning("PlayerSpawner: No spawn point found and fallback is disabled!");
        }
        
        isSpawning = false;
    }
    
    private IEnumerator SpawnPlayerAtPositionCoroutine(Vector3 position, Quaternion rotation = default)
    {
        isSpawning = true;
        
        // Wait for spawn delay
        yield return new WaitForSeconds(spawnDelay);
        
        // Destroy existing player if needed
        if (destroyExistingPlayer && currentPlayer != null)
        {
            DespawnPlayer();
        }
        
        // Spawn new player
        if (playerPrefab != null)
        {
            currentPlayer = Instantiate(playerPrefab, position, rotation);
            InitializePlayer(currentPlayer);
            OnPlayerSpawned?.Invoke(currentPlayer);
            
            Debug.Log($"PlayerSpawner: Player spawned at position {position}");
        }
        else
        {
            Debug.LogError("PlayerSpawner: Player prefab is null! Please assign a player prefab.");
        }
        
        isSpawning = false;
    }
    
    private IEnumerator SpawnPlayerAtSpawnPointCoroutine(Transform spawnPoint)
    {
        isSpawning = true;
        
        // Wait for spawn delay
        yield return new WaitForSeconds(spawnDelay);
        
        // Destroy existing player if needed
        if (destroyExistingPlayer && currentPlayer != null)
        {
            DespawnPlayer();
        }
        
        // Spawn new player
        if (playerPrefab != null && spawnPoint != null)
        {
            currentPlayer = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
            InitializePlayer(currentPlayer);
            OnPlayerSpawned?.Invoke(currentPlayer);
            
            Debug.Log($"PlayerSpawner: Player spawned at spawn point {spawnPoint.name}");
        }
        else
        {
            Debug.LogError("PlayerSpawner: Player prefab or spawn point is null!");
        }
        
        isSpawning = false;
    }
    
    private Transform FindBestSpawnPoint()
    {
        // First, try to use the assigned default spawn point
        if (defaultSpawnPoint != null)
        {
            return defaultSpawnPoint;
        }
        
        // Then, try to find a spawn point by tag
        GameObject spawnPointObj = GameObject.FindGameObjectWithTag(spawnPointTag);
        if (spawnPointObj != null)
        {
            return spawnPointObj.transform;
        }
        
        // Finally, try to find any object with "SpawnPoint" in the name
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("SpawnPoint") || obj.name.Contains("PlayerSpawn"))
            {
                return obj.transform;
            }
        }
        
        return null;
    }
    
    private void InitializePlayer(GameObject player)
    {
        if (player == null) return;
        
        // Set the player tag if not already set
        if (player.tag != "Player")
        {
            player.tag = "Player";
        }
        
        // Reset player stats if needed
        if (resetPlayerStatsOnSpawn)
        {
            ResetPlayerStats(player);
        }
        
        // Enable invulnerability if needed
        if (enableInvulnerabilityOnSpawn)
        {
            StartCoroutine(EnableInvulnerability(player));
        }
        
        // Initialize player components
        InitializePlayerComponents(player);
    }
    
    private void ResetPlayerStats(GameObject player)
    {
        // Reset health
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.RestoreFullHealth();
        }
        
        // Reset money (optional - you might want to keep money between spawns)
        PlayerMoney playerMoney = player.GetComponent<PlayerMoney>();
        if (playerMoney != null)
        {
            // Uncomment the line below if you want to reset money on spawn
            playerMoney.SetMoney(0);
        }
        
        // Reset weapon ammo
        WeaponManager weaponManager = player.GetComponent<WeaponManager>();
        if (weaponManager != null)
        {
            weaponManager.Reload();
        }
    }
    
    private IEnumerator EnableInvulnerability(GameObject player)
    {
        // Make player invulnerable for a short time
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            // Store original invulnerability state
            bool wasInvulnerable = playerHealth.IsInvulnerable();
            
            // Enable invulnerability
            playerHealth.SetInvulnerable(true);
            
            // Wait for invulnerability duration
            yield return new WaitForSeconds(invulnerabilityDuration);
            
            // Restore original invulnerability state
            playerHealth.SetInvulnerable(wasInvulnerable);
        }
    }
    
    private void InitializePlayerComponents(GameObject player)
    {
        // Initialize camera controller
        CameraController cameraController = player.GetComponent<CameraController>();
        if (cameraController != null)
        {
            cameraController.Initialize();
        }
        
        // Initialize weapon manager
        WeaponManager weaponManager = player.GetComponent<WeaponManager>();
        if (weaponManager != null)
        {
            weaponManager.Initialize();
        }
        
        // Initialize aura system
        AuraSystem auraSystem = player.GetComponent<AuraSystem>();
        if (auraSystem != null)
        {
            // Aura system doesn't need special initialization, but you could add it here
        }
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Auto-spawn player when a new scene is loaded
        if (autoSpawnOnSceneLoad && !IsPlayerSpawned())
        {
            SpawnPlayer();
        }
    }
    
    // Public methods for external control
    [ContextMenu("Spawn Player")]
    public void SpawnPlayerContext()
    {
        SpawnPlayer();
    }
    
    [ContextMenu("Despawn Player")]
    public void DespawnPlayerContext()
    {
        DespawnPlayer();
    }
    
    [ContextMenu("Spawn Player at Fallback Position")]
    public void SpawnPlayerAtFallbackContext()
    {
        SpawnPlayerAtPosition(fallbackSpawnPosition);
    }
    
    // Getters for current settings
    public GameObject GetPlayerPrefab() => playerPrefab;
    public Transform GetDefaultSpawnPoint() => defaultSpawnPoint;
    public Vector3 GetFallbackSpawnPosition() => fallbackSpawnPosition;
    public bool IsSpawning() => isSpawning;
}
