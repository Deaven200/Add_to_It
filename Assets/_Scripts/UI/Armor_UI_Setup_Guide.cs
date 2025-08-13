using UnityEngine;
using TMPro;

/// <summary>
/// Setup guide for Armor UI system.
/// This script shows how to set up the armor display.
/// </summary>
public class Armor_UI_Setup_Guide : MonoBehaviour
{
    [Header("UI Setup Instructions")]
    [TextArea(10, 20)]
    public string setupInstructions = @"
ARMOR UI SETUP GUIDE:

1. CREATE ARMOR UI:
   - Create a UI Image for the armor icon (your shield image)
   - Add a TextMeshProUGUI component for the armor number
   - Position it where you want the armor to display

2. CONFIGURE PLAYER ARMOR:
   - Add PlayerArmor component to your Player GameObject
   - Set Max Armor and Current Armor values in inspector
   - Assign the TextMeshProUGUI component to the 'Armor Text' field

3. CONFIGURE DAMAGE HANDLER:
   - Make sure PlayerDamageHandler is on your Player GameObject
   - The damage handler will automatically use armor if PlayerArmor component exists

4. TEST THE SYSTEM:
   - Use context menu on PlayerArmor: 'Add 5 Armor'
   - Use context menu on PlayerDamageHandler: 'Take 20 Damage (Shield Priority)'
   - Watch console for armor damage reduction logs

DAMAGE PRIORITY:
Armor -> Shield -> Health

Example: 10 damage with 3 armor, 5 shield, 100 health
- Armor reduces 10 to 7 damage (3 armor consumed)
- Shield takes 5 damage (shield: 0)
- Health takes 2 damage (health: 98)
";

    [Header("Example UI Hierarchy")]
    public string uiHierarchy = @"
Canvas
├── ArmorContainer (Image)
│   ├── ArmorIcon (Image) - Your shield sprite
│   └── ArmorText (TextMeshProUGUI) - Shows armor number
";

    [Header("Component Requirements")]
    public string requiredComponents = @"
Player GameObject needs:
- PlayerArmor
- PlayerDamageHandler (already exists)
- PlayerShield (already exists)
- PlayerHealth (already exists)
";
}
