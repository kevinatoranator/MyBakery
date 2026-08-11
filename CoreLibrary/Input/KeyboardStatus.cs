using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CoreLibrary.Input;

public class KeyboardStatus
{
    KeyboardState currentKeyState;
     KeyboardState previousKeyState;

    public KeyboardState CheckKey()
    {
        previousKeyState = currentKeyState;
        currentKeyState = Keyboard.GetState();
        return currentKeyState;
    }
    public bool CheckKeyRelease(Keys key)
    {
        return currentKeyState.IsKeyUp(key) && previousKeyState.IsKeyDown(key);
    }

    public bool CheckKeyPress(Keys key)
    {
        return currentKeyState.IsKeyDown(key) && previousKeyState.IsKeyUp(key);
    }
    public bool CheckKeyDown(Keys key)
    {
        return currentKeyState.IsKeyDown(key);
    }

    public void Update(GameTime gameTime)
    {
        CheckKey();
    }
}

