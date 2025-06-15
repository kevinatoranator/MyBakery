
using System;
using System.Collections.Generic;
using GeneralUtil;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyBakery;

public class Dropdown : Component
{
    //only thing changed is type of dropdown from product to bakerydisplay just needs name and sprite and location of menu
    private List<ShopObject> _shopObjects;//changed
    private List<Button> _buttons;
    private Vector2 _position;
    private SpriteFont _font;
    public BakeryDisplay selectedDisplay;//changed
    public Boolean Clicked;

    public Dropdown(List<ShopObject> shopObjects, SpriteFont font, Vector2 position){
        _shopObjects = shopObjects;
        _buttons = new List<Button>();
        _position = position;
        _font = font;
        selectedDisplay = null;
        Clicked = false;

        int number = 0; //changed to spawn where clicked instead of offset down one
        foreach(ShopObject shopObject in _shopObjects){
            if (shopObject.Type == Shop.ShopObjectTypes.Display)
            {
                BakeryDisplay display = shopObject as BakeryDisplay;
                if (display.Quantity > 0)
                {
                    Button b = new UIButton(display.Name, display.Sprite, new Vector2(position.X, position.Y + (display.Sprite.TextureMapLocation.Height * number)));
                    b.Hitbox = new Rectangle((int)b.Location.X, (int)b.Location.Y, b.Sprite.TextureMapLocation.Width, b.Sprite.TextureMapLocation.Height);
                    _buttons.Add(b);
                    number += 1;
                }
            }
            
        }
        Button cancel = new UIButton("Cancel", GameManager.cancelSprite, new Vector2(position.X, position.Y + (GameManager.cancelSprite.TextureMapLocation.Height*number)));
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
            b.Update(gameTime);
            if(b.IsClicked()){
                if(b.Name == "Cancel"){
                    selectedDisplay = null;
                }else{
                    foreach(ShopObject shopObject in _shopObjects){
                        if (shopObject.Type == Shop.ShopObjectTypes.Display)
                        {
                            BakeryDisplay display = shopObject as BakeryDisplay;
                           if (display.Name == b.Name)
                            {
                                selectedDisplay = display;
                            } 
                        }
                        
                    }
                }
                Clicked = true;
            }
        }
    }

    public Vector2 Location{
        get{ return _position; }
        set{_position = value;}
    }
}