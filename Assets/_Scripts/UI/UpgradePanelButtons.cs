using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradePanelButtons : MonoBehaviour
{
    [Header("Button References")]
    [SerializeField] private Button closeButton;
    [SerializeField] private Button rerollButton;
    
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI rerollCostText; // Optional: Text to show the cost
    [SerializeField] private Image rerollButtonImage; // Optional: To change color when can't afford
    
    private UpgradeManager upgradeManager;
    
    void Start()
    {
        // Find the upgrade manager
        upgradeManager = FindObjectOfType<UpgradeManager>();
        
        if (upgradeManager == null)
        {
            Debug.LogError("UpgradePanelButtons: No UpgradeManager found in scene!");
            return;
        }
        
        // Set up button listeners
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(OnCloseButtonClicked);
        }
        else
        {
            Debug.LogError("UpgradePanelButtons: Close button not assigned!");
        }
        
        if (rerollButton != null)
        {
            rerollButton.onClick.AddListener(OnRerollButtonClicked);
        }
        else
        {
            Debug.LogError("UpgradePanelButtons: Reroll button not assigned!");
        }
        
        // Update UI initially
        UpdateRerollUI();
    }
    
    void Update()
    {
        // Update reroll UI every frame to show current cost and affordability
        UpdateRerollUI();
    }
    
    void OnCloseButtonClicked()
    {
        if (upgradeManager != null)
        {
            upgradeManager.CloseChest();
        }
    }
    
    void OnRerollButtonClicked()
    {
        if (upgradeManager != null)
        {
            // Check if player can afford reroll
            if (upgradeManager.CanAffordReroll())
            {
                upgradeManager.RerollUpgrades();
            }
            else
            {
                Debug.Log("Not enough money to reroll!");
                // You could show a UI message here
            }
        }
    }
    
    void UpdateRerollUI()
    {
        if (upgradeManager == null) return;
        
        // Update cost text if assigned
        if (rerollCostText != null)
        {
            int cost = upgradeManager.GetCurrentRerollCost();
            if (cost > 0)
            {
                rerollCostText.text = $"Reroll ({cost})";
            }
            else
            {
                rerollCostText.text = "Reroll (Free)";
            }
        }
        
        // Update button interactability and color based on affordability
        if (rerollButton != null)
        {
            bool canAfford = upgradeManager.CanAffordReroll();
            rerollButton.interactable = canAfford;
            
            // Change button color if image is assigned
            if (rerollButtonImage != null)
            {
                if (canAfford)
                {
                    rerollButtonImage.color = Color.white; // Normal color
                }
                else
                {
                    rerollButtonImage.color = new Color(0.5f, 0.5f, 0.5f, 0.5f); // Grayed out
                }
            }
        }
    }
    
    void OnDestroy()
    {
        // Clean up button listeners
        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(OnCloseButtonClicked);
        }
        
        if (rerollButton != null)
        {
            rerollButton.onClick.RemoveListener(OnRerollButtonClicked);
        }
    }
}
