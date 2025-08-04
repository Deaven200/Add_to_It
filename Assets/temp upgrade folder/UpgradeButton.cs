using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeButton : MonoBehaviour
{
    private UpgradeData upgradeData;
    private UpgradeManager upgradeManager;

    [SerializeField] private Button buttonComponent;
    [SerializeField] private TextMeshProUGUI upgradeNameText;
    [SerializeField] private TextMeshProUGUI upgradeDescriptionText;

    public void SetUpgradeData(UpgradeData data, UpgradeManager manager)
    {
        upgradeData = data;
        upgradeManager = manager;

        if (upgradeNameText != null) upgradeNameText.text = data.upgradeName;
        if (upgradeDescriptionText != null) upgradeDescriptionText.text = data.description;

        if (buttonComponent != null)
        {
            buttonComponent.onClick.RemoveAllListeners();
            buttonComponent.onClick.AddListener(OnButtonClicked);
        }
    }

    private void OnButtonClicked()
    {
        if (upgradeManager != null && upgradeData != null)
        {
            upgradeManager.OnUpgradeSelected(upgradeData);
        }
    }
}
