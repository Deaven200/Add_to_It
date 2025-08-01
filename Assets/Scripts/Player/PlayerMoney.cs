using UnityEngine;
using TMPro; // Add this

public class PlayerMoney : MonoBehaviour
{
    public int money = 0;
    public TextMeshProUGUI moneyText; // Update this line

    void Start()
    {
        UpdateMoneyUI();
    }

    public void AddMoney(int amount)
    {
        money += amount;
        UpdateMoneyUI();
    }

    void UpdateMoneyUI()
    {
        moneyText.text = "Money: " + money;
    }
}
