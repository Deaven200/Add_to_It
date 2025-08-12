using UnityEngine;
using System.Collections.Generic;

public class Chunk : MonoBehaviour
{
    private Vector2Int chunkPosition;
    private int size;
    private bool isInitialized = false;
    
    // Terrain components
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;
    
    // Features
    private List<GameObject> features = new List<GameObject>();
    
    // Terrain data
    private Mesh terrainMesh;
    private Material terrainMaterial;
    
    public void Initialize(Vector2Int position, int chunkSize, TerrainGenerator terrainGenerator = null)
    {
        try
    {
        chunkPosition = position;
        size = chunkSize;
        isInitialized = true;
        
            // Set up components
            SetupComponents();
            
            // Generate terrain if terrain generator is provided
            if (terrainGenerator != null)
            {
                terrainGenerator.GenerateChunkTerrain(this, chunkPosition);
            }
            else
            {
                // Fallback to simple geometry
                // Debug.LogWarning($"No terrain generator provided for chunk at {position}. Using simple geometry.");
                GenerateSimpleGeometry();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error initializing chunk at {position}: {e.Message}");
            // Fallback to simple geometry on error
            GenerateSimpleGeometry();
        }
    }
    
    void SetupComponents()
    {
        // Get or add MeshFilter
        meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }
        
        // Get or add MeshRenderer
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
        }
        
        // Get or add MeshCollider
        meshCollider = GetComponent<MeshCollider>();
        if (meshCollider == null)
        {
            meshCollider = gameObject.AddComponent<MeshCollider>();
        }
        
        // Create terrain material if it doesn't exist
        if (terrainMaterial == null)
        {
            // Try to use our custom terrain shader first
            Shader vertexColorShader = Shader.Find("Custom/TerrainVertexColor");
            if (vertexColorShader == null)
            {
                // Fallback to Standard shader
                vertexColorShader = Shader.Find("Standard");
                if (vertexColorShader == null)
                {
                    vertexColorShader = Shader.Find("Diffuse");
                }
            }
            
            terrainMaterial = new Material(vertexColorShader);
            terrainMaterial.color = Color.white; // Set to white so vertex colors show properly
        }
        
        meshRenderer.material = terrainMaterial;
    }
    
    void GenerateSimpleGeometry()
    {
        // Create a simple flat mesh as fallback
        Mesh mesh = new Mesh();
        
        // Vertices for a flat square with some thickness
        Vector3[] vertices = new Vector3[]
        {
            // Top face
            new Vector3(0, 0, 0),
            new Vector3(size, 0, 0),
            new Vector3(size, 0, size),
            new Vector3(0, 0, size),
            
            // Bottom face (for collision)
            new Vector3(0, -1, 0),
            new Vector3(size, -1, 0),
            new Vector3(size, -1, size),
            new Vector3(0, -1, size)
        };
        
        // Triangles for top and bottom faces
        int[] triangles = new int[]
        {
            // Top face
            0, 2, 1,
            0, 3, 2,
            
            // Bottom face
            4, 5, 6,
            4, 6, 7,
            
            // Side faces for better collision
            0, 1, 5,
            0, 5, 4,
            1, 2, 6,
            1, 6, 5,
            2, 3, 7,
            2, 7, 6,
            3, 0, 4,
            3, 4, 7
        };
        
        // UVs
        Vector2[] uvs = new Vector2[]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 1),
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 1)
        };
        
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        
        ApplyTerrainMesh(mesh);
        
        // Debug.Log($"Generated simple geometry for chunk at {chunkPosition}");
    }
    
    public void ApplyTerrainMesh(Mesh mesh)
    {
        terrainMesh = mesh;
        
        if (meshFilter != null)
        {
            meshFilter.mesh = mesh;
        }
        
        if (meshCollider != null)
        {
            meshCollider.sharedMesh = mesh;
        }
    }
    
    public void AddFeature(GameObject featurePrefab, Vector3 localPosition, Quaternion rotation)
    {
        if (featurePrefab == null) return;
        
        GameObject feature = Instantiate(featurePrefab, transform);
        feature.transform.localPosition = localPosition;
        feature.transform.localRotation = rotation;
        
        features.Add(feature);
    }
    
    public void ClearFeatures()
    {
        foreach (var feature in features)
        {
            if (feature != null)
            {
                DestroyImmediate(feature);
            }
        }
        features.Clear();
    }
    
    public Vector2Int GetChunkPosition()
    {
        return chunkPosition;
    }
    
    public int GetChunkSize()
    {
        return size;
    }
    
    public bool IsInitialized()
    {
        return isInitialized;
    }
    
    public List<GameObject> GetFeatures()
    {
        return new List<GameObject>(features);
    }
    
    void OnDrawGizmos()
    {
        if (isInitialized)
        {
            // Draw chunk bounds
            Gizmos.color = Color.blue;
            Vector3 center = transform.position + new Vector3(size * 0.5f, 0.5f, size * 0.5f);
            Gizmos.DrawWireCube(center, new Vector3(size, 1, size));
            
            // Draw chunk position
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
    }
    
    void OnDestroy()
    {
        // Clean up features
        ClearFeatures();
        
        // Clean up material
        if (terrainMaterial != null)
        {
            DestroyImmediate(terrainMaterial);
        }
    }
}
