using System.Collections.Generic;
using UnityEngine;

public class UpgradeSelectionUI : MonoBehaviour
{
    public List<UpgradeData> possibleUpgrades;

    [SerializeField] private GameObject upgradePanelPrefab;
    [SerializeField] private GameObject upgradeCardPrefab;
    [SerializeField] private GameObject canvas;
    
    private GameObject _activeUpgradeInstance;
    private GameObject _buttonContainerInstance;
    private UpgradeChest _currentChest; // Reference to the chest that opened this menu

    public bool isPaused = false;
    
    public void ShowUpgradeChoices()
    {
        _activeUpgradeInstance.SetActive(true);

        // Create a button for each level
        foreach(UpgradeData upgradeData in possibleUpgrades)
        {
            GameObject cardGO = Instantiate(upgradeCardPrefab, _buttonContainerInstance.transform);
            cardGO.GetComponent<UpgradeCard>().SetUpgradeData(upgradeData);
        }
    }

    public void SelectUpgrade(UpgradeData upgrade)
    {
        Debug.Log($"Upgrade selected: {upgrade.upgradeName}");
        
        // Apply the upgrade based on its type
        ApplyUpgrade(upgrade);
        
        // Close the menu and remove the chest
        ResumeGame();
        
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
                // Apply damage upgrade to player shooting
                PlayerShooting playerShooting = player.GetComponent<PlayerShooting>();
                if (playerShooting != null)
                {
                    // You'll need to add a damage field to PlayerShooting
                    Debug.Log($"Applied damage upgrade: +{upgrade.value}");
                }
                break;
                
            case UpgradeData.UpgradeType.MoveSpeed:
                // Apply speed upgrade to player movement
                PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
                if (playerMovement != null)
                {
                    // You'll need to add a speed field to PlayerMovement
                    Debug.Log($"Applied speed upgrade: +{upgrade.value}");
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
        Debug.Log(_activeUpgradeInstance == null ? "Instantiate new UpgradePanel" : "Open active UpgradePanel");
        
        // Store reference to the chest that opened this menu
        _currentChest = chest;
        
        // Only create a new settings panel if one isn't already active.
        if (_activeUpgradeInstance == null)
        {
            _activeUpgradeInstance = Instantiate(upgradePanelPrefab, canvas.transform);
            _buttonContainerInstance = _activeUpgradeInstance.transform.Find("UpgradeButtonContainer").gameObject;
            ShowUpgradeChoices();
        }
    }
}
