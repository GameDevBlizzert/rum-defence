using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RumDefence.UI.Box;
using RumDefence.UI.Box.Components;

namespace RumDefence
{
    public class RumGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private ScreenManager _screenManager;

        public static RumGame Instance { get; private set; }

        public static int VirtualWidth = 1920;
        public static int VirtualHeight = 1080;

        private Matrix scaleMatrix;

        public Grid CurrentGrid { get; set; }
        public Level CurrentLevel { get; set; }

        private bool wasActive = true;

        public RumGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.PreferredBackBufferWidth = 1280; // probeer verschillende resoluties om de scaling te testen
            _graphics.PreferredBackBufferHeight = 720; // 1920 x 1080 1280 x 720
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += OnResize;

            Instance = this;
        }

        private void OnResize(object sender, System.EventArgs e) => UpdateScaleMatrix();

        private void UpdateScaleMatrix()
        {
            float scaleX = (float)GraphicsDevice.Viewport.Width / VirtualWidth;
            float scaleY = (float)GraphicsDevice.Viewport.Height / VirtualHeight;
            scaleMatrix = Matrix.CreateScale(scaleX, scaleY, 1f);
        }
        // wip
        private Box testbox;
        protected override void Initialize()
        {
            SaveManager.Load();

            _screenManager = new ScreenManager();


            base.Initialize();
            // wip
            var box2 = new Box() { Padding = 0, AlignX = Align.End, Direction = Direction.Column };
            box2.Add(new TextItem() { Text = "helloooooooooooooooooooooo" });
            box2.Add(new TextItem() { Text = "hooiii" });
            // box2.Add(new ImageBox(Primitives.PanelTexture, 128 * 3, 128 * 3));
            testbox = new Box()
            {
                // SpanCol = 2,
                AlignY = Align.Start,
                AlignX = Align.Start,
                // Direction = Direction.Row
                // AlignX = Align.Center,
                // Width = 800,
                // Height = 500
            };
            // testbox.AddBackground(new ImageItem(Primitives.PanelTexture, new Vector2(testbox.Width, testbox.Height)));

            // testbox.Add(box2);
            testbox.Add(new ImageBox(Primitives.PanelTexture, 64 * 16, 64 * 12));
            testbox.Add(new ImageBox(Primitives.PanelTexture, 64 * 2, 64));
            testbox.Add(new ImageBox(Primitives.ButtonTexture, 64 * 2, 64));
            // testbox.Add(new TextItem() { Text = "hooiii" });
            // testbox.Add(new TextItem() { Text = "hooiii222efffg" });
            // testbox.Add(new PixelBox(Color.AntiqueWhite, 300, 100));
            // testbox.Add(box2);
            testbox.AddBackground(new PixelBox(Color.Beige));
            testbox.PlaceAt(0, 0, VirtualWidth, VirtualHeight);

            UpdateScaleMatrix();
            SaveManager.Load();
            AudioManager.Instance.MusicVolume = SaveManager.CurrentSave.MusicVolume;
            AudioManager.Instance.SoundVolume = SaveManager.CurrentSave.SfxVolume;

            _screenManager.SetScreen(new LoadingSplashScreen(_screenManager));
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            var cursor = Content.Load<Texture2D>("Art/UI/Cursor");
            Mouse.SetCursor(MouseCursor.FromTexture2D(cursor, 0, 0));

            AudioManager.Instance.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {

            // wip
            testbox.Update(gameTime);
            return;
            // Used for disabling behavior while using the debugger
            bool pauseOnBlurDisabled = bool.Parse(
                System.Environment.GetEnvironmentVariable("DISABLE_PAUSE_ON_BLUR") ?? "false"
            );
            bool active = IsActive || pauseOnBlurDisabled;

            if (wasActive && !active) // Focus lost
            {
                AudioManager.Instance.SuspendAudio();

                if (_screenManager != null)
                {
                    Screen current = _screenManager.GetCurrentScreen();
                    if (current is GameScreen)
                    {
                        _screenManager.SetScreen(new PauseScreen(_screenManager, current, focusLoss: true));
                    }
                }
            }

            if (!wasActive && active) // Focus regained
            {
                AudioManager.Instance.ResumeAudio();
            }

            wasActive = active;

            // Block game logic when window is inactive.
            if (!active)
            {
                base.Update(gameTime);
                return;
            }

            _screenManager.Update(gameTime);
            AudioManager.Instance.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(30, 144, 255));
            _spriteBatch.Begin(transformMatrix: scaleMatrix);

            // wip
            testbox.Draw(_spriteBatch);

            _spriteBatch.End();
            // _screenManager.Draw(_spriteBatch, scaleMatrix);
            base.Draw(gameTime);
        }
    }
}
