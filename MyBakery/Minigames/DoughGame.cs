using System;
using System.Collections.Generic;
using GeneralUtil;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MyBakery;

public static class DoughGame
{

    private static int gameXOrigin = (int)GameManager.bottomScreenOrigin.X;
    private static int gameYOrigin = (int)GameManager.bottomScreenOrigin.Y;
    const int spriteSize = 64;

    //DoughGame
    private static Sprite whiskSprite;
    private static Texture2D whiteBox;
    private static int collectedDough, gameTimeLeft, quota;
    private static double timePassed;
    private static Vector2 whiskPos;
    private static Keys lastUpDown, lastLeftRight;
    public static void Initialize(GraphicsDevice graphicsDevice, Texture2D spriteSheet, Texture2D whisk)
    {

        //DoughGame
        whiskSprite = new Sprite(whisk, new Rectangle(0, 0, 256, 256));
        whiskPos = new Vector2(gameXOrigin + 300, gameYOrigin + 100);
        whiteBox = new Texture2D(graphicsDevice, 1, 1);
        whiteBox.SetData(new[] {Color.White});

        collectedDough = 0;
        timePassed = 0;
        quota = 20; //Make dynamic based on... average?
    }


    public static void Draw(SpriteFont font, SpriteBatch spriteBatch)
    {
                //Doughgame
        spriteBatch.Draw(whiteBox, new Rectangle(gameXOrigin, gameYOrigin, GameManager.gameWidth*2/3, GameManager.gameHeight/2), Color.Beige);
        if(collectedDough < quota)
            spriteBatch.DrawString(font, "Dough collected: " + collectedDough +"/"+quota, new Vector2(gameXOrigin + 10, gameYOrigin + 10), Color.Red);
        else
            spriteBatch.DrawString(font, "Dough collected: " + collectedDough +"/"+quota, new Vector2(gameXOrigin + 10, gameYOrigin + 10), Color.Green);
        spriteBatch.DrawString(font, "Time Left: " + gameTimeLeft, new Vector2(gameXOrigin + 10, gameYOrigin + 30), Color.Black);


        spriteBatch.Draw(whiskSprite.Texture, whiskPos, whiskSprite.TextureMapLocation, Color.White);


    }

    public static void Update(GameTime gameTime)
    {

        //Doughgame
        KBoard.CheckKey();
        if(KBoard.CheckKeyRelease(Keys.Left) && lastLeftRight != Keys.Left){
            whiskPos = new Vector2(whiskPos.X - 40, whiskPos.Y);
            lastLeftRight = Keys.Left;
            Shake();
        }if(KBoard.CheckKeyRelease(Keys.Right) && lastLeftRight != Keys.Right){
            whiskPos = new Vector2(whiskPos.X + 40, whiskPos.Y);
            lastLeftRight = Keys.Right;
            Shake();
        }if(KBoard.CheckKeyRelease(Keys.Up) && lastUpDown != Keys.Up){
            whiskPos = new Vector2(whiskPos.X, whiskPos.Y - 40);
            lastUpDown = Keys.Up;
            Shake();
        }if(KBoard.CheckKeyRelease(Keys.Down) && lastUpDown != Keys.Down){
            whiskPos = new Vector2(whiskPos.X, whiskPos.Y + 40);
            lastUpDown = Keys.Down;
            Shake();
        }
        timePassed += gameTime.ElapsedGameTime.TotalSeconds;
        gameTimeLeft = (int)(30 - timePassed);
        if(gameTimeLeft < 0){
            gameTimeLeft = 0;
            if(collectedDough > quota)
                collectedDough += collectedDough/2;
            
            if (GameManager.PlayerInfo.inventory.ContainsKey(GameManager.Items.Dough))
            {
                GameManager.PlayerInfo.inventory[GameManager.Items.Dough] += collectedDough; 
            }
            else
            {
                GameManager.PlayerInfo.inventory[GameManager.Items.Dough] = collectedDough; // change to dough
            }
            GameManager.CurrentMinigameState = GameManager.MinigameState.Select;
        }
    }

    private static void Shake(){
            Random rand = new();
            int spawnChance = rand.Next(100)+1;
            if(spawnChance > 50){
               collectedDough++;
            }
    }
}
