

using System;
using System.Collections.Generic;
using GeneralUtil;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyBakery;

public class BakeryDisplay : Button, ShopObject
{

    public GameManager.Items product {get; set;}
    public ProductSelectorMenu menu;
    private SpriteFont _font;
    public int Quantity { get; set; }
    public Rectangle InteractZone { get; set; }
    public Shop.ShopObjectTypes Type { get; set; }
    public HashSet<Product.ProductQualities> DisplayQualities { get; set; }
    private Dictionary<GameManager.Items, Product> _products;

    public BakeryDisplay(Sprite sprite, Vector2 location, SpriteFont font, Rectangle izone, Shop.ShopObjectTypes type, HashSet<Product.ProductQualities> displayQualities)
    {
        Sprite = sprite;
        Location = location;
        _font = font;
        Hitbox = new Rectangle((int)Location.X, (int)Location.Y, Sprite.TextureMapLocation.Width, Sprite.TextureMapLocation.Height);
        InteractZone = izone;
        Type = type;
        DisplayQualities = displayQualities;
        _products = new Dictionary<GameManager.Items, Product>();
        foreach (KeyValuePair<GameManager.Items, Product> values in GameManager.ItemDB)
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

    public void Draw(SpriteBatch spriteBatch){
        spriteBatch.Draw(Sprite.Texture, Location, Sprite.TextureMapLocation, Color.White);
        if(menu != null){
            menu.Draw(spriteBatch);
        }
        
        if(product is not GameManager.Items.None){
            spriteBatch.Draw(GameManager.TextureDB[product].Texture, Location, GameManager.TextureDB[product].TextureMapLocation, Color.White);
        }
    }
}