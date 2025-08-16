using UnityEngine;
using System.Collections.Generic;

public class TerrainGenerator : MonoBehaviour
{
    private TerrainSettings settings;
    private int chunkSize;
    private System.Random random;
    
    // Noise generation
    private float[,] noiseMap;
    private BiomeSettings[,] biomeMap;
    
    // Feature prefabs (you can assign these in the inspector)
    [Header("Feature Prefabs")]
    [SerializeField] private GameObject[] treePrefabs;
    [SerializeField] private GameObject[] rockPrefabs;
    [SerializeField] private GameObject[] structurePrefabs;
    
    public void Initialize(TerrainSettings terrainSettings, int size)
    {
        settings = terrainSettings;
        chunkSize = size;
        random = new System.Random();
        
        // Initialize noise map
        noiseMap = new float[chunkSize + 1, chunkSize + 1];
        biomeMap = new BiomeSettings[chunkSize + 1, chunkSize + 1];
        
        // Randomize the noise offset for different terrain each game
        if (settings != null)
        {
            settings.RandomizeNoiseOffset();
        }
        
        // Add some randomization to feature generation
        RandomizeFeatureGeneration();
    }
    
    public void GenerateChunkTerrain(Chunk chunk, Vector2Int chunkPosition)
    {
        try
        {
            if (settings == null)
            {
                // Debug.LogWarning($"No terrain settings for chunk at {chunkPosition}. Using default settings.");
                settings = ScriptableObject.CreateInstance<TerrainSettings>();
            }
            
            // Generate noise for this chunk
            GenerateNoiseMap(chunkPosition);
            
            // Generate terrain mesh
            GenerateTerrainMesh(chunk);
            
            // Generate features (trees, rocks, etc.)
            if (settings.EnableTrees || settings.EnableRocks || settings.EnableStructures)
            {
                GenerateFeatures(chunk, chunkPosition);
            }
            
            // Debug.Log($"Generated terrain for chunk at {chunkPosition}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error generating terrain for chunk at {chunkPosition}: {e.Message}");
            // The chunk will fall back to simple geometry
        }
    }
    
    void GenerateNoiseMap(Vector2Int chunkPosition)
    {
        Vector2 offset = settings.NoiseOffset;
        
        for (int x = 0; x <= chunkSize; x++)
        {
            for (int z = 0; z <= chunkSize; z++)
            {
                float worldX = (chunkPosition.x * chunkSize + x) / settings.NoiseScale + offset.x;
                float worldZ = (chunkPosition.y * chunkSize + z) / settings.NoiseScale + offset.y;
                
                float noiseValue = GeneratePerlinNoise(worldX, worldZ);
                noiseMap[x, z] = noiseValue;
                
                // Determine biome based on height
                float normalizedHeight = Mathf.Clamp01(noiseValue);
                biomeMap[x, z] = settings.GetBiomeForHeight(normalizedHeight);
            }
        }
    }
    
    float GeneratePerlinNoise(float x, float z)
    {
        float amplitude = 1f;
        float frequency = 1f;
        float noiseHeight = 0f;
        float maxValue = 0f;
        
        for (int i = 0; i < settings.Octaves; i++)
        {
            float sampleX = x * frequency;
            float sampleZ = z * frequency;
            
            float perlinValue = Mathf.PerlinNoise(sampleX, sampleZ) * 2f - 1f;
            noiseHeight += perlinValue * amplitude;
            
            maxValue += amplitude;
            amplitude *= settings.Persistence;
            frequency *= settings.Lacunarity;
        }
        
        if (maxValue > 0)
        {
            noiseHeight /= maxValue;
        }
        
        // Apply height curve
        noiseHeight = settings.HeightCurve.Evaluate(Mathf.Clamp01(noiseHeight));
        
        // Map to height range
        return Mathf.Lerp(settings.MinHeight, settings.MaxHeight, noiseHeight);
    }
    
    void GenerateTerrainMesh(Chunk chunk)
    {
        // Create mesh data
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        List<Color> colors = new List<Color>();
        
        // Generate vertices and colors
        for (int z = 0; z <= chunkSize; z++)
        {
            for (int x = 0; x <= chunkSize; x++)
            {
                float height = noiseMap[x, z];
                Vector3 vertex = new Vector3(x, height, z);
                vertices.Add(vertex);
                
                // UV coordinates
                uvs.Add(new Vector2(x / (float)chunkSize, z / (float)chunkSize));
                
                // Color based on biome
                BiomeSettings biome = biomeMap[x, z];
                Color vertexColor = biome.GroundColor;
                
                // Add some variation based on height and noise
                float heightVariation = Mathf.PerlinNoise(x * 0.1f, z * 0.1f) * 0.3f;
                float noiseVariation = Mathf.PerlinNoise(x * 0.05f, z * 0.05f) * 0.2f;
                
                // Blend with white for variation
                vertexColor = Color.Lerp(vertexColor, Color.white, heightVariation + noiseVariation);
                
                // Ensure the color is not too dark
                vertexColor = Color.Lerp(vertexColor, Color.white, 0.1f);
                
                colors.Add(vertexColor);
            }
        }
        
        // Generate triangles
        for (int z = 0; z < chunkSize; z++)
        {
            for (int x = 0; x < chunkSize; x++)
            {
                int topLeft = z * (chunkSize + 1) + x;
                int topRight = topLeft + 1;
                int bottomLeft = (z + 1) * (chunkSize + 1) + x;
                int bottomRight = bottomLeft + 1;
                
                // First triangle
                triangles.Add(topLeft);
                triangles.Add(bottomLeft);
                triangles.Add(topRight);
                
                // Second triangle
                triangles.Add(topRight);
                triangles.Add(bottomLeft);
                triangles.Add(bottomRight);
            }
        }
        
        // Create mesh
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.colors = colors.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        
        // Apply mesh to chunk
        chunk.ApplyTerrainMesh(mesh);
        
        // Ensure proper collision setup for NavMesh
        SetupCollisionForNavMesh(chunk);
    }
    
    void GenerateFeatures(Chunk chunk, Vector2Int chunkPosition)
    {
        // Generate trees
        if (settings.EnableTrees && treePrefabs != null && treePrefabs.Length > 0)
        {
            GenerateTrees(chunk, chunkPosition);
        }
        
        // Generate rocks
        if (settings.EnableRocks && rockPrefabs != null && rockPrefabs.Length > 0)
        {
            GenerateRocks(chunk, chunkPosition);
        }
        
        // Generate structures
        if (settings.EnableStructures && structurePrefabs != null && structurePrefabs.Length > 0)
        {
            GenerateStructures(chunk, chunkPosition);
        }
    }
    
    void GenerateTrees(Chunk chunk, Vector2Int chunkPosition)
    {
        for (int x = 0; x < chunkSize; x++)
        {
            for (int z = 0; z < chunkSize; z++)
            {
                // Check if we should place a tree here
                float randomValue = (float)random.NextDouble();
                if (randomValue < settings.TreeDensity * treeDensityVariation)
                {
                    // Check if this is a suitable location (not too steep)
                    float height = noiseMap[x, z];
                    BiomeSettings biome = biomeMap[x, z];
                    
                    // Only place trees in suitable biomes and not on steep slopes
                    if (biome.BiomeName.Contains("Plains") || biome.BiomeName.Contains("Hills"))
                    {
                        Vector3 position = new Vector3(x, height, z);
                        GameObject treePrefab = treePrefabs[random.Next(treePrefabs.Length)];
                        
                        chunk.AddFeature(treePrefab, position, Quaternion.Euler(0, random.Next(360), 0));
                    }
                }
            }
        }
    }
    
    void GenerateRocks(Chunk chunk, Vector2Int chunkPosition)
    {
        // Use spacing to prevent rocks from being too close together
        float spacing = settings.RockSpacing;
        int spacingInt = Mathf.Max(1, Mathf.RoundToInt(spacing));
        
        for (int x = spacingInt; x < chunkSize - spacingInt; x += spacingInt)
        {
            for (int z = spacingInt; z < chunkSize - spacingInt; z += spacingInt)
            {
                // Add some randomness to the spacing
                float offsetX = Random.Range(-spacing * 0.5f, spacing * 0.5f);
                float offsetZ = Random.Range(-spacing * 0.5f, spacing * 0.5f);
                
                int finalX = Mathf.Clamp(x + Mathf.RoundToInt(offsetX), 0, chunkSize - 1);
                int finalZ = Mathf.Clamp(z + Mathf.RoundToInt(offsetZ), 0, chunkSize - 1);
                
                float randomValue = (float)random.NextDouble();
                if (randomValue < settings.RockDensity * rockDensityVariation)
                {
                    float height = noiseMap[finalX, finalZ];
                    BiomeSettings biome = biomeMap[finalX, finalZ];
                    
                    // Rocks can appear in most biomes, but prefer certain areas
                    if (biome.BiomeName.Contains("Plains") || biome.BiomeName.Contains("Hills"))
                    {
                        Vector3 position = new Vector3(finalX, height, finalZ);
                        GameObject rockPrefab = rockPrefabs[random.Next(rockPrefabs.Length)];
                        
                        chunk.AddFeature(rockPrefab, position, Quaternion.Euler(0, random.Next(360), 0));
                    }
                }
            }
        }
    }
    
    void GenerateStructures(Chunk chunk, Vector2Int chunkPosition)
    {
        // Generate structures less frequently
        float randomValue = (float)random.NextDouble();
        if (randomValue < 0.01f) // 1% chance per chunk
        {
            int x = random.Next(chunkSize);
            int z = random.Next(chunkSize);
            float height = noiseMap[x, z];
            
            Vector3 position = new Vector3(x, height, z);
            GameObject structurePrefab = structurePrefabs[random.Next(structurePrefabs.Length)];
            
            chunk.AddFeature(structurePrefab, position, Quaternion.Euler(0, random.Next(360), 0));
        }
    }
    
    // Public method to get height at world position
    public float GetHeightAt(Vector3 worldPosition, Vector2Int chunkPosition)
    {
        Vector2 localPos = new Vector2(worldPosition.x - chunkPosition.x * chunkSize, 
                                     worldPosition.z - chunkPosition.y * chunkSize);
        
        int x = Mathf.Clamp(Mathf.FloorToInt(localPos.x), 0, chunkSize);
        int z = Mathf.Clamp(Mathf.FloorToInt(localPos.y), 0, chunkSize);
        
        return noiseMap[x, z];
    }
    
    // Public method to get biome at world position
    public BiomeSettings GetBiomeAt(Vector3 worldPosition, Vector2Int chunkPosition)
    {
        Vector2 localPos = new Vector2(worldPosition.x - chunkPosition.x * chunkSize, 
                                     worldPosition.z - chunkPosition.y * chunkSize);
        
        int x = Mathf.Clamp(Mathf.FloorToInt(localPos.x), 0, chunkSize);
        int z = Mathf.Clamp(Mathf.FloorToInt(localPos.y), 0, chunkSize);
        
        return biomeMap[x, z];
    }
    
    // Method to randomize feature generation parameters
    private float treeDensityVariation = 1f;
    private float rockDensityVariation = 1f;
    
    private void RandomizeFeatureGeneration()
    {
        if (settings == null) return;
        
        // Add some variation to tree and rock density
        treeDensityVariation = Random.Range(0.7f, 1.3f);
        
        // Use the rock density variation setting from TerrainSettings
        float variationRange = settings.RockDensityVariation;
        rockDensityVariation = Random.Range(1f - variationRange, 1f + variationRange);
        
        // Debug.Log($"Feature generation randomized - Tree variation: {treeDensityVariation:F2}, Rock variation: {rockDensityVariation:F2}");
    }
    
    /// <summary>
    /// Ensure proper collision setup for NavMesh generation
    /// </summary>
    void SetupCollisionForNavMesh(Chunk chunk)
    {
        try
        {
            // Ensure the chunk has a MeshCollider
            MeshCollider meshCollider = chunk.GetComponent<MeshCollider>();
            if (meshCollider == null)
            {
                meshCollider = chunk.gameObject.AddComponent<MeshCollider>();
            }
            
            // Make sure the collider is properly configured
            meshCollider.sharedMesh = chunk.GetComponent<MeshFilter>()?.sharedMesh;
            meshCollider.convex = false; // Keep false for terrain
            
            // Ensure the chunk is on a layer that NavMesh can see
            if (chunk.gameObject.layer == LayerMask.NameToLayer("Ignore Raycast"))
            {
                chunk.gameObject.layer = LayerMask.NameToLayer("Default");
            }
            
            // Add a small delay to ensure the collider is properly set up
            // This helps with NavMesh generation timing
            chunk.StartCoroutine(DelayedColliderSetup(chunk));
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error setting up collision for NavMesh: {e.Message}");
        }
    }
    
    System.Collections.IEnumerator DelayedColliderSetup(Chunk chunk)
    {
        yield return new WaitForEndOfFrame();
        
        // Force collider update
        MeshCollider meshCollider = chunk.GetComponent<MeshCollider>();
        if (meshCollider != null && meshCollider.sharedMesh != null)
        {
            meshCollider.enabled = false;
            meshCollider.enabled = true;
        }
    }
}
