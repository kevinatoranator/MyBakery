using System;
using System.Collections.Generic;
using GeneralUtil;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MyBakery;

public class DoughGame : Minigame
{

    const int spriteSize = 64;

    //DoughGame
    private int collectedDough, gameTimeLeft, quota;
    private double timePassed;
    public override void Start(Texture2D spriteSheet, Texture2D background)
    {

        collectedDough = 0;
        timePassed = 0;
        quota = 20; //Make dynamic based on... average?
    }


    public override void Draw(SpriteFont font, SpriteBatch spriteBatch)
    {
        if(collectedDough < quota)
            spriteBatch.DrawString(font, "Dough collected: " + collectedDough +"/"+quota, new Vector2(gameXOrigin + 10, gameYOrigin + 10), Color.Red);
        else
            spriteBatch.DrawString(font, "Dough collected: " + collectedDough +"/"+quota, new Vector2(gameXOrigin + 10, gameYOrigin + 10), Color.Green);
        spriteBatch.DrawString(font, "Time Left: " + gameTimeLeft, new Vector2(gameXOrigin + 10, gameYOrigin + 30), Color.Black);

    }

    public override void Update(GameTime gameTime)
    {
       
        timePassed += gameTime.ElapsedGameTime.TotalSeconds;
        gameTimeLeft = (int)(30 - timePassed);
        if(gameTimeLeft < 0){
            gameTimeLeft = 0;
            if(collectedDough > quota)
                collectedDough += collectedDough/2;
            
            if (GameManager.PlayerInfo.inventory.ContainsKey(GameManager.Items.Dough))
            {
                GameManager.PlayerInfo.inventory[GameManager.Items.Dough] += collectedDough; 
            }
            else
            {
                GameManager.PlayerInfo.inventory[GameManager.Items.Dough] = collectedDough;
            }
            MinigameManager.CurrentMinigameState = MinigameManager.MinigameState.Select;
        }
    }


}
