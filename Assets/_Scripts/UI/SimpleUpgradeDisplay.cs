using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SimpleUpgradeDisplay : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform upgradeListContainer;
    [SerializeField] private GameObject upgradeItemPrefab;
    [SerializeField] private Text totalUpgradesText;
    [SerializeField] private TextMeshProUGUI totalUpgradesTextTMP;
    
    [Header("Display Settings")]
    [SerializeField] private bool showRarityColors = true;
    
    [Header("Rarity Colors")]
    [SerializeField] private Color commonColor = Color.white;
    [SerializeField] private Color uncommonColor = Color.green;
    [SerializeField] private Color rareColor = Color.blue;
    [SerializeField] private Color epicColor = Color.magenta;
    [SerializeField] private Color legendaryColor = Color.yellow;
    
    private PlayerUpgradeTracker upgradeTracker;
    private List<GameObject> displayedItems = new List<GameObject>();
    
    void Start()
    {
        // Find the upgrade tracker
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            upgradeTracker = player.GetComponent<PlayerUpgradeTracker>();
        }
        
        // Show initially and refresh
        gameObject.SetActive(true);
        
        // Add a small delay to ensure everything is initialized
        StartCoroutine(DelayedRefresh());
    }
    
    private System.Collections.IEnumerator DelayedRefresh()
    {
        yield return new WaitForSeconds(0.1f);
        RefreshDisplay();
    }
    
    public void ShowUpgrades()
    {
        gameObject.SetActive(true);
        RefreshDisplay();
    }
    
    public void HideUpgrades()
    {
        gameObject.SetActive(false);
    }
    
    // Called when pause menu opens
    public void OnPauseMenuOpen()
    {
        RefreshDisplay();
    }
    
    // Called when pause menu closes
    public void OnPauseMenuClose()
    {
        // Keep the display visible but refresh it
        RefreshDisplay();
    }
    
    public void RefreshDisplay()
    {
        if (upgradeTracker == null)
        {
            // Try to find it again
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                upgradeTracker = player.GetComponent<PlayerUpgradeTracker>();
                if (upgradeTracker == null)
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }
        
        // Clear existing items
        ClearDisplayedItems();
        
        // Get all upgrades
        List<UpgradeEntry> upgrades = upgradeTracker.GetAllUpgrades();
        
        // Create display items
        foreach (var upgrade in upgrades)
        {
            CreateUpgradeItem(upgrade);
        }
        
        // Update total text
        if (totalUpgradesText != null)
        {
            totalUpgradesText.text = $"Total Upgrades: {upgradeTracker.GetTotalUpgradeCount()}";
        }
        else if (totalUpgradesTextTMP != null)
        {
            totalUpgradesTextTMP.text = $"Total Upgrades: {upgradeTracker.GetTotalUpgradeCount()}";
        }
    }
    
    private void CreateUpgradeItem(UpgradeEntry upgrade)
    {
        if (upgradeItemPrefab == null || upgradeListContainer == null)
        {
            Debug.LogWarning("Upgrade item prefab or container not assigned!");
            return;
        }
        
        GameObject itemGO = Instantiate(upgradeItemPrefab, upgradeListContainer);
        displayedItems.Add(itemGO);
        
        // Set the upgrade text - try both Text and TextMeshProUGUI
        Text upgradeText = itemGO.GetComponent<Text>();
        TextMeshProUGUI upgradeTextTMP = itemGO.GetComponent<TextMeshProUGUI>();
        
        if (upgradeText == null)
        {
            upgradeText = itemGO.GetComponentInChildren<Text>();
        }
        if (upgradeTextTMP == null)
        {
            upgradeTextTMP = itemGO.GetComponentInChildren<TextMeshProUGUI>();
        }
        
        if (upgradeText != null)
        {
            string displayText = upgrade.GetFullDisplayText();
            upgradeText.text = displayText;
            
            // Apply rarity color if enabled
            if (showRarityColors)
            {
                upgradeText.color = GetRarityColor(upgrade.GetHighestRarity());
            }
        }
        else if (upgradeTextTMP != null)
        {
            string displayText = upgrade.GetFullDisplayText();
            upgradeTextTMP.text = displayText;
            
            // Apply rarity color if enabled
            if (showRarityColors)
            {
                upgradeTextTMP.color = GetRarityColor(upgrade.GetHighestRarity());
            }
        }
    }
    
    private void ClearDisplayedItems()
    {
        foreach (var item in displayedItems)
        {
            if (item != null)
            {
                Destroy(item);
            }
        }
        displayedItems.Clear();
    }
    
    private Color GetRarityColor(UpgradeData.Rarity rarity)
    {
        switch (rarity)
        {
            case UpgradeData.Rarity.Common: return commonColor;
            case UpgradeData.Rarity.Uncommon: return uncommonColor;
            case UpgradeData.Rarity.Rare: return rareColor;
            case UpgradeData.Rarity.Epic: return epicColor;
            case UpgradeData.Rarity.Legendary: return legendaryColor;
            default: return Color.white;
        }
    }
    
    // Context menu for testing
    [ContextMenu("Test Refresh Display")]
    public void TestRefreshDisplay()
    {
        RefreshDisplay();
    }
    
    // Manual test method
    [ContextMenu("Manual Test")]
    public void ManualTest()
    {
        RefreshDisplay();
    }
    
    // Debug container layout
    [ContextMenu("Debug Container")]
    public void DebugContainer()
    {
        Debug.Log("=== CONTAINER DEBUG ===");
        Debug.Log($"Container null: {upgradeListContainer == null}");
        if (upgradeListContainer != null)
        {
            Debug.Log($"Container name: {upgradeListContainer.name}");
            Debug.Log($"Container active: {upgradeListContainer.gameObject.activeInHierarchy}");
            Debug.Log($"Container child count: {upgradeListContainer.childCount}");
            
            // Check for layout components
            var layoutGroup = upgradeListContainer.GetComponent<VerticalLayoutGroup>();
            if (layoutGroup != null)
            {
                Debug.Log($"VerticalLayoutGroup found: spacing={layoutGroup.spacing}, padding={layoutGroup.padding}");
            }
            
            var contentSizeFitter = upgradeListContainer.GetComponent<ContentSizeFitter>();
            if (contentSizeFitter != null)
            {
                Debug.Log($"ContentSizeFitter found: verticalFit={contentSizeFitter.verticalFit}");
            }
            
            // List all children
            for (int i = 0; i < upgradeListContainer.childCount; i++)
            {
                var child = upgradeListContainer.GetChild(i);
                Debug.Log($"Child {i}: {child.name} - Active: {child.gameObject.activeInHierarchy}");
            }
        }
        Debug.Log("=== END CONTAINER DEBUG ===");
    }
}
