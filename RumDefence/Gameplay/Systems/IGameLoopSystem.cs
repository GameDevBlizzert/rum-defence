using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public interface IGameLoopSystem
{
    
    public virtual void Load() { }
    public virtual void Update(GameTime gameTime) { }
    
}