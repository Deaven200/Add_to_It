using System.Collections.Generic;
using UnityEngine;

public class RandomUpgradeGenerator : MonoBehaviour
{
    [System.Serializable]
    public class RarityWeights
    {
        public UpgradeData.Rarity rarity;
        [Range(0f, 100f)]
        public float weight = 10f;
    }

    [Header("Rarity Settings")]
    [SerializeField] private List<RarityWeights> rarityWeights = new List<RarityWeights>();
    
    [Header("Upgrade Type Settings")]
    [SerializeField] private List<UpgradeTypeConfig> upgradeTypeConfigs = new List<UpgradeTypeConfig>();

    [System.Serializable]
    public class UpgradeTypeConfig
    {
        public UpgradeData.UpgradeType upgradeType;
        public string baseName;
        public string descriptionTemplate;
        public float minValue = 1f;
        public float maxValue = 10f;
        public bool isPercentage = false;
    }

    private void Awake()
    {
        Debug.Log("RandomUpgradeGenerator: Awake called");
        
        // Force random seed to be different each time
        Random.InitState(System.DateTime.Now.Millisecond + System.Environment.TickCount);
        
        // Initialize the lists
        InitializeDefaultRarityWeights();
        InitializeDefaultUpgradeTypeConfigs();
        
        // Verify initialization
        Debug.Log($"RandomUpgradeGenerator: Initialized with {upgradeTypeConfigs.Count} upgrade types and {rarityWeights.Count} rarity weights");
    }

    private void InitializeDefaultRarityWeights()
    {
        if (rarityWeights.Count == 0)
        {
            rarityWeights = new List<RarityWeights>
            {
                new RarityWeights { rarity = UpgradeData.Rarity.Trashy, weight = 25f },
                new RarityWeights { rarity = UpgradeData.Rarity.Poor, weight = 20f },
                new RarityWeights { rarity = UpgradeData.Rarity.Common, weight = 18f },
                new RarityWeights { rarity = UpgradeData.Rarity.Uncommon, weight = 15f },
                new RarityWeights { rarity = UpgradeData.Rarity.Rare, weight = 10f },
                new RarityWeights { rarity = UpgradeData.Rarity.Epic, weight = 6f },
                new RarityWeights { rarity = UpgradeData.Rarity.Legendary, weight = 4f },
                new RarityWeights { rarity = UpgradeData.Rarity.Mythic, weight = 1.5f },
                new RarityWeights { rarity = UpgradeData.Rarity.Exotic, weight = 0.5f }
            };
        }
    }

    private void InitializeDefaultUpgradeTypeConfigs()
    {
        Debug.Log("RandomUpgradeGenerator: Initializing default upgrade type configs");
        
        if (upgradeTypeConfigs == null)
        {
            upgradeTypeConfigs = new List<UpgradeTypeConfig>();
        }
        
        if (upgradeTypeConfigs.Count == 0)
        {
            upgradeTypeConfigs = new List<UpgradeTypeConfig>
            {
                new UpgradeTypeConfig 
                { 
                    upgradeType = UpgradeData.UpgradeType.Damage, 
                    baseName = "Damage Boost",
                    descriptionTemplate = "Increases weapon damage by {0}",
                    minValue = 1f,
                    maxValue = 9f
                },
                new UpgradeTypeConfig 
                { 
                    upgradeType = UpgradeData.UpgradeType.Health, 
                    baseName = "Health Boost",
                    descriptionTemplate = "Increases maximum health by {0}",
                    minValue = 5f,
                    maxValue = 45f
                },
                new UpgradeTypeConfig 
                { 
                    upgradeType = UpgradeData.UpgradeType.FireRate, 
                    baseName = "Fire Rate",
                    descriptionTemplate = "Increases fire rate by {0}%",
                    minValue = 5f,
                    maxValue = 45f,
                    isPercentage = true
                },
                new UpgradeTypeConfig 
                { 
                    upgradeType = UpgradeData.UpgradeType.MoveSpeed, 
                    baseName = "Speed Boost",
                    descriptionTemplate = "Increases movement speed by {0}%",
                    minValue = 5f,
                    maxValue = 45f,
                    isPercentage = true
                },
                new UpgradeTypeConfig 
                { 
                    upgradeType = UpgradeData.UpgradeType.DamageResistance, 
                    baseName = "Armor",
                    descriptionTemplate = "Reduces damage taken by {0}%",
                    minValue = 5f,
                    maxValue = 45f,
                    isPercentage = true
                },
                new UpgradeTypeConfig 
                { 
                    upgradeType = UpgradeData.UpgradeType.LifeOnKill, 
                    baseName = "Life Steal",
                    descriptionTemplate = "Heals {0} health on kill",
                    minValue = 1f,
                    maxValue = 9f
                },
                new UpgradeTypeConfig 
                { 
                    upgradeType = UpgradeData.UpgradeType.CoinMagnetRange, 
                    baseName = "Coin Magnet",
                    descriptionTemplate = "Increases coin pickup range by {0}%",
                    minValue = 10f,
                    maxValue = 90f,
                    isPercentage = true
                },
                new UpgradeTypeConfig 
                { 
                    upgradeType = UpgradeData.UpgradeType.Shield, 
                    baseName = "Shield",
                    descriptionTemplate = "Grants {0} shield points",
                    minValue = 10f,
                    maxValue = 90f
                },
                new UpgradeTypeConfig 
                { 
                    upgradeType = UpgradeData.UpgradeType.ChestDropRate, 
                    baseName = "Lucky Find",
                    descriptionTemplate = "Increases chest drop rate by {0}%",
                    minValue = 5f,
                    maxValue = 45f,
                    isPercentage = true
                },
                new UpgradeTypeConfig 
                { 
                    upgradeType = UpgradeData.UpgradeType.ExplosionOnKill, 
                    baseName = "Explosive Kill",
                    descriptionTemplate = "Creates explosion on kill with {0} radius",
                    minValue = 1f,
                    maxValue = 9f
                },
                new UpgradeTypeConfig 
                { 
                    upgradeType = UpgradeData.UpgradeType.MoreOptions, 
                    baseName = "Choice Expansion",
                    descriptionTemplate = "Increases upgrade choices by {0}",
                    minValue = 1f,
                    maxValue = 3f
                }
            };
            
            Debug.Log($"RandomUpgradeGenerator: Added {upgradeTypeConfigs.Count} upgrade type configs");
        }
        else
        {
            Debug.Log($"RandomUpgradeGenerator: Already has {upgradeTypeConfigs.Count} upgrade type configs");
        }
    }

    public UpgradeData GenerateRandomUpgrade()
    {
        // Check if upgradeTypeConfigs is initialized
        if (upgradeTypeConfigs == null || upgradeTypeConfigs.Count == 0)
        {
            Debug.LogError("UpgradeTypeConfigs is empty! Initializing defaults...");
            InitializeDefaultUpgradeTypeConfigs();
            
            // Check again after initialization
            if (upgradeTypeConfigs == null || upgradeTypeConfigs.Count == 0)
            {
                Debug.LogError("Failed to initialize upgradeTypeConfigs!");
                return CreateDefaultUpgrade();
            }
        }
        
        // Create a new UpgradeData instance
        UpgradeData upgrade = ScriptableObject.CreateInstance<UpgradeData>();
        
        // Select random rarity
        UpgradeData.Rarity selectedRarity = SelectRandomRarity();
        upgrade.rarity = selectedRarity;
        
        // Select random upgrade type (with safety check)
        int randomIndex = Random.Range(0, upgradeTypeConfigs.Count);
        UpgradeTypeConfig selectedConfig = upgradeTypeConfigs[randomIndex];
        upgrade.upgradeType = selectedConfig.upgradeType;
        
        // Calculate value based on rarity
        float rarityMultiplier = GetRarityMultiplier(selectedRarity);
        float baseValue = Random.Range(selectedConfig.minValue, selectedConfig.maxValue);
        upgrade.value = Mathf.Round(baseValue * rarityMultiplier);
        
        // Generate unique name and description
        upgrade.upgradeName = GenerateUpgradeName(selectedConfig.baseName, selectedRarity);
        upgrade.description = GenerateUpgradeDescription(selectedConfig, upgrade.value);
        
        return upgrade;
    }

    public List<UpgradeData> GenerateMultipleUpgrades(int count)
    {
        List<UpgradeData> upgrades = new List<UpgradeData>();
        
        // Ensure we don't generate more upgrades than we have types
        int maxUniqueUpgrades = Mathf.Min(count, upgradeTypeConfigs.Count);
        
        if (count <= maxUniqueUpgrades)
        {
            // Generate random upgrades (may have duplicates)
            for (int i = 0; i < count; i++)
            {
                upgrades.Add(GenerateRandomUpgrade());
            }
        }
        else
        {
            // If we need more upgrades than types, generate unique ones first, then random
            List<UpgradeTypeConfig> availableTypes = new List<UpgradeTypeConfig>(upgradeTypeConfigs);
            
            // Generate one of each type first
            for (int i = 0; i < maxUniqueUpgrades; i++)
            {
                int randomIndex = Random.Range(0, availableTypes.Count);
                UpgradeTypeConfig selectedConfig = availableTypes[randomIndex];
                availableTypes.RemoveAt(randomIndex);
                
                UpgradeData upgrade = CreateUpgradeFromConfig(selectedConfig);
                upgrades.Add(upgrade);
            }
            
            // Fill remaining slots with random upgrades
            for (int i = maxUniqueUpgrades; i < count; i++)
            {
                upgrades.Add(GenerateRandomUpgrade());
            }
        }
        
        return upgrades;
    }

    public List<UpgradeData> GenerateDifferentUpgrades(int count)
    {
        List<UpgradeData> upgrades = new List<UpgradeData>();
        List<UpgradeTypeConfig> availableTypes = new List<UpgradeTypeConfig>(upgradeTypeConfigs);
        
        // Force a new random seed for this generation
        ForceRandomSeed();
        
        // Shuffle the available types using Fisher-Yates algorithm
        for (int i = availableTypes.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            var temp = availableTypes[i];
            availableTypes[i] = availableTypes[randomIndex];
            availableTypes[randomIndex] = temp;
        }
        
        // Take the first 'count' types and generate random upgrades for each
        for (int i = 0; i < Mathf.Min(count, availableTypes.Count); i++)
        {
            // Force another random seed for each individual upgrade
            ForceRandomSeed();
            UpgradeData upgrade = CreateUpgradeFromConfig(availableTypes[i]);
            upgrades.Add(upgrade);
        }
        
        return upgrades;
    }

    public List<UpgradeData> GenerateUniqueUpgrades(int count)
    {
        List<UpgradeData> upgrades = new List<UpgradeData>();
        
        // Create a list of all possible upgrade types
        List<UpgradeData.UpgradeType> allTypes = new List<UpgradeData.UpgradeType>
        {
            UpgradeData.UpgradeType.Damage,
            UpgradeData.UpgradeType.Health,
            UpgradeData.UpgradeType.FireRate,
            UpgradeData.UpgradeType.MoveSpeed,
            UpgradeData.UpgradeType.DamageResistance,
            UpgradeData.UpgradeType.LifeOnKill,
            UpgradeData.UpgradeType.CoinMagnetRange,
            UpgradeData.UpgradeType.Shield,
            UpgradeData.UpgradeType.ChestDropRate,
            UpgradeData.UpgradeType.ExplosionOnKill,
            UpgradeData.UpgradeType.MoreOptions
        };
        
        // Shuffle the types
        for (int i = allTypes.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            var temp = allTypes[i];
            allTypes[i] = allTypes[randomIndex];
            allTypes[randomIndex] = temp;
        }
        
        // Take the first 'count' types and generate random upgrades
        for (int i = 0; i < Mathf.Min(count, allTypes.Count); i++)
        {
            UpgradeData upgrade = GenerateUpgradeOfType(allTypes[i]);
            upgrades.Add(upgrade);
        }
        
        return upgrades;
    }

    private UpgradeData CreateUpgradeFromConfig(UpgradeTypeConfig config)
    {
        UpgradeData upgrade = ScriptableObject.CreateInstance<UpgradeData>();
        
        // Select random rarity
        UpgradeData.Rarity selectedRarity = SelectRandomRarity();
        upgrade.rarity = selectedRarity;
        upgrade.upgradeType = config.upgradeType;
        
        // Calculate value based on rarity
        float rarityMultiplier = GetRarityMultiplier(selectedRarity);
        float baseValue = Random.Range(config.minValue, config.maxValue);
        upgrade.value = Mathf.Round(baseValue * rarityMultiplier);
        
        // Generate name and description
        upgrade.upgradeName = GenerateUpgradeName(config.baseName, selectedRarity);
        upgrade.description = GenerateUpgradeDescription(config, upgrade.value);
        
        return upgrade;
    }

    private UpgradeData GenerateUpgradeOfType(UpgradeData.UpgradeType type)
    {
        UpgradeData upgrade = ScriptableObject.CreateInstance<UpgradeData>();
        
        // Select random rarity
        UpgradeData.Rarity selectedRarity = SelectRandomRarity();
        upgrade.rarity = selectedRarity;
        upgrade.upgradeType = type;
        
        // Find the config for this type
        UpgradeTypeConfig config = upgradeTypeConfigs.Find(c => c.upgradeType == type);
        if (config != null)
        {
            // Calculate value based on rarity
            float rarityMultiplier = GetRarityMultiplier(selectedRarity);
            float baseValue = Random.Range(config.minValue, config.maxValue);
            upgrade.value = Mathf.Round(baseValue * rarityMultiplier);
            
            // Generate name and description
            upgrade.upgradeName = GenerateUpgradeName(config.baseName, selectedRarity);
            upgrade.description = GenerateUpgradeDescription(config, upgrade.value);
        }
        
        return upgrade;
    }

    private UpgradeData.Rarity SelectRandomRarity()
    {
        float totalWeight = 0f;
        foreach (var rarityWeight in rarityWeights)
        {
            totalWeight += rarityWeight.weight;
        }
        
        float randomValue = Random.Range(0f, totalWeight);
        float currentWeight = 0f;
        
        foreach (var rarityWeight in rarityWeights)
        {
            currentWeight += rarityWeight.weight;
            if (randomValue <= currentWeight)
            {
                return rarityWeight.rarity;
            }
        }
        
        return UpgradeData.Rarity.Common; // Fallback
    }

    private float GetRarityMultiplier(UpgradeData.Rarity rarity)
    {
        switch (rarity)
        {
            case UpgradeData.Rarity.Trashy: return 0.5f;
            case UpgradeData.Rarity.Poor: return 0.75f;
            case UpgradeData.Rarity.Common: return 1f;
            case UpgradeData.Rarity.Uncommon: return 1.5f;
            case UpgradeData.Rarity.Rare: return 2f;
            case UpgradeData.Rarity.Epic: return 3f;
            case UpgradeData.Rarity.Legendary: return 4f;
            case UpgradeData.Rarity.Mythic: return 6f;
            case UpgradeData.Rarity.Exotic: return 9f;
            default: return 1f;
        }
    }

    private string GenerateUpgradeName(string baseName, UpgradeData.Rarity rarity)
    {
        string rarityPrefix = GetRarityPrefix(rarity);
        return $"{rarityPrefix} {baseName}";
    }

    private string GetRarityPrefix(UpgradeData.Rarity rarity)
    {
        switch (rarity)
        {
            case UpgradeData.Rarity.Trashy: return "Broken";
            case UpgradeData.Rarity.Poor: return "Worn";
            case UpgradeData.Rarity.Common: return "Standard";
            case UpgradeData.Rarity.Uncommon: return "Enhanced";
            case UpgradeData.Rarity.Rare: return "Superior";
            case UpgradeData.Rarity.Epic: return "Mythical";
            case UpgradeData.Rarity.Legendary: return "Legendary";
            case UpgradeData.Rarity.Mythic: return "Divine";
            case UpgradeData.Rarity.Exotic: return "Cosmic";
            default: return "Standard";
        }
    }

    private string GenerateUpgradeDescription(UpgradeTypeConfig config, float value)
    {
        string valueText = config.isPercentage ? $"{value}%" : value.ToString();
        return string.Format(config.descriptionTemplate, valueText);
    }
    
    // Debug method to test upgrade generation
    [ContextMenu("Test Upgrade Generation")]
    public void TestUpgradeGeneration()
    {
        Debug.Log("=== Testing Upgrade Generation ===");
        
        // Force new random seed
        ForceRandomSeed();
        
        List<UpgradeData> testUpgrades = GenerateMultipleUpgrades(3);
        
        for (int i = 0; i < testUpgrades.Count; i++)
        {
            UpgradeData upgrade = testUpgrades[i];
            Debug.Log($"Upgrade {i + 1}: {upgrade.upgradeName} ({upgrade.rarity}) - {upgrade.description} - Value: {upgrade.value}");
        }
        
        // Test multiple generations
        Debug.Log("=== Testing Multiple Generations ===");
        for (int gen = 0; gen < 3; gen++)
        {
            ForceRandomSeed();
            List<UpgradeData> genUpgrades = GenerateMultipleUpgrades(2);
            Debug.Log($"Generation {gen + 1}:");
            for (int i = 0; i < genUpgrades.Count; i++)
            {
                Debug.Log($"  {genUpgrades[i].upgradeName} ({genUpgrades[i].rarity})");
            }
        }
    }

    public void ForceRandomSeed()
    {
        int seed = System.DateTime.Now.Millisecond + 
               System.Environment.TickCount + 
               Random.Range(0, 10000) + 
               (int)(Time.realtimeSinceStartup * 1000);
        Random.InitState(seed);
    }

    public void SetRandomSeedFromTime()
    {
        int seed = (int)(System.DateTime.Now.Ticks % int.MaxValue);
        Random.InitState(seed);
        Debug.Log($"Set random seed to: {seed}");
    }

    private UpgradeData CreateDefaultUpgrade()
    {
        UpgradeData upgrade = ScriptableObject.CreateInstance<UpgradeData>();
        upgrade.rarity = UpgradeData.Rarity.Common;
        upgrade.upgradeType = UpgradeData.UpgradeType.Health;
        upgrade.value = 10f;
        upgrade.upgradeName = "Default Health Boost";
        upgrade.description = "Increases maximum health by 10";
        return upgrade;
    }

    [ContextMenu("Force Initialize")]
    public void ForceInitialize()
    {
        Debug.Log("RandomUpgradeGenerator: Force initializing...");
        InitializeDefaultRarityWeights();
        InitializeDefaultUpgradeTypeConfigs();
        Debug.Log($"RandomUpgradeGenerator: Force initialized with {upgradeTypeConfigs.Count} upgrade types");
    }
}
