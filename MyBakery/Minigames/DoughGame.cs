using System;
using System.Collections.Generic;
using GeneralUtil;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyBakery;

public class DoughGame : Minigame
{

    const int spriteSize = 64;

    //DoughGame
    private int collectedDough, gameTimeLeft;
    private double timePassed;
    private Sprite doughSprite;
    private List<Rectangle> doughGrid;
    private Rectangle clickedDough, quota;
    public override void Start(Texture2D spriteSheet, Texture2D background)
    {

        collectedDough = 0;
        timePassed = 0;
        doughSprite = new Sprite(spriteSheet, new Rectangle(512, 512, spriteSize, spriteSize));
        doughGrid = new List<Rectangle>() { new Rectangle(gameXOrigin + 500, gameYOrigin + 200, spriteSize, spriteSize) };
        quota = new Rectangle(gameXOrigin + 400, gameYOrigin + 100, 300, 300);
    }


    public override void Draw(SpriteFont font, SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(doughSprite.Texture, quota, Color.White);//change to actual border
        foreach (Rectangle dough in doughGrid)
            spriteBatch.Draw(doughSprite.Texture, dough, doughSprite.TextureMapLocation, Color.White);

        spriteBatch.DrawString(font, "Time Left: " + gameTimeLeft, new Vector2(gameXOrigin + 10, gameYOrigin + 30), Color.White);
        
    }

    public override void Update(GameTime gameTime)
    {
        
        KMouse.CheckMouse();
        if (KMouse.CheckLeftPress())
        {
            foreach (Rectangle dough in doughGrid)
            {
                if (dough.Contains(KMouse.MouseLocation()))
                {
                    clickedDough = dough;
                    break;
                }
            }
        }
        if (KMouse.CheckLeftRelease())
        {
            if (IsAdjacentTile(KMouse.MouseLocation()) && clickedDough != Rectangle.Empty)
            {
                int newX = (int)(clickedDough.X + Math.Clamp(spriteSize * ((KMouse.MouseLocation().X - clickedDough.X - spriteSize / 2) / 200.0), -spriteSize, spriteSize));
                int newY = (int)(clickedDough.Y + Math.Clamp(spriteSize * ((KMouse.MouseLocation().Y - clickedDough.Y - spriteSize / 2) / 200.0), -spriteSize, spriteSize));
                doughGrid.Add(new Rectangle(newX, newY, spriteSize, spriteSize));
            }

        }

        timePassed += gameTime.ElapsedGameTime.TotalSeconds;
        gameTimeLeft = (int)(30 - timePassed);
        Rectangle totalRect = clickedDough;
        foreach (Rectangle rect in doughGrid)
        {
            totalRect = Rectangle.Union(totalRect, rect);
        }
        if (gameTimeLeft < 0 || totalRect.Contains(quota))
        {

            collectedDough = totalRect.Height * totalRect.Width / 2000 + gameTimeLeft * 4;
            gameTimeLeft = 0;

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
    private bool IsAdjacentTile(Point point)
    {
        foreach (Rectangle dough in doughGrid)
        {
            if (dough.Contains(point))
                return false;
        }
        return true;
    }

}
