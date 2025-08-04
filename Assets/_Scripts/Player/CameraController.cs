using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;           // The player object to rotate left/right
    public Transform cameraPivot;      // The camera pivot for up/down rotation
    public Transform mainCamera;       // The actual camera to move
    public Vector3 thirdPersonOffset = new Vector3(0, 1.5f, -4f);
    public Vector3 firstPersonOffset = new Vector3(0, 1.5f, 0.2f);
    public float mouseSensitivity = 3f;
    public bool isFirstPerson = false;

    float pitch = 0f;  // vertical rotation

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
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
        pitch = Mathf.Clamp(pitch, -60f, 85f);
        if (cameraPivot != null)
        {
            cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }

        // Toggle view mode
        if (Input.GetKeyDown(KeyCode.V))
        {
            isFirstPerson = !isFirstPerson;
        }

        // Move camera to correct offset for first/third person
        if (mainCamera != null)
        {
            mainCamera.localPosition = isFirstPerson ? firstPersonOffset : thirdPersonOffset;
        }
    }

    // Remove the LateUpdate method entirely for now
}
