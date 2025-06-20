
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MyBakery;

public static class MinigameManager
{

    private static List<UIButton> _buttons;
    private static int _gameXOrigin, _gameYOrigin;
    public static Texture2D SpriteSheet, whiteBox;

    public enum MinigameState
    {
        Menu,
        Select,
        ChocoLatte,
        Baking,
        Dough,
        FruitJump,
        JellyEater,
        CoffeeSnake,
        FlourGrind
    }

    public static MinigameState CurrentMinigameState;
    public static Minigame currentGame;

    public static void Initialize(GraphicsDevice graphicsDevice, Texture2D button, Texture2D spriteSheet, Texture2D chocobg)
    {
        _gameXOrigin = (int)GameManager.bottomScreenOrigin.X;
        _gameYOrigin = (int)GameManager.bottomScreenOrigin.Y;

        whiteBox = new Texture2D(graphicsDevice, 1, 1);
        whiteBox.SetData(new[] { Color.White });
        UIButton chocoGameButton = new UIButton("ChocoLatte", new GeneralUtil.Sprite(button, new Rectangle(0, 0, 128, 64)), new Vector2(_gameXOrigin + 20, _gameYOrigin + 20), () =>
        {
            currentGame = new ChocoGame();
            currentGame.Start(spriteSheet, chocobg);
            CurrentMinigameState = MinigameState.ChocoLatte;
        });
        UIButton bakingGameButton = new UIButton("Baking", new GeneralUtil.Sprite(button, new Rectangle(0, 0, 128, 64)), new Vector2(_gameXOrigin + 150, _gameYOrigin + 20), () =>
        {
            if (HasIngredients(GameManager.ItemDB[GameManager.Items.Cookie].Recipe))
            {
                currentGame = new BakingGame();
                currentGame.Start(spriteSheet, chocobg);
                CurrentMinigameState = MinigameState.Baking;
            }
        });
        UIButton doughGameButton = new UIButton("Dough", new GeneralUtil.Sprite(button, new Rectangle(0, 0, 128, 64)), new Vector2(_gameXOrigin + 280, _gameYOrigin + 20), () =>
        {
            if (HasIngredients(GameManager.ItemDB[GameManager.Items.Dough].Recipe))
            {
                currentGame = new DoughGame();
                currentGame.Start(spriteSheet, chocobg);
                CurrentMinigameState = MinigameState.Dough;
            }
        });
        UIButton fruitJumpGameButton = new UIButton("Fruit Jump", new GeneralUtil.Sprite(button, new Rectangle(0, 0, 128, 64)), new Vector2(_gameXOrigin + 410, _gameYOrigin + 20), () =>
        {
            currentGame = new FruitJumpGame();
            currentGame.Start(spriteSheet, chocobg);
            CurrentMinigameState = MinigameState.FruitJump;
        });
        UIButton jellyGameButton = new UIButton("Jelly Eater", new GeneralUtil.Sprite(button, new Rectangle(0, 0, 128, 64)), new Vector2(_gameXOrigin + 540, _gameYOrigin + 20), () =>
        {
            currentGame = new JellyGame();
            currentGame.Start(spriteSheet, chocobg);
            CurrentMinigameState = MinigameState.JellyEater;
        });
        UIButton coffeeGameButton = new UIButton("Coffee Snake", new GeneralUtil.Sprite(button, new Rectangle(0, 0, 128, 64)), new Vector2(_gameXOrigin + 20, _gameYOrigin + 100), () =>
        {
            currentGame = new CoffeeSnakeGame();
            currentGame.Start(spriteSheet, chocobg);
            CurrentMinigameState = MinigameState.CoffeeSnake;
        });
        UIButton flourGameButton = new UIButton("Flour Grind", new GeneralUtil.Sprite(button, new Rectangle(0, 0, 128, 64)), new Vector2(_gameXOrigin + 150, _gameYOrigin + 100), () =>
        {
            if (HasIngredients(GameManager.ItemDB[GameManager.Items.Flour].Recipe))
                    {
                        currentGame = new FlourGame();
                        currentGame.Start(spriteSheet, chocobg);
                        CurrentMinigameState = MinigameState.FlourGrind;
                    }
        });

        _buttons = new List<UIButton>(){
            chocoGameButton, bakingGameButton, doughGameButton, fruitJumpGameButton, jellyGameButton, coffeeGameButton, flourGameButton
        };    
    }

    public static void Draw(SpriteFont font, SpriteBatch spriteBatch)
    {

        switch (CurrentMinigameState)
        {
            case MinigameState.Select:
                spriteBatch.Draw(whiteBox, new Rectangle(_gameXOrigin, _gameYOrigin, GameManager.gameWidth / 2 * 3, GameManager.gameHeight / 2), Color.CornflowerBlue);
                foreach (UIButton c in _buttons)
                {
                    c.Draw(spriteBatch, font);
                }
                break;
            case MinigameState.Menu:
                break;
            default:
                currentGame.Draw(font, spriteBatch);
                break;
        }
    }

    public static void Update(GameTime gameTime)
    {

        switch (CurrentMinigameState)
        {
            case MinigameState.Select:
                foreach (UIButton c in _buttons)
                {
                    c.Update();
                }
                break;
            case MinigameState.Menu:
                break;
            default:
                currentGame.Update(gameTime);
                break;
        }
    }

    public static Boolean isInside(Point p1, Vector2 vec2, int xsize, int ysize)
    {
        Rectangle obj1 = new Rectangle((int)vec2.X, (int)vec2.Y, xsize, ysize);
        return obj1.Contains(p1);
    }
    
    private static bool HasIngredients(Dictionary<GameManager.Items, int> recipe) {
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