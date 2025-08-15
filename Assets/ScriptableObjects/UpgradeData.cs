using UnityEngine;

[CreateAssetMenu(fileName = "New Upgrade", menuName = "Upgrades/Upgrade Data")]
public class UpgradeData : ScriptableObject
{
    public string upgradeName;
    [TextArea] public string description;
    public Sprite icon;

    // This enum defines what kind of upgrade this is
    public UpgradeType upgradeType;

    // Generic numeric value (like +1 damage, +10% speed, etc.)
    public float value;

    // New rarity system
    public Rarity rarity = Rarity.Common;

    public enum UpgradeType
    {
        Damage,
        MoveSpeed,
        LifeOnKill,
        FireRate,
        ExplosionOnKill,
        ChestDropRate,
        Shield,
        Health,
        Armor,
        MoreOptions, // New upgrade type for more upgrade options
        BulletSpeed, // New weapon upgrade type
        AmmoCapacity, // New weapon upgrade type
        ReloadSpeed, // New weapon upgrade type
        // Aura System Types
        CoinMagnetAura, // Creates a coin magnet aura
        SlowAura, // Creates a slow aura that affects enemies
        ShieldAura, // Creates a shield aura that protects the player
        DamageAura, // Creates a damage aura that hurts enemies
        HealAura, // Creates a heal aura that heals the player
        // You can add more types later
    }

    public enum Rarity
    {
        Trashy,
        Poor,
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary,
        Mythic,
        Exotic
    }
}
