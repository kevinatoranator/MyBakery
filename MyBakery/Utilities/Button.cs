using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyBakery;

namespace GeneralUtil;


public abstract class Button{

    public Rectangle HitBox { get; set;}
    public string Name { get; set;}//character name
    public Sprite Sprite { get; set;}
    public Vector2 Location { get; set;}

    public Boolean IsClicked(){
        
        Rectangle mouseLoc = new Rectangle(Mouse.GetState().Position.X, Mouse.GetState().Position.Y, 1, 1);
        if(HitBox.Intersects(mouseLoc) && GameManager.MouseClicked){
            return true;
        }
        return false;
    }

    public void Draw(SpriteBatch spriteBatch, SpriteFont font){
        spriteBatch.Draw(Sprite.Texture, Location, Sprite.TextureMapLocation, Color.White, 0.0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
        spriteBatch.DrawString(font, Name, new Vector2(Location.X + HitBox.Width/6, Location.Y + HitBox.Height/3), Color.Black);//TEMP TEXT
    }
}