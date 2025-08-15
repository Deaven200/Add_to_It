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

    [Header("Rarity Settings")]
    [SerializeField] private List<RarityWeights> rarityWeights = new List<RarityWeights>();
    
    [Header("Upgrade Type Settings")]
    [SerializeField] private List<UpgradeTypeConfig> upgradeTypeConfigs = new List<UpgradeTypeConfig>();

    private System.Random systemRandom;
    private static int generationCounter = 0;
    
    [Header("Progress-Based Rarity")]
    [SerializeField] private bool useProgressBasedRarity = true;
    [SerializeField] private float progressRarityMultiplier = 0.1f; // How much progress affects rarity

    private void Awake()
    {
        InitializeDefaults();
        CreateFreshRandomGenerator();
    }

    private void InitializeDefaults()
    {
        // Initialize rarity weights if empty - Much more dramatic rarity differences
        if (rarityWeights.Count == 0)
        {
            rarityWeights = new List<RarityWeights>
            {
                new RarityWeights { rarity = UpgradeData.Rarity.Trashy, weight = 35f },
                new RarityWeights { rarity = UpgradeData.Rarity.Poor, weight = 25f },
                new RarityWeights { rarity = UpgradeData.Rarity.Common, weight = 20f },
                new RarityWeights { rarity = UpgradeData.Rarity.Uncommon, weight = 12f },
                new RarityWeights { rarity = UpgradeData.Rarity.Rare, weight = 5f },
                new RarityWeights { rarity = UpgradeData.Rarity.Epic, weight = 2f },
                new RarityWeights { rarity = UpgradeData.Rarity.Legendary, weight = 0.8f },
                new RarityWeights { rarity = UpgradeData.Rarity.Mythic, weight = 0.15f },
                new RarityWeights { rarity = UpgradeData.Rarity.Exotic, weight = 0.05f }
            };
        }

        // Initialize upgrade type configs if empty
        if (upgradeTypeConfigs.Count == 0)
        {
            upgradeTypeConfigs = new List<UpgradeTypeConfig>
            {
                new UpgradeTypeConfig { upgradeType = UpgradeData.UpgradeType.Damage, baseName = "Damage Boost", descriptionTemplate = "Increases weapon damage by {0}", minValue = 1f, maxValue = 9f },
                new UpgradeTypeConfig { upgradeType = UpgradeData.UpgradeType.Health, baseName = "Health Boost", descriptionTemplate = "Increases maximum health by {0}", minValue = 1f, maxValue = 9f },
                new UpgradeTypeConfig { upgradeType = UpgradeData.UpgradeType.FireRate, baseName = "Fire Rate", descriptionTemplate = "Increases fire rate by {0}%", minValue = 1f, maxValue = 9f, isPercentage = true },
                new UpgradeTypeConfig { upgradeType = UpgradeData.UpgradeType.MoveSpeed, baseName = "Speed Boost", descriptionTemplate = "Increases movement speed by {0}%", minValue = 1f, maxValue = 9f, isPercentage = true },
                new UpgradeTypeConfig { upgradeType = UpgradeData.UpgradeType.LifeOnKill, baseName = "Life Steal", descriptionTemplate = "Heals {0} health on kill", minValue = 1f, maxValue = 9f },
    
                new UpgradeTypeConfig { upgradeType = UpgradeData.UpgradeType.Shield, baseName = "Shield", descriptionTemplate = "Grants {0} shield points", minValue = 1f, maxValue = 9f },
                new UpgradeTypeConfig { upgradeType = UpgradeData.UpgradeType.Armor, baseName = "Armor", descriptionTemplate = "Grants {0} armor points", minValue = 1f, maxValue = 9f },
                new UpgradeTypeConfig { upgradeType = UpgradeData.UpgradeType.ChestDropRate, baseName = "Lucky Find", descriptionTemplate = "Increases chest drop rate by {0}%", minValue = 1f, maxValue = 9f, isPercentage = true },
                new UpgradeTypeConfig { upgradeType = UpgradeData.UpgradeType.ExplosionOnKill, baseName = "Explosive Kill", descriptionTemplate = "Creates explosion on kill with {0} radius", minValue = 1f, maxValue = 9f },
                new UpgradeTypeConfig { upgradeType = UpgradeData.UpgradeType.MoreOptions, baseName = "Choice Expansion", descriptionTemplate = "Increases upgrade choices by {0}", minValue = 1f, maxValue = 3f },
                // Aura System Upgrades
                new UpgradeTypeConfig { upgradeType = UpgradeData.UpgradeType.CoinMagnetAura, baseName = "Coin Magnet Aura", descriptionTemplate = "Creates a coin magnet aura with {0} radius", minValue = 2f, maxValue = 8f },
                new UpgradeTypeConfig { upgradeType = UpgradeData.UpgradeType.SlowAura, baseName = "Slow Aura", descriptionTemplate = "Creates a slow aura with {0} radius", minValue = 2f, maxValue = 8f },
                new UpgradeTypeConfig { upgradeType = UpgradeData.UpgradeType.ShieldAura, baseName = "Shield Aura", descriptionTemplate = "Creates a protective shield aura with {0} radius", minValue = 2f, maxValue = 8f },
                new UpgradeTypeConfig { upgradeType = UpgradeData.UpgradeType.DamageAura, baseName = "Damage Aura", descriptionTemplate = "Creates a damage aura with {0} radius", minValue = 2f, maxValue = 8f },
                new UpgradeTypeConfig { upgradeType = UpgradeData.UpgradeType.HealAura, baseName = "Heal Aura", descriptionTemplate = "Creates a healing aura with {0} radius", minValue = 2f, maxValue = 8f }
            };
        }
    }

    public void CreateFreshRandomGenerator()
    {
        // Create a completely fresh random generator with multiple entropy sources
        generationCounter++;
        long currentTicks = System.DateTime.Now.Ticks;
        int seed = (int)(currentTicks % int.MaxValue) + 
                   System.Environment.TickCount + 
                   Time.frameCount + 
                   generationCounter * 1000;
        
        systemRandom = new System.Random(seed);
    }

    public List<UpgradeData> GenerateUniqueUpgrades(int count)
    {
        // Always create a fresh generator for each call to ensure randomness
        CreateFreshRandomGenerator();
        
        // Generate unique upgrades
        
        List<UpgradeData> upgrades = new List<UpgradeData>();
        List<UpgradeData.UpgradeType> availableTypes = new List<UpgradeData.UpgradeType>
        {
            UpgradeData.UpgradeType.Damage,
            UpgradeData.UpgradeType.Health,
            UpgradeData.UpgradeType.FireRate,
            UpgradeData.UpgradeType.MoveSpeed,
            UpgradeData.UpgradeType.DamageResistance,
            UpgradeData.UpgradeType.LifeOnKill,

            UpgradeData.UpgradeType.Shield,
            UpgradeData.UpgradeType.Armor,
            UpgradeData.UpgradeType.ChestDropRate,
            UpgradeData.UpgradeType.ExplosionOnKill,
            UpgradeData.UpgradeType.MoreOptions,
            // Aura System Types
            UpgradeData.UpgradeType.CoinMagnetAura,
            UpgradeData.UpgradeType.SlowAura,
            UpgradeData.UpgradeType.ShieldAura,
            UpgradeData.UpgradeType.DamageAura,
            UpgradeData.UpgradeType.HealAura
        };

        // Shuffle the available types
        for (int i = availableTypes.Count - 1; i > 0; i--)
        {
            int randomIndex = systemRandom.Next(0, i + 1);
            var temp = availableTypes[i];
            availableTypes[i] = availableTypes[randomIndex];
            availableTypes[randomIndex] = temp;
        }

        // Generate upgrades for the first 'count' types
        for (int i = 0; i < Mathf.Min(count, availableTypes.Count); i++)
        {
            UpgradeData upgrade = GenerateUpgradeOfType(availableTypes[i]);
            upgrades.Add(upgrade);
            // Upgrade generated successfully
        }

        return upgrades;
    }

    private UpgradeData GenerateUpgradeOfType(UpgradeData.UpgradeType type)
    {
        UpgradeData upgrade = ScriptableObject.CreateInstance<UpgradeData>();
        upgrade.upgradeType = type;
        
        // Find the config for this type
        UpgradeTypeConfig config = upgradeTypeConfigs.Find(c => c.upgradeType == type);
        if (config == null)
        {
            Debug.LogError($"No config found for upgrade type: {type}");
            return CreateFallbackUpgrade();
        }

        // Generate random rarity
        upgrade.rarity = SelectRandomRarity();
        
        // Generate random value within the config range
        float baseValue = (float)(systemRandom.NextDouble() * (config.maxValue - config.minValue) + config.minValue);
        float rarityMultiplier = GetRarityMultiplier(upgrade.rarity);
        upgrade.value = Mathf.Round(baseValue * rarityMultiplier);
        
        // Clamp the final value to ensure it stays within 1-9 range
        upgrade.value = Mathf.Clamp(upgrade.value, 1f, 9f);
        
        // Generate name and description
        upgrade.upgradeName = GenerateUpgradeName(config.baseName, upgrade.rarity);
        upgrade.description = GenerateUpgradeDescription(config, upgrade.value);
        
        return upgrade;
    }

    private UpgradeData.Rarity SelectRandomRarity()
    {
        // Get player progress to influence rarity
        float playerProgress = GetPlayerProgress();
        
        // Create adjusted weights based on progress
        List<RarityWeights> adjustedWeights = new List<RarityWeights>();
        foreach (var rarityWeight in rarityWeights)
        {
            RarityWeights adjusted = new RarityWeights
            {
                rarity = rarityWeight.rarity,
                weight = rarityWeight.weight
            };
            
            // Boost rarer upgrades based on progress
            if (useProgressBasedRarity && playerProgress > 0)
            {
                float rarityBoost = GetRarityProgressBoost(rarityWeight.rarity, playerProgress);
                adjusted.weight *= rarityBoost;
            }
            
            adjustedWeights.Add(adjusted);
        }
        
        float totalWeight = 0f;
        foreach (var rarityWeight in adjustedWeights)
        {
            totalWeight += rarityWeight.weight;
        }
        
        float randomValue = (float)(systemRandom.NextDouble() * totalWeight);
        float currentWeight = 0f;
        
        foreach (var rarityWeight in adjustedWeights)
        {
            currentWeight += rarityWeight.weight;
            if (randomValue <= currentWeight)
            {
                return rarityWeight.rarity;
            }
        }
        
        return UpgradeData.Rarity.Common; // Fallback
    }
    
    private float GetPlayerProgress()
    {
        // Get player progress from various sources
        float progress = 0f;
        
        // Check highest level unlocked
        if (ProgressManager.Instance != null)
        {
            progress += ProgressManager.Instance.GetHighestLevelUnlocked() * 0.1f;
        }
        
        // Check player stats (if available)
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                progress += (playerHealth.maxHealth - 100) * 0.01f; // Health upgrades
            }
        }
        
        return Mathf.Clamp(progress, 0f, 10f); // Cap progress at 10
    }
    
    private float GetRarityProgressBoost(UpgradeData.Rarity rarity, float progress)
    {
        // Higher rarity upgrades get bigger boosts from progress
        float baseBoost = 1f + (progress * progressRarityMultiplier);
        
        switch (rarity)
        {
            case UpgradeData.Rarity.Trashy: return 1f; // No boost for trashy
            case UpgradeData.Rarity.Poor: return 1f; // No boost for poor
            case UpgradeData.Rarity.Common: return baseBoost * 0.5f;
            case UpgradeData.Rarity.Uncommon: return baseBoost * 1f;
            case UpgradeData.Rarity.Rare: return baseBoost * 2f;
            case UpgradeData.Rarity.Epic: return baseBoost * 3f;
            case UpgradeData.Rarity.Legendary: return baseBoost * 5f;
            case UpgradeData.Rarity.Mythic: return baseBoost * 8f;
            case UpgradeData.Rarity.Exotic: return baseBoost * 12f;
            default: return 1f;
        }
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

    private UpgradeData CreateFallbackUpgrade()
    {
        UpgradeData upgrade = ScriptableObject.CreateInstance<UpgradeData>();
        upgrade.rarity = UpgradeData.Rarity.Common;
        upgrade.upgradeType = UpgradeData.UpgradeType.Health;
        upgrade.value = 10f;
        upgrade.upgradeName = "Standard Health Boost";
        upgrade.description = "Increases maximum health by 10";
        return upgrade;
    }

    // Test method for debugging
    [ContextMenu("Test Clean Generation")]
    public void TestCleanGeneration()
    {
        for (int test = 1; test <= 3; test++)
        {
            List<UpgradeData> testUpgrades = GenerateUniqueUpgrades(3);
            
            // Small delay to ensure different seeds
            System.Threading.Thread.Sleep(50);
        }
    }
}