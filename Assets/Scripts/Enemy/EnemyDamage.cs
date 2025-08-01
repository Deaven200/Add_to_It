using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public float damageRange = 1.5f;  // how close before damaging
    public int damageAmount = 1;
    public float damageCooldown = 1f;  // seconds between hits

    private float lastDamageTime;
    private Transform player;

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
        if (distance <= damageRange && Time.time - lastDamageTime > damageCooldown)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
                lastDamageTime = Time.time;
            }
        }
    }
}
