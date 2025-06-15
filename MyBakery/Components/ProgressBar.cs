

using System;
using GeneralUtil;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyBakery;


public class ProgressBar
{

    Sprite foreground, background;
    Vector2 position, vertPosition;

    float maxValue, currentValue;
    Boolean vertical;
    Rectangle progress;

    public ProgressBar(Sprite fg, Sprite bg, float max, float start, Vector2 pos, Boolean vert){
        foreground = fg;
        background = bg;
        maxValue = max;
        currentValue = start;
        position = pos;
        vertPosition = pos;
        vertical = vert;
        progress = new Rectangle(foreground.TextureMapLocation.X, foreground.TextureMapLocation.Y, foreground.TextureMapLocation.Width, foreground.TextureMapLocation.Height);
    }
    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(background.Texture, position, background.TextureMapLocation, Color.White);
        if(vertical)
            spriteBatch.Draw(foreground.Texture, vertPosition, progress, Color.White);
        else
            spriteBatch.Draw(foreground.Texture, position, progress, Color.White);
    }

    public void Update(float value)
    {
        currentValue = value;
        if(currentValue > maxValue)
            currentValue = maxValue;
        if(currentValue < 0)
            currentValue = 0;
        if(vertical){
            progress.Height =  (int)(currentValue / maxValue * foreground.TextureMapLocation.Height);
            progress.Y = foreground.TextureMapLocation.Y + foreground.TextureMapLocation.Height - progress.Height;
            vertPosition.Y = position.Y + foreground.TextureMapLocation.Height - progress.Height;
        }else
            progress.Width = (int)(currentValue / maxValue * foreground.TextureMapLocation.Width);
    }
}