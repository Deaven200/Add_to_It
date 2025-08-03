using System.Collections.Generic;
using UnityEngine;

public class UpgradeUITester : MonoBehaviour
{
    public UpgradeSelectionUI upgradeUI;
    public List<UpgradeData> testUpgrades;

    void Start()
    {
        if (upgradeUI != null)
        {
            upgradeUI.ShowUpgradeChoices(testUpgrades);
        }
    }
}
