using UnityEngine;

/// <summary>
/// Automatically sets up the Main_level scene when it loads.
/// This ensures the teleportation system works without manual scene editing.
/// </summary>
public class AutoSetupManager : MonoBehaviour
{
    [Header("Auto Setup")]
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
        
        // Find ProceduralLevelManager
        ProceduralLevelManager levelManager = FindObjectOfType<ProceduralLevelManager>();
        if (levelManager == null)
        {
            Debug.LogError("AutoSetupManager: No ProceduralLevelManager found in Main_level scene!");
            return;
        }
        
        // Check if MainLevelSetup already exists
        MainLevelSetup existingSetup = levelManager.GetComponent<MainLevelSetup>();
        if (existingSetup != null)
        {
            Debug.Log("MainLevelSetup already exists on ProceduralLevelManager");
            return;
        }
        
        // Add MainLevelSetup component
        MainLevelSetup setup = levelManager.gameObject.AddComponent<MainLevelSetup>();
        
        // Set the player prefab if provided
        if (playerPrefab != null)
        {
            // Use reflection to set the private field
            var playerPrefabField = typeof(MainLevelSetup).GetField("playerPrefab", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (playerPrefabField != null)
            {
                playerPrefabField.SetValue(setup, playerPrefab);
            }
        }
        
        Debug.Log("AutoSetupManager: Added MainLevelSetup to ProceduralLevelManager");
    }
    
    // Context menu option for manual setup
    [ContextMenu("Setup Main Level")]
    public void ManualSetup()
    {
        SetupMainLevel();
    }
}
