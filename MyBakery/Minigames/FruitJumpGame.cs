using System;
using System.Collections.Generic;
using GeneralUtil;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MyBakery;


public static class FruitJumpGame{
    

    private static int gameXOrigin = (int)GameManager.bottomScreenOrigin.X;
    private static int gameYOrigin = (int)GameManager.bottomScreenOrigin.Y;
    const int spriteSize = 64;
    const float fallSpeed = 6.5f;

    private static Sprite orangeSprite, cherrySprite, appleSprite, playerSprite;
    private static Texture2D whiteBox;
    private static List<Fruit> fruits;
    private static Player player1, player2, activePlayer, passivePlayer;
    private static int collectedOranges, collectedCherries, collectedApples, gameTimeLeft;
    private static double timePassed, tick;
    public static void Initialize(GraphicsDevice graphicsDevice, Texture2D spriteSheet)
    {
        collectedOranges = collectedCherries = collectedApples = 0;
        orangeSprite = new Sprite(spriteSheet, new Rectangle(128, 0, 64, 64));
        cherrySprite = new Sprite(spriteSheet, new Rectangle(64, 0, 64, 64));
        appleSprite = new Sprite(spriteSheet, new Rectangle(0, 0, 64, 64));
        playerSprite = new Sprite(spriteSheet, new Rectangle(128, 64, 64, 64));

        whiteBox = new Texture2D(graphicsDevice, 1, 1);
        whiteBox.SetData(new[] {Color.White});

        player1 = new Player(new Vector2(gameXOrigin*1.5f, GameManager.gameHeight-100), playerSprite, true);
        player2 = new Player(new Vector2(gameXOrigin*2.5f, GameManager.gameHeight-100), playerSprite, false);
        fruits = new List<Fruit>();
        timePassed = tick = 0;
        activePlayer = player1;
        passivePlayer = player2;
    }

    public static void Draw(SpriteFont font, SpriteBatch spriteBatch)
    {

        spriteBatch.Draw(whiteBox, new Rectangle(gameXOrigin, gameYOrigin, GameManager.gameWidth*2/3, GameManager.gameHeight/2), Color.Beige);
        foreach(Fruit f in fruits){
            f.Draw(spriteBatch);
        }
        player1.Draw(spriteBatch);
        player2.Draw(spriteBatch);
        spriteBatch.DrawString(font, "Apples: " + collectedApples, new Vector2(gameXOrigin, gameYOrigin), Color.Black);
        spriteBatch.DrawString(font, "Cherries: " + collectedCherries, new Vector2(gameXOrigin, gameYOrigin+32), Color.Black);
        spriteBatch.DrawString(font, "Oranges: " + collectedOranges, new Vector2(gameXOrigin, gameYOrigin+64), Color.Black);
        spriteBatch.DrawString(font, "Time Left: " + gameTimeLeft, new Vector2(gameXOrigin, gameYOrigin+96), Color.Black);

    }

    public static void Update(GameTime gameTime)
    {
        if(Keyboard.GetState().IsKeyDown(Keys.Left) && activePlayer.X > gameXOrigin)
            activePlayer.velocityX = -3;
        if(Keyboard.GetState().IsKeyDown(Keys.Right) && activePlayer.X < GameManager.gameWidth-spriteSize)
            activePlayer.velocityX = 3;
        if(activePlayer.Y > GameManager.gameHeight - 100){
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
            GameManager.inventory[4].Quantity += collectedApples;
            GameManager.inventory[5].Quantity += collectedCherries;
            GameManager.inventory[6].Quantity += collectedOranges;
            GameManager.CurrentMinigameState = GameManager.MinigameState.Select;
        }else{
            Random rand = new();
            int spawnChance = rand.Next(100)+1;
            int xlocation = (int)(rand.Next(GameManager.gameWidth-gameXOrigin-96)/2 + passivePlayer.leftBound);
            int ylocation = rand.Next(GameManager.gameHeight - gameYOrigin - gameYOrigin*2/3) + gameYOrigin;
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
            hitBox = new Rectangle((int)_location.X, (int)_location.Y, sprite.TextureMapLocation.Width, sprite.TextureMapLocation.Height);
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
        public Player(Vector2 location, Sprite sprite, Boolean active) : base(){
            _x = location.X;
            _y = location.Y;
            this.sprite = sprite;
            velocityX = 0;
            velocityY = 0; 
            rotation = 0;
            Active = active;
            rightBound = _x + (GameManager.gameWidth - gameXOrigin)/4;
            leftBound = _x - (GameManager.gameWidth - gameXOrigin)/4;
        }

        public void Update(GameTime gameTime){
            if((_x < leftBound && velocityX < 0) || (_x > rightBound - 64 && velocityX > 0))
                velocityX = 0;
            if((_y < gameYOrigin && velocityY < 0)|| _y > GameManager.gameHeight)
                velocityY = 0;
            UpdateLocation();
        }
    }
}