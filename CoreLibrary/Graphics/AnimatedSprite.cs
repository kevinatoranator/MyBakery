using System;
using Microsoft.Xna.Framework;

namespace CoreLibrary.Graphics;

public class AnimatedSprite : Sprite
{
    public int CurrentFrame;
    private TimeSpan _elapsed;
    private Animation _animation;

    public Animation Animation
    {
        get => _animation;
        set
        {
            _animation = value;
            Region = _animation.Frames[0];
        }
    }

    public AnimatedSprite()
    {
        
    }

    public AnimatedSprite(Animation animation)
    {
        Animation = animation;
    }

    public void Update(GameTime gameTime)
    {
        _elapsed += gameTime.ElapsedGameTime;

        if(_elapsed >= _animation.Delay && CurrentFrame < _animation.Frames.Count-1)
        {
            _elapsed -= _animation.Delay;
            CurrentFrame++;
        }

        if(CurrentFrame >= _animation.Frames.Count && _animation.Loop)
        {
            CurrentFrame = 0;
        }
        Region = _animation.Frames[CurrentFrame];
    }
}