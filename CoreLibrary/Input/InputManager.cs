using Microsoft.Xna.Framework;

namespace CoreLibrary.Input;

public class InputManager
{
    public KeyboardStatus Keyboard {get; private set;}
    public MouseStatus Mouse {get; private set;}

    public InputManager()
    {
        Keyboard = new KeyboardStatus();
        Mouse = new MouseStatus();
    }

    public void Update(GameTime gameTime)
    {
        Keyboard.Update(gameTime);
        Mouse.Update(gameTime);
    }

}