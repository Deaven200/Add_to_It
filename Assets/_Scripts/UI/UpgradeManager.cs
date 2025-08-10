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
            Debug.Log("[UPGRADE MANAGER] Generating new random upgrades for chest opening");
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
        Debug.Log($"[CHEST DEBUG] Creating {_currentUpgrades.Count} upgrade cards:");
        foreach(UpgradeData upgradeData in _currentUpgrades)
        {
            GameObject cardGO = Instantiate(upgradeCardPrefab, _buttonContainerInstance.transform);
            cardGO.GetComponent<UpgradeCard>().SetUpgradeData(upgradeData);
            
            // Debug log each upgrade being created with more details
            Debug.Log($"[CHEST DEBUG] Card: {upgradeData.upgradeName} ({upgradeData.rarity}) - {upgradeData.description} - Value: {upgradeData.value} - Type: {upgradeData.upgradeType}");
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
        Debug.Log($"[CHEST DEBUG] Upgrade selected: {upgrade.upgradeName} ({upgrade.rarity})");
        
        // Apply the upgrade based on its type
        ApplyUpgrade(upgrade);
        
        // Close the menu and remove the chest
        ResumeGame();
        
        // Destroy the upgrade panel to ensure fresh generation next time
        if (_activeUpgradeInstance != null)
        {
            Debug.Log("[CHEST DEBUG] Destroying upgrade panel for fresh generation next time");
            Destroy(_activeUpgradeInstance);
            _activeUpgradeInstance = null;
            _buttonContainerInstance = null;
        }
        
        // Remove the chest that opened this menu
        if (_currentChest != null)
        {
            Debug.Log($"[CHEST DEBUG] Destroying chest with ID: {_currentChest.GetInstanceID()}");
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
                    WeaponData currentWeapon = weaponManager.GetCurrentWeapon();
                    currentWeapon.damage += upgrade.value;
                    Debug.Log($"Applied damage upgrade: +{upgrade.value} (Total: {currentWeapon.damage})");
                }
                break;
                
            case UpgradeData.UpgradeType.FireRate:
                // Apply fire rate upgrade to weapon manager
                WeaponManager weaponMgr = WeaponManager.Instance;
                if (weaponMgr != null && weaponMgr.GetCurrentWeapon() != null)
                {
                    WeaponData weapon = weaponMgr.GetCurrentWeapon();
                    // Calculate new fire rate (lower = faster shooting)
                    float fireRateReduction = weapon.fireRate * (upgrade.value / 100f);
                    weapon.fireRate = Mathf.Max(0.1f, weapon.fireRate - fireRateReduction);
                    Debug.Log($"Applied fire rate upgrade: +{upgrade.value}% (New rate: {weapon.fireRate}s between shots)");
                }
                break;
                
            case UpgradeData.UpgradeType.MoveSpeed:
                // Apply speed upgrade to player movement
                PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
                if (playerMovement != null)
                {
                    // You'll need to add a speed field to PlayerMovement
                    Debug.Log($"Applied speed upgrade: +{upgrade.value}%");
                }
                break;
                
            case UpgradeData.UpgradeType.DamageResistance:
                // Apply damage resistance to player health
                PlayerHealth health = player.GetComponent<PlayerHealth>();
                if (health != null)
                {
                    // You'll need to add damage resistance to PlayerHealth
                    Debug.Log($"Applied damage resistance: +{upgrade.value}%");
                }
                break;
                
            case UpgradeData.UpgradeType.LifeOnKill:
                // Apply life on kill to player
                Debug.Log($"Applied life on kill: +{upgrade.value} health per kill");
                break;
                
            case UpgradeData.UpgradeType.CoinMagnetRange:
                // Apply coin magnet range
                Debug.Log($"Applied coin magnet range: +{upgrade.value}%");
                break;
                
            case UpgradeData.UpgradeType.Shield:
                // Apply shield to player
                PlayerHealth playerHealthShield = player.GetComponent<PlayerHealth>();
                if (playerHealthShield != null)
                {
                    // You'll need to add shield functionality to PlayerHealth
                    Debug.Log($"Applied shield: +{upgrade.value} shield points");
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
                    Debug.Log($"Applied chest drop rate: +{upgrade.value}% (New rate: {newRate}%)");
                }
                break;
                
            case UpgradeData.UpgradeType.ExplosionOnKill:
                // Apply explosion on kill
                Debug.Log($"Applied explosion on kill: {upgrade.value} radius");
                break;
                
            case UpgradeData.UpgradeType.MoreOptions:
                // Apply more upgrade options
                upgradeOptionsBonus += (int)upgrade.value;
                upgradeOptionsBonus = Mathf.Min(upgradeOptionsBonus, maxUpgradesToShow - baseUpgradesToShow);
                Debug.Log($"Applied more options upgrade: +{upgrade.value} (Total bonus: {upgradeOptionsBonus})");
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
        string chestId = chest != null ? chest.GetInstanceID().ToString() : "null";
        Debug.Log($"[CHEST DEBUG] OnUpgradeButtonPressed called by chest {chestId}");
        Debug.Log(_activeUpgradeInstance == null ? "Instantiate new UpgradePanel" : "Open active UpgradePanel");
        
        // Store reference to the chest that opened this menu
        _currentChest = chest;
        
        // Only create a new settings panel if one isn't already active.
        if (_activeUpgradeInstance == null)
        {
            Debug.Log("[CHEST DEBUG] Creating new upgrade panel instance");
            _activeUpgradeInstance = Instantiate(upgradePanelPrefab, canvas.transform);
            _buttonContainerInstance = _activeUpgradeInstance.transform.Find("UpgradeButtonContainer").gameObject;
        }
        else
        {
            Debug.Log("[CHEST DEBUG] Reusing existing upgrade panel");
        }
        
        // Always show new upgrade choices when any chest is opened
        Debug.Log("[CHEST DEBUG] About to call ShowUpgradeChoices()");
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
        Debug.Log("Upgrade options bonus reset to 0");
    }
}
