using UnityEngine;

/// <summary>
/// Helper script to set up the weapon system automatically.
/// Attach this to any GameObject in your scene to automatically configure weapons.
/// </summary>
public class WeaponSetupHelper : MonoBehaviour
{
    [Header("Auto Setup")]
    [SerializeField] private bool autoSetupOnStart = true;
    [SerializeField] private bool addAllWeaponsOnStart = true;
    
    void Start()
    {
        if (autoSetupOnStart)
        {
            SetupWeaponSystem();
        }
    }
    
    [ContextMenu("Setup Weapon System")]
    public void SetupWeaponSystem()
    {
        // Find WeaponManager
        WeaponManager weaponManager = WeaponManager.Instance;
        if (weaponManager == null)
        {
            weaponManager = FindObjectOfType<WeaponManager>();
        }
        
        if (weaponManager == null)
        {
            Debug.LogError("WeaponSetupHelper: No WeaponManager found in scene!");
            return;
        }
        
        if (addAllWeaponsOnStart)
        {
            AddAllWeaponsToManager(weaponManager);
        }
    }
    
    public void AddAllWeaponsToManager(WeaponManager weaponManager)
    {
        // Find all weapon data assets
        WeaponData[] allWeaponData = Resources.FindObjectsOfTypeAll<WeaponData>();
        
        if (allWeaponData.Length == 0)
        {
            Debug.LogWarning("WeaponSetupHelper: No weapon data assets found! Make sure you have weapon data assets in your project.");
            return;
        }
        
        // Add each weapon to the manager
        foreach (WeaponData weapon in allWeaponData)
        {
            weaponManager.AddWeapon(weapon);
        }
    }
    
    [ContextMenu("Add All Weapons")]
    public void AddAllWeapons()
    {
        WeaponManager weaponManager = WeaponManager.Instance;
        if (weaponManager == null)
        {
            weaponManager = FindObjectOfType<WeaponManager>();
        }
        
        if (weaponManager != null)
        {
            AddAllWeaponsToManager(weaponManager);
        }
    }
    
    [ContextMenu("Check Weapon System Status")]
    public void CheckWeaponSystemStatus()
    {
        // Check WeaponManager
        WeaponManager weaponManager = WeaponManager.Instance;
        if (weaponManager == null)
        {
            weaponManager = FindObjectOfType<WeaponManager>();
        }
        
        if (weaponManager == null)
        {
            Debug.LogError("❌ No WeaponManager found in scene!");
            return;
        }
        
        // Check available weapons
        int weaponCount = weaponManager.GetAvailableWeapons().Count;
        
        // Check weapon data assets
        WeaponData[] allWeaponData = Resources.FindObjectsOfTypeAll<WeaponData>();
        
        // Check UI references
        if (weaponManager.GetComponent<WeaponManager>() != null)
        {
            // Use reflection to check private fields
            var weaponSelectionUIField = typeof(WeaponManager).GetField("weaponSelectionUI", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var weaponCardPrefabField = typeof(WeaponManager).GetField("weaponCardPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var weaponCardContainerField = typeof(WeaponManager).GetField("weaponCardContainer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (weaponSelectionUIField != null)
            {
                var weaponSelectionUI = weaponSelectionUIField.GetValue(weaponManager);
            }
            
            if (weaponCardPrefabField != null)
            {
                var weaponCardPrefab = weaponCardPrefabField.GetValue(weaponManager);
            }
            
            if (weaponCardContainerField != null)
            {
                var weaponCardContainer = weaponCardContainerField.GetValue(weaponManager);
            }
        }
    }
} 