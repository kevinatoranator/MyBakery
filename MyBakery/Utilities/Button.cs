using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyBakery;
using CoreLibrary.Graphics;
using CoreLibrary;
using CoreLibrary.Input;

namespace GeneralUtil;


public abstract class Button{

    public Rectangle Hitbox { get; set;}
    public string Name { get; set;}//character name
    public Vector2 Location { get; set;}
    
    public Action onClick;

    public Boolean IsClicked()
    {

        Rectangle mouseLoc = new Rectangle(Core.Input.Mouse.MouseLocation().X, Core.Input.Mouse.MouseLocation().Y, 1, 1);
        if (Hitbox.Intersects(mouseLoc) && Core.Input.Mouse.CheckLeftPress())
        {
            return true;
        }
        return false;
    }
    
    public void Update()
    {

        if (IsClicked())
        {
            onClick.Invoke();
        }
    }

    public void Draw(SpriteBatch spriteBatch, SpriteFont font, Sprite sprite)
    {
        sprite.Draw(spriteBatch, Location);
        spriteBatch.DrawString(font, Name, new Vector2(Location.X + Hitbox.Width / 6, Location.Y + Hitbox.Height / 3), Color.Black);//TEMP TEXT
    }
}