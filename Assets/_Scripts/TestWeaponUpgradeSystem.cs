using UnityEngine;

public class TestWeaponUpgradeSystem : MonoBehaviour
{
    [Header("Test Settings")]
    [SerializeField] private WeaponData testWeapon;
    [SerializeField] private bool runTestOnStart = false;
    
    void Start()
    {
        if (runTestOnStart)
        {
            RunWeaponUpgradeTest();
        }
    }
    
    [ContextMenu("Run Weapon Upgrade Test")]
    public void RunWeaponUpgradeTest()
    {
        if (testWeapon == null)
        {
            Debug.LogError("TestWeaponUpgradeSystem: No test weapon assigned!");
            return;
        }
        
        Debug.Log("=== Starting Weapon Upgrade System Test ===");
        
        // Get the weapon manager
        WeaponManager weaponManager = WeaponManager.Instance;
        if (weaponManager == null)
        {
            Debug.LogError("TestWeaponUpgradeSystem: WeaponManager not found!");
            return;
        }
        
        // Add the test weapon to available weapons
        weaponManager.AddWeapon(testWeapon);
        
        // Equip the weapon
        weaponManager.EquipWeapon(testWeapon);
        
        // Print initial stats
        Debug.Log("--- Initial Weapon Stats ---");
        weaponManager.PrintCurrentWeaponStats();
        
        // Store original base weapon values
        float originalDamage = testWeapon.damage;
        float originalFireRate = testWeapon.fireRate;
        float originalBulletSpeed = testWeapon.bulletSpeed;
        int originalAmmo = testWeapon.maxAmmo;
        float originalReloadTime = testWeapon.reloadTime;
        
        // Apply some upgrades
        Debug.Log("--- Applying Upgrades ---");
        
        PlayerWeaponData playerWeapon = weaponManager.GetCurrentWeapon();
        if (playerWeapon != null)
        {
            // Apply damage upgrade
            playerWeapon.AddDamageModifier(5f);
            Debug.Log("Applied +5 damage upgrade");
            
            // Apply fire rate upgrade
            playerWeapon.AddFireRateModifier(20f); // 20% faster
            Debug.Log("Applied +20% fire rate upgrade");
            
            // Apply bullet speed upgrade
            playerWeapon.AddBulletSpeedModifier(10f);
            Debug.Log("Applied +10 bullet speed upgrade");
            
            // Apply ammo upgrade
            playerWeapon.AddAmmoModifier(5);
            Debug.Log("Applied +5 ammo upgrade");
            
            // Apply reload speed upgrade
            playerWeapon.AddReloadTimeModifier(0.5f);
            Debug.Log("Applied -0.5s reload time upgrade");
        }
        
        // Print stats after upgrades
        Debug.Log("--- Stats After Upgrades ---");
        weaponManager.PrintCurrentWeaponStats();
        
        // Verify base weapon data is unchanged
        Debug.Log("--- Verifying Base Weapon Data is Unchanged ---");
        bool baseDataUnchanged = true;
        
        if (testWeapon.damage != originalDamage)
        {
            Debug.LogError($"Base weapon damage was modified! Original: {originalDamage}, Current: {testWeapon.damage}");
            baseDataUnchanged = false;
        }
        
        if (testWeapon.fireRate != originalFireRate)
        {
            Debug.LogError($"Base weapon fire rate was modified! Original: {originalFireRate}, Current: {testWeapon.fireRate}");
            baseDataUnchanged = false;
        }
        
        if (testWeapon.bulletSpeed != originalBulletSpeed)
        {
            Debug.LogError($"Base weapon bullet speed was modified! Original: {originalBulletSpeed}, Current: {testWeapon.bulletSpeed}");
            baseDataUnchanged = false;
        }
        
        if (testWeapon.maxAmmo != originalAmmo)
        {
            Debug.LogError($"Base weapon ammo was modified! Original: {originalAmmo}, Current: {testWeapon.maxAmmo}");
            baseDataUnchanged = false;
        }
        
        if (testWeapon.reloadTime != originalReloadTime)
        {
            Debug.LogError($"Base weapon reload time was modified! Original: {originalReloadTime}, Current: {testWeapon.reloadTime}");
            baseDataUnchanged = false;
        }
        
        if (baseDataUnchanged)
        {
            Debug.Log("✓ Base weapon data is unchanged - upgrade system working correctly!");
        }
        else
        {
            Debug.LogError("✗ Base weapon data was modified - upgrade system has issues!");
        }
        
        Debug.Log("=== Weapon Upgrade System Test Complete ===");
    }
    
    [ContextMenu("Test Multiple Players")]
    public void TestMultiplePlayers()
    {
        Debug.Log("=== Testing Multiple Players ===");
        
        // This test simulates what would happen if multiple players used the same weapon
        // In a real game, each player would have their own WeaponManager instance
        
        // Create a second weapon manager for testing
        GameObject secondPlayer = new GameObject("SecondPlayer");
        WeaponManager secondWeaponManager = secondPlayer.AddComponent<WeaponManager>();
        
        // Add the same weapon to both managers
        WeaponManager.Instance.AddWeapon(testWeapon);
        secondWeaponManager.AddWeapon(testWeapon);
        
        // Equip the weapon on both
        WeaponManager.Instance.EquipWeapon(testWeapon);
        secondWeaponManager.EquipWeapon(testWeapon);
        
        // Apply upgrades to the first player
        PlayerWeaponData firstPlayerWeapon = WeaponManager.Instance.GetCurrentWeapon();
        firstPlayerWeapon.AddDamageModifier(10f);
        
        // Check that the second player's weapon is unaffected
        PlayerWeaponData secondPlayerWeapon = secondWeaponManager.GetCurrentWeapon();
        
        if (firstPlayerWeapon.damage != secondPlayerWeapon.damage)
        {
            Debug.Log("✓ Player-specific upgrades working correctly!");
            Debug.Log($"Player 1 damage: {firstPlayerWeapon.damage}");
            Debug.Log($"Player 2 damage: {secondPlayerWeapon.damage}");
        }
        else
        {
            Debug.LogError("✗ Player-specific upgrades not working!");
        }
        
        // Clean up
        Destroy(secondPlayer);
        
        Debug.Log("=== Multiple Players Test Complete ===");
    }
}
