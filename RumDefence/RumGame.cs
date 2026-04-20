using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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

        private void OnResize(object sender, System.EventArgs e)
        {
            UpdateScaleMatrix();
        }

        private void UpdateScaleMatrix()
        {
            float scaleX = (float)GraphicsDevice.Viewport.Width / VirtualWidth;
            float scaleY = (float)GraphicsDevice.Viewport.Height / VirtualHeight;

            scaleMatrix = Matrix.CreateScale(scaleX, scaleY, 1f);
        }

        protected override void Initialize()
        {
            _screenManager = new ScreenManager();
            base.Initialize();
            UpdateScaleMatrix();
            _screenManager.SetScreen(new MainMenuScreen(_screenManager));
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
            _screenManager.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(30, 144, 255));

            _screenManager.Draw(_spriteBatch, scaleMatrix);

            base.Draw(gameTime);
        }
    }
}
