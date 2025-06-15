



using Microsoft.Xna.Framework;
using MyBakery;

public interface ShopObject
{
    Rectangle Hitbox { get; set; }
    Rectangle InteractZone { get; set; }
    int Quantity { get; set; }
    Shop.ShopObjectTypes Type { get; set; }
}