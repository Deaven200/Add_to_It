using UnityEngine;
using UnityEngine.UI;
using TMPro;  // If you use TextMeshPro for text

public class UpgradeCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI upgradeNameText;
    [SerializeField] private TextMeshProUGUI upgradeDescriptionText;
    [SerializeField] private TextMeshProUGUI rarityText;
    [SerializeField] private Image rarityBackground;
    [SerializeField] private Button upgradeButton;

    private UpgradeData currentUpgrade;
    private UpgradeManager upgradeManager;

    void Start()
    {
        // Get the upgrade manager
        upgradeManager = FindObjectOfType<UpgradeManager>();
        
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
        
        // Set rarity text and color
        if (rarityText != null)
        {
            rarityText.text = upgradeData.rarity.ToString();
            rarityText.color = GetRarityColor(upgradeData.rarity);
        }
        
        // Set rarity background color
        if (rarityBackground != null)
        {
            rarityBackground.color = GetRarityBackgroundColor(upgradeData.rarity);
        }
    }
    
    private Color GetRarityColor(UpgradeData.Rarity rarity)
    {
        switch (rarity)
        {
            case UpgradeData.Rarity.Trashy: return Color.gray;
            case UpgradeData.Rarity.Poor: return Color.white;
            case UpgradeData.Rarity.Common: return Color.green;
            case UpgradeData.Rarity.Uncommon: return Color.blue;
            case UpgradeData.Rarity.Rare: return Color.magenta;
            case UpgradeData.Rarity.Epic: return Color.red;
            case UpgradeData.Rarity.Legendary: return Color.yellow;
            case UpgradeData.Rarity.Mythic: return Color.cyan;
            case UpgradeData.Rarity.Exotic: return new Color(1f, 0.5f, 0f); // Orange
            default: return Color.white;
        }
    }
    
    private Color GetRarityBackgroundColor(UpgradeData.Rarity rarity)
    {
        Color baseColor = GetRarityColor(rarity);
        return new Color(baseColor.r, baseColor.g, baseColor.b, 0.2f); // Semi-transparent
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
