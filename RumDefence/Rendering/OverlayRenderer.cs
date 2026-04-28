using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace RumDefence;

public class OverlayRenderer
{
    private Texture2D placeableOverlay;
    private Texture2D destroyableOverlay;
    private Grid grid;
    private BuildManager buildManager;
    private Dictionary<Point, bool> occupiedTiles;

    public OverlayRenderer(Grid grid, BuildManager buildManager, Dictionary<Point, bool> occupiedTiles)
    {
        this.grid = grid;
        this.buildManager = buildManager;
        this.occupiedTiles = occupiedTiles;

        var content = RumGame.Instance.Content;
        placeableOverlay = content.Load<Texture2D>("Art/Themes/Grass/Utilities/placeable");
        destroyableOverlay = content.Load<Texture2D>("Art/Themes/Grass/Utilities/destroyable");
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        var mode = buildManager.GetMode();
        var hovered = buildManager.GetHoveredTile();

        if (mode == BuildMode.Remove)
        {
            DrawDestroyableOverlay(spriteBatch, hovered);
        }
        else if (mode != BuildMode.None)
        {
            DrawPlaceableOverlays(spriteBatch);
        }
    }

    private void DrawDestroyableOverlay(SpriteBatch spriteBatch, Point? hoveredTile)
    {
        foreach (var occupiedTile in occupiedTiles.Keys)
        {
            Color tint = (hoveredTile == occupiedTile) ? Color.Red : Color.Red * 0.9f;
            DrawOverlayAtTile(spriteBatch, occupiedTile, destroyableOverlay, tint);
        }
    }

    private void DrawPlaceableOverlays(SpriteBatch spriteBatch)
    {
        for (int y = 0; y < grid.Height; y++)
        {
            for (int x = 0; x < grid.Width; x++)
            {
                var tile = grid.Tiles[y, x];
                if (tile == TileRules.Center && !IsOccupied(new Point(x, y)))
                {
                    DrawOverlayAtTile(spriteBatch, new Point(x, y), placeableOverlay, Color.White * 0.5f);
                }
            }
        }
    }

    private void DrawOverlayAtTile(SpriteBatch spriteBatch, Point gridPos, Texture2D overlay, Color tint)
    {
        Vector2 world = grid.GridToWorld(gridPos);
        Rectangle rect = new Rectangle(
            (int)(world.X - grid.TileSize / 2),
            (int)(world.Y - grid.TileSize / 2),
            grid.TileSize,
            grid.TileSize
        );

        spriteBatch.Draw(overlay, rect, tint);
    }

    private bool IsOccupied(Point tile)
    {
        return occupiedTiles.ContainsKey(tile);
    }
}
