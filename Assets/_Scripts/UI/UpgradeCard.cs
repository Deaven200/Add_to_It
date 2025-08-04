using UnityEngine;
using UnityEngine.UI;
using TMPro;  // If you use TextMeshPro for text

public class UpgradeCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI upgradeNameText;
    [SerializeField] private TextMeshProUGUI upgradeDescriptionText;

    private UpgradeData currentUpgrade;

    public void SetUpgradeData(UpgradeData upgradeData)
    {
        currentUpgrade = upgradeData;

        upgradeNameText.text = upgradeData.upgradeName;
        upgradeDescriptionText.text = upgradeData.description;
    }
}
