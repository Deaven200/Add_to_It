using UnityEngine;

public class UpgradeChest : MonoBehaviour
{
    [Header("Upgrade Manager Reference")]
    public UpgradeSelectionUI upgradeManager;
    
    [Header("Interaction Settings")]
    public bool canInteract = true;
    public bool openOnTouch = true; // New setting to control automatic opening
    [Range(0.5f, 5f)]
    public float interactionRange = 2f; // Configurable interaction range
    
    [Header("Visual Feedback")]
    public Material normalMaterial;
    public Material highlightMaterial;
    private Renderer chestRenderer;
    
    private bool playerInRange = false;
    private Transform playerTransform;
    
    void Start()
    {
        // Get the renderer for visual feedback
        chestRenderer = GetComponent<Renderer>();
        
        // Try to find the upgrade manager if not assigned
        if (upgradeManager == null)
        {
            upgradeManager = FindObjectOfType<UpgradeSelectionUI>();
        }
        
        // Try to find the player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        
        // Set initial material
        if (chestRenderer != null && normalMaterial != null)
        {
            chestRenderer.material = normalMaterial;
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
                    
                    // Visual feedback - highlight the chest
                    if (chestRenderer != null && highlightMaterial != null)
                    {
                        chestRenderer.material = highlightMaterial;
                    }
                    
                    // If automatic opening is enabled, open immediately
                    if (openOnTouch)
                    {
                        OpenUpgradeManager();
                    }
                    else
                    {
                        // Show interaction prompt (you can implement UI here)
                        Debug.Log("Press E to open upgrade chest");
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
                    if (chestRenderer != null && normalMaterial != null)
                    {
                        chestRenderer.material = normalMaterial;
                    }
                }
            }
        }
        
        // Only check for E key if automatic opening is disabled
        if (!openOnTouch && playerInRange && canInteract && Input.GetKeyDown(KeyCode.E))
        {
            OpenUpgradeManager();
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Keep this for backward compatibility, but range-based detection takes priority
        if (other.CompareTag("Player") && !openOnTouch)
        {
            playerInRange = true;
            
            // Visual feedback - highlight the chest
            if (chestRenderer != null && highlightMaterial != null)
            {
                chestRenderer.material = highlightMaterial;
            }
            
            Debug.Log("Press E to open upgrade chest");
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        // Keep this for backward compatibility
        if (other.CompareTag("Player") && !openOnTouch)
        {
            playerInRange = false;
            
            // Visual feedback - return to normal material
            if (chestRenderer != null && normalMaterial != null)
            {
                chestRenderer.material = normalMaterial;
            }
        }
    }
    
    void OpenUpgradeManager()
    {
        if (upgradeManager != null)
        {
            upgradeManager.OnUpgradeButtonPressed(this);
            upgradeManager.PauseGame();
            
            // Optional: Disable interaction after use
            canInteract = false;
            
            Debug.Log("Upgrade chest opened!");
        }
        else
        {
            Debug.LogError("UpgradeManager not found! Please assign it in the inspector or ensure it exists in the scene.");
        }
    }
    
    // Alternative method for automatic opening on collision (without key press)
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && canInteract && openOnTouch)
        {
            OpenUpgradeManager();
        }
    }
    
    // Optional: Draw the interaction range in the scene view for debugging
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
} 