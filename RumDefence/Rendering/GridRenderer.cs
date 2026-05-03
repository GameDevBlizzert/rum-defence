using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace RumDefence;

public class GridRenderer
{
    private ITileTheme theme;
    private Texture2D pixel;

    private BuildManager buildManager;
    private Grid grid;
    private OverlayRenderer overlayRenderer;

    public GridRenderer(ITileTheme theme, BuildManager buildManager, Grid grid)
    {
        this.theme = theme;
        this.buildManager = buildManager;
        this.grid = grid;

        pixel = new Texture2D(RumGame.Instance.GraphicsDevice, 1, 1);
        pixel.SetData(new[] { Color.White });
    }

    public void SetOccupiedTiles(Dictionary<Point, bool> occupiedTiles)
    {
        overlayRenderer = new OverlayRenderer(grid, buildManager, occupiedTiles);
    }

    public OverlayRenderer GetOverlayRenderer()
    {
        return overlayRenderer;
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

                int tile = grid.Tiles[y, x];
                Texture2D texture;

                if (tile == 0)
                {
                    texture = theme.GetTexture(0, x, y);
                }
                else
                {
                    int mask = GetMask(grid.Tiles, x, y);

                    texture = theme.GetTexture(MapMaskToTile(mask), x, y);
                }

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
                    level.RumBarrel.Draw(spriteBatch);
                }
            }
        }

        var hovered = buildManager.GetHoveredTile();

        if (buildManager.GetMode() == BuildMode.None)
            return;

        if (hovered != null)
        {
            var p = hovered.Value;

            if (grid.Tiles[p.Y, p.X] == 1)
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

    private int GetMask(int[,] map, int x, int y)
    {
        int h = map.GetLength(0);
        int w = map.GetLength(1);

        bool IsLand(int px, int py)
        {
            if (px < 0 || px >= w || py < 0 || py >= h)
                return true;

            return map[py, px] == 1;
        }

        int mask = 0;

        if (IsLand(x, y - 1)) mask |= 1;
        if (IsLand(x + 1, y)) mask |= 2;
        if (IsLand(x, y + 1)) mask |= 8;
        if (IsLand(x - 1, y)) mask |= 4;

        return mask;
    }

    private int MapMaskToTile(int mask)
    {
        if (mask == 15) return 5;

        if (mask == 14) return 2;
        if (mask == 13) return 6;
        if (mask == 11) return 8;
        if (mask == 7) return 4;

        if (mask == 10) return 3;
        if (mask == 12) return 1;
        if (mask == 2) return 9;
        if (mask == 4) return 7;

        if (mask == 3) return 10;
        if (mask == 6) return 11;
        if (mask == 9) return 12;
        if (mask == 12) return 13;

        return 5;
    }
}