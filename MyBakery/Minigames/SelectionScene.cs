using System;
using System.Collections.Generic;
using CoreLibrary;
using CoreLibrary.Graphics;
using CoreLibrary.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyBakery;
using MyBakery.Scenes;

public class SelectionScene : Scene{
    
    private TextureRegion _buttonSprite;
    private DayScene _mainScene;
    private static List<UIButton> _buttons;
    private SpriteFont _font;

    public SelectionScene(DayScene main) : base()
    {
        _mainScene = main;
    }

    public override void Initialize()
    {

        base.Initialize();
    }
    public override void LoadContent()
    {

       _buttonSprite = new TextureRegion(Content.Load<Texture2D>("Button"), 0, 0, 128, 64);
       _font = Content.Load<SpriteFont>("font");

       UIButton chocoGameButton = new UIButton("ChocoLatte", new Vector2(_mainScene.GameBounds.X + 20, _mainScene.GameBounds.Y + 20), _buttonSprite.Width, _buttonSprite.Height, () =>
            {
                _mainScene.ChangeLowerTab(new ChocoGame(_mainScene));
            });
        UIButton bakingGameButton = new UIButton("Baking", new Vector2(_mainScene.GameBounds.X + 150, _mainScene.GameBounds.Y + 20), _buttonSprite.Width, _buttonSprite.Height, () =>
            {
                //if (HasIngredients(_mainScene.ItemDB["Cookie"].Recipe))
                //{
                    _mainScene.ChangeLowerTab(new BakingGame(_mainScene));
               // }
            });
            UIButton doughGameButton = new UIButton("Dough", new Vector2(_mainScene.GameBounds.X + 280, _mainScene.GameBounds.Y + 20), _buttonSprite.Width, _buttonSprite.Height, () =>
            {
                //if (HasIngredients(_mainScene.ItemDB["Dough"].Recipe))
                //{
                    _mainScene.ChangeLowerTab(new DoughGame(_mainScene));
                //}
            });
            UIButton fruitJumpGameButton = new UIButton("Fruit Jump", new Vector2(_mainScene.GameBounds.X + 410, _mainScene.GameBounds.Y + 20), _buttonSprite.Width, _buttonSprite.Height, () =>
            {
                _mainScene.ChangeLowerTab(new FruitJumpGame(_mainScene));
            });
            UIButton jellyGameButton = new UIButton("Jelly Eater", new Vector2(_mainScene.GameBounds.X + 540, _mainScene.GameBounds.Y + 20), _buttonSprite.Width, _buttonSprite.Height, () =>
            {
                _mainScene.ChangeLowerTab(new JellyGame(_mainScene));
            });
            UIButton coffeeGameButton = new UIButton("Coffee Snake", new Vector2(_mainScene.GameBounds.X + 20, _mainScene.GameBounds.Y + 100), _buttonSprite.Width, _buttonSprite.Height, () =>
            {
                _mainScene.ChangeLowerTab(new CoffeeSnakeGame(_mainScene));
            });
            UIButton flourGameButton = new UIButton("Flour Grind", new Vector2(_mainScene.GameBounds.X + 150, _mainScene.GameBounds.Y + 100), _buttonSprite.Width, _buttonSprite.Height, () =>
            {
                //if (HasIngredients(_mainScene.ItemDB["Flour"].Recipe))
                       // {
                            _mainScene.ChangeLowerTab(new FlourGame(_mainScene));
                       // }
            });

            UIButton chocoMineButton = new UIButton("Choco Mine", new Vector2(_mainScene.GameBounds.X + 280, _mainScene.GameBounds.Y + 100), _buttonSprite.Width, _buttonSprite.Height, () =>
            {
                //if (HasIngredients(_mainScene.ItemDB["BoxedChocolate"].Recipe))
               // {
                    _mainScene.ChangeLowerTab(new ChocoMineGame(_mainScene));
                //}
            });

            _buttons = new List<UIButton>(){
                chocoGameButton, bakingGameButton, doughGameButton, fruitJumpGameButton, jellyGameButton, coffeeGameButton, flourGameButton, chocoMineButton
            };
    }

    public override void Draw(GameTime gameTime)
    {
        
        foreach (UIButton c in _buttons)
        {
            c.Draw(Core.SpriteBatch, _font, new Sprite(_buttonSprite));
        }
               
    }
    public override void Update(GameTime gameTime)
    {
        foreach (UIButton c in _buttons)
        {
            c.Update();
        }
    }
    private bool HasIngredients(Dictionary<string, int> recipe) {
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