

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
   
    public enum Items{
        None, Coin, ChocoChip, Dough, Cookie, Orange, Cherry, Apple, Jelly, CoffeeBean, Flour, Wheat, BoxedChocolate
    }

    public static Dictionary<Items, Product> ItemDB = new Dictionary<Items, Product>();
    public static Dictionary<Items, Sprite> TextureDB = new Dictionary<Items, Sprite>();
    public static Dictionary<int, Sprite> ButtonDB = new Dictionary<int, Sprite>();


    public static GameState CurrentGameState;
    public static BakeryState CurrentBakeryState;
    
    public static PlayerProfile PlayerInfo = new PlayerProfile("");

    public static void Initialize(Texture2D SpriteSheet)
    {
        TextureDB[Items.Coin] = new Sprite(SpriteSheet, new Rectangle(128, 128, 64, 64));
        TextureDB[Items.ChocoChip] = new Sprite(SpriteSheet, new Rectangle(0, 64, 64, 64));
        TextureDB[Items.Dough] = new Sprite(SpriteSheet, new Rectangle(0, 128, 64, 64));
        TextureDB[Items.Cookie] = new Sprite(SpriteSheet, new Rectangle(64, 128, 64, 64));
        TextureDB[Items.Apple] = new Sprite(SpriteSheet, new Rectangle(0, 0, 64, 64));
        TextureDB[Items.Cherry] = new Sprite(SpriteSheet, new Rectangle(64, 0, 64, 64));
        TextureDB[Items.Orange] = new Sprite(SpriteSheet, new Rectangle(128, 0, 64, 64));
        TextureDB[Items.Jelly] = new Sprite(SpriteSheet, new Rectangle(128, 192, 64, 64));
        TextureDB[Items.CoffeeBean] = new Sprite(SpriteSheet, new Rectangle(256, 128, 64, 64));
        TextureDB[Items.Flour] = new Sprite(SpriteSheet, new Rectangle(320, 128, 64, 64));
        TextureDB[Items.Wheat] = new Sprite(SpriteSheet, new Rectangle(192, 64, 64, 64));
        TextureDB[Items.BoxedChocolate] = new Sprite(SpriteSheet, new Rectangle(128, 256, 64, 64));
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