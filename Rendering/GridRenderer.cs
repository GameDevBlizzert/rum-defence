using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class GridRenderer
{
    private ITileTheme theme;
    private Texture2D rumTexture;
    private Texture2D pixel;

    private BuildManager buildManager;
    private Grid grid;

    public GridRenderer(ITileTheme theme, BuildManager buildManager, Grid grid)
    {
        this.theme = theme;
        this.buildManager = buildManager;
        this.grid = grid;


        rumTexture = RumGame.Instance.Content.Load<Texture2D>("Art/Objects/RumBarrel");

        pixel = new Texture2D(RumGame.Instance.GraphicsDevice, 1, 1);
        pixel.SetData(new[] { Color.White });
    }

    public void Draw(Grid grid, SpriteBatch spriteBatch)
    {
        var level = RumGame.Instance.CurrentLevel;

        for (int y = 0; y < grid.Height; y++)
        {
            for (int x = 0; x < grid.Width; x++)
            {
                var worldPos = grid.GridToWorld(new Point(x, y));
                var drawPos = worldPos - new Vector2(grid.TileSize / 2f);

                var texture = theme.GetTexture(grid.Tiles[y, x], x, y);

                if (texture != null)
                {
                    spriteBatch.Draw(
                        texture,
                        new Rectangle((int)drawPos.X, (int)drawPos.Y, grid.TileSize, grid.TileSize),
                        Color.White
                    );
                }

                if (level?.RumTile == new Point(x, y))
                {
                    spriteBatch.Draw(
                        rumTexture,
                        new Rectangle((int)drawPos.X, (int)drawPos.Y, grid.TileSize, grid.TileSize),
                        Color.White
                    );
                }
            }
        }

        var hovered = buildManager.GetHoveredTile();

        if (buildManager.GetMode() == BuildMode.None)
            return;

        if (hovered != null)
        {
            var p = hovered.Value;

            if (grid.Tiles[p.Y, p.X] == 5)
            {
                DrawHighlight(spriteBatch, p);
            }
        }
    }

    private void DrawHighlight(SpriteBatch spriteBatch, Point gridPos)
    {
        Vector2 world = grid.GridToWorld(gridPos);

        Rectangle rect = new Rectangle(
            (int)(world.X - grid.TileSize / 2),
            (int)(world.Y - grid.TileSize / 2),
            grid.TileSize,
            grid.TileSize
        );

        spriteBatch.Draw(pixel, rect, Color.Gray * 0.5f);
    }
}