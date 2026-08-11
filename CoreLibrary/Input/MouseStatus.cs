using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CoreLibrary.Input;

public class MouseStatus
{
    MouseState currentMouseState;
    MouseState previousMouseState;
    Point mousePos;
    Point clickPoint;

    public MouseState CheckMouse()
    {
        previousMouseState = currentMouseState;
        currentMouseState = Mouse.GetState();
        mousePos = new Point(currentMouseState.X, currentMouseState.Y);
        return currentMouseState;
    }
    public bool CheckLeftPress()
    {
        clickPoint = MouseLocation();
        return currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released;
    }
    public bool IsDragging()
    {
        return currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Pressed;
    }
    public bool CheckLeftRelease()
    {
        return currentMouseState.LeftButton == ButtonState.Released && previousMouseState.LeftButton == ButtonState.Pressed;
    }
    public Point MouseLocation()
    {
        return mousePos;
    }

    public void Update(GameTime gameTime)
    {
        CheckMouse();
    }
}