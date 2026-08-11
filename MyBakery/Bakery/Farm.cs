


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

public class Farm : Scene
{
    private List<List<Plot>> plots;
    private SpriteFont _font;
    private Sprite _dirt;
    private Vector2 farmOrigin;
    private DayScene _dayScene;


    public Farm(DayScene dayScene) : base()
    {
        _dayScene = dayScene;
        
    }
    public override void Initialize()
    {
        farmOrigin = new Vector2(_dayScene.BakeryBounds.Left + 64, 64);

        plots = new List<List<Plot>>() { new List<Plot>() { new Plot(farmOrigin), new Plot(new Vector2(_dayScene.BakeryBounds.Left + 128, 64)), new Plot(new Vector2(_dayScene.BakeryBounds.Left + 192, 64))},
        new List<Plot>() { new Plot(new Vector2(_dayScene.BakeryBounds.Left + 64, 128)), new Plot(new Vector2(_dayScene.BakeryBounds.Left + 128, 128)), new Plot(new Vector2(_dayScene.BakeryBounds.Left + 192, 128))},
        new List<Plot>() { new Plot(new Vector2(_dayScene.BakeryBounds.Left + 64, 192)), new Plot(new Vector2(_dayScene.BakeryBounds.Left + 128, 192)), new Plot(new Vector2(_dayScene.BakeryBounds.Left + 192, 192))} };
        base.Initialize();
    }

    public override void LoadContent()
    {
        _dirt = _dayScene.Atlas.CreateSprite("Dirt");
    }


    public override void Update(GameTime gameTime)
    {
        Core.Input.Mouse.CheckMouse(); //Should this be in only 1 update method
        if (Core.Input.Mouse.CheckLeftPress())
        {
            if (Core.Input.Mouse.MouseLocation().X > farmOrigin.X && Core.Input.Mouse.MouseLocation().X < farmOrigin.X + plots[0].Count * 64 &&
            Core.Input.Mouse.MouseLocation().Y > farmOrigin.Y && Core.Input.Mouse.MouseLocation().Y < farmOrigin.Y + plots.Count * 64)
            {
                int clickedTileX = (int)(Core.Input.Mouse.MouseLocation().X - farmOrigin.X) / 64;
                int clickedTileY = (int)(Core.Input.Mouse.MouseLocation().Y - farmOrigin.Y) / 64;
                Console.WriteLine("planted", clickedTileY, clickedTileX);

                plots[clickedTileY][clickedTileX].crop = new Crop(1, 4, new int[5] {10, 10, 10, 10, 10}, new AnimatedSprite(_dayScene.Atlas.GetAnimation("Wheat")));
                plots[clickedTileY][clickedTileX].growthTime = 0;
                plots[clickedTileY][clickedTileX].currentStage = 0;
            }
        }
        foreach (List<Plot> row in plots)
        {
            foreach (Plot plot in row)
            {
                if (plot.crop != null)
                {
                    plot.crop.AnimatedSprite.Update(gameTime);
                    /*DEPRECATED
                    
                    plot.growthTime += gameTime.ElapsedGameTime.TotalSeconds;
                    if (plot.growthTime > plot.crop.stageTimes[plot.currentStage])
                    {
                        plot.currentStage += 1;
                        plot.growthTime = 0;
                    }
                    if (plot.currentStage > plot.crop.stages)
                    {
                        plot.crop = null;
                    }*/ 

                }
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch, SpriteFont font)
    {
        foreach (List<Plot> row in plots)
        {
            foreach (Plot plot in row)
            {
                _dirt.Draw(spriteBatch, plot.location);
                if (plot.crop != null)
                {
                    plot.crop.AnimatedSprite.Draw(spriteBatch, plot.location);
                }
            }
        }

    }

    private class Plot
    {
        public Crop crop { get; set; }
        public double growthTime { get; set; }
        public int currentStage { get; set; }
        public Vector2 location;
        //Maybe on later interations there are bonuses, fertility, etc.

        public Plot(Vector2 location)
        {
            this.location = location;
        }
    }

    private class Crop
    {
        public int stages;
        public int cost;
        public int[] stageTimes;
        public AnimatedSprite AnimatedSprite; // should have a value that correlates to inventory items

        public Crop(int cost, int stages, int[] stageTimes, AnimatedSprite animatedSprite)
        {
            this.cost = cost;
            this.stages = stages;
            this.stageTimes = stageTimes;
            AnimatedSprite = animatedSprite;
        }
    }
}