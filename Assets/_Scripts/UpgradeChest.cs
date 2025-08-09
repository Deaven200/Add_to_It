using UnityEngine;

public class UpgradeChest : MonoBehaviour
{
    [Header("Upgrade Manager Reference")]
    public UpgradeManager upgradeManager;
    
    [Header("Interaction Settings")]
    public bool canInteract = true;
    public bool openOnTouch = true;
    [Range(0.5f, 5f)]
    public float interactionRange = 2f;
    
    [Header("Visual Feedback")]
    public Material normalMaterial;
    public Material highlightMaterial;
    public Material openedMaterial; // New material for when chest is opened
    private Renderer chestRenderer;
    
    [Header("Audio")]
    public AudioClip openSound;
    public AudioClip closeSound;
    private AudioSource audioSource;
    
    private bool playerInRange = false;
    private bool isOpened = false;
    private Transform playerTransform;
    
    void Start()
    {
        // Get the renderer for visual feedback
        chestRenderer = GetComponent<Renderer>();
        
        // Get or add audio source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Try to find the upgrade manager if not assigned
        if (upgradeManager == null)
        {
            upgradeManager = FindObjectOfType<UpgradeManager>();
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
        // Don't process if already opened
        if (isOpened) return;
        
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
                        // Show interaction prompt
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
        if (other.CompareTag("Player") && !openOnTouch && !isOpened)
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
        if (upgradeManager != null && !isOpened)
        {
            // Mark as opened
            isOpened = true;
            
            // Change material to opened state
            if (chestRenderer != null && openedMaterial != null)
            {
                chestRenderer.material = openedMaterial;
            }
            
            // Play open sound
            if (audioSource != null && openSound != null)
            {
                audioSource.PlayOneShot(openSound);
            }
            
            // Open the upgrade manager
            upgradeManager.OnUpgradeButtonPressed(this);
            upgradeManager.PauseGame();
            
            // Disable interaction after use
            canInteract = false;
            
            Debug.Log("Upgrade chest opened!");
        }
        else
        {
            Debug.LogError("UpgradeManager not found! Please assign it in the inspector or ensure it exists in the scene.");
        }
    }
    
    // Alternative method for automatic opening on collision
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && canInteract && openOnTouch && !isOpened)
        {
            OpenUpgradeManager();
        }
    }
    
    // Method to reset chest (for testing)
    [ContextMenu("Reset Chest")]
    public void ResetChest()
    {
        isOpened = false;
        canInteract = true;
        
        if (chestRenderer != null && normalMaterial != null)
        {
            chestRenderer.material = normalMaterial;
        }
    }
    
    // Optional: Draw the interaction range in the scene view for debugging
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
} 