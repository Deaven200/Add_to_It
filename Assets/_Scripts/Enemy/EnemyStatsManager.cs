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
                break;
            case 1:
                speed += 1f;
                break;
            case 2:
                damage += 1f;
                break;
        }
    }
}
