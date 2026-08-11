using System;
using System.Collections.Generic;
using System.Linq;
using GeneralUtil;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CoreLibrary.Graphics;
using CoreLibrary.Input;
using CoreLibrary;
using CoreLibrary.Scenes;
using MyBakery.Scenes;

namespace MyBakery;

public class FlourGame : Scene
{
    const int spriteSize = 64;

    //DoughGame
    private Sprite grinderSprite, wheelSprite, _wheat;
    private int collectedFlour, gameTimeLeft, quota;
    private double timePassed;
    private Vector2 grinderPos, hopperPos, wheatPilePos, wheelPos;
    private Keys lastKey, firstKey;
    private bool containsWheat, pickedUp, isTurning;
    private MouseState currentMouse;
    private Point mousePos;
    private List<FallingObject> fallingObjects;
    private float wheelRotation, lastWheelRotation, totalRotations;
    private DayScene _mainScene;
    private SpriteFont _font;

    public FlourGame(DayScene main) : base()
    {
        _mainScene = main;
    }
    public override void Initialize()
    {

        //DoughGame
        grinderPos = new Vector2(_mainScene.GameBounds.X + 300, _mainScene.GameBounds.Y + 100);
        wheelPos = new Vector2(grinderPos.X + 128, grinderPos.Y + 128);
        hopperPos = new Vector2(grinderPos.X + 32, grinderPos.Y + 16);
        wheatPilePos = new Vector2(grinderPos.X - 128, grinderPos.Y);

        collectedFlour = 0;
        timePassed = 0;
        quota = 20; //Make dynamic based on... average?
        lastKey = Keys.None;
        containsWheat = false;
        pickedUp = false;
        isTurning = false;
        wheelRotation = 0.0f;

        fallingObjects = new List<FallingObject>();
        base.Initialize();
    }

    public override void LoadContent()
    {
        grinderSprite = _mainScene.Atlas.CreateSprite("Grinder");
        wheelSprite = _mainScene.Atlas.CreateSprite("Wheel");
        _wheat = _mainScene.Atlas.CreateSprite("Wheat");
        _font = Content.Load<SpriteFont>("font");
    }

    public override void Draw(GameTime gameTime)
    {
        //Doughgame
        Core.SpriteBatch.Draw(grinderSprite.Region.Texture, new Rectangle((int)wheatPilePos.X, (int)wheatPilePos.Y, 64, 64), Color.Green); //WHEAT BIN
        if (collectedFlour < quota)
            Core.SpriteBatch.DrawString(_font, "Flour collected: " + collectedFlour + "/" + quota, new Vector2(_mainScene.GameBounds.X + 10, _mainScene.GameBounds.Y + 10), Color.Red);
        else
            Core.SpriteBatch.DrawString(_font, "Flour collected: " + collectedFlour + "/" + quota, new Vector2(_mainScene.GameBounds.X + 10, _mainScene.GameBounds.Y + 10), Color.Green);
        Core.SpriteBatch.DrawString(_font, "Time Left: " + gameTimeLeft, new Vector2(_mainScene.GameBounds.X + 10, _mainScene.GameBounds.Y + 30), Color.White);
        Core.SpriteBatch.DrawString(_font, "Wheat: " + containsWheat, new Vector2(_mainScene.GameBounds.X + 10, _mainScene.GameBounds.Y + 50), Color.White);


        grinderSprite.Draw(Core.SpriteBatch, grinderPos);
        Core.SpriteBatch.Draw(wheelSprite.Region.Texture, wheelPos, wheelSprite.Region.SourceRectangle, Color.White, wheelRotation, new Vector2(64, 64), 1.0f, SpriteEffects.None, 1);
        if (pickedUp)
        {
            _wheat.Draw(Core.SpriteBatch, new Vector2(mousePos.X, mousePos.Y));
        }
        foreach(FallingObject o in fallingObjects){
            _wheat.Draw(Core.SpriteBatch, o.location);
        }


    }

    public override void Update(GameTime gameTime)
    {
        currentMouse = Mouse.GetState();
        mousePos = new Point(currentMouse.X, currentMouse.Y);
        if (isInside(mousePos, wheatPilePos, 64, 64) && currentMouse.LeftButton == ButtonState.Pressed)
        {
            pickedUp = true;
        }
        if (pickedUp && currentMouse.LeftButton == ButtonState.Released)
        {
            pickedUp = false;
            fallingObjects.Add(new FallingObject() { location = new Vector2(currentMouse.X, currentMouse.Y), fallSpeed = 4 });
        }

        List<FallingObject> qclist = fallingObjects.ToList();
        foreach (FallingObject o in qclist)
        {
            if (o.location.Y >= _mainScene.GameBounds.Bottom - 64)
                fallingObjects.Remove(o);
            else if (o.hitBox.Intersects(new Rectangle((int)hopperPos.X, (int)hopperPos.Y, 218, 32)))
            {
                fallingObjects.Remove(o);
                containsWheat = true;
            }
            else
                o.location = new Vector2(o.location.X, o.location.Y + o.fallSpeed);
        }
        if ((isTurning || isInside(mousePos, new Vector2(wheelPos.X - 64, wheelPos.Y - 64), 128, 128)) && currentMouse.LeftButton == ButtonState.Pressed && containsWheat)
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

        bool pressed = false;
        if (Core.Input.Keyboard.CheckKeyRelease(Keys.Left) && (lastKey == Keys.Up || lastKey == Keys.None))
        {
            if (lastKey == Keys.None)
            {
                firstKey = Keys.Left;
            }
            wheelRotation = (float)(3 * Math.PI / 2);
            lastKey = Keys.Left;
            pressed = true;
        }
        if (Core.Input.Keyboard.CheckKeyRelease(Keys.Right) && (lastKey == Keys.Down || lastKey == Keys.None))
        {
            if (lastKey == Keys.None)
            {
                firstKey = Keys.Right;
            }
            wheelRotation = (float)(Math.PI / 2); ;
            lastKey = Keys.Right;
            pressed = true;
        }
        if (Core.Input.Keyboard.CheckKeyRelease(Keys.Up) && (lastKey == Keys.Right || lastKey == Keys.None))
        {
            if (lastKey == Keys.None)
            {
                firstKey = Keys.Up;
            }
            wheelRotation = 0;
            lastKey = Keys.Up;
            pressed = true;
        }
        if (Core.Input.Keyboard.CheckKeyRelease(Keys.Down) && (lastKey == Keys.Left || lastKey == Keys.None))
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

            if (GameManager.PlayerInfo.inventory.ContainsKey("Flour"))
            {
                GameManager.PlayerInfo.inventory["Flour"] += collectedFlour;
            }
            else
            {
                GameManager.PlayerInfo.inventory["Flour"] = collectedFlour;
            }
            fallingObjects.Clear();
            _mainScene.ChangeLowerTab(new SelectionScene(_mainScene));
        }
        lastWheelRotation = wheelRotation;
    }

    public static Boolean isInside(Point p1, Vector2 vec2, int xsize, int ysize)
    {
        Rectangle obj1 = new Rectangle((int)vec2.X, (int)vec2.Y, xsize, ysize);
        return obj1.Contains(p1);
    }

    private class FallingObject
    {
        Vector2 _location;
        int _fallSpeed;
        String _type;

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
        public String type
        {
            get => _type;
            set => _type = value;
        }

        public Rectangle hitBox
        {
            get => new Rectangle((int)location.X, (int)location.Y, 64, 64);
        }
    }
}
