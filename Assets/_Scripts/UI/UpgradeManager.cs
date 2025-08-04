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
            _buttonContainerInstance = _activeUpgradeInstance.transform.Find("UpgradeButtonContainer").gameObject;
            ShowUpgradeChoices();
        }
    }
}
