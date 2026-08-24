

using System;
using System.Collections.Generic;
using GeneralUtil;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CoreLibrary.Graphics;

namespace MyBakery;

public class Register : UIButton
{
    public Rectangle InteractZone { get; set; }
    public int Quantity { get; set; }
    public String Type { get; set; }
    private SpriteFont _font;
    public Employee employee{ get; set; }
    public List<UIButton> buttons;
    public Boolean isOpened { get; set; }


    public Register(Rectangle bounds, TextureRegion texture, Action onClick) : base(bounds, texture, onClick)
    {
        Type = "Register";
        buttons = new List<UIButton>();
        isOpened = false;
    }

    public void Update()
    {
        if (IsClicked())
        {
            //onClick.Invoke();
            isOpened = true;
        }
        foreach(UIButton b in buttons) {
            if (b.IsClicked())
            {
                //b.onClick.Invoke();
                employee = new Employee("Bob", 10); //temp name
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

    public void Draw(GameTime gameTime)
    {
        foreach (UIButton button in buttons)
        {
            button.Draw(gameTime);
        }
        //if (employee != null)
            //spriteSheet.CreateSprite("ToastDog").Draw(spriteBatch, new Vector2(Location.X, Location.Y - 32));//sprite made with employee.Name temp for testing
            
        //spriteSheet.CreateSprite(Type).Draw(spriteBatch, Location);
    }
}