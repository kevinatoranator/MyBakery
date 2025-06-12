using System;
using System.Collections.Generic;
using System.Linq;
using GeneralUtil;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MyBakery;

public static class ChocoGame
{

    private static int gameXOrigin = (int)GameManager.bottomScreenOrigin.X;
    private static int gameYOrigin = (int)GameManager.bottomScreenOrigin.Y;
    const int spriteSize = 64;

    //Chocogame
    private static Sprite chocoSprite, playerSprite, emberSprite;
    private static Texture2D whiteBox, bg;
    private static List<FallingObject> fallingObjects;
    private static Player player;
    private static int collectedChocolate, gameTimeLeft, quota;
    private static double timePassed;

    private static ProgressBar timerBar, quotaBar;
    public static void Initialize(GraphicsDevice graphicsDevice, Texture2D spriteSheet, Texture2D progressFront, Texture2D progressBack, Texture2D background)
    {

        //Chocogame
        playerSprite = new Sprite(spriteSheet, new Rectangle(128, 64, 64, 64));
        chocoSprite = new Sprite(spriteSheet, new Rectangle(0, 64, 64, 64));
        emberSprite = new Sprite(spriteSheet, new Rectangle(64, 64, 64, 64));
        whiteBox = new Texture2D(graphicsDevice, 1, 1);
        whiteBox.SetData(new[] {Color.White});
        bg = background;

        fallingObjects = new List<FallingObject>();
        player = new Player(){location = new Vector2(gameXOrigin*2, GameManager.gameHeight-100)};
        collectedChocolate = 0;
        timePassed = 0;
        quota = 20; //Make dynamic based on... average?

        timerBar = new ProgressBar(progressFront, progressBack, 60, 60, new Vector2(gameXOrigin + 10, gameYOrigin + 30), false);
        quotaBar = new ProgressBar(progressFront, progressBack, quota, 0, new Vector2(gameXOrigin + 10, gameYOrigin + 130), false);
    }


    public static void Draw(SpriteFont font, SpriteBatch spriteBatch)
    {
                //Chocogame

        spriteBatch.Draw(bg, new Rectangle(gameXOrigin, gameYOrigin, GameManager.gameWidth*2/3, GameManager.gameHeight/2), Color.White);
        /*if(collectedChocolate < quota)
            spriteBatch.DrawString(font, "Chocolate collected: " + collectedChocolate +"/"+quota, new Vector2(gameXOrigin + 10, gameYOrigin + 100), Color.Red);
        else
            spriteBatch.DrawString(font, "Chocolate collected: " + collectedChocolate +"/"+quota, new Vector2(gameXOrigin + 10, gameYOrigin + 100), Color.Green);*/
        //spriteBatch.DrawString(font, "Time Left: " + gameTimeLeft, new Vector2(gameXOrigin + 10, gameYOrigin + 30), Color.Black);
        timerBar.Draw(spriteBatch);
        spriteBatch.DrawString(font, "Chocolate Quota: ", new Vector2(gameXOrigin + 10, gameYOrigin + 100), Color.Black);
        quotaBar.Draw(spriteBatch);

        foreach(FallingObject o in fallingObjects){
            if(o.type == "chocolate")
                spriteBatch.Draw(chocoSprite.Texture, o.location, chocoSprite.TextureMapLocation, Color.White);
            else if(o.type == "ember")
                spriteBatch.Draw(emberSprite.Texture, o.location, emberSprite.TextureMapLocation, Color.White);
        }

        spriteBatch.Draw(playerSprite.Texture, player.location, playerSprite.TextureMapLocation, Color.White);

    }

    public static void Update(GameTime gameTime)
    {
        //Chocogame

        if(Keyboard.GetState().IsKeyDown(Keys.Left) && player.location.X > gameXOrigin)
            player.location = new Vector2(player.location.X - 7, player.location.Y);
        if(Keyboard.GetState().IsKeyDown(Keys.Right) && player.location.X < GameManager.gameWidth-spriteSize)
            player.location = new Vector2(player.location.X + 7, player.location.Y);

        timePassed += gameTime.ElapsedGameTime.TotalSeconds;
        gameTimeLeft = (int)(60 - timePassed);
        timerBar.Update(gameTimeLeft);
        quotaBar.Update(collectedChocolate);
        if(gameTimeLeft < 0){
            fallingObjects.Clear();
            gameTimeLeft = 0;
            if(collectedChocolate > quota)
                collectedChocolate += collectedChocolate/2;
            if (GameManager.PlayerInfo.inventory.ContainsKey(GameManager.Items.ChocoChip))
            {
                GameManager.PlayerInfo.inventory[GameManager.Items.ChocoChip] += collectedChocolate;
            }
            else
            {
                GameManager.PlayerInfo.inventory[GameManager.Items.ChocoChip] = collectedChocolate;
            }
            
            GameManager.CurrentMinigameState = GameManager.MinigameState.Select;
        }

        if(gameTimeLeft > 0){
            Random rand = new();
            int spawnChance = rand.Next(100)+1;
            int xlocation = rand.Next(GameManager.gameWidth-gameXOrigin-spriteSize) + gameXOrigin;
            int fallSpeed = rand.Next(5)+2;
            if(spawnChance > 98){
                fallingObjects.Add(new FallingObject(){location = new Vector2(xlocation, gameYOrigin), fallSpeed = fallSpeed, type="chocolate"});
            }else if(spawnChance == 1){
                fallingObjects.Add(new FallingObject(){location = new Vector2(xlocation, gameYOrigin), fallSpeed = fallSpeed, type="ember"});
            }
        }

        List<FallingObject> qclist = fallingObjects.ToList();
        foreach(FallingObject o in qclist){
            if(o.location.Y >= player.location.Y + 5)
                o.fallSpeed = 0;
                //fallingObjects.Remove(o);
            else if(Collide(new Vector2(o.location.X, o.location.Y+spriteSize*3/4), player.location, spriteSize, spriteSize/4, spriteSize, 5)){
                fallingObjects.Remove(o);
                if(o.type == "chocolate")
                    collectedChocolate++;
                else if(o.type == "ember")
                    collectedChocolate = 0;
            }else
                o.location = new Vector2(o.location.X, o.location.Y+o.fallSpeed);
        }
        if(fallingObjects.Count>15)
            fallingObjects.Remove(fallingObjects.First());
    }

    public static Boolean Collide(Vector2 vec1, Vector2 vec2, int xsize1, int ysize1, int xsize2, int ysize2){
        Rectangle obj1 = new Rectangle((int)vec1.X, (int)vec1.Y, xsize1, ysize1);
        Rectangle obj2 = new Rectangle((int)vec2.X, (int)vec2.Y, xsize2, ysize2);

        return obj1.Intersects(obj2);
    }

    //Chocogame
    private class Player{
        Vector2 _location;

        public Vector2 location{
            get => _location;
            set => _location = value;
        }
    }
    private class FallingObject{
        Vector2 _location;
        int _fallSpeed;
        String _type;

        public Vector2 location{
            get => _location;
            set => _location = value;
        }
        public int fallSpeed{
            get => _fallSpeed;
            set => _fallSpeed = value;
        }
        public String type{
            get => _type;
            set => _type = value;
        }
    }
}
