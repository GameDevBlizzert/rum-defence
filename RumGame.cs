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

        public RumGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();

            Instance = this;
        }

        protected override void Initialize()
        {
            _screenManager = new ScreenManager();

            base.Initialize();

            float scaleX = (float)GraphicsDevice.Viewport.Width / VirtualWidth;
            float scaleY = (float)GraphicsDevice.Viewport.Height / VirtualHeight;

            scaleMatrix = Matrix.CreateScale(scaleX, scaleY, 1f);

            _screenManager.SetScreen(new MainMenuScreen(_screenManager));
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            var cursor = Content.Load<Texture2D>("Art/UI/cursor");
            Mouse.SetCursor(MouseCursor.FromTexture2D(cursor, 0, 0));
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