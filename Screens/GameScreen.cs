using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace RumDefence;

public class GameScreen : Screen
{
    private Grid grid;
    private GridRenderer renderer;
    private Level currentLevel;

    private ShipSpawner spawner;
    private List<Ship> ships = new();

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

        renderer = new GridRenderer(currentLevel.Theme);

        spawner = new ShipSpawner(currentLevel, grid);
    }

    public override void Update(GameTime gameTime)
    {
        var keyboard = Keyboard.GetState();

        if (keyboard.IsKeyDown(Keys.Escape))
        {
            manager.SetScreen(new PauseScreen(manager, this));
            return;
        }

        var newShip = spawner.Update(gameTime);
        if (newShip != null)
        {
            ships.Add(newShip);
        }

        for (int i = ships.Count - 1; i >= 0; i--)
        {
            ships[i].Update(gameTime);

            if (ships[i].IsFinished)
            {
                ships.RemoveAt(i);
            }
        }

        // 🔹 Level completion
        if (!levelCompleted && spawner.IsFinished && ships.Count == 0)
        {
            levelCompleted = true;
            UnlockNextLevel();
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        RumGame.Instance.GraphicsDevice.Clear(new Color(30, 144, 255));

        renderer.Draw(grid, spriteBatch);

        foreach (var ship in ships)
        {
            ship.Draw(spriteBatch);
        }
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