# Add_to_It

A community-driven survival game. The project's goal is to build a playable game through open-source contributions.

## A Note from the Original Author

> "You don't have to chip in, but my goal is to make a silly game with inspiration from COD Zombies, Nova Drift, Vampire Survivors, brotato, and Bounty of One. It's nothing serious, but I have spent about 3 hours, and a lot of that time asking ChatGPT for help. I'm gonna get back to working on it tomorrow, so even if you add something small like funny faces on the enemy capsules, I il put it in. (*^3^)/~☆"

## Current State: A Detailed Look

The project is built in a single scene with a robust set of interconnected systems.

* **Player & Camera**
    * **Movement:** Full `WASD` directional movement (`PlayerMovement.cs`).
    * **Combat:** Fires a `Bullet` prefab by **left-clicking** (`PlayerShooting.cs`). The bullet's damage is defined in its own `Bullet.cs` script.
    * **Health:** The player has a health system managed by `PlayerHealth.cs`. When health reaches zero, a death message is displayed and the game pauses, creating a loss condition.
    * **Currency:** A `PlayerMoney.cs` script on the player tracks the amount of money collected.
    * **Camera:** A sophisticated camera rig (`CameraController.cs`) follows the player, handles mouse-look aiming, and allows toggling between first-person and third-person views with the 'V' key.

* **Enemies**
    * **AI & Movement:** Enemies use a NavMeshAgent to seek the player (`EnemyAI.cs`).
    * **Health:** Enemy health is managed by `EnemyHealth.cs`. When health is depleted, the `Die()` method is called.
    * **Damaging the Player:** Enemies deal damage to the player when they get within range, managed by the `EnemyDamage.cs` script.
    * **Death Mechanic:** Upon death, the `EnemyHealth` script instantiates a `coinPrefab` at its location before the enemy object is destroyed.

* **Game Loop & Systems**
    * **Spawning:** The `EnemySpawner.cs` script manages a wave-based system. It spawns a progressively larger number of enemies each wave and waits for the player to clear them before starting the next.
    * **Dynamic Difficulty:** A global `EnemyStatManager.cs` script increases one of three base enemy stats (health, speed, or damage) every 10 seconds.
    * **Stat Integration:** The `EnemySpawner` reads the current values from the `EnemyStatManager` and applies them to each newly spawned enemy, making the game progressively harder.
    * **Currency Pickup:** The dropped coin (`Money` prefab) is managed by `Money.cs`. When the player collides with it, the script adds value to the player's `PlayerMoney` script and the coin is destroyed.

## Areas for Contribution

The project is ready for expansion. The existing systems provide a solid base for new features.

* **Gameplay Mechanics:**
    * **Upgrade Shop:** The most logical next step. Create a system (perhaps a UI panel opened with a key) where the player can spend their collected "Money" on upgrades (e.g., increased bullet damage, faster fire rate, more player health).
    * **New Enemy Types:** Design enemies with different behaviors, such as ranged attacks or unique movement patterns.

* **Content and Assets:**
    * **Visuals & Audio:** Replace the placeholder primitives with actual 2D or 3D art. Add sound effects for shooting, enemy hits, coin pickups, and player death. Implement background music.

* **System and UI Development:**
    * **User Interface:** Build a UI to display player health, money count, current wave number, and maybe the current enemy stat levels from the `EnemyStatManager`.
    * **Code Refactoring:** The `EnemyAI.cs` script has redundant health/damage variables that could be removed to avoid confusion with `EnemyHealth.cs` and `EnemyDamage.cs`. Similarly, `MoneyPickup.cs` appears to be an unused duplicate of `Money.cs`.

* **Go Insane:**
    * This is an open invitation to add something wild. If you have a chaotic, hilarious, or completely unpredictable idea, this is the place for it. Examples: make enemies explode into confetti, give the player a temporary "disco mode" that makes everyone dance, or add a power-up that turns all bullets into chickens. The goal here is fun over balance.

## Getting Started

### Prerequisites

* **Unity Hub**
* **Unity Version:** `2022.3.4f1` (Using this specific version is required for compatibility).
* **Git**

### Contribution Workflow

1.  **Fork** the repository to your own GitHub account.
2.  **Clone** your fork to your local machine.
3.  **Open** the project in Unity.
4.  **Create a new branch** for your changes.
5.  **Make your changes** and commit them with clear messages.
6.  **Push** your branch to your fork.
7.  **Open a Pull Request** to the `main` branch of the original repository.
