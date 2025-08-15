using UnityEngine;

public class AuraCleanup : MonoBehaviour
{
    void Start()
    {
        // Find all AuraEffect components
        AuraEffect[] auraEffects = FindObjectsOfType<AuraEffect>();
        Debug.Log($"Found {auraEffects.Length} AuraEffect components to check");
        
        foreach (var effect in auraEffects)
        {
            Debug.Log($"Checking AuraEffect on {effect.gameObject.name}, Type: {effect.GetAuraType()}");
            
            // Check if this AuraEffect is a child of an AuraSystem-managed aura
            bool isManaged = false;
            Transform current = effect.transform;
            
            // Check if this effect is under an AuraSystem-managed GameObject
            while (current != null)
            {
                AuraSystem parentSystem = current.GetComponent<AuraSystem>();
                if (parentSystem != null)
                {
                    // This is a managed aura, don't remove it
                    isManaged = true;
                    Debug.Log($"AuraEffect on {effect.gameObject.name} is managed by AuraSystem on {current.name}");
                    break;
                }
                current = current.parent;
            }
            
            // If not managed and it's on the Player, remove it
            if (!isManaged && effect.gameObject.CompareTag("Player"))
            {
                Debug.LogError($"Removing rogue AuraEffect component from Player: {effect.GetAuraType()}");
                DestroyImmediate(effect);
            }
        }
    }
    
    [ContextMenu("Clean Up Rogue Auras")]
    public void CleanUpRogueAuras()
    {
        Start();
    }
    
    [ContextMenu("Force Remove All AuraEffects from Player")]
    public void ForceRemoveAllAuraEffectsFromPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            AuraEffect[] effects = player.GetComponents<AuraEffect>();
            Debug.LogError($"Found {effects.Length} AuraEffect components on Player, removing them all");
            foreach (var effect in effects)
            {
                DestroyImmediate(effect);
            }
        }
        else
        {
            Debug.LogError("Player not found!");
        }
    }
}
