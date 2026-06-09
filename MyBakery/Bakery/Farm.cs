


using System;
using System.Collections.Generic;
using GeneralUtil;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CoreLibrary.Graphics;

namespace MyBakery;

public class Farm
{
    private List<List<Plot>> plots;
    private TextureAtlas spriteSheet;
    SpriteFont font;
    private Sprite _dirt;
    private Vector2 farmOrigin;
    private Crop wheat;


    public Farm(TextureAtlas spriteSheet, SpriteFont font)
    {

        this.spriteSheet = spriteSheet;
        this.font = font;
        _dirt = spriteSheet.CreateSprite("Dirt");
        wheat = new Crop(1, 4, new int[5] {10, 10, 10, 10, 10}, new Vector2(0, 576));

        farmOrigin = new Vector2(GameManager.gameWidth / 3 + 64, 64);

        plots = new List<List<Plot>>() { new List<Plot>() { new Plot(farmOrigin), new Plot(new Vector2(GameManager.gameWidth / 3 + 128, 64)), new Plot(new Vector2(GameManager.gameWidth / 3 + 192, 64))},
        new List<Plot>() { new Plot(new Vector2(GameManager.gameWidth / 3 + 64, 128)), new Plot(new Vector2(GameManager.gameWidth / 3 + 128, 128)), new Plot(new Vector2(GameManager.gameWidth / 3 + 192, 128))},
        new List<Plot>() { new Plot(new Vector2(GameManager.gameWidth / 3 + 64, 192)), new Plot(new Vector2(GameManager.gameWidth / 3 + 128, 192)), new Plot(new Vector2(GameManager.gameWidth / 3 + 192, 192))} };
    }


    public void Update(GameTime gameTime)
    {
        KMouse.CheckMouse(); //Should this be in only 1 update method
        if (KMouse.CheckLeftPress())
        {
            if (KMouse.MouseLocation().X > farmOrigin.X && KMouse.MouseLocation().X < farmOrigin.X + plots[0].Count * 64 &&
            KMouse.MouseLocation().Y > farmOrigin.Y && KMouse.MouseLocation().Y < farmOrigin.Y + plots.Count * 64)
            {
                int clickedTileX = (int)(KMouse.MouseLocation().X - farmOrigin.X) / 64;
                int clickedTileY = (int)(KMouse.MouseLocation().Y - farmOrigin.Y) / 64;
                Console.WriteLine("planted", clickedTileY, clickedTileX);

                plots[clickedTileY][clickedTileX].crop = wheat;
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
                    plot.growthTime += gameTime.ElapsedGameTime.TotalSeconds;
                    if (plot.growthTime > plot.crop.stageTimes[plot.currentStage])
                    {
                        plot.currentStage += 1;
                        plot.growthTime = 0;
                    }
                    if (plot.currentStage > plot.crop.stages)
                    {
                        plot.crop = null;
                    }

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
                    spriteBatch.Draw(spriteSheet.Texture, plot.location, new Rectangle((int)(plot.crop.animationStart.X + 64 * plot.currentStage), (int)plot.crop.animationStart.Y, 64, 64), Color.White);
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
        public Vector2 animationStart; // should have a value that correlates to inventory items

        public Crop(int cost, int stages, int[] stageTimes, Vector2 animationStart)
        {
            this.cost = cost;
            this.stages = stages;
            this.stageTimes = stageTimes;
            this.animationStart = animationStart;
        }
    }
}