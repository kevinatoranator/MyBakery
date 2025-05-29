

using System;
using System.Collections.Generic;
using GeneralUtil;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MyBakery;


public static class GameManager{

    public static int gameWidth = 1920;
    public static int gameHeight = 1080;

    public static Vector2 topScreenOrigin = new Vector2(gameWidth/3, 0);
    public static Vector2 bottomScreenOrigin = new Vector2(gameWidth/3, gameHeight/2);
    public static ButtonState LastMouseState = ButtonState.Released;
    public static Boolean MouseClicked = false;
    public static List<Product> inventory = new List<Product>();
    public static Sprite cancelSprite;

    //GAME STATES

    public enum GameState{
        MainMenu,
        Start,
        Inventory,
    }

    public enum BakeryState{
        Menu,
        Day,
        Closing,
        WorldMap
    }
    public enum MinigameState{
        Menu,
        Select,
        ChocoLatte,
        Baking,
        Dough,
        FruitJump,
        JellyEater,
        CoffeeSnake
    }
    public enum Items{
        Coin, ChocoChip, Dough, Cookie, Orange, Cherry, Apple, Jelly, CoffeeBean
    }

    public static GameState CurrentGameState;
    public static BakeryState CurrentBakeryState;
    public static MinigameState CurrentMinigameState;
    
    public static void Initialize(Texture2D SpriteSheet){
        inventory.Add(new Product(Items.Coin, 1, 0, false, new Sprite(SpriteSheet, new Rectangle(128, 128, 64, 64))));
        inventory.Add(new Product(Items.ChocoChip, 1, 0, false, new Sprite(SpriteSheet, new Rectangle(0, 64, 64, 64))));
        inventory.Add(new Product(Items.Dough, 1, 0, false, new Sprite(SpriteSheet, new Rectangle(0, 128, 64, 64))));
        inventory.Add(new Product(Items.Cookie, 1, 0, true, new Sprite(SpriteSheet, new Rectangle(64, 128, 64, 64))));
        inventory.Add(new Product(Items.Apple, 1, 0, false, new Sprite(SpriteSheet, new Rectangle(0, 0, 64, 64))));
        inventory.Add(new Product(Items.Cherry, 1, 0, false, new Sprite(SpriteSheet, new Rectangle(64, 0, 64, 64))));
        inventory.Add(new Product(Items.Orange, 1, 0, false, new Sprite(SpriteSheet, new Rectangle(128, 0, 64, 64))));
        inventory.Add(new Product(Items.Jelly, 1, 0, true, new Sprite(SpriteSheet, new Rectangle(128, 192, 64, 64))));
        inventory.Add(new Product(Items.CoffeeBean, 1, 0, false, new Sprite(SpriteSheet, new Rectangle(256, 128, 64, 64))));
        cancelSprite = new Sprite(SpriteSheet, new Rectangle(0, 192, 64, 64));
    }

    public static void Update(GameTime gameTime) {
        MouseClicked = IsClicked();
        LastMouseState = Mouse.GetState().LeftButton;
    }

    public static Boolean IsClicked(){
        if(Mouse.GetState().LeftButton == ButtonState.Pressed && LastMouseState != ButtonState.Pressed){
            return true;
        }
        return false;
    }

}