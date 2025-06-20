

using System;
using System.Collections.Generic;
using GeneralUtil;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyBakery;

public class ProductSelectorMenu : Component
{
    private List<Button> _buttons;
    private Vector2 _position;
    private SpriteFont _font;
    public GameManager.Items selectedProduct;
    public Boolean Clicked;

    //is Visible bool?
    public ProductSelectorMenu( SpriteFont font, Vector2 position){
        
        _buttons = new List<Button>();
        _position = position;
        _font = font;
        selectedProduct = GameManager.Items.None;
        Clicked = false;

        int number = 1;
        foreach(KeyValuePair<GameManager.Items, Product> item in GameManager.ItemDB){
            int quantity;
            GameManager.PlayerInfo.inventory.TryGetValue(item.Key, out quantity);
            if (item.Value.Sellable && quantity > 0)
            {
                Button b = new UIButton(item.Key.ToString(), GameManager.TextureDB[item.Key], new Vector2(position.X, position.Y + (GameManager.TextureDB[item.Key].TextureMapLocation.Height * number)), () =>
                {
                    foreach (KeyValuePair<GameManager.Items, Product> itemdb in GameManager.ItemDB)
                    {
                        if (itemdb.Key == item.Key)
                        {
                            selectedProduct = item.Key;
                        }
                    }
                    Clicked = true;
                });
                b.Hitbox = new Rectangle((int)b.Location.X, (int)b.Location.Y, b.Sprite.TextureMapLocation.Width, b.Sprite.TextureMapLocation.Height);
                _buttons.Add(b);
                number += 1;
            }
        }
        Button cancel = new UIButton("Cancel", GameManager.cancelSprite, new Vector2(position.X, position.Y + (GameManager.cancelSprite.TextureMapLocation.Height*number)), () =>
        {
            selectedProduct = GameManager.Items.None;
            Clicked = true;
        });
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
}