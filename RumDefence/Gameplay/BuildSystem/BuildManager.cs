using Microsoft.Xna.Framework;
using RumDefence;
using System;

public class BuildManager
{
    public const int WallCost = 10;

    private Grid grid;
    private Point targetTile;
    private BuildMode currentMode = BuildMode.None;
    private Point? hoveredTile;

    private Action<Point> onWallPlaced;
    private Action<Point> onMusketTowerPlaced;
    private Action<Point> onCannonTowerPlaced;
    private Action<Point> onFisherTowerPlaced;
    private Action<Point> onRemove;
    private Action<Point> onSelect;

    public BuildManager(Grid grid, Point targetTile)
    {
        this.grid = grid;
        this.targetTile = targetTile;
    }

    public bool CtrlHeld { get; private set; }

    public void Update(Vector2 mousePosition, bool isClick, bool ctrlHeld = false)
    {
        CtrlHeld = ctrlHeld;
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
                if (CanPlace(p))
                {
                    onWallPlaced?.Invoke(p);
                }
                break;
            case BuildMode.MusketTower:
                if (CanPlace(p))
                    onMusketTowerPlaced?.Invoke(p);
                break;
            case BuildMode.CannonTower:
                if (CanPlace(p))
                    onCannonTowerPlaced?.Invoke(p);
                break;
            case BuildMode.FisherTower:
                if (CanPlace(p))
                    onFisherTowerPlaced?.Invoke(p);
                break;
            case BuildMode.Remove:
                onRemove?.Invoke(p);
                break;
            case BuildMode.None:
                onSelect?.Invoke(p);
                break;
        }
    }

    public Point? GetHoveredTile()
    {
        return hoveredTile;
    }

    public bool DiagonalMode { get; private set; } = false;

    public void ToggleDiagonalMode()
    {
        DiagonalMode = !DiagonalMode;
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

    private bool CanPlace(Point p)
    {
        var map = grid.Tiles;

        if (!TileRules.IsLand(map[p.Y, p.X]))
            return false;

        if (TileRules.IsCoast(map, p.X, p.Y))
            return false;

        if (p == targetTile)
            return false;

        return true;
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

    public void SetFisherTowerPlacementCallback(Action<Point> callback)
    {
        onFisherTowerPlaced = callback;
    }

    public void SetSelectCallback(Action<Point> callback)
    {
        onSelect = callback;
    }
}