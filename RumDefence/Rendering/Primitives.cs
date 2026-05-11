using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public static class Primitives
{
    private static Texture2D _pixel;
    private static SpriteFont _font;

    public static Texture2D Pixel
    {
        get
        {
            if (_pixel == null)
            {
                _pixel = new Texture2D(RumGame.Instance.GraphicsDevice, 1, 1);
                _pixel.SetData(new[] { Color.White });
            }
            return _pixel;
        }
    }

    public static SpriteFont Font => _font ??= RumGame.Instance.Content.Load<SpriteFont>("Fonts/KenneyFuture");

    public static Color FontColor => new Color(255, 200, 0);
}
