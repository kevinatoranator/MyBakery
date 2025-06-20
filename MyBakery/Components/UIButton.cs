using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GeneralUtil;
using System;
using System.Collections.Generic;

namespace MyBakery;

public class UIButton : Button
{

    public UIButton(string name, Sprite sprite, Vector2 location, Action onClick)
    {
        Name = name;
        Sprite = sprite;
        Location = location;
        Hitbox = new Rectangle((int)Location.X, (int)Location.Y, Sprite.TextureMapLocation.Width, Sprite.TextureMapLocation.Height);
        this.onClick = onClick;
    }

    
}

