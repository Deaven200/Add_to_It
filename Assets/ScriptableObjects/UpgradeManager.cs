using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public void ApplyUpgrade(UpgradeData upgrade)
    {
        // TEMP: just log for now
        Debug.Log($"Applied upgrade: {upgrade.upgradeName}");

        // Later you’ll apply logic based on upgrade type, value, etc.
        // Example:
        // if (upgrade.type == UpgradeType.Damage)
        //     player.damage += upgrade.value;
    }
}
