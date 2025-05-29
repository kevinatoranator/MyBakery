

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyBakery;


public class ProgressBar
{

    Texture2D foreground, background;
    Vector2 position, vertPosition;

    float maxValue, currentValue;
    Boolean vertical;
    Rectangle progress;

    public ProgressBar(Texture2D fg, Texture2D bg, float max, float start, Vector2 pos, Boolean vert){
        foreground = fg;
        background = bg;
        maxValue = max;
        currentValue = start;
        position = pos;
        vertPosition = pos;
        vertical = vert;
        progress = new Rectangle(0, 0, foreground.Width, foreground.Height);
    }
    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(background, position, Color.White);
        if(vertical)
            spriteBatch.Draw(foreground, vertPosition, progress, Color.White);
        else
            spriteBatch.Draw(foreground, position, progress, Color.White);
    }

    public void Update(float value)
    {
        currentValue = value;
        if(currentValue > maxValue)
            currentValue = maxValue;
        if(currentValue < 0)
            currentValue = 0;
        if(vertical){
            progress.Y = foreground.Height - progress.Height;
            progress.Height =  (int)(currentValue / maxValue * foreground.Height);
            vertPosition.Y = position.Y + foreground.Height - progress.Height;
        }else
            progress.Width = (int)(currentValue / maxValue * foreground.Width);
    }
}