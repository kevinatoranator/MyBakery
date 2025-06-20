



using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyBakery;

public interface ShopObject
{
    Rectangle Hitbox { get; set; }
    Rectangle InteractZone { get; set; }
    int Quantity { get; set; }
    Shop.ShopObjectTypes Type { get; set; }

    public void Update(GameTime gameTime);
    public void Draw(SpriteBatch spriteBatch);
}