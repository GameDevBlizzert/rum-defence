using Microsoft.Xna.Framework;

namespace RumDefence;

public class Camera2D
{
    public float Zoom { get; private set; } = 1f;

    public void AdjustZoom(float amount)
    {
        Zoom += amount;
        Zoom = MathHelper.Clamp(Zoom, 0.5f, 2.5f);
    }

    public Matrix GetMatrix()
    {
        return Matrix.CreateScale(Zoom, Zoom, 1f);
    }
}