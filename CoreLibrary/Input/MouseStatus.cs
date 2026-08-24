using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CoreLibrary.Input;

public enum MouseButton
{
    Left,
    Middle,
    Right
}

public class MouseStatus
{
    public MouseState CurrentMouseState {get; private set;}
    public MouseState PreviousMouseState {get; private set;}
    Point mousePos;
    Point clickPoint;
    public Point PositionDelta => CurrentMouseState.Position - PreviousMouseState.Position;
    public int XDelta => CurrentMouseState.X - PreviousMouseState.X;
    public int YDelta => CurrentMouseState.Y - PreviousMouseState.Y;
    public bool WasMoved => PositionDelta != Point.Zero;

    public MouseState CheckMouse()
    {
        PreviousMouseState = CurrentMouseState;
        CurrentMouseState = Mouse.GetState();
        mousePos = new Point(CurrentMouseState.X, CurrentMouseState.Y);
        return CurrentMouseState;
    }
    public bool CheckLeftPress()
    {
        clickPoint = MouseLocation();
        return CurrentMouseState.LeftButton == ButtonState.Pressed && PreviousMouseState.LeftButton == ButtonState.Released;
    }
    public bool IsDragging()
    {
        return CurrentMouseState.LeftButton == ButtonState.Pressed && PreviousMouseState.LeftButton == ButtonState.Pressed && WasMoved;
    }
    public bool CheckLeftRelease()
    {
        return CurrentMouseState.LeftButton == ButtonState.Released && PreviousMouseState.LeftButton == ButtonState.Pressed;
    }
    public Point MouseLocation()
    {
        return mousePos;
    }

    public bool IsButtonDown(MouseButton button)
    {
        switch (button)
        {
            case MouseButton.Left:
              return CurrentMouseState.LeftButton == ButtonState.Pressed;
            case MouseButton.Middle:
              return CurrentMouseState.MiddleButton == ButtonState.Pressed;
            case MouseButton.Right:
              return CurrentMouseState.RightButton == ButtonState.Pressed;
            default:
                return false;
        }
        
    }

    public void Update(GameTime gameTime)
    {
        CheckMouse();
    }
}