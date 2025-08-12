using UnityEngine;

public class QuickSetup : MonoBehaviour
{
    void Start()
    {
        // Create player if none exists
        if (GameObject.FindGameObjectWithTag("Player") == null)
        {
            GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "Player";
            player.tag = "Player";
            player.transform.position = new Vector3(0, 2, 0);
            
            // Add rigidbody
            Rigidbody rb = player.AddComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            
            Debug.Log("Created test player");
        }
        
        // Create chunk generator
        GameObject chunkGeneratorGO = new GameObject("ChunkGenerator");
        ChunkGenerator chunkGenerator = chunkGeneratorGO.AddComponent<ChunkGenerator>();
        
        Debug.Log("Created chunk generator");
        
        // Set up camera
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                mainCamera.transform.position = player.transform.position + new Vector3(0, 5, -10);
                mainCamera.transform.LookAt(player.transform);
            }
        }
        
        Debug.Log("Procedural level setup complete!");
    }
}
