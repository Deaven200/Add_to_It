using UnityEngine;

public class TestProceduralLevel : MonoBehaviour
{
    void Start()
    {
        // Add the simple setup script
        gameObject.AddComponent<SimpleProceduralSetup>();
        
        Debug.Log("Procedural level setup complete! Move the player to see chunks generate.");
    }
}
