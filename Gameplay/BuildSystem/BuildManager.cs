using Microsoft.Xna.Framework;
using RumDefence;
using System;

public class BuildManager
{
    private Grid grid;
    private BuildMode currentMode = BuildMode.None; 
    private Point? hoveredTile;

    private Action<Point> onWallPlaced;

    public BuildManager(Grid grid)
    {
        this.grid = grid;
    }

    public void Update(Vector2 mousePosition, bool isClick)
    {
        if (currentMode == BuildMode.None)
        {
            hoveredTile = null; 
            return;
        }

        hoveredTile = grid.WorldToGrid(mousePosition);

        if (hoveredTile == null) return;

        HandleClick(isClick);
    }

    private void HandleClick(bool isClick)
    {
        if (!isClick) return;
        if (hoveredTile == null) return;

        var p = hoveredTile.Value;

        switch (currentMode)
        {
            case BuildMode.Wall:
                if (CanPlaceWall(p))
                {
                    onWallPlaced?.Invoke(p);
                }
                break;
        }
    }

    public Point? GetHoveredTile()
    {
        return hoveredTile;
    }

    public void SetMode(BuildMode mode)
    {
        if (currentMode == mode)
            currentMode = BuildMode.None;
        else
            currentMode = mode;
    }

    public BuildMode GetMode()
    {
        return currentMode;
    }

    private bool CanPlaceWall(Point p)
    {
        if (grid.Tiles[p.Y, p.X] != 5)
            return false;

        // later:
        // if (playerCoins < wallCost) return false;

        return true;
    }
    public void SetWallPlacementCallback(Action<Point> callback)
    {
        onWallPlaced = callback;
    }
}