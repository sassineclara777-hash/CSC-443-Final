# CSC 443 Final
# Infinite Runner Extension
## Clara Sassine - 20231035

## Project Description

This project extends the in-class Infinite Runner project into a complete playable game. The player runs through an endlessly scrolling level, switches lanes, jumps over or onto objects, collects coins, and loses when hitting an obstacle head-on.

---

## Required Core Features

The project includes all three required core features:

1. **Collision & Game Over**
   - When the player hits an obstacle head-on, the game stops.
   - A Game Over panel appears with the final score, the player's best score/high score, a restart button, and main menu button.

2. **Restart**
   - The player can restart the game using the Restart button in the Game over Panel.
   - The scene can also be reloaded anytime while playing the game using the Esc key.

3. **Score HUD**
   - The current score increases live during gameplay.
   - The final score is shown on the Game Over screen.

---

## Chosen Extensions

1. **B - Main menu scene**
   - A main menu scene was added with Start and Quit buttons.
   - The Start button loads the gameplay scene.
   - The Quit button exits Play Mode in the Unity Editor.

2. **D - High score saved between runs**
   - The game saves the player's best score using PlayerPrefs.
   - When the player beats the previous high score, the old high score is replaced with the new one.

Additional feature added:

- **Coin collection system**
  - Coins can be collected during gameplay.
  - The number of collected coins is shown on the HUD.
  - A particle effect and sound effect play when a coin is collected.
 
- **Background Music and SFX**
   - The Main Menu scene and Game scene each have respective background tracks looping in the bakcground.
   - SFX include a coin collection sound and a player jumping sound.
   - A "game over" music track stops the background music and plays when the Game Over panel becomes visible.

---

## Controls

| Control | Action |
|---|---|
| A | Move left |
| D | Move right |
| W | Jump |
| Esc | Restart / reload the gameplay scene |
| Start Button | Start the game from the main menu |
| Quit Button | Quit the game |
| Restart Button | Restart after Game Over |
| Main Menu Button | Return to the main menu after Game Over |

---

## Known Bugs / Unfinished Features

- If the player jumps too early before an obstacle, the player's collider will collide with the Obstacle collider rather than the Walkable collider on top of the "train" obstacles, causing the Game Over state to trigger.
- The Esc key can still reload the gameplay scene even when the Game Over panel is visible.




