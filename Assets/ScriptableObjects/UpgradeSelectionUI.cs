using System.Collections.Generic;
using UnityEngine;

public class UpgradeSelectionUI : MonoBehaviour
{
    public GameObject selectionPanel;           // Assign your panel holding upgrade buttons here
    public List<UpgradeButton> upgradeButtons;  // Assign your UpgradeButton components here

    private void DebugReferences()
    {
        if (selectionPanel == null)
        {
            Debug.LogError("UpgradeSelectionUI: selectionPanel is NOT assigned!");
        }
        else
        {
            Debug.Log("UpgradeSelectionUI: selectionPanel assigned correctly.");
        }

        if (upgradeButtons == null)
        {
            Debug.LogError("UpgradeSelectionUI: upgradeButtons list is NULL!");
        }
        else if (upgradeButtons.Count == 0)
        {
            Debug.LogError("UpgradeSelectionUI: upgradeButtons list is EMPTY!");
        }
        else
        {
            Debug.Log($"UpgradeSelectionUI: upgradeButtons list has {upgradeButtons.Count} elements.");

            for (int i = 0; i < upgradeButtons.Count; i++)
            {
                if (upgradeButtons[i] == null)
                    Debug.LogError($"UpgradeSelectionUI: upgradeButtons[{i}] is NULL!");
                else
                    Debug.Log($"UpgradeSelectionUI: upgradeButtons[{i}] assigned to '{upgradeButtons[i].gameObject.name}'.");
            }
        }
    }

    public void ShowUpgradeChoices(List<UpgradeData> availableUpgrades)
    {
        DebugReferences();

        if (upgradeButtons == null || upgradeButtons.Count == 0 || selectionPanel == null)
        {
            Debug.LogError("UpgradeSelectionUI is not set up correctly!");
            return;
        }

        selectionPanel.SetActive(true);

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
        selectionPanel.SetActive(false);
    }
}
