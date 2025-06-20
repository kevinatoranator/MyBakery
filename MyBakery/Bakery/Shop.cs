


using System;
using System.Collections.Generic;
using GeneralUtil;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MyBakery;

public class Shop{

    public List<ShopObject> placedShopObjects; //placed shopObjects
    UIButton dayStartButton;//Start button
    Sprite shopSprite;
    SpriteFont font;
    //or
    //List<Sprite> shoptTiles;
    private Dropdown displayMenu;
    public Dictionary<ShopObjectTypes, int> placeableShopObjects; //List of placedable
    public List<Employee> employees; //List of employees
    private Texture2D spriteSheet; //Maybe remove
    private Boolean employeeMenuOpen,mouseOnDisplay;
    public enum ShopObjectTypes
    {
        Counter,
        Door,
        Display,
        Fridge,
        Register,
        None
    }

    public Shop(Sprite sprite, UIButton dayStartButton, Texture2D spriteSheet, SpriteFont font)
    {
        this.dayStartButton = dayStartButton;
        shopSprite = sprite;
        this.spriteSheet = spriteSheet;
        employeeMenuOpen = false;
        this.font = font;
        placeableShopObjects = new Dictionary<ShopObjectTypes, int>() { {ShopObjectTypes.Display, 2}, {ShopObjectTypes.Fridge, 1}};
        Register register = new Register(BakeryManager.BakeryTextureDB[ShopObjectTypes.Register], new Vector2(GameManager.gameWidth / 3 + 10, 50), font, new Rectangle(0, 64, 64, 64), () => Console.WriteLine("reg"));
        register.onClick = () => { mouseOnDisplay = true;
            employeeMenuOpen = true;
            register.buttons.Add(new UIButton("Employee1", new Sprite(spriteSheet, new Rectangle(128, 64, 64, 64)), new Vector2(GameManager.gameWidth / 2, GameManager.gameHeight / 4), () => employeeMenuOpen = false)); };
        placedShopObjects = new List<ShopObject>() { register };
    }

    public void Update(GameTime gameTime)
    {
        mouseOnDisplay = false;
        bool displayWasOpen = false;
        if (!BakeryManager.IsOpen)
        {
            dayStartButton.Update();
        }
        foreach (ShopObject shopObject in placedShopObjects)
        {
            if (shopObject.Type == ShopObjectTypes.Display || shopObject.Type == ShopObjectTypes.Fridge)//can these be generalized
            {
                BakeryDisplay display = shopObject as BakeryDisplay;
                if (display.menu != null)
                    displayWasOpen = true;//Better fix for this?
                display.Update(gameTime);
                if (display.IsClicked())
                {
                    mouseOnDisplay = true;
                    display.onClick.Invoke();
                }

            }
            else if (shopObject.Type == ShopObjectTypes.Register)
            {
                Register register = shopObject as Register;
                if (!BakeryManager.IsOpen)
                {
                    if (register.isOpened)
                    {
                        displayWasOpen = true;
                    }
                    register.Update();
                }
            }
            shopObject.Update(gameTime);
        }
        if (GameManager.MouseClicked)
        {
            Boolean occupied = false;
            Vector2 mouseLocation = new Vector2(Mouse.GetState().Position.X, Mouse.GetState().Y);

            if (displayMenu == null && mouseLocation.X > GameManager.gameWidth / 3 && mouseLocation.Y < GameManager.gameHeight / 2 && !mouseOnDisplay && !displayWasOpen)
            {
                displayMenu = new Dropdown(placeableShopObjects, font, mouseLocation);
            }
            else if (displayMenu != null)
            {
                displayMenu.Update(gameTime);
                if (displayMenu.selectedDisplay != ShopObjectTypes.None)
                {
                    foreach (ShopObject shopObject in placedShopObjects.ToArray())
                    {//should probably check when clicking before making menu but this works if displays are different sizes

                        if (shopObject.Hitbox.Intersects(new Rectangle(Mouse.GetState().Position.X, Mouse.GetState().Y, shopObject.Hitbox.Width, shopObject.Hitbox.Height)))
                        {
                            occupied = true;//Checking if place,emt os va;od
                        }
                    }
                    if (!occupied && placeableShopObjects[displayMenu.selectedDisplay] > 0)
                    {
                        PlaceShopObject(displayMenu.Location, displayMenu.selectedDisplay);
                        placeableShopObjects[displayMenu.selectedDisplay] -= 1;
                    }

                }
                if (displayMenu.Clicked)
                {
                    displayMenu = null;
                }
            }
        }
    }
    public void Draw(SpriteBatch spriteBatch, SpriteFont font){

        if (!employeeMenuOpen)
        {
            spriteBatch.Draw(shopSprite.Texture, new Rectangle(GameManager.gameWidth/3, 0, GameManager.gameWidth*2/3, GameManager.gameHeight/2), Color.White);
        } 
        
        if(!BakeryManager.IsOpen)
            dayStartButton.Draw(spriteBatch, font);
        foreach(ShopObject shopObject in placedShopObjects){
            shopObject.Draw(spriteBatch);
        }
        if(displayMenu != null){
            displayMenu.Draw(spriteBatch);
        }
    }

    public void PlaceShopObject(Vector2 location, ShopObjectTypes type){
        placedShopObjects.Add(new BakeryDisplay(BakeryManager.BakeryTextureDB[type], location, font, new Rectangle((int)location.X, (int)(location.Y + 64), 64, 64), type));
    }

}