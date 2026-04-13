using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class WaterRenderer
{
    private Texture2D pixel;
    private Effect waterEffect;
    private Grid grid;

    public WaterRenderer(Grid grid)
    {
        this.grid = grid;

        waterEffect = RumGame.Instance.Content.Load<Effect>("Shaders/WaterShader");

        pixel = new Texture2D(RumGame.Instance.GraphicsDevice, 1, 1);
        pixel.SetData(new[] { Color.White });
    }

    public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        waterEffect.Parameters["Time"].SetValue(
            (float)gameTime.TotalGameTime.TotalSeconds
        );

        Rectangle fullRect = new Rectangle(
            0,
            0,
            grid.Width * grid.TileSize,
            grid.Height * grid.TileSize
        );

        spriteBatch.End();

        spriteBatch.Begin(effect: waterEffect);

        spriteBatch.Draw(pixel, fullRect, Color.White);

        spriteBatch.End();

        spriteBatch.Begin();
    }
}