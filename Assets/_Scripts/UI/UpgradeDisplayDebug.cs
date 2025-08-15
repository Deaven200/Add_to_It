using UnityEngine;
using UnityEngine.UI;

public class UpgradeDisplayDebug : MonoBehaviour
{
    [Header("Debug Settings")]
    [SerializeField] private KeyCode testKey = KeyCode.T;
    
    private PlayerUpgradeTracker upgradeTracker;
    private SimpleUpgradeDisplay upgradeDisplay;
    
    void Start()
    {
        // Find the upgrade tracker
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            upgradeTracker = player.GetComponent<PlayerUpgradeTracker>();
        }
        
        // Find the upgrade display
        upgradeDisplay = FindObjectOfType<SimpleUpgradeDisplay>();
        if (upgradeDisplay == null)
        {
            // Try to find it by name
            GameObject upgradePanel = GameObject.Find("UpgradeSidePanel");
            if (upgradePanel != null)
            {
                upgradeDisplay = upgradePanel.GetComponent<SimpleUpgradeDisplay>();
            }
        }
    }
    
    void Update()
    {
        if (Input.GetKeyDown(testKey))
        {
            TestUpgradeSystem();
        }
    }
    
    [ContextMenu("Test Upgrade System")]
    public void TestUpgradeSystem()
    {
        // Check PlayerUpgradeTracker
        if (upgradeTracker == null)
        {
            return;
        }
        
        // Check if any upgrades exist
        var upgrades = upgradeTracker.GetAllUpgrades();
        
        if (upgrades.Count == 0)
        {
            AddTestUpgrades();
        }
        
        // Check SimpleUpgradeDisplay - try to find it again
        if (upgradeDisplay == null)
        {
            upgradeDisplay = FindObjectOfType<SimpleUpgradeDisplay>();
            
            if (upgradeDisplay == null)
            {
                // Try to find it by name
                GameObject upgradePanel = GameObject.Find("UpgradeSidePanel");
                if (upgradePanel != null)
                {
                    upgradeDisplay = upgradePanel.GetComponent<SimpleUpgradeDisplay>();
                }
            }
        }
        
        // Test refresh
        if (upgradeDisplay != null)
        {
            upgradeDisplay.RefreshDisplay();
        }
    }
    
    private void AddTestUpgrades()
    {
        if (upgradeTracker == null) return;
        
        // Add some test upgrades
        upgradeTracker.AddUpgrade(UpgradeData.UpgradeType.Health, 5, UpgradeData.Rarity.Common);
        upgradeTracker.AddUpgrade(UpgradeData.UpgradeType.Health, 3, UpgradeData.Rarity.Uncommon);
        upgradeTracker.AddUpgrade(UpgradeData.UpgradeType.MoveSpeed, 2.5f, UpgradeData.Rarity.Rare);
        upgradeTracker.AddUpgrade(UpgradeData.UpgradeType.Damage, 10, UpgradeData.Rarity.Epic);
    }
    
    [ContextMenu("Clear All Upgrades")]
    public void ClearAllUpgrades()
    {
        if (upgradeTracker != null)
        {
            upgradeTracker.ClearAllUpgrades();
        }
    }
    
    [ContextMenu("Print All Upgrades")]
    public void PrintAllUpgrades()
    {
        if (upgradeTracker != null)
        {
            upgradeTracker.PrintAllUpgrades();
        }
    }
}
