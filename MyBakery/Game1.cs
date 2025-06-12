using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MyBakery;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private SpriteFont font;
    private Texture2D whiteBox, spriteSheet, button, progressFront, progressBack, oven, match, wood, ice, matchBox, tempFront, tempBack, whisk, display1, bakery, chocobg;
    const int spriteSize = 64;
    

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        _graphics.PreferredBackBufferHeight = GameManager.gameHeight;
        _graphics.PreferredBackBufferWidth = GameManager.gameWidth;


    }

    protected override void Initialize()
    {

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        font = Content.Load<SpriteFont>("font");
        whiteBox = new Texture2D(_graphics.GraphicsDevice, 1, 1);
        whiteBox.SetData(new[] {Color.White});
        spriteSheet = Content.Load<Texture2D>("SpriteSheet");
        button = Content.Load<Texture2D>("Button"); 
        progressFront = Content.Load<Texture2D>("ProgressFront"); 
        progressBack = Content.Load<Texture2D>("ProgressBack");
        oven = Content.Load<Texture2D>("Oven"); 
        match = Content.Load<Texture2D>("Match"); 
        wood = Content.Load<Texture2D>("Wood"); 
        ice = Content.Load<Texture2D>("Ice");
        matchBox = Content.Load<Texture2D>("MatchBox"); 
        tempFront = Content.Load<Texture2D>("TempFront"); 
        tempBack = Content.Load<Texture2D>("TempBack");
        whisk = Content.Load<Texture2D>("Whisk"); 
        display1 = Content.Load<Texture2D>("display1");  
        bakery = Content.Load<Texture2D>("Bakery1");
        chocobg = Content.Load<Texture2D>("chocofall_bg");

        GameManager.Initialize(spriteSheet);
        BakeryManager.Initialize(GraphicsDevice, button, display1, bakery, font);
        MinigameManager.Initialize(GraphicsDevice, button);
        ChocoGame.Initialize(GraphicsDevice, spriteSheet, progressFront, progressBack, chocobg);
        BakingGame.Initialize(GraphicsDevice, spriteSheet, oven, match, wood, ice, matchBox, progressFront, progressBack, tempFront, tempBack);
        DoughGame.Initialize(GraphicsDevice, spriteSheet, whisk);
        FruitJumpGame.Initialize(GraphicsDevice, spriteSheet);
        JellyGame.Initialize(GraphicsDevice, spriteSheet);
        CoffeeSnakeGame.Initialize(spriteSheet);
        FlourGame.Initialize(GraphicsDevice, spriteSheet);
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
                GameManager.PlayerInfo.inventory[GameManager.Items.Coin] = 10;
                GameManager.PlayerInfo.Name = "NewPlayer";
            }
           
        }

        string itemsFilePath = "items.json";
        if(File.Exists(itemsFilePath)){
            string json = File.ReadAllText(itemsFilePath);
            GameManager.ItemDB = JsonSerializer.Deserialize<Dictionary<GameManager.Items, Product>>(json);
        }
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


        //Bakery updates
        if(GameManager.CurrentBakeryState == GameManager.BakeryState.Day){
            BakeryManager.IsOpen = true;
        }else{
            BakeryManager.IsOpen = false;
        }
        //Minigame updates
        switch(GameManager.CurrentMinigameState){
            case GameManager.MinigameState.Select:
                MinigameManager.Update(gameTime);
                break;
            case GameManager.MinigameState.ChocoLatte:
                ChocoGame.Update(gameTime);
                break;
            case GameManager.MinigameState.Baking:
                BakingGame.Update(gameTime);
                break;
            case GameManager.MinigameState.Dough:
                DoughGame.Update(gameTime);
                break;
            case GameManager.MinigameState.FruitJump:
                FruitJumpGame.Update(gameTime);
                break;
            case GameManager.MinigameState.JellyEater:
                JellyGame.Update(gameTime);
                break;
            case GameManager.MinigameState.CoffeeSnake:
                CoffeeSnakeGame.Update(gameTime);
                break;
            case GameManager.MinigameState.FlourGrind:
                FlourGame.Update(gameTime);
                break;
        }

        
        BakeryManager.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        int gameXOrigin = _graphics.PreferredBackBufferWidth/3;
        int gameYOrigin = _graphics.PreferredBackBufferHeight/2;


        _spriteBatch.Begin();
        //Inventory BG
        _spriteBatch.Draw(whiteBox, new Rectangle(0, 0, gameXOrigin, _graphics.PreferredBackBufferHeight), Color.Gray);


        //Inventory
        int inventoryNum = 0;
        foreach (KeyValuePair<GameManager.Items, int> inv in GameManager.PlayerInfo.inventory)
        {
            _spriteBatch.Draw(GameManager.TextureDB[inv.Key].Texture, new Vector2(20, inventoryNum * 80), GameManager.TextureDB[inv.Key].TextureMapLocation, Color.White);
            _spriteBatch.DrawString(font,inv.Value.ToString(), new Vector2(90, inventoryNum * 80 + 24), Color.Black);
            inventoryNum++;
        }

        

        //Minigame updates
        switch(GameManager.CurrentMinigameState){
            case GameManager.MinigameState.Select:
                MinigameManager.Draw(font, _spriteBatch);
                break;
            case GameManager.MinigameState.ChocoLatte:
                ChocoGame.Draw(font, _spriteBatch);
                break;
            case GameManager.MinigameState.Baking:
                BakingGame.Draw(font, _spriteBatch);
                break;
            case GameManager.MinigameState.Dough:
                DoughGame.Draw(font, _spriteBatch);
                break;
            case GameManager.MinigameState.FruitJump:
                FruitJumpGame.Draw(font, _spriteBatch);
                break;
            case GameManager.MinigameState.JellyEater:
                JellyGame.Draw(_spriteBatch);
                break;
            case GameManager.MinigameState.CoffeeSnake:
                CoffeeSnakeGame.Draw(_spriteBatch);
                break;
            case GameManager.MinigameState.FlourGrind:
                FlourGame.Draw(font, _spriteBatch);
                break;
        }
        BakeryManager.Draw(font, _spriteBatch);

        _spriteBatch.End();
        base.Draw(gameTime);
    }
}

public class KBoard
{
    static KeyboardState currentKeyState;
    static KeyboardState previousKeyState;
    
    public static KeyboardState CheckKey(){
        previousKeyState = currentKeyState;
        currentKeyState = Keyboard.GetState();
        return currentKeyState;
    }
    public static bool CheckKeyRelease(Keys key){
        return currentKeyState.IsKeyDown(key) && !previousKeyState.IsKeyDown(key);
    }
}
