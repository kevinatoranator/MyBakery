

using System;
using System.Collections.Generic;
using GeneralUtil;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyBakery;

public class Register : Button, ShopObject
{
    public Rectangle InteractZone { get; set; }
    public int Quantity { get; set; }
    public Shop.ShopObjectTypes Type { get; set; }
    private SpriteFont _font;
    public Employee employee{ get; set; }
    public List<Button> buttons;
    public Boolean isOpened { get; set; }


    public Register(Sprite sprite, Vector2 location, SpriteFont font, Rectangle izone, Action onClick)
    {
        Sprite = sprite;
        Location = location;
        _font = font;
        Hitbox = new Rectangle((int)Location.X, (int)Location.Y, Sprite.TextureMapLocation.Width, Sprite.TextureMapLocation.Height);
        InteractZone = izone;
        Type = Shop.ShopObjectTypes.Register;
        buttons = new List<Button>();
        this.onClick = onClick;
        isOpened = false;
    }

    new public void Update()
    {
        if (IsClicked())
        {
            onClick.Invoke();
            isOpened = true;
        }
        foreach(Button b in buttons) {
            if (b.IsClicked())
            {
                b.onClick.Invoke();
                employee = new Employee(b.Name, b.Sprite, 10);
                isOpened = false;
                buttons.Clear();//may change in the future if assigning to or managing staff
                break;
            }
        }
    }

    public void Update(GameTime gametime)
    {
        //While shop is running
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (Button button in buttons)
        {
            button.Draw(spriteBatch, _font);
        }
        if (employee != null)
            spriteBatch.Draw(employee.Sprite.Texture, new Vector2(Location.X, Location.Y - 32), employee.Sprite.TextureMapLocation, Color.White);
            
        spriteBatch.Draw(Sprite.Texture, Location, Sprite.TextureMapLocation, Color.White);
    }
}