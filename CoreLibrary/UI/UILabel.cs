using System;
using CoreLibrary;
using CoreLibrary.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class UILabel : UIElement
{

    private string _text;
    private SpriteFont _font;
    private bool _centered;
    private Vector2 _textPosition;
    public Color TextColor;
    public UILabel(Rectangle bounds, TextureRegion textureRegion, string text, SpriteFont font, bool centered = true) : base(bounds, textureRegion)
    {
        _text = text;
        _font = font;
        _centered = centered;

        TextColor = Color.Black;
    }

    public override void Draw(GameTime gameTime)
    {
        Core.SpriteBatch.DrawString(_font, _text, _textPosition, TextColor);
    }

    public override void Update(GameTime gameTime)
    {
        if (_centered)
        {
            _textPosition = new Vector2(Location.X + (Bounds.Width - _font.MeasureString(_text).X)/2, Location.Y + (Bounds.Height - _font.MeasureString(_text).Y)/2);
        }
        else
        {
            _textPosition = Location;
        }
    }
}

//FUTURE TODO
/*
Multirow word wrapper text if text length exceeds bounds horizontally
DropShadow from title screen -> Core.SpriteBatch.DrawString(_font, TITLE_TEXT, _titleLocation + new Vector2(10, 10), dropShadowColor, 0.0f, _titleOrigin, 1.0f, SpriteEffects.None, 1.0f);
Make resizable based on text length or hardcoded (resizable bool?)
*/