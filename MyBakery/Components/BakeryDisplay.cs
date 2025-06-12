

using System;
using GeneralUtil;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyBakery;

public class BakeryDisplay : Button
{

    public GameManager.Items product {get; set;}
    public ProductSelectorMenu menu;
    private SpriteFont _font;
    public int Quantity { get; set; }
    public BakeryDisplay(Sprite sprite, Vector2 location, SpriteFont font)
    {
        Sprite = sprite;
        Location = location;
        _font = font;
        HitBox = new Rectangle((int)Location.X, (int)Location.Y, Sprite.TextureMapLocation.Width, Sprite.TextureMapLocation.Height);
    }

    public void Update(GameTime gameTime){
        if(IsClicked() && menu == null){
            menu = new ProductSelectorMenu(_font, Location);
        }else if(IsClicked() && menu != null){
            menu = null;
        }
        if(menu != null){
            menu.Update(gameTime);
            product = menu.selectedProduct;
            if(menu.Clicked){
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