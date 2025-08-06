using UnityEngine;
using UnityEngine.UI;
using TMPro;  // If you use TextMeshPro for text

public class UpgradeCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI upgradeNameText;
    [SerializeField] private TextMeshProUGUI upgradeDescriptionText;
    [SerializeField] private Button upgradeButton;

    private UpgradeData currentUpgrade;
    private UpgradeSelectionUI upgradeManager;

    void Start()
    {
        // Get the upgrade manager
        upgradeManager = FindObjectOfType<UpgradeSelectionUI>();
        
        // Set up the button click event
        if (upgradeButton != null)
        {
            upgradeButton.onClick.AddListener(OnUpgradeSelected);
        }
        else
        {
            // Try to get the button component if not assigned
            upgradeButton = GetComponent<Button>();
            if (upgradeButton != null)
            {
                upgradeButton.onClick.AddListener(OnUpgradeSelected);
            }
        }
    }

    public void SetUpgradeData(UpgradeData upgradeData)
    {
        currentUpgrade = upgradeData;

        upgradeNameText.text = upgradeData.upgradeName;
        upgradeDescriptionText.text = upgradeData.description;
    }
    
    private void OnUpgradeSelected()
    {
        if (currentUpgrade != null && upgradeManager != null)
        {
            upgradeManager.SelectUpgrade(currentUpgrade);
        }
        else
        {
            Debug.LogError("Upgrade data or manager is null!");
        }
    }
}
