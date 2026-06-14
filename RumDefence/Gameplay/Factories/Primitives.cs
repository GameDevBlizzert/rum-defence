using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public enum Depth
{
    Map,
    Obstacles,
    Decorations,
    Troop,
    Towers,
    VisualEffects,
    UI,
}
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

    public static SpriteFont Font => _font ??= RumGame.Instance.Content.Load<SpriteFont>("Fonts/font");

    private static Texture2D _panelTexture;
    public static Texture2D PanelTexture => _panelTexture ??= RumGame.Instance.Content.Load<Texture2D>("Art/UI/Panels/panel");
    private static Texture2D _panelInvertedTexture;
    public static Texture2D PanelInvertedTexture => _panelInvertedTexture ??= RumGame.Instance.Content.Load<Texture2D>("Art/UI/Panels/panel.inverted");
    private static Texture2D _buttonTexture;
    public static Texture2D ButtonTexture => _buttonTexture ??= RumGame.Instance.Content.Load<Texture2D>("Art/UI/Buttons/button");
    private static Texture2D _buttonInvertedTexture;
    public static Texture2D ButtonInvertedTexture => _buttonInvertedTexture ??= RumGame.Instance.Content.Load<Texture2D>("Art/UI/Buttons/button.inverted");
    public static Color FontColor => new Color(255, 200, 0);
    public static Color FontDarkColor => new Color(215, 90, 0);
    public static Color FontLightColor => new Color(255, 225, 120);

    public const float TowerSize = 1f;
}
