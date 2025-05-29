
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

    public static void Initialize(GraphicsDevice graphicsDevice, Texture2D button)
    {
        _gameXOrigin = (int)GameManager.bottomScreenOrigin.X;
        _gameYOrigin = (int)GameManager.bottomScreenOrigin.Y;

        whiteBox = new Texture2D(graphicsDevice, 1, 1);
        whiteBox.SetData(new[] {Color.White});
        UIButton chocoGameButton = new UIButton("ChocoLatte", new GeneralUtil.Sprite(button, new Rectangle(0, 0, 128, 64)), new Vector2(_gameXOrigin + 20, _gameYOrigin + 20));
        UIButton bakingGameButton = new UIButton("Baking", new GeneralUtil.Sprite(button, new Rectangle(0, 0, 128, 64)), new Vector2(_gameXOrigin + 150, _gameYOrigin + 20));
        UIButton doughGameButton = new UIButton("Dough", new GeneralUtil.Sprite(button, new Rectangle(0, 0, 128, 64)), new Vector2(_gameXOrigin + 280, _gameYOrigin + 20));
        UIButton fruitJumpGameButton = new UIButton("Fruit Jump", new GeneralUtil.Sprite(button, new Rectangle(0, 0, 128, 64)), new Vector2(_gameXOrigin + 410, _gameYOrigin + 20));
        UIButton jellyGameButton = new UIButton("Jelly Eater", new GeneralUtil.Sprite(button, new Rectangle(0, 0, 128, 64)), new Vector2(_gameXOrigin + 540, _gameYOrigin + 20));
        UIButton coffeeGameButton = new UIButton("Coffee Snake", new GeneralUtil.Sprite(button, new Rectangle(0, 0, 128, 64)), new Vector2(_gameXOrigin + 20, _gameYOrigin + 100));

        _buttons = new List<UIButton>(){
            chocoGameButton, bakingGameButton, doughGameButton, fruitJumpGameButton, jellyGameButton, coffeeGameButton
        };
    }

    public static void Draw(SpriteFont font, SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(whiteBox, new Rectangle(_gameXOrigin, _gameYOrigin, GameManager.gameWidth/2*3, GameManager.gameHeight/2), Color.CornflowerBlue);
        foreach(UIButton c in _buttons){
            c.Draw(spriteBatch, font);
        }
    }

    public static void Update(GameTime gameTime)
    {
        foreach(UIButton c in _buttons){
            c.Update(gameTime);
        }
    }
}