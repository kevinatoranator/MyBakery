using System;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using GeneralUtil;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CoreLibrary.Graphics;
using CoreLibrary.Scenes;
using MyBakery.Scenes;
using CoreLibrary;

namespace MyBakery;

public class BakingGame : Scene
{

    const int spriteSize = 64;

    //Baking
    private Sprite ovenSprite, matchSprite, woodSprite, iceSprite, matchBoxSprite, progressFront, progressBack, tempFront, tempBack;
    private int bakedGoods, gameTimeLeft, previousTime, quota, idealTemp, currentTemp;//baked cookies needs to be whatever selected good
    private double timePassed;
    private  MouseState currentMouse;
    private Vector2 ovenPos, pilePos;
    private Point mousePos;
    private Item heldItem;
    private ProgressBar timerBar, quotaBar, tempBar;
    //Texture2D bg;
    private DayScene _mainScene;

    public BakingGame(DayScene main) : base()
    {
        _mainScene = main;
    }
    public override void Initialize()
    {
        bakedGoods = 0;
        timePassed = 0;
        previousTime = 30;
        idealTemp = 800;
        currentTemp = 800;
        quota = 20; //Make dynamic based on... average?
        base.Initialize();
    }
    public override void LoadContent()
    {

        //BakingGame
        ovenSprite = _mainScene.Atlas.CreateSprite("Oven");
        matchSprite = _mainScene.Atlas.CreateSprite("Match");
        woodSprite = _mainScene.Atlas.CreateSprite("Wood");
        iceSprite = _mainScene.Atlas.CreateSprite("Ice");
        matchBoxSprite = _mainScene.Atlas.CreateSprite("MatchBox");
        ovenPos = new Vector2(_mainScene.GameBounds.X + 100, _mainScene.GameBounds.Y + 100);
        pilePos = new Vector2(_mainScene.GameBounds.X + 400, _mainScene.GameBounds.Y + 100);
        progressFront = _mainScene.Atlas.CreateSprite("ProgressFront");
        progressBack = _mainScene.Atlas.CreateSprite("ProgressBack");
        tempFront = _mainScene.Atlas.CreateSprite("TempFront");
        tempBack = _mainScene.Atlas.CreateSprite("TempBack");

        timerBar = new ProgressBar(progressFront, progressBack, previousTime, previousTime, new Vector2(_mainScene.GameBounds.X + 10, _mainScene.GameBounds.Y + 30), false);
        quotaBar = new ProgressBar(progressFront, progressBack, quota, bakedGoods, new Vector2(_mainScene.GameBounds.X + 10, _mainScene.GameBounds.Y + 130), false);
        tempBar = new ProgressBar(tempFront, tempBack, 1000, currentTemp, new Vector2(1240, _mainScene.GameBounds.Y + 30), true);
    }


    public override void Draw(GameTime gameTime)
    {
        //BakingGame

        ovenSprite.Draw(Core.SpriteBatch, ovenPos);
        matchBoxSprite.Draw(Core.SpriteBatch, pilePos);

        Vector2 itemPos = new Vector2(mousePos.X-spriteSize/2, mousePos.Y-spriteSize);
        if(heldItem is not null){
            if(heldItem.type == "ice")
                iceSprite.Draw(Core.SpriteBatch, itemPos);
            else if(heldItem.type == "wood")
                woodSprite.Draw(Core.SpriteBatch, itemPos);
            else
                matchSprite.Draw(Core.SpriteBatch, itemPos);
        }

        /*if(bakedGoods < quota)
            spriteBatch.DrawString(font, "Baked Foods: " + bakedGoods +"/"+quota, new Vector2(gameXOrigin + 10, gameYOrigin + 10), Color.Red);
        else
            spriteBatch.DrawString(font, "Baked Foods: " + bakedGoods +"/"+quota, new Vector2(gameXOrigin + 10, gameYOrigin + 10), Color.Green);*/
        quotaBar.Draw(Core.SpriteBatch);
        //spriteBatch.DrawString(font, "Time Left: " + gameTimeLeft, new Vector2(gameXOrigin + 10, gameYOrigin + 30), Color.Black);
        timerBar.Draw(Core.SpriteBatch);
        
        /*if(currentTemp < idealTemp - 100)
            spriteBatch.DrawString(font, "Temp: " + currentTemp, new Vector2(gameXOrigin + 10, gameYOrigin + 50), Color.Blue);
        else if(currentTemp > idealTemp + 100)
            spriteBatch.DrawString(font, "Temp: " + currentTemp, new Vector2(gameXOrigin + 10, gameYOrigin + 50), Color.Red);
        else
            spriteBatch.DrawString(font, "Temp: " + currentTemp, new Vector2(gameXOrigin + 10, gameYOrigin + 50), Color.Green);*/
        tempBar.Draw(Core.SpriteBatch);

    }

    public override void Update(GameTime gameTime)
    {

        timePassed += gameTime.ElapsedGameTime.TotalSeconds;
        gameTimeLeft = (int)(30 - timePassed);
        //BakingGame
        currentMouse = Mouse.GetState();
        mousePos = new Point(currentMouse.X, currentMouse.Y);
        if(isInside(mousePos, pilePos, spriteSize, spriteSize) && currentMouse.LeftButton == ButtonState.Pressed && heldItem is null){
            Random rand = new();
            int spawnChance = rand.Next(100)+1;
            if(spawnChance < 20){
                heldItem = new Item(){type="ice"};
            }else if(spawnChance < 80){
                heldItem = new Item(){type="wood"};
            }else{
                heldItem = new Item(){type="match"};
            }
        }
        if(heldItem is not null){
            if(isInside(mousePos, ovenPos, ovenSprite.Region.Width, ovenSprite.Region.Height) && currentMouse.LeftButton == ButtonState.Released){
                if(heldItem.type == "ice")
                    currentTemp -= 125;
                else if(heldItem.type == "wood")
                    currentTemp += 125;
                else
                    currentTemp += 175;
                heldItem = null;
            }else if(!isInside(mousePos, ovenPos, ovenSprite.Region.Width, ovenSprite.Region.Height) && currentMouse.LeftButton == ButtonState.Released){
                heldItem = null;
            }
        }

        if(previousTime != gameTimeLeft && currentTemp > idealTemp - 100 && currentTemp < idealTemp + 100)
            bakedGoods += 1;


        currentTemp -= 1;
        previousTime = gameTimeLeft;

        if(gameTimeLeft < 0){
            gameTimeLeft = 0;
            if(bakedGoods > quota)
                bakedGoods += bakedGoods/2;
            if (GameManager.PlayerInfo.inventory.ContainsKey("Cookie"))
            {
                GameManager.PlayerInfo.inventory["Cookie"] += bakedGoods;
            }
            else
            {
                GameManager.PlayerInfo.inventory["Cookie"] = bakedGoods;
            }
            
            _mainScene.ChangeLowerTab(new SelectionScene(_mainScene));
        }

        quotaBar.Update(bakedGoods);
        timerBar.Update(gameTimeLeft);
        tempBar.Update(currentTemp);
    }

    private class Item{
        String _type;
        public String type{
            get => _type;
            set => _type = value;
        }
    }

    private static Boolean isInside(Point p1, Vector2 vec2, int xsize, int ysize)
    {
        Rectangle obj1 = new Rectangle((int)vec2.X, (int)vec2.Y, xsize, ysize);
        return obj1.Contains(p1);
    }
}