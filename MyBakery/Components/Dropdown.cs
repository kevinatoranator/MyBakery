
using System;
using System.Collections.Generic;
using GeneralUtil;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyBakery;

public class Dropdown : Component
{
    //only thing changed is type of dropdown from product to bakerydisplay just needs name and sprite and location of menu
    private Dictionary<Shop.ShopObjectTypes, int> _shopObjects;//changed
    private List<Button> _buttons;
    private Vector2 _position;
    private SpriteFont _font;
    public Shop.ShopObjectTypes selectedDisplay;
    public Boolean Clicked;

    public Dropdown(Dictionary<Shop.ShopObjectTypes, int> shopObjects, SpriteFont font, Vector2 position){
        _shopObjects = shopObjects;
        _buttons = new List<Button>();
        _position = position;
        _font = font;
        selectedDisplay = Shop.ShopObjectTypes.None;
        Clicked = false;

        int number = 0; //changed to spawn where clicked instead of offset down one
        foreach(KeyValuePair<Shop.ShopObjectTypes, int> shopObject in _shopObjects){
            if (shopObject.Value > 0)
            {
                Button b = new UIButton(shopObject.Key.ToString(), BakeryManager.BakeryTextureDB[shopObject.Key], new Vector2(position.X, position.Y + (BakeryManager.BakeryTextureDB[shopObject.Key].TextureMapLocation.Height * number)), () =>
                {
                    selectedDisplay = shopObject.Key;
                    Clicked = true;
                });
                b.Hitbox = new Rectangle((int)b.Location.X, (int)b.Location.Y, b.Sprite.TextureMapLocation.Width, b.Sprite.TextureMapLocation.Height);
                _buttons.Add(b);
                number += 1;
            }  
        }
        Button cancel = new UIButton("Cancel", GameManager.cancelSprite, new Vector2(position.X, position.Y + (GameManager.cancelSprite.TextureMapLocation.Height*number)), () => { selectedDisplay = Shop.ShopObjectTypes.None; Clicked = true; });
        cancel.Hitbox = new Rectangle((int)cancel.Location.X, (int)cancel.Location.Y, cancel.Sprite.TextureMapLocation.Width,cancel.Sprite.TextureMapLocation.Height);
        _buttons.Add(cancel);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        foreach(UIButton b in _buttons){
            b.Draw(spriteBatch, _font);
        }
    }

    public override void Update(GameTime gameTime)
    {
        foreach(UIButton b in _buttons){
            b.Update();
        }
    }

    public Vector2 Location{
        get{ return _position; }
        set{_position = value;}
    }
}