using System.Collections.Generic;
using UnityEngine;

public class UpgradeChest : MonoBehaviour
{
    public List<UpgradeData> testUpgrades;  // Assign some test upgrades in inspector

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (UpgradeManager.Instance != null)
            {
                UpgradeManager.Instance.OpenUpgradeMenu(testUpgrades);  // pass upgrades here
                Destroy(gameObject);
            }
        }
    }
}
