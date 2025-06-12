using System;
using System.Collections.Generic;
using System.Linq;
using GeneralUtil;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MyBakery;

public static class FlourGame
{

    private static int gameXOrigin = (int)GameManager.bottomScreenOrigin.X;
    private static int gameYOrigin = (int)GameManager.bottomScreenOrigin.Y;
    const int spriteSize = 64;

    //DoughGame
    private static Sprite grinderSprite, wheelSprite;
    private static Texture2D whiteBox;
    private static int collectedFlour, gameTimeLeft, quota;
    private static double timePassed;
    private static Vector2 grinderPos, hopperPos, wheatPilePos, wheelPos;
    private static Keys lastKey, firstKey;
    private static bool containsWheat, pickedUp, isTurning;
    private static MouseState currentMouse;
    private static Point mousePos;
    private static List<FallingObject> fallingObjects;
    private static float wheelRotation, lastWheelRotation, totalRotations;

    public static void Initialize(GraphicsDevice graphicsDevice, Texture2D spriteSheet)
    {

        //DoughGame
        grinderSprite = new Sprite(spriteSheet, new Rectangle(384, 0, 256, 256));
        wheelSprite = new Sprite(spriteSheet, new Rectangle(256, 0, 128, 128));
        grinderPos = new Vector2(gameXOrigin + 300, gameYOrigin + 100);
        wheelPos = new Vector2(grinderPos.X + 128, grinderPos.Y + 128);
        hopperPos = new Vector2(grinderPos.X + 32, grinderPos.Y + 16);
        wheatPilePos = new Vector2(grinderPos.X - 128, grinderPos.Y);
        whiteBox = new Texture2D(graphicsDevice, 1, 1);
        whiteBox.SetData(new[] { Color.White });

        collectedFlour = 0;
        timePassed = 0;
        quota = 20; //Make dynamic based on... average?
        lastKey = Keys.None;
        containsWheat = false;
        pickedUp = false;
        isTurning = false;
        wheelRotation = 0.0f;

        fallingObjects = new List<FallingObject>();
    }


    public static void Draw(SpriteFont font, SpriteBatch spriteBatch)
    {
        //Doughgame
        spriteBatch.Draw(whiteBox, new Rectangle(gameXOrigin, gameYOrigin, GameManager.gameWidth * 2 / 3, GameManager.gameHeight / 2), Color.Beige);
        spriteBatch.Draw(whiteBox, new Rectangle((int)wheatPilePos.X, (int)wheatPilePos.Y, 64, 64), Color.Green);
        if (collectedFlour < quota)
            spriteBatch.DrawString(font, "Flour collected: " + collectedFlour + "/" + quota, new Vector2(gameXOrigin + 10, gameYOrigin + 10), Color.Red);
        else
            spriteBatch.DrawString(font, "Flour collected: " + collectedFlour + "/" + quota, new Vector2(gameXOrigin + 10, gameYOrigin + 10), Color.Green);
        spriteBatch.DrawString(font, "Time Left: " + gameTimeLeft, new Vector2(gameXOrigin + 10, gameYOrigin + 30), Color.Black);
        spriteBatch.DrawString(font, "Wheat: " + containsWheat, new Vector2(gameXOrigin + 10, gameYOrigin + 50), Color.Black);


        spriteBatch.Draw(grinderSprite.Texture, grinderPos, grinderSprite.TextureMapLocation, Color.White);
        spriteBatch.Draw(wheelSprite.Texture, wheelPos, wheelSprite.TextureMapLocation, Color.White, wheelRotation, new Vector2(64, 64), 1.0f, SpriteEffects.None, 1);
        if (pickedUp)
        {
            spriteBatch.Draw(GameManager.TextureDB[GameManager.Items.Wheat].Texture, new Vector2(mousePos.X, mousePos.Y), GameManager.TextureDB[GameManager.Items.Wheat].TextureMapLocation, Color.White);
        }
        foreach(FallingObject o in fallingObjects){
            spriteBatch.Draw(GameManager.TextureDB[GameManager.Items.Wheat].Texture, o.location, GameManager.TextureDB[GameManager.Items.Wheat].TextureMapLocation, Color.White);
        }


    }

    public static void Update(GameTime gameTime)
    {
        currentMouse = Mouse.GetState();
        mousePos = new Point(currentMouse.X, currentMouse.Y);
        if (MinigameManager.isInside(mousePos, wheatPilePos, 64, 64) && currentMouse.LeftButton == ButtonState.Pressed)
        {
            pickedUp = true;
        }
        if (pickedUp && currentMouse.LeftButton == ButtonState.Released)
        {
            pickedUp = false;
            fallingObjects.Add(new FallingObject() { location = new Vector2(currentMouse.X, currentMouse.Y) });
        }

        List<FallingObject> qclist = fallingObjects.ToList();
        foreach (FallingObject o in qclist)
        {
            if (o.location.Y >= GameManager.gameHeight - 64)
                fallingObjects.Remove(o);
            else if (o.hitBox.Intersects(new Rectangle((int)hopperPos.X, (int)hopperPos.Y, 218, 32)))
            {
                fallingObjects.Remove(o);
                containsWheat = true;
            }
            else
                o.location = new Vector2(o.location.X, o.location.Y + o.fallSpeed);
        }
        if ((isTurning || MinigameManager.isInside(mousePos, new Vector2(wheelPos.X - 64, wheelPos.Y - 64), 128, 128)) && currentMouse.LeftButton == ButtonState.Pressed && containsWheat)
        {
            isTurning = true;
            wheelRotation = (float)(Math.Atan2(mousePos.Y - wheelPos.Y, mousePos.X - wheelPos.X) + Math.PI / 2);
            totalRotations += Math.Abs(wheelRotation - lastWheelRotation);
            if (totalRotations >= (3 * Math.PI))
            {
                collectedFlour++;
                if (collectedFlour % 8 == 0)
                {
                    containsWheat = false;
                    isTurning = false;
                }
                totalRotations -= (float)(3 * Math.PI);
            }

        }
        else
        {
            isTurning = false;
        }


        KBoard.CheckKey();
        bool pressed = false;
        if (KBoard.CheckKeyRelease(Keys.Left) && (lastKey == Keys.Up || lastKey == Keys.None))
        {
            if (lastKey == Keys.None)
            {
                firstKey = Keys.Left;
            }
            wheelRotation = (float)(3 * Math.PI / 2);
            lastKey = Keys.Left;
            pressed = true;
        }
        if (KBoard.CheckKeyRelease(Keys.Right) && (lastKey == Keys.Down || lastKey == Keys.None))
        {
            if (lastKey == Keys.None)
            {
                firstKey = Keys.Right;
            }
            wheelRotation = (float)(Math.PI / 2); ;
            lastKey = Keys.Right;
            pressed = true;
        }
        if (KBoard.CheckKeyRelease(Keys.Up) && (lastKey == Keys.Right || lastKey == Keys.None))
        {
            if (lastKey == Keys.None)
            {
                firstKey = Keys.Up;
            }
            wheelRotation = 0;
            lastKey = Keys.Up;
            pressed = true;
        }
        if (KBoard.CheckKeyRelease(Keys.Down) && (lastKey == Keys.Left || lastKey == Keys.None))
        {
            if (lastKey == Keys.None)
            {
                firstKey = Keys.Down;
            }
            wheelRotation = (float)Math.PI;
            lastKey = Keys.Down;
            pressed = true;
        }
        if (lastKey == firstKey && pressed && containsWheat)
        {
            collectedFlour++;
            if (collectedFlour % 4 == 0)
            {
                containsWheat = false;
            }
        }
        timePassed += gameTime.ElapsedGameTime.TotalSeconds;
        gameTimeLeft = (int)(30 - timePassed);
        if (gameTimeLeft < 0)
        {
            gameTimeLeft = 0;
            if (collectedFlour > quota)
                collectedFlour += collectedFlour / 2;

            if (GameManager.PlayerInfo.inventory.ContainsKey(GameManager.Items.Flour))
            {
                GameManager.PlayerInfo.inventory[GameManager.Items.Flour] += collectedFlour;
            }
            else
            {
                GameManager.PlayerInfo.inventory[GameManager.Items.Flour] = collectedFlour;
            }
            GameManager.CurrentMinigameState = GameManager.MinigameState.Select;
        }
        lastWheelRotation = wheelRotation;
    }
    
    public static void Restart(){
        fallingObjects.Clear();
        timePassed = 0;
        collectedFlour = 0;
        lastKey = Keys.None;
        containsWheat = false;
        pickedUp = false;
        isTurning = false;
        wheelRotation = 0.0f;
    }
    private class FallingObject
    {
        Vector2 _location;
        int _fallSpeed = 4;


        public Vector2 location
        {
            get => _location;
            set => _location = value;
        }
        public int fallSpeed
        {
            get => _fallSpeed;
            set => _fallSpeed = value;
        }
        public Rectangle hitBox
        {
            get => new Rectangle((int)location.X, (int)location.Y, spriteSize, spriteSize);
        }
    }
}
