using UnityEngine;

public class EnemyStatManager : MonoBehaviour
{
    public float health = 1f;
    public float speed = 1f;
    public float damage = 1f;

    public float increaseInterval = 10f;
    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= increaseInterval)
        {
            IncreaseRandomStat();
            timer = 0f;
        }
    }

    void IncreaseRandomStat()
    {
        int choice = Random.Range(0, 3);
        switch (choice)
        {
            case 0:
                health += 1f;
                Debug.Log("Increased enemy health: " + health);
                break;
            case 1:
                speed += 1f;
                Debug.Log("Increased enemy speed: " + speed);
                break;
            case 2:
                damage += 1f;
                Debug.Log("Increased enemy damage: " + damage);
                break;
        }
    }
}
