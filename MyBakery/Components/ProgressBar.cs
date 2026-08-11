

using System;
using GeneralUtil;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CoreLibrary.Graphics;

namespace MyBakery;


public class ProgressBar
{

    Sprite foreground, background, progressBar;
    Vector2 position, vertPosition;

    float maxValue, currentValue;
    Boolean vertical;
    Rectangle progress;
    int baseWidth, baseHeight, baseY;

    public ProgressBar(Sprite fg, Sprite bg, float max, float start, Vector2 pos, Boolean vert){
        foreground = fg;
        background = bg;
        maxValue = max;
        currentValue = start;
        position = pos;
        vertPosition = pos;
        vertical = vert;
        progress = new Rectangle(foreground.Region.SourceRectangle.X, foreground.Region.SourceRectangle.Y, foreground.Region.Width, foreground.Region.Height);
        baseWidth = foreground.Region.Width;
        baseHeight = foreground.Region.Height;
        baseY = foreground.Region.SourceRectangle.Y;
        progressBar = new Sprite(new TextureRegion(foreground.Region.Texture, progress.X, progress.Y, progress.Width, progress.Height));
    }
    public void Draw(SpriteBatch spriteBatch)
    {
        background.Draw(spriteBatch, position);
        progressBar.Region.SourceRectangle = progress;
        if(vertical)
            progressBar.Draw(spriteBatch, vertPosition);
        else
            progressBar.Draw(spriteBatch, position);
    }

    public void Update(float value)
    {
        currentValue = value;
        if(currentValue > maxValue)
            currentValue = maxValue;
        if(currentValue < 0)
            currentValue = 0;
        if(vertical){
            progress.Height =  (int)(currentValue / maxValue * baseHeight);
            progress.Y = baseY + baseHeight - progress.Height; // Source Rectangle
            vertPosition.Y = position.Y + baseHeight - progress.Height;
        }else
            progress.Width = (int)(currentValue / maxValue * baseWidth);
    }
}