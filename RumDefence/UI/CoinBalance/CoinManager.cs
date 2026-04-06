using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace RumDefence;

public class CoinManager
{
    private List<Coin> coins = new();

    private Texture2D coinTexture;
    private Func<Vector2> getTargetPosition;

    public int Balance { get; private set; }

    public CoinManager(Func<Vector2> getTargetPosition)
    {
        this.getTargetPosition = getTargetPosition;
        coinTexture = RumGame.Instance.Content.Load<Texture2D>("Art/UI/coin");
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
                Balance++;
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