using Microsoft.Xna.Framework;
using RumDefence;
using System;

public class BuildManager
{
    private Grid grid;
    private BuildMode currentMode = BuildMode.None;
    private Point? hoveredTile;

    private Action<Point> onWallPlaced;
    private Action<Point> onRemove;
    private Action<Point> onMusketTowerPlaced;
    private Action<Point> onCannonTowerPlaced;

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
                    onWallPlaced?.Invoke(p);
                break;
            case BuildMode.CannonTower:
                if (CanPlaceTower(p))
                    onTowerPlaced?.Invoke(p);
                break;
            case BuildMode.Remove:
                onRemove?.Invoke(p);
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

    private bool CanPlaceTower(Point p)
    {
        if (grid.Tiles[p.Y, p.X] != TileRules.Center)
            return false;

        return true;
    }

    private bool CanPlaceTower(Point p)
    {
        return TileRules.IsLand(grid.Tiles[p.Y, p.X]);
    }

    public void SetWallPlacementCallback(Action<Point> callback)
    {
        onWallPlaced = callback;
    }


    public void SetRemoveCallback(Action<Point> callback)
    {
        onRemove = callback;
    }

    public void SetMusketTowerPlacementCallback(Action<Point> callback)
    {
        onMusketTowerPlaced = callback;
    }

    public void SetCannonTowerPlacementCallback(Action<Point> callback)
    {
        onCannonTowerPlaced = callback;
    }
}