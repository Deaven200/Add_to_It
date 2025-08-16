using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Debug script to visualize NavMesh in the scene view
/// </summary>
public class NavMeshDebugger : MonoBehaviour
{
    [Header("Debug Settings")]
    [SerializeField] private bool showNavMesh = true;
    [SerializeField] private bool showNavMeshBounds = true;
    [SerializeField] private bool showNavMeshTriangles = true;
    [SerializeField] private Color navMeshColor = Color.green;
    [SerializeField] private Color boundsColor = Color.yellow;
    [SerializeField] private Color triangleColor = Color.blue;
    
    [Header("Performance")]
    [SerializeField] private bool updateEveryFrame = false;
    [SerializeField] private float updateInterval = 1f;
    
    private NavMeshTriangulation navMeshData;
    private float lastUpdateTime;
    
    void Start()
    {
        UpdateNavMeshData();
    }
    
    void Update()
    {
        if (updateEveryFrame || Time.time - lastUpdateTime > updateInterval)
        {
            UpdateNavMeshData();
            lastUpdateTime = Time.time;
        }
    }
    
    void UpdateNavMeshData()
    {
        try
        {
            navMeshData = NavMesh.CalculateTriangulation();
        }
        catch (System.Exception e)
        {
            // NavMesh might not be ready yet
            if (Time.time > 5f) // Only log after 5 seconds to avoid spam
            {
                Debug.LogWarning($"NavMesh not ready yet: {e.Message}");
            }
        }
    }
    
    void OnDrawGizmos()
    {
        if (!showNavMesh || navMeshData.vertices == null || navMeshData.vertices.Length == 0)
            return;
        
        // Draw NavMesh triangles
        if (showNavMeshTriangles)
        {
            Gizmos.color = triangleColor;
            for (int i = 0; i < navMeshData.indices.Length; i += 3)
            {
                Vector3 v1 = navMeshData.vertices[navMeshData.indices[i]];
                Vector3 v2 = navMeshData.vertices[navMeshData.indices[i + 1]];
                Vector3 v3 = navMeshData.vertices[navMeshData.indices[i + 2]];
                
                Gizmos.DrawLine(v1, v2);
                Gizmos.DrawLine(v2, v3);
                Gizmos.DrawLine(v3, v1);
            }
        }
        
        // Draw NavMesh bounds
        if (showNavMeshBounds)
        {
            Gizmos.color = boundsColor;
            Bounds bounds = CalculateNavMeshBounds();
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }
    }
    
    Bounds CalculateNavMeshBounds()
    {
        if (navMeshData.vertices == null || navMeshData.vertices.Length == 0)
            return new Bounds(Vector3.zero, Vector3.one);
        
        Vector3 min = Vector3.positiveInfinity;
        Vector3 max = Vector3.negativeInfinity;
        
        foreach (Vector3 vertex in navMeshData.vertices)
        {
            min = Vector3.Min(min, vertex);
            max = Vector3.Max(max, vertex);
        }
        
        return new Bounds((min + max) * 0.5f, max - min);
    }
    
    void OnGUI()
    {
        if (!showNavMesh) return;
        
        // Display NavMesh info
        GUILayout.BeginArea(new Rect(10, Screen.height - 150, 300, 140));
        GUILayout.BeginVertical("box");
        
        GUILayout.Label("NavMesh Debug Info", GUI.skin.box);
        
        if (navMeshData.vertices != null)
        {
            GUILayout.Label($"Vertices: {navMeshData.vertices.Length}");
            GUILayout.Label($"Triangles: {navMeshData.indices.Length / 3}");
            GUILayout.Label($"Areas: {navMeshData.areas.Length}");
        }
        else
        {
            GUILayout.Label("NavMesh not ready");
        }
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
    
    /// <summary>
    /// Get NavMesh info as string
    /// </summary>
    public string GetNavMeshInfo()
    {
        if (navMeshData.vertices == null || navMeshData.vertices.Length == 0)
            return "NavMesh not ready";
        
        return $"NavMesh: {navMeshData.vertices.Length} vertices, {navMeshData.indices.Length / 3} triangles";
    }
}
