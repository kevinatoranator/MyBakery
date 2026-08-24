


using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CoreLibrary.Graphics;
using CoreLibrary;
using CoreLibrary.Scenes;
using MyBakery.Scenes;
using System.Linq;

namespace MyBakery;

public class Shop : Scene{

    public List<ShopObject> placedShopObjects; //placed shopObjects
    private UIButton _dayStartButton;//Start button
    //or
    //List<Sprite> shoptTiles;
    private List<UIElement> _UIElements;
    public Dictionary<string, int> placeableShopObjects; //List of placedable
    public List<Employee> employees; //List of employees
    private Sprite _register, _employee, _display, _button, _shopBackground;
    private bool _menuOpen;
    public bool ShopOpen, isActive;
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
        _menuOpen = false;
        placeableShopObjects = new Dictionary<String, int>() { {"Display", 2}, {"Fridge", 1}};
        ShopOpen = false;
        previousTime = dayLength;
        elapsedDayTime = 0;
        isActive = true;

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

        _UIElements = new List<UIElement>();
        _dayStartButton = new UIButton(new Rectangle(_dayScene.GameBounds.Right / 2, _dayScene.GameBounds.Bottom / 2, (int)_button.Width, (int)_button.Height), 
        _button.Region, "Start Day", _font,  () =>
        {
            ShopOpen = true;
            _UIElements.Remove(_dayStartButton);
            if (_menuOpen)//temp solution for bug if you start game with dropdown open it becomes locked 
                {
                    //close menu
                    _UIElements.RemoveAt(_UIElements.Count-1);//temp test since the last element in theory could be anything, find away to find the actually dropdown
                    _menuOpen = false;
                }
        });
        _UIElements.Add(_dayStartButton);
        UIButton register = new UIButton(new Rectangle(_dayScene.BakeryBounds.Left + 10, 50, 64, 64), _register.Region, () => { Console.WriteLine("Register clicked");});
        //placedShopObjects = new List<ShopObject>() { register };
    }

    public override void Update(GameTime gameTime)
    {
        if(isActive){
            foreach(UIElement element in _UIElements.ToList())
            {
                element.Update(gameTime);
            }
        }
        if (ShopOpen)
        {
            elapsedDayTime += gameTime.ElapsedGameTime.TotalMilliseconds;
            dayTimeLeft = dayLength - (int)elapsedDayTime / 1000;
            if (dayTimeLeft < 0)
            {
                dayTimeLeft = dayLength;
                elapsedDayTime = 0;
                ShopOpen = false;
                _UIElements.Add(_dayStartButton);
            } 
        }
        else
        {
            if (Core.Input.Mouse.CheckLeftPress() && _dayScene.BakeryBounds.Contains(Core.Input.Mouse.MouseLocation()) && isActive)
            {
                if (_menuOpen)
                {
                    //close menu
                    _UIElements.RemoveAt(_UIElements.Count-1);//temp test since the last element in theory could be anything, find away to find the actually dropdown
                    _menuOpen = false;
                }
                else
                {
                    List<UIButton> shopObjectList = new List<UIButton>();
                    int buttonCount = 0;
                    foreach (KeyValuePair<string, int> sobject in placeableShopObjects)
                    {
                        if(sobject.Value > 0)
                        {
                            Sprite _buttonSprite = _dayScene.Atlas.CreateSprite(sobject.Key);
                            shopObjectList.Add(new UIButton(new Rectangle(Core.Input.Mouse.MouseLocation().X, (int)(Core.Input.Mouse.MouseLocation().Y + buttonCount * _buttonSprite.Height + 1), (int)_buttonSprite.Width, (int)_buttonSprite.Height),
                            _buttonSprite.Region, () => { Console.WriteLine("Clicked" + sobject.Key);}));
                            buttonCount++;
                        }
                        
                    }
                    _UIElements.Add(new UIDropdown(new Rectangle(Core.Input.Mouse.MouseLocation().X, Core.Input.Mouse.MouseLocation().Y, shopObjectList[0].TextureRegion.Width, shopObjectList[0].TextureRegion.Height), _button.Region, shopObjectList){Opened = true});
                    _menuOpen = true;
                }
            }
        }
    }
    public override void Draw(GameTime gameTime){
        
        _shopBackground.Draw(Core.SpriteBatch, new Vector2(_dayScene.BakeryBounds.Left, 0));

        
        if(ShopOpen)
            Core.SpriteBatch.DrawString(_font, "Time Left in Day: " + dayTimeLeft / 60 + " Minutes " + dayTimeLeft % 60 + " Seconds", new Vector2(_dayScene.BakeryBounds.X + 10, _dayScene.BakeryBounds.Y + 30), Color.White); 
        else
            _dayStartButton.Draw(gameTime);
        foreach(UIElement element in _UIElements.ToList())
        {
            element.Draw(gameTime);
        }
        //foreach(ShopObject shopObject in placedShopObjects){
            //shopObject.Draw(Core.SpriteBatch, _dayScene.Atlas);
        //}
    }

    public void PlaceShopObject(Vector2 location, string type){
        HashSet<Product.ProductQualities> qualities = new HashSet<Product.ProductQualities>();
        if (type == "Fridge")
            qualities.UnionWith(new[] { Product.ProductQualities.Refrigerated});
        else if (type == "Shelf")
            qualities.UnionWith(new[] { Product.ProductQualities.Stackable});
            //placedShopObjects.Add(new BakeryDisplay(location, _font, new Rectangle((int)location.X, (int)(location.Y + 64), 64, 64), type, qualities, _dayScene.ItemDB));
    }

}