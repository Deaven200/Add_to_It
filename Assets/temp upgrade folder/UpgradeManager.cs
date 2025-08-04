using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpgradeManager : MonoBehaviour
{
    // Singleton instance
    public static UpgradeManager Instance { get; private set; }

    [Header("UI References")]
    public GameObject upgradeUIPanel;        // The full panel (enable/disable)
    public Transform buttonContainer;        // Parent transform where buttons are spawned
    public GameObject upgradeButtonPrefab;   // Prefab for upgrade buttons (card style)

    private List<GameObject> activeButtons = new List<GameObject>();

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject); // Optional
    }

    /// <summary>
    /// Show upgrade panel and create buttons for the available upgrades
    /// </summary>
    public void OpenUpgradeMenu(List<UpgradeData> availableUpgrades)
    {
        Debug.Log($"UpgradeManager UI refs - upgradeUIPanel: {upgradeUIPanel}, buttonContainer: {buttonContainer}, upgradeButtonPrefab: {upgradeButtonPrefab}");

        if (upgradeUIPanel == null || buttonContainer == null || upgradeButtonPrefab == null)
        {
            Debug.LogError("UpgradeManager: UI references are not set!");
            return;
        }

        Time.timeScale = 0f; // Pause the game
        upgradeUIPanel.SetActive(true);

        ClearUpgradeButtons();

        foreach (UpgradeData upgrade in availableUpgrades)
        {
            GameObject buttonObj = Instantiate(upgradeButtonPrefab, buttonContainer);
            buttonObj.SetActive(true);

            UpgradeButton upgradeButton = buttonObj.GetComponent<UpgradeButton>();
            if (upgradeButton != null)
            {
                upgradeButton.SetUpgradeData(upgrade, this);
            }
            else
            {
                Debug.LogError("UpgradeButton component missing on prefab!");
            }

            activeButtons.Add(buttonObj);
        }
    }

    /// <summary>
    /// Remove all existing upgrade buttons
    /// </summary>
    private void ClearUpgradeButtons()
    {
        foreach (GameObject btn in activeButtons)
        {
            Destroy(btn);
        }
        activeButtons.Clear();
    }

    /// <summary>
    /// Called by UpgradeButton when player selects an upgrade
    /// </summary>
    public void OnUpgradeSelected(UpgradeData selectedUpgrade)
    {
        Debug.Log("Upgrade selected: " + selectedUpgrade.upgradeName);

        // TODO: Apply the effect to the player

        upgradeUIPanel.SetActive(false);
        Time.timeScale = 1f; // Resume the game
    }
}
