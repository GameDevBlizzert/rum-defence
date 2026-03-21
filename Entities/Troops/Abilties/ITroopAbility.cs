using Microsoft.Xna.Framework;

namespace RumDefence;

public interface ITroopAbility
{
    void Update(Troop troop, GameTime gameTime);
}