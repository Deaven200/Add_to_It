using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeDisplayUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject upgradeListPanel;
    [SerializeField] private Transform upgradeListContainer;
    [SerializeField] private GameObject upgradeItemPrefab;
    [SerializeField] private Text totalUpgradesText;
    [SerializeField] private Text uniqueUpgradesText;
    
    [Header("Display Settings")]
    [SerializeField] private bool showRarityColors = true;
    [SerializeField] private bool sortByRarity = true;
    [SerializeField] private bool showCountBadges = true;
    
    [Header("Rarity Colors")]
    [SerializeField] private Color trashyColor = Color.gray;
    [SerializeField] private Color poorColor = Color.white;
    [SerializeField] private Color commonColor = Color.green;
    [SerializeField] private Color uncommonColor = Color.blue;
    [SerializeField] private Color rareColor = Color.magenta;
    [SerializeField] private Color epicColor = Color.red;
    [SerializeField] private Color legendaryColor = Color.yellow;
    [SerializeField] private Color mythicColor = Color.cyan;
    [SerializeField] private Color exoticColor = Color.magenta;
    
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
        
        // Hide the panel initially
        if (upgradeListPanel != null)
        {
            upgradeListPanel.SetActive(false);
        }
    }
    
    void Update()
    {
        // Toggle upgrade display with 'U' key (for testing)
        if (Input.GetKeyDown(KeyCode.U))
        {
            ToggleUpgradeDisplay();
        }
    }
    
    public void ToggleUpgradeDisplay()
    {
        if (upgradeListPanel != null)
        {
            bool isActive = upgradeListPanel.activeSelf;
            upgradeListPanel.SetActive(!isActive);
            
            if (!isActive)
            {
                RefreshUpgradeDisplay();
            }
        }
    }
    
    public void ShowUpgradeDisplay()
    {
        if (upgradeListPanel != null)
        {
            upgradeListPanel.SetActive(true);
            RefreshUpgradeDisplay();
        }
    }
    
    public void HideUpgradeDisplay()
    {
        if (upgradeListPanel != null)
        {
            upgradeListPanel.SetActive(false);
        }
    }
    
    public void RefreshUpgradeDisplay()
    {
        if (upgradeTracker == null)
        {
            Debug.LogWarning("UpgradeTracker not found!");
            return;
        }
        
        // Clear existing items
        ClearDisplayedItems();
        
        // Get upgrades (sorted if enabled)
        List<UpgradeEntry> upgrades;
        if (sortByRarity)
        {
            upgrades = upgradeTracker.GetUpgradesSortedByRarity();
        }
        else
        {
            upgrades = upgradeTracker.GetUpgradesSortedByType();
        }
        
        // Create display items
        foreach (var upgrade in upgrades)
        {
            CreateUpgradeItem(upgrade);
        }
        
        // Update summary texts
        UpdateSummaryTexts();
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
        
        // Set the upgrade text
        Text upgradeText = itemGO.GetComponentInChildren<Text>();
        if (upgradeText != null)
        {
            upgradeText.text = upgrade.GetFullDisplayText();
            
            // Apply rarity color if enabled
            if (showRarityColors)
            {
                upgradeText.color = GetRarityColor(upgrade.GetHighestRarity());
            }
        }
        
        // Add count badge if enabled and count > 1
        if (showCountBadges && upgrade.count > 1)
        {
            AddCountBadge(itemGO, upgrade.count);
        }
    }
    
    private void AddCountBadge(GameObject itemGO, int count)
    {
        // Create a small badge showing the count
        GameObject badgeGO = new GameObject("CountBadge");
        badgeGO.transform.SetParent(itemGO.transform);
        
        // Position the badge in the top-right corner
        RectTransform badgeRect = badgeGO.AddComponent<RectTransform>();
        badgeRect.anchorMin = new Vector2(1, 1);
        badgeRect.anchorMax = new Vector2(1, 1);
        badgeRect.anchoredPosition = new Vector2(-10, -10);
        badgeRect.sizeDelta = new Vector2(30, 20);
        
        // Add background image
        Image badgeImage = badgeGO.AddComponent<Image>();
        badgeImage.color = Color.red;
        
        // Add text
        GameObject textGO = new GameObject("BadgeText");
        textGO.transform.SetParent(badgeGO.transform);
        RectTransform textRect = textGO.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        Text badgeText = textGO.AddComponent<Text>();
        badgeText.text = count.ToString();
        badgeText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        badgeText.fontSize = 12;
        badgeText.color = Color.white;
        badgeText.alignment = TextAnchor.MiddleCenter;
    }
    
    private void UpdateSummaryTexts()
    {
        if (totalUpgradesText != null)
        {
            totalUpgradesText.text = $"Total Upgrades: {upgradeTracker.GetTotalUpgradeCount()}";
        }
        
        if (uniqueUpgradesText != null)
        {
            uniqueUpgradesText.text = $"Unique Types: {upgradeTracker.GetUniqueUpgradeCount()}";
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
            case UpgradeData.Rarity.Trashy: return trashyColor;
            case UpgradeData.Rarity.Poor: return poorColor;
            case UpgradeData.Rarity.Common: return commonColor;
            case UpgradeData.Rarity.Uncommon: return uncommonColor;
            case UpgradeData.Rarity.Rare: return rareColor;
            case UpgradeData.Rarity.Epic: return epicColor;
            case UpgradeData.Rarity.Legendary: return legendaryColor;
            case UpgradeData.Rarity.Mythic: return mythicColor;
            case UpgradeData.Rarity.Exotic: return exoticColor;
            default: return Color.white;
        }
    }
    
    // Public methods for external control
    public void SetShowRarityColors(bool show)
    {
        showRarityColors = show;
        RefreshUpgradeDisplay();
    }
    
    public void SetSortByRarity(bool sort)
    {
        sortByRarity = sort;
        RefreshUpgradeDisplay();
    }
    
    public void SetShowCountBadges(bool show)
    {
        showCountBadges = show;
        RefreshUpgradeDisplay();
    }
    
    // Context menu for testing
    [ContextMenu("Test Refresh Display")]
    public void TestRefreshDisplay()
    {
        RefreshUpgradeDisplay();
    }
}
