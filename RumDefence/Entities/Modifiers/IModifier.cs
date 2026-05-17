using Microsoft.Xna.Framework;

namespace RumDefence;

public interface IModifier
{
    void Apply(Troop troop);
    bool IsExpired { get; }
    void Update(Troop troop, GameTime gameTime);
}