


using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CoreLibrary.Graphics;

namespace MyBakery;

public abstract class Minigame
{
    protected int gameXOrigin = (int)GameManager.bottomScreenOrigin.X;
    protected int gameYOrigin = (int)GameManager.bottomScreenOrigin.Y;


    protected class FallingObject
    {
        Vector2 _location;
        int _fallSpeed;
        String _type;

        public Vector2 location
        {
            get => _location;
            set => _location = value;
        }
        public int fallSpeed
        {
            get => _fallSpeed;
            set => _fallSpeed = value;
        }
        public String type
        {
            get => _type;
            set => _type = value;
        }

        public Rectangle hitBox
        {
            get => new Rectangle((int)location.X, (int)location.Y, 64, 64);
        }
    }

    public abstract void Start(TextureAtlas spriteSheet, Texture2D background);
    public abstract void Draw(SpriteFont font, SpriteBatch spriteBatch);
    public abstract void Update(GameTime gameTime);
}