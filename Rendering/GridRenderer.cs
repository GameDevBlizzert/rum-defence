using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class GridRenderer
{
    private ITileTheme theme;
    private Texture2D rumTexture;

    public GridRenderer(ITileTheme theme)
    {
        this.theme = theme;
        rumTexture = RumGame.Instance.Content.Load<Texture2D>("Art/Objects/RumBarrel");
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
    }
}