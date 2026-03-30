using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using RumDefence;

namespace Rum_Defence.Input;

public class InputManager
{
    private MouseState currentMouse;
    private MouseState previousMouse;

    public Vector2 MousePosition { get; private set; }
    public Vector2 MousePositionScaled { get; private set; }

    public void Update()
    {
        previousMouse = currentMouse;
        currentMouse = Mouse.GetState();

        MousePosition = new Vector2(currentMouse.X, currentMouse.Y);

        MousePositionScaled = ScreenManager.GetMousePositionScaled();
    }

    public bool IsLeftClick()
    {
        return currentMouse.LeftButton == ButtonState.Pressed &&
               previousMouse.LeftButton == ButtonState.Released;
    }
}