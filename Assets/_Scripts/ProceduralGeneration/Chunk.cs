using UnityEngine;

public class Chunk : MonoBehaviour
{
    private Vector2Int chunkPosition;
    private int size;
    private bool isInitialized = false;
    
    public void Initialize(Vector2Int position, int chunkSize)
    {
        chunkPosition = position;
        size = chunkSize;
        isInitialized = true;
        
        GenerateChunkGeometry();
    }
    
    void GenerateChunkGeometry()
    {
        // For now, just create a simple cube as the chunk
        // You can expand this to generate more complex terrain later
        
        // Create a cube mesh for the chunk
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.SetParent(transform);
        cube.transform.localPosition = Vector3.zero;
        cube.transform.localScale = new Vector3(size, 1, size);
        
        // Set material (you can customize this)
        Renderer renderer = cube.GetComponent<Renderer>();
        if (renderer != null)
        {
            // Create a simple material with a grid pattern
            Material material = new Material(Shader.Find("Standard"));
            material.color = new Color(0.5f, 0.5f, 0.5f, 1f);
            renderer.material = material;
        }
        
        // Remove the collider from the cube since we'll handle collision differently
        DestroyImmediate(cube.GetComponent<Collider>());
        
        // Add a ground collider for the entire chunk
        BoxCollider groundCollider = gameObject.AddComponent<BoxCollider>();
        groundCollider.size = new Vector3(size, 0.1f, size);
        groundCollider.center = new Vector3(0, -0.05f, 0);
        groundCollider.isTrigger = false;
    }
    
    void OnDrawGizmos()
    {
        if (isInitialized)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(transform.position + Vector3.up * 0.5f, new Vector3(size, 1, size));
        }
    }
}
