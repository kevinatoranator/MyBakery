using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CoreLibrary;
using CoreLibrary.Graphics;
using CoreLibrary.Scenes;
using System.Text.Json;
using System.IO;

namespace MyBakery.Scenes;

public class DayScene : Scene{

    public Rectangle InventoryBounds, BakeryBounds, GameBounds;
    private Sprite _coin, _chocoChip, _dough, _cookie, _orange, _cherry, _apple, _jelly, _coffeeBean, _flour, _wheat, _boxedChocolate, _cancel, _button;
    private SpriteFont _font;
    public TextureAtlas Atlas;
    public Dictionary<string, Product> ItemDB = new Dictionary<string, Product>();
    private Scene _lowerScene;
    private List<Scene> _upperTabs;
    public bool DayStarted;
    private List<UIElement> _UIElements;

    public override void Initialize()
    {
        

        Rectangle screenBounds = Core.GraphicsDevice.PresentationParameters.Bounds;
        InventoryBounds = new Rectangle(0, 0, screenBounds.Width / 3, screenBounds.Height);
        BakeryBounds = new Rectangle(screenBounds.Width / 3, 0, screenBounds.Width * 2 / 3, screenBounds.Height / 2);
        GameBounds = new Rectangle(screenBounds.Width / 3, screenBounds.Height / 2, screenBounds.Width * 2 / 3, screenBounds.Height / 2);

        DayStarted = false;

        _lowerScene = new SelectionScene(this);
        _upperTabs = [new Shop(this), new Farm(this)];

        base.Initialize();
    }

    public override void LoadContent()
    {
        _font = Content.Load<SpriteFont>("font");

        Atlas = TextureAtlas.FromFile(Core.Content, "atlas-definition.xml");

        _coin = Atlas.CreateSprite("Coin");
        _chocoChip = Atlas.CreateSprite("ChocoChip");
        _dough = Atlas.CreateSprite("Dough");
        _cookie = Atlas.CreateSprite("Cookie");
        _apple = Atlas.CreateSprite("Apple");
        _cherry = Atlas.CreateSprite("Cherry");
        _orange = Atlas.CreateSprite("Orange");
        _jelly = Atlas.CreateSprite("Jelly");
        _coffeeBean = Atlas.CreateSprite("CoffeeBean");
        _flour = Atlas.CreateSprite("Flour");
        _wheat = Atlas.CreateSprite("Wheat");
        _boxedChocolate = Atlas.CreateSprite("BoxedChocolate");
        _cancel = Atlas.CreateSprite("Cancel");
        _button = Atlas.CreateSprite("Button");

        _UIElements = new List<UIElement>();
        TextureRegion testpanel = Atlas.CreateSprite("Panel").Region;
         _UIElements.Add(new UIPanel(new Rectangle(16, 16, InventoryBounds.Right-16, InventoryBounds.Bottom-16), testpanel));

        _UIElements.Add(new UIButton(new Rectangle(BakeryBounds.X, BakeryBounds.Y, (int)_button.Width, (int)_button.Height), _button.Region, "Shop", _font,
         () => {if(_upperTabs[0] is not Shop)
             {
                 (_upperTabs[0], _upperTabs[1]) = (_upperTabs[1], _upperTabs[0]);
                 ((Shop)_upperTabs[0]).isActive = true;
                 ((Farm)_upperTabs[1]).isActive = false;
             }
         }));
        _UIElements.Add(new UIButton(new Rectangle((int)(BakeryBounds.X + _button.Width), BakeryBounds.Y, (int)_button.Width, (int)_button.Height), _button.Region, "Farm", _font,
         () => {if(_upperTabs[0] is not Farm)
             {
                 (_upperTabs[0], _upperTabs[1]) = (_upperTabs[1], _upperTabs[0]);
                 ((Farm)_upperTabs[0]).isActive = true;
                 ((Shop)_upperTabs[1]).isActive = false;
             }}));
         
         //load vals
        string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string projectDirectory = Path.GetFullPath(Path.Combine(exeDirectory, @"..\..\..\"));
        string dataDirectory = Path.Combine(projectDirectory, "Data");
        string jsonFilePath = Path.Combine(dataDirectory, "playerProfile.json");
        if(File.Exists(jsonFilePath)){
            string json = File.ReadAllText(jsonFilePath);
            if (json != "")
            {
                GameManager.PlayerInfo = JsonSerializer.Deserialize<PlayerProfile>(json);
            }
        }else{
                GameManager.PlayerInfo.inventory["Coin"] = 10;
                GameManager.PlayerInfo.Name = "NewPlayer";
                Console.WriteLine("New Player Created");
        }

        string itemsFilePath = Path.Combine(dataDirectory, "items.json");
        //Console.WriteLine(itemsFilePath);
        if (File.Exists(itemsFilePath))
        {
            string json = File.ReadAllText(itemsFilePath);
            ItemDB = JsonSerializer.Deserialize<Dictionary<string, Product>>(json);
        }

        _lowerScene.Initialize();

        foreach(Scene tab in _upperTabs)
        {
            tab.Initialize();
        }
    }

    public override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)){
            string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string projectDirectory = Path.GetFullPath(Path.Combine(exeDirectory, @"..\..\..\"));
            string dataDirectory = Path.Combine(projectDirectory, "Data");
            
            Console.WriteLine("Saving");
            string fileName = "playerProfile.json";
             var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(GameManager.PlayerInfo, options);
            File.WriteAllText(Path.Combine(dataDirectory, fileName), jsonString);

            string fileRecipeName = "items.json";
            string jsonRecipes = JsonSerializer.Serialize(ItemDB, options);
            File.WriteAllText(Path.Combine(dataDirectory, fileRecipeName), jsonRecipes);
            Console.WriteLine("Saved");
        }   

        if(DayStarted)
            _lowerScene.Update(gameTime);

        int inventoryNum = 0;
        List<UIElement> contents = new List<UIElement>();
        foreach (KeyValuePair<string, int> inv in GameManager.PlayerInfo.inventory) //THese should probably be panels not 2 seperate elements
        {
            Sprite newSprite = Atlas.CreateSprite(inv.Key);
            contents.Add(new UISprite(new Rectangle(20, inventoryNum * 80, (int)newSprite.Width, (int)newSprite.Height), _button.Region, newSprite));
            contents.Add(new UILabel(new Rectangle(20 + (int)newSprite.Width, inventoryNum * 80, (int)newSprite.Width, (int)newSprite.Height), _button.Region, inv.Value.ToString(), _font));
            inventoryNum++;
        }
         ((UIPanel)_UIElements[0]).AddContents(contents);
        foreach(UIElement element in _UIElements)
        {
            element.Update(gameTime);
        }
        
        foreach(Scene tab in _upperTabs)
        {
            tab.Update(gameTime);
            if(tab is Shop shop)
            {
                DayStarted = shop.ShopOpen;
            }
        }
    }

    public override void Draw(GameTime gameTime)
    {
        Core.GraphicsDevice.Clear(Color.CornflowerBlue);

        Core.SpriteBatch.Begin(samplerState : SamplerState.PointClamp);

        _upperTabs[0].Draw(gameTime);
        

        if(DayStarted)
            _lowerScene.Draw(gameTime);

        //Inventory
        foreach(UIElement element in _UIElements)
        {
            element.Draw(gameTime);
        }
        //SpriteBatch.Draw(whiteBox, new Rectangle(0, 0, gameXOrigin, Graphics.PreferredBackBufferHeight), Color.Gray);
        Core.SpriteBatch.End();
    }

    public void ChangeLowerTab(Scene next)
    {
        if(_lowerScene != null)
        {
            _lowerScene.Dispose();
        }
        GC.Collect();
        _lowerScene = next;
        _lowerScene.Initialize();
    }
}