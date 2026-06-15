# Rum Defence — Architecture

> **Note:** This document was generated with AI assistance based on the source
> code. 

This document explains how the codebase is organised and how the major systems
fit together. It is aimed at developers who want to find their way around the
project or extend it. For build/run and credits, see [README.md](README.md).

## Tech stack

- **Language / runtime:** C# on **.NET 8**.
- **Framework:** **MonoGame** (`MonoGame.Framework.DesktopGL` 3.8.4). The game is
  a classic MonoGame `Game` subclass with an `Update`/`Draw` loop.
- **Content:** built by `MonoGame.Content.Builder.Task` from `Content/Content.mgcb`.
- **Tests:** xUnit v3 in the sibling `RumDefence.Tests` project (see [Testing](#testing)).

## Big picture

```
Program.Main
  └─ RumGame (MonoGame Game)        single window, fixed virtual resolution, global services
       └─ ScreenManager             holds exactly one active Screen
            └─ Screen               MainMenu / LevelSelect / GameScreen / Pause / GameOver / …
                 └─ GameScreen      the actual tower-defence gameplay (the orchestrator)
```

The game is **screen-driven**: at any moment exactly one `Screen` is active and
owns Update/Draw. Gameplay lives entirely inside `GameScreen`, which wires
together the grid, build system, spawner, entities, HUD and progress tracking.

## Entry point & game loop

- [`Program.cs`](Program.cs) — `Main` creates and runs a single `RumGame`.
- [`RumGame.cs`](RumGame.cs) — the MonoGame `Game`:
  - Exposes a global `RumGame.Instance` singleton plus `CurrentGrid` and
    `CurrentLevel` so any system can reach the active map without threading it
    through constructors.
  - Renders at a **fixed virtual resolution** of `1920×1080` (`VirtualWidth/Height`).
    A `scaleMatrix` maps that virtual space onto the real (resizable) back-buffer,
    so all gameplay code can work in virtual pixels regardless of window size.
  - Loads the save file, sets audio volumes, and hands control to a
    `LoadingSplashScreen` in `Initialize`.
  - `Update` pauses the game (and music) when the window loses focus — overridable
    for debugging with the `DISABLE_PAUSE_ON_BLUR` env var.

### Coordinate spaces

There are three coordinate spaces; mixing them up is the most common source of bugs:

1. **Screen pixels** — raw mouse/window coordinates.
2. **Virtual pixels (world space)** — the `1920×1080` space everything is drawn in.
   `ScreenManager.GetMousePositionScaled` / `InputManager.MousePositionScaled`
   convert mouse input into this space.
3. **Grid tiles** — integer `(x, y)` tile coordinates. Convert with
   `Grid.WorldToGrid` / `Grid.GridToWorld` (see [Map & grid](#map--grid)).

## Screens

[`Screen`](Screens/Screen.cs) is the base class; [`ScreenManager`](Screens/ScreenManager.cs)
keeps a single `currentScreen`, calls `Load()` once on first activation, and
forwards `Update`/`Draw`. Switching screens is just `manager.SetScreen(new SomeScreen(...))`.

Notable screens ([Screens/](Screens/)):

| Screen | Role |
| --- | --- |
| `LoadingSplashScreen` | First screen; preloads then goes to the main menu. |
| `MainMenuScreen` | Title screen and entry to level/theme selection and settings. |
| `ThemeSelectScreen` / `LevelSelectScreen` | Pick a theme (Grass/Ghost/…) and a level. |
| `GameScreen` | The gameplay itself (see below). |
| `PauseScreen` | Overlay shown on pause or focus-loss; can resume/retry/quit. |
| `GameOverScreen` | Win/lose screen with wave + coin score. |
| `SettingsScreen` / `KeyBindingsScreen` | Audio volumes and rebindable keys. |

## The gameplay loop — `GameScreen`

[`GameScreen`](Screens/GameScreen.cs) is the heart of the game and the best file
to read first. On `Load` it builds the grid from the level map, then constructs
and wires the gameplay services. Each frame `Update` runs roughly:

1. `HandlePause` / playback shortcuts (pause, fast-forward).
2. `UpdateBuildSystem` — feed mouse input to the `BuildManager`, drive the HUD,
   and apply build/upgrade/repair interactions.
3. **Time-scaled gameplay** — `GetGameplayGameTime` multiplies the frame delta by
   the playback speed (`Normal`=1×, `FastForward`=2×, `Paused`=0×), so the same
   gameplay code supports fast-forward without special-casing.
4. Update walls → towers → spawner → ships → troops.
5. `CheckLevelCompletion` — win/lose detection.

Key collections it owns: `Ships`, `Troops`, `placedTowers`, `walls`, plus visual
effect lists (`Explosions`, `NetEffects`, `FireEffects`, `FlameEffects`).
`GameScreen.Instance` exposes the active screen to entities that need it.

**Build callbacks.** `GameScreen` registers callbacks on the `BuildManager`
(`SetWallPlacementCallback`, `SetTowerPlacementCallback`, `SetRemoveCallback`,
`SetSelectCallback`). The `BuildManager` decides *which tile* and *what action*;
`GameScreen` performs the actual placement, spends/refunds coins via
`LevelProgressSystem`, and updates `occupiedTiles`.

## Map & grid

Maps are authored as text (rows of space-separated tokens) and parsed in
[`Level.ParseMap`](Gameplay/Levels/Level.cs):

- Tile encoding ([`TileRules`](Gameplay/Map/TileRules.cs)): `0` = **Water**, `1` = **Land**.
- The token `#` marks the **Rum barrel tile** (the thing you defend) and is stored
  as land. Its position is exposed as `Level.RumTile`.
- The parsed result is an `int[,]` indexed `[row, col]` i.e. `[y, x]`.

[`Grid`](Gameplay/Map/Grid.cs) wraps that array and is **pure logic** (no graphics):

- **Dimension convention:** `Width = Tiles.GetLength(1)` (columns),
  `Height = Tiles.GetLength(0)` (rows). Keep this in mind — it is the opposite of
  what `[width, height]` array construction would suggest.
- `GridToWorld(Point)` returns the **centre** of a tile in world space;
  `WorldToGrid(Vector2)` returns the containing tile or `null` if out of bounds.
- `GetTileCost` / `GetTilesOnLine` (Bresenham) support pathfinding and path smoothing.
- `UntraversableTiles` holds the tiles troops must route around (walls + water),
  recomputed by `GameScreen.GetUntraversableTiles`.

[`GridSystem`](Gameplay/Systems/GridSystem.cs) computes the on-screen layout
(tile size / offset) so the map is centred for the level.
[`CoastSystem`](Gameplay/Systems/CoastSystem.cs) / [`DockSystem`](Gameplay/Systems/DockSystem.cs)
identify shoreline tiles (`TileRules.IsCoast`) and ship spawn/dock positions.

## Pathfinding

Each troop owns a [`PathfindingSystem`](Gameplay/Systems/PathfindingSystem.cs)
that routes from its current tile to the level destination.

- **Algorithm — A\*** on the tile grid. Normal tiles cost `1`; untraversable tiles
  cost `10` (`Grid.GetTileCost`) so routes avoid them but can still cross if there
  is no alternative. The heuristic is **Manhattan distance** (admissible → optimal).
- **Replanning** happens when the set of untraversable tiles changes (wall placed/
  removed). `GameScreen.UpdateTroops` diffs the untraversable set each frame and
  only calls `troop.UpdatePathfinding()` when it actually changed (or the troop is
  newly spawned), avoiding per-frame re-searches.
- **Path representation:** a `Queue<Vector2>` of world-space waypoints (tile centres).
  `GetNextDirection` pops the front waypoint once the troop is within 5px and
  returns a normalised movement direction. When `UntraversableTiles` is supplied,
  the path is also **smoothed** (collinear/clear waypoints removed via `GetTilesOnLine`).

## Entities

[`Entity`](Core/Entity.cs) is the abstract base for anything drawn in the world:
it holds `Position`, `Size`, a `Texture`, rotation/scale, and a default
sprite-batch `Draw`. `ApplySize` uses [`SizeSystem`](Gameplay/Systems/SizeSystem.cs)
to convert a desired tile-based size into a texture scale.

- **Towers** ([Entities/Towers/](Entities/Towers/)) — `BaseTower` plus concrete
  `MusketTower`, `CannonTower`, `FisherTower`, `FireTower`, `BanditTower`. They
  acquire targets according to an [`AttackMode`](Entities/Towers/AttackMode.cs)
  (e.g. *Closest to Rum*), fire projectiles, and support upgrades.
- **Troops** ([Entities/Troops/](Entities/Troops/)) — `Troop : Entity, ICollidable`
  with subtypes `GruntTroop`, `GhostTroop`, `BomberTroop`, `BossTroop`. Each has a
  `HealthComponent` and a `PathfindingSystem`; reaching the rum barrel damages it.
- **Ships** ([Entities/Ships/](Entities/Ships/)) — carry troops from the coast and
  release them (`SpawnedTroops`) over time.
- **Walls** ([Entities/Walls/](Entities/Walls/)) — player-built obstacles with health
  that reroute troops; can be repaired and upgraded.
- **Modifiers** ([Entities/Modifiers/](Entities/Modifiers/)) — status effects applied
  to troops (`BurnModifier`, `PoisonModifier`, `SpeedModifier`, `AttackSpeedModifier`)
  via the `IModifier` interface; they tick down over time and can be refreshed.

[`HealthComponent`](Core/HealthComponent.cs) is a small reusable health/damage
helper shared by troops, walls and the rum barrel.

## Data-driven design: factories & records

Tower and troop *stats* are plain immutable `record` data, kept separate from the
entity *behaviour*:

- [`TowerFactory`](Gameplay/Factories/TowerFactory.cs) defines a `TowerData` record
  (range, fire rate, damage, cost, upgrade deltas, sprite info, …) and one static
  instance per `TowerType`. `TowerFactory.Create(data, location, troops)` switches
  on `TowerType` to construct the right `BaseTower` subclass.
- [`TroopFactory`](Gameplay/Factories/TroopFactory.cs) does the same for troops via
  `TroopData` (used heavily by infinite-wave scaling).

Because stats are records, variants are cheap: e.g. infinite waves do
`TroopFactory.Regular with { Health = scaledHp }`. To add a new tower you add a
`TowerData` + a `BaseTower` subclass + a `case` in `Create`.

## Spawning & waves

A level's enemies are described as a `List<Wave>`:

- [`Wave`](Gameplay/Waves/Wave.cs) = a list of `ShipGroup`s plus min/max spawn timing.
- [`ShipGroup`](Gameplay/Waves/ShipGroup.cs) = a ship type, a count, and the troop
  groups each ship carries.
- [`ShipSpawner`](Gameplay/Spawning/ShipSpawner.cs) drives spawning: it queues the
  current wave's ships, releases them on a randomised interval between
  `MinSpawnTime`/`MaxSpawnTime`, assigns each to the nearest free coast tile, and
  advances to the next wave once **all ships are spawned and all troops defeated**.
  It also tracks per-wave progress (`WaveTroopProgress`) for the HUD.
- **Infinite mode:** if a level has an [`InfiniteWaveConfig`](Gameplay/Waves/InfiniteWaveConfig.cs),
  the spawner generates a fresh wave each time the queue empties.
  `GenerateWave(generationIndex)` scales ship/troop counts and HP multiplicatively
  per wave (with caps and spawn-time floors) and injects ghost/bomber/boss groups
  on fixed wave intervals.

[`SpawnSystem`](Gameplay/Systems/SpawnSystem.cs) and the spawners in
[Gameplay/Spawning/](Gameplay/Spawning/) build the concrete `Ship`/`Troop` instances.

## Levels & themes

[`Level`](Gameplay/Levels/Level.cs) bundles a map, a `Theme`, its waves, starting
coins/lives, an optional `InfiniteWaveConfig`, and a `SaveKey` used for progress.
Levels are grouped into **theme sets** ([Gameplay/Levels/](Gameplay/Levels/)):
`Dev`, `Grass`, `Ghost`, `Infinity`, `OneHp`. Each theme provides tile/wall art via
[`ITileTheme`](Gameplay/Levels/ITileTheme.cs) / [`IWallTheme`](Gameplay/Levels/IWallTheme.cs)
(`BaseTheme`, `Theme`).

The Grass theme additionally has a progressive **tower-unlock** flow
([`TowerUnlockManager`](Gameplay/Levels/Grass/TowerUnlockManager.cs) +
`GrassTowerUnlockSchedule`) that reveals towers wave-by-wave and grants the first
placement free.

## Build system

[`BuildManager`](Gameplay/BuildSystem/BuildManager.cs) tracks the current
[`BuildMode`](Gameplay/BuildSystem/BuildMode.cs) (none / wall / tower / remove /
select) and converts mouse input into tile actions, firing the callbacks
`GameScreen` registered. It supports modifier keys (Ctrl to keep placing) and a
diagonal-placement toggle. Costs and refunds (`WallCost`, `Primitives.RefundBuildingPrc`)
are applied by `GameScreen` against `LevelProgressSystem`.

## Progress, persistence, input, audio

- [`LevelProgressSystem`](Gameplay/Systems/LevelProgressSystem.cs) — per-level state:
  lives, coins (add/spend with `InsufficientBalanceException`), win/lose checks, and
  unlocking the next level on a win.
- [`SaveManager`](Gameplay/Save/SaveManager.cs) — **static** persistence to
  `%AppData%/RumDefence/save.json` ([`SaveData`](Gameplay/Save/SaveData.cs)): unlocked
  levels, best scores, unlocked towers, encountered troops, volumes and key bindings.
  All file IO is best-effort (wrapped in try/catch).
- [`InputManager`](Input/InputManager.cs) — a singleton that exposes scaled mouse
  position and named, rebindable actions (`IsActionJustPressed("Pause")`, etc.),
  with defaults overridable from the save file.
- [`AudioManager`](Audio/AudioManager.cs) — a singleton for music and SFX, honouring
  the saved music/SFX volumes and suspending on focus loss.

## Rendering

Rendering helpers live in [Rendering/](Rendering/): `GridRenderer` (tiles + build
overlay), `WallRenderer`, `MiniMapRenderer`, `OverlayRenderer`, `Animation`, and
`Camera2D`. `GameScreen.Draw` composites the world (grid → walls → ships → troops →
towers → decorations → effects), then the HUD and any active overlays.
UI widgets (HUD, coin balance, nine-slice boxes, tutorial) are under [UI/](UI/).

## Game-loop systems

Several systems implement [`IGameLoopSystem`](Gameplay/Systems/IGameLoopSystem.cs)
(`Load` + `Update`). Most gameplay systems live in [Gameplay/Systems/](Gameplay/Systems/):
`GridSystem`, `CoastSystem`, `DockSystem`, `TileSystem`, `SizeSystem`, `SpawnSystem`,
`PathfindingSystem`, `LevelProgressSystem`. They are deliberately small and mostly
free of graphics dependencies, which is what makes them unit-testable.

## Testing

Unit tests live in the sibling **`RumDefence.Tests`** project (xUnit v3, referenced
by [`RumDefence.sln`](RumDefence.sln)). They cover the pure, deterministic logic:
colliders (`CircleCollider`, `LinePieceCollider`, `RectangleCollider`),
`HealthComponent`, `Grid`, `LevelProgressSystem` and `PathfindingSystem`.

Because the project uses xUnit v3 (Microsoft Testing Platform), run the suite with:

```
dotnet run --project ../RumDefence.Tests/RumDefence.Tests.csproj
```

(Classic `dotnet test` reports "no tests" for this runner.) Tests are deliberately
focused on graphics-free logic; anything that needs `RumGame.Instance`, a
`GraphicsDevice` or a `Texture2D` is generally not unit-tested.

## Directory map

| Path | Contents |
| --- | --- |
| [`Program.cs`](Program.cs), [`RumGame.cs`](RumGame.cs) | Entry point and MonoGame game loop. |
| [Core/](Core/) | `Entity`, `HealthComponent`, shared base types. |
| [Collision/](Collision/) | Collider shapes (`Circle`, `Rectangle`, `LinePiece`) + `ICollidable`. |
| [Screens/](Screens/) | All screens and the `ScreenManager`. |
| [Gameplay/Map/](Gameplay/Map/) | `Grid`, `TileRules`, decorations, coast tiles. |
| [Gameplay/Systems/](Gameplay/Systems/) | Game-loop systems (pathfinding, progress, spawn, …). |
| [Gameplay/Waves/](Gameplay/Waves/) | `Wave`, `ShipGroup`, infinite-wave config. |
| [Gameplay/Spawning/](Gameplay/Spawning/) | Ship/troop spawners. |
| [Gameplay/Factories/](Gameplay/Factories/) | `TowerData`/`TroopData` records + factories, `Primitives`. |
| [Gameplay/Levels/](Gameplay/Levels/) | Levels, themes and per-theme content. |
| [Gameplay/BuildSystem/](Gameplay/BuildSystem/) | `BuildManager`, `BuildMode`. |
| [Gameplay/Save/](Gameplay/Save/) | `SaveManager`, `SaveData`. |
| [Entities/](Entities/) | Towers, troops, ships, walls, projectiles, modifiers, effects. |
| [Rendering/](Rendering/) | Renderers, camera, animation. |
| [UI/](UI/) | HUD and UI widgets. |
| [Audio/](Audio/), [Input/](Input/) | Audio and input singletons. |
| [Content/](Content/) | Art/audio assets and the `.mgcb` content pipeline. |

## Debugging

Environment variables that change behaviour:

| Variable | Effect |
| --- | --- |
| `SHOW_PATHFINDING` | Visualise computed paths and waypoints. |
| `DISABLE_PAUSE_ON_BLUR` | Don't auto-pause when the window loses focus (useful under a debugger). |
