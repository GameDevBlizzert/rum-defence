using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RumDefence;

public class WallRepairMenu
{
    private Texture2D panelTexture;
    private Rectangle panelRect;
    private SimpleButton repairButton;
    private LevelProgressSystem progress;
    private KeyboardState previousKeyboardState;
    public bool RepairClicked { get; private set; }
    public bool IsDisabled { get; set; }

    public Wall SelectedWall { get; set; }

    public bool IsMouseOver(Vector2 mousePos)
    {
        return panelRect.Contains(mousePos);
    }

    public WallRepairMenu(LevelProgressSystem progress)
    {
        this.progress = progress;

        var content = RumGame.Instance.Content;
        panelTexture = content.Load<Texture2D>("Art/UI/Panels/panel");
        var buttonTexture = content.Load<Texture2D>("Art/UI/Buttons/button");

        int width = 340;
        int height = 240;
        int x = RumGame.VirtualWidth - width - 20;
        int y = RumGame.VirtualHeight - height - 320;
        panelRect = new Rectangle(x, y, width, height);

        repairButton = new SimpleButton(buttonTexture, "Repair (R)", new Vector2(0, 0), new Vector2(width - 40, 60));
        repairButton.SetBounds(new Rectangle(panelRect.X + 20, panelRect.Bottom - 80, width - 40, 60));
        repairButton.OnClick = () => { RepairClicked = true; };
    }

    public void Update(GameTime gameTime)
    {
        var keyboard = Keyboard.GetState();
        var repairShortcutPressed =
            SelectedWall != null &&
            !IsDisabled &&
            keyboard.IsKeyDown(Keys.R) &&
            previousKeyboardState.IsKeyUp(Keys.R);

        previousKeyboardState = keyboard;

        RepairClicked = false;

        if (IsDisabled)
            return;

        if (SelectedWall == null)
            return;

        if (repairShortcutPressed)
            RepairClicked = true;

        int cost = SelectedWall.GetRepairCostToFull();
        repairButton.IsDisabled = SelectedWall.IsDestroyed || !SelectedWall.IsDamaged || progress.CoinsRemaining < cost || cost <= 0;

        repairButton.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (SelectedWall == null) return;

        NineSlice.Draw(spriteBatch, panelTexture, panelRect, new Rectangle(0, 0, 128, 128), 20, Color.White);

        var title = "Wall";
        float titleScale = 0.8f;
        spriteBatch.DrawString(Primitives.Font, title, new Vector2(panelRect.X + 20, panelRect.Y + 20), Primitives.FontColor, 0f, Vector2.Zero, titleScale, SpriteEffects.None, 0f);

        var startY = panelRect.Y + 50;
        var spacing = 28;
        float statScale = 0.65f;

        spriteBatch.DrawString(Primitives.Font, $"HP: {SelectedWall.Health}/{Wall.MaxHealth}", new Vector2(panelRect.X + 20, startY), Primitives.FontColor, 0f, Vector2.Zero, statScale, SpriteEffects.None, 0f);

        int cost = SelectedWall.GetRepairCostToFull();
        if (SelectedWall.IsDamaged && cost > 0)
        {
            spriteBatch.DrawString(Primitives.Font, $"Cost: {cost} coins", new Vector2(panelRect.X + 20, startY + spacing), Primitives.FontColor, 0f, Vector2.Zero, statScale, SpriteEffects.None, 0f);
            repairButton.Draw(spriteBatch);
        }
        else
        {
            spriteBatch.DrawString(Primitives.Font, "No repair needed", new Vector2(panelRect.X + 20, startY + spacing), Primitives.FontColor, 0f, Vector2.Zero, statScale, SpriteEffects.None, 0f);
        }
    }
}
