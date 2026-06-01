using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class LoadingSplashScreen : Screen
{
    private const float DurationSeconds = 2.8f;
    private const float BoatRotationOffset = -MathHelper.PiOver2;

    private readonly Screen nextScreen;
    private float elapsedSeconds;

    private Texture2D panelTexture;
    private Texture2D barrelTexture;
    private Texture2D boatTexture;
    private SpriteFont font;

    public LoadingSplashScreen(ScreenManager manager) : base(manager)
    {
        nextScreen = new MainMenuScreen(manager);
    }

    public override void Load()
    {
        var content = RumGame.Instance.Content;

        font = content.Load<SpriteFont>("Fonts/KenneyFuture");
        panelTexture = content.Load<Texture2D>("Art/UI/Panels/panel");
        barrelTexture = content.Load<Texture2D>("Art/Objects/RumBarrel");
        boatTexture = content.Load<Texture2D>("Art/Themes/Grass/Ships/ship_1");
    }

    public override void Update(GameTime gameTime)
    {
        elapsedSeconds += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (elapsedSeconds >= DurationSeconds)
        {
            manager.SetScreen(nextScreen);
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        int width = RumGame.VirtualWidth;
        int height = RumGame.VirtualHeight;

        DrawBackground(spriteBatch, width, height, elapsedSeconds);
        DrawWater(spriteBatch, width, height, elapsedSeconds);
        DrawScene(spriteBatch, width, height);
        DrawTitlePanel(spriteBatch, width, height);
        DrawLoadingBar(spriteBatch, width, height);
    }

    public static void DrawBackground(SpriteBatch spriteBatch, int width, int height, float elapsedSeconds)
    {
        spriteBatch.Draw(Primitives.Pixel, new Rectangle(0, 0, width, height), new Color(13, 22, 38));
        spriteBatch.Draw(Primitives.Pixel, new Rectangle(0, 0, width, height / 2), new Color(25, 58, 98) * 0.9f);

        for (int i = 0; i < 8; i++)
        {
            int bandHeight = 88;
            float waveOffset = MathF.Sin(elapsedSeconds * 1.1f + i * 0.65f) * (8f + i * 1.5f);
            int y = 500 + i * 58 + (int)waveOffset;
            Color bandColor = i % 2 == 0
                ? new Color(11, 59, 92)
                : new Color(15, 74, 114);

            spriteBatch.Draw(Primitives.Pixel, new Rectangle(-40, y, width + 80, bandHeight), bandColor);
        }

        for (int i = 0; i < 14; i++)
        {
            float pulse = 0.5f + 0.5f * MathF.Sin(elapsedSeconds * 2.2f + i * 0.7f);
            int bubbleWidth = 6 + (int)(pulse * 10f);
            int x = 120 + i * 120 + (int)(MathF.Sin(elapsedSeconds * 1.3f + i) * 14f);
            int y = 700 + (int)(MathF.Sin(elapsedSeconds * 2.6f + i * 0.35f) * 24f);

            spriteBatch.Draw(Primitives.Pixel, new Rectangle(x, y, bubbleWidth, bubbleWidth), Color.White * 0.15f);
        }
    }

    public static void DrawWater(SpriteBatch spriteBatch, int width, int height, float elapsedSeconds)
    {
        for (int i = 0; i < 6; i++)
        {
            float wave = MathF.Sin(elapsedSeconds * 3.1f + i * 0.8f) * 16f;
            int y = 740 + i * 40 + (int)wave;
            Color water = i % 2 == 0 ? new Color(28, 118, 164) : new Color(18, 99, 148);

            spriteBatch.Draw(Primitives.Pixel, new Rectangle(-20, y, width + 40, 28), water * 0.7f);
            spriteBatch.Draw(Primitives.Pixel, new Rectangle(-20, y + 4, width + 40, 4), Color.White * 0.06f);
        }
    }

    private void DrawScene(SpriteBatch spriteBatch, int width, int height)
    {
        float progress = MathHelper.Clamp(elapsedSeconds / DurationSeconds, 0f, 1f);
        float easedProgress = progress * progress * (3f - 2f * progress);

        Vector2 boatOrigin = new(boatTexture.Width / 2f, boatTexture.Height / 2f);
        Vector2 barrelOrigin = new(barrelTexture.Width / 2f, barrelTexture.Height / 2f);

        Vector2 boatPosition = new(
            MathHelper.Lerp(-120f, width * 0.38f, easedProgress),
            585f + MathF.Sin(elapsedSeconds * 4.8f) * 10f
        );

        Vector2 barrelPosition = new(
            MathHelper.Lerp(width + 120f, width * 0.67f, easedProgress),
            620f + MathF.Sin(elapsedSeconds * 3.7f + 1.1f) * 12f
        );

        float boatRotation = MathF.Sin(elapsedSeconds * 4.5f) * 0.04f;
        float barrelRotation = MathF.Sin(elapsedSeconds * 3.4f + 0.4f) * 0.06f;

        spriteBatch.Draw(
            boatTexture,
            boatPosition,
            null,
            Color.White,
            boatRotation + BoatRotationOffset,
            boatOrigin,
            1.8f,
            SpriteEffects.FlipHorizontally,
            0f
        );

        spriteBatch.Draw(
            barrelTexture,
            barrelPosition,
            null,
            Color.White,
            barrelRotation,
            barrelOrigin,
            1.9f,
            SpriteEffects.None,
            0f
        );

        spriteBatch.Draw(
            Primitives.Pixel,
            new Rectangle((int)(barrelPosition.X - 140), (int)(barrelPosition.Y - 70), 280, 140),
            new Color(255, 196, 92) * 0.08f
        );
    }

    private void DrawTitlePanel(SpriteBatch spriteBatch, int width, int height)
    {
        int panelWidth = 980;
        int panelHeight = 290;
        var panelRect = new Rectangle(width / 2 - panelWidth / 2, 110, panelWidth, panelHeight);

        NineSlice.Draw(spriteBatch, panelTexture, panelRect, new Rectangle(0, 0, 128, 128), 20, Color.White * 0.96f);

        string title = "DEFEND THE RUM";
        string loading = GetLoadingText();

        Vector2 titleSize = font.MeasureString(title);
        Vector2 loadingSize = font.MeasureString(loading);

        spriteBatch.DrawString(
            font,
            title,
            new Vector2(panelRect.Center.X - titleSize.X / 2f, panelRect.Y + 36),
            new Color(38, 23, 12)
        );

        spriteBatch.DrawString(
            font,
            loading,
            new Vector2(panelRect.Center.X - loadingSize.X / 2f, panelRect.Y + 128),
            new Color(64, 36, 16)
        );
    }

    private void DrawLoadingBar(SpriteBatch spriteBatch, int width, int height)
    {
        float progress = MathHelper.Clamp(elapsedSeconds / DurationSeconds, 0f, 1f);

        var barBounds = new Rectangle(width / 2 - 360, height - 160, 720, 34);
        var fillBounds = new Rectangle(barBounds.X + 6, barBounds.Y + 6, (int)((barBounds.Width - 12) * progress), barBounds.Height - 12);

        spriteBatch.Draw(Primitives.Pixel, barBounds, new Color(20, 12, 8) * 0.9f);
        spriteBatch.Draw(Primitives.Pixel, fillBounds, new Color(245, 183, 72));

        for (int i = 0; i < 5; i++)
        {
            int portholeX = barBounds.X + 60 + i * 145;
            int portholeY = barBounds.Y + 8;
            float pulse = 0.4f + 0.6f * MathF.Sin(elapsedSeconds * 5f + i * 0.7f);

            spriteBatch.Draw(Primitives.Pixel, new Rectangle(portholeX, portholeY, 18, 18), new Color(255, 246, 215) * pulse * 0.5f);
        }

        string label = $"Preparing the cannon deck... {(int)(progress * 100f)}%";
        Vector2 labelSize = font.MeasureString(label);

        spriteBatch.DrawString(
            font,
            label,
            new Vector2(width / 2f - labelSize.X / 2f, barBounds.Y - 44),
            Color.White
        );
    }

    private string GetLoadingText()
    {
        int dots = (int)(elapsedSeconds * 4f) % 4;
        return $"Loading{new string('.', dots)}";
    }
}
