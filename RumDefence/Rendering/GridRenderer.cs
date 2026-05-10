using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
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
                    int mapped = MapMaskToTile(grid.Tiles, x, y, mask);
                    texture = theme.GetTexture(mapped, x, y);
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

    private bool IsLandSafe(int[,] map, int x, int y)
    {
        int h = map.GetLength(0);
        int w = map.GetLength(1);

        if (x < 0 || x >= w || y < 0 || y >= h)
            return true;

        if (RumGame.Instance.CurrentLevel?.RumTile == new Point(x, y))
            return true;

        return map[y, x] == 1;
    }

    private bool IsLandDiag(int[,] map, int x, int y)
    {
        int h = map.GetLength(0);
        int w = map.GetLength(1);

        if (x < 0 || x >= w || y < 0 || y >= h)
            return false;

        if (RumGame.Instance.CurrentLevel?.RumTile == new Point(x, y))
            return true;

        return map[y, x] == 1;
    }

    private int GetMask(int[,] map, int x, int y)
    {
        int mask = 0;

        if (IsLandSafe(map, x, y - 1)) mask |= 1;
        if (IsLandSafe(map, x + 1, y)) mask |= 2;
        if (IsLandSafe(map, x - 1, y)) mask |= 4;
        if (IsLandSafe(map, x, y + 1)) mask |= 8;

        return mask;
    }

    private int MapMaskToTile(int[,] map, int x, int y, int mask)
    {
        bool top = (mask & 1) != 0;
        bool right = (mask & 2) != 0;
        bool left = (mask & 4) != 0;
        bool bottom = (mask & 8) != 0;

        bool diagTL = IsLandDiag(map, x - 1, y - 1);
        bool diagTR = IsLandDiag(map, x + 1, y - 1);
        bool diagBR = IsLandDiag(map, x + 1, y + 1);
        bool diagBL = IsLandDiag(map, x - 1, y + 1);

        // 🔥 binnenhoeken (mask = 15)
        if (top && right && left && bottom)
        {
            if (!diagTL) return 10;
            if (!diagTR) return 11;
            if (!diagBR) return 12;
            if (!diagBL) return 13;

            return 5;
        }

        bool wTop = !top;
        bool wRight = !right;
        bool wLeft = !left;
        bool wBottom = !bottom;

        // rechte randen
        if (wTop && !wRight && !wLeft && !wBottom) return 8;
        if (!wTop && wRight && !wLeft && !wBottom) return 6;
        if (!wTop && !wRight && !wLeft && wBottom) return 2;
        if (!wTop && !wRight && wLeft && !wBottom) return 4;

        // buitenhoeken
        if (wTop && wLeft) return 7;
        if (wTop && wRight) return 9;
        if (wBottom && wLeft) return 1;
        if (wBottom && wRight) return 3;

        return 5;
    }
}