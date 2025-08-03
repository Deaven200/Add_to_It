using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportToHub : MonoBehaviour
{
    public string hubSceneName = "HubScene"; // Or hardcoded if preferred
    private float timer = 0f;
    public float timeToTeleport = 3f;

    private bool playerInside = false;

    void Update()
    {
        if (playerInside)
        {
            timer += Time.deltaTime;
            if (timer >= timeToTeleport)
            {
                Debug.Log("Teleporting to Hub...");
                SceneManager.LoadScene(hubSceneName); // or "Hub" directly
            }
        }
        else
        {
            timer = 0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
        }
    }
}
