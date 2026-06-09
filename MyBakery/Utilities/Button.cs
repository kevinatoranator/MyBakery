using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyBakery;
using CoreLibrary.Graphics;

namespace GeneralUtil;


public abstract class Button{

    public Rectangle Hitbox { get; set;}
    public string Name { get; set;}//character name
    public Vector2 Location { get; set;}
    
    public Action onClick;

    public Boolean IsClicked()
    {

        Rectangle mouseLoc = new Rectangle(Mouse.GetState().Position.X, Mouse.GetState().Position.Y, 1, 1);
        if (Hitbox.Intersects(mouseLoc) && GameManager.MouseClicked)
        {//Should be changed so that this can be independent library from game
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