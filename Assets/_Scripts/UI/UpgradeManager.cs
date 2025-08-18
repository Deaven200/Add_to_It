using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Add this for LayoutRebuilder
using UnityEngine.EventSystems; // Add this for EventSystem

public class UpgradeManager : MonoBehaviour
{
    [Header("Upgrade Generation")]
    [SerializeField] private RandomUpgradeGenerator upgradeGenerator;
    [SerializeField] private int baseUpgradesToShow = 3;
    [SerializeField] private int maxUpgradesToShow = 9;
    
    [Header("Reroll Cost Settings")]
    [SerializeField] private bool enableRerollCost = true;
    [SerializeField] private int baseRerollCost = 50;
    [SerializeField] private bool costIncreasesWithWaves = true;
    [SerializeField] private int costIncreasePerWave = 10;
    [SerializeField] private bool costIncreasesWithTime = true;
    [SerializeField] private int costIncreasePerMinute = 5;
    [SerializeField] private int maxRerollCost = 500; // Cap the maximum cost
    
    [Header("UI References")]
    [SerializeField] private GameObject upgradePanelPrefab;
    [SerializeField] private GameObject upgradeCardPrefab;
    [SerializeField] private GameObject canvas;
    
    private GameObject _activeUpgradeInstance;
    private GameObject _buttonContainerInstance;
    private UpgradeChest _currentChest; // Reference to the chest that opened this menu
    private List<UpgradeData> _currentUpgrades = new List<UpgradeData>();
    
    // Store upgrades for each chest so they remain the same when reopening
    private Dictionary<UpgradeChest, List<UpgradeData>> _chestUpgrades = new Dictionary<UpgradeChest, List<UpgradeData>>();
    
    // Track upgrade options bonus
    private int upgradeOptionsBonus = 0;
    
    // Reroll cost tracking
    private float gameStartTime;
    private int currentWave = 0;

    public bool isPaused = false;
    
    // Track if we're in upgrade menu mode vs regular pause mode
    private bool isInUpgradeMenu = false;
    
    private void Awake()
    {
        // Ensure EventSystem exists (required for UI interactions)
        EnsureEventSystemExists();
        
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
        
        // Initialize game start time for reroll cost calculation
        gameStartTime = Time.time;
    }
    
    void EnsureEventSystemExists()
    {
        // First check if there's already an EventSystem in the scene
        EventSystem existingEventSystem = FindObjectOfType<EventSystem>();
        
        if (existingEventSystem == null)
        {
            // Debug.Log("UpgradeManager: No EventSystem found! Creating one..."); // Commented out to reduce console spam
            GameObject eventSystemGO = new GameObject("EventSystem");
            eventSystemGO.AddComponent<EventSystem>();
            eventSystemGO.AddComponent<StandaloneInputModule>();
            
            // Force the EventSystem to be current
            EventSystem.current = eventSystemGO.GetComponent<EventSystem>();
            
            // Debug.Log("UpgradeManager: EventSystem created successfully!"); // Commented out to reduce console spam
        }
        else
        {
            // Debug.Log($"UpgradeManager: EventSystem found: {existingEventSystem.name}"); // Commented out to reduce console spam
            // Make sure it's set as current
            EventSystem.current = existingEventSystem;
        }
        
        // Double-check that EventSystem.current is not null
        if (EventSystem.current == null)
        {
            Debug.LogError("UpgradeManager: EventSystem.current is still null after creation! This is a critical error.");
        }
        else
        {
            // Debug.Log($"UpgradeManager: EventSystem.current is now: {EventSystem.current.name}"); // Commented out to reduce console spam
        }
    }
    
    void Update()
    {
        // Only handle ESC key if we're in upgrade mode
        // Let UIManager handle regular pause functionality
        if (Input.GetKeyDown(KeyCode.Escape) && IsUpgradeMenuActive())
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
    
    public void ShowUpgradeChoices()
    {
        _activeUpgradeInstance.SetActive(true);

        // Check if this chest already has upgrades stored
        if (_currentChest != null && _chestUpgrades.ContainsKey(_currentChest))
        {
            // Use stored upgrades for this chest
            _currentUpgrades = new List<UpgradeData>(_chestUpgrades[_currentChest]);
        }
        else
        {
            // Generate new upgrades for this chest
            // Calculate how many upgrades to show (base + bonus, capped at max)
            int upgradesToShow = Mathf.Min(baseUpgradesToShow + upgradeOptionsBonus, maxUpgradesToShow);
            
            // Generate random upgrades - the new clean generator handles randomness internally
            if (upgradeGenerator != null)
            {
                _currentUpgrades = upgradeGenerator.GenerateUniqueUpgrades(upgradesToShow);
                
                // Store the upgrades for this chest
                if (_currentChest != null)
                {
                    _chestUpgrades[_currentChest] = new List<UpgradeData>(_currentUpgrades);
                }
            }
            else
            {
                Debug.LogError("[UPGRADE MANAGER] No upgrade generator found!");
                _currentUpgrades = new List<UpgradeData>();
            }
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
            UpgradeCard card = cardGO.GetComponent<UpgradeCard>();
            if (card != null)
            {
                card.SetUpgradeData(upgradeData);
                // Debug.Log($"UpgradeManager: Created upgrade card for: {upgradeData.upgradeName}"); // Commented out to reduce console spam
                
                // Additional debugging for the button
                Button button = cardGO.GetComponent<Button>();
                if (button != null)
                {
                    // Debug.Log($"UpgradeManager: Button found on card {upgradeData.upgradeName} - Interactable: {button.interactable}"); // Commented out to reduce console spam
                    
                    // REMOVED: Automatic button testing - this was causing upgrades to be selected automatically
                    // StartCoroutine(TestButtonAfterDelay(button, upgradeData.upgradeName));
                }
                else
                {
                    Debug.LogError($"UpgradeManager: No Button component found on card {upgradeData.upgradeName}!");
                }
            }
            else
            {
                Debug.LogError("UpgradeManager: Failed to get UpgradeCard component from instantiated prefab!");
            }
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
        
        // Close the menu and permanently close the chest
        ResumeGame();
        
        // Destroy the upgrade panel to ensure fresh generation next time
        if (_activeUpgradeInstance != null)
        {
            Destroy(_activeUpgradeInstance);
            _activeUpgradeInstance = null;
            _buttonContainerInstance = null;
        }
        
        // Permanently close and destroy the chest that opened this menu
        if (_currentChest != null)
        {
            _currentChest.PermanentlyCloseChest();
            // Remove stored upgrades for this chest since it's permanently closed
            _chestUpgrades.Remove(_currentChest);
            // Destroy the chest GameObject
            Destroy(_currentChest.gameObject);
            _currentChest = null;
        }
    }
    
    // New method to close chest without selecting an upgrade
    public void CloseChest()
    {
        // Close the menu
        ResumeGame();
        
        // Destroy the upgrade panel to ensure fresh generation next time
        if (_activeUpgradeInstance != null)
        {
            Destroy(_activeUpgradeInstance);
            _activeUpgradeInstance = null;
            _buttonContainerInstance = null;
        }
        
        // Close the chest (allows reopening)
        if (_currentChest != null)
        {
            _currentChest.CloseChest();
            _currentChest = null;
        }
    }
    
    // New method to reroll upgrades
    public void RerollUpgrades()
    {
        // Check if reroll cost is enabled
        if (enableRerollCost)
        {
            int currentRerollCost = CalculateRerollCost();
            
            // Find player's money component
            PlayerMoney playerMoney = FindObjectOfType<PlayerMoney>();
            if (playerMoney != null)
            {
                // Check if player has enough money
                if (playerMoney.GetMoney() >= currentRerollCost)
                {
                    // Deduct money and reroll
                    playerMoney.SpendMoney(currentRerollCost);
                    GenerateNewUpgradesForCurrentChest();
                    ShowUpgradeChoices();
                    Debug.Log($"Rerolled upgrades for {currentRerollCost} money. New cost: {CalculateRerollCost()}");
                }
                else
                {
                    Debug.Log($"Not enough money to reroll! Cost: {currentRerollCost}, Available: {playerMoney.GetMoney()}");
                    // You could show a UI message here
                    return;
                }
            }
            else
            {
                Debug.LogWarning("PlayerMoney component not found! Rerolling without cost.");
                GenerateNewUpgradesForCurrentChest();
                ShowUpgradeChoices();
            }
        }
        else
        {
            // Reroll is free
            GenerateNewUpgradesForCurrentChest();
            ShowUpgradeChoices();
        }
    }
    
    // Helper method to generate new upgrades for the current chest
    private void GenerateNewUpgradesForCurrentChest()
    {
        if (_currentChest == null) return;
        
        // Calculate how many upgrades to show (base + bonus, capped at max)
        int upgradesToShow = Mathf.Min(baseUpgradesToShow + upgradeOptionsBonus, maxUpgradesToShow);
        
        // Generate new random upgrades
        if (upgradeGenerator != null)
        {
            List<UpgradeData> newUpgrades = upgradeGenerator.GenerateUniqueUpgrades(upgradesToShow);
            
            // Store the new upgrades for this chest
            _chestUpgrades[_currentChest] = new List<UpgradeData>(newUpgrades);
        }
    }
    
    // Calculate current reroll cost based on settings
    public int CalculateRerollCost()
    {
        if (!enableRerollCost)
            return 0;
            
        int cost = baseRerollCost;
        
        // Add wave-based cost increase
        if (costIncreasesWithWaves)
        {
            cost += currentWave * costIncreasePerWave;
        }
        
        // Add time-based cost increase
        if (costIncreasesWithTime)
        {
            float minutesPassed = (Time.time - gameStartTime) / 60f;
            cost += Mathf.FloorToInt(minutesPassed) * costIncreasePerMinute;
        }
        
        // Cap the maximum cost
        return Mathf.Min(cost, maxRerollCost);
    }
    
    // Method to update current wave (call this when waves change)
    public void SetCurrentWave(int wave)
    {
        currentWave = wave;
    }
    
    // Method to get current reroll cost for UI display
    public int GetCurrentRerollCost()
    {
        return CalculateRerollCost();
    }
    
    // Method to check if player can afford reroll
    public bool CanAffordReroll()
    {
        if (!enableRerollCost)
            return true;
            
        PlayerMoney playerMoney = FindObjectOfType<PlayerMoney>();
        if (playerMoney != null)
        {
            return playerMoney.GetMoney() >= CalculateRerollCost();
        }
        return false;
    }
    
    // Getter methods for testing and debugging
    public int GetCurrentWave()
    {
        return currentWave;
    }
    
    public float GetGameStartTime()
    {
        return gameStartTime;
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
        
        // Track the upgrade
        PlayerUpgradeTracker upgradeTracker = player.GetComponent<PlayerUpgradeTracker>();
        if (upgradeTracker != null)
        {
            upgradeTracker.AddUpgrade(upgrade.upgradeType, upgrade.value, upgrade.rarity);
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
                

                
            case UpgradeData.UpgradeType.LifeOnKill:
                // Apply life on kill to player
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
                
            // Aura System Upgrades
            case UpgradeData.UpgradeType.CoinMagnetAura:
                // Apply coin magnet aura
                AuraSystem auraSystem = player.GetComponent<AuraSystem>();
                if (auraSystem != null)
                {
                    auraSystem.AddAura(upgrade.upgradeType, upgrade.value, upgrade.rarity);
                }
                else
                {
                    Debug.LogError("AuraSystem component not found! Cannot apply aura upgrade.");
                }
                break;
                
            case UpgradeData.UpgradeType.SlowAura:
                // Apply slow aura
                AuraSystem slowAuraSystem = player.GetComponent<AuraSystem>();
                if (slowAuraSystem != null)
                {
                    slowAuraSystem.AddAura(upgrade.upgradeType, upgrade.value, upgrade.rarity);
                }
                else
                {
                    Debug.LogError("AuraSystem component not found! Cannot apply aura upgrade.");
                }
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
        // Debug.Log("UpgradeManager: Pausing game..."); // Commented out to reduce console spam
        isPaused = true;
        
        // Set the game's time scale to 0, which freezes all physics-based movement and animations.
        Time.timeScale = 0f;
        
        // If we're in upgrade menu mode, show the upgrade UI
        if (isInUpgradeMenu && _activeUpgradeInstance != null)
        {
            _activeUpgradeInstance.SetActive(true);
        }
        // Otherwise, show a regular pause menu (you can create one or just show a simple message)
        else
        {
            Debug.Log("UpgradeManager: Game paused - Press ESC to resume");
            // You can add a simple pause menu here if needed
        }

        // Unlock the cursor and make it visible so we can click buttons.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        // Force canvas update to ensure UI is properly rendered
        Canvas.ForceUpdateCanvases();
        
        // Ensure the EventSystem is working
        if (UnityEngine.EventSystems.EventSystem.current != null)
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        }
        
        // Debug.Log("UpgradeManager: Game paused successfully. TimeScale: " + Time.timeScale + ", Cursor visible: " + Cursor.visible); // Commented out to reduce console spam
    }

    /// <summary>
    /// Resumes the game, hides the menu, and restores time.
    /// This is public so our "Continue" button can call it.
    /// </summary>
    public void ResumeGame()
    {
        // Debug.Log("UpgradeManager: Resuming game..."); // Commented out to reduce console spam
        isPaused = false;
        isInUpgradeMenu = false;
        
        // Set the time scale back to 1 to resume normal game speed.
        Time.timeScale = 1f;
        
        // Deactivate the pause menu UI.
        if (_activeUpgradeInstance != null)
        {
            _activeUpgradeInstance.SetActive(false);
        }

        // Re-lock the cursor and hide it for FPS controls.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Debug.Log("UpgradeManager: Game resumed successfully. TimeScale: " + Time.timeScale + ", Cursor visible: " + Cursor.visible); // Commented out to reduce console spam
    }
    
    /// <summary>
    /// Public method to check if upgrade menu is currently active
    /// </summary>
    public bool IsUpgradeMenuActive()
    {
        return isPaused && _activeUpgradeInstance != null && _activeUpgradeInstance.activeInHierarchy;
    }

    public void OnUpgradeButtonPressed(UpgradeChest chest = null)
    {
        // Debug.Log("UpgradeManager: OnUpgradeButtonPressed called"); // Commented out to reduce console spam
        
        // Ensure EventSystem exists before creating UI
        EnsureEventSystemExists();
        
        // Store reference to the chest that opened this menu
        _currentChest = chest;
        
        // Only create a new settings panel if one isn't already active.
        if (_activeUpgradeInstance == null)
        {
            _activeUpgradeInstance = Instantiate(upgradePanelPrefab, canvas.transform);
            _buttonContainerInstance = _activeUpgradeInstance.transform.Find("UpgradeButtonContainer").gameObject;
            // Debug.Log("UpgradeManager: Created new upgrade panel instance"); // Commented out to reduce console spam
        }
        
        // Set upgrade menu mode and show choices
        isInUpgradeMenu = true;
        ShowUpgradeChoices();
        
        // Pause the game after showing the upgrade choices
        // Debug.Log("UpgradeManager: About to pause game..."); // Commented out to reduce console spam
        PauseGame();
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
    
    // Test coroutine to check if buttons are working (DISABLED - was causing automatic upgrades)
    /*
    System.Collections.IEnumerator TestButtonAfterDelay(Button button, string upgradeName)
    {
        yield return new WaitForSecondsRealtime(1f); // Wait 1 second in real time (not affected by timeScale)
        
        if (button != null && button.interactable)
        {
            Debug.Log($"UpgradeManager: Testing button for {upgradeName} - attempting programmatic click");
            button.onClick.Invoke();
        }
        else
        {
            Debug.LogError($"UpgradeManager: Button for {upgradeName} is null or not interactable!");
        }
    }
    */
}
