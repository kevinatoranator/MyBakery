

using System;
using GeneralUtil;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CoreLibrary.Graphics;

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
        progress = new Rectangle(foreground.Region.SourceRectangle.X, foreground.Region.SourceRectangle.Y, foreground.Region.Width, foreground.Region.Height);
    }
    public void Draw(SpriteBatch spriteBatch)
    {
        background.Draw(spriteBatch, position);
        foreground.Region.SourceRectangle = progress;
        if(vertical)
            foreground.Draw(spriteBatch, vertPosition);
        else
            foreground.Draw(spriteBatch, position);
    }

    public void Update(float value)
    {
        currentValue = value;
        if(currentValue > maxValue)
            currentValue = maxValue;
        if(currentValue < 0)
            currentValue = 0;
        if(vertical){
            progress.Height =  (int)(currentValue / maxValue * foreground.Region.Height);
            progress.Y = foreground.Region.SourceRectangle.Y + foreground.Region.Height - progress.Height;
            vertPosition.Y = position.Y + foreground.Region.Height - progress.Height;
        }else
            progress.Width = (int)(currentValue / maxValue * foreground.Region.Width);
    }
}