

using System;
using System.Collections.Generic;
using GeneralUtil;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using CoreLibrary;
using CoreLibrary.Graphics;

namespace MyBakery;

public static class BakeryManager
{

    public enum Tabs
    {
        Shop,
        Farm
    }

    public static Shop shop;
    public static Farm farm;
    private static int gameXOrigin, gameYOrigin, dayTimeLeft, previousTime;
    private static double elapsedDayTime;
    const int dayLength = 120;
    private static Texture2D bakeryBG;
    public static Boolean IsOpen;
    public static Tabs currentTab;
    private static TextureAtlas _spriteSheet;
    private static Sprite _button;

    public static void Initialize(Texture2D Button, TextureAtlas spriteSheet, Texture2D bakery, SpriteFont font)
    {
        gameXOrigin = GameManager.gameWidth / 3;
        gameYOrigin = 0;
        _spriteSheet = spriteSheet;
        _button = new Sprite(new TextureRegion(Button, 0, 0, 128, 64));

        UIButton startDayButton = new UIButton("Start Day", new Vector2(GameManager.gameWidth * 2 / 3, GameManager.gameHeight / 2), (int)_button.Width, (int)_button.Height,  () =>
        {
            GameManager.CurrentGameState = GameManager.GameState.Inventory;//CHANGE when later developed
            GameManager.CurrentBakeryState = GameManager.BakeryState.Day;
            MinigameManager.CurrentMinigameState = MinigameManager.MinigameState.Select;
        });

        bakeryBG = bakery;
        IsOpen = false;
        previousTime = dayLength;
        elapsedDayTime = 0;
        currentTab = Tabs.Shop;
        //currentTab = Tabs.Farm;

        shop = new Shop(new Sprite(new TextureRegion(bakery, 0, 0, bakery.Width, bakery.Height)), startDayButton, spriteSheet, font);
        farm = new Farm(spriteSheet, font);

    }


    public static void Draw(SpriteFont font, SpriteBatch spriteBatch)
    {

        if (IsOpen)
        {
            // spriteBatch.Draw(whiteBox, new Rectangle(gameXOrigin, 0, GameManager.gameWidth*2/3, GameManager.gameHeight/2), Color.Green);
            spriteBatch.DrawString(font, "Time Left in Day: " + dayTimeLeft / 60 + " Minutes " + dayTimeLeft % 60 + " Seconds", new Vector2(gameXOrigin + 10, gameYOrigin + 30), Color.White);

        }
        if(currentTab == Tabs.Shop)
            shop.Draw(spriteBatch, font);
        if(currentTab == Tabs.Farm)
            farm.Draw(spriteBatch, font);

    }

    public static void Update(GameTime gameTime)
    {
        if (IsOpen)
        {
            elapsedDayTime += gameTime.ElapsedGameTime.TotalMilliseconds;
            //Bakery
            dayTimeLeft = dayLength - (int)elapsedDayTime / 1000;
            if (dayTimeLeft < 0)
            {
                GameManager.CurrentBakeryState = GameManager.BakeryState.Menu;
                MinigameManager.CurrentMinigameState = MinigameManager.MinigameState.Menu;
                dayTimeLeft = dayLength;
                elapsedDayTime = 0;
            }
            foreach (ShopObject sobject in shop.placedShopObjects)
            {

                if (previousTime != dayTimeLeft)
                {//remove when sell function is moved to customers
                    if (sobject.Type == "Display")
                    {
                        BakeryDisplay display = sobject as BakeryDisplay;
                        if (display.product != "None")
                        {
                            if (GameManager.PlayerInfo.inventory[display.product] < 1)
                            {
                                display.product = "None";
                                //display.Text = "Select\nProduct";
                            }
                            else
                            {
                                GameManager.ItemDB[display.product].Sell(display.product, 1);
                            }
                        }
                    }
                }
            }
            shop.Update(gameTime);
            previousTime = dayTimeLeft;
        }
        else
        {
            shop.Update(gameTime);
            farm.Update(gameTime);
        }
    }
}