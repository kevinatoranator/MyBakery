using System;
using System.Collections.Generic;
using GeneralUtil;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CoreLibrary.Graphics;
using CoreLibrary.Input;
using CoreLibrary;
using CoreLibrary.Scenes;
using MyBakery.Scenes;

namespace MyBakery;

public class DoughGame : Scene
{

    const int spriteSize = 64;
    private SpriteFont _font;
    //DoughGame
    private int collectedDough, gameTimeLeft;
    private double timePassed;
    private Sprite doughSprite;
    private List<Rectangle> doughGrid;
    private Rectangle clickedDough, quota;
    private DayScene _mainScene;
    public DoughGame(DayScene main) : base()
    {
        _mainScene = main;
    }
    public override void Initialize()
    {

        collectedDough = 0;
        timePassed = 0;
        
        doughGrid = new List<Rectangle>() { new Rectangle(_mainScene.GameBounds.X + 500, _mainScene.GameBounds.Y + 200, spriteSize, spriteSize) };
        quota = new Rectangle(_mainScene.GameBounds.X + 400, _mainScene.GameBounds.Y + 100, 300, 300);
        base.Initialize();
    }

    public override void LoadContent()
    {
        doughSprite = _mainScene.Atlas.CreateSprite("Dough");
        _font = Content.Load<SpriteFont>("font");
    }

    public override void Draw(GameTime gameTime)
    {
        //Core.SpriteBatch.Draw(doughSprite.Region.Texture, quota, Color.White);//change to actual border
        foreach (Rectangle dough in doughGrid)
            doughSprite.Draw(Core.SpriteBatch, new Vector2(dough.X, dough.Y));

        Core.SpriteBatch.DrawString(_font, "Time Left: " + gameTimeLeft, new Vector2(_mainScene.GameBounds.X + 10, _mainScene.GameBounds.Y + 30), Color.White);
        
    }

    public override void Update(GameTime gameTime)
    {
        if (Core.Input.Mouse.CheckLeftPress())
        {
            foreach (Rectangle dough in doughGrid)
            {
                if (dough.Contains(Core.Input.Mouse.MouseLocation()))
                {
                    clickedDough = dough;
                    break;
                }
            }
        }
        if (Core.Input.Mouse.CheckLeftRelease())
        {
            if (IsAdjacentTile(Core.Input.Mouse.MouseLocation()) && clickedDough != Rectangle.Empty)
            {
                int newX = (int)(clickedDough.X + Math.Clamp(spriteSize * ((Core.Input.Mouse.MouseLocation().X - clickedDough.X - spriteSize / 2) / 200.0), -spriteSize, spriteSize));
                int newY = (int)(clickedDough.Y + Math.Clamp(spriteSize * ((Core.Input.Mouse.MouseLocation().Y - clickedDough.Y - spriteSize / 2) / 200.0), -spriteSize, spriteSize));
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

            if (GameManager.PlayerInfo.inventory.ContainsKey("Dough"))
            {
                GameManager.PlayerInfo.inventory["Dough"] += collectedDough;
            }
            else
            {
                GameManager.PlayerInfo.inventory["Dough"] = collectedDough;
            }
            _mainScene.ChangeLowerTab(new SelectionScene(_mainScene));
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
