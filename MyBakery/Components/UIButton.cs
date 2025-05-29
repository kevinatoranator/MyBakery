using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GeneralUtil;
using System;

namespace MyBakery;

public class UIButton : Button{


    public UIButton(string name, Sprite sprite, Vector2 location){
        Name = name;
        Sprite = sprite;
        Location = location;
        HitBox = new Rectangle((int)Location.X, (int)Location.Y, Sprite.TextureMapLocation.Width, Sprite.TextureMapLocation.Height);
    }

    public void Update(GameTime gameTime) { 

        if(IsClicked()){
            switch(Name){
                case "ChocoLatte":
                    GameManager.CurrentMinigameState = GameManager.MinigameState.ChocoLatte;
                    break;
                case "Dough":
                    if(GameManager.inventory[1].Quantity >= 10){
                        GameManager.CurrentMinigameState = GameManager.MinigameState.Dough;
                        GameManager.inventory[1].Quantity -= 10;
                    }else{
                        Console.WriteLine("Not enough chips");
                    }
                    break;
                case "Baking":
                    if(GameManager.inventory[2].Quantity >= 10){
                        GameManager.CurrentMinigameState = GameManager.MinigameState.Baking;
                        GameManager.inventory[2].Quantity -= 10;
                    }else{
                        Console.WriteLine("Not enough dough");
                    }
                    break;
                case "Fruit Jump":
                    GameManager.CurrentMinigameState = GameManager.MinigameState.FruitJump;
                    break;
                case "Jelly Eater":
                    GameManager.CurrentMinigameState = GameManager.MinigameState.JellyEater;
                    break;
                case "Coffee Snake":
                    CoffeeSnakeGame.Restart();
                    GameManager.CurrentMinigameState = GameManager.MinigameState.CoffeeSnake;
                    break;
                case "Start Day":
                    GameManager.CurrentGameState = GameManager.GameState.Inventory;//CHANGE when later developed
                    GameManager.CurrentBakeryState = GameManager.BakeryState.Day;
                    GameManager.CurrentMinigameState = GameManager.MinigameState.Select;
                    break;
            }
            
        }
    }
}