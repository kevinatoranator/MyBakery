


using System;
using System.Collections.Generic;
using GeneralUtil;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CoreLibrary.Graphics;
using CoreLibrary;
using CoreLibrary.Scenes;
using MyBakery.Scenes;

namespace MyBakery;

public class Shop : Scene{

    public List<ShopObject> placedShopObjects; //placed shopObjects
    private UIButton _dayStartButton;//Start button
    //or
    //List<Sprite> shoptTiles;
    private Dropdown displayMenu;
    public Dictionary<String, int> placeableShopObjects; //List of placedable
    public List<Employee> employees; //List of employees
    private Sprite _register, _employee, _display, _button, _shopBackground;
    private Boolean employeeMenuOpen, mouseOnDisplay;
    public Boolean IsOpen;
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
    private DayScene _dayScene;
    private SpriteFont _font;
    private int dayTimeLeft, previousTime;
    private double elapsedDayTime;
    const int dayLength = 120;

    public Shop(DayScene dayScene) : base()
    {
        _dayScene = dayScene;
    }

    public override void Initialize()
    {
        employeeMenuOpen = false;
        placeableShopObjects = new Dictionary<String, int>() { {"Display", 2}, {"Fridge", 1}};
        IsOpen = false;
        previousTime = dayLength;
        elapsedDayTime = 0;

        base.Initialize();
    }

    public override void LoadContent()
    {
        _register = _dayScene.Atlas.CreateSprite("Register");
        _employee = _dayScene.Atlas.CreateSprite("ToastDog");
        _display = _dayScene.Atlas.CreateSprite("Display");
        _button = _dayScene.Atlas.CreateSprite("Button");
        Texture2D bg = Core.Content.Load<Texture2D>("Bakery1");
        _shopBackground = new Sprite(new TextureRegion(bg, _dayScene.BakeryBounds.X, _dayScene.BakeryBounds.Y, bg.Width, bg.Height));
        _font = Content.Load<SpriteFont>("font");
        _dayStartButton = new UIButton("Start Day", new Vector2(_dayScene.GameBounds.Right / 2, _dayScene.GameBounds.Bottom / 2), (int)_button.Width, (int)_button.Height,  () =>
        {
            IsOpen = true;
        });

        Register register = new Register(new Vector2(_dayScene.BakeryBounds.Left + 10, 50), _font, new Rectangle(0, 64, 64, 64), () => Console.WriteLine("reg"));
        register.onClick = () => { mouseOnDisplay = true;
            employeeMenuOpen = true;
            register.buttons.Add(new UIButton("Employee1", new Vector2(_dayScene.BakeryBounds.Width, _dayScene.BakeryBounds.Height / 2), (int)_employee.Width, (int)_employee.Height, () => employeeMenuOpen = false)); };
        placedShopObjects = new List<ShopObject>() { register };
    }

    public override void Update(GameTime gameTime)
    {
        mouseOnDisplay = false;
        bool displayWasOpen = false;
        if (!IsOpen)
        {
            _dayStartButton.Update();
        }
        else
        {
            elapsedDayTime += gameTime.ElapsedGameTime.TotalMilliseconds;
            dayTimeLeft = dayLength - (int)elapsedDayTime / 1000;
            if (dayTimeLeft < 0)
            {
                dayTimeLeft = dayLength;
                elapsedDayTime = 0;
            }
            foreach (ShopObject sobject in placedShopObjects)
            {

                if (previousTime != dayTimeLeft)
                {//remove when sell function is moved to customers
                    if (sobject.Type == "Display")
                    {
                        BakeryDisplay display = sobject as BakeryDisplay;
                        if (display.product != "None")
                        {
                            if (GameManager.PlayerInfo.inventory[display.product] < 1)
                            {
                                display.product = "None";
                                //display.Text = "Select\nProduct";
                            }
                            else
                            {
                                _dayScene.ItemDB[display.product].Sell(display.product, 1);
                            }
                        }
                    }
                }
            }
            previousTime = dayTimeLeft;
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
                if (!IsOpen)
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
        if (Core.Input.Mouse.CheckLeftPress())
        {
            Boolean occupied = false;
            Vector2 mouseLocation = new Vector2(Mouse.GetState().Position.X, Mouse.GetState().Y);

            if (displayMenu == null && mouseLocation.X > _dayScene.BakeryBounds.Right / 3 && mouseLocation.Y < _dayScene.BakeryBounds.Bottom / 2 && !mouseOnDisplay && !displayWasOpen)
            {
                displayMenu = new Dropdown(placeableShopObjects, _font, mouseLocation);
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
                            occupied = true;//Checking if placememt is va;od
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
    public override void Draw(GameTime gameTime){
        
        _shopBackground.Draw(Core.SpriteBatch, new Vector2(_dayScene.BakeryBounds.Left, 0));

        
        if(!IsOpen)
            _dayStartButton.Draw(Core.SpriteBatch, _font, _button);
        else
            Core.SpriteBatch.DrawString(_font, "Time Left in Day: " + dayTimeLeft / 60 + " Minutes " + dayTimeLeft % 60 + " Seconds", new Vector2(_dayScene.BakeryBounds.X + 10, _dayScene.BakeryBounds.Y + 30), Color.White);
        foreach(ShopObject shopObject in placedShopObjects){
            shopObject.Draw(Core.SpriteBatch, _dayScene.Atlas);
        }
        if(displayMenu != null){
            displayMenu.Draw(Core.SpriteBatch, _dayScene.Atlas);
        }
    }

    public void PlaceShopObject(Vector2 location, string type){
        HashSet<Product.ProductQualities> qualities = new HashSet<Product.ProductQualities>();
        if (type == "Fridge")
            qualities.UnionWith(new[] { Product.ProductQualities.Refrigerated});
        else if (type == "Shelf")
            qualities.UnionWith(new[] { Product.ProductQualities.Stackable});
            placedShopObjects.Add(new BakeryDisplay(location, _font, new Rectangle((int)location.X, (int)(location.Y + 64), 64, 64), type, qualities, _dayScene.ItemDB));
    }

}