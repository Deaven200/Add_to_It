using UnityEngine;

// Interface for enemy attack detection
public interface IEnemyAttack
{
    bool IsAttacking();
    float GetAttackDamage();
}

// Example implementation - you can modify this based on your enemy system
public class EnemyAttack : MonoBehaviour, IEnemyAttack
{
    [Header("Attack Settings")]
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 2f;
    
    private bool isAttacking = false;
    private float lastAttackTime = 0f;
    private Transform player;
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }
    
    void Update()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            
            // Check if we should attack
            if (distanceToPlayer <= attackRange && Time.time >= lastAttackTime + attackCooldown)
            {
                StartAttack();
            }
        }
    }
    
    private void StartAttack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;
        
        // Simulate attack duration
        Invoke(nameof(EndAttack), 0.5f);
    }
    
    private void EndAttack()
    {
        isAttacking = false;
    }
    
    // Interface implementation
    public bool IsAttacking()
    {
        return isAttacking;
    }
    
    public float GetAttackDamage()
    {
        return attackDamage;
    }
    
    // Public methods for external control
    public void SetAttackDamage(float damage)
    {
        attackDamage = damage;
    }
    
    public void SetAttackRange(float range)
    {
        attackRange = range;
    }
    
    public void SetAttackCooldown(float cooldown)
    {
        attackCooldown = cooldown;
    }
}
