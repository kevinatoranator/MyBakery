using CoreLibrary;
using CoreLibrary.Graphics;
using Microsoft.Xna.Framework;

public class UISprite : UIElement
{
    private Sprite _sprite;
    public UISprite(Rectangle bounds, TextureRegion textureRegion, Sprite sprite) : base(bounds, textureRegion)
    {
        _sprite = sprite;
    }

    public override void Draw(GameTime gameTime)
    {
        _sprite.Draw(Core.SpriteBatch, Location);
    }

    public override void Update(GameTime gameTime)
    {
        
    }
}

//FUTURE TODO
/*
Trim image based on bounds or scale to fit
*/