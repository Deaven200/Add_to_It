using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public float damageRange = 1.5f;  // how close before damaging
    public int damageAmount = 1;
    public float damageCooldown = 1f;  // seconds between hits

    private float lastDamageTime;
    private Transform player;
    private bool isDamaging = false; // Track if currently damaging

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        
        // Check if we can damage the player
        bool canDamage = distance <= damageRange && Time.time - lastDamageTime > damageCooldown;
        
        if (canDamage && !isDamaging)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                isDamaging = true;
                playerHealth.TakeDamage(damageAmount);
                lastDamageTime = Time.time;
                
                // Damage applied successfully
            }
        }
        else if (distance > damageRange && isDamaging)
        {
            // Reset damage flag when player moves out of range
            isDamaging = false;
        }
    }
    
    // Optional: Visual debugging in scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, damageRange);
    }
}
