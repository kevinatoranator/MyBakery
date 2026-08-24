using System;
using CoreLibrary;
using CoreLibrary.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class UIButton : UIElement
{
    private Action _onClick;
    private Sprite _sprite;
    public string Text {get; private set;}
    public SpriteFont Font {get; private set;}
    public UIButton(Rectangle bounds, TextureRegion texture, Action onClick) : base(bounds, texture)
    {
        _onClick = onClick;
        _sprite = new Sprite(TextureRegion);
    }
    public UIButton(Rectangle bounds, TextureRegion texture, string text, SpriteFont font, Action onClick) : base(bounds, texture)
    {
        _onClick = onClick;
        _sprite = new Sprite(TextureRegion);
        Text = text;
        Font = font;
    }
    public override void Update(GameTime gameTime)
    {
        if(IsClicked())
            _onClick.Invoke();
        
    }
    public override void Draw(GameTime gameTime)
    {
        _sprite.Draw(Core.SpriteBatch, Location);
        if(Text != null)
        {
            Core.SpriteBatch.DrawString(Font, Text, new Vector2(Location.X + Bounds.Width/6, Location.Y + Bounds.Height/2), Color.Black);
        }
    }
}