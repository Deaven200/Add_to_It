using UnityEngine;

public class WeaponStation : MonoBehaviour
{
    [Header("Weapon Manager Reference")]
    public WeaponManager weaponManager;
    
    [Header("Interaction Settings")]
    public bool canInteract = true;
    [Range(0.5f, 5f)]
    public float interactionRange = 2f;
    
    [Header("Visual Feedback")]
    public Material normalMaterial;
    public Material highlightMaterial;
    private Renderer stationRenderer;
    
    private bool playerInRange = false;
    private Transform playerTransform;
    
    void Start()
    {
        // Get the renderer for visual feedback
        stationRenderer = GetComponent<Renderer>();
        
        // Try to find the weapon manager if not assigned
        if (weaponManager == null)
        {
            weaponManager = WeaponManager.Instance;
            if (weaponManager == null)
            {
                weaponManager = FindObjectOfType<WeaponManager>();
            }
        }
        
        // Try to find the player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("WeaponStation: No GameObject with 'Player' tag found! Make sure your player has the 'Player' tag.");
        }
        
        // Set initial material
        if (stationRenderer != null && normalMaterial != null)
        {
            stationRenderer.material = normalMaterial;
        }
    }
    
    void Update()
    {
        // Check distance to player for range-based interaction
        if (playerTransform != null && canInteract)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            
            // Check if player is within range
            if (distanceToPlayer <= interactionRange)
            {
                if (!playerInRange)
                {
                    // Player just entered range
                    playerInRange = true;
                    
                    // Visual feedback - highlight the station
                    if (stationRenderer != null && highlightMaterial != null)
                    {
                        stationRenderer.material = highlightMaterial;
                    }
                }
            }
            else
            {
                if (playerInRange)
                {
                    // Player just left range
                    playerInRange = false;
                    
                    // Visual feedback - return to normal material
                    if (stationRenderer != null && normalMaterial != null)
                    {
                        stationRenderer.material = normalMaterial;
                    }
                }
            }
        }
        
        // Check for E key when player is in range
        if (playerInRange && canInteract)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                OpenWeaponSelection();
            }
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            
            // Visual feedback - highlight the station
            if (stationRenderer != null && highlightMaterial != null)
            {
                stationRenderer.material = highlightMaterial;
            }
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            
            // Visual feedback - return to normal material
            if (stationRenderer != null && normalMaterial != null)
            {
                stationRenderer.material = normalMaterial;
            }
        }
    }
    
    void OpenWeaponSelection()
    {
        if (weaponManager != null)
        {
            weaponManager.ShowWeaponSelection(this);
        }
        else
        {
            Debug.LogError("WeaponStation: WeaponManager not found! Please assign it in the inspector or ensure it exists in the scene.");
            
            // Try to find it again
            weaponManager = FindObjectOfType<WeaponManager>();
            if (weaponManager != null)
            {
                weaponManager.ShowWeaponSelection(this);
            }
            else
            {
                Debug.LogError("WeaponStation: Still cannot find WeaponManager!");
            }
        }
    }
    
    // Optional: Draw the interaction range in the scene view for debugging
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
    
    // Debug method to test weapon selection manually
    [ContextMenu("Test Open Weapon Selection")]
    public void TestOpenWeaponSelection()
    {
        OpenWeaponSelection();
    }
}