# Aura System

This system allows upgrades to create capsule colliders (auras) around the player that provide various effects.

## How It Works

1. **AuraSystem**: The main component that manages all auras on the player
2. **AuraEffect**: Individual aura effects with capsule colliders and visual effects
3. **Upgrade Integration**: New upgrade types that create auras when selected

## Aura Types

### 1. Coin Magnet Aura
- **Effect**: Attracts coins towards the player
- **Color**: Yellow
- **Target**: Money objects with "Money" tag

### 2. Slow Aura
- **Effect**: Slows down enemies that enter the aura
- **Color**: Blue
- **Target**: Enemies with "Enemy" tag

### 3. Shield Aura
- **Effect**: Provides passive protection (visual indicator)
- **Color**: Green
- **Target**: Enemies (for visual purposes)

### 4. Damage Aura
- **Effect**: Deals damage to enemies over time
- **Color**: Red
- **Target**: Enemies with "Enemy" tag

### 5. Heal Aura
- **Effect**: Heals the player over time
- **Color**: Magenta
- **Target**: Player

## Setup Instructions

### 1. Add to Player
Add the `PlayerAuraSetup` component to your player GameObject. This will automatically add the `AuraSystem` component.

### 2. Configure Layer Masks
In the `AuraSystem` component inspector:
- Set `Enemy Layer Mask` to the layer your enemies are on
- Set `Coin Layer Mask` to the layer your money/coins are on

### 3. Add Visual Materials (Optional)
You can assign custom materials for each aura type in the `AuraSystem` inspector for better visual effects.

## Upgrade Integration

The system automatically integrates with your existing upgrade system. New aura upgrades will appear in upgrade chests:

- **Coin Magnet Aura**: Creates a coin magnet aura
- **Slow Aura**: Creates a slow aura
- **Shield Aura**: Creates a protective shield aura
- **Damage Aura**: Creates a damage aura
- **Heal Aura**: Creates a healing aura

## Aura Properties

- **Radius**: Based on upgrade value and rarity
- **Rarity Multipliers**: Higher rarity = larger radius
- **Visual Effects**: Pulsing transparent spheres
- **Stacking**: Multiple auras of the same type will replace each other

## Testing

Use the context menu options on the `AuraSystem` component:
- **Test Add All Auras**: Adds one of each aura type for testing
- **Remove All Auras**: Removes all active auras

## Technical Details

- Auras use capsule colliders as triggers
- Visual effects are created using sphere primitives with transparent materials
- Each aura type has its own event handlers for different effects
- Auras automatically follow the player's position
- The system supports multiple auras of different types simultaneously

## Troubleshooting

1. **Auras not appearing**: Make sure the `AuraSystem` component is on the player
2. **Effects not working**: Check that target objects have the correct tags ("Enemy", "Money", "Player")
3. **Visual effects missing**: Ensure materials are assigned or the system will use default colors
4. **Performance issues**: Limit the number of active auras or reduce their radius
