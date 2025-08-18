using UnityEngine;

/// <summary>
/// Helper script to ensure the GameManager is properly set up for player spawning.
/// This should be placed in your main menu scene or any scene that needs player spawning.
/// </summary>
public class PlayerSpawnSetup : MonoBehaviour
{
    [Header("Player Spawning")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private bool autoSetupOnStart = true;
    
    void Start()
    {
        if (autoSetupOnStart)
        {
            SetupPlayerSpawning();
        }
    }
    
    /// <summary>
    /// Sets up the GameManager for player spawning
    /// </summary>
    public void SetupPlayerSpawning()
    {
        // Check if GameManager already exists
        GameManager existingManager = GameManager.Instance;
        
        if (existingManager == null)
        {
            // Create GameManager if it doesn't exist
            GameObject gameManagerGO = new GameObject("GameManager");
            GameManager gameManager = gameManagerGO.AddComponent<GameManager>();
            
            // Set the player prefab
            if (playerPrefab != null)
            {
                gameManager.playerPrefab = playerPrefab;
            }
            
            Debug.Log("PlayerSpawnSetup: Created GameManager for player spawning");
        }
        else
        {
            Debug.Log("PlayerSpawnSetup: GameManager already exists");
        }
    }
    
    /// <summary>
    /// Manually spawn a player (useful for testing)
    /// </summary>
    [ContextMenu("Spawn Player")]
    public void SpawnPlayer()
    {
        if (playerPrefab != null)
        {
            GameObject player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
            Debug.Log("PlayerSpawnSetup: Manually spawned player");
        }
        else
        {
            Debug.LogWarning("PlayerSpawnSetup: No player prefab assigned!");
        }
    }
}
