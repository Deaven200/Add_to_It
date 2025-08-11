using UnityEngine;
using TMPro;

public class PlayerMoney : MonoBehaviour
{
    public int money = 0;

    void Start()
    {
        // Load money from UIManager if available, otherwise use default
        if (UIManager.Instance != null)
        {
            money = UIManager.Instance.GetPlayerMoney();
        }
        else
        {
            money = 0;
        }
        
        // Update UIManager with current money
        if (UIManager.Instance != null)
        {
            UIManager.Instance.SetPlayerMoney(money);
        }
    }

    public void AddMoney(int amount)
    {
        money += amount;
        
        // Update UIManager
        if (UIManager.Instance != null)
        {
            UIManager.Instance.AddPlayerMoney(amount);
        }
        else
        {
            // Fallback to local update if UIManager not available
            UpdateMoneyUI();
        }
    }
    
    public void SetMoney(int newAmount)
    {
        money = newAmount;
        
        // Update UIManager
        if (UIManager.Instance != null)
        {
            UIManager.Instance.SetPlayerMoney(money);
        }
        else
        {
            // Fallback to local update if UIManager not available
            UpdateMoneyUI();
        }
    }

    void UpdateMoneyUI()
    {
        // This is now handled by UIManager, but kept as fallback
        // Money updated successfully
    }
    
    public int GetMoney()
    {
        return money;
    }
}
