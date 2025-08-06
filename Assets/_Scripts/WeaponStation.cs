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
            weaponManager = FindObjectOfType<WeaponManager>();
        }
        
        // Try to find the player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
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
                    
                    // Show interaction prompt
                    Debug.Log("Press E to access weapon station");
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
        if (playerInRange && canInteract && Input.GetKeyDown(KeyCode.E))
        {
            OpenWeaponSelection();
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
            
            Debug.Log("Press E to access weapon station");
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
            Debug.Log("Weapon station opened!");
        }
        else
        {
            Debug.LogError("WeaponManager not found! Please assign it in the inspector or ensure it exists in the scene.");
        }
    }
    
    // Optional: Draw the interaction range in the scene view for debugging
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
} 