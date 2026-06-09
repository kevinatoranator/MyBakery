using Microsoft.Xna.Framework;
using GeneralUtil;
using System;
using CoreLibrary.Graphics;

namespace MyBakery;

public class UIButton : Button
{

    public UIButton(string name, Vector2 location, int width, int height, Action onClick)
    {
        Name = name;
        Location = location;
        Hitbox = new Rectangle((int)Location.X, (int)Location.Y, width, height);
        this.onClick = onClick;
    }

    
}

