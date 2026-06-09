
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using CoreLibrary;
using CoreLibrary.Graphics;

namespace MyBakery;

public static class MinigameManager
{

    private static List<UIButton> _buttons;
    private static int _gameXOrigin, _gameYOrigin;
    public static TextureAtlas SpriteSheet;

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
        FlourGrind,
        ChocoMine
    }

    public static MinigameState CurrentMinigameState;
    public static Minigame currentGame;
    private static TextureRegion _buttonSprite;

    public static void Initialize(Texture2D button, TextureAtlas spriteSheet, Texture2D chocobg)
    {
        _gameXOrigin = (int)GameManager.bottomScreenOrigin.X;
        _gameYOrigin = (int)GameManager.bottomScreenOrigin.Y;
        _buttonSprite = new TextureRegion(button, 0, 0, 128, 64);

        UIButton chocoGameButton = new UIButton("ChocoLatte", new Vector2(_gameXOrigin + 20, _gameYOrigin + 20), _buttonSprite.Width, _buttonSprite.Height, () =>
        {
            currentGame = new ChocoGame();
            currentGame.Start(spriteSheet, chocobg);
            CurrentMinigameState = MinigameState.ChocoLatte;
        });
        UIButton bakingGameButton = new UIButton("Baking", new Vector2(_gameXOrigin + 150, _gameYOrigin + 20), _buttonSprite.Width, _buttonSprite.Height, () =>
        {
            if (HasIngredients(GameManager.ItemDB["Cookie"].Recipe))
            {
                currentGame = new BakingGame();
                currentGame.Start(spriteSheet, chocobg);
                CurrentMinigameState = MinigameState.Baking;
            }
        });
        UIButton doughGameButton = new UIButton("Dough", new Vector2(_gameXOrigin + 280, _gameYOrigin + 20), _buttonSprite.Width, _buttonSprite.Height, () =>
        {
            if (HasIngredients(GameManager.ItemDB["Dough"].Recipe))
            {
                currentGame = new DoughGame();
                currentGame.Start(spriteSheet, chocobg);
                CurrentMinigameState = MinigameState.Dough;
            }
        });
        UIButton fruitJumpGameButton = new UIButton("Fruit Jump", new Vector2(_gameXOrigin + 410, _gameYOrigin + 20), _buttonSprite.Width, _buttonSprite.Height, () =>
        {
            currentGame = new FruitJumpGame();
            currentGame.Start(spriteSheet, chocobg);
            CurrentMinigameState = MinigameState.FruitJump;
        });
        UIButton jellyGameButton = new UIButton("Jelly Eater", new Vector2(_gameXOrigin + 540, _gameYOrigin + 20), _buttonSprite.Width, _buttonSprite.Height, () =>
        {
            currentGame = new JellyGame();
            currentGame.Start(spriteSheet, chocobg);
            CurrentMinigameState = MinigameState.JellyEater;
        });
        UIButton coffeeGameButton = new UIButton("Coffee Snake", new Vector2(_gameXOrigin + 20, _gameYOrigin + 100), _buttonSprite.Width, _buttonSprite.Height, () =>
        {
            currentGame = new CoffeeSnakeGame();
            currentGame.Start(spriteSheet, chocobg);
            CurrentMinigameState = MinigameState.CoffeeSnake;
        });
        UIButton flourGameButton = new UIButton("Flour Grind", new Vector2(_gameXOrigin + 150, _gameYOrigin + 100), _buttonSprite.Width, _buttonSprite.Height, () =>
        {
            if (HasIngredients(GameManager.ItemDB["Flour"].Recipe))
                    {
                        currentGame = new FlourGame();
                        currentGame.Start(spriteSheet, chocobg);
                        CurrentMinigameState = MinigameState.FlourGrind;
                    }
        });

        UIButton chocoMineButton = new UIButton("Choco Mine", new Vector2(_gameXOrigin + 280, _gameYOrigin + 100), _buttonSprite.Width, _buttonSprite.Height, () =>
        {
            if (HasIngredients(GameManager.ItemDB["BoxedChocolate"].Recipe))
            {
                currentGame = new ChocoMineGame();
                currentGame.Start(spriteSheet, chocobg);
                CurrentMinigameState = MinigameState.ChocoMine;
            }
        });

        _buttons = new List<UIButton>(){
            chocoGameButton, bakingGameButton, doughGameButton, fruitJumpGameButton, jellyGameButton, coffeeGameButton, flourGameButton, chocoMineButton
        };    
    }

    public static void Draw(SpriteFont font, SpriteBatch spriteBatch)
    {

        switch (CurrentMinigameState)
        {
            case MinigameState.Select:
                //spriteBatch.Draw(whiteBox, new Rectangle(_gameXOrigin, _gameYOrigin, GameManager.gameWidth / 2 * 3, GameManager.gameHeight / 2), Color.CornflowerBlue);
                foreach (UIButton c in _buttons)
                {
                    c.Draw(spriteBatch, font, new Sprite(_buttonSprite));
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
    
    private static bool HasIngredients(Dictionary<string, int> recipe) {
        foreach (KeyValuePair<string, int> ingredient in recipe)
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