


using System;
using System.Collections.Generic;
using GeneralUtil;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CoreLibrary.Graphics;

namespace MyBakery;

public class Shop{

    public List<ShopObject> placedShopObjects; //placed shopObjects
    UIButton dayStartButton;//Start button
    Sprite shopSprite;
    SpriteFont font;
    //or
    //List<Sprite> shoptTiles;
    private Dropdown displayMenu;
    public Dictionary<String, int> placeableShopObjects; //List of placedable
    public List<Employee> employees; //List of employees
    private TextureAtlas _spriteSheet; //Maybe remove
    private Sprite _register, _employee, _display, _button;
    private Boolean employeeMenuOpen,mouseOnDisplay;
    public enum ShopObjectTypes
    {
        Counter,
        Door,
        Display,
        Fridge,
        Register,
        Shelf,
        None
    }

    public Shop(Sprite sprite, UIButton dayStartButton, TextureAtlas spriteSheet, SpriteFont font)
    {
        this.dayStartButton = dayStartButton;
        shopSprite = sprite;
        _spriteSheet = spriteSheet;
        _register = spriteSheet.CreateSprite("Register");
        _employee = spriteSheet.CreateSprite("ToastDog");
        _display = spriteSheet.CreateSprite("Display");
        _button = spriteSheet.CreateSprite("Button");
        employeeMenuOpen = false;
        this.font = font;
        placeableShopObjects = new Dictionary<String, int>() { {"Display", 2}, {"Fridge", 1}};
        Register register = new Register(new Vector2(GameManager.gameWidth / 3 + 10, 50), font, new Rectangle(0, 64, 64, 64), () => Console.WriteLine("reg"));
        register.onClick = () => { mouseOnDisplay = true;
            employeeMenuOpen = true;
            register.buttons.Add(new UIButton("Employee1", new Vector2(GameManager.gameWidth / 2, GameManager.gameHeight / 4), (int)_employee.Width, (int)_employee.Height, () => employeeMenuOpen = false)); };
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
            if (shopObject.Type == "Display" || shopObject.Type == "Fridge")//can these be generalized
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
            else if (shopObject.Type == "Register")
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
                if (displayMenu.selectedDisplay != "None")
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
            shopSprite.Draw(spriteBatch, new Vector2(GameManager.gameWidth/3, 0));
        } 
        
        if(!BakeryManager.IsOpen)
            dayStartButton.Draw(spriteBatch, font, _button);
        foreach(ShopObject shopObject in placedShopObjects){
            shopObject.Draw(spriteBatch, _spriteSheet);
        }
        if(displayMenu != null){
            displayMenu.Draw(spriteBatch, _spriteSheet);
        }
    }

    public void PlaceShopObject(Vector2 location, string type){
        HashSet<Product.ProductQualities> qualities = new HashSet<Product.ProductQualities>();
        if (type == "Fridge")
            qualities.UnionWith(new[] { Product.ProductQualities.Refrigerated});
        else if (type == "Shelf")
            qualities.UnionWith(new[] { Product.ProductQualities.Stackable});
            placedShopObjects.Add(new BakeryDisplay(location, font, new Rectangle((int)location.X, (int)(location.Y + 64), 64, 64), type, qualities));
    }

}