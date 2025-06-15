using System;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using GeneralUtil;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MyBakery;

public class BakingGame : Minigame
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
    Texture2D bg;
    public override void Start(Texture2D spriteSheet, Texture2D background)
    {

        //BakingGame
        ovenSprite = new Sprite(spriteSheet, new Rectangle(192, 320, 256, 256));
        matchSprite = new Sprite(spriteSheet, new Rectangle(64, 320, 64, 128));
        woodSprite = new Sprite(spriteSheet, new Rectangle(128, 320, 64, 128));
        iceSprite = new Sprite(spriteSheet, new Rectangle(0, 320, 64, 128));
        matchBoxSprite = new Sprite(spriteSheet, new Rectangle(320, 192, 64, 64));
        ovenPos = new Vector2(gameXOrigin + 100, gameYOrigin + 100);
        pilePos = new Vector2(gameXOrigin + 400, gameYOrigin + 100);
        progressFront = new Sprite(spriteSheet, new Rectangle(512, 256, 128, 64));
        progressBack = new Sprite(spriteSheet, new Rectangle(192, 192, 128, 64));
        tempFront = new Sprite(spriteSheet, new Rectangle(480, 320, 32, 128));
        tempBack = new Sprite(spriteSheet, new Rectangle(448, 320, 32, 128));
        bg = background;

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


    public override void Draw(SpriteFont font, SpriteBatch spriteBatch)
    {
        //BakingGame
        spriteBatch.Draw(bg, new Rectangle(gameXOrigin, gameYOrigin, GameManager.gameWidth*2/3, GameManager.gameHeight/2), Color.White);

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

    public override void Update(GameTime gameTime)
    {

        timePassed += gameTime.ElapsedGameTime.TotalSeconds;
        gameTimeLeft = (int)(30 - timePassed);
        //BakingGame
        currentMouse = Mouse.GetState();
        mousePos = new Point(currentMouse.X, currentMouse.Y);
        if(MinigameManager.isInside(mousePos, pilePos, spriteSize, spriteSize) && currentMouse.LeftButton == ButtonState.Pressed && heldItem is null){
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
            if(MinigameManager.isInside(mousePos, ovenPos, ovenSprite.TextureMapLocation.Width, ovenSprite.TextureMapLocation.Height) && currentMouse.LeftButton == ButtonState.Released){
                if(heldItem.type == "ice")
                    currentTemp -= 125;
                else if(heldItem.type == "wood")
                    currentTemp += 125;
                else
                    currentTemp += 175;
                heldItem = null;
            }else if(!MinigameManager.isInside(mousePos, ovenPos, ovenSprite.TextureMapLocation.Width, ovenSprite.TextureMapLocation.Height) && currentMouse.LeftButton == ButtonState.Released){
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
            if (GameManager.PlayerInfo.inventory.ContainsKey(GameManager.Items.Cookie))
            {
                GameManager.PlayerInfo.inventory[GameManager.Items.Cookie] += bakedGoods;
            }
            else
            {
                GameManager.PlayerInfo.inventory[GameManager.Items.Cookie] = bakedGoods;
            }
            
            MinigameManager.CurrentMinigameState = MinigameManager.MinigameState.Select;
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
}