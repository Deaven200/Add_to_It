using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UpgradeEntry
{
    public UpgradeData.UpgradeType upgradeType;
    public float totalValue;
    public int count;
    public List<UpgradeData.Rarity> rarities = new List<UpgradeData.Rarity>();
    
    public UpgradeEntry(UpgradeData.UpgradeType type, float value, UpgradeData.Rarity rarity)
    {
        upgradeType = type;
        totalValue = value;
        count = 1;
        rarities.Add(rarity);
    }
    
    public void AddUpgrade(float value, UpgradeData.Rarity rarity)
    {
        totalValue += value;
        count++;
        rarities.Add(rarity);
    }
    
    public string GetDisplayName()
    {
        switch (upgradeType)
        {
            case UpgradeData.UpgradeType.Health:
                return "Health";
            case UpgradeData.UpgradeType.MoveSpeed:
                return "Speed";
            case UpgradeData.UpgradeType.Damage:
                return "Damage";
            case UpgradeData.UpgradeType.FireRate:
                return "Fire Rate";
            case UpgradeData.UpgradeType.CoinMagnetAura:
                return "Coin Magnet Aura";
            case UpgradeData.UpgradeType.SlowAura:
                return "Slow Aura";
            case UpgradeData.UpgradeType.ShieldAura:
                return "Shield Aura";
            case UpgradeData.UpgradeType.DamageAura:
                return "Damage Aura";
            case UpgradeData.UpgradeType.HealAura:
                return "Heal Aura";
            default:
                return upgradeType.ToString();
        }
    }
    
    public string GetFormattedValue()
    {
        switch (upgradeType)
        {
            case UpgradeData.UpgradeType.Health:
            case UpgradeData.UpgradeType.Damage:
                return $"+{totalValue:F0}";
            case UpgradeData.UpgradeType.MoveSpeed:
            case UpgradeData.UpgradeType.FireRate:
                return $"+{totalValue:F1}";
            case UpgradeData.UpgradeType.CoinMagnetAura:
            case UpgradeData.UpgradeType.SlowAura:
            case UpgradeData.UpgradeType.ShieldAura:
            case UpgradeData.UpgradeType.DamageAura:
            case UpgradeData.UpgradeType.HealAura:
                return $"Radius: {totalValue:F1}";
            default:
                return $"+{totalValue:F1}";
        }
    }
    
    public string GetFullDisplayText()
    {
        string displayName = GetDisplayName();
        string formattedValue = GetFormattedValue();
        
        return $"{displayName} {formattedValue}";
    }
    
    public UpgradeData.Rarity GetHighestRarity()
    {
        if (rarities.Count == 0) return UpgradeData.Rarity.Common;
        
        UpgradeData.Rarity highest = rarities[0];
        foreach (var rarity in rarities)
        {
            if ((int)rarity > (int)highest)
            {
                highest = rarity;
            }
        }
        return highest;
    }
}

public class PlayerUpgradeTracker : MonoBehaviour
{
    [Header("Upgrade Tracking")]
    [SerializeField] private List<UpgradeEntry> collectedUpgrades = new List<UpgradeEntry>();
    
    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = false;
    
    void Start()
    {
        // Initialize with empty list
        collectedUpgrades.Clear();
    }
    
    // Called when an upgrade is applied
    public void AddUpgrade(UpgradeData.UpgradeType upgradeType, float value, UpgradeData.Rarity rarity)
    {
        // Find existing upgrade of the same type
        UpgradeEntry existingEntry = collectedUpgrades.Find(entry => entry.upgradeType == upgradeType);
        
        if (existingEntry != null)
        {
            // Add to existing entry
            existingEntry.AddUpgrade(value, rarity);
        }
        else
        {
            // Create new entry
            UpgradeEntry newEntry = new UpgradeEntry(upgradeType, value, rarity);
            collectedUpgrades.Add(newEntry);
        }
        
        if (showDebugInfo)
        {
            Debug.Log($"Added upgrade: {upgradeType} +{value} ({rarity})");
        }
    }
    
    // Get all collected upgrades
    public List<UpgradeEntry> GetAllUpgrades()
    {
        return new List<UpgradeEntry>(collectedUpgrades);
    }
    
    // Get specific upgrade entry
    public UpgradeEntry GetUpgradeEntry(UpgradeData.UpgradeType upgradeType)
    {
        return collectedUpgrades.Find(entry => entry.upgradeType == upgradeType);
    }
    
    // Get total value for a specific upgrade type
    public float GetTotalValue(UpgradeData.UpgradeType upgradeType)
    {
        UpgradeEntry entry = GetUpgradeEntry(upgradeType);
        return entry != null ? entry.totalValue : 0f;
    }
    
    // Get count for a specific upgrade type
    public int GetUpgradeCount(UpgradeData.UpgradeType upgradeType)
    {
        UpgradeEntry entry = GetUpgradeEntry(upgradeType);
        return entry != null ? entry.count : 0;
    }
    
    // Get formatted display text for a specific upgrade
    public string GetUpgradeDisplayText(UpgradeData.UpgradeType upgradeType)
    {
        UpgradeEntry entry = GetUpgradeEntry(upgradeType);
        return entry != null ? entry.GetFullDisplayText() : "";
    }
    
    // Get all upgrade display texts for UI
    public List<string> GetAllUpgradeDisplayTexts()
    {
        List<string> displayTexts = new List<string>();
        
        foreach (var entry in collectedUpgrades)
        {
            displayTexts.Add(entry.GetFullDisplayText());
        }
        
        return displayTexts;
    }
    
    // Get upgrades sorted by rarity (highest first)
    public List<UpgradeEntry> GetUpgradesSortedByRarity()
    {
        List<UpgradeEntry> sorted = new List<UpgradeEntry>(collectedUpgrades);
        sorted.Sort((a, b) => b.GetHighestRarity().CompareTo(a.GetHighestRarity()));
        return sorted;
    }
    
    // Get upgrades sorted by type
    public List<UpgradeEntry> GetUpgradesSortedByType()
    {
        List<UpgradeEntry> sorted = new List<UpgradeEntry>(collectedUpgrades);
        sorted.Sort((a, b) => a.upgradeType.CompareTo(b.upgradeType));
        return sorted;
    }
    
    // Clear all upgrades (for testing or new game)
    public void ClearAllUpgrades()
    {
        collectedUpgrades.Clear();
        Debug.Log("All upgrades cleared");
    }
    
    // Get total number of upgrades collected
    public int GetTotalUpgradeCount()
    {
        int total = 0;
        foreach (var entry in collectedUpgrades)
        {
            total += entry.count;
        }
        return total;
    }
    
    // Get number of different upgrade types
    public int GetUniqueUpgradeCount()
    {
        return collectedUpgrades.Count;
    }
    
    // Debug method to print all upgrades
    [ContextMenu("Print All Upgrades")]
    public void PrintAllUpgrades()
    {
        Debug.Log("=== PLAYER UPGRADES ===");
        
        if (collectedUpgrades.Count == 0)
        {
            Debug.Log("No upgrades collected yet.");
            return;
        }
        
        foreach (var entry in collectedUpgrades)
        {
            Debug.Log(entry.GetFullDisplayText());
        }
        
        Debug.Log($"Total upgrades: {GetTotalUpgradeCount()}");
        Debug.Log($"Unique types: {GetUniqueUpgradeCount()}");
        Debug.Log("=======================");
    }
}
