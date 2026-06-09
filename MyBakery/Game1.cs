using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CoreLibrary;
using CoreLibrary.Graphics;

namespace MyBakery;

public class Game1 : Core
{

    private SpriteFont font;
    private Texture2D spriteSheet, button, bakery, chocobg;
    private Sprite _coin, _chocoChip, _dough, _cookie, _orange, _cherry, _apple, _jelly, _coffeeBean, _flour, _wheat, _boxedChocolate, _cancel;
    const int spriteSize = 64;  

    public TextureAtlas atlas;

    public Game1() : base("My Bakery", 1920, 1080, false)
    {

    }

    protected override void Initialize()
    {

        base.Initialize();
    }

    protected override void LoadContent()
    {
        font = Content.Load<SpriteFont>("font");
        spriteSheet = Content.Load<Texture2D>("SpriteSheet");
        button = Content.Load<Texture2D>("Button"); 
        bakery = Content.Load<Texture2D>("Bakery1");
        chocobg = Content.Load<Texture2D>("chocofall_bg");

        //LATER REWRITE AS XML
        atlas = new TextureAtlas(spriteSheet);
        atlas.AddRegion("Coin", 128, 128, 64, 64);
        atlas.AddRegion("ChocoChip", 0, 64, 64, 64);
        atlas.AddRegion("Ember", 64, 64, 64, 64);
        atlas.AddRegion("Dough", 0, 128, 64, 64);
        atlas.AddRegion("Cookie", 64, 128, 64, 64);
        atlas.AddRegion("Apple", 0, 0, 64, 64);
        atlas.AddRegion("Cherry", 64, 0, 64, 64);
        atlas.AddRegion("Orange", 128, 0, 64, 64);
        atlas.AddRegion("Jelly", 128, 192, 64, 64);
        atlas.AddRegion("CoffeeBean", 256, 128, 64, 64);
        atlas.AddRegion("Flour", 320, 128, 64, 64);
        atlas.AddRegion("Wheat", 192, 64, 64, 64);
        atlas.AddRegion("BoxedChocolate", 128, 256, 64, 64);
        atlas.AddRegion("Cancel", 0, 192, 64, 64);
        atlas.AddRegion("Display", 0, 448, 64, 64);
        atlas.AddRegion("Fridge", 0, 512, 64, 64);
        atlas.AddRegion("Register", 64, 448, 64, 64);
        atlas.AddRegion("Dirt", 128, 512, 64, 64);
        atlas.AddRegion("ToastDog", 128, 64, 64, 64);
        atlas.AddRegion("Button", 448, 576, 128, 64);
        atlas.AddRegion("ProgressFront", 512, 256, 128, 64);
        atlas.AddRegion("ProgressBack", 192, 192, 128, 64);
        atlas.AddRegion("Pick", 0, 256, 64, 64);
        atlas.AddRegion("Hammer", 64, 256, 64, 64);
        atlas.AddRegion("Layer1", 128, 448, 32, 32);
        atlas.AddRegion("Layer2", 160, 448, 32, 32);
        atlas.AddRegion("Layer3", 128, 480, 32, 32);
        atlas.AddRegion("Layer0", 160, 480, 32, 32);
        atlas.AddRegion("Circle", 320, 256, 64, 64);
        atlas.AddRegion("Square", 384, 256, 64, 64);
        atlas.AddRegion("Star", 448, 256, 64, 64);
        atlas.AddRegion("Grinder", 384, 0, 256, 256);
        atlas.AddRegion("Wheel", 256, 0, 128, 128);
        atlas.AddRegion("Oven", 192, 320, 256, 256);
        atlas.AddRegion("Match", 64, 320, 64, 128);
        atlas.AddRegion("Wood", 128, 320, 64, 128);
        atlas.AddRegion("Ice", 0, 320, 64, 128);
        atlas.AddRegion("MatchBox", 320, 192, 64, 64);
        atlas.AddRegion("TempFront", 480, 320, 32, 128);
        atlas.AddRegion("TempBack", 448, 320, 32, 128);

        _coin = atlas.CreateSprite("Coin");
        _chocoChip = atlas.CreateSprite("ChocoChip");
        _dough = atlas.CreateSprite("Dough");
        _cookie = atlas.CreateSprite("Cookie");
        _apple = atlas.CreateSprite("Apple");
        _cherry = atlas.CreateSprite("Cherry");
        _orange = atlas.CreateSprite("Orange");
        _jelly = atlas.CreateSprite("Jelly");
        _coffeeBean = atlas.CreateSprite("CoffeeBean");
        _flour = atlas.CreateSprite("Flour");
        _wheat = atlas.CreateSprite("Wheat");
        _boxedChocolate = atlas.CreateSprite("BoxedChocolate");
        _cancel = atlas.CreateSprite("Cancel");


        GameManager.Initialize(atlas);
        BakeryManager.Initialize(button, atlas, bakery, font);
        MinigameManager.Initialize(button, atlas, chocobg);
        //load vals
        string jsonFilePath = "playerProfile.json";
        if(File.Exists(jsonFilePath)){
            string json = File.ReadAllText(jsonFilePath);
            if (json != "")
            {
                GameManager.PlayerInfo = JsonSerializer.Deserialize<PlayerProfile>(json);
            }
            else
            {
                GameManager.PlayerInfo.inventory["Coin"] = 10;
                GameManager.PlayerInfo.Name = "NewPlayer";
            }
           
        }

        string itemsFilePath = "items.json";
        if (File.Exists(itemsFilePath))
        {
            string json = File.ReadAllText(itemsFilePath);
            GameManager.ItemDB = JsonSerializer.Deserialize<Dictionary<string, Product>>(json);
        }
        base.LoadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)){
            string fileName = "playerProfile.json";
             var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(GameManager.PlayerInfo, options);
            File.WriteAllText(fileName, jsonString);

            string fileRecipeName = "items.json";
            string jsonRecipes = JsonSerializer.Serialize(GameManager.ItemDB, options);
            File.WriteAllText(fileRecipeName, jsonRecipes);
            Exit();
        }

        GameManager.Update(gameTime);
        MinigameManager.Update(gameTime);

        //Bakery updates
        if(GameManager.CurrentBakeryState == GameManager.BakeryState.Day){
            BakeryManager.IsOpen = true;
        }else{
            BakeryManager.IsOpen = false;
        }
        
        
        BakeryManager.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        int gameXOrigin = Graphics.PreferredBackBufferWidth/3;
        int gameYOrigin = Graphics.PreferredBackBufferHeight/2;


        SpriteBatch.Begin();
        
        //Minigame updates
        MinigameManager.Draw(font, SpriteBatch);
        BakeryManager.Draw(font, SpriteBatch);

        //Inventory BG
        //SpriteBatch.Draw(whiteBox, new Rectangle(0, 0, gameXOrigin, Graphics.PreferredBackBufferHeight), Color.Gray);


        //Inventory
        int inventoryNum = 0;
        foreach (KeyValuePair<string, int> inv in GameManager.PlayerInfo.inventory)
        {
            SpriteBatch.Draw(atlas.GetRegion(inv.Key).Texture, new Vector2(20, inventoryNum * 80), atlas.GetRegion(inv.Key).SourceRectangle, Color.White);
            SpriteBatch.DrawString(font,inv.Value.ToString(), new Vector2(90, inventoryNum * 80 + 24), Color.Black);
            inventoryNum++;
        }

        SpriteBatch.End();
        base.Draw(gameTime);
    }
}

public class KBoard
{
    static KeyboardState currentKeyState;
    static KeyboardState previousKeyState;

    public static KeyboardState CheckKey()
    {
        previousKeyState = currentKeyState;
        currentKeyState = Keyboard.GetState();
        return currentKeyState;
    }
    public static bool CheckKeyRelease(Keys key)
    {
        return currentKeyState.IsKeyDown(key) && !previousKeyState.IsKeyDown(key);
    }
}

public class KMouse
{
    static MouseState currentMouseState;
    static MouseState previousMouseState;
    static Point mousePos;
    static Point clickPoint;

    public static MouseState CheckMouse()
    {
        previousMouseState = currentMouseState;
        currentMouseState = Mouse.GetState();
        mousePos = new Point(currentMouseState.X, currentMouseState.Y);
        return currentMouseState;
    }
    public static bool CheckLeftPress()
    {
        clickPoint = MouseLocation();
        return currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released;
    }
    public static bool IsDragging()
    {
        return currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Pressed;
    }
    public static bool CheckLeftRelease()
    {
        return currentMouseState.LeftButton == ButtonState.Released && previousMouseState.LeftButton == ButtonState.Pressed;
    }
    public static Point MouseLocation()
    {
        return mousePos;
    }
}
