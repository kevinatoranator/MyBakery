using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GeneralUtil;
using System;
using System.Collections.Generic;

namespace MyBakery;

public class UIButton : Button
{

    Texture2D spriteSheet, chocobg;
    public UIButton(string name, Sprite sprite, Vector2 location, Texture2D spritesheet = null, Texture2D background = null)
    {
        Name = name;
        Sprite = sprite;
        Location = location;
        Hitbox = new Rectangle((int)Location.X, (int)Location.Y, Sprite.TextureMapLocation.Width, Sprite.TextureMapLocation.Height);
        spriteSheet = spritesheet;
        chocobg = background;
    }

    public void Update(GameTime gameTime)
    {

        if (IsClicked())
        {
            switch (Name)
            {
                case "ChocoLatte":
                    MinigameManager.currentGame = new ChocoGame();
                    MinigameManager.currentGame.Start(spriteSheet, chocobg);
                    MinigameManager.CurrentMinigameState = MinigameManager.MinigameState.ChocoLatte;
                    break;
                case "Dough":
                    if (HasIngredients(GameManager.ItemDB[GameManager.Items.Dough].Recipe))
                    {
                        MinigameManager.currentGame = new DoughGame();
                        MinigameManager.currentGame.Start(spriteSheet, chocobg);
                        MinigameManager.CurrentMinigameState = MinigameManager.MinigameState.Dough;
                    }
                    break;
                case "Baking":
                    if (HasIngredients(GameManager.ItemDB[GameManager.Items.Cookie].Recipe))
                    {
                        MinigameManager.currentGame = new BakingGame();
                        MinigameManager.currentGame.Start(spriteSheet, chocobg);
                        MinigameManager.CurrentMinigameState = MinigameManager.MinigameState.Baking;
                    }
                    break;
                case "Fruit Jump":
                    MinigameManager.currentGame = new FruitJumpGame();
                    MinigameManager.currentGame.Start(spriteSheet, chocobg);
                    MinigameManager.CurrentMinigameState = MinigameManager.MinigameState.FruitJump;
                    break;
                case "Jelly Eater":
                    MinigameManager.currentGame = new JellyGame();
                    MinigameManager.currentGame.Start(spriteSheet, chocobg);
                    MinigameManager.CurrentMinigameState = MinigameManager.MinigameState.JellyEater;
                    break;
                case "Flour Grind":
                    if (HasIngredients(GameManager.ItemDB[GameManager.Items.Flour].Recipe))
                    {
                        MinigameManager.currentGame = new FlourGame();
                        MinigameManager.currentGame.Start(spriteSheet, chocobg);
                        MinigameManager.CurrentMinigameState = MinigameManager.MinigameState.FlourGrind;
                    }
                    break;
                case "Coffee Snake":
                    MinigameManager.currentGame = new CoffeeSnakeGame();
                    MinigameManager.currentGame.Start(spriteSheet, chocobg);
                    MinigameManager.CurrentMinigameState = MinigameManager.MinigameState.CoffeeSnake;
                    break;
                case "Start Day":
                    GameManager.CurrentGameState = GameManager.GameState.Inventory;//CHANGE when later developed
                    GameManager.CurrentBakeryState = GameManager.BakeryState.Day;
                    MinigameManager.CurrentMinigameState = MinigameManager.MinigameState.Select;
                    break;
            }

        }
    }

    public bool HasIngredients(Dictionary<GameManager.Items, int> recipe) {
        foreach (KeyValuePair<GameManager.Items, int> ingredient in recipe)
        {
            int quantity;
             GameManager.PlayerInfo.inventory.TryGetValue(ingredient.Key, out quantity);
            if (quantity >= ingredient.Value)
            {
                GameManager.PlayerInfo.inventory[ingredient.Key] -= ingredient.Value;
            }
            else
            {
                Console.WriteLine("Not enough " + ingredient.Key);
                return false;
            }
        }
        return true;
    }
}

