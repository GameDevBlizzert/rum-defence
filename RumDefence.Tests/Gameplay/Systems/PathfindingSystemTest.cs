using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Xunit;

namespace RumDefence.Tests.Gameplay.Systems;


[TestSubject(typeof(PathfindingSystem))]
public class PathfindingSystemTest
{

    [Fact]
    public void UpdatePath_WithCurrentPosition_ShouldUpdatePath()
    {
        // Arrange
        var destination = new Vector2(100, 100);
        var pathfindingSystem = new PathfindingSystem(destination);
        var currentPosition = new Vector2(50, 50);
        var grid = new Grid(new int[30, 30])
        {
            TileSize = 10
        };

        // Act
        pathfindingSystem.UpdatePath(currentPosition, grid, new HashSet<Point>());

        // Assert: the path leads to the destination tile (waypoints snap to grid coords).
        Assert.NotEmpty(pathfindingSystem.Path);
        Assert.True(Vector2.Distance(pathfindingSystem.Path.Last(), destination) <= grid.TileSize);
    }

}