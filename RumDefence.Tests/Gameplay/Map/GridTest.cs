// AI generated: this test file was created with AI assistance.
using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Xunit;

namespace RumDefence.Tests.Gameplay.Map;

[TestSubject(typeof(Grid))]
public class GridTest
{
    // 1. Width/Height reflect the array dimensions (non-square to prove GetLength convention)
    [Fact]
    public void WidthAndHeight_NonSquareArray_ReflectCorrectDimensions()
    {
        // 3 rows, 5 cols => Height=3, Width=5
        var grid = new Grid(new int[3, 5]) { TileSize = 10 };

        Assert.Equal(5, grid.Width);
        Assert.Equal(3, grid.Height);
    }

    // 2. GridToWorld returns the tile center (no offset)
    [Theory]
    [InlineData(0, 0, 5f, 5f)]
    [InlineData(2, 3, 25f, 35f)]
    public void GridToWorld_NoOffset_ReturnsTileCenter(int tileX, int tileY, float expectedX, float expectedY)
    {
        var grid = new Grid(new int[30, 30]) { TileSize = 10 };

        var world = grid.GridToWorld(new Point(tileX, tileY));

        Assert.Equal(new Vector2(expectedX, expectedY), world);
    }

    // 2b. GridToWorld with non-zero offset shifts the result
    [Fact]
    public void GridToWorld_WithOffset_ShiftsResultByOffset()
    {
        var grid = new Grid(new int[30, 30])
        {
            TileSize = 10,
            Offset = new Vector2(100f, 200f)
        };

        var world = grid.GridToWorld(new Point(1, 2));

        // tile (1,2) => center at (15, 25) + offset (100, 200) = (115, 225)
        Assert.Equal(new Vector2(115f, 225f), world);
    }

    // 3. WorldToGrid returns the correct tile for in-bounds positions
    [Theory]
    [InlineData(5f, 5f, 0, 0)]
    [InlineData(25f, 35f, 2, 3)]
    [InlineData(19f, 9f, 1, 0)]
    public void WorldToGrid_InBounds_ReturnsCorrectTile(float worldX, float worldY, int expectedX, int expectedY)
    {
        var grid = new Grid(new int[30, 30]) { TileSize = 10 };

        var tile = grid.WorldToGrid(new Vector2(worldX, worldY));

        Assert.Equal(new Point(expectedX, expectedY), tile);
    }

    // 4. WorldToGrid returns null for out-of-bounds positions
    [Theory]
    [InlineData(-10f, 5f)]  // negative X (must be at least -TileSize to get x < 0 after int cast)
    [InlineData(5f, -10f)]  // negative Y
    [InlineData(300f, 5f)]  // X beyond Width*TileSize
    [InlineData(5f, 300f)]  // Y beyond Height*TileSize
    public void WorldToGrid_OutOfBounds_ReturnsNull(float worldX, float worldY)
    {
        var grid = new Grid(new int[30, 30]) { TileSize = 10 };

        var tile = grid.WorldToGrid(new Vector2(worldX, worldY));

        Assert.Null(tile);
    }

    // 5. Round-trip: WorldToGrid(GridToWorld(tile)) == tile
    [Theory]
    [InlineData(0, 0)]
    [InlineData(5, 7)]
    [InlineData(29, 29)]
    public void WorldToGrid_AfterGridToWorld_ReturnsSameTile(int tileX, int tileY)
    {
        var grid = new Grid(new int[30, 30]) { TileSize = 10 };
        var originalTile = new Point(tileX, tileY);

        var worldPos = grid.GridToWorld(originalTile);
        var roundTripped = grid.WorldToGrid(worldPos);

        Assert.Equal(originalTile, roundTripped);
    }

    // 6. GetTileCost returns 10 for untraversable tiles, 1 for traversable
    [Fact]
    public void GetTileCost_TileInUntraversableSet_Returns10()
    {
        var grid = new Grid(new int[30, 30]) { TileSize = 10 };
        var blocked = new Point(3, 4);
        var untraversable = new HashSet<Point> { blocked };

        Assert.Equal(10, grid.GetTileCost(blocked, untraversable));
    }

    [Fact]
    public void GetTileCost_TileNotInUntraversableSet_Returns1()
    {
        var grid = new Grid(new int[30, 30]) { TileSize = 10 };
        var untraversable = new HashSet<Point> { new Point(3, 4) };

        Assert.Equal(1, grid.GetTileCost(new Point(0, 0), untraversable));
    }

    // 7. GetTilesOnLine for a straight horizontal line returns ordered tile list
    [Fact]
    public void GetTilesOnLine_HorizontalLine_ReturnsOrderedTiles()
    {
        var grid = new Grid(new int[30, 30]) { TileSize = 10 };

        // Tile (0,2) to tile (4,2) — horizontal, same row
        var start = grid.GridToWorld(new Point(0, 2));
        var end = grid.GridToWorld(new Point(4, 2));

        var tiles = grid.GetTilesOnLine(start, end);

        Assert.Equal(5, tiles.Count);
        Assert.Equal(new Point(0, 2), tiles[0]);
        Assert.Equal(new Point(4, 2), tiles[^1]);
    }

    // 8. GetTilesOnLine throws ArgumentException when a point is out of bounds
    [Fact]
    public void GetTilesOnLine_StartOutOfBounds_ThrowsArgumentException()
    {
        var grid = new Grid(new int[30, 30]) { TileSize = 10 };
        // -10f / 10 = -1, which is < 0, so WorldToGrid returns null => ArgumentException
        var outOfBounds = new Vector2(-10f, 5f);
        var inBounds = grid.GridToWorld(new Point(2, 2));

        Assert.Throws<ArgumentException>(() => grid.GetTilesOnLine(outOfBounds, inBounds));
    }

    [Fact]
    public void GetTilesOnLine_EndOutOfBounds_ThrowsArgumentException()
    {
        var grid = new Grid(new int[30, 30]) { TileSize = 10 };
        var inBounds = grid.GridToWorld(new Point(2, 2));
        var outOfBounds = new Vector2(500f, 5f);

        Assert.Throws<ArgumentException>(() => grid.GetTilesOnLine(inBounds, outOfBounds));
    }
}
