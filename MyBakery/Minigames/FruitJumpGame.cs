using System;
using System.Collections.Generic;
using GeneralUtil;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CoreLibrary.Graphics;
using MyBakery.Scenes;
using CoreLibrary.Scenes;
using CoreLibrary;

namespace MyBakery;


public class FruitJumpGame : Scene{
    
    const int spriteSize = 64;
    const float fallSpeed = 6.5f;

    private Sprite orangeSprite, cherrySprite, appleSprite, playerSprite;
    //private Texture2D bg;
    private List<Fruit> fruits;
    private Player player1, player2, activePlayer, passivePlayer;
    private int collectedOranges, collectedCherries, collectedApples, gameTimeLeft;
    private double timePassed, tick;
    private DayScene _mainScene;
    private SpriteFont _font;
    

    public FruitJumpGame(DayScene main) : base()
    {
        _mainScene = main;
    }
    public override void Initialize()
    {
        collectedOranges = collectedCherries = collectedApples = 0;
        fruits = new List<Fruit>();
        timePassed = tick = 0;
        
        base.Initialize();
    }

    public override void LoadContent()
    {
        orangeSprite = _mainScene.Atlas.CreateSprite("Orange");
        cherrySprite = _mainScene.Atlas.CreateSprite("Cherry");
        appleSprite = _mainScene.Atlas.CreateSprite("Apple");
        playerSprite = _mainScene.Atlas.CreateSprite("ToastDog");
        _font = Content.Load<SpriteFont>("font");


        player1 = new Player(new Vector2(_mainScene.GameBounds.X*1.5f, _mainScene.GameBounds.Bottom-100), playerSprite, true, this);
        player2 = new Player(new Vector2(_mainScene.GameBounds.X*2.5f, _mainScene.GameBounds.Bottom-100), playerSprite, false, this);
        activePlayer = player1;
        passivePlayer = player2;
    }

    public override void Draw(GameTime gameTime)
    {

        foreach(Fruit f in fruits){
            f.Draw(Core.SpriteBatch);
        }
        player1.Draw(Core.SpriteBatch);
        player2.Draw(Core.SpriteBatch);
        Core.SpriteBatch.DrawString(_font, "Apples: " + collectedApples, new Vector2(_mainScene.GameBounds.X, _mainScene.GameBounds.Y), Color.Black);
        Core.SpriteBatch.DrawString(_font, "Cherries: " + collectedCherries, new Vector2(_mainScene.GameBounds.X, _mainScene.GameBounds.Y+32), Color.Black);
        Core.SpriteBatch.DrawString(_font, "Oranges: " + collectedOranges, new Vector2(_mainScene.GameBounds.X, _mainScene.GameBounds.Y+64), Color.Black);
        Core.SpriteBatch.DrawString(_font, "Time Left: " + gameTimeLeft, new Vector2(_mainScene.GameBounds.X, _mainScene.GameBounds.Y+96), Color.Black);

    }

    public override void Update(GameTime gameTime)
    {
        if(Keyboard.GetState().IsKeyDown(Keys.Left) && activePlayer.X > _mainScene.GameBounds.X)
            activePlayer.velocityX = -3;
        if(Keyboard.GetState().IsKeyDown(Keys.Right) && activePlayer.X < _mainScene.GameBounds.Right-spriteSize)
            activePlayer.velocityX = 3;
        if(activePlayer.Y > _mainScene.GameBounds.Bottom - 100){
            if(activePlayer == player1){
                activePlayer = player2;
                passivePlayer = player1;
            }else{
                activePlayer = player1;
                passivePlayer = player2;
            }
                
            activePlayer.velocityY = -10;
            activePlayer.velocityX = 0;
        }
    
        timePassed += gameTime.ElapsedGameTime.TotalSeconds;
        gameTimeLeft = (int)(60 - timePassed);
        tick += gameTime.ElapsedGameTime.TotalSeconds;

        activePlayer.velocityY += (float)(fallSpeed *gameTime.ElapsedGameTime.TotalSeconds);
        activePlayer.Update(gameTime);
        
        
        foreach(Fruit fruit in fruits.ToArray()){
            if(activePlayer.hitBox.Intersects(fruit.hitBox)){
                if(fruit.type == "cherry"){
                    collectedCherries += 1;
                }else if(fruit.type == "orange"){
                    collectedOranges += 1;
                }else if(fruit.type == "apple"){
                    collectedApples += 1;
                }
                    
                fruits.Remove(fruit);
            }
        }

        if(gameTimeLeft < 0){
            fruits.Clear();
            gameTimeLeft = 0;
            if (GameManager.PlayerInfo.inventory.ContainsKey("Apple") && GameManager.PlayerInfo.inventory.ContainsKey("Cherry") && GameManager.PlayerInfo.inventory.ContainsKey("Orange"))
            {
                GameManager.PlayerInfo.inventory["Apple"] += collectedApples;
                GameManager.PlayerInfo.inventory["Cherry"] += collectedCherries;
                GameManager.PlayerInfo.inventory["Orange"] += collectedOranges;
            }
            else
            {
                GameManager.PlayerInfo.inventory["Apple"] = collectedApples;
                GameManager.PlayerInfo.inventory["Cherry"] = collectedCherries;
                GameManager.PlayerInfo.inventory["Orange"] = collectedOranges;
            }
            
            _mainScene.ChangeLowerTab(new SelectionScene(_mainScene));
        }else{
            Random rand = new();
            int spawnChance = rand.Next(100)+1;
            int xlocation = (int)(rand.Next(_mainScene.GameBounds.Width-96)/2 + passivePlayer.leftBound);
            int ylocation = rand.Next(_mainScene.GameBounds.Height - _mainScene.GameBounds.Y*2/3) + _mainScene.GameBounds.Y;
            String sType = "";
            Sprite randSprite;
            if(spawnChance > 67){
                sType="cherry";
                randSprite = cherrySprite;
            }else if(spawnChance > 33){
                sType="orange";
                randSprite = orangeSprite;
            }else{
                sType="apple";
                randSprite = appleSprite;
            }

            if(tick > 1){
                fruits.Add(new Fruit(new Vector2(xlocation, ylocation), randSprite){ type=sType});
                if(fruits.Count > 7){
                    fruits.RemoveAt(0);
                }
                tick = 0;
            }
        }
    }


    private class Fruit : PhysicsObject{
        Vector2 _location;
        String _type;

        public Fruit(Vector2 location, Sprite sprite){
            _location = location;
            _x = _location.X;
            _y = _location.Y;
            this.sprite = sprite;
            hitBox = new Rectangle((int)_location.X, (int)_location.Y, sprite.Region.Width, sprite.Region.Height);
        }
        public Vector2 location{
            get => _location;
            set => _location = value;
        }

        public String type{
            get => _type;
            set => _type = value;
        }
    }

    private class Player : PhysicsObject{
        public Boolean Active;
        public float rightBound, leftBound;
        private FruitJumpGame _game;
        public Player(Vector2 location, Sprite sprite, Boolean active, FruitJumpGame game) : base(){
            _x = location.X;
            _y = location.Y;
            this.sprite = sprite;
            velocityX = 0;
            velocityY = 0; 
            rotation = 0;
            Active = active;
            _game = game;
            rightBound = _x + _game._mainScene.GameBounds.Width / 4;
            leftBound = _x - _game._mainScene.GameBounds.Width / 4;
        }

        public void Update(GameTime gameTime){
            if((_x < leftBound && velocityX < 0) || (_x > rightBound - 64 && velocityX > 0))
                velocityX = 0;
            if((_y < _game._mainScene.GameBounds.Top && velocityY < 0)|| _y > _game._mainScene.GameBounds.Bottom)
                velocityY = 0;
            UpdateLocation();
        }
    }
}