using Microsoft.Xna.Framework;
using RumDefence;

public class BuildManager
{
    private Grid grid;
    private BuildMode currentMode = BuildMode.Wall; // tijdelijk direct actief

    private Point? hoveredTile;

    public BuildManager(Grid grid)
    {
        this.grid = grid;
    }

    public void Update(Vector2 mousePosition, bool isClick)
    {
        hoveredTile = grid.WorldToGrid(mousePosition);

        HandleClick(isClick);

        if (currentMode == BuildMode.None)
            return;

    }

    private void HandleClick(bool isClick)
    {
        if (!isClick) return;
        if (hoveredTile == null) return;

        var p = hoveredTile.Value;

        if (currentMode == BuildMode.Wall)
        {
            if (grid.Tiles[p.Y, p.X] == 5)
            {
                grid.Tiles[p.Y, p.X] = 99;
            }
        }
    }

    public Point? GetHoveredTile()
    {
        return hoveredTile;
    }

    public void SetMode(BuildMode mode)
    {
        currentMode = mode;
    }

    private bool CanPlaceWall(Point p)
    {
        if (grid.Tiles[p.Y, p.X] != 5)
            return false;

        // later:
        // if (playerCoins < wallCost) return false;

        return true;
    }
}