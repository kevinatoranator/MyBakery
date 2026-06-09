using System;
using System.Collections.Generic;
using System.Linq;
using GeneralUtil;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CoreLibrary.Graphics;

namespace MyBakery;

public class ChocoGame : Minigame
{
    const int spriteSize = 64;

    //Chocogame
    private Sprite chocoSprite, playerSprite, emberSprite, progressFront, progressBack;
    private Texture2D bg;
    private List<FallingObject> fallingObjects;
    private Player player;
    private int collectedChocolate, gameTimeLeft, quota;
    private double timePassed;

    private ProgressBar timerBar, quotaBar;

    public override void Start(TextureAtlas spriteSheet, Texture2D background)
    {
        playerSprite = spriteSheet.CreateSprite("ToastDog");
        chocoSprite = spriteSheet.CreateSprite("ChocoChip");
        emberSprite = spriteSheet.CreateSprite("Ember");
        progressFront = spriteSheet.CreateSprite("ProgressFront");
        progressBack = spriteSheet.CreateSprite("ProgressBack");
        bg = background;

        fallingObjects = new List<FallingObject>();

        player = new Player() { location = new Vector2(gameXOrigin * 2, GameManager.gameHeight - 100) };
        collectedChocolate = 0;
        timePassed = 0;
        quota = 20; //Make dynamic based on... average?

        timerBar = new ProgressBar(progressFront, progressBack, 60, 60, new Vector2(gameXOrigin + 10, gameYOrigin + 30), false);
        quotaBar = new ProgressBar(progressFront, progressBack, quota, 0, new Vector2(gameXOrigin + 10, gameYOrigin + 130), false);

    }

    public override void Draw(SpriteFont font, SpriteBatch spriteBatch)
    {
                //Chocogame

        spriteBatch.Draw(bg, new Rectangle(gameXOrigin, gameYOrigin, GameManager.gameWidth*2/3, GameManager.gameHeight/2), Color.White);
        timerBar.Draw(spriteBatch);
        spriteBatch.DrawString(font, "Chocolate Quota: ", new Vector2(gameXOrigin + 10, gameYOrigin + 100), Color.Black);
        quotaBar.Draw(spriteBatch);

        foreach(FallingObject o in fallingObjects){
            if(o.type == "chocolate")
                chocoSprite.Draw(spriteBatch, o.location);
            else if(o.type == "ember")
                emberSprite.Draw(spriteBatch, o.location);
        }

        playerSprite.Draw(spriteBatch, player.location);

    }

    public override void Update(GameTime gameTime)
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
            if (GameManager.PlayerInfo.inventory.ContainsKey("ChocoChip"))
            {
                GameManager.PlayerInfo.inventory["ChocoChip"] += collectedChocolate;
            }
            else
            {
                GameManager.PlayerInfo.inventory["ChocoChip"] = collectedChocolate;
            }
            
            MinigameManager.CurrentMinigameState = MinigameManager.MinigameState.Select;
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

    public Boolean Collide(Vector2 vec1, Vector2 vec2, int xsize1, int ysize1, int xsize2, int ysize2){
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
}