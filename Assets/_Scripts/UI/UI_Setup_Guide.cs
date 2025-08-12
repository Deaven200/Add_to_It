using UnityEngine;

/// <summary>
/// This is a guide script showing the recommended UI hierarchy setup for the new bar system.
/// This script doesn't need to be attached to anything - it's just for reference.
/// </summary>
public class UI_Setup_Guide : MonoBehaviour
{
    /*
    RECOMMENDED UI HIERARCHY SETUP:
    
    PlayerCanvas (Canvas)
    ├── HealthBarContainer (GameObject)
    │   ├── HealthBarUI (Script: HealthBarUI)
    │   ├── BarContainer (Image - Background/Container)
    │   │   └── BarFill (Image - Fill that shrinks/grows)
    │   └── HealthText (TextMeshProUGUI - Optional)
    ├── PauseMenu (GameObject - Initially inactive)
    │   ├── PauseManager (Script: PauseManager)
    │   └── [Pause menu UI elements]
    └── Other UI elements...
    
    SETUP INSTRUCTIONS:
    
    1. Create a Canvas as a child of your Player GameObject
    2. Add the HealthBarUI script to a GameObject in the Canvas
    3. Create the bar structure:
       - Create an Image for the container (background)
       - Create a child Image for the fill
       - Set the fill image's anchor to the left side
       - Set the fill image's pivot to the left center
    4. Add the BarUI script to the container GameObject
    5. Assign the references in the HealthBarUI script:
       - Bar Container: The background image
       - Bar Fill: The fill image
       - Value Text: Optional text component
    6. Configure the BarUI settings:
       - Max Value: 100 (or your desired max health)
       - Current Value: 100 (or starting health)
       - Fill Direction: LeftToRight (or your preference)
       - Animation Speed: 5 (or your preference)
    
    BAR SETTINGS:
    
    - Container Image: Should be a solid color or background image
    - Fill Image: Should be the color you want for the health bar
    - Fill Direction: Choose how the bar fills (LeftToRight, RightToLeft, etc.)
    - Animation: Enable/disable smooth transitions
    - Animation Speed: How fast the bar animates to new values
    
    MULTIPLAYER CONSIDERATIONS:
    
    - Each player should have their own Canvas
    - The Canvas should be a child of the player GameObject
    - This ensures each player has independent UI
    - No need to recreate UI for new levels/scenes
    */
}
