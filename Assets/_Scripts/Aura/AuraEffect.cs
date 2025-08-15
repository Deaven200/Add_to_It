using System;
using UnityEngine;

public class AuraEffect : MonoBehaviour
{
    [Header("Aura Configuration")]
    [SerializeField] private UpgradeData.UpgradeType auraType;
    [SerializeField] private float radius = 3f;
    [SerializeField] private LayerMask targetLayerMask;
    
    [Header("Visual Settings")]
    [SerializeField] private bool showVisualEffect = true;
    [SerializeField] private float pulseSpeed = 1f;
    [SerializeField] private float pulseIntensity = 0.2f;
    
    // Events for trigger detection
    public event Action<Collider> OnAuraTriggerEnter;
    public event Action<Collider> OnAuraTriggerExit;
    public event Action<Collider> OnAuraTriggerStay;
    
    // Components
    private CapsuleCollider auraCollider;
    private MeshRenderer auraRenderer;
    private Material auraMaterial;
    private float originalAlpha;
    
    void Awake()
    {
        SetupAuraCollider();
        SetupVisualEffect();
    }
    
    void Update()
    {
        if (showVisualEffect && auraRenderer != null)
        {
            PulseEffect();
        }
    }
    
    public void Initialize(UpgradeData.UpgradeType type, float auraRadius, LayerMask layerMask)
    {
        auraType = type;
        radius = auraRadius;
        targetLayerMask = layerMask;
        
        // Update collider radius
        if (auraCollider != null)
        {
            auraCollider.radius = radius;
        }
        
        // Update visual effect
        if (auraRenderer != null)
        {
            auraRenderer.transform.localScale = Vector3.one * (radius * 2f);
        }
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
        
        // Create visual representation
        GameObject visualObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        visualObject.name = "AuraVisual";
        visualObject.transform.SetParent(transform);
        visualObject.transform.localPosition = Vector3.zero;
        visualObject.transform.localScale = Vector3.one * (radius * 2f);
        
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
            
            // Set default color based on aura type
            Color defaultColor = GetDefaultColorForAuraType(auraType);
            auraMaterial.color = defaultColor;
            originalAlpha = defaultColor.a;
        }
        
        auraRenderer.material = auraMaterial;
    }
    
    private Color GetDefaultColorForAuraType(UpgradeData.UpgradeType type)
    {
        switch (type)
        {
            case UpgradeData.UpgradeType.CoinMagnetAura:
                return new Color(1f, 1f, 0f, 0.3f); // Yellow
            case UpgradeData.UpgradeType.SlowAura:
                return new Color(0f, 0f, 1f, 0.3f); // Blue
            case UpgradeData.UpgradeType.ShieldAura:
                return new Color(0f, 1f, 0f, 0.3f); // Green
            case UpgradeData.UpgradeType.DamageAura:
                return new Color(1f, 0f, 0f, 0.3f); // Red
            case UpgradeData.UpgradeType.HealAura:
                return new Color(1f, 0f, 1f, 0.3f); // Magenta
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
