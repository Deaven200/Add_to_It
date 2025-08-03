using UnityEngine;
using UnityEngine.UI;
using TMPro;  // If you use TextMeshPro for text

public class UpgradeButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI upgradeNameText;
    [SerializeField] private TextMeshProUGUI upgradeDescriptionText;

    private UpgradeData currentUpgrade;
    private System.Action<UpgradeData> onClickAction;

    public void SetUpgradeData(UpgradeData upgradeData, System.Action<UpgradeData> onClick)
    {
        currentUpgrade = upgradeData;
        onClickAction = onClick;

        upgradeNameText.text = upgradeData.upgradeName;
        upgradeDescriptionText.text = upgradeData.description;

        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(() => onClickAction.Invoke(currentUpgrade));
    }
}
