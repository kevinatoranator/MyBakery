using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GeneralUtil;
using System;
using System.Collections.Generic;

namespace MyBakery;

public class UIButton : Button
{


    public UIButton(string name, Sprite sprite, Vector2 location)
    {
        Name = name;
        Sprite = sprite;
        Location = location;
        HitBox = new Rectangle((int)Location.X, (int)Location.Y, Sprite.TextureMapLocation.Width, Sprite.TextureMapLocation.Height);
    }

    public void Update(GameTime gameTime)
    {

        if (IsClicked())
        {
            switch (Name)
            {
                case "ChocoLatte":
                    GameManager.CurrentMinigameState = GameManager.MinigameState.ChocoLatte;
                    break;
                case "Dough":
                    if (HasIngredients(GameManager.ItemDB[GameManager.Items.Dough].Recipe))
                    {
                        GameManager.CurrentMinigameState = GameManager.MinigameState.Dough;
                    }
                    break;
                case "Baking":
                    if (HasIngredients(GameManager.ItemDB[GameManager.Items.Cookie].Recipe))
                    {
                        GameManager.CurrentMinigameState = GameManager.MinigameState.Baking;
                    }
                    break;
                case "Fruit Jump":
                    GameManager.CurrentMinigameState = GameManager.MinigameState.FruitJump;
                    break;
                case "Jelly Eater":
                    GameManager.CurrentMinigameState = GameManager.MinigameState.JellyEater;
                    break;
                case "Flour Grind":
                    FlourGame.Restart();
                    if (HasIngredients(GameManager.ItemDB[GameManager.Items.Flour].Recipe))
                    {
                        GameManager.CurrentMinigameState = GameManager.MinigameState.FlourGrind;
                    }
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

