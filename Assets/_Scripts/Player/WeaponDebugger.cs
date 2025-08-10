using UnityEngine;

public class WeaponDebugger : MonoBehaviour
{
    private WeaponManager weaponManager;
    private PlayerShooting playerShooting;
    
    void Start()
    {
        weaponManager = FindObjectOfType<WeaponManager>();
        playerShooting = FindObjectOfType<PlayerShooting>();
        
        if (weaponManager == null)
        {
            Debug.LogError("WeaponManager not found!");
            enabled = false;
            return;
        }
        
        if (playerShooting == null)
        {
            Debug.LogError("PlayerShooting not found!");
            enabled = false;
            return;
        }
        
        Debug.Log("Weapon Debugger Active!");
        Debug.Log("Press 'I' to show weapon info, 'T' to test shoot");
    }
    
    void Update()
    {
        if (weaponManager == null) return;
        
        // Show weapon info
        if (Input.GetKeyDown(KeyCode.I))
        {
            ShowWeaponInfo();
        }
        
        // Test shoot
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("Testing shoot...");
            weaponManager.Shoot();
        }
        
        // Show current weapon status every few seconds
        if (Time.frameCount % 300 == 0) // Every 5 seconds at 60fps
        {
            ShowWeaponInfo();
        }
    }
    
    void ShowWeaponInfo()
    {
        Debug.Log("=== WEAPON DEBUG INFO ===");
        Debug.Log($"Has Weapon: {weaponManager.HasWeapon()}");
        Debug.Log($"Current Weapon: {(weaponManager.GetCurrentWeapon()?.weaponName ?? "None")}");
        Debug.Log($"Current Ammo: {weaponManager.GetCurrentAmmo()}/{weaponManager.GetMaxAmmo()}");
        Debug.Log($"Is Reloading: {weaponManager.IsReloading()}");
        Debug.Log($"Available Weapons Count: {weaponManager.GetAvailableWeapons().Count}");
        
        if (weaponManager.GetCurrentWeapon() != null)
        {
            var weapon = weaponManager.GetCurrentWeapon();
            Debug.Log($"Weapon Details:");
            Debug.Log($"  - Name: {weapon.weaponName}");
            Debug.Log($"  - Damage: {weapon.damage}");
            Debug.Log($"  - Fire Rate: {weapon.fireRate}");
            Debug.Log($"  - Bullet Speed: {weapon.bulletSpeed}");
            Debug.Log($"  - Max Ammo: {weapon.maxAmmo}");
            Debug.Log($"  - Weapon Prefab: {(weapon.weaponPrefab != null ? "Assigned" : "NULL!")}");
        }
        
        Debug.Log("=== END WEAPON DEBUG INFO ===");
    }
} 