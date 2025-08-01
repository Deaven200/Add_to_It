using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public float maxHealth = 1f;
    private float currentHealth;

    public float moveSpeed = 3f;
    public float damage = 1f;

    private Transform player;
    private NavMeshAgent agent;

    void Start()
    {
        currentHealth = maxHealth;

        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
    }

    void Update()
    {
        if (player != null && agent.isOnNavMesh)
        {
            agent.SetDestination(player.position);
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        // Drop money or effects here if needed
        Destroy(gameObject);
    }
}


