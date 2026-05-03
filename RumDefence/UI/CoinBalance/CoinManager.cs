using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace RumDefence;

public class CoinManager
{
    private List<Coin> coins = new();

    private Texture2D coinTexture;
    private Func<Vector2> getTargetPosition;
    private readonly LevelProgressSystem progress;


    public CoinManager(Func<Vector2> getTargetPosition, LevelProgressSystem progress)
    {
        this.getTargetPosition = getTargetPosition;
        this.progress = progress;
        coinTexture = RumGame.Instance.Content.Load<Texture2D>("Art/UI/Coin");
    }

    public void SpawnCoin(Vector2 worldPosition, int amount = 1)
    {
        for (int i = 0; i < amount; i++)
        {
            coins.Add(new Coin(worldPosition, getTargetPosition, coinTexture));
        }
    }

    public void Update(GameTime gameTime)
    {
        for (int i = coins.Count - 1; i >= 0; i--)
        {
            coins[i].Update(gameTime);

            if (coins[i].IsFinished)
            {
                progress.AddCoins(1);
                AudioManager.Instance.PlaySound("coin");
                coins.RemoveAt(i);
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var coin in coins)
        {
            coin.Draw(spriteBatch);
        }
    }
}
