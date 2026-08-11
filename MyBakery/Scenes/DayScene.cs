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
    private Sprite _coin, _chocoChip, _dough, _cookie, _orange, _cherry, _apple, _jelly, _coffeeBean, _flour, _wheat, _boxedChocolate, _cancel;
    private Texture2D  button, bakery;
    private SpriteFont _font;
    public TextureAtlas Atlas;
    public Dictionary<string, Product> ItemDB = new Dictionary<string, Product>();
    private Scene _lowerScene;
    private List<Scene> _upperTabs;
    private Boolean _shopIsOpen;

    public override void Initialize()
    {
        

        Rectangle screenBounds = Core.GraphicsDevice.PresentationParameters.Bounds;
        InventoryBounds = new Rectangle(0, 0, screenBounds.Width / 3, screenBounds.Height);
        BakeryBounds = new Rectangle(screenBounds.Width / 3, 0, screenBounds.Width * 2 / 3, screenBounds.Height / 2);
        GameBounds = new Rectangle(screenBounds.Width / 3, screenBounds.Height / 2, screenBounds.Width * 2 / 3, screenBounds.Height / 2);

        _shopIsOpen = false;

        _lowerScene = new SelectionScene(this);
        _upperTabs = [new Shop(this), new Farm(this)];

        base.Initialize();
    }

    public override void LoadContent()
    {

        button = Content.Load<Texture2D>("Button"); 
        bakery = Content.Load<Texture2D>("Bakery1");
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
        Console.WriteLine(itemsFilePath);
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

        if(_shopIsOpen)
            _lowerScene.Update(gameTime);

        foreach(Scene tab in _upperTabs)
        {
            tab.Update(gameTime);
            if(tab is Shop shop)
            {
                _shopIsOpen = shop.IsOpen;
            }
        }
    }

    public override void Draw(GameTime gameTime)
    {
        Core.GraphicsDevice.Clear(Color.CornflowerBlue);

        Core.SpriteBatch.Begin(samplerState : SamplerState.PointClamp);

        _upperTabs[0].Draw(gameTime);

        if(_shopIsOpen)
            _lowerScene.Draw(gameTime);

        //Inventory
        //SpriteBatch.Draw(whiteBox, new Rectangle(0, 0, gameXOrigin, Graphics.PreferredBackBufferHeight), Color.Gray);
        int inventoryNum = 0;
        foreach (KeyValuePair<string, int> inv in GameManager.PlayerInfo.inventory)
        {
            Atlas.GetRegion(inv.Key).Draw(Core.SpriteBatch, new Vector2(20, inventoryNum * 80), Color.White);
            Core.SpriteBatch.DrawString(_font,inv.Value.ToString(), new Vector2(90, inventoryNum * 80 + 24), Color.Black);
            inventoryNum++;
        }

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