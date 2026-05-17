using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RumDefence;

public class KeyBindingsScreen : Screen
{
    private readonly Screen previous;

    private Texture2D panelTexture;
    private Texture2D buttonTexture;

    private SimpleButton backButton;
    private Rectangle panelRect;
    private MouseState prevMouse;

    private const int PanelLeft   = 560;
    private const int PanelTop    = 190;
    private const int PanelWidth  = 800;
    private const int PanelHeight = 620;

    private const int RowHeight   = 78;
    private const int RowsStartY  = PanelTop + 110;
    private const int RowMargin   = 30;

    private static readonly (string Id, string Name)[] Actions =
    {
        ("Pause",      "Pause Game"),
        ("MultiPlace", "Multi-Place Mode"),
        ("LevelNext",  "Next Level Page"),
        ("LevelPrev",  "Prev Level Page"),
    };

    private string rebindingAction = null;

    public KeyBindingsScreen(ScreenManager manager, Screen previous) : base(manager)
    {
        this.previous = previous;
    }

    public override void Load()
    {
        var content = RumGame.Instance.Content;
        panelTexture = content.Load<Texture2D>("Art/UI/Panels/panel");
        buttonTexture = content.Load<Texture2D>("Art/UI/Buttons/button");

        panelRect = new Rectangle(PanelLeft, PanelTop, PanelWidth, PanelHeight);

        int backX = PanelLeft + (PanelWidth - 200) / 2;
        backButton = new SimpleButton(buttonTexture, "Back",
            new Vector2(backX, PanelTop + PanelHeight - 110),
            new Vector2(200, 70));

        backButton.OnClick = () =>
        {
            InputManager.Instance.SaveToSave();
            manager.SetScreen(previous);
        };
    }

    public override void Update(GameTime gameTime)
    {
        var mouse = Mouse.GetState();
        var mousePos = ScreenManager.GetMousePositionScaled();

        if (rebindingAction != null)
        {
            if (InputManager.Instance.IsAnyKeyJustPressed(out Keys pressedKey))
            {
                if (pressedKey == Keys.Escape)
                    rebindingAction = null;
                else
                {
                    InputManager.Instance.SetBinding(rebindingAction, pressedKey);
                    rebindingAction = null;
                }
            }
        }
        else
        {
            bool clicked = mouse.LeftButton == ButtonState.Released &&
                           prevMouse.LeftButton == ButtonState.Pressed;

            if (clicked)
            {
                var mp = new Point((int)mousePos.X, (int)mousePos.Y);
                for (int i = 0; i < Actions.Length; i++)
                {
                    if (GetRowRect(i).Contains(mp))
                    {
                        rebindingAction = Actions[i].Id;
                        break;
                    }
                }
            }

            if (InputManager.Instance.IsActionJustPressed("Pause"))
            {
                InputManager.Instance.SaveToSave();
                manager.SetScreen(previous);
                return;
            }

            backButton.Update(gameTime);
        }

        prevMouse = mouse;
    }

    private Rectangle GetRowRect(int index) =>
        new Rectangle(
            PanelLeft + RowMargin,
            RowsStartY + index * RowHeight,
            PanelWidth - RowMargin * 2,
            68
        );

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (previous != null)
            previous.Draw(spriteBatch);
        else
            RumGame.Instance.GraphicsDevice.Clear(Color.DarkSlateGray);

        spriteBatch.Draw(Primitives.Pixel,
            new Rectangle(0, 0, RumGame.VirtualWidth, RumGame.VirtualHeight),
            Color.Black * 0.5f);

        NineSlice.Draw(spriteBatch, panelTexture, panelRect, new Rectangle(0, 0, 128, 128), 20, Color.White);

        var title = "Key Bindings";
        var titleSize = Primitives.Font.MeasureString(title);
        spriteBatch.DrawString(Primitives.Font, title,
            new Vector2(PanelLeft + (PanelWidth - titleSize.X) / 2f, PanelTop + 40),
            Primitives.FontColor);

        var mousePos = ScreenManager.GetMousePositionScaled();
        var mp = new Point((int)mousePos.X, (int)mousePos.Y);

        const float rowTextScale = 0.75f;

        for (int i = 0; i < Actions.Length; i++)
        {
            var (id, name) = Actions[i];
            var rowRect = GetRowRect(i);
            bool isRebinding = rebindingAction == id;
            bool isHovered = rebindingAction == null && rowRect.Contains(mp);

            Color rowBg = isRebinding
                ? new Color(80, 60, 20) * 0.9f
                : isHovered
                    ? new Color(255, 255, 255) * 0.12f
                    : new Color(255, 255, 255) * 0.05f;

            spriteBatch.Draw(Primitives.Pixel, rowRect, rowBg);

            Color borderColor = isRebinding
                ? new Color(255, 200, 80)
                : new Color(255, 255, 255) * 0.2f;

            spriteBatch.Draw(Primitives.Pixel, new Rectangle(rowRect.X, rowRect.Y, rowRect.Width, 1), borderColor);
            spriteBatch.Draw(Primitives.Pixel, new Rectangle(rowRect.X, rowRect.Bottom - 1, rowRect.Width, 1), borderColor);
            spriteBatch.Draw(Primitives.Pixel, new Rectangle(rowRect.X, rowRect.Y, 1, rowRect.Height), borderColor);
            spriteBatch.Draw(Primitives.Pixel, new Rectangle(rowRect.Right - 1, rowRect.Y, 1, rowRect.Height), borderColor);

            float scaledTextH = Primitives.Font.MeasureString(name).Y * rowTextScale;
            int textY = rowRect.Y + (int)((rowRect.Height - scaledTextH) / 2);

            spriteBatch.DrawString(Primitives.Font, name,
                new Vector2(rowRect.X + 20, textY),
                Primitives.FontColor, 0f, Vector2.Zero, rowTextScale, SpriteEffects.None, 0f);

            string keyText = isRebinding
                ? "Press any key..."
                : InputManager.GetKeyDisplayName(InputManager.Instance.GetBinding(id));

            Color keyColor = isRebinding ? new Color(255, 200, 80) : new Color(160, 210, 255);
            var keySize = Primitives.Font.MeasureString(keyText) * rowTextScale;
            spriteBatch.DrawString(Primitives.Font, keyText,
                new Vector2(rowRect.Right - 20 - keySize.X, textY),
                keyColor, 0f, Vector2.Zero, rowTextScale, SpriteEffects.None, 0f);
        }

        string hint = rebindingAction != null
            ? "Press Escape to cancel"
            : "Click a row to rebind";
        var hintSize = Primitives.Font.MeasureString(hint);
        spriteBatch.DrawString(Primitives.Font, hint,
            new Vector2(PanelLeft + (PanelWidth - hintSize.X) / 2f, PanelTop + PanelHeight - 160),
            new Color(160, 160, 160));

        if (rebindingAction == null)
            backButton.Draw(spriteBatch);
    }
}
