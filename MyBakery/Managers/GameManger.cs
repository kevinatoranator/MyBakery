

using System;
using System.Collections.Generic;
using GeneralUtil;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CoreLibrary.Graphics;

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
   
    public static List<string> Items = new List<string>(){
        "None", "Coin", "ChocoChip", "Dough", "Cookie", "Orange", "Cherry", "Apple", "Jelly", "CoffeeBean", "Flour", "Wheat", "BoxedChocolate"
    };

    public static Dictionary<string, Product> ItemDB = new Dictionary<string, Product>();


    public static GameState CurrentGameState;
    public static BakeryState CurrentBakeryState;
    
    public static PlayerProfile PlayerInfo = new PlayerProfile("");

    public static void Initialize(TextureAtlas SpriteSheet)
    {
        
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