using GeneralUtil;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CoreLibrary.Graphics;

namespace GeneralUtil;

public abstract class PhysicsObject
{

    protected float _x, _y;
    public float velocityX, velocityY;
    public Sprite sprite;
    public float rotation;
    public Rectangle hitBox;
    public Color color = Color.White;
    public float scale = 1.0f;
        //public Circle hitBox;


    public void UpdateLocation() {
        _x += velocityX;
        _y += velocityY;
            //hitBox = new Quadrilateral(new Vector2((int)x, (int)y), new Vector2((int)x + sprite.Texture.Width, (int)y), new Vector2((int)x, (int)y + sprite.Texture.Height), new Vector2((int)x + sprite.Texture.Width, (int)y + sprite.Texture.Height));
        hitBox = new Rectangle((int)_x, (int)_y, (int)(sprite.Region.Width*scale), (int)(sprite.Region.Height*scale));
        //hitBox = new Circle(new Vector2(x + sprite.Texture.Width / 2, y + sprite.Texture.Height / 2), sprite.Texture.Width / 2);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
         sprite.Draw(spriteBatch, new Vector2(_x, _y));
    }

    public float X{
        get => _x;
        set => _x = value;
    }
    public float Y{
        get => _y;
        set => _y = value;
    }
}

