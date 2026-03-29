using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Rum_Defence.Input;
using System.Collections.Generic;

namespace RumDefence;

public class GameScreen : Screen
{
    private Grid grid { get; set; }
    private GridRenderer renderer { get; set; }
    private Level currentLevel { get; set; }
    private ShipSpawner spawner { get; set; }
    private BuildManager buildManager { get; set; }
    private InputManager input { get; set; }
    private Hud hud { get; set; }

    private List<Ship> ships = new();
    private List<Troop> troops = new();
    private bool levelCompleted = false;

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

        renderer = new GridRenderer(currentLevel.Theme, buildManager, grid);

        spawner = new ShipSpawner(currentLevel, grid);

        hud = new Hud(buildManager);
    }

    public override void Update(GameTime gameTime)
    {
        if (HandlePause()) return;

        UpdateBuildSystem();
        UpdateSpawner(gameTime);
        UpdateShips(gameTime);
        UpdateTroops(gameTime);
        input.Update();

        CheckLevelCompletion();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        RumGame.Instance.GraphicsDevice.Clear(new Color(30, 144, 255));

        renderer.Draw(grid, spriteBatch);

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

    private void UpdateBuildSystem()
    {
        input.Update();

        hud.Update(input.MousePositionScaled, input.IsLeftClick());

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
            ships.Add(newShip);
        }
    }

    private void UpdateShips(GameTime gameTime)
    {
        for (int i = ships.Count - 1; i >= 0; i--)
        {
            ships[i].Update(gameTime);

            if (ships[i].SpawnedTroops.Count > 0)
            {
                troops.AddRange(ships[i].SpawnedTroops);
                ships[i].SpawnedTroops.Clear();
            }

            if (ships[i].IsFinished)
            {
                ships.RemoveAt(i);
            }
        }
    }

    private void UpdateTroops(GameTime gameTime)
    {
        for (int i = troops.Count - 1; i >= 0; i--)
        {
            troops[i].Update(gameTime);

            if (troops[i].IsFinished || troops[i].IsDead)
            {
                troops.RemoveAt(i);
            }
        }
    }

    private void CheckLevelCompletion()
    {
        if (!levelCompleted && spawner.IsFinished && ships.Count == 0 && troops.Count == 0)
        {
            levelCompleted = true;
            UnlockNextLevel();
        }
    }
}