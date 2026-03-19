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
        renderer = new GridRenderer(currentLevel.Theme);

        spawner = new ShipSpawner(currentLevel);
    }

    public override void Update(GameTime gameTime)
    {
        var keyboard = Keyboard.GetState();

        // 🔥 pause
        if (keyboard.IsKeyDown(Keys.Escape))
        {
            manager.SetScreen(new PauseScreen(manager, this));
            return;
        }

        // 🔥 spawn ships (wave system)
        var newShip = spawner.Update(gameTime);

        if (newShip != null)
        {
            ships.Add(newShip);
        }

        // 🔥 update ships
        for (int i = ships.Count - 1; i >= 0; i--)
        {
            ships[i].Update(gameTime);

            // optioneel: later pirates hier spawnen
            if (ships[i].HasArrived)
            {
                ships.RemoveAt(i);
            }
        }

        // 🔥 level completion check
        if (!levelCompleted && spawner.IsFinished && ships.Count == 0)
        {
            levelCompleted = true;

            UnlockNextLevel();
        }
    }

    public override void Draw(SpriteBatch spriteBatch, Matrix scale)
    {
        RumGame.Instance.GraphicsDevice.Clear(new Color(30, 144, 255));

        spriteBatch.Begin(transformMatrix: scale);

        renderer.Draw(grid, spriteBatch);

        // 🔥 draw ships
        foreach (var ship in ships)
        {
            ship.Draw(spriteBatch);
        }

        spriteBatch.End();
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