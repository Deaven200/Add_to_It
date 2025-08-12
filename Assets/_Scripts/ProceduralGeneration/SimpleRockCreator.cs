using UnityEngine;

public class SimpleRockCreator : MonoBehaviour
{
    [Header("Rock Settings")]
    [SerializeField] private int numberOfRocks = 5;
    [SerializeField] private float minScale = 0.5f;
    [SerializeField] private float maxScale = 2f;
    [SerializeField] private Color rockColor = new Color(0.4f, 0.4f, 0.4f);
    
    [Header("Generation")]
    [SerializeField] private bool generateOnStart = false;
    [SerializeField] private Transform parentTransform;
    
    void Start()
    {
        if (generateOnStart)
        {
            GenerateRocks();
        }
    }
    
    [ContextMenu("Generate Rocks")]
    public void GenerateRocks()
    {
        if (parentTransform == null)
        {
            parentTransform = transform;
        }
        
        for (int i = 0; i < numberOfRocks; i++)
        {
            CreateRock($"SimpleRock_{i}");
        }
        
        Debug.Log($"Generated {numberOfRocks} simple rocks");
    }
    
    public GameObject CreateRock(string rockName)
    {
        GameObject rock = new GameObject(rockName);
        rock.transform.SetParent(parentTransform);
        
        // Create rock mesh (using a cube as base)
        GameObject rockMesh = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rockMesh.name = "RockMesh";
        rockMesh.transform.SetParent(rock.transform);
        rockMesh.transform.localPosition = Vector3.zero;
        
        // Random scale
        float scale = Random.Range(minScale, maxScale);
        rockMesh.transform.localScale = Vector3.one * scale;
        
        // Random rotation
        rockMesh.transform.localRotation = Quaternion.Euler(
            Random.Range(0, 360),
            Random.Range(0, 360),
            Random.Range(0, 360)
        );
        
        // Set rock material
        Renderer rockRenderer = rockMesh.GetComponent<Renderer>();
        Material rockMaterial = new Material(Shader.Find("Standard"));
        rockMaterial.color = rockColor;
        rockRenderer.material = rockMaterial;
        
        // Remove mesh collider (we'll handle collision at rock level)
        DestroyImmediate(rockMesh.GetComponent<Collider>());
        
        // Add collider to the whole rock
        BoxCollider rockCollider = rock.AddComponent<BoxCollider>();
        rockCollider.size = Vector3.one * scale;
        rockCollider.center = Vector3.zero;
        
        return rock;
    }
    
    [ContextMenu("Create Rock Prefab")]
    public void CreateRockPrefab()
    {
        GameObject rock = CreateRock("RockPrefab");
        
        // Save as prefab (this would need to be done manually in the editor)
        Debug.Log("Rock created! Drag it to your Prefabs folder to save as prefab.");
    }
}
