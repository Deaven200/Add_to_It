using System;
using System.Collections.Generic;
using UnityEngine;

public class AuraEffect : MonoBehaviour
{
    [Header("Aura Configuration")]
    [SerializeField] private UpgradeData.UpgradeType auraType;
    [SerializeField] private float radius = 3f;
    [SerializeField] private LayerMask targetLayerMask;
    
    [Header("Visual Settings")]
    [SerializeField] private bool showVisualEffect = true;
    [SerializeField] private bool startActive = true; // Whether the aura starts active or not
    [SerializeField] private float pulseSpeed = 1f;
    [SerializeField] private float pulseIntensity = 0.2f;
    
    [Header("Orbiting Prefabs")]
    [SerializeField] private GameObject orbitingPrefab; // Prefab to orbit around the player
    [SerializeField] private float orbitSpeed = 30f; // Degrees per second
    [SerializeField] private bool randomizeStartPositions = true; // Randomize initial positions
    
    // Events for trigger detection
    public event Action<Collider> OnAuraTriggerEnter;
    public event Action<Collider> OnAuraTriggerExit;
    public event Action<Collider> OnAuraTriggerStay;
    
    // Components
    private CapsuleCollider auraCollider;
    private MeshRenderer auraRenderer;
    private Material auraMaterial;
    private float originalAlpha;
    private bool isAuraEnabled = true; // Track if aura functionality is enabled
    
    // Orbiting prefabs
    private List<GameObject> orbitingObjects = new List<GameObject>();
    private List<float> orbitAngles = new List<float>();
    
    void Awake()
    {
        SetupAuraCollider();
        SetupVisualEffect();
        
        // Set initial aura enabled state based on startActive setting
        if (!startActive)
        {
            SetAuraEnabled(false);
        }
    }
    
    void Update()
    {
        if (isAuraEnabled && showVisualEffect && auraRenderer != null)
        {
            PulseEffect();
        }
        
        // Update orbiting prefabs
        UpdateOrbitingPrefabs();
    }
    
    public void Initialize(UpgradeData.UpgradeType type, float auraRadius, LayerMask layerMask, Vector3? visualOffset = null, float? cylinderHeight = null)
    {
        auraType = type;
        radius = auraRadius;
        targetLayerMask = layerMask;
        
        // Update collider radius
        if (auraCollider != null)
        {
            auraCollider.radius = radius;
        }
        
        // Update visual effect (cylinder)
        if (auraRenderer != null)
        {
            float height = cylinderHeight ?? 10f;
            auraRenderer.transform.localScale = new Vector3(radius * 2f, height, radius * 2f);
            
            // Apply visual offset if provided
            if (visualOffset.HasValue)
            {
                auraRenderer.transform.localPosition = visualOffset.Value;
            }
        }
        
        // Update ground ring (recreate mesh for new radius)
        Transform groundRing = transform.Find("GroundRing");
        if (groundRing != null)
        {
            MeshFilter meshFilter = groundRing.GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                meshFilter.mesh = CreateHollowRingMesh(radius, 0.2f);
            }
            
            // Apply visual offset to ground ring if provided
            if (visualOffset.HasValue)
            {
                groundRing.localPosition = new Vector3(visualOffset.Value.x, 0f, visualOffset.Value.z);
            }
        }
        
        // Update orbiting prefabs
        UpdateOrbitingPrefabCount();
    }
    
    public void SetMaterial(Material material)
    {
        if (material != null && auraRenderer != null)
        {
            auraMaterial = new Material(material);
            auraRenderer.material = auraMaterial;
            
            // Store original alpha for pulsing effect
            if (auraMaterial.HasProperty("_Color"))
            {
                originalAlpha = auraMaterial.color.a;
            }
            
            // Also apply material to ground ring with adjusted alpha
            Transform groundRing = transform.Find("GroundRing");
            if (groundRing != null)
            {
                MeshRenderer ringRenderer = groundRing.GetComponent<MeshRenderer>();
                if (ringRenderer != null)
                {
                    Material ringMaterial = new Material(material);
                    Color ringColor = ringMaterial.color;
                    ringColor.a = 0.6f; // More visible ring
                    ringMaterial.color = ringColor;
                    ringRenderer.material = ringMaterial;
                }
            }
        }
        else if (material == null)
        {
            Debug.LogWarning($"AuraEffect: Attempted to set null material for {auraType} aura");
        }
        else if (auraRenderer == null)
        {
            Debug.LogWarning($"AuraEffect: AuraRenderer is null for {auraType} aura");
        }
    }
    
    private void SetupAuraCollider()
    {
        // Add capsule collider
        auraCollider = gameObject.AddComponent<CapsuleCollider>();
        auraCollider.radius = radius;
        auraCollider.height = radius * 2f;
        auraCollider.isTrigger = true; // Default to trigger for most auras
        auraCollider.direction = 1; // Y-axis
        
        // Set layer mask for collision detection (use Default layer if Aura layer doesn't exist)
        int auraLayer = LayerMask.NameToLayer("Aura");
        if (auraLayer != -1)
        {
            gameObject.layer = auraLayer;
        }
        else
        {
            gameObject.layer = 0; // Default layer
            Debug.LogWarning("AuraEffect: 'Aura' layer not found, using Default layer instead.");
        }
    }
    
    // Method to make collider physical (for shield aura)
    public void MakeColliderPhysical()
    {
        if (auraCollider != null)
        {
            auraCollider.isTrigger = false;
        }
    }
    
    private void SetupVisualEffect()
    {
        if (!showVisualEffect) return;
        
        // Create cylindrical visual representation (invisible cylinder with ground ring)
        GameObject visualObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        visualObject.name = "AuraVisual";
        visualObject.transform.SetParent(transform);
        visualObject.transform.localPosition = Vector3.zero;
        visualObject.transform.localScale = new Vector3(radius * 2f, 10f, radius * 2f); // Tall cylinder
        
        // Remove the collider from the visual object
        Destroy(visualObject.GetComponent<Collider>());
        
        // Get renderer
        auraRenderer = visualObject.GetComponent<MeshRenderer>();
        
        // Create default material if none assigned
        if (auraMaterial == null)
        {
            auraMaterial = new Material(Shader.Find("Standard"));
            auraMaterial.SetFloat("_Mode", 3); // Transparent mode
            auraMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            auraMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            auraMaterial.SetInt("_ZWrite", 0);
            auraMaterial.DisableKeyword("_ALPHATEST_ON");
            auraMaterial.EnableKeyword("_ALPHABLEND_ON");
            auraMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            auraMaterial.renderQueue = 3000;
            
            // Set default color based on aura type - make cylinder invisible
            Color defaultColor = GetDefaultColorForAuraType(auraType);
            defaultColor.a = 0f; // Make cylinder completely transparent
            auraMaterial.color = defaultColor;
            originalAlpha = defaultColor.a;
        }
        
        auraRenderer.material = auraMaterial;
        
        // Make the cylinder completely invisible by disabling the renderer
        auraRenderer.enabled = false;
        
        // Create ground ring indicator
        CreateGroundRing();
    }
    
    private void CreateGroundRing()
    {
        // Create a hollow ring on the ground to show aura boundary
        GameObject ringObject = new GameObject("GroundRing");
        ringObject.transform.SetParent(transform);
        ringObject.transform.localPosition = new Vector3(0, 0f, 0); // Exactly on the ground
        
        // Create mesh filter and renderer
        MeshFilter meshFilter = ringObject.AddComponent<MeshFilter>();
        MeshRenderer ringRenderer = ringObject.AddComponent<MeshRenderer>();
        
        // Create hollow ring mesh
        Mesh ringMesh = CreateHollowRingMesh(radius, 0.2f); // 0.2f is ring thickness
        meshFilter.mesh = ringMesh;
        
        // Create ring material
        Material ringMaterial = new Material(Shader.Find("Standard"));
        ringMaterial.SetFloat("_Mode", 3); // Transparent mode
        ringMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        ringMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        ringMaterial.SetInt("_ZWrite", 0);
        ringMaterial.DisableKeyword("_ALPHATEST_ON");
        ringMaterial.EnableKeyword("_ALPHABLEND_ON");
        ringMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        ringMaterial.renderQueue = 3001; // Above the main aura
        
        // Set ring color based on aura type (more visible than the cylinder)
        Color ringColor = GetDefaultColorForAuraType(auraType);
        ringColor.a = 0.6f; // More visible ring
        ringMaterial.color = ringColor;
        
        ringRenderer.material = ringMaterial;
        
        // Store reference for pulsing effect
        ringMaterial.name = "RingMaterial";
    }
    
    private Mesh CreateHollowRingMesh(float outerRadius, float thickness)
    {
        Mesh mesh = new Mesh();
        
        int segments = 32; // Number of segments in the ring
        float innerRadius = outerRadius - thickness;
        
        Vector3[] vertices = new Vector3[segments * 2];
        int[] triangles = new int[segments * 6];
        
        // Create vertices for inner and outer circles
        for (int i = 0; i < segments; i++)
        {
            float angle = (2f * Mathf.PI * i) / segments;
            float x = Mathf.Cos(angle);
            float z = Mathf.Sin(angle);
            
            // Outer circle vertices
            vertices[i] = new Vector3(x * outerRadius, 0, z * outerRadius);
            // Inner circle vertices
            vertices[i + segments] = new Vector3(x * innerRadius, 0, z * innerRadius);
        }
        
        // Create triangles to connect inner and outer circles
        for (int i = 0; i < segments; i++)
        {
            int next = (i + 1) % segments;
            
            // First triangle
            triangles[i * 6] = i;
            triangles[i * 6 + 1] = next;
            triangles[i * 6 + 2] = i + segments;
            
            // Second triangle
            triangles[i * 6 + 3] = next;
            triangles[i * 6 + 4] = next + segments;
            triangles[i * 6 + 5] = i + segments;
        }
        
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        
        return mesh;
    }
    
    private Color GetDefaultColorForAuraType(UpgradeData.UpgradeType type)
    {
        switch (type)
        {
            case UpgradeData.UpgradeType.CoinMagnetAura:
                return new Color(1f, 1f, 0f, 0.3f); // Yellow
            case UpgradeData.UpgradeType.SlowAura:
                return new Color(0f, 0f, 1f, 0.3f); // Blue
            default:
                return new Color(1f, 1f, 1f, 0.3f); // White
        }
    }
    
    private void PulseEffect()
    {
        if (auraMaterial != null && auraMaterial.HasProperty("_Color"))
        {
            float pulse = Mathf.Sin(Time.time * pulseSpeed) * pulseIntensity + 1f;
            Color currentColor = auraMaterial.color;
            currentColor.a = originalAlpha * pulse;
            auraMaterial.color = currentColor;
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (!isAuraEnabled) return; // Skip if aura is disabled
        
        // For coin magnet aura, check by tag instead of layer
        if (auraType == UpgradeData.UpgradeType.CoinMagnetAura)
        {
            if (other.CompareTag("Money"))
            {
                OnAuraTriggerEnter?.Invoke(other);
            }
        }
        else
        {
            // Check if the collider is on the target layer
            if (((1 << other.gameObject.layer) & targetLayerMask) != 0)
            {
                OnAuraTriggerEnter?.Invoke(other);
            }
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (!isAuraEnabled) return; // Skip if aura is disabled
        
        // For coin magnet aura, check by tag instead of layer
        if (auraType == UpgradeData.UpgradeType.CoinMagnetAura)
        {
            if (other.CompareTag("Money"))
            {
                OnAuraTriggerExit?.Invoke(other);
            }
        }
        else
        {
            // Check if the collider is on the target layer
            if (((1 << other.gameObject.layer) & targetLayerMask) != 0)
            {
                OnAuraTriggerExit?.Invoke(other);
            }
        }
    }
    
    void OnTriggerStay(Collider other)
    {
        if (!isAuraEnabled) return; // Skip if aura is disabled
        
        // For coin magnet aura, check by tag instead of layer
        if (auraType == UpgradeData.UpgradeType.CoinMagnetAura)
        {
            if (other.CompareTag("Money"))
            {
                OnAuraTriggerStay?.Invoke(other);
            }
        }
        else
        {
            // Check if the collider is on the target layer
            if (((1 << other.gameObject.layer) & targetLayerMask) != 0)
            {
                OnAuraTriggerStay?.Invoke(other);
            }
        }
    }
    
    // Public getters
    public UpgradeData.UpgradeType GetAuraType() => auraType;
    public float GetRadius() => radius;
    public bool IsActive() => gameObject.activeInHierarchy;
    public bool IsAuraEnabled() => isAuraEnabled;
    
    // Public methods to control aura state
    public void SetAuraEnabled(bool enabled)
    {
        isAuraEnabled = enabled;
        
        // Show/hide visual effects based on enabled state
        if (auraRenderer != null)
        {
            auraRenderer.enabled = enabled;
        }
        
        // Show/hide ground ring
        Transform groundRing = transform.Find("GroundRing");
        if (groundRing != null)
        {
            MeshRenderer ringRenderer = groundRing.GetComponent<MeshRenderer>();
            if (ringRenderer != null)
            {
                ringRenderer.enabled = enabled;
            }
        }
        
        // Enable/disable collider
        if (auraCollider != null)
        {
            auraCollider.enabled = enabled;
        }
        
        // Show/hide orbiting prefabs
        foreach (GameObject orbitingObj in orbitingObjects)
        {
            if (orbitingObj != null)
            {
                orbitingObj.SetActive(enabled);
            }
        }
    }
    
    public void EnableAura()
    {
        SetAuraEnabled(true);
    }
    
    public void DisableAura()
    {
        SetAuraEnabled(false);
    }
    
    // Legacy methods for backward compatibility
    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
    
    public void Activate()
    {
        SetActive(true);
    }
    
    public void Deactivate()
    {
        SetActive(false);
    }
    
    // Getter for start active setting
    public bool GetStartActive() => startActive;
    
    // Orbiting prefab methods
    private void UpdateOrbitingPrefabCount()
    {
        // Only create default prefab if we still don't have one after a short delay
        if (orbitingPrefab == null)
        {
            StartCoroutine(DelayedCreateDefaultPrefab());
            return;
        }
        
        if (orbitingPrefab == null) return;
        
        int targetCount = Mathf.RoundToInt(radius);
        targetCount = Mathf.Max(targetCount, 1); // Minimum 1 prefab
        targetCount = Mathf.Min(targetCount, 20); // Maximum 20 prefabs for performance
        
        // Remove excess prefabs
        while (orbitingObjects.Count > targetCount)
        {
            if (orbitingObjects.Count > 0)
            {
                GameObject obj = orbitingObjects[orbitingObjects.Count - 1];
                if (obj != null)
                {
                    Destroy(obj);
                }
                orbitingObjects.RemoveAt(orbitingObjects.Count - 1);
                orbitAngles.RemoveAt(orbitAngles.Count - 1);
            }
        }
        
        // Add missing prefabs
        while (orbitingObjects.Count < targetCount)
        {
            CreateOrbitingPrefab();
        }
    }
    
    private System.Collections.IEnumerator DelayedCreateDefaultPrefab()
    {
        // Wait a short time to allow AuraManagerUI to set the prefab
        yield return new WaitForSeconds(0.1f);
        
        // If we still don't have a prefab, create the default one
        if (orbitingPrefab == null)
        {
            CreateDefaultCoinPrefab();
            UpdateOrbitingPrefabCount(); // Call again to create the objects
        }
    }
    
    private void CreateDefaultCoinPrefab()
    {
        // Create a simple coin-like object for orbiting
        GameObject defaultCoin = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        defaultCoin.name = "DefaultCoin";
        defaultCoin.transform.localScale = new Vector3(0.5f, 0.1f, 0.5f); // Flat coin shape
        
        // Remove the collider to avoid physics issues
        Destroy(defaultCoin.GetComponent<Collider>());
        
        // Create a yellow material for the coin
        Material coinMaterial = new Material(Shader.Find("Standard"));
        coinMaterial.color = new Color(1f, 1f, 0f, 1f); // Bright yellow
        defaultCoin.GetComponent<MeshRenderer>().material = coinMaterial;
        
        // Set as the orbiting prefab
        orbitingPrefab = defaultCoin;
        
        Debug.Log("Created default coin prefab for orbiting");
    }
    
    private void CreateOrbitingPrefab()
    {
        if (orbitingPrefab == null) return;
        
        GameObject newPrefab = Instantiate(orbitingPrefab, transform);
        orbitingObjects.Add(newPrefab);
        
        // Set initial angle
        float startAngle;
        if (randomizeStartPositions)
        {
            startAngle = UnityEngine.Random.Range(0f, 360f);
        }
        else
        {
            // Evenly distribute prefabs around the circle
            int currentIndex = orbitingObjects.Count - 1;
            startAngle = (360f / Mathf.Max(orbitingObjects.Count, 1)) * currentIndex;
        }
        orbitAngles.Add(startAngle);
        
        // Set initial position
        UpdateOrbitingPrefabPosition(orbitingObjects.Count - 1);
    }
    
    private void UpdateOrbitingPrefabs()
    {
        if (!isAuraEnabled) return;
        
        for (int i = 0; i < orbitingObjects.Count; i++)
        {
            if (orbitingObjects[i] != null)
            {
                // Update angle
                orbitAngles[i] += orbitSpeed * Time.deltaTime;
                if (orbitAngles[i] >= 360f)
                {
                    orbitAngles[i] -= 360f;
                }
                
                // Update position
                UpdateOrbitingPrefabPosition(i);
            }
        }
    }
    
    private void UpdateOrbitingPrefabPosition(int index)
    {
        if (index < 0 || index >= orbitingObjects.Count || orbitingObjects[index] == null) return;
        
        float angleRad = orbitAngles[index] * Mathf.Deg2Rad;
        Vector3 position = new Vector3(
            Mathf.Cos(angleRad) * radius,
            1f, // Slightly above ground level for better visibility
            Mathf.Sin(angleRad) * radius
        );
        
        // Use smooth movement to prevent vibration
        orbitingObjects[index].transform.localPosition = Vector3.Lerp(
            orbitingObjects[index].transform.localPosition, 
            position, 
            Time.deltaTime * 10f // Smooth interpolation speed
        );
        
        // Add smooth rotation in place
        orbitingObjects[index].transform.Rotate(0f, 90f * Time.deltaTime, 0f, Space.Self);
    }
    
    public void SetOrbitingPrefab(GameObject prefab)
    {
        // Clear existing orbiting objects first
        foreach (GameObject orbitingObj in orbitingObjects)
        {
            if (orbitingObj != null)
            {
                Destroy(orbitingObj);
            }
        }
        orbitingObjects.Clear();
        orbitAngles.Clear();
        
        // Set new prefab
        orbitingPrefab = prefab;
        
        // Update orbiting prefab count (this will create new objects with the new prefab)
        UpdateOrbitingPrefabCount();
        
        Debug.Log($"AuraEffect: Set orbiting prefab to {(prefab != null ? prefab.name : "null")}");
    }
    
    public void SetOrbitSpeed(float speed)
    {
        orbitSpeed = speed;
    }
    
    // Method to set a coin prefab for the coin magnet aura
    public void SetCoinPrefab(GameObject coinPrefab)
    {
        if (auraType == UpgradeData.UpgradeType.CoinMagnetAura)
        {
            SetOrbitingPrefab(coinPrefab);
        }
    }
    
    // Getter methods for aura information
    public GameObject GetOrbitingPrefab() => orbitingPrefab;
    public float GetOrbitSpeed() => orbitSpeed;
    public int GetOrbitingObjectCount() => orbitingObjects.Count;
    public bool GetRandomizeStartPositions() => randomizeStartPositions;
    public List<GameObject> GetOrbitingObjects() => new List<GameObject>(orbitingObjects);
    
    // Method to get aura info as string
    public string GetAuraInfo()
    {
        return $"Type: {auraType}, Radius: {radius}, Enabled: {isAuraEnabled}, Orbiting Objects: {orbitingObjects.Count}";
    }
    
    // Cleanup method to properly destroy orbiting objects
    void OnDestroy()
    {
        // Clean up orbiting objects
        foreach (GameObject orbitingObj in orbitingObjects)
        {
            if (orbitingObj != null)
            {
                Destroy(orbitingObj);
            }
        }
        orbitingObjects.Clear();
        orbitAngles.Clear();
    }
    
    // Debug methods
    void OnDrawGizmosSelected()
    {
        Gizmos.color = GetDefaultColorForAuraType(auraType);
        Gizmos.DrawWireSphere(transform.position, radius);
    }
    
    void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = GetDefaultColorForAuraType(auraType);
            Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 0.1f);
            Gizmos.DrawSphere(transform.position, radius);
        }
    }
}
