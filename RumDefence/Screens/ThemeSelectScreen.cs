using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class ThemeSelectScreen : Screen
{
    private SimpleButton grassButton;
    private SimpleButton stoneButton;

    private Texture2D buttonTexture;
    private SpriteFont font;

    public ThemeSelectScreen(ScreenManager manager) : base(manager) { }

    public override void Load()
    {
        var content = RumGame.Instance.Content;

        font = content.Load<SpriteFont>("Fonts/KenneyFuture");
        buttonTexture = content.Load<Texture2D>("Art/UI/Buttons/button_blue");

        grassButton = new SimpleButton(buttonTexture, font, "Grass",
            new Vector2(800, 400), new Vector2(300, 100));

        stoneButton = new SimpleButton(buttonTexture, font, "Stone",
            new Vector2(800, 550), new Vector2(300, 100));

        grassButton.OnClick = () =>
        {
            manager.SetScreen(new LevelSelectScreen(manager, GrassLevels.All));
        };

        stoneButton.OnClick = () =>
        {
            manager.SetScreen(new LevelSelectScreen(manager, GrassLevels.All)); // later StoneLevels
        };
    }

    public override void Update(GameTime gameTime)
    {
        grassButton.Update(gameTime);
        stoneButton.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        RumGame.Instance.GraphicsDevice.Clear(Color.CornflowerBlue);

        grassButton.Draw(spriteBatch);
        stoneButton.Draw(spriteBatch);
    }
}