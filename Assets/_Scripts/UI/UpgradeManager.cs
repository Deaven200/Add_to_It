using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Add this for LayoutRebuilder

public class UpgradeManager : MonoBehaviour
{
    [Header("Upgrade Generation")]
    [SerializeField] private RandomUpgradeGenerator upgradeGenerator;
    [SerializeField] private int baseUpgradesToShow = 3;
    [SerializeField] private int maxUpgradesToShow = 9;
    
    [Header("UI References")]
    [SerializeField] private GameObject upgradePanelPrefab;
    [SerializeField] private GameObject upgradeCardPrefab;
    [SerializeField] private GameObject canvas;
    
    private GameObject _activeUpgradeInstance;
    private GameObject _buttonContainerInstance;
    private UpgradeChest _currentChest; // Reference to the chest that opened this menu
    private List<UpgradeData> _currentUpgrades = new List<UpgradeData>();
    
    // Track upgrade options bonus
    private int upgradeOptionsBonus = 0;

    public bool isPaused = false;
    
    private void Awake()
    {
        // Try to find the upgrade generator if not assigned
        if (upgradeGenerator == null)
        {
            upgradeGenerator = FindObjectOfType<RandomUpgradeGenerator>();
            if (upgradeGenerator == null)
            {
                // Create one if it doesn't exist
                GameObject generatorGO = new GameObject("RandomUpgradeGenerator");
                upgradeGenerator = generatorGO.AddComponent<RandomUpgradeGenerator>();
            }
        }
    }
    
    public void ShowUpgradeChoices()
    {
        _activeUpgradeInstance.SetActive(true);

        // Calculate how many upgrades to show (base + bonus, capped at max)
        int upgradesToShow = Mathf.Min(baseUpgradesToShow + upgradeOptionsBonus, maxUpgradesToShow);
        
        // Generate random upgrades - the new clean generator handles randomness internally
        if (upgradeGenerator != null)
        {
            _currentUpgrades = upgradeGenerator.GenerateUniqueUpgrades(upgradesToShow);
        }
        else
        {
            Debug.LogError("[UPGRADE MANAGER] No upgrade generator found!");
            _currentUpgrades = new List<UpgradeData>();
        }

        // Clear any existing cards first
        if (_buttonContainerInstance != null)
        {
            foreach (Transform child in _buttonContainerInstance.transform)
            {
                Destroy(child.gameObject);
            }
        }

        // Create a button for each upgrade
        foreach(UpgradeData upgradeData in _currentUpgrades)
        {
            GameObject cardGO = Instantiate(upgradeCardPrefab, _buttonContainerInstance.transform);
            cardGO.GetComponent<UpgradeCard>().SetUpgradeData(upgradeData);
        }
        
        // Force layout update to ensure proper positioning
        Canvas.ForceUpdateCanvases();
        if (_buttonContainerInstance != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(_buttonContainerInstance.GetComponent<RectTransform>());
        }
    }

    public void SelectUpgrade(UpgradeData upgrade)
    {
        // Apply the upgrade based on its type
        ApplyUpgrade(upgrade);
        
        // Close the menu and remove the chest
        ResumeGame();
        
        // Destroy the upgrade panel to ensure fresh generation next time
        if (_activeUpgradeInstance != null)
        {
            Destroy(_activeUpgradeInstance);
            _activeUpgradeInstance = null;
            _buttonContainerInstance = null;
        }
        
        // Remove the chest that opened this menu
        if (_currentChest != null)
        {
            Destroy(_currentChest.gameObject);
            _currentChest = null;
        }
    }
    
    private void ApplyUpgrade(UpgradeData upgrade)
    {
        // Find the player to apply upgrades to
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player not found! Cannot apply upgrade.");
            return;
        }
        
        switch (upgrade.upgradeType)
        {
            case UpgradeData.UpgradeType.Health:
                PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.SetMaxHealth(playerHealth.maxHealth + (int)upgrade.value);
                    playerHealth.Heal((int)upgrade.value);
                }
                break;
                
            case UpgradeData.UpgradeType.Damage:
                // Apply damage upgrade to weapon manager
                WeaponManager weaponManager = WeaponManager.Instance;
                if (weaponManager != null && weaponManager.GetCurrentWeapon() != null)
                {
                    PlayerWeaponData currentWeapon = weaponManager.GetCurrentWeapon();
                    currentWeapon.AddDamageModifier(upgrade.value);
                }
                break;
                
            case UpgradeData.UpgradeType.FireRate:
                // Apply fire rate upgrade to weapon manager
                WeaponManager weaponMgr = WeaponManager.Instance;
                if (weaponMgr != null && weaponMgr.GetCurrentWeapon() != null)
                {
                    PlayerWeaponData weapon = weaponMgr.GetCurrentWeapon();
                    weapon.AddFireRateModifier(upgrade.value);
                }
                break;
                
            case UpgradeData.UpgradeType.BulletSpeed:
                // Apply bullet speed upgrade to weapon manager
                WeaponManager bulletSpeedMgr = WeaponManager.Instance;
                if (bulletSpeedMgr != null && bulletSpeedMgr.GetCurrentWeapon() != null)
                {
                    PlayerWeaponData bulletWeapon = bulletSpeedMgr.GetCurrentWeapon();
                    bulletWeapon.AddBulletSpeedModifier(upgrade.value);
                }
                break;
                
            case UpgradeData.UpgradeType.AmmoCapacity:
                // Apply ammo capacity upgrade to weapon manager
                WeaponManager ammoMgr = WeaponManager.Instance;
                if (ammoMgr != null && ammoMgr.GetCurrentWeapon() != null)
                {
                    PlayerWeaponData ammoWeapon = ammoMgr.GetCurrentWeapon();
                    ammoWeapon.AddAmmoModifier((int)upgrade.value);
                }
                break;
                
            case UpgradeData.UpgradeType.ReloadSpeed:
                // Apply reload speed upgrade to weapon manager
                WeaponManager reloadMgr = WeaponManager.Instance;
                if (reloadMgr != null && reloadMgr.GetCurrentWeapon() != null)
                {
                    PlayerWeaponData reloadWeapon = reloadMgr.GetCurrentWeapon();
                    reloadWeapon.AddReloadTimeModifier(upgrade.value);
                }
                break;
                
            case UpgradeData.UpgradeType.MoveSpeed:
                // Apply speed upgrade to player movement
                PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
                if (playerMovement != null)
                {
                    // You'll need to add a speed field to PlayerMovement
                }
                break;
                
            case UpgradeData.UpgradeType.DamageResistance:
                // Apply damage resistance to player health
                PlayerHealth health = player.GetComponent<PlayerHealth>();
                if (health != null)
                {
                    // You'll need to add damage resistance to PlayerHealth
                }
                break;
                
            case UpgradeData.UpgradeType.LifeOnKill:
                // Apply life on kill to player
                break;
                
            case UpgradeData.UpgradeType.CoinMagnetRange:
                // Apply coin magnet range
                break;
                
            case UpgradeData.UpgradeType.Shield:
                // Apply shield upgrade to player shield
                PlayerShield playerShield = player.GetComponent<PlayerShield>();
                if (playerShield != null)
                {
                    playerShield.SetMaxShield(playerShield.maxShield + (int)upgrade.value);
                    playerShield.RegenerateShield((int)upgrade.value);
                }
                else
                {
                    Debug.LogError("PlayerShield component not found! Cannot apply shield upgrade.");
                }
                break;
                
            case UpgradeData.UpgradeType.Armor:
                // Apply armor upgrade to player armor
                PlayerArmor playerArmor = player.GetComponent<PlayerArmor>();
                if (playerArmor != null)
                {
                    playerArmor.SetMaxArmor(playerArmor.GetMaxArmor() + (int)upgrade.value);
                    playerArmor.AddArmor((int)upgrade.value);
                }
                else
                {
                    Debug.LogError("PlayerArmor component not found! Cannot apply armor upgrade.");
                }
                break;
                
            case UpgradeData.UpgradeType.ChestDropRate:
                // Apply chest drop rate increase
                EnemyDropManager dropManager = EnemyDropManager.Instance;
                if (dropManager != null)
                {
                    float currentRate = dropManager.GetGlobalDropRate();
                    float newRate = currentRate + upgrade.value;
                    dropManager.SetGlobalDropRate(newRate);
                }
                break;
                
            case UpgradeData.UpgradeType.ExplosionOnKill:
                // Apply explosion on kill
                break;
                
            case UpgradeData.UpgradeType.MoreOptions:
                // Apply more upgrade options
                upgradeOptionsBonus += (int)upgrade.value;
                upgradeOptionsBonus = Mathf.Min(upgradeOptionsBonus, maxUpgradesToShow - baseUpgradesToShow);
                break;
                
            default:
                Debug.Log($"Upgrade type {upgrade.upgradeType} not implemented yet.");
                break;
        }
    }

    /// <summary>
    /// Pauses the game, shows the menu, and stops time.
    /// </summary>
    public void PauseGame()
    {
        isPaused = true;
        // Set the game's time scale to 0, which freezes all physics-based movement and animations.
        Time.timeScale = 0f;
        // Activate the pause menu UI.
        _activeUpgradeInstance.SetActive(true);

        // Unlock the cursor and make it visible so we can click buttons.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /// <summary>
    /// Resumes the game, hides the menu, and restores time.
    /// This is public so our "Continue" button can call it.
    /// </summary>
    public void ResumeGame()
    {
        isPaused = false;
        // Set the time scale back to 1 to resume normal game speed.
        Time.timeScale = 1f;
        // Deactivate the pause menu UI.
        _activeUpgradeInstance.SetActive(false);

        // Re-lock the cursor and hide it for FPS controls.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnUpgradeButtonPressed(UpgradeChest chest = null)
    {
        // Store reference to the chest that opened this menu
        _currentChest = chest;
        
        // Only create a new settings panel if one isn't already active.
        if (_activeUpgradeInstance == null)
        {
            _activeUpgradeInstance = Instantiate(upgradePanelPrefab, canvas.transform);
            _buttonContainerInstance = _activeUpgradeInstance.transform.Find("UpgradeButtonContainer").gameObject;
        }
        
        // Always show new upgrade choices when any chest is opened
        ShowUpgradeChoices();
    }
    
    // Getter for current upgrade options bonus
    public int GetUpgradeOptionsBonus()
    {
        return upgradeOptionsBonus;
    }
    
    // Method to reset upgrade options bonus (for testing or game reset)
    public void ResetUpgradeOptionsBonus()
    {
        upgradeOptionsBonus = 0;
    }
}
