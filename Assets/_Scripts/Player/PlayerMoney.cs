using UnityEngine;
using TMPro; // Add this

public class PlayerMoney : MonoBehaviour
{
    public int money = 0;
    public TextMeshProUGUI moneyText; // Update this line

    void Start()
    {
        // Try to find the TextMeshProUGUI component if not assigned
        if (moneyText == null)
        {
            moneyText = GetComponentInChildren<TextMeshProUGUI>();
            
            // If still null, try to find it in the scene
            if (moneyText == null)
            {
                moneyText = FindObjectOfType<TextMeshProUGUI>();
            }
        }
        
        UpdateMoneyUI();
    }

    public void AddMoney(int amount)
    {
        money += amount;
        UpdateMoneyUI();
    }

    void UpdateMoneyUI()
    {
        // Add null check to prevent NullReferenceException
        if (moneyText != null)
        {
            moneyText.text = "Money: " + money;
        }
        else
        {
            Debug.LogWarning("PlayerMoney: moneyText is not assigned! Please assign a TextMeshProUGUI component in the Inspector.");
        }
    }
}
