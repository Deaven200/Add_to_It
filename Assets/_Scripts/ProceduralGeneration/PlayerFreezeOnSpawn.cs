using UnityEngine;
using System.Collections;

/// <summary>
/// Freezes the player for a few seconds when they spawn in the Main_level scene.
/// Add this to a player spawn point GameObject.
/// </summary>
public class PlayerFreezeOnSpawn : MonoBehaviour
{
    [Header("Freeze Settings")]
    [SerializeField] private float freezeDuration = 3f;
    [SerializeField] private string targetScene = "Main_level";
    [SerializeField] private bool showFreezeMessage = true;
    
    [Header("Debug")]
    [SerializeField] private bool debugMode = false;
    
    private GameObject player;
    private PlayerMovement playerMovement;
    private CameraController cameraController;
    private Rigidbody playerRigidbody;
    
    void Start()
    {
        // Check if we're in the target scene
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == targetScene)
        {
            StartCoroutine(FreezePlayerOnSpawn());
        }
    }
    
    System.Collections.IEnumerator FreezePlayerOnSpawn()
    {
        if (debugMode)
        {
            Debug.Log($"PlayerFreezeOnSpawn: Starting freeze sequence for {freezeDuration} seconds");
        }
        
        // Wait a frame to ensure player is spawned
        yield return null;
        
        // Find the player
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            if (debugMode)
            {
                Debug.LogWarning("PlayerFreezeOnSpawn: No player found with 'Player' tag!");
            }
            yield break;
        }
        
        // Get player components
        playerMovement = player.GetComponent<PlayerMovement>();
        cameraController = player.GetComponent<CameraController>();
        playerRigidbody = player.GetComponent<Rigidbody>();
        
        // Freeze the player
        FreezePlayer();
        
        if (showFreezeMessage)
        {
            Debug.Log($"Player frozen for {freezeDuration} seconds while terrain generates...");
        }
        
        // Wait for the freeze duration
        yield return new WaitForSeconds(freezeDuration);
        
        // Unfreeze the player
        UnfreezePlayer();
        
        if (showFreezeMessage)
        {
            Debug.Log("Player unfrozen! You can now move around.");
        }
    }
    
    void FreezePlayer()
    {
        // Disable player movement
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }
        
        // Disable camera controller
        if (cameraController != null)
        {
            cameraController.enabled = false;
        }
        
        // Freeze rigidbody
        if (playerRigidbody != null)
        {
            playerRigidbody.isKinematic = true;
        }
        
        if (debugMode)
        {
            Debug.Log("PlayerFreezeOnSpawn: Player frozen");
        }
    }
    
    void UnfreezePlayer()
    {
        // Enable player movement
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }
        
        // Enable camera controller
        if (cameraController != null)
        {
            cameraController.enabled = true;
        }
        
        // Unfreeze rigidbody
        if (playerRigidbody != null)
        {
            playerRigidbody.isKinematic = false;
        }
        
        if (debugMode)
        {
            Debug.Log("PlayerFreezeOnSpawn: Player unfrozen");
        }
    }
    
    // Public method to manually trigger freeze (for testing)
    [ContextMenu("Test Freeze Player")]
    public void TestFreezePlayer()
    {
        StartCoroutine(FreezePlayerOnSpawn());
    }
    
    // Public method to change freeze duration
    public void SetFreezeDuration(float duration)
    {
        freezeDuration = duration;
    }
    
    // Public method to get current freeze duration
    public float GetFreezeDuration()
    {
        return freezeDuration;
    }
}
