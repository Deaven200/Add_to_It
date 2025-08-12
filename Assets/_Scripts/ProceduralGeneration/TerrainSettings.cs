using UnityEngine;

[CreateAssetMenu(fileName = "TerrainSettings", menuName = "Procedural Generation/Terrain Settings")]
public class TerrainSettings : ScriptableObject
{
    [Header("Noise Settings")]
    [SerializeField] private float noiseScale = 50f;
    [SerializeField] private int octaves = 4;
    [SerializeField] private float persistence = 0.5f;
    [SerializeField] private float lacunarity = 2f;
    [SerializeField] private Vector2 noiseOffset = Vector2.zero;
    [SerializeField] private bool randomizeOnStart = true;
    [SerializeField] private float noiseScaleVariation = 0.2f; // ±20% variation
    
    [Header("Height Settings")]
    [SerializeField] private float maxHeight = 10f;
    [SerializeField] private float minHeight = -2f;
    [SerializeField] private AnimationCurve heightCurve = AnimationCurve.Linear(0, 0, 1, 1);
    
    [Header("Biome Settings")]
    [SerializeField] private BiomeSettings[] biomes = new BiomeSettings[]
    {
        new BiomeSettings("Plains", 0f, 0.3f, new Color(0.2f, 0.8f, 0.2f), 0.1f), // Brighter green
        new BiomeSettings("Hills", 0.3f, 0.6f, new Color(0.4f, 0.7f, 0.3f), 0.3f), // Medium green
        new BiomeSettings("Mountains", 0.6f, 1f, new Color(0.6f, 0.6f, 0.6f), 0.8f) // Light gray
    };
    
    [Header("Feature Settings")]
    [SerializeField] private bool enableTrees = true;
    [SerializeField] private bool enableRocks = true;
    [SerializeField] private bool enableStructures = false;
    [SerializeField] private float treeDensity = 0.1f;
    [SerializeField] private float rockDensity = 0.02f; // Reduced from 0.05f
    [SerializeField] private float rockSpacing = 3f; // Minimum distance between rocks
    [SerializeField] private float rockDensityVariation = 0.5f; // ±50% variation range
    
    // Public properties
    public float NoiseScale => noiseScale;
    public int Octaves => octaves;
    public float Persistence => persistence;
    public float Lacunarity => lacunarity;
    public Vector2 NoiseOffset => noiseOffset;
    public float MaxHeight => maxHeight;
    public float MinHeight => minHeight;
    public AnimationCurve HeightCurve => heightCurve;
    public BiomeSettings[] Biomes => biomes;
    public bool EnableTrees => enableTrees;
    public bool EnableRocks => enableRocks;
    public bool EnableStructures => enableStructures;
    public float TreeDensity => treeDensity;
    public float RockDensity => rockDensity;
    public float RockSpacing => rockSpacing;
    public float RockDensityVariation => rockDensityVariation;
    
    // Method to randomize the noise offset for different terrain each game
    public void RandomizeNoiseOffset()
    {
        if (!randomizeOnStart) return;
        
        // Generate random offset values between -10000 and 10000
        float randomX = Random.Range(-10000f, 10000f);
        float randomY = Random.Range(-10000f, 10000f);
        noiseOffset = new Vector2(randomX, randomY);
        
        // Add some variation to noise scale for more variety
        float scaleVariation = Random.Range(1f - noiseScaleVariation, 1f + noiseScaleVariation);
        noiseScale *= scaleVariation;
        
        // Debug.Log($"Randomized terrain seed: {noiseOffset}, Scale: {noiseScale:F1}");
    }
    
    // Method to set a specific seed for reproducible terrain
    public void SetSeed(int seed)
    {
        Random.InitState(seed);
        RandomizeNoiseOffset();
        Random.InitState((int)System.DateTime.Now.Ticks); // Reset to random state
    }
    
    // Context menu option to manually randomize terrain
    [ContextMenu("Randomize Terrain")]
    public void ManualRandomize()
    {
        RandomizeNoiseOffset();
    }
    
    public BiomeSettings GetBiomeForHeight(float normalizedHeight)
    {
        foreach (var biome in biomes)
        {
            if (normalizedHeight >= biome.MinHeight && normalizedHeight <= biome.MaxHeight)
            {
                return biome;
            }
        }
        
        // Return default biome if none found
        return biomes.Length > 0 ? biomes[0] : new BiomeSettings("Default", 0f, 1f, Color.gray, 0.5f);
    }
    
    // Method to log biome colors for debugging
    [ContextMenu("Log Biome Colors")]
    public void LogBiomeColors()
    {
        Debug.Log("=== Biome Colors ===");
        for (int i = 0; i < biomes.Length; i++)
        {
            var biome = biomes[i];
            Debug.Log($"Biome {i}: {biome.BiomeName} - Color: {biome.GroundColor}, Height Range: {biome.MinHeight:F2}-{biome.MaxHeight:F2}");
        }
    }
}
