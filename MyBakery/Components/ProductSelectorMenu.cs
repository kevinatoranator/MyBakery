

using System;
using System.Collections.Generic;
using CoreLibrary.Graphics;
using GeneralUtil;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyBakery;

public class ProductSelectorMenu : Component
{
    private List<Button> _buttons;
    private Vector2 _position;
    private SpriteFont _font;
    public String selectedProduct;
    public Boolean Clicked;
    private Dictionary<String, Product> _products;

    //is Visible bool?
    public ProductSelectorMenu(SpriteFont font, Vector2 position, Dictionary<String, Product> products) {

        _buttons = new List<Button>();
        _position = position;
        _font = font;
        _products = products;
        selectedProduct = "None";
        Clicked = false;

        int number = 1;
        foreach (KeyValuePair<String, Product> item in _products) {
            int quantity;
            GameManager.PlayerInfo.inventory.TryGetValue(item.Key, out quantity);
            if (quantity > 0)
            {
                Button b = new UIButton(item.Key.ToString(), new Vector2(position.X, position.Y + 64 * number), 64, 64, () =>
                {
                    selectedProduct = item.Key;
                    Clicked = true;
                });
                _buttons.Add(b);
                number += 1;
            }
        }
        Button cancel = new UIButton("Cancel", new Vector2(position.X, position.Y + (64 * number)), 128, 64, () =>
        {
            selectedProduct = "None";
            Clicked = true;
        });
        _buttons.Add(cancel);
    }

    public override void Draw(SpriteBatch spriteBatch, TextureAtlas _spriteSheet)
    {
        foreach(UIButton b in _buttons){
            b.Draw(spriteBatch, _font, _spriteSheet.CreateSprite("Button"));
        }
    }

    public override void Update(GameTime gameTime)
    {
        foreach(UIButton b in _buttons){
            b.Update();
        }
    }
}