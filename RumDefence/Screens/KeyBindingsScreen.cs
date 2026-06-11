using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RumDefence.UI.Box;

namespace RumDefence;

public class KeyBindingsScreen : Screen
{
    private readonly Screen previous;
    private Box panel;
    private string rebindingAction = null;

    private const int PanelLeft = 560;
    private const int PanelTop = 120;
    private const int PanelWidth = 800;
    private const int PanelHeight = 860;
    private static readonly (string Id, string Name)[] Actions =
    {
        // ("Pause",             "Pause Game"),
        ("MultiPlace",        "Multi-Place Mode"),
        ("Upgrade",           "Upgrade Tower / Wall"),
        ("Repair",            "Repair Wall"),
        ("TogglePause",       "Pause / Resume"),
        ("ToggleFastForward", "Toggle 2x Speed"),
    };
    public KeyBindingsScreen(ScreenManager manager, Screen previous) : base(manager)
    {
        this.previous = previous;
    }
    public override void Load()
    {
        panel = new Box() { Direction = Direction.Row, Gap = 10 };
        panel.AddBackground(new ImageBox(Primitives.PanelTexture));

        panel.Add(new TextItem("Key Bindings"));
        for (int i = 0; i < Actions.Length; i++)
        {
            var (id, name) = Actions[i];
            var row = new KeyBindBox(id, name) { Size = new(PanelWidth - 60, 70) };
            row.OnClick = () => rebindingAction = row.ActionId;
            panel.Add(row);
        }
        panel.Add(new TextItem("Click a row to rebind", 0.75f, new Color(160, 160, 160)));
        var saveButton = new ButtonBox(Primitives.ButtonTexture, "Save", size: new Vector2(300, 100));
        saveButton.OnClick = () =>
        {
            InputManager.Instance.SaveToSave();
            manager.SetScreen(previous);
        };
        panel.Add(saveButton);
        var backButton = new ButtonBox(Primitives.ButtonTexture, "Back", size: new Vector2(300, 100));
        backButton.OnClick = () =>
        {
            manager.SetScreen(previous);
        };
        panel.Add(backButton);
        panel.Arrange(new Rectangle(PanelLeft, PanelTop, PanelWidth, PanelHeight));
    }

    public override void Update(GameTime gameTime)
    {
        panel.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (previous != null)
            previous.Draw(spriteBatch);
        else
            RumGame.Instance.GraphicsDevice.Clear(Color.DarkSlateGray);

        spriteBatch.Draw(Primitives.Pixel,
            new Rectangle(0, 0, RumGame.VirtualWidth, RumGame.VirtualHeight),
            Color.Black * 0.5f);

        panel.Draw(spriteBatch);
    }
}
