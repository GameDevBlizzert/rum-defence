using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace RumDefence;

public class InputManager
{
    private static readonly Dictionary<string, Keys> Defaults = new()
    {
        ["Pause"] = Keys.Escape,
        ["MultiPlace"] = Keys.LeftControl,
        ["LevelNext"] = Keys.D,
        ["LevelPrev"] = Keys.A,
        ["Upgrade"] = Keys.U,
        ["Repair"] = Keys.R,
        ["TogglePause"] = Keys.Space,
        ["ToggleFastForward"] = Keys.F,
    };

    public static IReadOnlyDictionary<string, Keys> DefaultBindings => Defaults;

    public static InputManager Instance { get; } = new InputManager();

    private MouseState currentMouse;
    private MouseState previousMouse;
    private KeyboardState currentKeyboard;
    private KeyboardState previousKeyboard;

    private Dictionary<string, Keys> bindings = new(Defaults);

    public Vector2 MousePosition { get; private set; }
    public Vector2 MousePositionScaled { get; private set; }

    public void Update()
    {
        previousMouse = currentMouse;
        currentMouse = Mouse.GetState();
        previousKeyboard = currentKeyboard;
        currentKeyboard = Keyboard.GetState();

        MousePosition = new Vector2(currentMouse.X, currentMouse.Y);
        MousePositionScaled = ScreenManager.GetMousePositionScaled();
    }

    public bool IsLeftClick() =>
        currentMouse.LeftButton == ButtonState.Pressed &&
        previousMouse.LeftButton == ButtonState.Released;

    public bool IsRightClick() =>
        currentMouse.RightButton == ButtonState.Pressed &&
        previousMouse.RightButton == ButtonState.Released;

    public bool IsActionJustPressed(string action) =>
        bindings.TryGetValue(action, out var key) &&
        currentKeyboard.IsKeyDown(key) && !previousKeyboard.IsKeyDown(key);

    public bool IsActionHeld(string action) =>
        bindings.TryGetValue(action, out var key) && currentKeyboard.IsKeyDown(key);

    // MultiPlace also accepts RightControl regardless of binding.
    public bool IsCtrlHeld() =>
        IsActionHeld("MultiPlace") || currentKeyboard.IsKeyDown(Keys.RightControl);

    public bool IsAnyKeyJustPressed(out Keys key)
    {
        foreach (var k in currentKeyboard.GetPressedKeys())
        {
            if (!previousKeyboard.IsKeyDown(k))
            {
                key = k;
                return true;
            }
        }
        key = Keys.None;
        return false;
    }

    public Keys GetBinding(string action) =>
        bindings.TryGetValue(action, out var key) ? key : Defaults.GetValueOrDefault(action, Keys.None);

    public void SetBinding(string action, Keys key) => bindings[action] = key;

    public void LoadFromSave()
    {
        bindings = new Dictionary<string, Keys>(Defaults);
        foreach (var (action, keyName) in SaveManager.CurrentSave.KeyBindings)
        {
            if (Enum.TryParse<Keys>(keyName, out var key))
                bindings[action] = key;
        }
    }

    public void SaveToSave()
    {
        SaveManager.CurrentSave.KeyBindings = bindings
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString());
        SaveManager.Save();
    }

    public static string GetKeyDisplayName(Keys key) => key switch
    {
        Keys.LeftControl => "L.Ctrl",
        Keys.RightControl => "R.Ctrl",
        Keys.LeftShift => "L.Shift",
        Keys.RightShift => "R.Shift",
        Keys.LeftAlt => "L.Alt",
        Keys.RightAlt => "R.Alt",
        Keys.Escape => "Escape",
        Keys.Space => "Space",
        Keys.Enter => "Enter",
        Keys.Back => "Backspace",
        Keys.Delete => "Delete",
        Keys.Up => "Up",
        Keys.Down => "Down",
        Keys.Left => "Left",
        Keys.Right => "Right",
        Keys.Tab => "Tab",
        Keys.OemComma => ",",
        Keys.OemPeriod => ".",
        Keys.OemSemicolon => ";",
        _ => key.ToString()
    };
}
