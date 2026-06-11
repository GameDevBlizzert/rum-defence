using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RumDefence.UI.Box;

namespace RumDefence;

public class KeyBindingsScreen : Screen
{
    private readonly Screen previous;
    private Box screenBox = new Box();
    private const int PanelWidth = 700;
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
        var panel = new Box() { Direction = Direction.Row, Gap = 10, Padding = 20 };
        panel.AddBackground(new ImageBox(Primitives.PanelTexture));

        panel.Add(new TextItem("Key Bindings"));
        for (int i = 0; i < Actions.Length; i++)
        {
            var (id, name) = Actions[i];
            var row = new KeyBindBox(id, name, 0.6f) { Size = new(PanelWidth - 60, 100), Padding = 12 };
            panel.Add(row);
        }
        panel.Add(new TextItem("Click a row to rebind", 0.75f, new Color(160, 160, 160)));
        var backOrSave = new Box();
        var saveButton = new ButtonBox(Primitives.ButtonTexture, "Save", size: new Vector2(260, 100));
        saveButton.OnClick = () =>
        {
            InputManager.Instance.SaveToSave();
            manager.SetScreen(previous);
        };
        backOrSave.Add(saveButton);
        var backButton = new ButtonBox(Primitives.ButtonTexture, "Back", size: new Vector2(260, 100));
        backButton.OnClick = () =>
        {
            InputManager.Instance.LoadFromSave();
            manager.SetScreen(previous);
        };
        backOrSave.Add(backButton);
        panel.Add(backOrSave);
        screenBox.Add(panel);
        screenBox.Arrange(new(0, 0, RumGame.VirtualWidth, RumGame.VirtualHeight));
    }

    public override void Update(GameTime gameTime)
    {
        if (InputManager.Instance.IsActionJustPressed("Pause"))
        {
            InputManager.Instance.LoadFromSave();
            manager.SetScreen(previous);
            return;
        }
        screenBox.Update(gameTime);
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

        screenBox.Draw(spriteBatch);
    }
}
