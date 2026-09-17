# 🚗 Car Puzzle Game

A simple 2D puzzle game made in Unity where the player has to place different vehicles into their corresponding places on a map.

The player must drag each vehicle to the correct location and adjust its:

- Position
- Size
- Rotation
- Mirrored state

until it matches the corresponding place.

---

## 🎮 Gameplay

The game consists of a map containing vehicles and corresponding vehicle places.

Each vehicle and its place have a matching tag. The player must identify where each vehicle belongs and then transform it until it fits the target.

### Controls

| Key | Action |
|---|---|
| `Left Mouse Button` | Drag a vehicle |
| `Z` | Rotate clockwise |
| `X` | Rotate counter-clockwise |
| `↑` | Increase height |
| `↓` | Decrease height |
| `←` | Decrease width |
| `→` | Increase width |
| `Space` | Flip / mirror the vehicle |

A vehicle is considered correctly placed when its:

- Position matches the target
- Rotation is within the allowed tolerance
- Width and height are within the allowed tolerance
- Mirrored state matches the target

---

## 🧩 Randomization

The game uses randomized spawn positions for both vehicles and their corresponding places.

There are currently:

- **12 vehicles**
- **12 vehicle places**
- **17 possible vehicle spawn positions**
- **17 possible vehicle-place spawn positions**

At the start of each game, the available positions are shuffled and 12 of the 17 positions are selected for the vehicles and places.

This leaves **5 unused positions** on each side.

Vehicle properties are also randomized at the start of each game:

- Position
- Rotation
- Width
- Height
- Mirrored state

This makes the puzzle different each time the game starts.

---

## 🖼️ Screenshots

### Main Game

<!-- Add screenshot here -->

### Example Gameplay

<!-- Add screenshot here -->

### Correctly Placed Vehicle

<!-- Add screenshot here -->

---

## 🛠️ Technologies

- **Unity**
- **C#**
- Unity UI / `RectTransform`
- Unity Event System
- Git / GitHub

---

## 📁 Project Structure

The main scripts currently used by the project include:

```text
Scripts/
├── DragAndDropScript.cs
├── DropPlaceScript.cs
├── GameObjectsScript.cs
├── ObjectTransformationScript.cs
└── ScreenBoundaryScript.cs
