

using System;
using System.Collections.Generic;
using GeneralUtil;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CoreLibrary.Graphics;

namespace MyBakery;

public class BakeryDisplay : Button, ShopObject
{

    public String product {get; set;}
    public ProductSelectorMenu menu;
    private SpriteFont _font;
    public int Quantity { get; set; }
    public Rectangle InteractZone { get; set; } 
    public String Type { get; set; }
    public HashSet<Product.ProductQualities> DisplayQualities { get; set; }
    private Dictionary<String, Product> _products;

    public BakeryDisplay(Vector2 location, SpriteFont font, Rectangle izone, String type, HashSet<Product.ProductQualities> displayQualities)
    {
        Location = location;
        _font = font;
        Hitbox = new Rectangle((int)Location.X, (int)Location.Y, 64, 64);
        InteractZone = izone;
        Type = type;
        DisplayQualities = displayQualities;
        _products = new Dictionary<String, Product>();
        foreach (KeyValuePair<String, Product> values in GameManager.ItemDB)
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
        onClick = () =>
        {
            if (menu == null)
            {
                menu = new ProductSelectorMenu(_font, Location, _products);
            }
            else if (menu != null)
            {
                menu = null;
            }
        };
    }

    public void Update(GameTime gameTime)
    {
        if (menu != null)
        {
            menu.Update(gameTime);
            product = menu.selectedProduct;
            if (menu.Clicked)
            {
                menu = null;
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch, TextureAtlas spriteSheet){
        spriteSheet.CreateSprite(Type).Draw(spriteBatch, Location);
        if(menu != null){
            menu.Draw(spriteBatch, spriteSheet);
        }
        
        if(product is not "None"){
            spriteSheet.CreateSprite(product).Draw(spriteBatch, Location);
        }
    }
}