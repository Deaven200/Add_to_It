using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WeaponManager weaponManager;
    
    [Header("Input")]
    [SerializeField] private KeyCode shootKey = KeyCode.Mouse0;
    [SerializeField] private KeyCode reloadKey = KeyCode.R;
    
    void Start()
    {
        // Try to find weapon manager if not assigned
        if (weaponManager == null)
        {
            weaponManager = FindObjectOfType<WeaponManager>();
        }
        
        if (weaponManager == null)
        {
            Debug.LogError("WeaponManager not found! Please assign it in the inspector.");
        }
    }

    void Update()
    {
        if (weaponManager == null) return;
        
        // Handle shooting
        if (Input.GetKey(shootKey))
        {
            weaponManager.Shoot();
        }
        
        // Handle reloading
        if (Input.GetKeyDown(reloadKey))
        {
            weaponManager.Reload();
        }
    }
}
