using System;
using System.Collections.Generic;
using GeneralUtil;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CoreLibrary.Graphics;

namespace MyBakery;


public class JellyGame : Minigame{

    const int spriteSize = 64;
    const float startingSize = 0.25f;
    private Sprite jellySprite;
    private List<Jelly> jellies;
    private Jelly player;
    private int gameTimeLeft;
    private double timePassed, tick;

    public override void Start(TextureAtlas spriteSheet, Texture2D background)
    {
        jellySprite = spriteSheet.CreateSprite("Jelly");
        timePassed = tick = 0;
        jellies = new List<Jelly>();

        System.Random random = new System.Random();
        int red = random.Next(255);
        int green = random.Next(255);
        int blue = random.Next(255);
        Color newPlayerColor = new Color(red, green, blue);
        player = new Jelly(new Vector2(gameXOrigin * 2, gameYOrigin + gameYOrigin / 2), startingSize, newPlayerColor, jellySprite);
        
        timePassed = 0;
        player.scale = startingSize;
        player.velocityX = 0;
        player.velocityY = 0;

    }

    public override void Update(GameTime gameTime){
        KBoard.CheckKey();
        if(KBoard.CheckKeyRelease(Keys.Left)){
            player.velocityX -= 1;
        }
        if(KBoard.CheckKeyRelease(Keys.Right)){
            player.velocityX += 1;
        }
        if(KBoard.CheckKeyRelease(Keys.Up)){
            player.velocityY -= 1;
        }
        if(KBoard.CheckKeyRelease(Keys.Down)){
            player.velocityY += 1;
        }
        player.Update(gameTime);

        timePassed += gameTime.ElapsedGameTime.TotalSeconds;
        gameTimeLeft = (int)(60 - timePassed);
        tick += gameTime.ElapsedGameTime.TotalSeconds;

        foreach(Jelly jelly in jellies.ToArray()){
            jelly.Update(gameTime);
            if(jelly.X > GameManager.gameWidth || jelly.X < gameXOrigin - spriteSize * jelly.scale + 1|| jelly.Y > GameManager.gameHeight || jelly.Y < gameYOrigin - spriteSize * jelly.scale + 1)
                jellies.Remove(jelly);
            if(player.hitBox.Intersects(jelly.hitBox)){
                if(player.scale >= jelly.scale){
                    player.scale += jelly.scale*0.25f/player.scale;
                }else{
                    EndGame();
                }
                jellies.Remove(jelly);
            }
        }
            

        if(tick > 1){
            System.Random random = new System.Random();
            int xSpawn = random.Next(gameXOrigin, GameManager.gameWidth);
            int ySpawn = random.Next(gameYOrigin, GameManager.gameHeight);
            float spawnSize = (float)(random.NextDouble()*player.scale*2);
            int red = random.Next(255);
            int green = random.Next(255);
            int blue = random.Next(255);
            Color spawnColor = new Color(red, green, blue);
            int spawnEdge = random.Next(4);
            Vector2 spawnLocation = Vector2.Zero;
            float spawnVelocityX = 1;
            float spawnVelocityY = 1;

            switch(spawnEdge){
                case 0://Top
                    spawnLocation = new Vector2(xSpawn, gameYOrigin - spawnSize*spriteSize + 1);
                    spawnVelocityX = (float)(random.NextDouble()*6)-3;
                    spawnVelocityY = (float)(random.NextDouble()*3);
                    break;
                case 1://Right
                    spawnLocation = new Vector2(GameManager.gameWidth, ySpawn);
                    spawnVelocityX = (float)(random.NextDouble()*-3);
                    spawnVelocityY = (float)(random.NextDouble()*6)-3;
                    break;
                case 2://Bottom
                    spawnLocation = new Vector2(xSpawn, GameManager.gameHeight);
                    spawnVelocityX = (float)(random.NextDouble()*6)-3;
                    spawnVelocityY = (float)(random.NextDouble()*-3);
                    break;
                case 3://Left
                    spawnLocation = new Vector2(gameXOrigin - spawnSize*spriteSize + 1, ySpawn);
                    spawnVelocityX = (float)(random.NextDouble()*3);
                    spawnVelocityY = (float)(random.NextDouble()*6)-3;
                    break;
            }

            jellies.Add(new Jelly(spawnLocation, spawnSize, spawnColor, jellySprite){velocityX = spawnVelocityX, velocityY = spawnVelocityY});
            tick = 0;
        }

    }

    public override void Draw(SpriteFont font, SpriteBatch spriteBatch){
        player.Draw(spriteBatch);
        foreach(Jelly jelly in jellies)
            jelly.Draw(spriteBatch);
    }

    private void EndGame(){
        if (GameManager.PlayerInfo.inventory.ContainsKey("Jelly"))
            {
                GameManager.PlayerInfo.inventory["Jelly"] += (int)Math.Ceiling(player.scale*10);
            }
            else
            {
                GameManager.PlayerInfo.inventory["Jelly"] = (int)Math.Ceiling(player.scale*10);
            }
        
        jellies.Clear();
        MinigameManager.CurrentMinigameState = MinigameManager.MinigameState.Select;
    }


    private class Jelly : PhysicsObject{

        public Jelly(Vector2 location, float size, Color color, Sprite sprite) : base(){
            _x = location.X;
            _y = location.Y;
            this.color = color;
            scale = size;
            rotation = 0;
            this.sprite = sprite;
        }

        public void Update(GameTime gameTime){
            
            if(_x < (int)GameManager.bottomScreenOrigin.X - spriteSize * scale)
                _x = GameManager.gameWidth;
            else if(_x > GameManager.gameWidth + 1)
                _x = (int)GameManager.bottomScreenOrigin.X;
            if(_y < (int)GameManager.bottomScreenOrigin.Y - spriteSize * scale)
                _y = GameManager.gameHeight;
            else if(_y > GameManager.gameHeight + 1)
                _y = (int)GameManager.bottomScreenOrigin.Y;
            UpdateLocation();
            
        }
    }
}

