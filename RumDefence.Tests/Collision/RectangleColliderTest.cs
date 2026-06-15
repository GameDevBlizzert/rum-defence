// AI generated: this test file was created with AI assistance.
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using RumDefence;
using Xunit;

namespace RumDefence.Tests.Collision;

[TestSubject(typeof(RectangleCollider))]
public class RectangleColliderTest
{
    // ── Contains ────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(5, 5, true)]     // center of rect — inside
    [InlineData(0, 0, true)]     // top-left corner — inclusive
    [InlineData(9, 9, true)]     // one step inside bottom-right corner
    [InlineData(10, 10, false)]  // bottom-right corner — exclusive
    [InlineData(10, 5, false)]   // right edge — exclusive
    [InlineData(5, 10, false)]   // bottom edge — exclusive
    [InlineData(-1, 5, false)]   // outside left
    public void Contains_VariousPoints_ReturnsExpected(int px, int py, bool expected)
    {
        // Rectangle(0, 0, 10, 10): Left=0, Top=0, Right=10, Bottom=10
        // MonoGame uses half-open intervals: [Left, Right) x [Top, Bottom)
        var rect = new RectangleCollider(new Rectangle(0, 0, 10, 10));

        Assert.Equal(expected, rect.Contains(new Vector2(px, py)));
    }

    // ── Intersects(RectangleCollider) ──────────────────────────────────────

    [Fact]
    public void Intersects_OverlappingRectangles_ReturnsTrue()
    {
        var a = new RectangleCollider(new Rectangle(0, 0, 20, 20));
        var b = new RectangleCollider(new Rectangle(10, 10, 20, 20));

        Assert.True(a.Intersects(b));
    }

    [Fact]
    public void Intersects_ClearlySeperatedRectangles_ReturnsFalse()
    {
        var a = new RectangleCollider(new Rectangle(0, 0, 10, 10));
        var b = new RectangleCollider(new Rectangle(100, 100, 10, 10));

        Assert.False(a.Intersects(b));
    }

    [Fact]
    public void Intersects_EdgeTouchingRectangles_ReturnsFalse()
    {
        // a occupies x=[0,10), b occupies x=[10,20) — they share an edge but do not overlap
        var a = new RectangleCollider(new Rectangle(0, 0, 10, 10));
        var b = new RectangleCollider(new Rectangle(10, 0, 10, 10));

        Assert.False(a.Intersects(b));
    }

    // ── Intersects(CircleCollider) ──────────────────────────────────────────

    [Fact]
    public void Intersects_CircleOverlapsRectangle_ReturnsTrue()
    {
        var rect = new RectangleCollider(new Rectangle(0, 0, 20, 20));
        var circle = new CircleCollider(10f, 10f, 5f);  // center inside the rect

        Assert.True(rect.Intersects(circle));
    }

    [Fact]
    public void Intersects_CircleFarFromRectangle_ReturnsFalse()
    {
        var rect = new RectangleCollider(new Rectangle(0, 0, 10, 10));
        var circle = new CircleCollider(100f, 100f, 5f);

        Assert.False(rect.Intersects(circle));
    }

    // ── Intersects(LinePieceCollider) ──────────────────────────────────────

    [Fact]
    public void Intersects_LineThroughRectangle_ReturnsTrue()
    {
        var rect = new RectangleCollider(new Rectangle(0, 0, 10, 10));
        // Diagonal line from (-5,-5) to (5,5) passes through the rect
        var line = new LinePieceCollider(new Vector2(-5, -5), new Vector2(5, 5));

        Assert.True(rect.Intersects(line));
    }

    [Fact]
    public void Intersects_LineEntirelyOutsideRectangle_ReturnsFalse()
    {
        var rect = new RectangleCollider(new Rectangle(0, 0, 10, 10));
        // Horizontal line far below the rect
        var line = new LinePieceCollider(new Vector2(-10, 50), new Vector2(50, 50));

        Assert.False(rect.Intersects(line));
    }

    // ── GetBoundingBox ───────────────────────────────────────────────────────

    [Fact]
    public void GetBoundingBox_ReturnsOriginalShape()
    {
        var shape = new Rectangle(3, 7, 15, 25);
        var rect = new RectangleCollider(shape);

        Assert.Equal(shape, rect.GetBoundingBox());
    }

    // ── Equals ───────────────────────────────────────────────────────────────

    [Fact]
    public void Equals_SameRectangle_ReturnsTrue()
    {
        var a = new RectangleCollider(new Rectangle(1, 2, 3, 4));
        var b = new RectangleCollider(new Rectangle(1, 2, 3, 4));

        Assert.True(a.Equals(b));
    }
}
