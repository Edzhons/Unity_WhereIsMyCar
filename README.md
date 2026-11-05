# Car Puzzle & Tower of Hanoi

A Unity puzzle game featuring two game modes: a car-placement puzzle and
the classic Tower of Hanoi. The project includes progression, star
rewards, a shared in-game currency, a shop with unlockable disk skins
and helper tokens, and Unity Ads integration.

> **Screenshots:** Add your own screenshots to `docs/screenshots/` and
> use the filenames shown below. The image placeholders will display
> once the files are added.

## Table of contents

-   [Game modes](#game-modes)
    -   [Car Puzzle](#car-puzzle)
    -   [Tower of Hanoi](#tower-of-hanoi)
-   [Progression and star system](#progression-and-star-system)
-   [Shop](#shop)
-   [Advertisements](#advertisements)
-   [Project structure](#project-structure)
-   [How to run](#how-to-run)
-   [Controls](#controls)
-   [Development notes](#development-notes)

## Game modes

The project contains three Unity scenes:

  -----------------------------------------------------------------------
  Scene                               Purpose
  ----------------------------------- -----------------------------------
  `StartMenu`                         Main menu, game-mode selection,
                                      shop and settings/interactions

  `CityScene`                         Car puzzle gameplay

  `TowerOfHanoi`                      Tower of Hanoi puzzle gameplay
  -----------------------------------------------------------------------

### Main menu

The main menu is the central hub. Players can enter either game mode and
open the in-game shop. The menu also includes interactive elements and
audio feedback.

**Screenshot --- Main menu**

![Main menu screenshot](docs/screenshots/main-menu.png)

### Car Puzzle

In the Car Puzzle mode, players drag cars into their matching parking
spaces on the map. Cars can require transformations such as resizing,
mirroring or rotation to fit the target space.

The game includes: - Drag-and-drop car placement. - Car transformation
mechanics. - A timer and life system. - Flying hazards that can cost the
player a life. - Win and lose screens. - A star reward based on
completion time.

**Screenshot --- Car Puzzle gameplay**

![Car Puzzle gameplay
screenshot](docs/screenshots/car-puzzle-gameplay.png)

**Screenshot --- Car Puzzle result screen**

![Car Puzzle result screenshot](docs/screenshots/car-puzzle-results.png)

### Tower of Hanoi

Tower of Hanoi is an additional puzzle mode based on the classic
disk-stacking challenge. The objective is to move the complete stack
from the first rod to the third rod while following the standard rules:

1.  Move only one disk at a time.
2.  Only the top disk of a rod can be moved.
3.  A larger disk cannot be placed on top of a smaller disk.

The mode has **three progressively harder levels**:

  Level     Number of disks   Minimum moves
  ------- ----------------- ---------------
  1                       3               7
  2                       4              15
  3                       5              31

Players drag disks between the three rods. The game tracks the number of
moves and compares it with the theoretical minimum.

#### Star rating and level progression

Each completed level awards stars based on the number of moves:

-   **3 stars:** solved in the minimum possible number of moves.
-   **2 stars:** solved in up to 1.5 times the minimum number of moves.
-   **1 star:** completed with more moves.

The best star result for each level is saved. Completing a level unlocks
the next one. The level selection screen displays each level's unlock
state and saved star result.

#### Hints and undo

Players can request: - **Hint:** highlights the disk to move and
indicates the destination rod. - **Undo:** reverses the most recent
valid move.

Hints and undo can be used with tokens purchased in the shop. If the
player has no tokens, the game can offer a rewarded advertisement
instead. The bonus is granted only when the rewarded ad is completed
successfully.

**Screenshot --- Tower of Hanoi level selection**

![Tower of Hanoi level selection
screenshot](docs/screenshots/hanoi-level-selection.png)

**Screenshot --- Tower of Hanoi gameplay**

![Tower of Hanoi gameplay
screenshot](docs/screenshots/hanoi-gameplay.png)

**Screenshot --- Tower of Hanoi hint or undo**

![Tower of Hanoi helper
screenshot](docs/screenshots/hanoi-hint-undo.png)

**Screenshot --- Tower of Hanoi level completion**

![Tower of Hanoi results screenshot](docs/screenshots/hanoi-results.png)

## Progression and star system

Both game modes contribute to a shared star balance.

-   The Car Puzzle awards stars according to the time taken to complete
    the level.
-   Tower of Hanoi awards stars according to move efficiency.
-   Stars are stored locally using Unity `PlayerPrefs`.
-   The shared balance can be spent in the shop.
-   Hanoi level unlocks and each level's best star result are also
    stored locally.

The star balance is managed by the static `StarBank` class.

## Shop

The shop is accessible from the main menu and uses the shared star
balance. It contains permanent purchases, disk skins and consumable
helper-token packs.

The current offers configured in the project are:

  Item                                        Type                        Price
  ------------------------------------------- ---------------------- ----------
  Disable advertisements (`bez_reklamam`)     Permanent                30 stars
  Marble disk skin (`marmors`)                Disk skin                15 stars
  Gold disk skin (`zelts`)                    Disk skin                25 stars
  Wooden disk skin (`koks`)                   Disk skin                 8 stars
  Hint tokens (`padomi`)                      Token pack, 3 tokens      5 stars
  Undo tokens (`atsauksanas`)                 Token pack, 3 tokens      5 stars
  Colourful/default disk skin (`krasainie`)   Default skin                 Free

Purchased disk skins can be selected for Tower of Hanoi. Token packs
provide three uses per purchase. The shop updates the displayed balance
and offer states when the star balance or owned items change.

**Screenshot --- Shop**

![Shop screenshot](docs/screenshots/shop.png)

## Advertisements

The project integrates Unity Ads through the scripts in
`Assets/Scripts/Ads/`.

The implementation includes: - Interstitial advertisements. - Rewarded
advertisements. - Banner advertisements. - A persistent ad manager that
survives scene changes. - A shop purchase that disables advertisements.

Rewarded ads are used as an alternative way to obtain Tower of Hanoi
hints and undo moves when the player does not have the corresponding
tokens.

> Ad availability and display depend on Unity Ads configuration,
> platform support, placement IDs, and the project's Unity Ads dashboard
> settings. Use test mode during development.

## Project structure

``` text
Assets/
├── Animations/
├── Editor/
├── Fonts/
├── Prefabs/
├── Resources/
├── Scenes/
│   ├── StartMenu.unity
│   ├── CityScene.unity
│   └── TowerOfHanoi.unity
├── Scripts/
│   ├── Ads/
│   │   ├── AdManager.cs
│   │   ├── AdsInitializer.cs
│   │   ├── BannerAd.cs
│   │   ├── InterstitialAd.cs
│   │   └── RewardedAds.cs
│   ├── Shop/
│   │   ├── StarBank.cs
│   │   ├── ShopPanel.cs
│   │   ├── ShopOffer.cs
│   │   ├── StarBalanceLabel.cs
│   │   └── ...
│   ├── TowerOfHanoi/
│   │   ├── HanoiGameManager.cs
│   │   ├── HanoiRod.cs
│   │   └── HanoiDisk.cs
│   ├── GameManagerScript.cs
│   ├── GameObjectsScript.cs
│   ├── DragAndDropScript.cs
│   └── ...
└── ...
```

### Main scripts

  -----------------------------------------------------------------------
  Script                              Responsibility
  ----------------------------------- -----------------------------------
  `StartMenuScript.cs`                Main menu navigation, audio and
                                      interactive menu actions

  `GameManagerScript.cs`              Car Puzzle timer, lives, win/lose
                                      state and time-based star rewards

  `GameObjectsScript.cs`              Car objects and placement-related
                                      game logic

  `DragAndDropScript.cs`              Dragging car UI objects

  `HanoiGameManager.cs`               Hanoi levels, legal moves, move
                                      count, stars, hints, undo and
                                      progression

  `HanoiRod.cs`                       Tracks disks on a rod and handles
                                      disk drop targets

  `HanoiDisk.cs`                      Disk initialization, dragging,
                                      movement animation and hint
                                      highlighting

  `StarBank.cs`                       Shared star balance, shop
                                      ownership, disk skin selection and
                                      helper tokens

  `ShopPanel.cs` / `ShopOffer.cs`     Shop panel, purchase handling and
                                      offer UI

  `AdManager.cs`                      Persistent advertisement management
                                      across scenes

  `AdsInitializer.cs`                 Unity Ads initialization

  `InterstitialAd.cs`                 Interstitial ad loading and display

  `RewardedAds.cs`                    Rewarded ad loading, display and
                                      completion callbacks

  `BannerAd.cs`                       Banner ad loading, positioning and
                                      visibility
  -----------------------------------------------------------------------

## How to run

1.  Install a compatible version of Unity that supports the packages
    used by the project.
2.  Open the project through Unity Hub.
3.  Allow Unity to import assets and resolve packages.
4.  Open `Assets/Scenes/StartMenu.unity`.
5.  Press **Play** to start from the main menu.

The scenes are named `StartMenu`, `CityScene` and `TowerOfHanoi`. Keep
these names consistent with the scene-loading references in the scripts.

For Android builds, configure the Android Build Support module and the
required Unity Ads settings. Verify the Android Game ID and ad unit IDs
in the relevant scripts and Unity Ads dashboard before making a
production build.

## Controls

### Car Puzzle

-   Drag cars with the mouse or touchscreen.
-   Place each car in its matching location.
-   Use the on-screen controls where available.

### Tower of Hanoi

-   Drag the top disk from one rod to another.
-   A move is rejected if it breaks the disk-size rule.
-   Use the Hint and Undo buttons when available.
-   Complete the current level to unlock the next one.

## Development notes

-   Player progress, star balance, purchases, disk skin selection, Hanoi
    unlocks and best level stars use `PlayerPrefs` and are stored
    locally on the device.
-   The project contains test shortcuts: `F1` forces a Car Puzzle win or
    a Hanoi level win, depending on the active game mode; `F2` adds 50
    stars. The main menu also contains a hidden duck interaction that
    awards test stars. Remove or disable these test mechanisms before a
    public release.
-   The project includes editor scripts under `Assets/Editor/`.
-   The game uses Unity UI and TextMeshPro components.

## Screenshots folder

Create this folder in the repository and add your screenshots using the
filenames referenced above:

``` text
docs/
└── screenshots/
    ├── main-menu.png
    ├── car-puzzle-gameplay.png
    ├── car-puzzle-results.png
    ├── hanoi-level-selection.png
    ├── hanoi-gameplay.png
    ├── hanoi-hint-undo.png
    ├── hanoi-results.png
    └── shop.png
```

------------------------------------------------------------------------
