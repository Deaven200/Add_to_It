using UnityEngine;

public class MoneyPickup : MonoBehaviour
{
    public int value = 1;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMoney playerMoney = other.GetComponent<PlayerMoney>();
            if (playerMoney != null)
            {
                playerMoney.AddMoney(value);
            }

            Destroy(gameObject); // Remove the money after collecting
        }
    }
}
