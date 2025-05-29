

using System;
using System.Collections.Generic;
using GeneralUtil;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyBakery;

public class ProductSelectorMenu : Component
{

    private List<Product> _products;
    private List<Button> _buttons;
    private Vector2 _position;
    private SpriteFont _font;
    public Product selectedProduct;
    public Boolean Clicked;

    //is Visible bool?
    public ProductSelectorMenu(List<Product> products, SpriteFont font, Vector2 position){
        _products = products;
        _buttons = new List<Button>();
        _position = position;
        _font = font;
        selectedProduct = null;
        Clicked = false;

        int number = 1;
        foreach(Product p in _products){
            if(p.Sellable && p.Quantity > 0){
                Button b = new UIButton(p.Type.ToString(), p.Sprite, new Vector2(position.X, position.Y + (p.Sprite.TextureMapLocation.Height*number)));
                b.HitBox = new Rectangle((int)b.Location.X, (int)b.Location.Y, b.Sprite.TextureMapLocation.Width, b.Sprite.TextureMapLocation.Height);
                _buttons.Add(b);
                number += 1;
            }
        }
        Button cancel = new UIButton("Cancel", GameManager.cancelSprite, new Vector2(position.X, position.Y + (GameManager.cancelSprite.TextureMapLocation.Height*number)));
        cancel.HitBox = new Rectangle((int)cancel.Location.X, (int)cancel.Location.Y, cancel.Sprite.TextureMapLocation.Width,cancel.Sprite.TextureMapLocation.Height);
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
                    selectedProduct = null;
                }else{
                    foreach(Product product in _products){
                        if(product.Type.ToString() == b.Name){
                            selectedProduct = product;
                        }
                    }
                }
                Clicked = true;
            }
        }
    }
}