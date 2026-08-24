using CoreLibrary;
using CoreLibrary.Graphics;
using CoreLibrary.Input;
using Microsoft.Xna.Framework;

public abstract class UIElement
{
    public Rectangle Bounds;
    public Vector2 Location;
    public TextureRegion TextureRegion;

    public UIElement(Rectangle bounds, TextureRegion textureRegion)
    {
        Bounds = bounds;
        TextureRegion = textureRegion;
        Location = new Vector2(bounds.X, bounds.Y);
    }
    public abstract void Update(GameTime gameTime);
    public abstract void Draw(GameTime gameTime);

    public bool isHovered()
    {
        if(Core.Input.Mouse.MouseLocation().X < Bounds.Right && Core.Input.Mouse.MouseLocation().X > Bounds.Left &&
        Core.Input.Mouse.MouseLocation().Y < Bounds.Bottom && Core.Input.Mouse.MouseLocation().Y > Bounds.Top)
            return true;
        return false;
    }
    public bool IsClicked()
    {
        if(Bounds.Contains(Core.Input.Mouse.MouseLocation()) && Core.Input.Mouse.CheckLeftPress())
            return true;
        return false;
    }
}