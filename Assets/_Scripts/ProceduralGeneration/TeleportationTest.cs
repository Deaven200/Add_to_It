using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Test script to verify teleportation and player spawning functionality.
/// </summary>
public class TeleportationTest : MonoBehaviour
{
    [Header("Test Settings")]
    [SerializeField] private bool testOnStart = false;
    [SerializeField] private string testSceneName = "Main_level";
    
    void Start()
    {
        if (testOnStart)
        {
            TestTeleportation();
        }
    }
    
    void TestTeleportation()
    {
        Debug.Log("=== Testing Teleportation System ===");
        
        // Check current scene
        string currentScene = SceneManager.GetActiveScene().name;
        Debug.Log($"Current scene: {currentScene}");
        
        // Test teleporting to Main_level
        if (currentScene != testSceneName)
        {
            Debug.Log($"Teleporting to {testSceneName}...");
            SceneManager.LoadScene(testSceneName);
        }
        else
        {
            Debug.Log("Already in Main_level scene!");
            TestPlayerSpawning();
        }
    }
    
    void TestPlayerSpawning()
    {
        Debug.Log("=== Testing Player Spawning ===");
        
        // Check if PlayerSpawnManager exists
        PlayerSpawnManager spawnManager = FindObjectOfType<PlayerSpawnManager>();
        if (spawnManager != null)
        {
            Debug.Log("✓ PlayerSpawnManager found");
        }
        else
        {
            Debug.LogError("✗ PlayerSpawnManager not found!");
        }
        
        // Check if ProceduralLevelManager exists
        ProceduralLevelManager levelManager = FindObjectOfType<ProceduralLevelManager>();
        if (levelManager != null)
        {
            Debug.Log("✓ ProceduralLevelManager found");
        }
        else
        {
            Debug.LogError("✗ ProceduralLevelManager not found!");
        }
        
        // Check if player exists
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Debug.Log($"✓ Player found: {player.name}");
        }
        else
        {
            Debug.LogWarning("⚠ No player found yet (may still be spawning)");
        }
    }
    
    void Update()
    {
        // Test keys
        if (Input.GetKeyDown(KeyCode.T))
        {
            TestTeleportation();
        }
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            TestPlayerSpawning();
        }
    }
    
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.BeginVertical("box");
        
        GUILayout.Label("Teleportation Test", GUI.skin.box);
        
        if (GUILayout.Button("Test Teleportation (T)"))
        {
            TestTeleportation();
        }
        
        if (GUILayout.Button("Test Player Spawning (P)"))
        {
            TestPlayerSpawning();
        }
        
        GUILayout.Space(10);
        
        GUILayout.Label($"Current Scene: {SceneManager.GetActiveScene().name}");
        
        PlayerSpawnManager spawnManager = FindObjectOfType<PlayerSpawnManager>();
        GUILayout.Label($"Spawn Manager: {(spawnManager != null ? "Found" : "Missing")}");
        
        ProceduralLevelManager levelManager = FindObjectOfType<ProceduralLevelManager>();
        GUILayout.Label($"Level Manager: {(levelManager != null ? "Found" : "Missing")}");
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GUILayout.Label($"Player: {(player != null ? "Found" : "Missing")}");
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
}
