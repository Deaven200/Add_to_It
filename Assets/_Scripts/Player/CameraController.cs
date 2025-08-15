using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera References")]
    public Transform player;           // The player object to rotate left/right
    public Transform cameraPivot;      // The camera pivot for up/down rotation
    public Transform mainCamera;       // The actual camera to move
    
    [Header("Camera Positions")]
    public Vector3 thirdPersonOffset = new Vector3(0, 1.5f, -4f);
    public Vector3 firstPersonOffset = new Vector3(0, 1.5f, 0.2f);
    
    [Header("Camera Settings")]
    public float mouseSensitivity = 3f;
    public bool isFirstPerson = false;
    public bool smoothTransition = true;
    public float transitionSpeed = 5f;
    
    [Header("Pitch Limits")]
    public float minPitch = -60f;
    public float maxPitch = 85f;

    private float pitch = 0f;  // vertical rotation
    private UpgradeManager upgradeManager;
    private Vector3 targetCameraPosition;
    private bool isTransitioning = false;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        
        // Find the upgrade manager to check pause state
        upgradeManager = FindObjectOfType<UpgradeManager>();
        
        // Set initial camera position
        if (mainCamera != null)
        {
            targetCameraPosition = isFirstPerson ? firstPersonOffset : thirdPersonOffset;
            mainCamera.localPosition = targetCameraPosition;
        }
        

    }

    void Update()
    {
        // Check if the game is paused (upgrade menu is open)
        bool isPaused = false;
        if (upgradeManager != null)
        {
            isPaused = upgradeManager.isPaused;
        }
        
        // Only process mouse input if the game is not paused
        if (!isPaused)
        {
            // Get mouse input
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            // Rotate player left/right (yaw)
            if (player != null)
            {
                player.Rotate(Vector3.up * mouseX);
            }

            // Rotate camera pivot up/down (pitch)
            pitch -= mouseY;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
            if (cameraPivot != null)
            {
                cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
            }
        }

        // Toggle view mode (only when not paused)
        if (Input.GetKeyDown(KeyCode.V) && !isPaused)
        {
            ToggleViewMode();
        }

        // Handle camera position transition
        UpdateCameraPosition();
    }
    
    void ToggleViewMode()
    {
        isFirstPerson = !isFirstPerson;
        targetCameraPosition = isFirstPerson ? firstPersonOffset : thirdPersonOffset;
        isTransitioning = true;
        
        Debug.Log($"Switched to {(isFirstPerson ? "First" : "Third")} Person mode");
        
        // Optional: Add sound effect or visual feedback here
        // AudioManager.Instance.PlaySound("camera_switch");
    }
    
    void UpdateCameraPosition()
    {
        if (mainCamera == null) return;
        
        if (smoothTransition && isTransitioning)
        {
            // Smooth transition between positions
            mainCamera.localPosition = Vector3.Lerp(mainCamera.localPosition, targetCameraPosition, Time.deltaTime * transitionSpeed);
            
            // Check if transition is complete
            if (Vector3.Distance(mainCamera.localPosition, targetCameraPosition) < 0.01f)
            {
                mainCamera.localPosition = targetCameraPosition;
                isTransitioning = false;
            }
        }
        else
        {
            // Instant transition
            mainCamera.localPosition = targetCameraPosition;
        }
    }
    
    // Public methods for external control
    public void SetFirstPerson(bool firstPerson)
    {
        if (isFirstPerson != firstPerson)
        {
            ToggleViewMode();
        }
    }
    
    public void SetThirdPerson(bool thirdPerson)
    {
        if (isFirstPerson == thirdPerson)
        {
            ToggleViewMode();
        }
    }
    
    public bool IsFirstPerson()
    {
        return isFirstPerson;
    }
    
    public bool IsThirdPerson()
    {
        return !isFirstPerson;
    }
}

