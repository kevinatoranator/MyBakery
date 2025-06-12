using System;
using System.Collections.Generic;
using GeneralUtil;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MyBakery;


public static class CoffeeSnakeGame{

    private enum Direction{
        North,
        South,
        East,
        West,
        None
    };
    private static int gameXOrigin = (int)GameManager.bottomScreenOrigin.X;
    private static int gameYOrigin = (int)GameManager.bottomScreenOrigin.Y;
    const int spriteSize = 64;

    private static double timePassed, tick;

    private static Player player;
    private static Sprite playerSprite, coffeeSprite;
    private static float playerSpeed;
    private static List<PhysicsObject> availableBeans = new List<PhysicsObject>();
    private static List<Player> snake = new List<Player>();

    public static void Initialize(Texture2D spriteSheet){
        playerSprite = new Sprite(spriteSheet, new Rectangle(128, 64, spriteSize, spriteSize));
        coffeeSprite = new Sprite(spriteSheet, new Rectangle(256, 128, spriteSize, spriteSize));

    }

    public static void Update(GameTime gameTime){
        KBoard.CheckKey();
        if(KBoard.CheckKeyRelease(Keys.Left) && player.velocityX == 0){
            player.velocityX = -playerSpeed;
            player.velocityY = 0;
            player.nextMoves.Add((new Vector2(player.X, player.Y), Direction.West));
        }
        if(KBoard.CheckKeyRelease(Keys.Right) && player.velocityX == 0){
            player.velocityX = playerSpeed;
            player.velocityY = 0;
            player.nextMoves.Add((new Vector2(player.X, player.Y), Direction.East));
        }
        if(KBoard.CheckKeyRelease(Keys.Up) && player.velocityY == 0){
            player.velocityY = -playerSpeed;
            player.velocityX = 0;
            player.nextMoves.Add((new Vector2(player.X, player.Y), Direction.North));
        }
        if(KBoard.CheckKeyRelease(Keys.Down) && player.velocityY == 0){
            player.velocityY = playerSpeed;
            player.velocityX = 0;
            player.nextMoves.Add((new Vector2(player.X, player.Y), Direction.South));
        }
        if(snake.Count == 1){
            player.nextMoves.Clear();
        }
        player.Update(gameTime);

        if(player.X < gameXOrigin || player.X > GameManager.gameWidth - 63 || player.Y < gameYOrigin || player.Y > GameManager.gameHeight - 96){
            EndGame();
        }

        if(availableBeans.Count < 1){
            Random rand = new Random();
            int spawnX = rand.Next(gameXOrigin, GameManager.gameWidth-64);
            int spawnY = rand.Next(gameYOrigin, GameManager.gameHeight-96);    
            availableBeans.Add(new Player(new Vector2(spawnX, spawnY), coffeeSprite));
        }
        foreach(Player bean in availableBeans.ToArray()){
            bean.Update(gameTime);
            if(player.hitBox.Intersects(bean.hitBox)){
                /*playerSpeed += 0.2f; when making turns gradually gets farther and farther off
                for(int i = 0; i < snake.Count; i++){
                    snake[i].velocityX = Math.Clamp(snake[i].velocityX, -1, 1) * playerSpeed;
                    snake[i].velocityY = Math.Clamp(snake[i].velocityY, -1, 1) * playerSpeed;
                }*/
                snake.Add(new Player(new Vector2(snake[snake.Count-1].X - 48 * Math.Clamp(snake[snake.Count-1].velocityX, -1, 1), snake[snake.Count-1].Y - 48 * Math.Clamp(snake[snake.Count-1].velocityY, -1, 1)), coffeeSprite));
                snake[snake.Count-1].velocityX = Math.Clamp(snake[snake.Count-2].velocityX, -1, 1) * playerSpeed;
                snake[snake.Count-1].velocityY = Math.Clamp(snake[snake.Count-2].velocityY, -1, 1) * playerSpeed;
                availableBeans.Remove(bean);
            }
        }

        for(int i = 1; i < snake.Count; i++){
            if(snake[i-1].nextMoves.Count > 0){
                if(snake[i-1].nextMoves[0].Item2 == Direction.North){
                    if((snake[i].velocityX < 0 && snake[i].X <= snake[i-1].nextMoves[0].Item1.X) || (snake[i].velocityX > 0 && snake[i].X >= snake[i-1].nextMoves[0].Item1.X)){
                        snake[i].velocityX = 0;
                        snake[i].velocityY = -playerSpeed;
                        //snake[i].X = snake[i-1].nextMoves[0].Item1.X;
                        if(i + 1 < snake.Count){
                            snake[i].nextMoves.Add(snake[i-1].nextMoves[0]);
                        }
                        snake[i-1].nextMoves.RemoveAt(0);
                    } 
                }
                else if(snake[i-1].nextMoves[0].Item2 == Direction.South){
                    if((snake[i].velocityX < 0 && snake[i].X <= snake[i-1].nextMoves[0].Item1.X) || (snake[i].velocityX > 0 && snake[i].X >= snake[i-1].nextMoves[0].Item1.X)){
                        snake[i].velocityX = 0;
                        snake[i].velocityY = playerSpeed;
                        //snake[i].X = snake[i-1].nextMoves[0].Item1.X;
                        if(i + 1 < snake.Count){
                            snake[i].nextMoves.Add(snake[i-1].nextMoves[0]);
                        }
                        snake[i-1].nextMoves.RemoveAt(0);
                    } 
                }
                else if(snake[i-1].nextMoves[0].Item2 == Direction.East){
                    if((snake[i].velocityY < 0 && snake[i].Y <= snake[i-1].nextMoves[0].Item1.Y) || (snake[i].velocityY > 0 && snake[i].Y >= snake[i-1].nextMoves[0].Item1.Y)){
                        snake[i].velocityX = playerSpeed;
                        snake[i].velocityY = 0;
                        //snake[i].Y = snake[i-1].nextMoves[0].Item1.Y;
                        if(i + 1 < snake.Count){
                            snake[i].nextMoves.Add(snake[i-1].nextMoves[0]);
                        }
                        snake[i-1].nextMoves.RemoveAt(0);
                    } 
                }
                else if(snake[i-1].nextMoves[0].Item2 == Direction.West){
                    if((snake[i].velocityY < 0 && snake[i].Y <= snake[i-1].nextMoves[0].Item1.Y) || (snake[i].velocityY > 0 && snake[i].Y >= snake[i-1].nextMoves[0].Item1.Y)){
                        snake[i].velocityX = -playerSpeed;
                        snake[i].velocityY = 0;
                        //snake[i].Y = snake[i-1].nextMoves[0].Item1.Y;
                        if(i + 1 < snake.Count){
                            snake[i].nextMoves.Add(snake[i-1].nextMoves[0]);
                        }
                        snake[i-1].nextMoves.RemoveAt(0);
                    } 
                }
            }
            snake[i].Update(gameTime);
            /* HARD MODE
            snake[i-1].velocityX = (float)(playerSpeed * (snake[i-2].X - snake[i-1].X) * gameTime.ElapsedGameTime.TotalSeconds);
            snake[i-1].velocityY = (float)(playerSpeed * (snake[i-2].Y - snake[i-1].Y) * gameTime.ElapsedGameTime.TotalSeconds);
            snake[i-1].Update(gameTime);*/
            if(i > 2){
                if(snake[i].hitBox.Intersects(player.hitBox)){
                    EndGame();
                }
            }
        }
    }

    public static void Draw(SpriteBatch spriteBatch){
        player.Draw(spriteBatch);
        foreach(Player part in snake){
            part.Draw(spriteBatch);
        }
        foreach(Player bean in availableBeans){
            bean.Draw(spriteBatch);
        }
    }

    private static void EndGame(){
        GameManager.PlayerInfo.inventory[GameManager.Items.CoffeeBean] += snake.Count*5;
        GameManager.CurrentMinigameState = GameManager.MinigameState.Select;
    }

    public static void Restart(){
        availableBeans.Clear();
        snake.Clear();
        timePassed = tick = 0;
        player = new Player(new Vector2(gameXOrigin*2, gameYOrigin + gameYOrigin/2), playerSprite);
        snake.Add(player);
        //snake.Add(new Player(new Vector2(gameXOrigin*2 + 70, gameYOrigin + gameYOrigin/2), coffeeSprite));
        playerSpeed = 2;
    }


    private class Player : PhysicsObject{

        public List<(Vector2, Direction)> nextMoves = new List<(Vector2, Direction)>();


        public Player(Vector2 location, Sprite sprite){
            _x = location.X;
            _y = location.Y;
            this.sprite = sprite;
        }

        public void Update(GameTime gameTime){

            UpdateLocation();
        }
    }
}