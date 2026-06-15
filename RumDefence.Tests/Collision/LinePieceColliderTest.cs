// AI generated: this test file was created with AI assistance.
using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Xunit;
using RumDefence;

namespace RumDefence.Tests.Collision;

[TestSubject(typeof(LinePieceCollider))]
public class LinePieceColliderTest
{
    // ── Length ───────────────────────────────────────────────────────────────

    [Fact]
    public void Length_Get_3_4_5Triangle_Returns5()
    {
        var collider = new LinePieceCollider(new Vector2(0, 0), new Vector2(3, 4));

        Assert.True(Math.Abs(collider.Length - 5f) < 1e-3f);
    }

    // ── StandardA / B / C ────────────────────────────────────────────────────

    [Fact]
    public void StandardABC_HorizontalLineFromOriginTo1_0_ReturnsZeroMinusOneZero()
    {
        // Line from (0,0) to (1,0): A = End.Y - Start.Y = 0-0 = 0
        //                           B = Start.X - End.X  = 0-1 = -1
        //                           C = End.X*Start.Y - Start.X*End.Y = 1*0 - 0*0 = 0
        var collider = new LinePieceCollider(new Vector2(0, 0), new Vector2(1, 0));

        Assert.True(Math.Abs(collider.StandardA - 0f) < 1e-3f);
        Assert.True(Math.Abs(collider.StandardB - (-1f)) < 1e-3f);
        Assert.True(Math.Abs(collider.StandardC - 0f) < 1e-3f);
    }

    // ── GetDirection ─────────────────────────────────────────────────────────

    [Fact]
    public void GetDirection_FromOriginToRightAlongX_ReturnsUnitX()
    {
        Vector2 direction = LinePieceCollider.GetDirection(new Vector2(0, 0), new Vector2(10, 0));

        Assert.True(Math.Abs(direction.X - 1f) < 1e-3f);
        Assert.True(Math.Abs(direction.Y - 0f) < 1e-3f);
    }

    // ── NearestPointOnLine ───────────────────────────────────────────────────

    [Theory]
    // Point directly above the midpoint of a horizontal segment → projects onto (5, 0)
    [InlineData(5f, 3f, 0f, 0f, 10f, 0f, 5f, 0f)]
    // Point beyond the right end → clamps to end (10, 0)
    [InlineData(20f, 0f, 0f, 0f, 10f, 0f, 10f, 0f)]
    // Point before the left end → clamps to start (0, 0)
    [InlineData(-5f, 0f, 0f, 0f, 10f, 0f, 0f, 0f)]
    public void NearestPointOnLine_VariousPoints_ReturnsExpected(
        float px, float py,
        float sx, float sy, float ex, float ey,
        float expectedX, float expectedY)
    {
        var collider = new LinePieceCollider(new Vector2(sx, sy), new Vector2(ex, ey));

        Vector2 result = collider.NearestPointOnLine(new Vector2(px, py));

        Assert.True(Math.Abs(result.X - expectedX) < 1e-3f,
            $"Expected X={expectedX} but got X={result.X}");
        Assert.True(Math.Abs(result.Y - expectedY) < 1e-3f,
            $"Expected Y={expectedY} but got Y={result.Y}");
    }

    // ── Intersects(CircleCollider) ────────────────────────────────────────────

    [Fact]
    public void Intersects_CircleOverlapsSegment_ReturnsTrue()
    {
        // Horizontal segment from (0,0) to (10,0); circle centered at (5,1) radius 2 → nearest (5,0), distance 1 < 2
        var line = new LinePieceCollider(new Vector2(0, 0), new Vector2(10, 0));
        var circle = new CircleCollider(5f, 1f, 2f);

        Assert.True(line.Intersects(circle));
    }

    [Fact]
    public void Intersects_CircleFarFromSegment_ReturnsFalse()
    {
        // Same segment; circle centered at (5, 100) radius 1 → far away
        var line = new LinePieceCollider(new Vector2(0, 0), new Vector2(10, 0));
        var circle = new CircleCollider(5f, 100f, 1f);

        Assert.False(line.Intersects(circle));
    }

    // ── GetIntersection ──────────────────────────────────────────────────────

    [Fact]
    public void GetIntersection_TwoCrossingLines_ReturnsExpectedPoint()
    {
        // Horizontal line y=2: from (0,2) to (10,2)
        // Vertical line x=4:   from (4,0) to (4,10)
        // Expected intersection: (4, 2)
        var horizontal = new LinePieceCollider(new Vector2(0, 2), new Vector2(10, 2));
        var vertical = new LinePieceCollider(new Vector2(4, 0), new Vector2(4, 10));

        Vector2 result = horizontal.GetIntersection(vertical);

        Assert.True(Math.Abs(result.X - 4f) < 1e-3f, $"Expected X=4 but got X={result.X}");
        Assert.True(Math.Abs(result.Y - 2f) < 1e-3f, $"Expected Y=2 but got Y={result.Y}");
    }

    [Fact]
    public void GetIntersection_ParallelLines_ReturnsVectorZero()
    {
        // Two horizontal parallel lines at y=0 and y=5 — no intersection
        var line1 = new LinePieceCollider(new Vector2(0, 0), new Vector2(10, 0));
        var line2 = new LinePieceCollider(new Vector2(0, 5), new Vector2(10, 5));

        Vector2 result = line1.GetIntersection(line2);

        Assert.Equal(Vector2.Zero, result);
    }
}
