using UnityEngine;

/// <summary>
/// Automatically sets up the Main_level scene for player spawning.
/// This ensures the player can play the procedural generation when teleporting in.
/// </summary>
public class MainLevelSetup : MonoBehaviour
{
    [Header("Setup Settings")]
    [SerializeField] private bool autoSetup = true;
    [SerializeField] private GameObject playerPrefab;
    
    void Start()
    {
        if (autoSetup)
        {
            SetupMainLevel();
        }
    }
    
    void SetupMainLevel()
    {
        // Check if we're in the Main_level scene
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "Main_level")
        {
            return;
        }
        
        // Find or create PlayerSpawnManager
        PlayerSpawnManager spawnManager = FindObjectOfType<PlayerSpawnManager>();
        if (spawnManager == null)
        {
            GameObject spawnManagerGO = new GameObject("PlayerSpawnManager");
            spawnManager = spawnManagerGO.AddComponent<PlayerSpawnManager>();
            
            // Set the player prefab if provided
            if (playerPrefab != null)
            {
                // Use reflection to set the private field
                var playerPrefabField = typeof(PlayerSpawnManager).GetField("playerPrefab", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (playerPrefabField != null)
                {
                    playerPrefabField.SetValue(spawnManager, playerPrefab);
                }
            }
            
            Debug.Log("PlayerSpawnManager created for Main_level scene");
        }
        
        // Find ProceduralLevelManager and ensure it's set up
        ProceduralLevelManager levelManager = FindObjectOfType<ProceduralLevelManager>();
        if (levelManager != null)
        {
            // Ensure the level manager is set to auto-setup
            var autoSetupField = typeof(ProceduralLevelManager).GetField("autoSetup", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (autoSetupField != null)
            {
                autoSetupField.SetValue(levelManager, true);
            }
            
            Debug.Log("ProceduralLevelManager found and configured");
        }
        else
        {
            Debug.LogError("No ProceduralLevelManager found in Main_level scene!");
        }
    }
    
    // Context menu option for manual setup
    [ContextMenu("Setup Main Level")]
    public void ManualSetup()
    {
        SetupMainLevel();
    }
}
