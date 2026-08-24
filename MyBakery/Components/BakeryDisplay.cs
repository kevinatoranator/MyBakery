

using System;
using System.Collections.Generic;
using GeneralUtil;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CoreLibrary.Graphics;

namespace MyBakery;

public class BakeryDisplay : UIButton, ShopObject
{

    public String product {get; set;}
    public UIDropdown menu;
    private SpriteFont _font;
    public int Quantity { get; set; }
    public Rectangle InteractZone { get; set; } 
    public String Type { get; set; }
    public HashSet<Product.ProductQualities> DisplayQualities { get; set; }
    public Rectangle Hitbox { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    private Dictionary<String, Product> _products;

    public BakeryDisplay(Vector2 location, SpriteFont font, Rectangle izone, String type, HashSet<Product.ProductQualities> displayQualities, Dictionary<String, Product> _items, TextureRegion tex, Action action) : base(izone, tex, action)
    {
        Location = location;
        _font = font;
        Hitbox = new Rectangle((int)Location.X, (int)Location.Y, 64, 64);
        InteractZone = izone;
        Type = type;
        DisplayQualities = displayQualities;
        product = "None";
        _products = new Dictionary<String, Product>();
        foreach (KeyValuePair<String, Product> values in _items)
        {
            bool valid = true;
            foreach (Product.ProductQualities qual in values.Value.ProductQualitiesSet)
            {
                if (!DisplayQualities.Contains(qual))
                    valid = false;
                break;
            }
            if (valid && values.Value.Sellable)
                _products[values.Key] = values.Value;
        }
    }

    public void Update(GameTime gameTime)
    {
        if (menu != null)
        {
            menu.Update(gameTime);
            product = menu.Selected.ToString();
        }
    }

    public void Draw(GameTime gameTime){
        //spriteSheet.CreateSprite(Type).Draw(spriteBatch, Location);
        if(menu != null){
            menu.Draw(gameTime);
        }
        
        if(product is not "None"){
            //spriteSheet.CreateSprite(product).Draw(spriteBatch, Location);
        }
    }
}