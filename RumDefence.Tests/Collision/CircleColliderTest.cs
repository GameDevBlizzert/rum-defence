// AI generated: this test file was created with AI assistance.
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Xunit;

namespace RumDefence.Tests.Collision;

[TestSubject(typeof(CircleCollider))]
public class CircleColliderTest
{
    // ── Contains ────────────────────────────────────────────────────────────

    [Fact]
    public void Contains_PointAtCenter_ReturnsTrue()
    {
        var circle = new CircleCollider(0f, 0f, 10f);

        Assert.True(circle.Contains(new Vector2(0, 0)));
    }

    [Theory]
    [InlineData(5f, 0f, true)]      // inside
    [InlineData(9.9f, 0f, true)]    // just inside
    [InlineData(11f, 0f, false)]    // outside
    public void Contains_VariousPoints_ReturnsExpected(float px, float py, bool expected)
    {
        var circle = new CircleCollider(0f, 0f, 10f);

        Assert.Equal(expected, circle.Contains(new Vector2(px, py)));
    }

    [Fact]
    public void Contains_PointExactlyOnEdge_ReturnsFalse()
    {
        // Implementation uses strict < so a point exactly at radius distance is NOT contained.
        var circle = new CircleCollider(0f, 0f, 10f);

        Assert.False(circle.Contains(new Vector2(10f, 0f)));
    }

    // ── Intersects(CircleCollider) ───────────────────────────────────────────

    [Fact]
    public void Intersects_OverlappingCircles_ReturnsTrue()
    {
        var a = new CircleCollider(0f, 0f, 5f);
        var b = new CircleCollider(8f, 0f, 5f);   // overlap of 2 units

        Assert.True(a.Intersects(b));
    }

    [Fact]
    public void Intersects_FarApartCircles_ReturnsFalse()
    {
        var a = new CircleCollider(0f, 0f, 5f);
        var b = new CircleCollider(100f, 0f, 5f);

        Assert.False(a.Intersects(b));
    }

    [Fact]
    public void Intersects_ExactlyTouchingCircles_ReturnsTrue()
    {
        // Distance equals sum of radii — touching counts as intersecting (<=).
        var a = new CircleCollider(0f, 0f, 5f);
        var b = new CircleCollider(10f, 0f, 5f);  // distance == 10 == 5+5

        Assert.True(a.Intersects(b));
    }

    // ── Intersects(RectangleCollider) ──────────────────────────────────────

    [Fact]
    public void Intersects_CircleOverlapsRectangle_ReturnsTrue()
    {
        var circle = new CircleCollider(0f, 0f, 10f);
        var rect = new RectangleCollider(new Rectangle(5, 0, 20, 20));

        Assert.True(circle.Intersects(rect));
    }

    [Fact]
    public void Intersects_CircleClearlyOutsideRectangle_ReturnsFalse()
    {
        var circle = new CircleCollider(0f, 0f, 5f);
        var rect = new RectangleCollider(new Rectangle(100, 100, 20, 20));

        Assert.False(circle.Intersects(rect));
    }

    // ── GetBoundingBox ───────────────────────────────────────────────────────

    [Fact]
    public void GetBoundingBox_KnownCircle_ReturnsExpectedRectangle()
    {
        var circle = new CircleCollider(20f, 30f, 10f);

        var box = circle.GetBoundingBox();

        // Expected: x=10, y=20, width=20, height=20
        Assert.Equal(new Rectangle(10, 20, 20, 20), box);
    }

    // ── Center setter / Equals ───────────────────────────────────────────────

    [Fact]
    public void CenterSetter_UpdatesXAndY()
    {
        var circle = new CircleCollider(0f, 0f, 5f);

        circle.Center = new Vector2(3f, 4f);

        Assert.Equal(3f, circle.X);
        Assert.Equal(4f, circle.Y);
    }

    [Fact]
    public void Equals_SameXYRadius_ReturnsTrue()
    {
        var a = new CircleCollider(1f, 2f, 3f);
        var b = new CircleCollider(new Vector2(1f, 2f), 3f);

        Assert.True(a.Equals(b));
    }
}
