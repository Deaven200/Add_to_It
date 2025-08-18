using UnityEngine;

/// <summary>
/// Helper script to test and display reroll cost information in the inspector
/// </summary>
public class RerollCostTester : MonoBehaviour
{
    [Header("Reroll Cost Info")]
    [SerializeField] private bool showDebugInfo = true;
    [SerializeField] private int currentWave = 0;
    [SerializeField] private float timePassed = 0f;
    [SerializeField] private int calculatedCost = 0;
    [SerializeField] private bool canAfford = false;
    
    private UpgradeManager upgradeManager;
    private PlayerMoney playerMoney;
    
    void Start()
    {
        upgradeManager = FindObjectOfType<UpgradeManager>();
        playerMoney = FindObjectOfType<PlayerMoney>();
    }
    
    void Update()
    {
        if (upgradeManager != null)
        {
            currentWave = upgradeManager.GetCurrentWave();
            calculatedCost = upgradeManager.GetCurrentRerollCost();
            canAfford = upgradeManager.CanAffordReroll();
            
            if (playerMoney != null)
            {
                timePassed = Time.time - upgradeManager.GetGameStartTime();
            }
        }
    }
    
    [ContextMenu("Test Reroll")]
    public void TestReroll()
    {
        if (upgradeManager != null)
        {
            upgradeManager.RerollUpgrades();
        }
    }
    
    [ContextMenu("Add 100 Money")]
    public void AddMoney()
    {
        if (playerMoney != null)
        {
            playerMoney.AddMoney(100);
        }
    }
    
    [ContextMenu("Set Wave to 5")]
    public void SetWave5()
    {
        if (upgradeManager != null)
        {
            upgradeManager.SetCurrentWave(5);
        }
    }
    
    [ContextMenu("Set Wave to 10")]
    public void SetWave10()
    {
        if (upgradeManager != null)
        {
            upgradeManager.SetCurrentWave(10);
        }
    }
}
