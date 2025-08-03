using System.Collections.Generic;
using UnityEngine;

public class UpgradeSelectionUI : MonoBehaviour
{
    public List<UpgradeButton> upgradeButtons;  // Assign your UpgradeButton components here

    [SerializeField] private GameObject upgradePanelPrefab;
    [SerializeField] private GameObject canvas;
    private GameObject _activeUpgradeInstance;
    private bool isPaused = false;
    public void ShowUpgradeChoices(List<UpgradeData> availableUpgrades)
    {

        if (upgradeButtons == null || upgradeButtons.Count == 0 || _activeUpgradeInstance == null)
        {
            Debug.LogError("UpgradeSelectionUI is not set up correctly!");
            return;
        }

        _activeUpgradeInstance.SetActive(true);

        // Display upgrade info on buttons (just an example)
        for (int i = 0; i < upgradeButtons.Count; i++)
        {
            if (i < availableUpgrades.Count)
            {
                upgradeButtons[i].gameObject.SetActive(true);
                upgradeButtons[i].SetUpgradeData(availableUpgrades[i], OnUpgradeSelected);
            }
            else
            {
                upgradeButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private void OnUpgradeSelected(UpgradeData upgrade)
    {
        Debug.Log($"Upgrade selected: {upgrade.upgradeName}");
        // Handle upgrade selection logic here
        ResumeGame();
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

    public void OnUpgradeButtonPressed()
    {
        Debug.Log(_activeUpgradeInstance == null ? "Instantiate new UpgradePanel" : "Open active UpgradePanel");
        // Only create a new settings panel if one isn't already active.
        if (_activeUpgradeInstance == null)
        {
            _activeUpgradeInstance = Instantiate(upgradePanelPrefab, canvas.transform);
        }
    }
}
