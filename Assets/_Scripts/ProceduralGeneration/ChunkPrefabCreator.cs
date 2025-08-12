using UnityEngine;

public class ChunkPrefabCreator : MonoBehaviour
{
    [Header("Chunk Prefab Settings")]
    [SerializeField] private int chunkSize = 16;
    [SerializeField] private Material chunkMaterial;
    
    [ContextMenu("Create Chunk Prefab")]
    void CreateChunkPrefab()
    {
        // Create the chunk GameObject
        GameObject chunk = new GameObject("ChunkPrefab");
        
        // Add Chunk component
        Chunk chunkComponent = chunk.AddComponent<Chunk>();
        
        // Create the visual representation (cube)
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.SetParent(chunk.transform);
        cube.transform.localPosition = Vector3.zero;
        cube.transform.localScale = new Vector3(chunkSize, 1, chunkSize);
        
        // Set material
        Renderer renderer = cube.GetComponent<Renderer>();
        if (renderer != null && chunkMaterial != null)
        {
            renderer.material = chunkMaterial;
        }
        
        // Remove collider from cube (we'll add it to the chunk itself)
        DestroyImmediate(cube.GetComponent<Collider>());
        
        // Add ground collider to chunk
        BoxCollider groundCollider = chunk.AddComponent<BoxCollider>();
        groundCollider.size = new Vector3(chunkSize, 0.1f, chunkSize);
        groundCollider.center = new Vector3(0, -0.05f, 0);
        groundCollider.isTrigger = false;
        
        Debug.Log("Chunk prefab created! You can now drag this into your Assets folder to create a prefab.");
    }
}
