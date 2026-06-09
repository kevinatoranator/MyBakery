
using System;
using System.Collections.Generic;
using CoreLibrary.Graphics;
using GeneralUtil;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyBakery;

public class Dropdown : Component
{
    //only thing changed is type of dropdown from product to bakerydisplay just needs name and sprite and location of menu
    private Dictionary<String, int> _shopObjects;//changed
    private List<Button> _buttons;
    private Vector2 _position;
    private SpriteFont _font;
    public String selectedDisplay;
    public Boolean Clicked;

    public Dropdown(Dictionary<String, int> shopObjects, SpriteFont font, Vector2 position){
        _shopObjects = shopObjects;
        _buttons = new List<Button>();
        _position = position;
        _font = font;
        selectedDisplay = "None";
        Clicked = false;

        int number = 0; //changed to spawn where clicked instead of offset down one
        foreach(KeyValuePair<String, int> shopObject in _shopObjects){
            if (shopObject.Value > 0)
            {
                Button b = new UIButton(shopObject.Key.ToString(), new Vector2(position.X, position.Y + (64 * number)), 64, 64, () =>
                {
                    selectedDisplay = shopObject.Key;
                    Clicked = true;
                });
                _buttons.Add(b);
                number += 1;
            }  
        }
        Button cancel = new UIButton("Cancel", new Vector2(position.X, position.Y + (GameManager.cancelSprite.Height*number)), 128, 64, () => { selectedDisplay = "None"; Clicked = true; });
        _buttons.Add(cancel);
    }

    public override void Draw(SpriteBatch spriteBatch, TextureAtlas spriteSheet)
    {
        foreach(UIButton b in _buttons){
            b.Draw(spriteBatch, _font, new Sprite(spriteSheet.GetRegion("Button")));
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