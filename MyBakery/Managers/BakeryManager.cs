

using System;
using System.Collections.Generic;
using GeneralUtil;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MyBakery;

public static class BakeryManager
{

    public static Shop shop;
    private static int gameXOrigin, gameYOrigin, dayTimeLeft, previousTime;
    private static double elapsedDayTime;
    const int dayLength = 120;
    private static Texture2D bakeryBG;
    public static Boolean IsOpen;
    private static BakeryDisplay display1;//temp test

    public static void Initialize(GraphicsDevice graphicsDevice, Texture2D Button, Texture2D display, Texture2D bakery,SpriteFont font)
    {
        gameXOrigin = GameManager.gameWidth/3;
        gameYOrigin = 0;
        UIButton startDayButton = new UIButton("Start Day", new GeneralUtil.Sprite(Button, new Rectangle(0, 0, 128, 64)), new Vector2(GameManager.gameWidth*2/3, GameManager.gameHeight/2));
        
        bakeryBG = bakery;
        IsOpen = false;
        previousTime = dayLength;
        elapsedDayTime = 0;
        display1 = new BakeryDisplay(new Sprite(display, new Rectangle(0, 0, 64, 64)), new Vector2(gameXOrigin+10, gameYOrigin+50), font, new Rectangle(0, 64, 64, 64)){
            Name = "dispaly1"
        };

        shop = new Shop(new Sprite(bakery, new Rectangle(0, 0, bakery.Width, bakery.Height)), startDayButton, new Sprite(display, new Rectangle(0, 0, 64, 64)), font){
            placedShopObjects = new List<ShopObject>(){display1}
        };

    }


    public static void Draw(SpriteFont font, SpriteBatch spriteBatch)
    {

        if(IsOpen){
           // spriteBatch.Draw(whiteBox, new Rectangle(gameXOrigin, 0, GameManager.gameWidth*2/3, GameManager.gameHeight/2), Color.Green);
            shop.Draw(spriteBatch, font);
            spriteBatch.DrawString(font, "Time Left in Day: " + dayTimeLeft/60 + " Minutes " + dayTimeLeft%60 + " Seconds", new Vector2(gameXOrigin + 10, 30), Color.White);
            
        }else{
            //spriteBatch.Draw(whiteBox, new Rectangle(gameXOrigin, gameYOrigin, GameManager.gameWidth/3*2, GameManager.gameHeight), Color.Beige);
            shop.Draw(spriteBatch, font);
        }

    }

    public static void Update(GameTime gameTime)
    {
        if(IsOpen){
            elapsedDayTime += gameTime.ElapsedGameTime.TotalMilliseconds;
            //Bakery
            dayTimeLeft = dayLength - (int)elapsedDayTime/1000;
            if(dayTimeLeft < 0){
                GameManager.CurrentBakeryState = GameManager.BakeryState.Menu;
                MinigameManager.CurrentMinigameState = MinigameManager.MinigameState.Menu;
                dayTimeLeft = dayLength;
                elapsedDayTime = 0;
            }
            foreach(BakeryDisplay display in shop.placedShopObjects){

                if(previousTime != dayTimeLeft){//remove when sell function is moved to customers
                    if(display.product != GameManager.Items.None){
                        if( GameManager.PlayerInfo.inventory[display.product] < 1){
                            display.product = GameManager.Items.None;
                            //display.Text = "Select\nProduct";
                        }else{
                            GameManager.ItemDB[display.product].Sell(display.product, 1);
                        }
                    }
                }
            }
            shop.Update(gameTime);
            previousTime = dayTimeLeft;
        }else{
            shop.Update(gameTime);
        }
    }
}