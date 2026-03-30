using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Rum_Defence.Input;
using Rum_Defence.Entities.Towers;
using System;
using System.Collections.Generic;

namespace RumDefence;

public class GameScreen : Screen
{
    private Grid grid { get; set; }
    private GridRenderer renderer { get; set; }
    private Level currentLevel { get; set; }
    private BuildManager buildManager { get; set; }
    private InputManager input { get; set; }
    private Hud hud { get; set; }

    private Dictionary<Point, Wall> walls = new();
    private WallRenderer wallRenderer;
    private bool levelCompleted = false;

    public ShipSpawner Spawner { get; private set; }

    public List<Ship> Ships { get; private set; } = new();
    public List<Troop> Troops { get; private set; } = new();

    //remove when hud is done
    private List<CannonTower> testTowers;

    private bool levelCompleted;

    private LevelProgressSystem progress;

    public GameScreen(ScreenManager manager, Level level) : base(manager)
    {
        currentLevel = level;
    }

    public override void Load()
    {
        grid = new Grid(currentLevel.Map);

        RumGame.Instance.CurrentGrid = grid;
        RumGame.Instance.CurrentLevel = currentLevel;

        GridSystem.CalculateLayout(grid);

        input = new InputManager();
        buildManager = new BuildManager(grid);

        renderer = new GridRenderer(currentLevel.Theme.Tiles, buildManager, grid);

        spawner = new ShipSpawner(currentLevel, grid);

        hud = new Hud(buildManager);

        wallRenderer = new WallRenderer(
            grid,
            currentLevel.Theme.Walls,
            walls
        );

        buildManager.SetWallPlacementCallback(p =>
        {
            if (!walls.ContainsKey(p))
            {
                walls[p] = new Wall(p);
            }
        });

        //remove when hud is done now only spawn at level 3
        testTowers = currentLevel.Id == 3 ? new List<CannonTower>()
        {
            new (new Vector2(1500, 300), Troops),
            new (new Vector2(1500, 900), Troops),
            new (new Vector2(1700, 500), Troops),

        } : new();

        Spawner = new ShipSpawner(currentLevel, grid);
        progress = new(currentLevel.StartingLives, currentLevel.StartingCoinBalance);
    }

    public override void Update(GameTime gameTime)
    {
        if (HandlePause()) return;
        input.Update();
        UpdateBuildSystem(gameTime);
        UpdateSpawner(gameTime);
        UpdateShips(gameTime);
        UpdateTroops(gameTime);
        CheckLevelCompletion();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        RumGame.Instance.GraphicsDevice.Clear(new Color(30, 144, 255));

        renderer.Draw(grid, spriteBatch);

        wallRenderer.Draw(spriteBatch);

        foreach (var ship in ships)
            ship.Draw(spriteBatch);

        foreach (var troop in troops)
            troop.Draw(spriteBatch);

        hud.Draw(spriteBatch);
    }

    private void UnlockNextLevel()
    {
        int currentIndex = GrassLevels.All.IndexOf(currentLevel);

        if (currentIndex + 1 < GrassLevels.All.Count)
        {
            GrassLevels.All[currentIndex + 1].IsUnlocked = true;
        }
    }

    private void UpdateBuildSystem(GameTime gameTime)
    {
        hud.Update(gameTime);

        buildManager.Update(
            input.MousePositionScaled,
            input.IsLeftClick()
        );
    }

    private bool HandlePause()
    {
        var keyboard = Keyboard.GetState();

        if (keyboard.IsKeyDown(Keys.Escape))
        {
            manager.SetScreen(new PauseScreen(manager, this));
            return true;
        }

        return false;
    }

    private void UpdateSpawner(GameTime gameTime)
    {
        var newShip = spawner.Update(gameTime);

        if (newShip != null)
        {
            Ships.Add(newShip);
        }
    }

    private void UpdateShips(GameTime gameTime)
    {
        for (int i = ships.Count - 1; i >= 0; i--)
        {
            Ships[i].Update(gameTime);

            if (Ships[i].SpawnedTroops.Count > 0)
            {
                Troops.AddRange(Ships[i].SpawnedTroops);
                Ships[i].SpawnedTroops.Clear();
            }

            if (Ships[i].IsFinished)
            {
                Ships.RemoveAt(i);
            }
        }
    }

    private void UpdateTroops(GameTime gameTime)
    {
        for (int i = troops.Count - 1; i >= 0; i--)
        {
            var troop = Troops[i];
            troop.Update(gameTime);

            if (troop.IsFinished || troop.IsDead)
            {
                // TODO: Base the hits on the damage stat of the troop
                if (troop.IsFinished) progress.TakeHits(1);

                Troops.RemoveAt(i);
            }
        }
    }

    private void CheckLevelCompletion()
    {
        if (!levelCompleted && spawner.IsFinished && ships.Count == 0 && troops.Count == 0)
            progress.Update(gameTime, this);

        // TODO: Do not ignore IsLost after testing
        levelCompleted = progress.IsWon() /*|| progress.IsLost()*/;

        if (progress.IsWon())
        {
            UnlockNextLevel();
        }

        testTowers.ForEach(x => x.Update(gameTime));

        if (levelCompleted)
        {
            // TODO: Show win or lose screen based
            manager.SetScreen(new MainMenuScreen(manager));
        }

    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        RumGame.Instance.GraphicsDevice.Clear(new Color(30, 144, 255));

        renderer.Draw(grid, spriteBatch);

        foreach (var ship in Ships)
            ship.Draw(spriteBatch);

        foreach (var troop in Troops)
            troop.Draw(spriteBatch);


        testTowers.ForEach(x => x.Draw(spriteBatch));
    }

    private void UnlockNextLevel()
    {
        int currentIndex = GrassLevels.All.IndexOf(currentLevel);

        if (currentIndex + 1 < GrassLevels.All.Count)
        {
            GrassLevels.All[currentIndex + 1].IsUnlocked = true;
        }
    }
}