using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 1;
    public int currentHealth;

    [SerializeField] private DeathScreenUI deathScreenUI;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log("Player took damage. Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        deathScreenUI.ShowDeathScreen();

        // Make sure to disable player controls or destroy the player object to prevent moving while dead.
        GetComponent<PlayerMovement>().enabled = false;
    }
}
