using System;
using System.Collections.Generic;
using GeneralUtil;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CoreLibrary;
using CoreLibrary.Graphics;
using CoreLibrary.Input;
using CoreLibrary.Scenes;
using MyBakery.Scenes;

namespace MyBakery;


public class JellyGame : Scene{

    const int spriteSize = 64;
    const float startingSize = 0.25f;
    private List<Jelly> jellies;
    private Jelly player;
    private int gameTimeLeft;
    private double timePassed, tick;
    private DayScene _mainScene;

    public JellyGame(DayScene main) : base()
    {
        _mainScene = main;
    }
    public override void Initialize()
    {
        timePassed = tick = 0;
        jellies = new List<Jelly>();
        timePassed = 0;

        base.Initialize();
    }
    public override void LoadContent()
    {
        System.Random random = new System.Random();
        int red = random.Next(255);
        int green = random.Next(255);
        int blue = random.Next(255);
        Color newPlayerColor = new Color(red, green, blue);
        player = new Jelly(new Vector2(_mainScene.GameBounds.X * 2, _mainScene.GameBounds.Y + _mainScene.GameBounds.Y / 2), startingSize, newPlayerColor, _mainScene.Atlas.CreateSprite("JellyBlob"), this);
        player.scale = startingSize;
        player.velocityX = 0;
        player.velocityY = 0;
    }

    public override void Update(GameTime gameTime){
        if(Core.Input.Keyboard.CheckKeyPress(Keys.Left)){
            player.velocityX -= 1;
        }
        if(Core.Input.Keyboard.CheckKeyPress(Keys.Right)){
            player.velocityX += 1;
        }
        if(Core.Input.Keyboard.CheckKeyPress(Keys.Up)){
            player.velocityY -= 1;
        }
        if(Core.Input.Keyboard.CheckKeyPress(Keys.Down)){
            player.velocityY += 1;
        }
        player.Update(gameTime);

        timePassed += gameTime.ElapsedGameTime.TotalSeconds;
        gameTimeLeft = (int)(60 - timePassed);
        tick += gameTime.ElapsedGameTime.TotalSeconds;

        foreach(Jelly jelly in jellies.ToArray()){
            jelly.Update(gameTime);
            if(jelly.X > _mainScene.GameBounds.Right || jelly.X < _mainScene.GameBounds.X - spriteSize * jelly.scale + 1|| jelly.Y > _mainScene.GameBounds.Bottom || jelly.Y < _mainScene.GameBounds.Top - spriteSize * jelly.scale + 1)
                jellies.Remove(jelly);
            if(player.hitBox.Intersects(jelly.hitBox)){
                if(player.scale >= jelly.scale){
                    player.scale += jelly.scale*0.25f/player.scale;
                    player.sprite.Scale = new Vector2(player.scale, player.scale);
                }else{
                    EndGame();
                }
                jellies.Remove(jelly);
            }
        }
            

        if(tick > 1){
            System.Random random = new System.Random();
            int xSpawn = random.Next(_mainScene.GameBounds.X, _mainScene.GameBounds.Right);
            int ySpawn = random.Next(_mainScene.GameBounds.Y, _mainScene.GameBounds.Bottom);
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
                    spawnLocation = new Vector2(xSpawn, _mainScene.GameBounds.Y - spawnSize*spriteSize + 1);
                    spawnVelocityX = (float)(random.NextDouble()*6)-3;
                    spawnVelocityY = (float)(random.NextDouble()*3);
                    break;
                case 1://Right
                    spawnLocation = new Vector2(_mainScene.GameBounds.Right, ySpawn);
                    spawnVelocityX = (float)(random.NextDouble()*-3);
                    spawnVelocityY = (float)(random.NextDouble()*6)-3;
                    break;
                case 2://Bottom
                    spawnLocation = new Vector2(xSpawn, _mainScene.GameBounds.Bottom);
                    spawnVelocityX = (float)(random.NextDouble()*6)-3;
                    spawnVelocityY = (float)(random.NextDouble()*-3);
                    break;
                case 3://Left
                    spawnLocation = new Vector2(_mainScene.GameBounds.X - spawnSize*spriteSize + 1, ySpawn);
                    spawnVelocityX = (float)(random.NextDouble()*3);
                    spawnVelocityY = (float)(random.NextDouble()*6)-3;
                    break;
            }

            jellies.Add(new Jelly(spawnLocation, spawnSize, spawnColor, _mainScene.Atlas.CreateSprite("JellyBlob"), this){velocityX = spawnVelocityX, velocityY = spawnVelocityY});
            tick = 0;
        }

    }

    public override void Draw(GameTime gameTime){
        player.Draw(Core.SpriteBatch);
        foreach(Jelly jelly in jellies)
            jelly.Draw(Core.SpriteBatch);
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
        _mainScene.ChangeLowerTab(new SelectionScene(_mainScene));
    }


    private class Jelly : PhysicsObject{
        private JellyGame _game;

        public Jelly(Vector2 location, float size, Color color, Sprite sprite, JellyGame game) : base(){
            _x = location.X;
            _y = location.Y;
            this.color = color;
            sprite.color = color;
            sprite.Scale = new Vector2(size, size);
            scale = size;
            rotation = 0;
            this.sprite = sprite;
            _game = game;
        }

        public void Update(GameTime gameTime){
            
            if(_x < _game._mainScene.GameBounds.Left - spriteSize * scale)
                _x = _game._mainScene.GameBounds.Right;
            else if(_x > _game._mainScene.GameBounds.Right + 1)
                _x = _game._mainScene.GameBounds.Left;
            if(_y < _game._mainScene.GameBounds.Top - spriteSize * scale)
                _y = _game._mainScene.GameBounds.Bottom;
            else if(_y > _game._mainScene.GameBounds.Bottom + 1)
                _y = _game._mainScene.GameBounds.Top;
            UpdateLocation();
            
        }
    }
}

