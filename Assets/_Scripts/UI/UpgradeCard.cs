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
        SetupButton();
    }
    
    void SetupButton()
    {
        // Clear any existing listeners first
        if (upgradeButton != null)
        {
            upgradeButton.onClick.RemoveAllListeners();
            upgradeButton.onClick.AddListener(OnUpgradeSelected);
        }
        else
        {
            // Try to get the button component if not assigned
            upgradeButton = GetComponent<Button>();
            if (upgradeButton != null)
            {
                upgradeButton.onClick.RemoveAllListeners();
                upgradeButton.onClick.AddListener(OnUpgradeSelected);
            }
            else
            {
                // Try to find a button in children
                upgradeButton = GetComponentInChildren<Button>();
                if (upgradeButton != null)
                {
                    upgradeButton.onClick.RemoveAllListeners();
                    upgradeButton.onClick.AddListener(OnUpgradeSelected);
                }
            }
        }
        
        // Debug log to help troubleshoot
        if (upgradeButton == null)
        {
            Debug.LogError("UpgradeCard: No Button component found! Make sure the upgrade card prefab has a Button component.");
        }
        else
        {
            // Debug.Log("UpgradeCard: Button click event set up successfully."); // Commented out to reduce console spam
        }
    }

    public void SetUpgradeData(UpgradeData upgradeData)
    {
        currentUpgrade = upgradeData;

        // Ensure we have the upgrade manager reference
        if (upgradeManager == null)
        {
            upgradeManager = FindObjectOfType<UpgradeManager>();
        }

        // Set up the button again to ensure it's properly connected
        SetupButton();

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
        // Debug.Log("UpgradeCard: Button clicked! Attempting to select upgrade..."); // Commented out to reduce console spam
        
        // Only proceed if the game is actually paused (user should be able to click)
        if (Time.timeScale > 0f)
        {
            // Debug.LogWarning("UpgradeCard: Game is not paused! Ignoring button click."); // Commented out to reduce console spam
            return;
        }
        
        if (currentUpgrade != null && upgradeManager != null)
        {
            // Debug.Log($"UpgradeCard: Selecting upgrade: {currentUpgrade.upgradeName}"); // Commented out to reduce console spam
            upgradeManager.SelectUpgrade(currentUpgrade);
        }
        else
        {
            Debug.LogError($"UpgradeCard: Cannot select upgrade! currentUpgrade: {(currentUpgrade != null ? "not null" : "null")}, upgradeManager: {(upgradeManager != null ? "not null" : "null")}");
        }
    }
    
    // Alternative method to test if the button is working
    public void TestButtonClick()
    {
        Debug.Log("UpgradeCard: Test button click called!");
        OnUpgradeSelected();
    }
}
