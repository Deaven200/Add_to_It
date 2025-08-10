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
            Debug.Log($"WeaponStation: WeaponManager found automatically: {(weaponManager != null ? "YES" : "NO")}");
        }
        else
        {
            Debug.Log("WeaponStation: WeaponManager assigned in inspector");
        }
        
        // Try to find the player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            Debug.Log($"WeaponStation: Player found with tag 'Player': {player.name}");
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
        
        Debug.Log($"WeaponStation initialized. Interaction range: {interactionRange}, Can interact: {canInteract}");
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
                    Debug.Log("WeaponStation: Player in range! Press E to access weapon station");
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
                    
                    Debug.Log("WeaponStation: Player left range");
                }
            }
        }
        
        // Check for E key when player is in range
        if (playerInRange && canInteract)
        {
            // Debug: Check if E key is being pressed
            if (Input.GetKey(KeyCode.E))
            {
                Debug.Log("WeaponStation: E key is being held down");
            }
            
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("WeaponStation: E key pressed! Attempting to open weapon selection...");
                OpenWeaponSelection();
            }
        }
        
        // Debug: Check for any key press to test input system
        if (Input.anyKeyDown)
        {
            Debug.Log($"WeaponStation: Key pressed: {Input.inputString}");
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"WeaponStation: Trigger entered by: {other.name} with tag: {other.tag}");
        
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            
            // Visual feedback - highlight the station
            if (stationRenderer != null && highlightMaterial != null)
            {
                stationRenderer.material = highlightMaterial;
            }
            
            Debug.Log("WeaponStation: Player entered trigger! Press E to access weapon station");
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        Debug.Log($"WeaponStation: Trigger exited by: {other.name} with tag: {other.tag}");
        
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            
            // Visual feedback - return to normal material
            if (stationRenderer != null && normalMaterial != null)
            {
                stationRenderer.material = normalMaterial;
            }
            
            Debug.Log("WeaponStation: Player exited trigger");
        }
    }
    
    void OpenWeaponSelection()
    {
        Debug.Log($"WeaponStation: OpenWeaponSelection called. WeaponManager null? {(weaponManager == null ? "YES" : "NO")}");
        
        if (weaponManager != null)
        {
            weaponManager.ShowWeaponSelection(this);
            Debug.Log("WeaponStation: Weapon station opened successfully!");
        }
        else
        {
            Debug.LogError("WeaponStation: WeaponManager not found! Please assign it in the inspector or ensure it exists in the scene.");
            
            // Try to find it again
            weaponManager = FindObjectOfType<WeaponManager>();
            if (weaponManager != null)
            {
                Debug.Log("WeaponStation: Found WeaponManager automatically, trying again...");
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
        Debug.Log("WeaponStation: Testing weapon selection manually...");
        OpenWeaponSelection();
    }
}