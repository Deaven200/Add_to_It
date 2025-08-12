using UnityEngine;

public class TerrainRandomizationTest : MonoBehaviour
{
    [Header("Test Settings")]
    [SerializeField] private bool testOnStart = false;
    [SerializeField] private int testSeed = 12345;
    
    private ProceduralLevelManager levelManager;
    private TerrainSettings terrainSettings;
    
    void Start()
    {
        if (testOnStart)
        {
            TestRandomization();
        }
    }
    
    void TestRandomization()
    {
        Debug.Log("=== Testing Terrain Randomization ===");
        
        // Find the level manager
        levelManager = FindObjectOfType<ProceduralLevelManager>();
        if (levelManager == null)
        {
            Debug.LogError("No ProceduralLevelManager found!");
            return;
        }
        
        // Find terrain settings
        TerrainGenerator terrainGen = levelManager.GetComponent<TerrainGenerator>();
        if (terrainGen == null)
        {
            Debug.LogError("No TerrainGenerator found!");
            return;
        }
        
        // Test random seed
        Debug.Log("Testing random seed generation...");
        TestRandomSeed();
        
        // Test specific seed
        Debug.Log("Testing specific seed...");
        TestSpecificSeed();
        
        Debug.Log("=== Randomization Test Complete ===");
    }
    
    void TestRandomSeed()
    {
        // Create a test terrain settings
        TerrainSettings testSettings = ScriptableObject.CreateInstance<TerrainSettings>();
        
        // Test multiple randomizations
        for (int i = 0; i < 3; i++)
        {
            testSettings.RandomizeNoiseOffset();
            Debug.Log($"Random test {i + 1}: Offset = {testSettings.NoiseOffset}, Scale = {testSettings.NoiseScale:F1}");
        }
        
        DestroyImmediate(testSettings);
    }
    
    void TestSpecificSeed()
    {
        // Create a test terrain settings
        TerrainSettings testSettings = ScriptableObject.CreateInstance<TerrainSettings>();
        
        // Test specific seed
        testSettings.SetSeed(testSeed);
        Debug.Log($"Specific seed {testSeed}: Offset = {testSettings.NoiseOffset}, Scale = {testSettings.NoiseScale:F1}");
        
        DestroyImmediate(testSettings);
    }
    
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.BeginVertical("box");
        
        GUILayout.Label("Terrain Randomization Test", GUI.skin.box);
        
        if (GUILayout.Button("Test Randomization"))
        {
            TestRandomization();
        }
        
        if (GUILayout.Button("Test Random Seed"))
        {
            TestRandomSeed();
        }
        
        if (GUILayout.Button("Test Specific Seed"))
        {
            TestSpecificSeed();
        }
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
}
