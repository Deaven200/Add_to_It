# Console Error Fix Guide

## Issues Found:

### 1. **Missing Script Reference**
**Error**: "The referenced script (Unknown) on this Behaviour is missing!"

**Solution**:
1. Add the `MissingScriptFinder` script to any GameObject in your scene
2. Run the scene - it will automatically find and mark missing scripts for removal
3. Save the scene to complete the cleanup
4. Remove the `MissingScriptFinder` script when done

### 2. **DontDestroyOnLoad Error**
**Error**: "DontDestroyOnLoad only works for root GameObjects or components on root GameObjects."

**Status**: ✅ **FIXED** - The UIManager now automatically becomes a root GameObject before using DontDestroyOnLoad.

### 3. **Button Connection Issue**
**Warning**: "Button 'Button' event 0 connected to GameObject. (should be PauseManager)"

**Solution**:
1. Find the button named "Button" in your pause menu
2. In the Inspector, check its OnClick() events
3. Make sure all events are connected to the PauseManager component, not the GameObject
4. The correct connections should be:
   - `PauseManager.OnResumeButtonPressed`
   - `PauseManager.OnSettingsButtonPressed`
   - `PauseManager.OnQuitToMainMenuButtonPressed`

### 4. **Player Money Issue**
**Problem**: Player starts with 5 money instead of 0

**Solution**:
1. **Quick Fix**: In the Unity Editor, find the UIManager GameObject in your scene
2. Right-click on the UIManager component in the Inspector
3. Select "Reset Player Data" from the context menu
4. This will clear all saved player data and reset money to 0

**Alternative Solution** (if you want to always start with 0 money):
1. Open `Assets/_Scripts/UI/UIManager.cs`
2. Find the `LoadPlayerData()` method (around line 460)
3. Change the line `currentPlayerMoney = PlayerPrefs.GetInt("PlayerMoney", 0);` to `currentPlayerMoney = 0;`

## Quick Fix Steps:

### Step 1: Fix Missing Scripts
1. Create an empty GameObject in your scene
2. Name it "ScriptFinder"
3. Add the `MissingScriptFinder` component to it
4. Run the scene - it will find and mark missing scripts
5. Save the scene
6. Delete the ScriptFinder GameObject

### Step 2: Fix Button Connections
1. Find your pause menu in the scene
2. Look for a button named "Button"
3. In the Inspector, expand the OnClick() section
4. Make sure all events point to the PauseManager component
5. If any point to "GameObject", change them to point to the PauseManager

### Step 3: Fix Player Money
1. Find the UIManager GameObject in your scene
2. Right-click on the UIManager component in the Inspector
3. Select "Reset Player Data" from the context menu
4. This will reset money to 0

### Step 4: Verify Setup
1. Run the scene again
2. Check the console - the errors should be gone
3. Test the pause menu functionality
4. Verify player starts with 0 money

## Using the MissingScriptFinder:

The `MissingScriptFinder` script provides several useful context menu options:

- **Find Missing Scripts**: Automatically finds and marks missing scripts
- **List All GameObjects**: Shows all GameObjects and their components
- **Check Button Connections**: Verifies all button event connections

Right-click on the MissingScriptFinder component in the Inspector to access these options.

## Expected Console Output After Fix:

```
✅ No missing scripts found!
UIManager: Scene loaded - PlayerRoom
PauseManager: Start() called in scene: PlayerRoom
✅ All button connections verified
Player starts with 0 money
```

The errors should be resolved after following these steps! 🎮
