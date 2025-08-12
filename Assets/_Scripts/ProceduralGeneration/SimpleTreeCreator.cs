using UnityEngine;

public class SimpleTreeCreator : MonoBehaviour
{
    [Header("Tree Settings")]
    [SerializeField] private int numberOfTrees = 5;
    [SerializeField] private float trunkHeight = 2f;
    [SerializeField] private float trunkRadius = 0.2f;
    [SerializeField] private float leavesRadius = 1f;
    [SerializeField] private Color trunkColor = new Color(0.4f, 0.2f, 0.1f);
    [SerializeField] private Color leavesColor = new Color(0.1f, 0.5f, 0.1f);
    
    [Header("Generation")]
    [SerializeField] private bool generateOnStart = false;
    [SerializeField] private Transform parentTransform;
    
    void Start()
    {
        if (generateOnStart)
        {
            GenerateTrees();
        }
    }
    
    [ContextMenu("Generate Trees")]
    public void GenerateTrees()
    {
        if (parentTransform == null)
        {
            parentTransform = transform;
        }
        
        for (int i = 0; i < numberOfTrees; i++)
        {
            CreateTree($"SimpleTree_{i}");
        }
        
        Debug.Log($"Generated {numberOfTrees} simple trees");
    }
    
    public GameObject CreateTree(string treeName)
    {
        GameObject tree = new GameObject(treeName);
        tree.transform.SetParent(parentTransform);
        
        // Create trunk
        GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        trunk.name = "Trunk";
        trunk.transform.SetParent(tree.transform);
        trunk.transform.localPosition = Vector3.up * trunkHeight * 0.5f;
        trunk.transform.localScale = new Vector3(trunkRadius * 2, trunkHeight, trunkRadius * 2);
        
        // Set trunk material
        Renderer trunkRenderer = trunk.GetComponent<Renderer>();
        Material trunkMaterial = new Material(Shader.Find("Standard"));
        trunkMaterial.color = trunkColor;
        trunkRenderer.material = trunkMaterial;
        
        // Remove trunk collider (we'll handle collision at tree level)
        DestroyImmediate(trunk.GetComponent<Collider>());
        
        // Create leaves
        GameObject leaves = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        leaves.name = "Leaves";
        leaves.transform.SetParent(tree.transform);
        leaves.transform.localPosition = Vector3.up * trunkHeight;
        leaves.transform.localScale = Vector3.one * leavesRadius * 2;
        
        // Set leaves material
        Renderer leavesRenderer = leaves.GetComponent<Renderer>();
        Material leavesMaterial = new Material(Shader.Find("Standard"));
        leavesMaterial.color = leavesColor;
        leavesRenderer.material = leavesMaterial;
        
        // Remove leaves collider
        DestroyImmediate(leaves.GetComponent<Collider>());
        
        // Add collider to the whole tree
        CapsuleCollider treeCollider = tree.AddComponent<CapsuleCollider>();
        treeCollider.height = trunkHeight + leavesRadius * 2;
        treeCollider.radius = Mathf.Max(trunkRadius, leavesRadius);
        treeCollider.center = Vector3.up * (trunkHeight + leavesRadius);
        
        return tree;
    }
    
    [ContextMenu("Create Tree Prefab")]
    public void CreateTreePrefab()
    {
        GameObject tree = CreateTree("TreePrefab");
        
        // Save as prefab (this would need to be done manually in the editor)
        Debug.Log("Tree created! Drag it to your Prefabs folder to save as prefab.");
    }
}
