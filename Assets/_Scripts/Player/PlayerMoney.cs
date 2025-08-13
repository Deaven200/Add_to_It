using UnityEngine;
using TMPro;
using System;

public class PlayerMoney : MonoBehaviour
{
    [Header("Currency Settings")]
    [SerializeField] private string currencyName = "Coins";
    [SerializeField] private int startingMoney = 0;
    
    [Header("UI Display")]
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private string displayFormat = "{0} {1}"; // {0} = amount, {1} = currency name
    
    private int currentMoney = 0;
    
    // Event that gets called when money changes
    public event Action<int> OnMoneyChanged;

    void Start()
    {
        // Load money from UIManager if available, otherwise use default
        if (UIManager.Instance != null)
        {
            currentMoney = UIManager.Instance.GetPlayerMoney();
            // Tell UIManager about our currency name and format
            UIManager.Instance.SetCurrencyName(currencyName);
            UIManager.Instance.SetMoneyDisplayFormat(displayFormat);
        }
        else
        {
            currentMoney = startingMoney;
        }
        
        // Update UIManager with current money
        if (UIManager.Instance != null)
        {
            UIManager.Instance.SetPlayerMoney(currentMoney);
        }
        
        // Update UI
        UpdateMoneyUI();
        
        // Notify listeners of initial money
        OnMoneyChanged?.Invoke(currentMoney);
    }

    public void AddMoney(int amount)
    {
        currentMoney += amount;
        
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
        
        // Notify listeners
        OnMoneyChanged?.Invoke(currentMoney);
    }
    
    public void SetMoney(int newAmount)
    {
        currentMoney = newAmount;
        
        // Update UIManager
        if (UIManager.Instance != null)
        {
            UIManager.Instance.SetPlayerMoney(currentMoney);
        }
        else
        {
            // Fallback to local update if UIManager not available
            UpdateMoneyUI();
        }
        
        // Notify listeners
        OnMoneyChanged?.Invoke(currentMoney);
    }
    
    public void SpendMoney(int amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            
            // Update UIManager
            if (UIManager.Instance != null)
            {
                UIManager.Instance.SetPlayerMoney(currentMoney);
            }
            else
            {
                // Fallback to local update if UIManager not available
                UpdateMoneyUI();
            }
            
            // Notify listeners
            OnMoneyChanged?.Invoke(currentMoney);
        }
        else
        {
            Debug.LogWarning($"Not enough {currencyName}! Need {amount}, have {currentMoney}");
        }
    }

    void UpdateMoneyUI()
    {
        if (moneyText != null)
        {
            string displayText = string.Format(displayFormat, currentMoney, currencyName);
            moneyText.text = displayText;
        }
    }
    
    public int GetMoney()
    {
        return currentMoney;
    }
    
    public string GetCurrencyName()
    {
        return currencyName;
    }
    
    public bool HasEnoughMoney(int amount)
    {
        return currentMoney >= amount;
    }
    
    // Testing methods
    [ContextMenu("Add 100 Money")]
    public void TestAddMoney()
    {
        AddMoney(100);
    }
    
    [ContextMenu("Spend 50 Money")]
    public void TestSpendMoney()
    {
        SpendMoney(50);
    }
    
    [ContextMenu("Set Money to 1000")]
    public void TestSetMoney()
    {
        SetMoney(1000);
    }
}
