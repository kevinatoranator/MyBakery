using System;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using GeneralUtil;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MyBakery;

public static class BakingGame
{

    private static int gameXOrigin = (int)GameManager.bottomScreenOrigin.X;
    private static int gameYOrigin = (int)GameManager.bottomScreenOrigin.Y;
    const int spriteSize = 64;

    //Baking
    private static Sprite ovenSprite, matchSprite, woodSprite, iceSprite, matchBoxSprite;
    private static Texture2D whiteBox;
    private static int bakedGoods, gameTimeLeft, previousTime, quota, idealTemp, currentTemp;//baked cookies needs to be whatever selected good
    private static double timePassed;
    private static  MouseState currentMouse;
    private static Vector2 ovenPos, pilePos;
    private static Point mousePos;
    private static Item heldItem;
    private static ProgressBar timerBar, quotaBar, tempBar;
    public static void Initialize(GraphicsDevice graphicsDevice, Texture2D spriteSheet, Texture2D oven, Texture2D match, Texture2D wood, Texture2D ice, Texture2D matchBox, Texture2D progressFront, Texture2D progressBack, Texture2D tempFront, Texture2D tempBack)
    {

        //BakingGame
        ovenSprite = new Sprite(oven, new Rectangle(0, 0, 256, 256));
        matchSprite = new Sprite(match, new Rectangle(0, 0, 64, 128));
        woodSprite = new Sprite(wood, new Rectangle(0, 0, 64, 128));
        iceSprite = new Sprite(ice, new Rectangle(0, 0, 64, 128));
        matchBoxSprite = new Sprite(matchBox, new Rectangle(0, 0, 64, 64));
        whiteBox = new Texture2D(graphicsDevice, 1, 1);
        whiteBox.SetData(new[] {Color.White});
        ovenPos = new Vector2(gameXOrigin+100,gameYOrigin+100);
        pilePos = new Vector2(gameXOrigin+400,gameYOrigin+100);

        bakedGoods = 0;
        timePassed = 0;
        previousTime = 30;
        idealTemp = 800;
        currentTemp = 800;
        quota = 20; //Make dynamic based on... average?

        timerBar = new ProgressBar(progressFront, progressBack, previousTime, previousTime, new Vector2(gameXOrigin + 10, gameYOrigin + 30), false);
        quotaBar = new ProgressBar(progressFront, progressBack, quota, bakedGoods, new Vector2(gameXOrigin + 10, gameYOrigin + 130), false);
        tempBar = new ProgressBar(tempFront, tempBack, 1000, currentTemp, new Vector2(1240, gameYOrigin + 30), true);
    }


    public static void Draw(SpriteFont font, SpriteBatch spriteBatch)
    {
        //BakingGame
        spriteBatch.Draw(whiteBox, new Rectangle(gameXOrigin, gameYOrigin, GameManager.gameWidth*2/3, GameManager.gameHeight/2), Color.Beige);

        spriteBatch.Draw(ovenSprite.Texture, ovenPos, ovenSprite.TextureMapLocation, Color.White);
        spriteBatch.Draw(matchBoxSprite.Texture, pilePos, matchBoxSprite.TextureMapLocation, Color.White);

        Vector2 itemPos = new Vector2(mousePos.X-spriteSize/2, mousePos.Y-spriteSize);
        if(heldItem is not null){
            if(heldItem.type == "ice")
                spriteBatch.Draw(iceSprite.Texture, itemPos, iceSprite.TextureMapLocation, Color.White);
            else if(heldItem.type == "wood")
                spriteBatch.Draw(woodSprite.Texture, itemPos, woodSprite.TextureMapLocation, Color.White);
            else
                spriteBatch.Draw(matchSprite.Texture, itemPos, matchSprite.TextureMapLocation, Color.White);
        }

        /*if(bakedGoods < quota)
            spriteBatch.DrawString(font, "Baked Foods: " + bakedGoods +"/"+quota, new Vector2(gameXOrigin + 10, gameYOrigin + 10), Color.Red);
        else
            spriteBatch.DrawString(font, "Baked Foods: " + bakedGoods +"/"+quota, new Vector2(gameXOrigin + 10, gameYOrigin + 10), Color.Green);*/
        quotaBar.Draw(spriteBatch);
        //spriteBatch.DrawString(font, "Time Left: " + gameTimeLeft, new Vector2(gameXOrigin + 10, gameYOrigin + 30), Color.Black);
        timerBar.Draw(spriteBatch);
        
        /*if(currentTemp < idealTemp - 100)
            spriteBatch.DrawString(font, "Temp: " + currentTemp, new Vector2(gameXOrigin + 10, gameYOrigin + 50), Color.Blue);
        else if(currentTemp > idealTemp + 100)
            spriteBatch.DrawString(font, "Temp: " + currentTemp, new Vector2(gameXOrigin + 10, gameYOrigin + 50), Color.Red);
        else
            spriteBatch.DrawString(font, "Temp: " + currentTemp, new Vector2(gameXOrigin + 10, gameYOrigin + 50), Color.Green);*/
        tempBar.Draw(spriteBatch);

    }

    public static void Update(GameTime gameTime)
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
            if(isInside(mousePos, ovenPos, ovenSprite.TextureMapLocation.Width, ovenSprite.TextureMapLocation.Height) && currentMouse.LeftButton == ButtonState.Released){
                if(heldItem.type == "ice")
                    currentTemp -= 125;
                else if(heldItem.type == "wood")
                    currentTemp += 125;
                else
                    currentTemp += 175;
                heldItem = null;
            }else if(!isInside(mousePos, ovenPos, ovenSprite.TextureMapLocation.Width, ovenSprite.TextureMapLocation.Height) && currentMouse.LeftButton == ButtonState.Released){
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
            GameManager.inventory[3].Quantity += bakedGoods;
            GameManager.CurrentMinigameState = GameManager.MinigameState.Select;
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

    public static Boolean isInside(Point p1, Vector2 vec2, int xsize, int ysize){
        Rectangle obj1 = new Rectangle((int)vec2.X, (int)vec2.Y, xsize, ysize);
        return obj1.Contains(p1);
    }
}