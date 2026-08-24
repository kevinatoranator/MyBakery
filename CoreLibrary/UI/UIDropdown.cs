using System;
using System.Collections.Generic;
using System.Linq;
using CoreLibrary;
using CoreLibrary.Graphics;
using Microsoft.Xna.Framework;

public class UIDropdown : UIElement
{
    private List<UIButton> _options;
    public UIButton Selected;
    public bool Opened;

    public UIDropdown(Rectangle bounds, TextureRegion textureRegion, List<UIButton> options) : base(bounds, textureRegion)
    {
        _options = options;
        Selected = options[0];
        Opened = false;
    }

    public override void Draw(GameTime gameTime)
    {
        
        if (Opened)
        {
            foreach(UIButton b in _options){
                b.Draw(gameTime);
            }
        }
        else
        {
            Selected.Draw(gameTime);
        }
    }

    public override void Update(GameTime gameTime)
    {
        if(Opened)
        {
            foreach(UIButton b in _options.ToList()){
                b.Update(gameTime);
                if (b.IsClicked())
                {
                    Selected = new UIButton(new Rectangle((int)Location.X, (int)Location.Y, b.Bounds.Width, b.Bounds.Height),
             b.TextureRegion, b.Text, b.Font, () => {Opened = true;});
                    Bounds.Height = Selected.Bounds.Height;
                    Opened = false;
                }
            }
        }
        else
        {
            if (IsClicked())
            {
                Bounds.Height = Selected.Bounds.Height * _options.Count;
                Opened = true;
            }
            Selected.Update(gameTime);
        } 
    }
}