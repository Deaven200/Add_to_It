using UnityEngine;

public class TerrainColorTest : MonoBehaviour
{
    [Header("Test Settings")]
    [SerializeField] private bool testOnStart = false;
    [SerializeField] private bool forceColorUpdate = false;
    
    private ProceduralLevelManager levelManager;
    private TerrainSettings terrainSettings;
    
    void Start()
    {
        if (testOnStart)
        {
            TestTerrainColors();
        }
    }
    
    void TestTerrainColors()
    {
        // Debug.Log("=== Testing Terrain Colors ===");
        
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
        
        // Test biome colors
        TestBiomeColors();
        
        // Test material setup
        TestMaterialSetup();
        
        // Debug.Log("=== Terrain Color Test Complete ===");
    }
    
    void TestBiomeColors()
    {
        // Create a test terrain settings
        TerrainSettings testSettings = ScriptableObject.CreateInstance<TerrainSettings>();
        
        // Test biome color assignment
        // Debug.Log("Testing biome colors...");
        for (float height = 0f; height <= 1f; height += 0.2f)
        {
            BiomeSettings biome = testSettings.GetBiomeForHeight(height);
            // Debug.Log($"Height {height:F1}: {biome.BiomeName} - Color: {biome.GroundColor}");
        }
        
        // Log all biome colors
        testSettings.LogBiomeColors();
        
        DestroyImmediate(testSettings);
    }
    
    void TestMaterialSetup()
    {
        // Debug.Log("Testing material setup...");
        
        // Test shader availability
        Shader customShader = Shader.Find("Custom/TerrainVertexColor");
        if (customShader != null)
        {
            // Debug.Log("Custom terrain shader found!");
        }
        else
        {
            // Debug.LogWarning("Custom terrain shader not found, will use fallback");
        }
        
        // Test standard shader
        Shader standardShader = Shader.Find("Standard");
        if (standardShader != null)
        {
            // Debug.Log("Standard shader found!");
        }
        else
        {
            // Debug.LogWarning("Standard shader not found!");
        }
        
        // Test diffuse shader
        Shader diffuseShader = Shader.Find("Diffuse");
        if (diffuseShader != null)
        {
            // Debug.Log("Diffuse shader found!");
        }
        else
        {
            // Debug.LogWarning("Diffuse shader not found!");
        }
    }
    
    void Update()
    {
        if (forceColorUpdate && Input.GetKeyDown(KeyCode.C))
        {
            ForceColorUpdate();
        }
    }
    
    void ForceColorUpdate()
    {
        // Debug.Log("Forcing terrain color update...");
        
        // Find all chunks and update their materials
        Chunk[] chunks = FindObjectsOfType<Chunk>();
        foreach (var chunk in chunks)
        {
            MeshRenderer renderer = chunk.GetComponent<MeshRenderer>();
            if (renderer != null && renderer.material != null)
            {
                // Force material update
                renderer.material.color = Color.white;
                // Debug.Log($"Updated material for chunk: {chunk.name}");
            }
        }
        
        // Debug.Log($"Updated {chunks.Length} chunks");
    }
    
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 150));
        GUILayout.BeginVertical("box");
        
        GUILayout.Label("Terrain Color Test", GUI.skin.box);
        
        if (GUILayout.Button("Test Terrain Colors"))
        {
            TestTerrainColors();
        }
        
        if (GUILayout.Button("Force Color Update"))
        {
            ForceColorUpdate();
        }
        
        GUILayout.Label("Press C to force color update");
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
}
