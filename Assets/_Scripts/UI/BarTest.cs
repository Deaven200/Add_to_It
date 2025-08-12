using UnityEngine;

/// <summary>
/// Test script for the BarUI system.
/// Press keys to test different bar functionalities.
/// </summary>
public class BarTest : MonoBehaviour
{
    [Header("Test Bars")]
    [SerializeField] private BarUI healthBar;
    [SerializeField] private BarUI staminaBar;
    
    [Header("Test Settings")]
    [SerializeField] private int damageAmount = 10;
    [SerializeField] private int healAmount = 10;
    [SerializeField] private int staminaDrainAmount = 5;
    [SerializeField] private int staminaRegenAmount = 5;
    
    void Update()
    {
        // Health bar tests
        if (Input.GetKeyDown(KeyCode.H))
        {
            // Heal
            if (healthBar != null)
            {
                PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.Heal(healAmount);
                    Debug.Log($"Healed {healAmount} health. Current: {playerHealth.currentHealth}/{playerHealth.maxHealth}");
                }
            }
        }
        
        if (Input.GetKeyDown(KeyCode.J))
        {
            // Take damage
            if (healthBar != null)
            {
                PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damageAmount);
                    Debug.Log($"Took {damageAmount} damage. Current: {playerHealth.currentHealth}/{playerHealth.maxHealth}");
                }
            }
        }
        
        if (Input.GetKeyDown(KeyCode.K))
        {
            // Set health to specific value
            if (healthBar != null)
            {
                PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
                if (playerHealth != null)
                {
                    // Calculate how much to heal or damage to reach 50
                    int difference = 50 - playerHealth.currentHealth;
                    if (difference > 0)
                    {
                        playerHealth.Heal(difference);
                    }
                    else if (difference < 0)
                    {
                        playerHealth.TakeDamage(-difference);
                    }
                    Debug.Log($"Set health to 50. Current: {playerHealth.currentHealth}/{playerHealth.maxHealth}");
                }
            }
        }
        
        if (Input.GetKeyDown(KeyCode.L))
        {
            // Restore full health
            if (healthBar != null)
            {
                PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.RestoreFullHealth();
                    Debug.Log($"Restored full health. Current: {playerHealth.currentHealth}/{playerHealth.maxHealth}");
                }
            }
        }
        
        // Bar scaling tests
        if (Input.GetKeyDown(KeyCode.Y))
        {
            // Increase max health
            if (healthBar != null)
            {
                int newMax = healthBar.GetMaxValue() + 50;
                healthBar.SetMaxValue(newMax);
                Debug.Log($"Increased max health to {newMax}. Bar width: {healthBar.GetBarWidth():F1}");
            }
        }
        
        if (Input.GetKeyDown(KeyCode.T))
        {
            // Decrease max health
            if (healthBar != null)
            {
                int newMax = Mathf.Max(20, healthBar.GetMaxValue() - 50);
                healthBar.SetMaxValue(newMax);
                Debug.Log($"Decreased max health to {newMax}. Bar width: {healthBar.GetBarWidth():F1}");
            }
        }
        
        // Exponential scaling tests
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // Set to 50 health (1:1 scaling) - Update actual player health
            if (healthBar != null)
            {
                PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.SetMaxHealth(50);
                    // Don't automatically heal - let current health stay as is
                    Debug.Log($"Set player max health to 50. Current: {playerHealth.currentHealth}/50. Bar width: {healthBar.GetBarWidth():F1}, Scaling: {healthBar.GetScalingFactor(50):F1}:1");
                }
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            // Set to 100 health (1:1 scaling) - Update actual player health
            if (healthBar != null)
            {
                PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.SetMaxHealth(100);
                    // Don't automatically heal - let current health stay as is
                    Debug.Log($"Set player max health to 100. Current: {playerHealth.currentHealth}/100. Bar width: {healthBar.GetBarWidth():F1}, Scaling: {healthBar.GetScalingFactor(100):F1}:1");
                }
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            // Set to 150 health (2:1 scaling) - Update actual player health
            if (healthBar != null)
            {
                PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.SetMaxHealth(150);
                    // Don't automatically heal - let current health stay as is
                    Debug.Log($"Set player max health to 150. Current: {playerHealth.currentHealth}/150. Bar width: {healthBar.GetBarWidth():F1}, Scaling: {healthBar.GetScalingFactor(150):F1}:1");
                }
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            // Set to 200 health (2:1 scaling) - Update actual player health
            if (healthBar != null)
            {
                PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.SetMaxHealth(200);
                    // Don't automatically heal - let current health stay as is
                    Debug.Log($"Set player max health to 200. Current: {playerHealth.currentHealth}/200. Bar width: {healthBar.GetBarWidth():F1}, Scaling: {healthBar.GetScalingFactor(200):F1}:1");
                }
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            // Set to 300 health (4:1 scaling) - Update actual player health
            if (healthBar != null)
            {
                PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.SetMaxHealth(300);
                    // Don't automatically heal - let current health stay as is
                    Debug.Log($"Set player max health to 300. Current: {playerHealth.currentHealth}/300. Bar width: {healthBar.GetBarWidth():F1}, Scaling: {healthBar.GetScalingFactor(300):F1}:1");
                }
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            // Set to 400 health (4:1 scaling) - Update actual player health
            if (healthBar != null)
            {
                PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.SetMaxHealth(400);
                    // Don't automatically heal - let current health stay as is
                    Debug.Log($"Set player max health to 400. Current: {playerHealth.currentHealth}/400. Bar width: {healthBar.GetBarWidth():F1}, Scaling: {healthBar.GetScalingFactor(400):F1}:1");
                }
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            // Set to 500 health (8:1 scaling) - Update actual player health
            if (healthBar != null)
            {
                PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.SetMaxHealth(500);
                    // Don't automatically heal - let current health stay as is
                    Debug.Log($"Set player max health to 500. Current: {playerHealth.currentHealth}/500. Bar width: {healthBar.GetBarWidth():F1}, Scaling: {healthBar.GetScalingFactor(500):F1}:1");
                }
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            // Set to 1000 health (16:1 scaling) - Update actual player health
            if (healthBar != null)
            {
                PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.SetMaxHealth(1000);
                    // Don't automatically heal - let current health stay as is
                    Debug.Log($"Set player max health to 1000. Current: {playerHealth.currentHealth}/1000. Bar width: {healthBar.GetBarWidth():F1}, Scaling: {healthBar.GetScalingFactor(1000):F1}:1");
                }
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            // Set to 2000 health (32:1 scaling) - Update actual player health
            if (healthBar != null)
            {
                PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.SetMaxHealth(2000);
                    // Don't automatically heal - let current health stay as is
                    Debug.Log($"Set player max health to 2000. Current: {playerHealth.currentHealth}/2000. Bar width: {healthBar.GetBarWidth():F1}, Scaling: {healthBar.GetScalingFactor(2000):F1}:1");
                }
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            // Set to 5000 health (64:1 scaling) - Update actual player health
            if (healthBar != null)
            {
                PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.SetMaxHealth(5000);
                    // Don't automatically heal - let current health stay as is
                    Debug.Log($"Set player max health to 5000. Current: {playerHealth.currentHealth}/5000. Bar width: {healthBar.GetBarWidth():F1}, Scaling: {healthBar.GetScalingFactor(5000):F1}:1");
                }
            }
        }
        
        // Stamina bar tests
        if (Input.GetKeyDown(KeyCode.U))
        {
            // Drain stamina
            if (staminaBar != null)
            {
                staminaBar.ModifyValue(-staminaDrainAmount);
                Debug.Log($"Drained {staminaDrainAmount} stamina. Current: {staminaBar.GetCurrentValue()}/{staminaBar.GetMaxValue()}");
            }
        }
        
        if (Input.GetKeyDown(KeyCode.I))
        {
            // Regenerate stamina
            if (staminaBar != null)
            {
                staminaBar.ModifyValue(staminaRegenAmount);
                Debug.Log($"Regenerated {staminaRegenAmount} stamina. Current: {staminaBar.GetCurrentValue()}/{staminaBar.GetMaxValue()}");
            }
        }
        
        if (Input.GetKeyDown(KeyCode.O))
        {
            // Restore full stamina
            if (staminaBar != null)
            {
                staminaBar.SetCurrentValue(staminaBar.GetMaxValue());
                Debug.Log($"Restored full stamina. Current: {staminaBar.GetCurrentValue()}/{staminaBar.GetMaxValue()}");
            }
        }
        
        // Print current values
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("=== Current Bar Values ===");
            if (healthBar != null)
            {
                Debug.Log($"Health: {healthBar.GetCurrentValue()}/{healthBar.GetMaxValue()} ({healthBar.GetFillPercentage():P1})");
                Debug.Log($"Bar Width: {healthBar.GetBarWidth():F1}");
                Debug.Log($"Current Scaling: {healthBar.GetScalingFactor(healthBar.GetMaxValue()):F1}:1");
                Debug.Log($"Scaling Interval: {healthBar.GetScalingInterval(healthBar.GetMaxValue())}");
            }
            if (staminaBar != null)
            {
                Debug.Log($"Stamina: {staminaBar.GetCurrentValue()}/{staminaBar.GetMaxValue()} ({staminaBar.GetFillPercentage():P1})");
                Debug.Log($"Bar Width: {staminaBar.GetBarWidth():F1}");
            }
        }
        
        // Print exponential scaling info
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("=== Exponential Scaling System Info ===");
            if (healthBar != null)
            {
                Debug.Log($"50 health = {healthBar.GetCalculatedWidth(50):F1} width (1:1 scaling)");
                Debug.Log($"100 health = {healthBar.GetCalculatedWidth(100):F1} width (1:1 scaling)");
                Debug.Log($"150 health = {healthBar.GetCalculatedWidth(150):F1} width (2:1 scaling)");
                Debug.Log($"200 health = {healthBar.GetCalculatedWidth(200):F1} width (2:1 scaling)");
                Debug.Log($"300 health = {healthBar.GetCalculatedWidth(300):F1} width (4:1 scaling)");
                Debug.Log($"400 health = {healthBar.GetCalculatedWidth(400):F1} width (4:1 scaling)");
                Debug.Log($"500 health = {healthBar.GetCalculatedWidth(500):F1} width (8:1 scaling)");
                Debug.Log($"1000 health = {healthBar.GetCalculatedWidth(1000):F1} width (16:1 scaling)");
                Debug.Log($"2000 health = {healthBar.GetCalculatedWidth(2000):F1} width (32:1 scaling)");
                Debug.Log($"5000 health = {healthBar.GetCalculatedWidth(5000):F1} width (64:1 scaling)");
            }
        }
    }
    
    void OnGUI()
    {
        // Display test instructions
        GUILayout.BeginArea(new Rect(10, 10, 400, 600));
        GUILayout.Label("Bar Test Controls:", GUI.skin.box);
        GUILayout.Label("H - Heal");
        GUILayout.Label("J - Take Damage");
        GUILayout.Label("K - Set Health to 50");
        GUILayout.Label("L - Restore Full Health");
        GUILayout.Label("Y - Increase Max Health (+50)");
        GUILayout.Label("T - Decrease Max Health (-50)");
        GUILayout.Label("1-6 - Set health to 50,100,150,200,300,400");
        GUILayout.Label("7-9 - Set health to 500,1000,2000");
        GUILayout.Label("0 - Set health to 5000");
        GUILayout.Label("U - Drain Stamina");
        GUILayout.Label("I - Regenerate Stamina");
        GUILayout.Label("O - Restore Full Stamina");
        GUILayout.Label("Space - Print Values");
        GUILayout.Label("P - Print Scaling Info");
        GUILayout.EndArea();
    }
}
