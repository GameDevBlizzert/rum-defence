using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence.UI.Box;

public interface IBoxItem
{
    Vector2 Measure();
    void Update(GameTime gameTime);
    void Arrange(Rectangle rect);
    void Draw(SpriteBatch spriteBatch);
}